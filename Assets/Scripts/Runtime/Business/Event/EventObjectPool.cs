using System;
using System.Collections.Generic;
using QFramework;

namespace Runtime.Business.Event
{
    public class EventObjectPool : Singleton<EventObjectPool>
    {
        private Dictionary<Type, GameEvent> _pool = new();

        public T GetNew<T>() where T : GameEvent, new()
        {
            var type = typeof(T);
            if (_pool.TryGetValue(type, out var @event) && @event != null)
            {
                _pool[type] = null;
                return @event as T;
            }
            
            return new T();
        }

        public object GetNew(Type type)
        {
            if (_pool.TryGetValue(type, out var @event) && @event != null)
            {
                _pool[type] = null;
                return @event;
            }
            
            return Activator.CreateInstance(type);
        }

        public void Release(GameEvent @event)
        {
            var type = @event.GetType();
            if (!_pool.TryGetValue(type, out var ge) || ge == null)
            {
                _pool[type] = @event;
            }
        }
    }
}