using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E6 RID: 1510
	public class EquipmentIcon : MonoBehaviour
	{
		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600220B RID: 8715 RVA: 0x00018CC5 File Offset: 0x00016EC5
		public bool hasEquipment
		{
			get
			{
				return this.currentDisplayData.hasEquipment;
			}
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x000A3238 File Offset: 0x000A1438
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

		// Token: 0x0600220D RID: 8717 RVA: 0x000A349C File Offset: 0x000A169C
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

		// Token: 0x0600220E RID: 8718 RVA: 0x000A34EC File Offset: 0x000A16EC
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

		// Token: 0x0600220F RID: 8719 RVA: 0x000A3538 File Offset: 0x000A1738
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

		// Token: 0x06002210 RID: 8720 RVA: 0x000A3608 File Offset: 0x000A1808
		private void Update()
		{
			this.SetDisplayData(this.GenerateDisplayData());
			this.equipmentReminderTimer -= Time.deltaTime;
			if (this.currentDisplayData.isReady && this.equipmentReminderTimer < 0f && this.currentDisplayData.equipmentDef != null)
			{
				this.DoReminderFlash();
			}
		}

		// Token: 0x04002471 RID: 9329
		public Inventory targetInventory;

		// Token: 0x04002472 RID: 9330
		public EquipmentSlot targetEquipmentSlot;

		// Token: 0x04002473 RID: 9331
		public GameObject displayRoot;

		// Token: 0x04002474 RID: 9332
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x04002475 RID: 9333
		public RawImage iconImage;

		// Token: 0x04002476 RID: 9334
		public TextMeshProUGUI cooldownText;

		// Token: 0x04002477 RID: 9335
		public TextMeshProUGUI stockText;

		// Token: 0x04002478 RID: 9336
		public GameObject stockFlashPanelObject;

		// Token: 0x04002479 RID: 9337
		public GameObject reminderFlashPanelObject;

		// Token: 0x0400247A RID: 9338
		public GameObject isReadyPanelObject;

		// Token: 0x0400247B RID: 9339
		public GameObject isAutoCastPanelObject;

		// Token: 0x0400247C RID: 9340
		public TooltipProvider tooltipProvider;

		// Token: 0x0400247D RID: 9341
		public bool displayAlternateEquipment;

		// Token: 0x0400247E RID: 9342
		private int previousStockCount;

		// Token: 0x0400247F RID: 9343
		private float equipmentReminderTimer;

		// Token: 0x04002480 RID: 9344
		private EquipmentIcon.DisplayData currentDisplayData;

		// Token: 0x020005E7 RID: 1511
		private struct DisplayData
		{
			// Token: 0x170002FA RID: 762
			// (get) Token: 0x06002212 RID: 8722 RVA: 0x00018CD2 File Offset: 0x00016ED2
			public bool isReady
			{
				get
				{
					return this.stock > 0;
				}
			}

			// Token: 0x170002FB RID: 763
			// (get) Token: 0x06002213 RID: 8723 RVA: 0x00018CDD File Offset: 0x00016EDD
			public bool hasEquipment
			{
				get
				{
					return this.equipmentDef != null;
				}
			}

			// Token: 0x170002FC RID: 764
			// (get) Token: 0x06002214 RID: 8724 RVA: 0x00018CE8 File Offset: 0x00016EE8
			public bool showCooldown
			{
				get
				{
					return !this.isReady && this.hasEquipment;
				}
			}

			// Token: 0x04002481 RID: 9345
			public EquipmentDef equipmentDef;

			// Token: 0x04002482 RID: 9346
			public int cooldownValue;

			// Token: 0x04002483 RID: 9347
			public int stock;

			// Token: 0x04002484 RID: 9348
			public int maxStock;

			// Token: 0x04002485 RID: 9349
			public bool hideEntireDisplay;
		}
	}
}
