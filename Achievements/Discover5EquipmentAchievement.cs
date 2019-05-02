using System;

namespace RoR2.Achievements
{
	// Token: 0x02000694 RID: 1684
	[RegisterAchievement("Discover5Equipment", "Items.EquipmentMagazine", null, null)]
	public class Discover5EquipmentAchievement : BaseAchievement
	{
		// Token: 0x0600258D RID: 9613 RVA: 0x0001B660 File Offset: 0x00019860
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x0001B685 File Offset: 0x00019885
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x0001B6A4 File Offset: 0x000198A4
		public override float ProgressForAchievement()
		{
			return (float)this.EquipmentDiscovered() / 5f;
		}

		// Token: 0x06002590 RID: 9616 RVA: 0x0001B6B3 File Offset: 0x000198B3
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			if (pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				this.Check();
			}
		}

		// Token: 0x06002591 RID: 9617 RVA: 0x000B0638 File Offset: 0x000AE838
		private int EquipmentDiscovered()
		{
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				if (this.userProfile.HasDiscoveredPickup(new PickupIndex(equipmentIndex)))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x0001B6C5 File Offset: 0x000198C5
		private void Check()
		{
			if (this.EquipmentDiscovered() >= 5)
			{
				base.Grant();
			}
		}

		// Token: 0x04002845 RID: 10309
		private const int requirement = 5;
	}
}
