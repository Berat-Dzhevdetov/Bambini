namespace Bambini.Services.DependencyResolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class DependencyResolver
    {
        private readonly Dictionary<Type, Type> regs = new();

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
            if (!regs.ContainsKey(typeof(T)))
            {
                throw new InvalidDataException($"Couldn't find a class to resolve '{typeof(T).FullName}'");
            }
            return CreateInstance<T>();
        }

        private T CreateInstance<T>()
        {
            if (typeof(T).IsAbstract)
            {
                throw new ArgumentException("Excepted interface but got class");
            }

            var classType = regs[typeof(T)];
            var constuctors = classType.GetType().GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (constuctors.Length > 1)
            {
                throw new ArgumentException($"Found more than one constuctor for '{typeof(T).FullName}' type");
            }

            var constuctor = constuctors.FirstOrDefault();

            if (constuctor == null)
            {
                throw new ArgumentNullException($"Excepted at least one constuctor for '{typeof(T).FullName}' type");
            }

            var parameters = constuctor.GetParameters();

            var a = new List<object>();

            foreach (var parameter in parameters)
            {

            }

            return Activator.CreateInstance<T>();
        }
    }
}