using System;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003F6 RID: 1014
	public class TeamFilter : NetworkBehaviour
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x0600164E RID: 5710 RVA: 0x00010A3B File Offset: 0x0000EC3B
		// (set) Token: 0x0600164F RID: 5711 RVA: 0x00010A44 File Offset: 0x0000EC44
		public TeamIndex teamIndex
		{
			get
			{
				return (TeamIndex)this.teamIndexInternal;
			}
			set
			{
				this.NetworkteamIndexInternal = (int)value;
			}
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x00076C40 File Offset: 0x00074E40
		// (set) Token: 0x06001653 RID: 5715 RVA: 0x00010A4D File Offset: 0x0000EC4D
		public int NetworkteamIndexInternal
		{
			get
			{
				return this.teamIndexInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.teamIndexInternal, 1u);
			}
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x00076C54 File Offset: 0x00074E54
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x00076CC0 File Offset: 0x00074EC0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.teamIndexInternal = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.teamIndexInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001991 RID: 6545
		[SyncVar]
		[FormerlySerializedAs("teamIndex")]
		private int teamIndexInternal;
	}
}
