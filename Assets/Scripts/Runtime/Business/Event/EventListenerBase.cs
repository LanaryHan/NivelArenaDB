using System;
using UnityEngine;

namespace Runtime.Business.Event
{
    public abstract class EventListenerBase
    {
        private bool _isDeleted;
        public abstract Type EventType { get; }

        public virtual void MarkDeleted()
        {
            _isDeleted = true;
        }
        
        public virtual bool IsDeleted => _isDeleted;

        public virtual void CallDelegate(GameEvent e)
        {
            
        }

        public virtual GameEvent GetEvent()
        {
            return null;
        }
    }

    public class EventListener<T> : EventListenerBase where T : GameEvent
    {
        public delegate void EventDelegate(T e);

        private EventDelegate _delegate;
        private GameObject _attachedGameObject;
        public override Type EventType => typeof(T);

        public override void CallDelegate(GameEvent e)
        {
            try
            {
                if (!_attachedGameObject)
                {
                    EventManager.Instance.RemoveListener(this);
                    return;
                }

                _delegate.Invoke((T)e);
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("Exception in listener : {0}", EventType.Name);
                Debug.LogException(exception);
            }
        }

        public EventListener(EventDelegate @delegate, GameObject attachedGameObject)
        {
            _attachedGameObject = attachedGameObject;
            _delegate = @delegate;
        }
    }
}