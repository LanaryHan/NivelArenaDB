using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Runtime.Business.Event
{
    public class EventManager : MonoSingleton<EventManager>
    {
        protected EventManager() { }
        private Dictionary<Type, EventDispatcher> _dispatchers = new();

        public EventListenerBase Listen<T>(EventListener<T>.EventDelegate callback, GameObject attached) where T : GameEvent
        {
            if (callback == null)
            {
                return null;
            }

            long d;

            EventListenerBase listener = new EventListener<T>(callback, attached);
            if (!_dispatchers.TryGetValue(listener.EventType, out var dispatcher))
            {
                dispatcher = new NormalEventDispatch();
                _dispatchers.Add(listener.EventType, dispatcher);
            }

            dispatcher.AddListener(listener);

            var gameListenEvent = GameListenEvent<T>.Create(listener);
            Send(gameListenEvent);
            
            return listener;
        }

        public void RemoveListener(EventListenerBase listener)
        {
            if (listener == null)
            {
                return;
            }

            if (_dispatchers.TryGetValue(listener.EventType, out var dispatcher))
            {
                dispatcher.RemoveListener(listener);
            }
        }

        public void Send(GameEvent gameEvent)
        {
            if (gameEvent == null)
            {
                return;
            }

            if (_dispatchers.TryGetValue(gameEvent.GetType(), out var dispatcher))
            {
                dispatcher.Dispatch(gameEvent);
            }
            
            EventObjectPool.Instance.Release(gameEvent);
        }

        public void Send(Type type)
        {
            if (_dispatchers.TryGetValue(type, out var dispatcher))
            {
                dispatcher.Dispatch(null);
            }
        }

        public void Send<T>() where T : GameEvent
        {
            Send(typeof(T));
        }
    }
}