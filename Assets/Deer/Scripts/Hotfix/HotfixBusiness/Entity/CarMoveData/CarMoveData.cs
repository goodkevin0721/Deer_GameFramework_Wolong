using System.Collections.Generic;
using cfg.Deer;
using cfg.Deer.Enum;
using Flower;
using GameFramework;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    public class CarMoveData : IReference
    {
        public ECarDirectionType DirectionType { get; private set; } = ECarDirectionType.HORIZONTAL;
        public Vector3 DefaultPos { get;private set; }
        /// <summary>
        /// 初始占用的格子
        /// </summary>
        public List<IntVector2> DefaultIntVector2
        {
            get { return m_DefaultIntVector2; }
        }
        /// <summary>
        /// 当前占用的格子
        /// </summary>
        public List<IntVector2> CurrentIndexList
        {
            get { return m_CurrentIntVector2; }
        }
        public CarEntityData EntityTbData
        {
            get { return m_EntityTbData; }
        }
        /// <summary>
        /// 大于默认占用的格子 需要删除一个格子
        /// </summary>
        public bool IsCorrect 
        {
            get
            {
                return m_CurrentIntVector2.Count > m_DefaultIntVector2.Count;
            }
        }
        private List<IntVector2> m_DefaultIntVector2 = new List<IntVector2>();
        private List<IntVector2> m_CurrentIntVector2 = new List<IntVector2>();
        private CarEntityData m_EntityTbData = null;


        public void Init(CarEntityData tempData)
        {
            m_EntityTbData = tempData;
            if (m_EntityTbData == null)
            {
                Logger.Error("配置错误，没有这个汽车");
                return;
            }

            foreach (var tempIndex in m_EntityTbData.VictoryPos)
            {
                m_DefaultIntVector2.Add(tempIndex.ConvertInVector2());
                m_CurrentIntVector2.Add(tempIndex.ConvertInVector2());
            }

            DirectionType = (ECarDirectionType)m_EntityTbData.MoveDirection;
        }

        public void CalculatePosition()
        {
            if (m_DefaultIntVector2.Count >= 2)
            {
                DefaultPos = GameEntry.CarPlacement.GetPos(m_DefaultIntVector2[0],
                    m_DefaultIntVector2[m_DefaultIntVector2.Count - 1],DirectionType == ECarDirectionType.VERTICAL);
                //Logger.Debug($"DefaultPos = {DefaultPos}");
            }
            else
            {
                Logger.Error("位置配置错误");
            }
        }
        public Vector3 CalculateLocalPosition()
        {
            return GameEntry.CarPlacement.GetLocalPos(m_CurrentIntVector2[0],
                m_CurrentIntVector2[m_CurrentIntVector2.Count - 1],DirectionType == ECarDirectionType.VERTICAL);
        }

        public void Clear()
        {
            m_CurrentIntVector2.Clear();
        }
        
        public static CarMoveData Create(CarEntityData tempData)
        {
            CarMoveData assetObjectInfo = ReferencePool.Acquire<CarMoveData>();
            assetObjectInfo.Init(tempData);
            return assetObjectInfo;
        }
    }
}