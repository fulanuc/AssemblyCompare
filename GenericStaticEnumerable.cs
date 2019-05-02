using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x0200043B RID: 1083
	public struct GenericStaticEnumerable<T, TEnumerator> : IEnumerable<T>, IEnumerable where TEnumerator : struct, IEnumerator<T>
	{
		// Token: 0x06001843 RID: 6211 RVA: 0x00012278 File Offset: 0x00010478
		static GenericStaticEnumerable()
		{
			GenericStaticEnumerable<T, TEnumerator>.defaultValue.Reset();
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x00012295 File Offset: 0x00010495
		public IEnumerator<T> GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x00012295 File Offset: 0x00010495
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GenericStaticEnumerable<T, TEnumerator>.defaultValue;
		}

		// Token: 0x04001B6E RID: 7022
		private static readonly TEnumerator defaultValue = default(TEnumerator);
	}
}
