using cfg.Deer;
using cfg.Deer.Enum;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    public class CarMoveData : EntityData
    {
        public Vector3 StartPosition { get; private set; }
        public Vector3 VictoryPosition { get; private set; }
        public CarMoveType MoveType { get; private set; } = CarMoveType.HORIZONTAL;

        public bool IsVictoryPos
        {
            get
            {
                return Position == VictoryPosition;
            }
        }

        private CarEntityData m_CarEntityData = null;
        public CarMoveData(int entityId, int typeId, string assetName) : base(entityId, typeId, assetName)
        {
            GameEntry.Config.Tables.TbCarEntityData.DataMap.TryGetValue(typeId, out m_CarEntityData);
            if (m_CarEntityData == null)
            {
                Logger.Error("配置错误，没有这个汽车");
                return;
            }

            StartPosition = m_CarEntityData.StartPos;
            VictoryPosition = m_CarEntityData.VictoryPos;

            MoveType = (CarMoveType)m_CarEntityData.MoveType;
        }
    }
}