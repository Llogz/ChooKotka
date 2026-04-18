### ЭТА ХУЙНЯ СГЕНЕРИРОВАНА С CHATGPT

# Документация проекта

## Структура проекта

Проект разделен на две части:

* **Core** – базовая логика и сервисы, не зависящие от Game.
* **Game** – игровая часть, зависит от Core, но Core не зависит от Game.

> **Важно:** Core не должен ссылаться на Game. Все сервисы и утилиты, которые могут использоваться в обеих частях, находятся в Core.

---

## Сервисы

Все взаимодействие с VContainer происходит через основной компонент **Root** и его дочерние компоненты.

Сервисы делятся на два типа:

1. **Сервисы на сцене** – наследники `SceneService` (MonoBehaviour), которые сами регистрируют себя. Root ищет их на сцене.
2. **Сервисы глобального уровня** – регистрируются прямо в Root и не привязаны к сцене.

### Получение доступа к сервисам

```csharp
// Через метод
IMyService _service;
[Inject] private void Init(IMyService service) 
{
    _service = service;
}

// Напрямую через инъекцию
[Inject] private IMyService service;
```

> Сервисы лучше использовать через интерфейсы, чтобы легко менять реализации.

---

## Создание своих сервисов

Для сервисов Game создайте собственный Root, наследуя его от Core `Root`:

```csharp
public class GameRoot : Root
{
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.Register<MyService>().As<IMyService>(); // Регистрируем через интерфейс
    }
}
```

Для сервисов на сцене создайте класс-наследник `SceneService`:

```csharp
public class MySceneService : SceneService
{
    public override void Configure(IContainerBuilder builder)
    {
        builder.Register<MySceneService>().As<IMySceneService>();
    }
}
```

---

## Работа с Lifetime

`Lifetime` — это объект для управления временем жизни задач и сервисов в проекте.

**Основные принципы:**

* Глобальный `Lifetime` создается в `Root` и живет столько же, сколько сцена.
* Дочерние `Lifetime` создаются через `Child()` и наследуют отмену от родителя.
* Все асинхронные задачи (`UniTask`) должны использовать `CancellationToken` из `Lifetime`.

### Пример использования Lifetime с UniTask

```csharp
public class ExampleService
{
    private readonly ILifetime _lifetime;

    public ExampleService(ILifetime lifetime)
    {
        _lifetime = lifetime;
        RunAsync(_lifetime.GetToken()).Forget();
    }

    private async UniTaskVoid RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            Debug.Log("Task running...");
        }
    }
}
```

> При выгрузке сцены глобальный `Lifetime` Dispose-ится, и все дочерние задачи автоматически отменяются.

---

## FabricService — создание объектов

`FabricService` используется для создания объектов на сцене с поддержкой Object Pooling.

### Пример создания объекта со своей датой

```csharp
// Данные для нового объекта
public class MyObjectData : FabricObjectData
{
    public int Health { get; }

    public MyObjectData(Vector3 position, Quaternion rotation, int health)
        : base(UpdateType.Update, position, rotation)
    {
        Health = health;
    }
}

// Объект с использованием данных
public class MyObject : FabricObject<MyObjectData>
{
    protected override void OnInitialize(MyObjectData data)
    {
        Debug.Log($"Object created at {data.Position} with health {data.Health}");
    }
}

// Создание объекта через FabricService
public void SpawnObject(IFabricService fabric, GameObject prefab)
{
    var data = new MyObjectData(new Vector3(0,0,0), Quaternion.identity, 100);
    var instance = fabric.Create<MyObject, MyObjectData>(data, prefab, usePool: true);
}
```

> Использование FabricService позволяет создавать объекты с пуллингом и автоматической инъекцией зависимостей.

---

## Полезные советы

1. Всегда регистрируйте сервисы через интерфейсы.
2. Для объектов на сцене используйте `SceneService` для автоматической регистрации.
3. Используйте `Lifetime` для всех асинхронных задач.
4. Для объектов с повторным использованием применяйте `IFabricService` с пуллингом.
5. Не используйте ссылки на Game внутри Core.

---

## Пример объединения сервисов и объектов

```csharp
public class GameManager
{
    [Inject] private ILifetime _lifetime;
    [Inject] private IFabricService _fabric;
    [Inject] private IAudioService _audio;

    public void Initialize()
    {
        // Создаем объект
        var data = new MyObjectData(Vector3.zero, Quaternion.identity, 50);
        var prefab = Resources.Load<GameObject>("MyPrefab");
        var obj = _fabric.Create<MyObject, MyObjectData>(data, prefab, usePool: true);

        // Запускаем задачу с Lifetime
        RunPeriodicTask();
    }

    private async UniTaskVoid RunPeriodicTask()
    {
        var token = _lifetime.GetToken();
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(2000, cancellationToken: token);
            _audio.PlaySound("Tick");
        }
    }
}
```

> Такой подход объединяет управление временем жизни задач, создание объектов и доступ к сервиса
