using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036D RID: 877
	public class NetworkRuleBook : NetworkBehaviour
	{
		// Token: 0x0600121C RID: 4636 RVA: 0x0000DCFF File Offset: 0x0000BEFF
		[Server]
		public void SetRuleBook([NotNull] RuleBook newRuleBook)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkRuleBook::SetRuleBook(RoR2.RuleBook)' called on client");
				return;
			}
			if (this.ruleBook.Equals(newRuleBook))
			{
				return;
			}
			base.SetDirtyBit(1u);
			this.ruleBook.Copy(newRuleBook);
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00068474 File Offset: 0x00066674
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
				writer.Write(this.ruleBook);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0000DD38 File Offset: 0x0000BF38
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleBook(this.ruleBook);
			}
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0400160E RID: 5646
		public readonly RuleBook ruleBook = new RuleBook();

		// Token: 0x0400160F RID: 5647
		private const uint ruleBookDirtyBit = 1u;
	}
}
