namespace Bambini.Services.DependencyResolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DependencyResolver
    {
        private readonly Dictionary<Type, Type> regs = new();
        private readonly Dictionary<Type, object> resolved = new();

        /// <summary>
        /// Adds the given class to a dictionary and when a command needs it
        /// it will automatically pass it if needed. You should call this method before running the
        /// application
        /// </summary>
        /// <typeparam name="TClass">A class to add</typeparam>
        public void Add<TClass>()
            where TClass : class
        {
            if (regs.ContainsKey(typeof(TClass))) return;
            regs.Add(typeof(TClass), typeof(TClass));
        }

        /// <summary>
        /// Takes an interface and class (what implementation should add to the given interface).
        /// When the interface is met it will pass the class. You should call this method before running the
        /// application
        /// </summary>
        /// <typeparam name="TInterface">Interface</typeparam>
        /// <typeparam name="TClass">The class</typeparam>
        public void Add<TInterface, TClass>()
            where TClass : class, TInterface
        {
            if (regs.ContainsKey(typeof(TInterface))) return;
            regs.Add(typeof(TInterface), typeof(TClass));
        }

        internal T Get<T>()
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
                throw new ArgumentException("Excepted class to be instantiable but got abstract");
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

                if (regs.ContainsKey(parameterType))
                {
                    var currentParameterClass = regs[parameterType];

                    var constructor = currentParameterClass.GetConstructors().FirstOrDefault();
                    
                    if(constructor != null)
                    {
                        var isThereACircularDependency = constructor.GetParameters().Any(x => x.ParameterType == typeof(T));
                        if (isThereACircularDependency)
                        {
                            throw new StackOverflowException($"Circular dependency caught between: {currentParameterClass.FullName} and {classType.FullName}");
                        }
                    }
                }

                MethodInfo method = this.GetType().GetMethod(nameof(Get), BindingFlags.NonPublic | BindingFlags.Instance)
                             .MakeGenericMethod(new Type[] { parameterType });

                var resolvedParameter = method.Invoke(this, Array.Empty<object>());
                resolvedParameters.Add(resolvedParameter);
            }

            return (T)constuctor.Invoke(resolvedParameters.ToArray());
        }
    }
}