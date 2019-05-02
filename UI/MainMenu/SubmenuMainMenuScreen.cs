using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000675 RID: 1653
	public class SubmenuMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x06002516 RID: 9494 RVA: 0x0001B079 File Offset: 0x00019279
		public override void OnEnter(MainMenuController mainMenuController)
		{
			base.OnEnter(mainMenuController);
			this.submenuPanelInstance = UnityEngine.Object.Instantiate<GameObject>(this.submenuPanelPrefab, base.transform);
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x0001B099 File Offset: 0x00019299
		public override void OnExit(MainMenuController mainMenuController)
		{
			UnityEngine.Object.Destroy(this.submenuPanelInstance);
			base.OnExit(mainMenuController);
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x0001B0AD File Offset: 0x000192AD
		public void Update()
		{
			if (!this.submenuPanelInstance && this.myMainMenuController)
			{
				this.myMainMenuController.SetDesiredMenuScreen(this.myMainMenuController.titleMenuScreen);
			}
		}

		// Token: 0x040027D6 RID: 10198
		[FormerlySerializedAs("settingsPanelPrefab")]
		public GameObject submenuPanelPrefab;

		// Token: 0x040027D7 RID: 10199
		private GameObject submenuPanelInstance;
	}
}
