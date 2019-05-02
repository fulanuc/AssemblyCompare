using System;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003FC RID: 1020
	public class TeamFilter : NetworkBehaviour
	{
		// Token: 0x17000208 RID: 520
		// (get) Token: 0x0600168D RID: 5773 RVA: 0x00010E54 File Offset: 0x0000F054
		// (set) Token: 0x0600168E RID: 5774 RVA: 0x00010E5D File Offset: 0x0000F05D
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

		// Token: 0x06001690 RID: 5776 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06001691 RID: 5777 RVA: 0x000771D0 File Offset: 0x000753D0
		// (set) Token: 0x06001692 RID: 5778 RVA: 0x00010E66 File Offset: 0x0000F066
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

		// Token: 0x06001693 RID: 5779 RVA: 0x000771E4 File Offset: 0x000753E4
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

		// Token: 0x06001694 RID: 5780 RVA: 0x00077250 File Offset: 0x00075450
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

		// Token: 0x040019BA RID: 6586
		[FormerlySerializedAs("teamIndex")]
		[SyncVar]
		private int teamIndexInternal;
	}
}
