using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062C RID: 1580
	public class ScoreboardStrip : MonoBehaviour
	{
		// Token: 0x06002382 RID: 9090 RVA: 0x000A9D28 File Offset: 0x000A7F28
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

		// Token: 0x06002383 RID: 9091 RVA: 0x00019E7B File Offset: 0x0001807B
		private void UpdateMoneyText()
		{
			if (this.master)
			{
				this.moneyText.text = string.Format("${0}", this.master.money);
			}
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x00019EAF File Offset: 0x000180AF
		private void Update()
		{
			this.UpdateMoneyText();
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000A9E00 File Offset: 0x000A8000
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

		// Token: 0x04002663 RID: 9827
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x04002664 RID: 9828
		public EquipmentIcon equipmentIcon;

		// Token: 0x04002665 RID: 9829
		public SocialUserIcon userAvatar;

		// Token: 0x04002666 RID: 9830
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002667 RID: 9831
		public RawImage classIcon;

		// Token: 0x04002668 RID: 9832
		public TextMeshProUGUI moneyText;

		// Token: 0x04002669 RID: 9833
		private CharacterMaster master;

		// Token: 0x0400266A RID: 9834
		private CharacterBody userBody;

		// Token: 0x0400266B RID: 9835
		private PlayerCharacterMasterController userPlayerCharacterMasterController;
	}
}
