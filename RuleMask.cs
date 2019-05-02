using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000479 RID: 1145
	public class RuleMask : SerializableBitArray
	{
		// Token: 0x06001999 RID: 6553 RVA: 0x00012FE3 File Offset: 0x000111E3
		public RuleMask() : base(RuleCatalog.ruleCount)
		{
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x00083F6C File Offset: 0x0008216C
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x00083F9C File Offset: 0x0008219C
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x00083FCC File Offset: 0x000821CC
		public override bool Equals(object obj)
		{
			RuleMask ruleMask = obj as RuleMask;
			if (ruleMask != null)
			{
				for (int i = 0; i < this.bytes.Length; i++)
				{
					if (this.bytes[i] != ruleMask.bytes[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0008400C File Offset: 0x0008220C
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0008403C File Offset: 0x0008223C
		public void Copy([NotNull] RuleMask src)
		{
			byte[] bytes = src.bytes;
			byte[] bytes2 = this.bytes;
			int i = 0;
			int num = bytes2.Length;
			while (i < num)
			{
				bytes2[i] = bytes[i];
				i++;
			}
		}
	}
}
