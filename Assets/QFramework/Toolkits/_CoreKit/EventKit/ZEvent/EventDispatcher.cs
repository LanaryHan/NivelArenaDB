using System.Collections.Generic;

namespace QFramework
{
    public abstract class EventDispatcher
    {
        private int _level;
        private List<EventListenerBase> _listeners = new();

        public virtual void AddListener(EventListenerBase listener)
        {
            _listeners.Add(listener);
        }

        protected abstract void HandleListener(GameEvent gameEvent, EventListenerBase listener);

        public void Dispatch(GameEvent gameEvent)
        {
            _level++;
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                var listener = _listeners[i];
                if (!listener.IsDeleted)
                {
                    HandleListener(gameEvent, listener);
                }
            }

            _level--;
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                var listener = _listeners[i];
                if (listener.IsDeleted)
                {
                    _listeners.RemoveAt(i);
                }
            }
        }

        public void RemoveListener(EventListenerBase listener)
        {
            if (_level > 0)
            {
                listener.MarkDeleted();
            }

            else
            {
                _listeners.Remove(listener);
            }
        }

        public void Clear()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].MarkDeleted();
            }

            _listeners.Clear();
        }
    }

    public class NormalEventDispatch : EventDispatcher
    {
        protected override void HandleListener(GameEvent gameEvent, EventListenerBase listener)
        {
            listener.CallDelegate(gameEvent);
        }
    }
}