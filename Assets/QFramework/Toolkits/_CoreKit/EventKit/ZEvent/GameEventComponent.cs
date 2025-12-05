using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class GameEventComponent : MonoBehaviour
    {
        private List<EventListenerBase> _listeners = new();
        private EventManager _eventManager;

        public static GameEventComponent Create(GameObject gameObject, EventManager manager = null)
        {
            if (!manager)
            {
                manager = EventManager.Instance;
            }

            var comp = gameObject.GetComponent<GameEventComponent>();
            if (!comp)
            {
                comp = gameObject.AddComponent<GameEventComponent>();
                comp._eventManager = manager;
                comp.hideFlags = HideFlags.DontSave;
            }
            else if (!comp._eventManager)
            {
                comp._eventManager = EventManager.Instance;
            }
            
            return comp;
        }

        public void Listen<T>(EventListener<T>.EventDelegate callback) where T : GameEvent
        {
            var listener = _eventManager.Listen(callback, gameObject);
            _listeners.Add(listener);
        }

        public void RemoveListener(EventListenerBase listener)
        {
            _eventManager.RemoveListener(listener);
            var index = _listeners.IndexOf(listener);
            if (index >= 0)
            {
                _listeners.RemoveAt(index);
            }
        }

        public void ClearListeners()
        {
            foreach (var listener in _listeners)
            {
                _eventManager.RemoveListener(listener);
            }

            _listeners.Clear();
        }

        public void Send(GameEvent gameEvent)
        {
            _eventManager.Send(gameEvent);
        }

        public void Send<T>() where T : GameEvent
        {
            _eventManager.Send<T>();
        }

        private void OnDestroy()
        {
            ClearListeners();
        }
    }
}