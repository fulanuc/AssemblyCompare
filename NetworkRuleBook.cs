using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000370 RID: 880
	public class NetworkRuleBook : NetworkBehaviour
	{
		// Token: 0x06001233 RID: 4659 RVA: 0x0000DDE8 File Offset: 0x0000BFE8
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

		// Token: 0x06001234 RID: 4660 RVA: 0x000687AC File Offset: 0x000669AC
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

		// Token: 0x06001235 RID: 4661 RVA: 0x0000DE21 File Offset: 0x0000C021
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				reader.ReadRuleBook(this.ruleBook);
			}
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x04001627 RID: 5671
		public readonly RuleBook ruleBook = new RuleBook();

		// Token: 0x04001628 RID: 5672
		private const uint ruleBookDirtyBit = 1u;
	}
}
