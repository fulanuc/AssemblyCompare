using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A5 RID: 1701
	[RegisterAchievement("Discover10UniqueTier1", "Items.Crowbar", null, null)]
	public class Discover10UniqueTier1Achievement : BaseAchievement
	{
		// Token: 0x0600261D RID: 9757 RVA: 0x0001BD36 File Offset: 0x00019F36
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x0001BD5B File Offset: 0x00019F5B
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x0001BD7A File Offset: 0x00019F7A
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueTier1Discovered() / 10f;
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x000B1CC0 File Offset: 0x000AFEC0
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1)
			{
				this.Check();
			}
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x000B1CEC File Offset: 0x000AFEEC
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

		// Token: 0x06002622 RID: 9762 RVA: 0x0001BD89 File Offset: 0x00019F89
		private void Check()
		{
			if (this.UniqueTier1Discovered() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x040028A0 RID: 10400
		private const int requirement = 10;
	}
}
