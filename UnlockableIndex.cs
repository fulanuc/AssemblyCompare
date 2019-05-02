using System;

namespace RoR2
{
	// Token: 0x020004D4 RID: 1236
	[Serializable]
	public struct UnlockableIndex : IEquatable<UnlockableIndex>, IComparable<UnlockableIndex>
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06001BF1 RID: 7153 RVA: 0x00014B81 File Offset: 0x00012D81
		public int value
		{
			get
			{
				return (int)(this.internalValue - 1u);
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06001BF2 RID: 7154 RVA: 0x00014B8B File Offset: 0x00012D8B
		public bool isValid
		{
			get
			{
				return this.internalValue > 0u;
			}
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x00014B96 File Offset: 0x00012D96
		public UnlockableIndex(int value)
		{
			this.internalValue = (uint)(value + 1);
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x00014BA1 File Offset: 0x00012DA1
		public override bool Equals(object obj)
		{
			return obj is UnlockableIndex && this.Equals((UnlockableIndex)obj);
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x00014BB9 File Offset: 0x00012DB9
		public bool Equals(UnlockableIndex other)
		{
			return this.internalValue.Equals(other.internalValue);
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x00014BCC File Offset: 0x00012DCC
		public int CompareTo(UnlockableIndex other)
		{
			return this.internalValue.CompareTo(other.internalValue);
		}

		// Token: 0x06001BF7 RID: 7159 RVA: 0x00014BDF File Offset: 0x00012DDF
		public override int GetHashCode()
		{
			return this.internalValue.GetHashCode();
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00014BEC File Offset: 0x00012DEC
		public static bool operator ==(UnlockableIndex a, UnlockableIndex b)
		{
			return a.internalValue == b.internalValue;
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x00014BFC File Offset: 0x00012DFC
		public static bool operator !=(UnlockableIndex a, UnlockableIndex b)
		{
			return !(a == b);
		}

		// Token: 0x04001E42 RID: 7746
		public uint internalValue;
	}
}
