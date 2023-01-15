using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EazTrialRemover {
    internal class Program {
        static void Main(string[] args) {
            string path = string.Empty;
            bool removed = false;
            if(args.Length > 0 ) {
                path = args[0];
            } else {
                do {
                    Console.Write("Path: ");
                    path = Console.ReadLine();
                } while(string.IsNullOrEmpty(path));
            }

            if(!path.Contains(".exe") && !path.Contains(".dll")) {
                Console.WriteLine("Invalid File Format");
                return;
            }
            var module = ModuleDefMD.Load(path);
            removed = patch(module);
            if(removed) {
                string output = path.Contains(".exe") ? path.Replace(".exe", "_removed.exe") : path.Replace(".dll", "_removed.dll");
                if(module.IsILOnly) {
                    module.Write(output);
                    return;
                }
                module.NativeWrite(output);
            }
            Console.WriteLine(removed ? "Removed Trial Succesufully" : "Couldn't remove trial");
            Thread.Sleep(1500);
            Environment.Exit(removed ? 0 : -1);
        }
        static bool patch(ModuleDefMD Module) {
            foreach(var type in Module.Types) {
                foreach(var method in type.Methods) {
                    if(!method.HasBody) continue;
                    var instrs = method.Body.Instructions;
                    if(instrs.Count > 3) continue; // why not
                    if(instrs[0].OpCode == OpCodes.Ldc_I4_1 && instrs[1].OpCode == OpCodes.Call) {
                        if(instrs[1].Operand is MethodDef) {
                            var methodCall = instrs[1].Operand as MethodDef;
                            int datetimeFound = 0;
                            foreach(var instr in methodCall.Body.Instructions.Where(x => x.OpCode == OpCodes.Call)) {
                                if(instr.ToString().Contains("DateTime")) datetimeFound++;
                            }
                            if(datetimeFound == 5) {
                                instrs[1].OpCode = OpCodes.Nop;
                                Console.WriteLine("Type Token: " + methodCall.DeclaringType.MDToken.ToInt32());
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
