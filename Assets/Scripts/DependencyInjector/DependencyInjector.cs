using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Unity.VisualScripting;

namespace SDI
{
    public class DependencyInjector : IDependencyInjector
    {
        private Dictionary<Type, object> _dependencies = new();
        private readonly IPortableDI _portableDI;

        public DependencyInjector()
        {
            _portableDI = new PortableDI();
            _portableDI.SetPrimaryInjector(this);
        }

        public void Register(params object[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Register<object>(dependency);
            }
        }

        public void Register<T>(T dependency)
        {
            Type dependencyType = dependency.GetType();
            if (_dependencies.ContainsKey(dependencyType))
            {
                CSDL.LogWarning($"Dependency of type {nameof(dependencyType)} already provided.");
                return;
            }
            _dependencies.Add(dependencyType, dependency);
        }

        public void Inject(params object[] targets)
        {
            foreach (var target in targets)
            {
                Inject<object>(target);
            }
        }

        public void Inject<T>(T target)
        {
            Type callerType = target.GetType();
            MethodInfo[] methods = callerType.GetMethods();
            if (GetInjectionMethod(methods, out MethodInfo inject))
            {
                if (!TryToInvokeInjectionMethod(target, inject))
                {
                    throw new MissingReferenceException($"Dependency injector can't resolve dependecies for " +
                        $"{target} of type {typeof(T)}, possibly, " +
                        $"because these dependencies is not provided.");
                }
            }
            else
            {
                throw new InvalidOperationException($"{target} doesn't contains a injection method with" +
                    $"{nameof(InjectionMethodAttribute)} atrribute.");
            }
        }

        private bool TryToInvokeInjectionMethod<T>(T caller, MethodInfo method)
        {
            ParameterInfo[] definedParams = method.GetParameters();
            object[] providedArgs = new object[definedParams.Length];
            for (int i = 0; i < definedParams.Length; i++)
            {
                Type paramType = definedParams[i].ParameterType;
                if (TryToFindDependency(paramType, out object value))
                {
                    providedArgs[i] = value;
                }
                else
                {
                    return false;
                }
            }
            method.Invoke(caller, providedArgs);
            return true;
        }

        private bool TryToFindDependency(Type typeToSearch, out object dependency)
        {
            if (_dependencies.TryGetValue(typeToSearch, out dependency))
            {
                return true;
            }
            foreach (Type depType in _dependencies.Keys)
            {
                if (depType.GetInterfaces().Contains(typeToSearch))
                {
                    dependency = _dependencies[depType];
                    _dependencies.Add(typeToSearch, dependency);
                    return true;
                }
            }
            return false;
        }

        private bool GetInjectionMethod(MethodInfo[] methods, out MethodInfo method)
        {
            method = null;
            foreach (MethodInfo m in methods)
            {
                if (m.GetCustomAttribute<InjectionMethodAttribute>() != null)
                {
                    method = m;
                    break;
                }
            }
            return method != null;
        }
    }
}