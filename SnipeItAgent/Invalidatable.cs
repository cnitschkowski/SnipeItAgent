using System;

namespace SnipeItAgent
{
    public class Invalidatable<T>
    {
        private readonly object _lockObject = new object();
        
        private readonly Func<T> _getNewInstance;

        private T _value;
        
        public Invalidatable()
            : this(Activator.CreateInstance<T>)
        {
        }

        public Invalidatable(Func<T> getNewInstance)
        {
            this._getNewInstance = getNewInstance;
        }

        public bool HasValue { get; private set; }

        public T Value
        {
            get
            {
                lock (this._lockObject)
                {
                    if (this.HasValue)
                    {
                        return this._value;
                    }

                    var value = this._getNewInstance();
                    this._value = value;

                    return this._value;
                }
            }
        }

        public void Invalidate()
        {
            lock (this._lockObject)
            {
                this.HasValue = false;
            }
        }

        public T GetValueOrDefault()
        {
            lock (this._lockObject)
            {
                return this.HasValue ? this._value : default(T);
            }
        }
    }
}