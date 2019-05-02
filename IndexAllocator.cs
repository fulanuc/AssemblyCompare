using System;

namespace RoR2
{
	// Token: 0x02000445 RID: 1093
	public class IndexAllocator
	{
		// Token: 0x06001873 RID: 6259 RVA: 0x0001251C File Offset: 0x0001071C
		public IndexAllocator()
		{
			this.ranges = new IndexAllocator.Range[16];
			this.ranges[0] = new IndexAllocator.Range(0, int.MaxValue);
			this.rangeCount = 1;
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0001254F File Offset: 0x0001074F
		public int RequestIndex()
		{
			int result = this.ranges[0].TakeIndex();
			if (this.ranges[0].empty)
			{
				this.RemoveAt(0);
			}
			return result;
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0001257C File Offset: 0x0001077C
		private void RemoveAt(int i)
		{
			HGArrayUtilities.ArrayRemoveAt<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, 1);
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x00012591 File Offset: 0x00010791
		private void InsertAt(int i, IndexAllocator.Range range)
		{
			HGArrayUtilities.ArrayInsert<IndexAllocator.Range>(ref this.ranges, ref this.rangeCount, i, ref range);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x0007E718 File Offset: 0x0007C918
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

		// Token: 0x04001BA5 RID: 7077
		private IndexAllocator.Range[] ranges;

		// Token: 0x04001BA6 RID: 7078
		private int rangeCount;

		// Token: 0x02000446 RID: 1094
		private struct Range
		{
			// Token: 0x06001878 RID: 6264 RVA: 0x000125A7 File Offset: 0x000107A7
			public Range(int startIndex, int endIndex)
			{
				this.startIndex = startIndex;
				this.endIndex = endIndex;
			}

			// Token: 0x06001879 RID: 6265 RVA: 0x0007E880 File Offset: 0x0007CA80
			public int TakeIndex()
			{
				int num = this.startIndex;
				this.startIndex = num + 1;
				return num;
			}

			// Token: 0x0600187A RID: 6266 RVA: 0x000125B7 File Offset: 0x000107B7
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

			// Token: 0x17000236 RID: 566
			// (get) Token: 0x0600187B RID: 6267 RVA: 0x000125EE File Offset: 0x000107EE
			public bool empty
			{
				get
				{
					return this.startIndex == this.endIndex;
				}
			}

			// Token: 0x17000237 RID: 567
			// (get) Token: 0x0600187C RID: 6268 RVA: 0x000125FE File Offset: 0x000107FE
			public int size
			{
				get
				{
					return this.endIndex - this.startIndex;
				}
			}

			// Token: 0x04001BA7 RID: 7079
			public int startIndex;

			// Token: 0x04001BA8 RID: 7080
			public int endIndex;
		}
	}
}
