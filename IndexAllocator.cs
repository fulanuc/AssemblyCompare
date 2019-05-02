using System;

namespace RoR2
{
	// Token: 0x0200043D RID: 1085
	public class IndexAllocator
	{
		// Token: 0x06001826 RID: 6182 RVA: 0x000120A8 File Offset: 0x000102A8
		public IndexAllocator()
		{
			this.ranges = new IndexAllocator.Range[16];
			this.ranges[0] = new IndexAllocator.Range(0, int.MaxValue);
			this.rangeCount = 1;
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x000120DB File Offset: 0x000102DB
		public int RequestIndex()
		{
			int result = this.ranges[0].TakeIndex();
			if (this.ranges[0].empty)
			{
				this.RemoveAt(0);
			}
			return result;
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x00012108 File Offset: 0x00010308
		private void RemoveAt(int i)
		{
			HGArrayUtilities.ArrayRemoveAt<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, 1);
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0001211D File Offset: 0x0001031D
		private void InsertAt(int i, IndexAllocator.Range range)
		{
			HGArrayUtilities.ArrayInsert<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, ref range);
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007DF5C File Offset: 0x0007C15C
		public void FreeIndex(int index)
		{
			if (index < this.ranges[0].startIndex)
			{
				if (this.ranges[0].TryExtending(index))
				{
					return;
				}
				this.InsertAt(0, new IndexAllocator.Range(index, index + 1));
				return;
			}
			else
			{
				if (this.ranges[this.rangeCount - 1].endIndex > index)
				{
					int i = 1;
					while (i < this.rangeCount)
					{
						int endIndex = this.ranges[i - 1].endIndex;
						int startIndex = this.ranges[i].startIndex;
						if (endIndex <= index && index < startIndex)
						{
							bool flag = index == endIndex;
							bool flag2 = index == startIndex - 1;
							if (flag ^ flag2)
							{
								if (flag)
								{
									IndexAllocator.Range[] array = this.ranges;
									int num = i - 1;
									array[num].endIndex = array[num].endIndex + 1;
									return;
								}
								IndexAllocator.Range[] array2 = this.ranges;
								int num2 = i;
								array2[num2].startIndex = array2[num2].startIndex - 1;
								return;
							}
							else
							{
								if (flag)
								{
									this.ranges[i - 1].endIndex = this.ranges[i].endIndex;
									this.RemoveAt(i);
									return;
								}
								this.InsertAt(i, new IndexAllocator.Range(index, index + 1));
								return;
							}
						}
						else
						{
							i++;
						}
					}
					return;
				}
				if (this.ranges[this.rangeCount - 1].TryExtending(index))
				{
					return;
				}
				this.InsertAt(this.rangeCount, new IndexAllocator.Range(index, index + 1));
				return;
			}
		}

		// Token: 0x04001B75 RID: 7029
		private IndexAllocator.Range[] ranges;

		// Token: 0x04001B76 RID: 7030
		private int rangeCount;

		// Token: 0x0200043E RID: 1086
		private struct Range
		{
			// Token: 0x0600182B RID: 6187 RVA: 0x00012133 File Offset: 0x00010333
			public Range(int startIndex, int endIndex)
			{
				this.startIndex = startIndex;
				this.endIndex = endIndex;
			}

			// Token: 0x0600182C RID: 6188 RVA: 0x0007E0C4 File Offset: 0x0007C2C4
			public int TakeIndex()
			{
				int num = this.startIndex;
				this.startIndex = num + 1;
				return num;
			}

			// Token: 0x0600182D RID: 6189 RVA: 0x00012143 File Offset: 0x00010343
			public bool TryExtending(int index)
			{
				if (index == this.startIndex - 1)
				{
					this.startIndex--;
					return true;
				}
				if (index == this.endIndex)
				{
					this.endIndex++;
					return true;
				}
				return false;
			}

			// Token: 0x1700022B RID: 555
			// (get) Token: 0x0600182E RID: 6190 RVA: 0x0001217A File Offset: 0x0001037A
			public bool empty
			{
				get
				{
					return this.startIndex == this.endIndex;
				}
			}

			// Token: 0x1700022C RID: 556
			// (get) Token: 0x0600182F RID: 6191 RVA: 0x0001218A File Offset: 0x0001038A
			public int size
			{
				get
				{
					return this.endIndex - this.startIndex;
				}
			}

			// Token: 0x04001B77 RID: 7031
			public int startIndex;

			// Token: 0x04001B78 RID: 7032
			public int endIndex;
		}
	}
}
