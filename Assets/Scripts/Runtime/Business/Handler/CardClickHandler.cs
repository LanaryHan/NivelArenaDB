using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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
            ec.Listen<UIEvents.OnDialogOpen>(e =>
            {
                if (e.Dialog.dialogName is Dialog.MessageUI)
                {
                    _enable = false;
                }
            });
            ec.Listen<UIEvents.OnDialogClose>(e =>
            {
                if (e.Dialog is Dialog.MessageUI)
                {
                    _enable = true;
                }
            });
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (!_enable)
            {
                return;
            }

            Vector2 screenPosition;

            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                screenPosition = Mouse.current.position.ReadValue();
            }
            else if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            {
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else
            {
                return;
            }
            
            var worldPoint = cardCamera.ScreenToWorldPoint(screenPosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider)
            {
                EventManager.Instance.Send(GameEvents.ReverseCard.Create());
            }
        }
    }
}