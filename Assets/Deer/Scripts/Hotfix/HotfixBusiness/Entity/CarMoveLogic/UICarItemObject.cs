using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    public class UICarItemObject : ObjectBase
    {
        public static UICarItemObject Create(object target)
        {
            UICarItemObject hpBarItemObject = ReferencePool.Acquire<UICarItemObject>();
            hpBarItemObject.Initialize(target);
            return hpBarItemObject;
        }

        protected override void Release(bool isShutdown)
        {
            UICarItem hpBarItem = (UICarItem)Target;
            if (hpBarItem == null)
            {
                return;
            }

            Object.Destroy(hpBarItem.gameObject);
        }
    }
}