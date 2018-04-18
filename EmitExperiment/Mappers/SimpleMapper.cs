using System;

namespace EmitExperiment.Mappers
{
    /// <summary>
    /// Simple reflection-based mapper.
    /// </summary>
    public class SimpleMapper: IMapper
    {
        /// <summary>
        /// Maps property values between types. 
        /// </summary>
        public T2 Map<T1, T2>(T1 source)
        {
            var t1 = typeof(T1);
            var t2 = typeof(T2);

            var dest = (T2) t2.GetConstructor(new Type[0]).Invoke(new object[0]);

            foreach (var prop in t2.GetProperties())
            {
                var srcProp = t1.GetProperty(prop.Name);
                prop.SetValue(dest, srcProp.GetValue(source));
            }

            return dest;
        }

        /// <summary>
        /// Builds required cache for the types.
        /// </summary>
        public void Prepare<T1, T2>()
        {
            // does nothing
        }
    }
}
