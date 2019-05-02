using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A6 RID: 1702
	[RegisterAchievement("Discover5Equipment", "Items.EquipmentMagazine", null, null)]
	public class Discover5EquipmentAchievement : BaseAchievement
	{
		// Token: 0x06002624 RID: 9764 RVA: 0x0001BD9B File Offset: 0x00019F9B
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x06002625 RID: 9765 RVA: 0x0001BDC0 File Offset: 0x00019FC0
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x06002626 RID: 9766 RVA: 0x0001BDDF File Offset: 0x00019FDF
		public override float ProgressForAchievement()
		{
			return (float)this.EquipmentDiscovered() / 5f;
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x0001BDEE File Offset: 0x00019FEE
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			if (pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				this.Check();
			}
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000B1D30 File Offset: 0x000AFF30
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

		// Token: 0x06002629 RID: 9769 RVA: 0x0001BE00 File Offset: 0x0001A000
		private void Check()
		{
			if (this.EquipmentDiscovered() >= 5)
			{
				base.Grant();
			}
		}

		// Token: 0x040028A1 RID: 10401
		private const int requirement = 5;
	}
}
