using GameFramework;
using GameFramework.Event;

namespace HotfixBusiness.Entity
{
    public class UICarCellClickEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UICarCellClickEventArgs).GetHashCode();

        public UICarCellClickEventArgs()
        {
            UserData = null;
        }
        public object UserData
        {
            get;
            private set;
        }

        public static UICarCellClickEventArgs Create(UICarItem userData)
        {
            UICarCellClickEventArgs eventArgs = ReferencePool.Acquire<UICarCellClickEventArgs>();
            eventArgs.UserData = userData;
            return eventArgs;
        }

        public override void Clear()
        {
            UserData = null;
        }

        public override int Id
        {
            get { return EventId; }
        }
    }
}