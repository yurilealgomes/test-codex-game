using System;
using System.Collections.Generic;

namespace ArcaneSurvival
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void Clear()
        {
            Services.Clear();
        }

        public static void Register<T>(T service) where T : class
        {
            Services[typeof(T)] = service;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            object value;
            if (Services.TryGetValue(typeof(T), out value))
            {
                service = value as T;
                return service != null;
            }

            service = null;
            return false;
        }

        public static T Get<T>() where T : class
        {
            T service;
            if (TryGet(out service))
            {
                return service;
            }

            throw new InvalidOperationException("Service not registered: " + typeof(T).Name);
        }
    }
}
