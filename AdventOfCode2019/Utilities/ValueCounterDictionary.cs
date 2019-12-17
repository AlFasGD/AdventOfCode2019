using System.Collections;
using System.Collections.Generic;

namespace AdventOfCode2019.Utilities
{
    public class ValueCounterDictionary<T> : IEnumerable<KeyValuePair<T, int>>
    {
        private Dictionary<T, int> counters = new Dictionary<T, int>();

        public ValueCounterDictionary() { }
        public ValueCounterDictionary(IEnumerable<T> collection)
        {
            foreach (var v in collection)
                Add(v);
        }
        public ValueCounterDictionary(IEnumerable collection)
        {
            foreach (var v in collection)
                Add((T)v);
        }

        public void Add(T value, int count = 1)
        {
            if (counters.ContainsKey(value))
                counters[value] += count;
            else
                counters.Add(value, count);
        }
        public void Remove(T value, int count = 1) => counters[value] -= count;

        public void Add(KeyValuePair<T, int> kvp) => counters.Add(kvp.Key, kvp.Value);
        public void Clear() => counters.Clear();
        public void AdjustValue(T oldValue, T newValue)
        {
            Remove(oldValue);
            Add(newValue);
        }

        public bool ContainsKey(T key) => counters.ContainsKey(key);
        public bool TryGetValue(T key, out int value) => counters.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator() => counters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => counters.GetEnumerator();

        public int this[T value] => counters[value];
    }
}
