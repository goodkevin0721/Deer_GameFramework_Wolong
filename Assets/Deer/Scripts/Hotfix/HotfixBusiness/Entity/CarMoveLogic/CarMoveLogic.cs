using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.Entity
{
    public class CarMoveLogic : EntityLogicBase
    {
        private CarMoveData m_CarMoveData;
        public CarMoveData CarMoveData { get { return m_CarMoveData; }}
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            m_CarMoveData = userData as CarMoveData;
            if (m_CarMoveData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            CachedTransform.localPosition = m_CarMoveData.Position;
            CachedTransform.localScale = Vector3.one;

        }

        private void OnTriggerEnter2D(Collider2D col)
        {
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            
        }
    }
}