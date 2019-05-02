using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000228 RID: 552
	[CreateAssetMenu]
	public class ItemDisplayRuleSet : ScriptableObject
	{
		// Token: 0x06000AB1 RID: 2737 RVA: 0x0004900C File Offset: 0x0004720C
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

		// Token: 0x06000AB2 RID: 2738 RVA: 0x000089EA File Offset: 0x00006BEA
		public void SetItemDisplayRuleGroup(ItemIndex itemIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.itemRuleGroups.Length)
			{
				return;
			}
			this.itemRuleGroups[(int)itemIndex] = displayRuleGroup;
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00049048 File Offset: 0x00047248
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

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00008A09 File Offset: 0x00006C09
		public void SetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.equipmentRuleGroups.Length)
			{
				return;
			}
			this.equipmentRuleGroups[(int)equipmentIndex] = displayRuleGroup;
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00008A28 File Offset: 0x00006C28
		public void Reset()
		{
			this.itemRuleGroups = new DisplayRuleGroup[78];
			this.equipmentRuleGroups = new DisplayRuleGroup[27];
		}

		// Token: 0x04000E19 RID: 3609
		[FormerlySerializedAs("ruleGroups")]
		[SerializeField]
		private DisplayRuleGroup[] itemRuleGroups = new DisplayRuleGroup[78];

		// Token: 0x04000E1A RID: 3610
		[SerializeField]
		private DisplayRuleGroup[] equipmentRuleGroups = new DisplayRuleGroup[27];
	}
}
