using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace EmitDebug
{
    /// <summary>
    /// The emitter that writes debug information.
    /// </summary>
    public class DebugEmitter
    {
        public MethodInfo CreateDebuggableMethod()
        {
            var an = new AssemblyName("DebugTestAssembly");
            var asm = Thread.GetDomain().DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);

            MarkAsDebuggable(asm);

            var mod = asm.DefineDynamicModule("CompareModule", true);
            var doc = mod.DefineDocument("Compare.txt", Guid.Empty, Guid.Empty, Guid.Empty);

            var type = mod.DefineType("TestType", TypeAttributes.Class | TypeAttributes.Public);
            var met = type.DefineMethod("Max", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Static, typeof(int), new[] {typeof(int[])});

            met.DefineParameter(0, ParameterAttributes.None, "items");

            EmitMethodBody(met, doc);

            type.CreateType();
            return type.GetMethod("Max");
        }

        private void MarkAsDebuggable(AssemblyBuilder asm)
        {
            var daType = typeof(DebuggableAttribute);
            var ctor = daType.GetConstructor(new[] {typeof(DebuggableAttribute.DebuggingModes)});
            var builder = new CustomAttributeBuilder(
                ctor,
                new object[]
                {
                    DebuggableAttribute.DebuggingModes.Default
                    | DebuggableAttribute.DebuggingModes.DisableOptimizations
                }
            );

            asm.SetCustomAttribute(builder);
        }

        private void EmitMethodBody(MethodBuilder met, ISymbolDocumentWriter doc)
        {
            var gen = met.GetILGenerator();

            var currMax = gen.DeclareLocal(typeof(int));
            var i = gen.DeclareLocal(typeof(int));
            var elem = gen.DeclareLocal(typeof(int));
          
            // if(items.length == 0)
            var afterCheckLabel = gen.DefineLabel();
            gen.MarkSequencePoint(doc, 3, 5, 3, 100);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldlen);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Bge, afterCheckLabel);

            // throw "Error"
            gen.MarkSequencePoint(doc, 4, 9, 4, 100);
            gen.Emit(OpCodes.Ldstr, "Array is empty!");
            gen.Emit(OpCodes.Newobj, typeof(ArgumentException).GetConstructor(new [] { typeof(string) }));
            gen.Emit(OpCodes.Throw);
            gen.MarkLabel(afterCheckLabel);

            // var currMax = items[0]
            gen.MarkSequencePoint(doc, 6, 5, 6, 100);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ldelem_I4);
            gen.Emit(OpCodes.Stloc, currMax);

            // var i = 1
            gen.MarkSequencePoint(doc, 7, 5, 7, 100);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Stloc, i);

            // gen.Emit(OpCodes.Ldc_I4_1);
            // gen.Emit(OpCodes.Ret);
            // return;

            var loopLabel = gen.DefineLabel();
            var afterLoopLabel = gen.DefineLabel();

            // while (i < items.length)
            gen.MarkSequencePoint(doc, 9, 5, 9, 100);
            gen.MarkLabel(loopLabel);
            gen.Emit(OpCodes.Ldloc, i);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldlen);
            gen.Emit(OpCodes.Beq, afterLoopLabel);

            // var elem = items[idx]
            gen.MarkSequencePoint(doc, 11, 9, 11, 100);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldloc, i);
            gen.Emit(OpCodes.Ldelem_I4);
            gen.Emit(OpCodes.Stloc, elem);
            
            // if(elem > currMax)
            var afterCompareLabel = gen.DefineLabel();
            gen.MarkSequencePoint(doc, 13, 9, 13, 100);
            gen.Emit(OpCodes.Ldloc, elem);
            gen.Emit(OpCodes.Ldloc, currMax);
            gen.Emit(OpCodes.Ble, afterCompareLabel);
            
            // currMax = elem
            gen.MarkSequencePoint(doc, 14, 13, 14, 100);
            gen.Emit(OpCodes.Ldloc, elem);
            gen.Emit(OpCodes.Stloc, currMax);
            gen.MarkLabel(afterCompareLabel);

            // i++
            gen.MarkSequencePoint(doc, 16, 9, 16, 100);
            gen.Emit(OpCodes.Ldloc, i);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Add);
            gen.Emit(OpCodes.Stloc, i);

            gen.Emit(OpCodes.Br, loopLabel);

            // return currMax
            gen.MarkSequencePoint(doc, 19, 5, 19, 100);
            gen.MarkLabel(afterLoopLabel);
            gen.Emit(OpCodes.Ldloc, currMax);
            gen.Emit(OpCodes.Ret);
        }
    }
}
