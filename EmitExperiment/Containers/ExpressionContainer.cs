using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EmitExperiment.Containers
{
    /// <summary>
    /// Dependency container implementation.
    /// </summary>
    public class ExpressionContainer: IContainer
    {
        #region Constructor

        public ExpressionContainer()
        {
            _constructors = new Dictionary<Type, Func<IContainer, object>>();
            _registeredTypes = new Dictionary<Type, Func<object>>();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Cached list of constructors.
        /// </summary>
        private Dictionary<Type, Func<IContainer, object>> _constructors;

        /// <summary>
        /// List of explicit constructors.
        /// </summary>
        private Dictionary<Type, Func<object>> _registeredTypes;

        #endregion

        #region Methods

        /// <summary>
        /// Registers a type to be creatable from the container.
        /// </summary>
        public IContainer Register<T>(Func<T> ctor = null)
            where T : class
        {
            var type = typeof(T);
            if(_registeredTypes.ContainsKey(type))
                throw new ArgumentException($"Type {type.Name} is already registered.", nameof(ctor));

            _registeredTypes.Add(type, ctor);

            return this;
        }

        /// <summary>
        /// Returns the implementation of a type from the container.
        /// </summary>
        public object Get(Type type)
        {
            if(!_registeredTypes.TryGetValue(type, out var explicitCtor))
                throw new ArgumentException($"Type {type.Name} is not registered.");

            if(explicitCtor != null)
                return explicitCtor();

            try
            {
                var obj = _constructors[type](this);
                return obj;
            }
            catch(KeyNotFoundException)
            {
                throw new InvalidOperationException("The container must be prepared before usage.");
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException($"Could not resolve type '{type.Name}' because one of its dependencies is not registered.", ex);
            }
        }

        /// <summary>
        /// Returns the implementation of a type from the container.
        /// </summary>
        public T Get<T>()
            where T : class
        {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Prepares the constructors for all types.
        /// </summary>
        public void Prepare()
        {
            foreach(var type in _registeredTypes.Keys)
            {
                if(_registeredTypes[type] == null)
                    _constructors[type] = CreateFactory(type);
            }
        }

        #endregion

        #region Private helpers

        /// <summary>
        /// Generates a factory for the type.
        /// </summary>
        private Func<IContainer, object> CreateFactory(Type t)
        {
            var containerType = typeof(IContainer);
            var containerGet = containerType.GetMethod(nameof(Get), new Type[0]);

            var ctors = t.GetConstructors();
            if(ctors.Length > 1)
                throw new ArgumentException($"Cannot register type '{t.Name}' because it has more than one constructor.");

            var ctor = ctors[0];

            var lambdaArg = Expression.Parameter(containerType, "container");
            var args = ctor.GetParameters()
                           .Select(x => Expression.Call(lambdaArg, containerGet.MakeGenericMethod(x.ParameterType)));

            var expr = Expression.Lambda<Func<IContainer, object>>(Expression.New(ctor, args), lambdaArg);
            return expr.Compile();
        }

        #endregion
    }
}
