
using Vowel.Errors;
using Vowel.Nodes;

namespace Vowel.Runtime
{
    public class VowelEnvironment(VowelEnvironment closure)
    {
        private VowelEnvironment enclosing_env = closure;
        private Dictionary<string, object> env_variables = [];

        public void Define(string variable, object obj)
        {
            if(env_variables.ContainsKey(variable))
            {
                return;
            }

            env_variables.Add(variable, obj);
        }

        public object Get(string variable)
        {
            if(env_variables.TryGetValue(variable, out object? obj))
            {
                return obj;
            }

            //if(enclosing_env is not null)
            //{
            //    return enclosing_env.Get(variable);
            //}

            throw new RuntimeError($"Undefined variable '{variable}'");
        }

        public object GetVariableAt(string variable, Int32 distance)
        {
            VowelEnvironment target_env = AncestorEnvironment(variable, distance);  
            return target_env.Get(variable);
        }

        public void AssignVariableAt(string variable, object obj,Int32 distance)
        {
            VowelEnvironment target_env = AncestorEnvironment(variable, distance);
            target_env.Assign(variable,obj);
        }

        private VowelEnvironment AncestorEnvironment(string variable, Int32 distance)
        {
            VowelEnvironment target_env = this;
            for (int i = 0; i <= distance; i++)
            {
                if (target_env.enclosing_env is not null)
                {
                    target_env = target_env.enclosing_env;
                }
            }

            return target_env;
        }

        public void Assign(string variable, object obj)
        {
            if (env_variables.ContainsKey(variable))
            {
                env_variables[variable] = obj;
                return;
            }

            if(enclosing_env is not null)
            {
                enclosing_env.Assign(variable, obj); 
                return;
            }

            throw new RuntimeError($"Undefined variable {variable}");
        }
    }
}
