namespace sticky_notes_wp8
{
    using System;
    using System.Collections.Generic;

    public static class Locator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void Register<T>(object service)
        {
            Services.Add(typeof(T), service);
        }

        public static T Instance<T>()
        {
            if (Services.ContainsKey(typeof(T)))
            {
                return (T)Services[typeof(T)];
            }

            throw new Exception(string.Format("Instance of {0} not in ServiceLocator", typeof(T)));
        }

        public static void RemoveAllInstances()
        {
            Services.Clear();
        }
    }
}
