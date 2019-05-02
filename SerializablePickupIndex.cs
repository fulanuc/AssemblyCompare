using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000469 RID: 1129
	[Serializable]
	public struct SerializablePickupIndex
	{
		// Token: 0x0600195A RID: 6490 RVA: 0x00012D9C File Offset: 0x00010F9C
		public static explicit operator PickupIndex(SerializablePickupIndex serializablePickupIndex)
		{
			return PickupIndex.Find(serializablePickupIndex.pickupName);
		}

		// Token: 0x04001CB7 RID: 7351
		[SerializeField]
		public string pickupName;
	}
}
