using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x02000642 RID: 1602
	public class SettingsPanelController : MonoBehaviour
	{
		// Token: 0x06002424 RID: 9252 RVA: 0x0001A5FF File Offset: 0x000187FF
		private void Start()
		{
			this.settingsControllers = base.GetComponentsInChildren<BaseSettingsControl>();
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x000AB754 File Offset: 0x000A9954
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

		// Token: 0x06002426 RID: 9254 RVA: 0x000AB794 File Offset: 0x000A9994
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

		// Token: 0x040026CF RID: 9935
		[FormerlySerializedAs("carouselControllers")]
		private BaseSettingsControl[] settingsControllers;

		// Token: 0x040026D0 RID: 9936
		public MPButton revertButton;
	}
}
