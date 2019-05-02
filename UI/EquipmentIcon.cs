using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D4 RID: 1492
	public class EquipmentIcon : MonoBehaviour
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600217A RID: 8570 RVA: 0x000185CB File Offset: 0x000167CB
		public bool hasEquipment
		{
			get
			{
				return this.currentDisplayData.hasEquipment;
			}
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x000A1C64 File Offset: 0x0009FE64
		private void SetDisplayData(EquipmentIcon.DisplayData newDisplayData)
		{
			if (!this.currentDisplayData.isReady && newDisplayData.isReady)
			{
				this.DoStockFlash();
			}
			if (this.displayRoot)
			{
				this.displayRoot.SetActive(!newDisplayData.hideEntireDisplay);
			}
			if (newDisplayData.stock > this.currentDisplayData.stock)
			{
				Util.PlaySound("Play_item_proc_equipMag", RoR2Application.instance.gameObject);
				this.DoStockFlash();
			}
			if (this.isReadyPanelObject)
			{
				this.isReadyPanelObject.SetActive(newDisplayData.isReady);
			}
			if (this.isAutoCastPanelObject)
			{
				if (this.targetInventory)
				{
					this.isAutoCastPanelObject.SetActive(this.targetInventory.GetItemCount(ItemIndex.AutoCastEquipment) > 0);
				}
				else
				{
					this.isAutoCastPanelObject.SetActive(false);
				}
			}
			if (this.iconImage)
			{
				Texture texture = null;
				Color color = Color.clear;
				if (newDisplayData.equipmentDef != null)
				{
					color = ((newDisplayData.stock > 0) ? Color.white : Color.gray);
					texture = Resources.Load<Texture>(newDisplayData.equipmentDef.pickupIconPath);
				}
				this.iconImage.texture = texture;
				this.iconImage.color = color;
			}
			if (this.cooldownText)
			{
				this.cooldownText.gameObject.SetActive(newDisplayData.showCooldown);
				if (newDisplayData.cooldownValue != this.currentDisplayData.cooldownValue)
				{
					this.cooldownText.text = newDisplayData.cooldownValue.ToString();
				}
			}
			if (this.stockText)
			{
				if (newDisplayData.maxStock > 1)
				{
					this.stockText.gameObject.SetActive(true);
					this.stockText.text = newDisplayData.stock.ToString();
				}
				else
				{
					this.stockText.gameObject.SetActive(false);
				}
			}
			string titleToken = null;
			string bodyToken = null;
			Color titleColor = Color.white;
			Color gray = Color.gray;
			if (newDisplayData.equipmentDef != null)
			{
				titleToken = newDisplayData.equipmentDef.nameToken;
				bodyToken = newDisplayData.equipmentDef.pickupToken;
				titleColor = ColorCatalog.GetColor(newDisplayData.equipmentDef.colorIndex);
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = titleToken;
				this.tooltipProvider.titleColor = titleColor;
				this.tooltipProvider.bodyToken = bodyToken;
				this.tooltipProvider.bodyColor = gray;
			}
			this.currentDisplayData = newDisplayData;
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x000A1EC8 File Offset: 0x000A00C8
		private void DoReminderFlash()
		{
			if (this.reminderFlashPanelObject)
			{
				AnimateUIAlpha component = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (component)
				{
					component.time = 0f;
				}
				this.reminderFlashPanelObject.SetActive(true);
			}
			this.equipmentReminderTimer = 5f;
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x000A1F18 File Offset: 0x000A0118
		private void DoStockFlash()
		{
			this.DoReminderFlash();
			if (this.stockFlashPanelObject)
			{
				AnimateUIAlpha component = this.stockFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (component)
				{
					component.time = 0f;
				}
				this.stockFlashPanelObject.SetActive(true);
			}
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x000A1F64 File Offset: 0x000A0164
		private EquipmentIcon.DisplayData GenerateDisplayData()
		{
			EquipmentIcon.DisplayData result = default(EquipmentIcon.DisplayData);
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			if (this.targetInventory)
			{
				EquipmentState equipmentState;
				if (this.displayAlternateEquipment)
				{
					equipmentState = this.targetInventory.alternateEquipmentState;
					result.hideEntireDisplay = (this.targetInventory.GetEquipmentSlotCount() <= 1);
				}
				else
				{
					equipmentState = this.targetInventory.currentEquipmentState;
					result.hideEntireDisplay = false;
				}
				Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
				Run.FixedTimeStamp chargeFinishTime = equipmentState.chargeFinishTime;
				equipmentIndex = equipmentState.equipmentIndex;
				result.cooldownValue = Mathf.CeilToInt(chargeFinishTime - now);
				result.stock = (int)equipmentState.charges;
				result.maxStock = (this.targetEquipmentSlot ? this.targetEquipmentSlot.maxStock : 1);
			}
			result.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			return result;
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x000A2034 File Offset: 0x000A0234
		private void Update()
		{
			this.SetDisplayData(this.GenerateDisplayData());
			this.equipmentReminderTimer -= Time.deltaTime;
			if (this.currentDisplayData.isReady && this.equipmentReminderTimer < 0f && this.currentDisplayData.equipmentDef != null)
			{
				this.DoReminderFlash();
			}
		}

		// Token: 0x0400241D RID: 9245
		public Inventory targetInventory;

		// Token: 0x0400241E RID: 9246
		public EquipmentSlot targetEquipmentSlot;

		// Token: 0x0400241F RID: 9247
		public GameObject displayRoot;

		// Token: 0x04002420 RID: 9248
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x04002421 RID: 9249
		public RawImage iconImage;

		// Token: 0x04002422 RID: 9250
		public TextMeshProUGUI cooldownText;

		// Token: 0x04002423 RID: 9251
		public TextMeshProUGUI stockText;

		// Token: 0x04002424 RID: 9252
		public GameObject stockFlashPanelObject;

		// Token: 0x04002425 RID: 9253
		public GameObject reminderFlashPanelObject;

		// Token: 0x04002426 RID: 9254
		public GameObject isReadyPanelObject;

		// Token: 0x04002427 RID: 9255
		public GameObject isAutoCastPanelObject;

		// Token: 0x04002428 RID: 9256
		public TooltipProvider tooltipProvider;

		// Token: 0x04002429 RID: 9257
		public bool displayAlternateEquipment;

		// Token: 0x0400242A RID: 9258
		private int previousStockCount;

		// Token: 0x0400242B RID: 9259
		private float equipmentReminderTimer;

		// Token: 0x0400242C RID: 9260
		private EquipmentIcon.DisplayData currentDisplayData;

		// Token: 0x020005D5 RID: 1493
		private struct DisplayData
		{
			// Token: 0x170002E7 RID: 743
			// (get) Token: 0x06002181 RID: 8577 RVA: 0x000185D8 File Offset: 0x000167D8
			public bool isReady
			{
				get
				{
					return this.stock > 0;
				}
			}

			// Token: 0x170002E8 RID: 744
			// (get) Token: 0x06002182 RID: 8578 RVA: 0x000185E3 File Offset: 0x000167E3
			public bool hasEquipment
			{
				get
				{
					return this.equipmentDef != null;
				}
			}

			// Token: 0x170002E9 RID: 745
			// (get) Token: 0x06002183 RID: 8579 RVA: 0x000185EE File Offset: 0x000167EE
			public bool showCooldown
			{
				get
				{
					return !this.isReady && this.hasEquipment;
				}
			}

			// Token: 0x0400242D RID: 9261
			public EquipmentDef equipmentDef;

			// Token: 0x0400242E RID: 9262
			public int cooldownValue;

			// Token: 0x0400242F RID: 9263
			public int stock;

			// Token: 0x04002430 RID: 9264
			public int maxStock;

			// Token: 0x04002431 RID: 9265
			public bool hideEntireDisplay;
		}
	}
}
