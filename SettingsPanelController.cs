using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000630 RID: 1584
	public class SettingsPanelController : MonoBehaviour
	{
		// Token: 0x06002394 RID: 9108 RVA: 0x00019F31 File Offset: 0x00018131
		private void Start()
		{
			this.settingsControllers = base.GetComponentsInChildren<BaseSettingsControl>();
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x000AA0D8 File Offset: 0x000A82D8
		private void Update()
		{
			bool interactable = false;
			for (int i = 0; i < this.settingsControllers.Length; i++)
			{
				if (this.settingsControllers[i].hasBeenChanged)
				{
					interactable = true;
				}
			}
			this.revertButton.interactable = interactable;
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x000AA118 File Offset: 0x000A8318
		public void RevertChanges()
		{
			if (base.isActiveAndEnabled)
			{
				for (int i = 0; i < this.settingsControllers.Length; i++)
				{
					this.settingsControllers[i].Revert();
				}
			}
		}

		// Token: 0x04002674 RID: 9844
		[FormerlySerializedAs("carouselControllers")]
		private BaseSettingsControl[] settingsControllers;

		// Token: 0x04002675 RID: 9845
		public MPButton revertButton;
	}
}
