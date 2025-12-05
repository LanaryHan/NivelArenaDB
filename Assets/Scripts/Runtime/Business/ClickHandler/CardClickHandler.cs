using QFramework;
using UnityEngine;

namespace Runtime.Business.ClickHandler
{
    public class CardClickHandler : MonoBehaviour
    {
        public Camera cardCamera;
        private void Update()
        {
            // if (!UIKit.GetPanel<CardDetailUI>())
            // {
            //     return;
            // }

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