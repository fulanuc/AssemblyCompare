using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002AD RID: 685
	public struct ConCommandArgs
	{
		// Token: 0x06000DE7 RID: 3559 RVA: 0x0000AC34 File Offset: 0x00008E34
		public void CheckArgumentCount(int count)
		{
			ConCommandException.CheckArgumentCount(this.userArgs, count);
		}

		// Token: 0x17000130 RID: 304
		public string this[int i]
		{
			get
			{
				return this.userArgs[i];
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x0000AC50 File Offset: 0x00008E50
		public int Count
		{
			get
			{
				return this.userArgs.Count;
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x0000AC5D File Offset: 0x00008E5D
		public GameObject senderMasterObject
		{
			get
			{
				if (!this.sender)
				{
					return null;
				}
				return this.sender.masterObject;
			}
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00056510 File Offset: 0x00054710
		public ulong? TryGetArgUlong(int index)
		{
			ulong value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new ulong?(value);
			}
			return null;
		}

		// Token: 0x06000DEC RID: 3564 RVA: 0x00056550 File Offset: 0x00054750
		public ulong GetArgULong(int index)
		{
			ulong? num = this.TryGetArgUlong(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an unsigned integer.", index));
			}
			return num.Value;
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x0005658C File Offset: 0x0005478C
		public int? TryGetArgInt(int index)
		{
			int value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new int?(value);
			}
			return null;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x000565CC File Offset: 0x000547CC
		public int GetArgInt(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be an integer.", index));
			}
			return num.Value;
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00056608 File Offset: 0x00054808
		public bool? TryGetArgBool(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num != null)
			{
				int? num2 = num;
				int num3 = 0;
				return new bool?(num2.GetValueOrDefault() > num3 & num2 != null);
			}
			return null;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x0005664C File Offset: 0x0005484C
		public bool GetArgBool(int index)
		{
			int? num = this.TryGetArgInt(index);
			if (num == null)
			{
				throw new ConCommandException(string.Format("Argument {0} must be a boolean.", index));
			}
			return num.Value > 0;
		}

		// Token: 0x040011EC RID: 4588
		public List<string> userArgs;

		// Token: 0x040011ED RID: 4589
		public NetworkUser sender;

		// Token: 0x040011EE RID: 4590
		public string commandName;
	}
}
