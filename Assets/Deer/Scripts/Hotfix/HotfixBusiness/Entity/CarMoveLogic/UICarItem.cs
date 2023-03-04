using System;
using System.Collections.Generic;
using cfg.Deer.Enum;
using Flower;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.Entity
{
    public class UICarItem : MonoBehaviour, IPointerClickHandler
    {
        private CarMoveData m_CarMoveData;
        private List<IntVector2> m_CurrentIndexList = null;

        private Camera m_Camera;
        private Image m_Image = null;
        private RectTransform m_RectTransform = null;
        private TextMeshProUGUI m_ProUGUI = null;
        private BoxCollider2D m_Collider2D = null;

        public CarMoveData CarMoveData
        {
            get { return m_CarMoveData; }
        }

        public Vector3 DefaultPos { get; private set; }

        public float MaxMoveValue { get; private set; }
        public float MinMoveValue { get; private set; }

        private void Awake()
        {
            m_Camera = Camera.main;
            m_Image = transform.GetComponent<Image>();
            m_ProUGUI = transform.GetComponent<TextMeshProUGUI>("Text");
            m_Collider2D = transform.GetComponent<BoxCollider2D>();
            m_RectTransform = transform.GetComponent<RectTransform>();
        }

        public void Init(object userData)
        {
            m_CarMoveData = userData as CarMoveData;
            if (m_CarMoveData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }
            m_CurrentIndexList = m_CarMoveData.CurrentIndexList;
            DefaultPos = m_CarMoveData.CalculateLocalPosition();

            gameObject.name = m_CarMoveData.EntityTbData.Id.ToString();
            m_ProUGUI.text = m_CarMoveData.EntityTbData.Id.ToString();
            m_Collider2D.size = m_CarMoveData.EntityTbData.CellSize;
            m_RectTransform.sizeDelta = m_CarMoveData.EntityTbData.CellSize;
            foreach (var tempPos in m_CarMoveData.DefaultIntVector2)
            {
                //m_CurrentIndexList.Add(tempPos.ConvertInVector2());
                GameEntry.CarPlacement.SetState(tempPos, PlacementCellState.Filled);
            }
            //m_CarMoveData.CalculatePosition();
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0,
                m_CarMoveData.DirectionType == ECarDirectionType.HORIZONTAL ? 0 : 90f));
            transform.localPosition = DefaultPos;
            //DefaultPos = transform.localPosition;
            //RefreshLimitValue();
        }
        
        /// <summary>
        /// 矫正到正确的位置
        /// </summary>
        public void CorrectPos()
        {
            m_CurrentIndexList.Sort(mSort);

            if (m_CarMoveData.IsCorrect)
            {
                float tempLeft = 0;
                float tempRight = 0;
                int tempListCount = m_CurrentIndexList.Count;
                Vector3 tempLocalPos = transform.localPosition;
                if (m_CarMoveData.DirectionType == ECarDirectionType.HORIZONTAL)
                {
                    tempLeft = Mathf.Abs(tempLocalPos.x -
                                         GameEntry.CarPlacement.GetCellLocalPos(m_CurrentIndexList[0]).x);
                    tempRight = Mathf.Abs(tempLocalPos.x -
                                          GameEntry.CarPlacement.GetCellLocalPos(m_CurrentIndexList[tempListCount - 1])
                                              .x);
                }
                else
                {
                    tempLeft = Mathf.Abs(tempLocalPos.y -
                                         GameEntry.CarPlacement.GetCellLocalPos(m_CurrentIndexList[0]).y);
                    tempRight = Mathf.Abs(tempLocalPos.y -
                                          GameEntry.CarPlacement.GetCellLocalPos(m_CurrentIndexList[tempListCount - 1])
                                              .y);
                }

                if (tempLeft < tempRight)
                {
                    GameEntry.CarPlacement.SetState(m_CurrentIndexList[tempListCount - 1], PlacementCellState.Empty);
                    m_CurrentIndexList.RemoveAt(tempListCount - 1);
                }
                else
                {
                    GameEntry.CarPlacement.SetState(m_CurrentIndexList[0], PlacementCellState.Empty);
                    m_CurrentIndexList.RemoveAt(0);
                }

                Log.Error($"{m_CurrentIndexList.Count} =={tempLeft} : {tempRight}");
            }

            transform.localPosition = m_CarMoveData.CalculateLocalPosition();
        }

        private Comparison<IntVector2> mSort = (left, right) =>
        {
            return IntVector2.GetKey(left).CompareTo(IntVector2.GetKey(right));
        };

        public void RefreshLimitValue()
        {
            m_CurrentIndexList.Sort(mSort);

            int width = GameEntry.CarPlacement.dimensions.x; //6
            float size = GameEntry.CarPlacement.gridSize; //200

            int tempMaxRight = 0;
            int tempMaxLeft = 0;
            int tempInvariantValue = 0;
            float tempDefaultValue = 0;
            
            int tempListCount = m_CurrentIndexList.Count;
            
            if (m_CarMoveData.DirectionType == ECarDirectionType.HORIZONTAL)
            {
                tempDefaultValue = transform.localPosition.x;
                tempInvariantValue = m_CurrentIndexList[0].y;
                tempMaxRight = m_CurrentIndexList[tempListCount - 1].x;
                tempMaxLeft = m_CurrentIndexList[0].x;
            }
            else
            {
                tempDefaultValue = transform.localPosition.y;
                tempInvariantValue = m_CurrentIndexList[0].x;
                tempMaxRight = m_CurrentIndexList[tempListCount - 1].y;
                tempMaxLeft = m_CurrentIndexList[0].y;
            }

            int tempMax = 0;
            int tempMin = 0;

            for (int i = tempMaxLeft - 1; i >= 0; i--)
            {
                if (m_CarMoveData.DirectionType == ECarDirectionType.HORIZONTAL)
                {
                    if (GameEntry.CarPlacement.AllCells[i, tempInvariantValue].CellState == PlacementCellState.Filled)
                    {
                        break;
                    }
                }
                else
                {
                    if (GameEntry.CarPlacement.AllCells[tempInvariantValue, i].CellState == PlacementCellState.Filled)
                    {
                        break;
                    }
                }

                tempMin++;
            }

            for (int i = tempMaxRight + 1; i < width; i++)
            {
                if (m_CarMoveData.DirectionType == ECarDirectionType.HORIZONTAL)
                {
                    if (GameEntry.CarPlacement.AllCells[i, tempInvariantValue].CellState == PlacementCellState.Filled)
                    {
                        break;
                    }
                }
                else
                {
                    if (GameEntry.CarPlacement.AllCells[tempInvariantValue, i].CellState == PlacementCellState.Filled)
                    {
                        break;
                    }
                }

                tempMax++;
            }

            Log.Debug($"D = {tempDefaultValue} Max:{size * tempMax} :Min = {size * tempMin}");

            MaxMoveValue = tempDefaultValue + (size * tempMax);
            MinMoveValue = tempDefaultValue - (size * tempMin);

            Log.Debug(
                $"{m_CarMoveData.EntityTbData.Id} :{m_CarMoveData.DirectionType} :Min = {MinMoveValue} Man = {MaxMoveValue}");
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            PlacementCell tempCell = col.gameObject.GetComponent<PlacementCell>();
            if (tempCell != null)
            {
                if (tempCell.CellState == PlacementCellState.Filled)
                {
                    return;
                }

                GameEntry.CarPlacement.SetState(tempCell.CellIndex, PlacementCellState.Filled);

                if (IsHaveCellIndex(tempCell.CellIndex) < 0)
                {
                    m_CurrentIndexList.Add(tempCell.CellIndex);
                }

                Log.Debug($"OnTriggerExit2D: {m_CurrentIndexList.Count}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlacementCell tempCell = other.gameObject.GetComponent<PlacementCell>();

            if (tempCell != null)
            {
                GameEntry.CarPlacement.SetState(tempCell.CellIndex, PlacementCellState.Empty);
                int tempIndex = IsHaveCellIndex(tempCell.CellIndex);
                if (tempIndex >= 0)
                {
                    m_CurrentIndexList.RemoveAt(tempIndex);
                }

                Log.Debug($"OnTriggerExit2D:{m_CurrentIndexList.Count}");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameEntry.Event.Fire(this, UICarCellClickEventArgs.Create(this));
        }

        private int IsHaveCellIndex(IntVector2 tempIndex)
        {
            for (int i = 0; i < m_CurrentIndexList.Count; i++)
            {
                IntVector2 tempListIndex = m_CurrentIndexList[i];

                if (tempListIndex.x == tempIndex.x && tempListIndex.y == tempIndex.y)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}