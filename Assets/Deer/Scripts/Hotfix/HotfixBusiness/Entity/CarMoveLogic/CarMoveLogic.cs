using System;
using cfg.Deer.Enum;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.Entity
{
    
    public class CarMoveLogic : EntityLogicBase
    {
        private CarMoveData m_CarMoveData;
        public CarMoveData CarMoveData { get { return m_CarMoveData; }}
        private Camera m_Camera;
        private Vector3 m_DefaultPos = Vector3.zero;
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_Camera = Camera.main;
        }

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
            m_DefaultPos = CachedTransform.position;
        }

        
        private void OnTriggerEnter2D(Collider2D col)
        {
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            
        }

        private Vector3 mMouseLastPos = Vector3.zero;
        private bool mMouseDown = false;
        private void Update()
        {
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
                    if (m_CarMoveData.MoveType == CarMoveType.HORIZONTAL)
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
                        }else if (tempLeftRay)
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
                        CachedTransform.position = new Vector3(CachedTransform.position.x + tempLimitOffset,m_DefaultPos.y,0);
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
                        }else if (tempDownRay)
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
                        CachedTransform.position = new Vector3(m_DefaultPos.x,CachedTransform.position.y + tempLimitOffset,0);
                    }
                }

                mMouseLastPos = tempPos;
            }
        }

        private RaycastHit2D SendRay(Vector3 tempOffset,Vector2 tempDirection,float distance)
        {
            Vector3 tempPos = CachedTransform.position + tempOffset;
            RaycastHit2D tempRayHit =
                Physics2D.Raycast(tempPos, tempDirection, distance,1 << LayerMask.NameToLayer("Move"));
            Debug.DrawRay(tempPos,tempDirection,tempRayHit ? Color.red : Color.green);
            if (tempDirection == Vector2.left || tempDirection == Vector2.right)
            {
                Debug.DrawLine(new Vector3(tempPos.x, tempPos.y + 0.2f, 0),
                    new Vector3(tempPos.x, tempPos.y + 0.2f, 0));
            }
            else
            {
                Debug.DrawLine(new Vector3(tempPos.x + 0.2f, tempPos.y , 0),
                    new Vector3(tempPos.x + 0.2f, tempPos.y , 0));
            }

            return tempRayHit;
        }
    }

    enum MouseDirectionType
    {
        None = 0,
        Up,
        Down,
        Left,
        Right,
    }
}