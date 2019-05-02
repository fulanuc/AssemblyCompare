using System;

namespace RoR2.Achievements
{
	// Token: 0x02000684 RID: 1668
	[RegisterAchievement("CarryLunarItems", "Items.Meteor", null, null)]
	public class CarryLunarItemsAchievement : BaseAchievement
	{
		// Token: 0x0600253F RID: 9535 RVA: 0x0001B1AA File Offset: 0x000193AA
		public override void OnInstall()
		{
			base.OnInstall();
			this.localUser.onMasterChanged += this.OnMasterChanged;
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x0001B1DA File Offset: 0x000193DA
		public override void OnUninstall()
		{
			this.SetMasterController(null);
			this.localUser.onMasterChanged -= this.OnMasterChanged;
			base.OnUninstall();
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x000B0180 File Offset: 0x000AE380
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

		// Token: 0x06002542 RID: 9538 RVA: 0x000B0200 File Offset: 0x000AE400
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

		// Token: 0x06002543 RID: 9539 RVA: 0x0001B200 File Offset: 0x00019400
		private void OnMasterChanged()
		{
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x04002836 RID: 10294
		public const int requirement = 5;

		// Token: 0x04002837 RID: 10295
		private PlayerCharacterMasterController currentMasterController;

		// Token: 0x04002838 RID: 10296
		private Inventory currentInventory;
	}
}
