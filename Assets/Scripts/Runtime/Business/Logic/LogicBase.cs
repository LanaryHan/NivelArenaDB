using QFramework;

namespace Logic
{
    public class LogicBase : EventMonoBehaviour
    {
        protected virtual void RegisterEvents()
        {
            
        }

        protected virtual void Awake()
        {
            RegisterEvents();
        }
    }
}