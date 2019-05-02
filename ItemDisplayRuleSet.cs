using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000228 RID: 552
	[CreateAssetMenu]
	public class ItemDisplayRuleSet : ScriptableObject
	{
		// Token: 0x06000AAD RID: 2733 RVA: 0x00048D50 File Offset: 0x00046F50
		public DisplayRuleGroup GetItemDisplayRuleGroup(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.itemRuleGroups.Length)
			{
				return new DisplayRuleGroup
				{
					rules = null
				};
			}
			return this.itemRuleGroups[(int)itemIndex];
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x000089C5 File Offset: 0x00006BC5
		public void SetItemDisplayRuleGroup(ItemIndex itemIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.itemRuleGroups.Length)
			{
				return;
			}
			this.itemRuleGroups[(int)itemIndex] = displayRuleGroup;
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00048D8C File Offset: 0x00046F8C
		public DisplayRuleGroup GetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.equipmentRuleGroups.Length)
			{
				return new DisplayRuleGroup
				{
					rules = null
				};
			}
			return this.equipmentRuleGroups[(int)equipmentIndex];
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x000089E4 File Offset: 0x00006BE4
		public void SetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.equipmentRuleGroups.Length)
			{
				return;
			}
			this.equipmentRuleGroups[(int)equipmentIndex] = displayRuleGroup;
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00008A03 File Offset: 0x00006C03
		public void Reset()
		{
			this.itemRuleGroups = new DisplayRuleGroup[78];
			this.equipmentRuleGroups = new DisplayRuleGroup[27];
		}

		// Token: 0x04000E15 RID: 3605
		[FormerlySerializedAs("ruleGroups")]
		[SerializeField]
		private DisplayRuleGroup[] itemRuleGroups = new DisplayRuleGroup[78];

		// Token: 0x04000E16 RID: 3606
		[SerializeField]
		private DisplayRuleGroup[] equipmentRuleGroups = new DisplayRuleGroup[27];
	}
}
