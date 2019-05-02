using System;
using System.Collections.Generic;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034E RID: 846
	[RequireComponent(typeof(CharacterBody))]
	public class MageCalibrationController : MonoBehaviour
	{
		// Token: 0x06001188 RID: 4488 RVA: 0x0000D5E7 File Offset: 0x0000B7E7
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterBody.onInventoryChanged += this.OnInventoryChanged;
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x0000D61D File Offset: 0x0000B81D
		private void Start()
		{
			this.currentElement = this.GetAwardedElementFromInventory();
			this.RefreshCalibrationElement(this.currentElement);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0000D637 File Offset: 0x0000B837
		private void OnDestroy()
		{
			this.characterBody.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600118B RID: 4491 RVA: 0x0000D650 File Offset: 0x0000B850
		// (set) Token: 0x0600118C RID: 4492 RVA: 0x0000D658 File Offset: 0x0000B858
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

		// Token: 0x0600118D RID: 4493 RVA: 0x0000D677 File Offset: 0x0000B877
		private void OnInventoryChanged()
		{
			base.enabled = true;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00066614 File Offset: 0x00064814
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

		// Token: 0x0600118F RID: 4495 RVA: 0x00066680 File Offset: 0x00064880
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

		// Token: 0x06001190 RID: 4496 RVA: 0x000666B8 File Offset: 0x000648B8
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

		// Token: 0x06001191 RID: 4497 RVA: 0x0000D680 File Offset: 0x0000B880
		public MageElement GetActiveCalibrationElement()
		{
			return this.currentElement;
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x000667F0 File Offset: 0x000649F0
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

		// Token: 0x06001193 RID: 4499 RVA: 0x00066838 File Offset: 0x00064A38
		public void RefreshCalibrationElement(MageElement targetElement)
		{
			MageCalibrationController.CalibrationInfo calibrationInfo = this.calibrationInfos[(int)targetElement];
			this.calibrationOverlayRenderer.enabled = calibrationInfo.enableCalibrationOverlay;
			this.calibrationOverlayRenderer.material = calibrationInfo.calibrationOverlayMaterial;
		}

		// Token: 0x04001581 RID: 5505
		public MageCalibrationController.CalibrationInfo[] calibrationInfos;

		// Token: 0x04001582 RID: 5506
		public SkinnedMeshRenderer calibrationOverlayRenderer;

		// Token: 0x04001583 RID: 5507
		[Tooltip("The state machine upon which to perform the calibration state.")]
		public EntityStateMachine stateMachine;

		// Token: 0x04001584 RID: 5508
		[Tooltip("The priority with which the calibration state will try to interrupt the current state.")]
		public InterruptPriority calibrationStateInterruptPriority;

		// Token: 0x04001585 RID: 5509
		private CharacterBody characterBody;

		// Token: 0x04001586 RID: 5510
		private bool hasEffectiveAuthority;

		// Token: 0x04001587 RID: 5511
		private MageElement _currentElement;

		// Token: 0x04001588 RID: 5512
		private static readonly int[] elementCounter = new int[4];

		// Token: 0x0200034F RID: 847
		[Serializable]
		public struct CalibrationInfo
		{
			// Token: 0x04001589 RID: 5513
			public bool enableCalibrationOverlay;

			// Token: 0x0400158A RID: 5514
			public Material calibrationOverlayMaterial;
		}
	}
}
