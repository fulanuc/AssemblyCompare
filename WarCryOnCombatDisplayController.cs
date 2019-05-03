using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200041F RID: 1055
	public class WarCryOnCombatDisplayController : MonoBehaviour
	{
		// Token: 0x06001780 RID: 6016 RVA: 0x0007A9A4 File Offset: 0x00078BA4
		public void Start()
		{
			CharacterModel component = base.transform.root.gameObject.GetComponent<CharacterModel>();
			if (component)
			{
				this.body = component.body;
			}
			this.UpdateReadyIndicator();
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00011929 File Offset: 0x0000FB29
		public void FixedUpdate()
		{
			this.UpdateReadyIndicator();
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0007A9E4 File Offset: 0x00078BE4
		private void UpdateReadyIndicator()
		{
			bool active = this.body && this.body.warCryReady;
			this.readyIndicator.SetActive(active);
		}

		// Token: 0x04001A9A RID: 6810
		private CharacterBody body;

		// Token: 0x04001A9B RID: 6811
		[Tooltip("The child gameobject to enable when the warcry is ready.")]
		public GameObject readyIndicator;
	}
}
