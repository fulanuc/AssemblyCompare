using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002AB RID: 683
	public struct ConCommandArgs
	{
		// Token: 0x06000DE0 RID: 3552 RVA: 0x0000ABE2 File Offset: 0x00008DE2
		public void CheckArgumentCount(int count)
		{
			ConCommandException.CheckArgumentCount(this.userArgs, count);
		}

		// Token: 0x1700012C RID: 300
		public string this[int i]
		{
			get
			{
				return this.userArgs[i];
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x0000ABFE File Offset: 0x00008DFE
		public int Count
		{
			get
			{
				return this.userArgs.Count;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x0000AC0B File Offset: 0x00008E0B
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

		// Token: 0x06000DE4 RID: 3556 RVA: 0x000565CC File Offset: 0x000547CC
		public ulong? TryGetArgUlong(int index)
		{
			ulong value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new ulong?(value);
			}
			return null;
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x0005660C File Offset: 0x0005480C
		public int? TryGetArgInt(int index)
		{
			int value;
			if (index < this.userArgs.Count && TextSerialization.TryParseInvariant(this.userArgs[index], out value))
			{
				return new int?(value);
			}
			return null;
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x0005664C File Offset: 0x0005484C
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

		// Token: 0x040011DA RID: 4570
		public List<string> userArgs;

		// Token: 0x040011DB RID: 4571
		public NetworkUser sender;

		// Token: 0x040011DC RID: 4572
		public string commandName;
	}
}
