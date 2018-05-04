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
            var met = type.DefineMethod("Max", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Static, typeof(int), new[] {typeof(int), typeof(int)});

            met.DefineParameter(0, ParameterAttributes.None, "a");
            met.DefineParameter(1, ParameterAttributes.None, "b");

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
           
            // if(a > b)
            gen.MarkSequencePoint(doc, 3, 5, 3, 14);

            var elseLabel = gen.DefineLabel();
            var endLabel = gen.DefineLabel();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Blt, elseLabel);
            
            // return a;
            gen.MarkSequencePoint(doc, 4, 9, 4, 18);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Br, endLabel);
            
            // return b;
            gen.MarkSequencePoint(doc, 6, 9, 6, 18);
            gen.MarkLabel(elseLabel);
            gen.Emit(OpCodes.Ldarg_1);
            
            gen.MarkLabel(endLabel);
            gen.Emit(OpCodes.Ret);
        }
    }
}
