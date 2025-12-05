using UnityEngine;

namespace QFramework
{
    public class EventMonoBehaviour : MonoBehaviour
    {
        private GameEventComponent _eventComponent;

        public GameEventComponent GetEventComponent()
        {
            if (!_eventComponent)
            {
                _eventComponent = GameEventComponent.Create(gameObject);
            }

            return _eventComponent;
        }
    }
}