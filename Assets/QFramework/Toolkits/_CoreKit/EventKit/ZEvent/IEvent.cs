namespace QFramework
{
    public interface IEvent
    {
        protected GameEventComponent EventComponent { get; }

        public GameEventComponent GetEventComponent();
    }
}