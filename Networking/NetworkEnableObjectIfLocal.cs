using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000583 RID: 1411
	public class NetworkEnableObjectIfLocal : NetworkBehaviour
	{
		// Token: 0x06001F96 RID: 8086 RVA: 0x000171C2 File Offset: 0x000153C2
		private void Start()
		{
			if (this.target)
			{
				this.target.SetActive(base.hasAuthority);
			}
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x000171E2 File Offset: 0x000153E2
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			if (this.target)
			{
				this.target.SetActive(true);
			}
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x00017203 File Offset: 0x00015403
		public override void OnStopAuthority()
		{
			if (this.target)
			{
				this.target.SetActive(false);
			}
			base.OnStopAuthority();
		}

		// Token: 0x06001F9A RID: 8090 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400220A RID: 8714
		[Tooltip("The GameObject to enable/disable.")]
		public GameObject target;
	}
}
