using System;

namespace EmitExperiment.Containers
{
    /// <summary>
    /// Common interface for IContainer implementations.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Registers a type to be creatable from the container.
        /// </summary>
        IContainer Register<T>(Func<T> ctor = null) where T : class;

        /// <summary>
        /// Returns the implementation of a type from the container.
        /// </summary>
        object Get(Type type);

        /// <summary>
        /// Returns the implementation of a type from the container.
        /// </summary>
        T Get<T>() where T : class;

        /// <summary>
        /// Prepares the container for further usage.
        /// </summary>
        void Prepare();
    }
}
