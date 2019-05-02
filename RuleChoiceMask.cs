using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000487 RID: 1159
	public class RuleChoiceMask : SerializableBitArray
	{
		// Token: 0x06001A01 RID: 6657 RVA: 0x0001351E File Offset: 0x0001171E
		public RuleChoiceMask() : base(RuleCatalog.choiceCount)
		{
		}

		// Token: 0x1700026A RID: 618
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

		// Token: 0x06001A04 RID: 6660 RVA: 0x000849D8 File Offset: 0x00082BD8
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x00084A08 File Offset: 0x00082C08
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x00084ADC File Offset: 0x00082CDC
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

		// Token: 0x06001A07 RID: 6663 RVA: 0x00084A78 File Offset: 0x00082C78
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x00084AA8 File Offset: 0x00082CA8
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
