using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x02000433 RID: 1075
	public struct GenericStaticEnumerable<T, TEnumerator> : IEnumerable<T>, IEnumerable where TEnumerator : struct, IEnumerator<T>
	{
		// Token: 0x060017F6 RID: 6134 RVA: 0x00011E04 File Offset: 0x00010004
		static GenericStaticEnumerable()
		{
			GenericStaticEnumerable<T, TEnumerator>.defaultValue.Reset();
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00011E21 File Offset: 0x00010021
		public IEnumerator<T> GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00011E21 File Offset: 0x00010021
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x04001B3E RID: 6974
		private static readonly TEnumerator defaultValue = default(TEnumerator);
	}
}
