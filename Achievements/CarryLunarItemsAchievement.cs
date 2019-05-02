using System;

namespace RoR2.Achievements
{
	// Token: 0x02000696 RID: 1686
	[RegisterAchievement("CarryLunarItems", "Items.Meteor", null, null)]
	public class CarryLunarItemsAchievement : BaseAchievement
	{
		// Token: 0x060025D6 RID: 9686 RVA: 0x0001B8E5 File Offset: 0x00019AE5
		public override void OnInstall()
		{
			base.OnInstall();
			this.localUser.onMasterChanged += this.OnMasterChanged;
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x0001B915 File Offset: 0x00019B15
		public override void OnUninstall()
		{
			this.SetMasterController(null);
			this.localUser.onMasterChanged -= this.OnMasterChanged;
			base.OnUninstall();
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x000B1870 File Offset: 0x000AFA70
		private void SetMasterController(PlayerCharacterMasterController newMasterController)
		{
			if (this.currentMasterController == newMasterController)
			{
				return;
			}
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			this.currentMasterController = newMasterController;
			PlayerCharacterMasterController playerCharacterMasterController = this.currentMasterController;
			Inventory inventory;
			if (playerCharacterMasterController == null)
			{
				inventory = null;
			}
			else
			{
				CharacterMaster master = playerCharacterMasterController.master;
				inventory = ((master != null) ? master.inventory : null);
			}
			this.currentInventory = inventory;
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged += this.OnInventoryChanged;
			}
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x000B18F0 File Offset: 0x000AFAF0
		private void OnInventoryChanged()
		{
			if (this.currentInventory)
			{
				int num = 5;
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.currentInventory.GetEquipmentIndex());
				if (equipmentDef != null && equipmentDef.isLunar)
				{
					num--;
				}
				if (this.currentInventory.HasAtLeastXTotalItemsOfTier(ItemTier.Lunar, num))
				{
					base.Grant();
				}
			}
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x0001B93B File Offset: 0x00019B3B
		private void OnMasterChanged()
		{
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x04002892 RID: 10386
		public const int requirement = 5;

		// Token: 0x04002893 RID: 10387
		private PlayerCharacterMasterController currentMasterController;

		// Token: 0x04002894 RID: 10388
		private Inventory currentInventory;
	}
}
