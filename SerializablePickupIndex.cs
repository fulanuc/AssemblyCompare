using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000474 RID: 1140
	[Serializable]
	public struct SerializablePickupIndex
	{
		// Token: 0x060019B7 RID: 6583 RVA: 0x000132B6 File Offset: 0x000114B6
		public static explicit operator PickupIndex(SerializablePickupIndex serializablePickupIndex)
		{
			return PickupIndex.Find(serializablePickupIndex.pickupName);
		}

		// Token: 0x04001CEB RID: 7403
		[SerializeField]
		public string pickupName;
	}
}
