
using Vowel.Errors;

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

            if(enclosing_env is not null)
            {
                return enclosing_env.Get(variable);
            }

            throw new RuntimeError($"Undefined variable '{variable}'");
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
