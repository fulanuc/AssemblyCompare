using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000663 RID: 1635
	public class SubmenuMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x0600247F RID: 9343 RVA: 0x0001A946 File Offset: 0x00018B46
		public override void OnEnter(MainMenuController mainMenuController)
		{
			base.OnEnter(mainMenuController);
			this.submenuPanelInstance = UnityEngine.Object.Instantiate<GameObject>(this.submenuPanelPrefab, base.transform);
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x0001A966 File Offset: 0x00018B66
		public override void OnExit(MainMenuController mainMenuController)
		{
			UnityEngine.Object.Destroy(this.submenuPanelInstance);
			base.OnExit(mainMenuController);
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x0001A97A File Offset: 0x00018B7A
		public void Update()
		{
			if (!this.submenuPanelInstance && this.myMainMenuController)
			{
				this.myMainMenuController.SetDesiredMenuScreen(this.myMainMenuController.titleMenuScreen);
			}
		}

		// Token: 0x0400277A RID: 10106
		[FormerlySerializedAs("settingsPanelPrefab")]
		public GameObject submenuPanelPrefab;

		// Token: 0x0400277B RID: 10107
		private GameObject submenuPanelInstance;
	}
}
