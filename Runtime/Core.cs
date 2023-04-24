#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
# endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmoyFeels.ProjectInitialization
{

    public static class Core
    {
        private static List<ICoreService> _services;
        private static Dictionary<Type, ICoreService> _cacheServices;
#if UNITASK_SUPPORT
        private static List<Func<UniTask>> _beforeInitializeTasks;
        private static List<Func<UniTask>> _afterInitializeTasks;
#endif
        private static List<Func<System.Action>> _beforeInitializeActions;
        private static List<Func<System.Action>> _afterInitializeActions;
        const string _coreConfigPath = "AmoyFeels/CoreConfig";

        [RuntimeInitializeOnLoadMethod()]
        private static void Init()
        {
            var data = Resources.Load<CoreConfig>(_coreConfigPath);
            if (!data.InitializeOnAwake)
                return;
#if UNITASK_SUPPORT
            InitializeAsync().Forget();
#else
            Initialize();
#endif
        }
        public static void Initialize()
        {
            IOrderedEnumerable<Type> types = OrderExecution();
            _services = new();
            _cacheServices = new();
            _beforeInitializeActions = new();
            _afterInitializeActions = new();
            foreach (var item in types)
            {
                var instance = Activator.CreateInstance(item) as ICoreService;
                _services.Add(instance);
            }

            foreach (var preTask in _beforeInitializeActions)
            {
                preTask();
            }

            foreach (var manager in _services)
            {
                manager.Initialize();
            }

            foreach (var postTask in _afterInitializeActions)
            {
                postTask();
            }
        }

#if UNITASK_SUPPORT

        public static async UniTask InitializeAsync()
        {
            IOrderedEnumerable<Type> types = OrderExecution();
            _services = new();
            _cacheServices = new();
            _beforeInitializeTasks = new();
            _afterInitializeTasks = new();
            foreach (var item in types)
            {
                var instance = Activator.CreateInstance(item) as ICoreService;
                _services.Add(instance);
            }

            foreach (var preTask in _beforeInitializeTasks)
            {
                await preTask();
            }

            foreach (var manager in _services)
            {
                await manager.InitializeAsync();
            }

            foreach (var postTask in _afterInitializeTasks)
            {
                await postTask();
            }

        }
#endif
        private static IOrderedEnumerable<Type> OrderExecution()
        {
            var type = typeof(ICoreService);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass).ToList();

            for (int i = 0; i < types.Count; i++)
            {
                if (GetAttribute(types[i]) == null)
                {
                    Debug.LogError($"{types[i].FullName} class that derived from {nameof(ICoreService)} must have attribute {nameof(InitializeAtRuntimeAttribute)}");
                    return null;
                }
            }

            var services = types.Where(x => GetAttribute(x) != null).OrderBy(x => GetAttribute(x).InitializationPriority);

            return services;
        }

        private static InitializeAtRuntimeAttribute GetAttribute(Type item)
        {
            return Attribute.GetCustomAttribute(item, typeof(InitializeAtRuntimeAttribute), false) as InitializeAtRuntimeAttribute;
        }
        /// <summary>
        /// Generic to get a manager class that have interface <see cref="ICoreService"/>
        /// </summary>
        public static T Get<T>() where T : class, ICoreService
        {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Get a manager class that have interface <see cref="ICoreService"/>
        /// </summary>
        public static ICoreService Get(Type type)
        {
            if (_cacheServices.TryGetValue(type, out var cachedResult))
                return cachedResult;
            var result = _services.FirstOrDefault(type.IsInstanceOfType);
            if (result is null)
                return null;
            _cacheServices[type] = result;
            return result;
        }
#if UNITASK_SUPPORT
        /// <summary>
        /// Add a callback function after initialized
        /// </summary>
        public static void AddAfterInitializeTask(Func<UniTask> func) => _afterInitializeTasks.Add(func);
        /// <summary>
        /// Remove a callback function after initialized
        /// </summary>
        public static void RemoveAfterInitializeTask(Func<UniTask> func) => _afterInitializeTasks.Add(func);
        /// <summary>
        /// Add a callback function before initialized
        /// </summary>
        public static void AddBeforeInitializeTask(Func<UniTask> func) => _beforeInitializeTasks.Add(func);
        /// <summary>
        /// Remove a callback function before initialized
        /// </summary>
        public static void RemoveBeforeInitializeTask(Func<UniTask> func) => _beforeInitializeTasks.Add(func);
#else
        /// <summary>
        /// Add a callback function after initialized
        /// </summary>
        public static void AddAfterInitializeTask(Func<Action> func) => _afterInitializeActions.Add(func);
        /// <summary>
        /// Remove a callback function after initialized
        /// </summary>
        public static void RemoveAfterInitializeTask(Func<Action> func) => _afterInitializeActions.Remove(func);
        /// <summary>
        /// Add a callback function before initialized
        /// </summary>
        public static void AddBeforeInitializeTask(Func<Action> func) => _beforeInitializeActions.Add(func);
        /// <summary>
        /// Remove a callback function before initialized
        /// </summary>
        public static void RemoveBeforeInitializeTask(Func<Action> func) => _beforeInitializeActions.Remove(func);

#endif
    }

}