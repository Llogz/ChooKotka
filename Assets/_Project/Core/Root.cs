using System;
using System.Linq;
using System.Reflection;
using Core.Services;
using Core.Services.SavingSystem;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class Root : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Lifetime>(VContainer.Lifetime.Singleton).As<ILifetime>(); // 1 lifetime per scene
            
            builder.Register<FabricService>(VContainer.Lifetime.Singleton).As<IFabricService>();
            builder.Register<SecureSaveSystem>(VContainer.Lifetime.Singleton).As<ISecureSaveSystem>();
            builder.Register<GameDataProvider>(VContainer.Lifetime.Singleton).As<IDataSubject>();
            builder.Register<InputControllerService>(VContainer.Lifetime.Singleton).As<IInputController>();
            builder.Register<SettingsSaveService>(VContainer.Lifetime.Singleton).As<ISettingsSaveService>();

            foreach (var obj in
                     FindObjectsByType<SceneService>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    ) obj.Configure(builder);
        }
        
        [MenuItem("Tools/Fill Services %e")] // %e = Ctrl+E
        public static void FillServices()
        {
            var roots = FindObjectsByType<Root>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (roots.Length > 1)
            {
                Debug.LogWarning("There is more than one root in the scene");
                return;
            }
            if (roots.Length == 0)
            {
                Debug.LogError("No root in scene");
                return;
            }

            var root = roots[0];

            var allBehaviours = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include, FindObjectsSortMode.None
            );

            var injectTargets = allBehaviours
                .Where(mb => mb != null && HasInject(mb.GetType()))
                .Select(mb => mb.gameObject)
                .Distinct()
                .ToList();

            injectTargets = injectTargets
                .Where(go => !injectTargets.Any(other =>
                    other != go && go.transform.IsChildOf(other.transform)))
                .ToList();

            var so = new SerializedObject(root);
            var prop = so.FindProperty("autoInjectGameObjects");

            prop.arraySize = injectTargets.Count;

            for (int i = 0; i < injectTargets.Count; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = injectTargets[i].gameObject;
            }

            so.ApplyModifiedProperties();

            Debug.Log($"[Root] Auto-inject objects filled: {injectTargets.Count}");
        }
        
        public static bool HasInject(Type type)
        {
            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            if (type.GetFields(flags).Any(f => f.GetCustomAttribute<InjectAttribute>() != null))
                return true;

            if (type.GetProperties(flags).Any(p => p.GetCustomAttribute<InjectAttribute>() != null))
                return true;

            if (type.GetMethods(flags).Any(m => m.GetCustomAttribute<InjectAttribute>() != null))
                return true;

            return false;
        }
    }
}
