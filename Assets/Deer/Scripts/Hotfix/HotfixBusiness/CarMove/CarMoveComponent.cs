using System.Collections;
using System.Collections.Generic;
using cfg.Deer;
using cfg.Deer.Enum;
using GameFramework.Event;
using GameFramework.ObjectPool;
using HotfixBusiness.Entity;
using HotfixFramework.Runtime;
using Main.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

public class CarMoveComponent : MonoBehaviour
{
    private IObjectPool<UICarItemObject> m_UICarItemObjectPool = null;
    private List<UICarItem> m_UICarItems = new List<UICarItem>();
    private List<RaycastResult> m_UIRaycastResultCache = new List<RaycastResult>();
    
    private Camera m_Camera;
    private UICarItem mSelectItem = null;
    private ECarDirectionType m_DirectionType = ECarDirectionType.HORIZONTAL;
    private RectTransform m_CarItemParentRect;
    private void Awake()
    {
        GameObject goCamera = GameObject.FindGameObjectWithTag("UICamera");
        m_Camera = goCamera.GetComponent<Camera>();
        m_UICarItemObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<UICarItemObject>("UICarItem", 36);
        m_CarItemParentRect = GameEntry.CarPlacement.MParent.GetComponent<RectTransform>();
    }

    private Vector3 mRayOffset = Vector3.zero;
    private Vector3 mMouseLastPos = Vector3.zero;
    private bool mMouseDown = false;
    //private Vector3 m_DefaultPos = Vector3.zero;
    //private Transform m_SelectTransform;
    public GameObject GetFirstPickGameObject(Vector2 position)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;
        //射线检测ui
        eventSystem.RaycastAll(pointerEventData, m_UIRaycastResultCache);
        foreach (var tempObj in m_UIRaycastResultCache)
        {
            int tempLayer = LayerMask.NameToLayer("CarMoveItem");
            Log.Debug($"tempLayer = {tempLayer}");
            if (tempObj.gameObject.layer == tempLayer && 
                tempObj.gameObject.GetComponent<UICarItem>() != null)
            {
                return tempObj.gameObject;
            }
        }
        if (m_UIRaycastResultCache.Count > 0)
            return m_UIRaycastResultCache[0].gameObject;
        return null;
    }

    private void Update()
    {
        // if (m_SelectTransform == null)
        // {
        //     return;
        // }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit tempHit;
            Ray tempRay = new Ray(Input.mousePosition, Vector3.forward);
            Debug.DrawLine(Input.mousePosition,new Vector3(Input.mousePosition.x,Input.mousePosition.y,Input.mousePosition.z + 5) ,Color.red);
            GameObject tempObj = GetFirstPickGameObject(Input.mousePosition);
            if (tempObj != null)
            {
                UICarItem tempItem = tempObj.GetComponent<UICarItem>();
                if (tempItem != null)
                {
                    mSelectItem = tempItem;
                    //mRayOffset = mSelectItem.CarMoveData.EntityTbData.RayOffset;
                    m_DirectionType = tempItem.CarMoveData.DirectionType;
                    //m_DefaultPos = tempItem.CarMoveData.DefaultPos;
                }
            }
            mMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mMouseDown = false;
            mMouseLastPos = Vector3.zero;
            if (mSelectItem != null)
            {
                mSelectItem.CorrectPos();
                foreach (var tempItem in m_UICarItems)
                {
                    tempItem.RefreshLimitValue();
                }
            }
        }

        if (mSelectItem == null)
        {
            return;
        }
        // Vector3 temp = mSelectItem.transform.position;
        // Debug.DrawLine(new Vector3(temp.x + Offset.x, temp.y, 0),
        //     new Vector3(temp.x + Offset.x + Distance, temp.y, 0),Color.blue);
        //
        // Debug.DrawLine(new Vector3(temp.x - Offset.x, temp.y, 0),
        //     new Vector3(temp.x - Offset.x - Distance, temp.y, 0),Color.blue);
        if (mMouseDown)
        {
            //Vector3 tempPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 tempPos1 = Vector3.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CarItemParentRect,Input.mousePosition,m_Camera,out tempPos1);
            if (mMouseLastPos != Vector3.zero)
            {
                //Vector3 tempMoveOffset = tempPos - mMouseLastPos;
                
            }
            float tempOffset = 0;
                
            if (m_DirectionType == ECarDirectionType.HORIZONTAL)
            {
                tempOffset = Mathf.Clamp(tempPos1.x, mSelectItem.MinMoveValue,
                    mSelectItem.MaxMoveValue);
                mSelectItem.transform.localPosition =
                    new Vector3(tempOffset, mSelectItem.DefaultPos.y,0);
                //mSelectItem.transform.localPosition = new Vector3(mSelectItem.transform.localPosition.x, mSelectItem.transform.localPosition.y, 0);
            }
            else
            {
                tempOffset = Mathf.Clamp(tempPos1.y, mSelectItem.MinMoveValue,
                    mSelectItem.MaxMoveValue);
                mSelectItem.transform.localPosition =
                    new Vector3(mSelectItem.DefaultPos.x, tempOffset,0);
                //mSelectItem.transform.localPosition = new Vector3(mSelectItem.transform.localPosition.x, tempPos1.y, 0);
            }
            mMouseLastPos = tempPos1;
        }
    }

    // public Vector3 Offset = Vector3.zero;
    // public float Distance = 5f;
    // private RaycastHit2D SendRay(Vector3 tempOffset, Vector2 tempDirection, float distance)
    // {
    //     Vector3 tempPos = Vector3.zero;
    //     float tempDistance = 0;
    //     if (tempDirection == Vector2.left || tempDirection == Vector2.down)
    //     {
    //         tempDistance = -Distance;
    //         tempPos = mSelectItem.transform.position - Offset;
    //     }
    //     else
    //     {
    //         tempDistance = Distance;
    //         tempPos = mSelectItem.transform.position + Offset;
    //     }
    //
    //     RaycastHit2D tempRayHit =
    //         Physics2D.Raycast(tempPos, tempDirection,tempDistance, 1 << LayerMask.NameToLayer("CarMoveItem"));
    //     Debug.DrawRay(tempPos, tempDirection, tempRayHit ? Color.red : Color.green);
    //     if (tempDirection == Vector2.left || tempDirection == Vector2.right)
    //     {
    //         Debug.DrawLine(new Vector3(tempPos.x, tempPos.y + 0.1f, 0),
    //             new Vector3(tempPos.x + tempDistance, tempPos.y + 0.1f, 0),tempRayHit ? Color.red : Color.blue);
    //     }
    //     else
    //     {
    //         Debug.DrawLine(new Vector3(tempPos.x + 0.2f, tempPos.y, 0),
    //             new Vector3(tempPos.x + 0.2f, tempPos.y + tempDistance, 0),tempRayHit ? Color.red : Color.blue);
    //     }
    //
    //     return tempRayHit;
    // }
    //
    // public void SetSelectTransform(UICarItem tempCarLogic)
    // {
    //     m_SelectTransform = tempCarLogic.transform;
    // }
    public UICarItem CreateHPBarItem(CarMoveData tempData,int tempCount)
    {
        UICarItem hpBarItem = null;
        UICarItemObject hpBarItemObject = m_UICarItemObjectPool.Spawn();
        if (hpBarItemObject != null)
        {
            hpBarItem = (UICarItem)hpBarItemObject.Target;
        }
        else
        {
            AssetObjectComponent tempObj = GameEntry.AssetObject;
            if (tempObj == null)
            {
                Log.Error("tempObj");
                return null;
            }
            GameEntry.AssetObject.LoadGameObject(GameEntry.AssetObject.GenSerialId(),AssetUtility.Entity.GetEntityAsset(tempData.EntityTbData.AssetPath)
                ,"ShootText",
                delegate(bool success, object obj, int serial)
                {
                    if (success)
                    {
                        GameObject gameObject = (GameObject) obj;
                        Transform transform = gameObject.GetComponent<Transform>();
                        transform.SetParent(GameEntry.CarPlacement.MParent);
                        transform.localScale = Vector3.one;
                        hpBarItem = gameObject.AddComponent<UICarItem>();
                        hpBarItem.Init(tempData);
                        Vector3 tempPos = hpBarItem.transform.localPosition;
                        hpBarItem.transform.localPosition = new Vector3(tempPos.x, tempPos.y, 0);
                        //Logger.Debug($"DefaultPos111 = {hpBarItem.transform.localPosition}");
                        m_UICarItems.Add(hpBarItem);
                        if (m_UICarItems.Count >= tempCount)
                        {
                            foreach (var tempItem in m_UICarItems)
                            {
                                tempItem.RefreshLimitValue();
                            }
                        }
                        m_UICarItemObjectPool.Register(UICarItemObject.Create(hpBarItem), true);
                        //gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                });;
        }

        return hpBarItem;
    }
}