using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063E RID: 1598
	public class ScoreboardStrip : MonoBehaviour
	{
		// Token: 0x06002412 RID: 9234 RVA: 0x000AB3A4 File Offset: 0x000A95A4
		public void SetMaster(CharacterMaster newMaster)
		{
			if (this.master == newMaster)
			{
				return;
			}
			this.userBody = null;
			this.master = newMaster;
			if (this.master)
			{
				this.userBody = this.master.GetBody();
				this.userPlayerCharacterMasterController = this.master.GetComponent<PlayerCharacterMasterController>();
				this.itemInventoryDisplay.SetSubscribedInventory(this.master.inventory);
				this.equipmentIcon.targetInventory = this.master.inventory;
				this.UpdateMoneyText();
			}
			if (this.userAvatar && this.userAvatar.isActiveAndEnabled)
			{
				this.userAvatar.SetFromMaster(newMaster);
			}
			this.nameLabel.text = Util.GetBestMasterName(this.master);
			this.classIcon.texture = this.FindMasterPortrait();
		}

		// Token: 0x06002413 RID: 9235 RVA: 0x0001A549 File Offset: 0x00018749
		private void UpdateMoneyText()
		{
			if (this.master)
			{
				this.moneyText.text = string.Format("${0}", this.master.money);
			}
		}

		// Token: 0x06002414 RID: 9236 RVA: 0x0001A57D File Offset: 0x0001877D
		private void Update()
		{
			this.UpdateMoneyText();
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x000AB47C File Offset: 0x000A967C
		private Texture FindMasterPortrait()
		{
			if (this.userBody)
			{
				return this.userBody.portraitIcon;
			}
			if (this.master)
			{
				GameObject bodyPrefab = this.master.bodyPrefab;
				if (bodyPrefab)
				{
					CharacterBody component = bodyPrefab.GetComponent<CharacterBody>();
					if (component)
					{
						return component.portraitIcon;
					}
				}
			}
			return null;
		}

		// Token: 0x040026BE RID: 9918
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x040026BF RID: 9919
		public EquipmentIcon equipmentIcon;

		// Token: 0x040026C0 RID: 9920
		public SocialUserIcon userAvatar;

		// Token: 0x040026C1 RID: 9921
		public TextMeshProUGUI nameLabel;

		// Token: 0x040026C2 RID: 9922
		public RawImage classIcon;

		// Token: 0x040026C3 RID: 9923
		public TextMeshProUGUI moneyText;

		// Token: 0x040026C4 RID: 9924
		private CharacterMaster master;

		// Token: 0x040026C5 RID: 9925
		private CharacterBody userBody;

		// Token: 0x040026C6 RID: 9926
		private PlayerCharacterMasterController userPlayerCharacterMasterController;
	}
}
