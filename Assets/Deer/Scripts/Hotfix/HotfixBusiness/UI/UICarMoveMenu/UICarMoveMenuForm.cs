// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-02-26 23-56-08
//修改作者:AlanDu
//修改时间:2023-02-27 00-00-51
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UICarMoveMenuForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			 m_Btn_StartGame.onClick.AddListener(Btn_StartGameEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		private void Btn_StartGameEvent()
		{
			
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
