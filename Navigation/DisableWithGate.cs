using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200052C RID: 1324
	public class DisableWithGate : MonoBehaviour
	{
		// Token: 0x06001DEE RID: 7662 RVA: 0x00015F36 File Offset: 0x00014136
		private void Awake()
		{
			if (SceneInfo.instance && SceneInfo.instance.groundNodes.IsGateOpen(this.gateToMatch) == this.invert)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001FFD RID: 8189
		public string gateToMatch;

		// Token: 0x04001FFE RID: 8190
		public bool invert;
	}
}
