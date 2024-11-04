
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

            return null!;
        }
    }
}
