using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000581 RID: 1409
	public class NetworkActivation : NetworkBehaviour
	{
		// Token: 0x06001FC6 RID: 8134 RVA: 0x000171DA File Offset: 0x000153DA
		public void OnEnable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06001FC7 RID: 8135 RVA: 0x000171DA File Offset: 0x000153DA
		public void OnDisable()
		{
			base.SetDirtyBit(1u);
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x000171E3 File Offset: 0x000153E3
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(base.gameObject.activeSelf);
			return true;
		}

		// Token: 0x06001FC9 RID: 8137 RVA: 0x000171F7 File Offset: 0x000153F7
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.gameObject.SetActive(reader.ReadBoolean());
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}
	}
}
