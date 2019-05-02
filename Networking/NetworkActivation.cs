using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000594 RID: 1428
	public class NetworkActivation : NetworkBehaviour
	{
		// Token: 0x06002057 RID: 8279 RVA: 0x000178EA File Offset: 0x00015AEA
		public void OnEnable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x000178EA File Offset: 0x00015AEA
		public void OnDisable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06002059 RID: 8281 RVA: 0x000178F3 File Offset: 0x00015AF3
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(base.gameObject.activeSelf);
			return true;
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x00017907 File Offset: 0x00015B07
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.gameObject.SetActive(reader.ReadBoolean());
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}
	}
}
