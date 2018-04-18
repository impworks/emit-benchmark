using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EmitExperiment.Mappers
{
    /// <summary>
    /// Emit-based mapper.
    /// </summary>
    public class EmitMapper: IMapper
    {
        /// <summary>
        /// Property map lookup.
        /// </summary>
        private Dictionary<Tuple<Type, Type>, object> _propMaps = new Dictionary<Tuple<Type, Type>, object>();

        /// <summary>
        /// Maps property values between types. 
        /// </summary>
        public T2 Map<T1, T2>(T1 source)
        {
            var key = Tuple.Create(typeof(T1), typeof(T2));
            if(!_propMaps.TryGetValue(key, out var map))
                throw new InvalidOperationException($"Map {typeof(T1).Name} to {typeof(T2).Name} is not defined.");

            var func = (Func<T1, T2>) map;
            return func(source);
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

            var dm = new DynamicMethod($"{t1.Name}To{t2.Name}", t2, new []{ t1 });
            var ig = dm.GetILGenerator();
            var loc = ig.DeclareLocal(t2);

            // ctor
            var ctor = t2.GetConstructor(Array.Empty<Type>());
            ig.Emit(OpCodes.Newobj, ctor);
            ig.Emit(OpCodes.Stloc, loc);

            // props
            foreach (var prop in t2.GetProperties())
            {
                var srcProp = t1.GetProperty(prop.Name);

                ig.Emit(OpCodes.Ldloc, loc);
                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(OpCodes.Callvirt, srcProp.GetGetMethod());
                ig.Emit(OpCodes.Callvirt, prop.GetSetMethod());
            }

            // return
            ig.Emit(OpCodes.Ldloc, loc);
            ig.Emit(OpCodes.Ret);

            var func = (object) dm.CreateDelegate(typeof(Func<T1, T2>));
            _propMaps[key] = func;
        }
    }
}
