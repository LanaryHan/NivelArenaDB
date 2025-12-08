using QFramework;
using UnityEngine;

namespace Runtime.Business.Handler
{
    public class CardUIFollowHandler : EventMonoBehaviour
    {
        public Camera uiCamera;
        public Camera cardCamera;
        public GameObject card;

        private RectTransform _uiCardTarget;
        private Vector3 _defaultPos;
        private bool _enable;
        private void Start()
        {
            _defaultPos = card.transform.position;
            var ec = GetEventComponent();
            ec.Listen<GameEvents.ShowCard>(_ =>
            {
                _enable = true;
            });
            ec.Listen<GameEvents.HideCard>(_ =>
            {
                _enable = false;
            });
            ec.Listen<GameEvents.CardFollowReady>(evt =>
            {
                _uiCardTarget = evt.Target;
                card.transform.position = _defaultPos;
            });
        }

        private void Update()
        {
            if (!_enable)
            {
                return;
            }

            var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, _uiCardTarget.position);
            var worldPos = cardCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
            worldPos.z = 200f;
            card.transform.position = worldPos;
            Debug.Log(worldPos);
        }
    }
}