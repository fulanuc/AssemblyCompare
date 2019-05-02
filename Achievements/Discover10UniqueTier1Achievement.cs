using System;

namespace RoR2.Achievements
{
	// Token: 0x02000693 RID: 1683
	[RegisterAchievement("Discover10UniqueTier1", "Items.Crowbar", null, null)]
	public class Discover10UniqueTier1Achievement : BaseAchievement
	{
		// Token: 0x06002586 RID: 9606 RVA: 0x0001B5FB File Offset: 0x000197FB
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x0001B620 File Offset: 0x00019820
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x0001B63F File Offset: 0x0001983F
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueTier1Discovered() / 10f;
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x000B05C8 File Offset: 0x000AE7C8
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1)
			{
				this.Check();
			}
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000B05F4 File Offset: 0x000AE7F4
		private int UniqueTier1Discovered()
		{
			int num = 0;
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1 && this.userProfile.HasDiscoveredPickup(new PickupIndex(itemIndex)))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x0001B64E File Offset: 0x0001984E
		private void Check()
		{
			if (this.UniqueTier1Discovered() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x04002844 RID: 10308
		private const int requirement = 10;
	}
}
