using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UICarMoveMenuForm
	{
		private UIButtonSuper m_Btn_StartGame;
		private TextMeshProUGUI m_TxtM_StartGameTxtM;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_StartGame = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_TxtM_StartGameTxtM = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
		}
	}
}
