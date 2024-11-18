
using Vowel.Errors;

namespace Vowel.Runtime
{
    public class Snapshot
    {
        public Stack<IDictionary<string, object>> frames = [];
        public List<Function> functions = [];
        public void TakeSnapshot(string lexeme, object obj)
        {
            if (obj is Function function)
            {
                CheckForDuplicateFunction(function);
                functions.Add(function);
            }

            if (frames.Count == 0)
            {
                Dictionary<string, object> snapshot = [];
                snapshot.Add(lexeme, obj);
                frames.Push(snapshot);
                return;
            }

            var current_frame = frames.Peek();
            Dictionary<string, object> frame = [];
            frame = frame.Concat(current_frame).ToDictionary();

            if(!frame.TryAdd(lexeme, obj))
            {
                frame[lexeme] = obj;
            }

            frames.Push(frame);
        }

        private void CheckForDuplicateFunction(Function function)
        {
            var overload = FindFunction(function.Arity());

            if (overload is not null)
            {
                throw new RuntimeError($"The member {function} was already defined");
            }
        }

        public Function FindFunction(int arity)
        {
            return functions.Where(f => f.Arity() == arity).FirstOrDefault()!;
        }
        public object GetValue(string lexeme)
        {
            var snapshot = GetFrame();

            if(snapshot.TryGetValue(lexeme, out var obj))
            {
                return obj;
            }

            throw new RuntimeError($"Undefined variable {lexeme}");
        }

        public void MutateValue(string lexeme, object value)
        {
            var snapshot = GetFrame();
            _ = GetValue(lexeme) ?? throw new RuntimeError($"Assignment to undefined variable {lexeme}");

            snapshot[lexeme] = value;
        }

        public Dictionary<string, object> GetFrame()
        {
            return (Dictionary<string, object>)frames.Peek();
        }

        public void PopFrame()
        {
            frames.Pop();
        }
    }
}
