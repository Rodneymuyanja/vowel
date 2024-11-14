
using Vowel.Errors;

namespace Vowel.Runtime
{
    public class Snapshot
    {
        public Stack<Dictionary<string, object>> frames = [];
        public void TakeSnapshot(string lexeme, object obj)
        {
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
            return frames.Peek();
        }

        public void PopFrame()
        {
            frames.Pop();
        }
    }
}
