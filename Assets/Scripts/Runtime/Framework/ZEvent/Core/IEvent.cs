namespace ZEvent
{
    public interface IEvent
    {
        protected GameEventComponent EventComponent { get; }

        public GameEventComponent GetEventComponent();
    }
}