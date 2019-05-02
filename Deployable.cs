using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002CD RID: 717
	public class Deployable : MonoBehaviour
	{
		// Token: 0x06000E7E RID: 3710 RVA: 0x0000B2D6 File Offset: 0x000094D6
		private void OnDestroy()
		{
			if (NetworkServer.active && this.ownerMaster)
			{
				this.ownerMaster.RemoveDeployable(this);
			}
		}

		// Token: 0x04001276 RID: 4726
		[NonSerialized]
		public CharacterMaster ownerMaster;

		// Token: 0x04001277 RID: 4727
		public UnityEvent onUndeploy;
	}
}
