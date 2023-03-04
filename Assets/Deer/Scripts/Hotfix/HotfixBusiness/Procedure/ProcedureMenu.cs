using cfg.Deer;
using HotfixBusiness.Entity;
using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using Main.Runtime;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

/// <summary>
/// ��ҳ�� ����
/// </summary>
public class ProcedureMenu : ProcedureBase
{
    private int? m_UIFormSerialId;

    private CarMoveComponent mCarMove = null;
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

		Debug.Log("tackor HotFix ProcedureMenu OnEnter");

		// GameEntry.CarPlacement.SetUpGrid();
		// if (mCarMove == null)
		// {
		// 	GameObject tempObj = new GameObject("MoveComponent");
		// 	tempObj.transform.SetParent(GameEntry.UI.GetInstanceRoot());
		// 	mCarMove = tempObj.AddComponent<CarMoveComponent>();
		// }
		// LoadLevelEntity(1);

		m_UIFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.UICarMoveMenuForm, this);
		GameEntry.Sound.PlayMusic((int)SoundId.MenuBGM);
	}
    public void LoadLevelEntity(int level)
    {
	    CarMoveLevelData tempLevelData = null;
	    GameEntry.Config.Tables.TbCarMoveLevelData.DataMap.TryGetValue(level, out tempLevelData);
	    foreach (var tempId in tempLevelData.CarEntityId)
	    {
		    CarEntityData tempEntityData = null;
		    GameEntry.Config.Tables.TbCarEntityData.DataMap.TryGetValue(tempId, out tempEntityData);
		    if (tempEntityData == null)	
		    {
			    continue;
		    }
		    
		    mCarMove.CreateHPBarItem(CarMoveData.Create(tempEntityData),tempLevelData.CarEntityId.Count);
	    }
    }
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

		//if (m_UIFormSerialId != 0)
		//{
		//    GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
		//}

		Debug.Log("tackor ProcedureMenu OnLeave ");
		GameEntry.UI.CloseAllLoadedUIForms();
	}

    public void PlayGame(int raceId, Vector3 playerPos)
    {
        GameEntry.Setting.SetInt("raceId", raceId);
        Debug.Log($"tackor ѡ�г���: {raceId} {playerPos}");

        m_ProcedureOwner.SetData<VarString>("nextProcedure", ProcedureEnum.ProcedureGamePlay.ToString());
        m_ProcedureOwner.SetData<VarInt16>("RaceId", (short)raceId);

        ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
	}

}