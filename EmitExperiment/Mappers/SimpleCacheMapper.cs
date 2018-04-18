using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitExperiment.Mappers
{
    /// <summary>
    /// Simple reflection-based mapper.
    /// </summary>
    public class SimpleCacheMapper: IMapper
    {
        /// <summary>
        /// Property map lookup.
        /// </summary>
        private Dictionary<Tuple<Type, Type>, Mapping> _propMaps = new Dictionary<Tuple<Type, Type>, Mapping>();

        /// <summary>
        /// Maps property values between types. 
        /// </summary>
        public T2 Map<T1, T2>(T1 source)
        {
            var key = Tuple.Create(typeof(T1), typeof(T2));
            if(!_propMaps.TryGetValue(key, out var map))
                throw new InvalidOperationException($"Map {typeof(T1).Name} to {typeof(T2).Name} is not defined.");

            var dest = (T2) map.Constructor();

            foreach (var prop in map.PropertyMaps)
                prop.Key.SetValue(dest, prop.Value.GetValue(source));

            return dest;
        }
        
        /// <summary>
        /// Builds required cache for the types.
        /// </summary>
        public void Prepare<T1, T2>()
        {
            var t1 = typeof(T1);
            var t2 = typeof(T2);
            var key = Tuple.Create(t1, t2);

            if (_propMaps.ContainsKey(key))
                return;

            var props = t2.GetProperties()
                          .ToDictionary(x => x, x => t1.GetProperty(x.Name));

            var ctor = t2.GetConstructor(new Type[0]);

            _propMaps[key] = new Mapping
            {
                PropertyMaps = props,
                Constructor = () => ctor.Invoke(Array.Empty<object>())
            };
        }

        /// <summary>
        /// Cached mapping info.
        /// </summary>
        private class Mapping
        {
            public Dictionary<PropertyInfo, PropertyInfo> PropertyMaps;
            public Func<object> Constructor;
        }
    }
}
