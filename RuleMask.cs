using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000486 RID: 1158
	public class RuleMask : SerializableBitArray
	{
		// Token: 0x060019FB RID: 6651 RVA: 0x00013511 File Offset: 0x00011711
		public RuleMask() : base(RuleCatalog.ruleCount)
		{
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x000849D8 File Offset: 0x00082BD8
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				writer.Write(this.bytes[i]);
			}
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x00084A08 File Offset: 0x00082C08
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.bytes[i] = reader.ReadByte();
			}
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x00084A38 File Offset: 0x00082C38
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

		// Token: 0x060019FF RID: 6655 RVA: 0x00084A78 File Offset: 0x00082C78
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num += (int)this.bytes[i];
			}
			return num;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x00084AA8 File Offset: 0x00082CA8
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
