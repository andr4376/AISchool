using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionChecker
{
    public  class NameSpaceChecker : MarshalByRefObject
    {
        List<string> invalidNames;

        public NameSpaceChecker()
        {
            invalidNames = new List<string> { "System.Diagnostics.StackTrace", "System.Reflection.Assembly" };
        }

        public bool IsAddonInvalid(out string error)
        {
            error = "";
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                if (!a.FullName.Contains("Microsoft") && !a.FullName.Contains("mscorlib") && !a.FullName.Contains("AIFramework") && !a.FullName.Contains("ReflectionChecker"))
                {
                    foreach (Type t in a.GetTypes())
                    {
                        foreach (MethodInfo mb in t.GetMethods())
                        {
                            string validationResult = validate(mb);
                            if (!String.IsNullOrEmpty(validationResult))
                            {

                                error = String.Format("Error in {0}, using namespace {1} in {2}", a.FullName, validationResult, mb.Name);
                                return true;
                            }
                           
                        }
                    }
                }
            }
            return false;
        }

        private string validate(MethodInfo mi)
        {
            if (mi.GetMethodBody() == null)
            {
                return "";
            }
            var instructions = MethodBodyReader.GetInstructions(mi);

            foreach (Instruction instruction in instructions)
            {
                MethodInfo methodInfo = instruction.Operand as MethodInfo;

                if (methodInfo != null)
                {
                    Type type = methodInfo.DeclaringType;
                    ParameterInfo[] parameters = methodInfo.GetParameters();

                    String temp = string.Format("{0}.{1}({2});",
                        type.FullName,
                        methodInfo.Name,
                        String.Join(", ", parameters.Select(p => p.ParameterType.FullName + " " + p.Name).ToArray())
                    );
                    foreach (string n in invalidNames)
                    {
                        if (temp.Contains(n))
                        {
                            return n;
                        }
                    }
                }
            }

            return "";
        }
    }
}
