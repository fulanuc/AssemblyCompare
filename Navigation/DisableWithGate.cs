using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200051D RID: 1309
	public class DisableWithGate : MonoBehaviour
	{
		// Token: 0x06001D86 RID: 7558 RVA: 0x00015A6D File Offset: 0x00013C6D
		private void Awake()
		{
			if (SceneInfo.instance && SceneInfo.instance.groundNodes.IsGateOpen(this.gateToMatch) == this.invert)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04001FBF RID: 8127
		public string gateToMatch;

		// Token: 0x04001FC0 RID: 8128
		public bool invert;
	}
}
