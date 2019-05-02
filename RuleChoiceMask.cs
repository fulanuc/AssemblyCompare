using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200047A RID: 1146
	public class RuleChoiceMask : SerializableBitArray
	{
		// Token: 0x0600199F RID: 6559 RVA: 0x00012FF0 File Offset: 0x000111F0
		public RuleChoiceMask() : base(RuleCatalog.choiceCount)
		{
		}

		// Token: 0x1700025E RID: 606
		public bool this[RuleChoiceDef choiceDef]
		{
			get
			{
				return base[choiceDef.globalIndex];
			}
			set
			{
				base[choiceDef.globalIndex] = value;
			}
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x00083F6C File Offset: 0x0008216C
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x00083F9C File Offset: 0x0008219C
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x00084070 File Offset: 0x00082270
		public override bool Equals(object obj)
		{
			RuleChoiceMask ruleChoiceMask = obj as RuleChoiceMask;
			if (ruleChoiceMask != null)
			{
				for (int i = 0; i < this.bytes.Length; i++)
				{
					if (this.bytes[i] != ruleChoiceMask.bytes[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x0008400C File Offset: 0x0008220C
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x0008403C File Offset: 0x0008223C
		public void Copy([NotNull] RuleChoiceMask src)
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
