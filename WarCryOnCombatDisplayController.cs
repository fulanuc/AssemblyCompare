using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000425 RID: 1061
	public class WarCryOnCombatDisplayController : MonoBehaviour
	{
		// Token: 0x060017C3 RID: 6083 RVA: 0x0007AF64 File Offset: 0x00079164
		public void Start()
		{
			CharacterModel component = base.transform.root.gameObject.GetComponent<CharacterModel>();
			if (component)
			{
				this.body = component.body;
			}
			this.UpdateReadyIndicator();
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00011D55 File Offset: 0x0000FF55
		public void FixedUpdate()
		{
			this.UpdateReadyIndicator();
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x0007AFA4 File Offset: 0x000791A4
		private void UpdateReadyIndicator()
		{
			bool active = this.body && this.body.warCryReady;
			this.readyIndicator.SetActive(active);
		}

		// Token: 0x04001AC3 RID: 6851
		private CharacterBody body;

		// Token: 0x04001AC4 RID: 6852
		[Tooltip("The child gameobject to enable when the warcry is ready.")]
		public GameObject readyIndicator;
	}
}
