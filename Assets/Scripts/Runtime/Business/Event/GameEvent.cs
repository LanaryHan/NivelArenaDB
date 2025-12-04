namespace Runtime.Business.Event
{
    public class GameEvent
    {
        public GameEvent()
        {
        }
    }

    public class GameEventBase<T> : GameEvent where T : GameEventBase<T>, new()
    {
        public static T Create()
        {
            return EventObjectPool.Instance.GetNew<T>();
        }
    }

    public class GameEventBaseNoDefaultCreate<T> : GameEvent where T : GameEventBaseNoDefaultCreate<T>, new()
    {
        protected static T Create()
        {
            return EventObjectPool.Instance.GetNew<T>();
        }
    }

    public class GameListenEvent : GameEvent
    {
        public EventListenerBase Listener;
    }

    public class GameListenEvent<T> : GameListenEvent where T : GameEvent
    {
        public static GameListenEvent<T> Create(EventListenerBase listener)
        {
            var evt = EventObjectPool.Instance.GetNew<GameListenEvent<T>>();
            evt.Listener = listener;
            return evt;
        }
    }
}