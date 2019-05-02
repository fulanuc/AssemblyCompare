using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000371 RID: 881
	public class NetworkRuleChoiceMask : NetworkBehaviour
	{
		// Token: 0x06001238 RID: 4664 RVA: 0x0000DE4C File Offset: 0x0000C04C
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

		// Token: 0x06001239 RID: 4665 RVA: 0x000687EC File Offset: 0x000669EC
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

		// Token: 0x0600123A RID: 4666 RVA: 0x0000DE85 File Offset: 0x0000C085
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleChoiceMask(this.ruleChoiceMask);
			}
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x04001629 RID: 5673
		public readonly RuleChoiceMask ruleChoiceMask = new RuleChoiceMask();

		// Token: 0x0400162A RID: 5674
		private const uint maskDirtyBit = 1u;
	}
}
