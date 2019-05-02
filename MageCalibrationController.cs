using System;
using System.Collections.Generic;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000351 RID: 849
	[RequireComponent(typeof(CharacterBody))]
	public class MageCalibrationController : MonoBehaviour
	{
		// Token: 0x0600119F RID: 4511 RVA: 0x0000D6D0 File Offset: 0x0000B8D0
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterBody.onInventoryChanged += this.OnInventoryChanged;
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x0000D706 File Offset: 0x0000B906
		private void Start()
		{
			this.currentElement = this.GetAwardedElementFromInventory();
			this.RefreshCalibrationElement(this.currentElement);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x0000D720 File Offset: 0x0000B920
		private void OnDestroy()
		{
			this.characterBody.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060011A2 RID: 4514 RVA: 0x0000D739 File Offset: 0x0000B939
		// (set) Token: 0x060011A3 RID: 4515 RVA: 0x0000D741 File Offset: 0x0000B941
		private MageElement currentElement
		{
			get
			{
				return this._currentElement;
			}
			set
			{
				if (value == this._currentElement)
				{
					return;
				}
				this._currentElement = value;
				this.RefreshCalibrationElement(this._currentElement);
			}
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x0000D760 File Offset: 0x0000B960
		private void OnInventoryChanged()
		{
			base.enabled = true;
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x0006694C File Offset: 0x00064B4C
		private void FixedUpdate()
		{
			base.enabled = false;
			this.currentElement = this.GetAwardedElementFromInventory();
			if (this.hasEffectiveAuthority && this.currentElement == MageElement.None)
			{
				MageElement mageElement = this.CalcElementToAward();
				if (mageElement != MageElement.None && !(this.stateMachine.state is MageCalibrate))
				{
					MageCalibrate mageCalibrate = new MageCalibrate();
					mageCalibrate.element = mageElement;
					this.stateMachine.SetInterruptState(mageCalibrate, this.calibrationStateInterruptPriority);
				}
			}
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x000669B8 File Offset: 0x00064BB8
		private MageElement GetAwardedElementFromInventory()
		{
			Inventory inventory = this.characterBody.inventory;
			if (inventory)
			{
				MageElement mageElement = (MageElement)inventory.GetItemCount(ItemIndex.MageAttunement);
				if (mageElement >= MageElement.None && mageElement < MageElement.Count)
				{
					return mageElement;
				}
			}
			return MageElement.None;
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x000669F0 File Offset: 0x00064BF0
		private MageElement CalcElementToAward()
		{
			for (int i = 0; i < MageCalibrationController.elementCounter.Length; i++)
			{
				MageCalibrationController.elementCounter[i] = 0;
			}
			Inventory inventory = this.characterBody.inventory;
			if (!inventory)
			{
				return MageElement.None;
			}
			List<ItemIndex> itemAcquisitionOrder = inventory.itemAcquisitionOrder;
			for (int j = 0; j < itemAcquisitionOrder.Count; j++)
			{
				ItemIndex itemIndex = itemAcquisitionOrder[j];
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				if (itemDef.mageElement != MageElement.None)
				{
					int num = 0;
					switch (itemDef.tier)
					{
					case ItemTier.Tier1:
						num = 1;
						break;
					case ItemTier.Tier2:
						num = 2;
						break;
					case ItemTier.Tier3:
						num = 3;
						break;
					case ItemTier.Lunar:
						num = 3;
						break;
					}
					MageCalibrationController.elementCounter[(int)itemDef.mageElement] += num * inventory.GetItemCount(itemIndex);
				}
			}
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			if (equipmentDef != null && equipmentDef.mageElement != MageElement.None)
			{
				MageCalibrationController.elementCounter[(int)equipmentDef.mageElement] += 2;
			}
			MageElement result = MageElement.None;
			int num2 = 0;
			for (MageElement mageElement = MageElement.Fire; mageElement < MageElement.Count; mageElement += 1)
			{
				int num3 = MageCalibrationController.elementCounter[(int)mageElement];
				if (num3 > num2)
				{
					result = mageElement;
					num2 = num3;
				}
			}
			if (num2 >= 5)
			{
				return result;
			}
			return MageElement.None;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0000D769 File Offset: 0x0000B969
		public MageElement GetActiveCalibrationElement()
		{
			return this.currentElement;
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x00066B28 File Offset: 0x00064D28
		public void SetElement(MageElement newElement)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Inventory inventory = this.characterBody.inventory;
			if (inventory)
			{
				MageElement mageElement = (MageElement)inventory.GetItemCount(ItemIndex.MageAttunement);
				if (mageElement != newElement)
				{
					int count = (int)(newElement - mageElement);
					inventory.GiveItem(ItemIndex.MageAttunement, count);
				}
			}
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x00066B70 File Offset: 0x00064D70
		public void RefreshCalibrationElement(MageElement targetElement)
		{
			MageCalibrationController.CalibrationInfo calibrationInfo = this.calibrationInfos[(int)targetElement];
			this.calibrationOverlayRenderer.enabled = calibrationInfo.enableCalibrationOverlay;
			this.calibrationOverlayRenderer.material = calibrationInfo.calibrationOverlayMaterial;
		}

		// Token: 0x0400159A RID: 5530
		public MageCalibrationController.CalibrationInfo[] calibrationInfos;

		// Token: 0x0400159B RID: 5531
		public SkinnedMeshRenderer calibrationOverlayRenderer;

		// Token: 0x0400159C RID: 5532
		[Tooltip("The state machine upon which to perform the calibration state.")]
		public EntityStateMachine stateMachine;

		// Token: 0x0400159D RID: 5533
		[Tooltip("The priority with which the calibration state will try to interrupt the current state.")]
		public InterruptPriority calibrationStateInterruptPriority;

		// Token: 0x0400159E RID: 5534
		private CharacterBody characterBody;

		// Token: 0x0400159F RID: 5535
		private bool hasEffectiveAuthority;

		// Token: 0x040015A0 RID: 5536
		private MageElement _currentElement;

		// Token: 0x040015A1 RID: 5537
		private static readonly int[] elementCounter = new int[4];

		// Token: 0x02000352 RID: 850
		[Serializable]
		public struct CalibrationInfo
		{
			// Token: 0x040015A2 RID: 5538
			public bool enableCalibrationOverlay;

			// Token: 0x040015A3 RID: 5539
			public Material calibrationOverlayMaterial;
		}
	}
}
