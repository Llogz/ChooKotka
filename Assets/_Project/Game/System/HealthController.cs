using System;
using UnityEngine;

/// <summary>
/// Отвечает за состояние здоровья и реакцию на его изменение.
/// Важно: изменение HP всегда должно происходить только через публичные методы.
/// Это защищает инварианты и предотвращает обход доменной логики.
/// </summary>
public class HealthController : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private int hpSet = 100;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private bool saveHP = false;
    [SerializeField] private string saveHPKey;
    [SerializeField] private bool destroyAfterDying = true;
    [SerializeField] private GameObject objectToDestroy;

    [Header("Spawn Objects")]
    [SerializeField] private GameObject[] spawnAfterDamage;
    [SerializeField] private GameObject[] spawnAfterDestroy;

    [Header("Animations")]
    [SerializeField] private string getDamageAnim;
    [SerializeField] private float getDamageAnimTime;

    private int _hp;

    /// <summary>
    /// Текущее здоровье. Только чтение.
    /// Изменять напрямую нельзя — только через методы.
    /// </summary>
    public int Health => _hp;

    public int MaxHP => maxHP;

    public Action OnDie { get; set; }
    public Action<int> OnHPChanged { get; set; }

    private void Awake()
    {
        if (objectToDestroy == null)
            objectToDestroy = gameObject;
    }

    private void Start()
    {
        SetHealth(hpSet);
    }

    #region Public API

    /// <summary>
    /// Нанесение урона.
    /// amount должен быть положительным.
    /// </summary>
    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Damage amount must be positive.");

        ChangeHealth(-amount);
        SpawnDamageEffects();
        PlayDamageAnimation();
    }

    /// <summary>
    /// Восстановление здоровья.
    /// amount должен быть положительным.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Heal amount must be positive.");

        ChangeHealth(amount);
    }
    
    /// <summary>
    /// Принудительное убийство объекта.
    /// Игнорирует текущий уровень здоровья.
    /// </summary>
    public void Kill()
    {
        if (_hp <= 0)
            return;

        _hp = 0;

        OnHPChanged?.Invoke(_hp);
        Die();
    }

    /// <summary>
    /// Прямое задание здоровья.
    /// Использовать осторожно — подходит для загрузки сохранений
    /// или инициализации.
    /// </summary>
    public void SetHealth(int value)
    {
        int clamped = Mathf.Clamp(value, 0, MaxHP);
        ChangeHealth(clamped - _hp);
    }

    #endregion

    #region Internal Logic

    /// <summary>
    /// Единственная точка изменения HP.
    /// Защищает инварианты и централизует логику.
    /// </summary>
    private void ChangeHealth(int delta)
    {
        if (delta == 0)
            return;

        int newHP = Mathf.Clamp(_hp + delta, 0, MaxHP);

        if (newHP == _hp)
            return;

        _hp = newHP;

        OnHPChanged?.Invoke(_hp);
        Debug.Log(gameObject.name + " hp changed to " + newHP);

        if (_hp <= 0)
            Die();
    }

    private void SpawnDamageEffects()
    {
        foreach (var obj in spawnAfterDamage)
            Instantiate(obj, transform.position, transform.rotation);
    }

    private void PlayDamageAnimation()
    {
        // Здесь должна быть интеграция с анимационной системой.
        // Важно: не смешивать анимацию и логику HP в одном методе.
    }

    private void Die()
    {
        OnDie?.Invoke();
        enabled = false;

        foreach (var obj in spawnAfterDestroy)
            Instantiate(obj, transform.position, transform.rotation);

        if (destroyAfterDying)
            Destroy(objectToDestroy);
    }

    #endregion
}
