namespace Bambini.Services.DependencyResolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class DependencyResolver
    {
        private readonly Dictionary<Type, Type> regs = new();
        private readonly Dictionary<Type, object> resolved = new();

        public void Add<TClass>()
            where TClass : class
        {
            regs.Add(typeof(TClass), typeof(TClass));
        }

        public void Add<TInterface, TClass>()
            where TClass : TInterface
        {
            regs.Add(typeof(TInterface), typeof(TClass));
        }

        public T Get<T>()
        {
            if (resolved.ContainsKey(typeof(T)))
            {
                return (T)resolved[typeof(T)];
            }
            else if (!regs.ContainsKey(typeof(T)) || regs[typeof(T)] == null)
            {
                throw new InvalidDataException($"Couldn't find a class to resolve '{typeof(T).FullName}'");
            }
            var instanceOfTheClass = CreateInstance<T>();
            resolved[typeof(T)] = instanceOfTheClass;
            return instanceOfTheClass;
        }

        private T CreateInstance<T>()
        {
            var classType = regs[typeof(T)];
            var classTypeName = classType.FullName;

            if (classType.IsAbstract)
            {
                throw new ArgumentException("Excepted interface but got class");
            }

            var constuctors = classType.GetConstructors();

            if (constuctors.Length > 1)
            {
                throw new ArgumentException($"Found more than one constuctor for '{classTypeName}' type");
            }

            var constuctor = constuctors.FirstOrDefault();

            if (constuctor == null)
            {
                throw new ArgumentNullException($"Excepted at least one constuctor for '{classTypeName}' type");
            }

            var parameters = constuctor.GetParameters();

            var resolvedParameters = new List<object>();

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;

                MethodInfo method = GetType().GetMethod(nameof(Get))
                             .MakeGenericMethod(new Type[] { parameterType });

                var resolvedParameter = method.Invoke(this, Array.Empty<object>());
                resolvedParameters.Add(resolvedParameter);
            }

            return (T)constuctor.Invoke(resolvedParameters.ToArray());
        }
    }
}