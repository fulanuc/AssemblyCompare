using System;

namespace RoR2
{
	// Token: 0x020004C6 RID: 1222
	[Serializable]
	public struct UnlockableIndex : IEquatable<UnlockableIndex>, IComparable<UnlockableIndex>
	{
		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06001B8D RID: 7053 RVA: 0x000146B4 File Offset: 0x000128B4
		public int value
		{
			get
			{
				return (int)(this.internalValue - 1u);
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06001B8E RID: 7054 RVA: 0x000146BE File Offset: 0x000128BE
		public bool isValid
		{
			get
			{
				return this.internalValue > 0u;
			}
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000146C9 File Offset: 0x000128C9
		public UnlockableIndex(int value)
		{
			this.internalValue = (uint)(value + 1);
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000146D4 File Offset: 0x000128D4
		public override bool Equals(object obj)
		{
			return obj is UnlockableIndex && this.Equals((UnlockableIndex)obj);
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x000146EC File Offset: 0x000128EC
		public bool Equals(UnlockableIndex other)
		{
			return this.internalValue.Equals(other.internalValue);
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x000146FF File Offset: 0x000128FF
		public int CompareTo(UnlockableIndex other)
		{
			return this.internalValue.CompareTo(other.internalValue);
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x00014712 File Offset: 0x00012912
		public override int GetHashCode()
		{
			return this.internalValue.GetHashCode();
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x0001471F File Offset: 0x0001291F
		public static bool operator ==(UnlockableIndex a, UnlockableIndex b)
		{
			return a.internalValue == b.internalValue;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x0001472F File Offset: 0x0001292F
		public static bool operator !=(UnlockableIndex a, UnlockableIndex b)
		{
			return !(a == b);
		}

		// Token: 0x04001E08 RID: 7688
		public uint internalValue;
	}
}
