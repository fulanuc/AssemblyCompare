using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000245 RID: 581
	[Serializable]
	public struct EquipmentMask
	{
		// Token: 0x06000AE8 RID: 2792 RVA: 0x00008CA7 File Offset: 0x00006EA7
		public bool HasEquipment(EquipmentIndex equipmentIndex)
		{
			return equipmentIndex >= EquipmentIndex.CommandMissile && equipmentIndex < EquipmentIndex.Count && (this.a & 1u << (int)equipmentIndex) > 0u;
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x00008CC4 File Offset: 0x00006EC4
		public void AddEquipment(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return;
			}
			this.a |= 1u << (int)equipmentIndex;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x00008CE3 File Offset: 0x00006EE3
		public void RemoveEquipment(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return;
			}
			this.a &= ~(1u << (int)equipmentIndex);
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00049A5C File Offset: 0x00047C5C
		public static EquipmentMask operator &(EquipmentMask mask1, EquipmentMask mask2)
		{
			return new EquipmentMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00049A88 File Offset: 0x00047C88
		static EquipmentMask()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				EquipmentMask.all.AddEquipment(equipmentIndex);
			}
		}

		// Token: 0x04000ECB RID: 3787
		[SerializeField]
		public uint a;

		// Token: 0x04000ECC RID: 3788
		public static readonly EquipmentMask none;

		// Token: 0x04000ECD RID: 3789
		public static readonly EquipmentMask all = default(EquipmentMask);
	}
}
