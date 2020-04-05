using Neurox.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Neurox.Core.Utilites
{
    public static class IoCFactory
    {
        private static Dictionary<Type, object> _registeredDependencies;
        private static void ContainerWarmUp()
        {
            _registeredDependencies = new Dictionary<Type, object>();
            _registeredDependencies.Add(typeof(IConfigurationProvider), new ConfigurationProvider());
            _registeredDependencies.Add(typeof(ILogger), new DirectFileLogger());
        }

        public static TInterface Resolve<TInterface>()
        {
            if (_registeredDependencies == null)
            {
                ContainerWarmUp();
            }
            TInterface instance = (TInterface)_registeredDependencies[typeof(TInterface)];
            if (instance != null)
            {
                return instance;
            }
            throw new TypeUnloadedException($"Could not resolve the dependency of type {typeof(TInterface).FullName}");
        }

    }
}
