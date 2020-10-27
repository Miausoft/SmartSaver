using System;
using System.Collections;

namespace SmartSaver
{
    public class WindowObjectCollection<T> : IEnumerable
    {
        private readonly T[] _tabs;

        public T this[int i]
        {
            get => _tabs[i];
            set => _tabs[i] = value;
        }

        public WindowObjectCollection(T[] tabs)
        {
            _tabs = tabs;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        public WindowObjectEnumerator<T> GetEnumerator()
        {
            return new WindowObjectEnumerator<T>(_tabs);
        }
    }

    public class WindowObjectEnumerator<T> : IEnumerator
    {
        private readonly T[] _tabs;
        private int _position = -1;

        public WindowObjectEnumerator(T[] tabs)
        {
            _tabs = tabs;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _tabs.Length);
        }

        public void Reset()
        {
            _position = -1;
        }

        object IEnumerator.Current => Current;

        public T Current
        {
            get
            {
                try
                {
                    return _tabs[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }

        }
    }
}