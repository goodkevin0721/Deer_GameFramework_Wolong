using System.Collections;
using System.Collections.Generic;
using cfg.Deer;
using cfg.Deer.Enum;
using HotfixBusiness.Entity;
using UnityEngine;
using UnityGameFramework.Runtime;

public class CarMoveComponent : GameFrameworkComponent
{
    // private CarMoveLogic m_CarMoveData;
    // public CarMoveLogic CarMoveData { get { return m_CarMoveData; }}
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
    }

    // Update is called once per frame
    private Vector3 mMouseLastPos = Vector3.zero;
    private bool mMouseDown = false;
    private Camera m_Camera;
    private Vector3 m_DefaultPos = Vector3.zero;
    private CarMoveType m_MoveType = CarMoveType.HORIZONTAL;
    private Transform m_SelectTransform;

    private void Update()
    {
        if (m_SelectTransform == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mMouseDown = false;
            mMouseLastPos = Vector3.zero;
        }

        if (mMouseDown)
        {
            Vector3 tempPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);

            if (mMouseLastPos != Vector3.zero)
            {
                Vector3 tempMoveOffset = tempPos - mMouseLastPos;
                float tempLimitOffset = 0;
                if (m_MoveType == CarMoveType.HORIZONTAL)
                {
                    RaycastHit2D tempRightRay = SendRay(tempMoveOffset, Vector2.right, 0.2f);
                    RaycastHit2D tempLeftRay = SendRay(tempMoveOffset, Vector2.left, 0.2f);
                    if (tempRightRay)
                    {
                        if (tempMoveOffset.x < 0)
                        {
                            tempLimitOffset = tempMoveOffset.x;
                        }
                        else
                        {
                            tempLimitOffset = 0;
                        }
                    }
                    else if (tempLeftRay)
                    {
                        if (tempMoveOffset.x > 0)
                        {
                            tempLimitOffset = tempMoveOffset.x;
                        }
                        else
                        {
                            tempLimitOffset = 0;
                        }
                    }

                    m_SelectTransform.position =
                        new Vector3(m_SelectTransform.position.x + tempLimitOffset, m_DefaultPos.y, 0);
                }
                else
                {
                    RaycastHit2D tempUpRay = SendRay(tempMoveOffset, Vector2.up, 0.2f);
                    RaycastHit2D tempDownRay = SendRay(tempMoveOffset, Vector2.down, 0.2f);
                    if (tempUpRay)
                    {
                        if (tempMoveOffset.y < 0)
                        {
                            tempLimitOffset = tempMoveOffset.y;
                        }
                        else
                        {
                            tempLimitOffset = 0;
                        }
                    }
                    else if (tempDownRay)
                    {
                        if (tempMoveOffset.y > 0)
                        {
                            tempLimitOffset = tempMoveOffset.y;
                        }
                        else
                        {
                            tempLimitOffset = 0;
                        }
                    }

                    m_SelectTransform.position =
                        new Vector3(m_DefaultPos.x, m_SelectTransform.position.y + tempLimitOffset, 0);
                }
            }

            mMouseLastPos = tempPos;
        }
    }

    private RaycastHit2D SendRay(Vector3 tempOffset, Vector2 tempDirection, float distance)
    {
        Vector3 tempPos = m_SelectTransform.position + tempOffset;
        RaycastHit2D tempRayHit =
            Physics2D.Raycast(tempPos, tempDirection, distance, 1 << LayerMask.NameToLayer("Move"));
        Debug.DrawRay(tempPos, tempDirection, tempRayHit ? Color.red : Color.green);
        if (tempDirection == Vector2.left || tempDirection == Vector2.right)
        {
            Debug.DrawLine(new Vector3(tempPos.x, tempPos.y + 0.2f, 0),
                new Vector3(tempPos.x, tempPos.y + 0.2f, 0));
        }
        else
        {
            Debug.DrawLine(new Vector3(tempPos.x + 0.2f, tempPos.y, 0),
                new Vector3(tempPos.x + 0.2f, tempPos.y, 0));
        }

        return tempRayHit;
    }

    public void SetSelectTransform(CarMoveLogic tempCarLogic)
    {
        m_SelectTransform = tempCarLogic.transform;
    }

    public void LoadLevelEntity(int level)
    {
        CarMoveLevelData tempLevelData = null;
        GameEntry.Config.Tables.TbCarMoveLevelData.DataMap.TryGetValue(level, out tempLevelData);
        foreach (var tempId in tempLevelData.CarEntityId)
        {
            CarEntityData tempEntityData = null;
            GameEntry.Config.Tables.TbCarEntityData.DataMap.TryGetValue(level, out tempEntityData);
            if (tempEntityData == null)
            {
                continue;
            }
            CarMoveData tempCarMoveData = new CarMoveData(GameEntry.Entity.GenEntityId(),tempId, tempEntityData.AssetPath);
            tempCarMoveData.IsOwner = true;
            GameEntry.Entity.ShowEntity(typeof(CarMoveLogic), "CarMoveEntity", 1, tempCarMoveData);
        }
    }
}