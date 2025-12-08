using QFramework;
using UnityEngine;

namespace Runtime.Business.Handler
{
    public class CardClickHandler : EventMonoBehaviour
    {
        public Camera cardCamera;

        private bool _enable;

        private void Start()
        {
            var ec = GetEventComponent();
            ec.Listen<GameEvents.ShowCard>(_ =>
            {
                _enable = true;
            });
            ec.Listen<GameEvents.HideCard>(_ =>
            {
                _enable = false;
            });
        }

        private void Update()
        {
            if (!_enable)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = cardCamera.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider)
                {
                    EventManager.Instance.Send(GameEvents.ReverseCard.Create());
                }
            }
        }
    }
}