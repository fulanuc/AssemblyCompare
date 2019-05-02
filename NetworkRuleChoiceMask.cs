using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036E RID: 878
	public class NetworkRuleChoiceMask : NetworkBehaviour
	{
		// Token: 0x06001221 RID: 4641 RVA: 0x0000DD63 File Offset: 0x0000BF63
		[Server]
		public void SetRuleChoiceMask([NotNull] RuleChoiceMask newRuleChoiceMask)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkRuleChoiceMask::SetRuleChoiceMask(RoR2.RuleChoiceMask)' called on client");
				return;
			}
			if (this.ruleChoiceMask.Equals(newRuleChoiceMask))
			{
				return;
			}
			base.SetDirtyBit(1u);
			this.ruleChoiceMask.Copy(newRuleChoiceMask);
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x000684B4 File Offset: 0x000666B4
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1u;
			}
			bool flag = (num & 1u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.ruleChoiceMask);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0000DD9C File Offset: 0x0000BF9C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleChoiceMask(this.ruleChoiceMask);
			}
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x04001610 RID: 5648
		public readonly RuleChoiceMask ruleChoiceMask = new RuleChoiceMask();

		// Token: 0x04001611 RID: 5649
		private const uint maskDirtyBit = 1u;
	}
}
