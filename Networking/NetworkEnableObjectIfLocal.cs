using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000574 RID: 1396
	public class NetworkEnableObjectIfLocal : NetworkBehaviour
	{
		// Token: 0x06001F2C RID: 7980 RVA: 0x00016CE3 File Offset: 0x00014EE3
		private void Start()
		{
			if (this.target)
			{
				this.target.SetActive(base.hasAuthority);
			}
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x00016D03 File Offset: 0x00014F03
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			if (this.target)
			{
				this.target.SetActive(true);
			}
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x00016D24 File Offset: 0x00014F24
		public override void OnStopAuthority()
		{
			if (this.target)
			{
				this.target.SetActive(false);
			}
			base.OnStopAuthority();
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040021CC RID: 8652
		[Tooltip("The GameObject to enable/disable.")]
		public GameObject target;
	}
}
