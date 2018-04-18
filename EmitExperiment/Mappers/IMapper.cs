namespace EmitExperiment.Mappers
{
    /// <summary>
    /// Interface for mapper implementations.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Maps object of type 1 to type 2.
        /// </summary>
        T2 Map<T1, T2>(T1 source);

        /// <summary>
        /// Builds required cache for the types.
        /// </summary>
        void Prepare<T1, T2>();
    }
}
