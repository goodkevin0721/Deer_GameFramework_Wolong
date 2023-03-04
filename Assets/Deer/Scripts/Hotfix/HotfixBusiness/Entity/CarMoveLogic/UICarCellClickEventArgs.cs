using GameFramework;
using GameFramework.Event;

namespace HotfixBusiness.Entity
{
    public class UICarCellCreatEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UICarCellCreatEventArgs).GetHashCode();

        public UICarCellCreatEventArgs()
        {
            UserData = null;
        }
        public object UserData
        {
            get;
            private set;
        }

        public static UICarCellClickEventArgs Create()
        {
            UICarCellClickEventArgs eventArgs = ReferencePool.Acquire<UICarCellClickEventArgs>();
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