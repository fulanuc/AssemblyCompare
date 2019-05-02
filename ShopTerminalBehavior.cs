using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DC RID: 988
	public class ShopTerminalBehavior : NetworkBehaviour
	{
		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06001574 RID: 5492 RVA: 0x000102CC File Offset: 0x0000E4CC
		public bool pickupIndexIsHidden
		{
			get
			{
				return this.hidden;
			}
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x0007371C File Offset: 0x0007191C
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				if (newHasBeenPurchased && this.animator)
				{
					int layerIndex = this.animator.GetLayerIndex("Body");
					this.animator.PlayInFixedTime("Opening", layerIndex);
				}
			}
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x000102D4 File Offset: 0x0000E4D4
		private void OnSyncHidden(bool newHidden)
		{
			this.SetPickupIndex(this.pickupIndex, newHidden);
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x000102E3 File Offset: 0x0000E4E3
		private void OnSyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.SetPickupIndex(newPickupIndex, this.hidden);
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x0007376C File Offset: 0x0007196C
		public void Start()
		{
			if (NetworkServer.active && this.selfGeneratePickup)
			{
				PickupIndex newPickupIndex = PickupIndex.none;
				switch (this.itemTier)
				{
				case ItemTier.Tier1:
					newPickupIndex = Run.instance.availableTier1DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier1DropList.Count)];
					break;
				case ItemTier.Tier2:
					newPickupIndex = Run.instance.availableTier2DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier2DropList.Count)];
					break;
				case ItemTier.Tier3:
					newPickupIndex = Run.instance.availableTier3DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier3DropList.Count)];
					break;
				case ItemTier.Lunar:
					newPickupIndex = Run.instance.availableLunarDropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableLunarDropList.Count)];
					break;
				}
				this.SetPickupIndex(newPickupIndex, false);
			}
			if (NetworkClient.active)
			{
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x000102F2 File Offset: 0x0000E4F2
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex != newPickupIndex || this.hidden != newHidden)
			{
				this.NetworkpickupIndex = newPickupIndex;
				this.Networkhidden = newHidden;
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00073890 File Offset: 0x00071A90
		private void UpdatePickupDisplayAndAnimations()
		{
			if (this.pickupDisplay)
			{
				this.pickupDisplay.SetPickupIndex(this.pickupIndex, this.hidden);
			}
			if (this.pickupIndex == PickupIndex.none)
			{
				Util.PlaySound("Play_UI_tripleChestShutter", base.gameObject);
				if (this.animator)
				{
					int layerIndex = this.animator.GetLayerIndex("Body");
					this.animator.PlayInFixedTime(this.hasBeenPurchased ? "Open" : "Closing", layerIndex);
				}
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0001031F File Offset: 0x0000E51F
		public PickupIndex CurrentPickupIndex()
		{
			return this.pickupIndex;
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00010327 File Offset: 0x0000E527
		[Server]
		public void SetNoPickup()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::SetNoPickup()' called on client");
				return;
			}
			this.SetPickupIndex(PickupIndex.none, false);
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x00073924 File Offset: 0x00071B24
		[Server]
		public void DropPickup()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
				return;
			}
			this.SetHasBeenPurchased(true);
			PickupDropletController.CreatePickupDroplet(this.pickupIndex, (this.dropTransform ? this.dropTransform : base.transform).position, base.transform.TransformVector(this.dropVelocity));
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06001580 RID: 5504 RVA: 0x0007398C File Offset: 0x00071B8C
		// (set) Token: 0x06001581 RID: 5505 RVA: 0x0001035D File Offset: 0x0000E55D
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06001582 RID: 5506 RVA: 0x000739A0 File Offset: 0x00071BA0
		// (set) Token: 0x06001583 RID: 5507 RVA: 0x0001039C File Offset: 0x0000E59C
		public bool Networkhidden
		{
			get
			{
				return this.hidden;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncHidden(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hidden, dirtyBit);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x000739B4 File Offset: 0x00071BB4
		// (set) Token: 0x06001585 RID: 5509 RVA: 0x000103DB File Offset: 0x0000E5DB
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			set
			{
				uint dirtyBit = 4u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x000739C8 File Offset: 0x00071BC8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.Write(this.hidden);
				writer.Write(this.hasBeenPurchased);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hidden);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hasBeenPurchased);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x00073AB4 File Offset: 0x00071CB4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.hidden = reader.ReadBoolean();
				this.hasBeenPurchased = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncPickupIndex(GeneratedNetworkCode._ReadPickupIndex_None(reader));
			}
			if ((num & 2) != 0)
			{
				this.OnSyncHidden(reader.ReadBoolean());
			}
			if ((num & 4) != 0)
			{
				this.SetHasBeenPurchased(reader.ReadBoolean());
			}
		}

		// Token: 0x040018B2 RID: 6322
		[SyncVar(hook = "OnSyncPickupIndex")]
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040018B3 RID: 6323
		[SyncVar(hook = "OnSyncHidden")]
		private bool hidden;

		// Token: 0x040018B4 RID: 6324
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x040018B5 RID: 6325
		[Tooltip("The PickupDisplay component that should show which item this shop terminal is offering.")]
		public PickupDisplay pickupDisplay;

		// Token: 0x040018B6 RID: 6326
		[Tooltip("The position from which the drop will be emitted")]
		public Transform dropTransform;

		// Token: 0x040018B7 RID: 6327
		[Tooltip("Whether or not the shop terminal shouldd drive itself")]
		public bool selfGeneratePickup;

		// Token: 0x040018B8 RID: 6328
		[Tooltip("The tier of items to drop - only works if the pickup generates itself")]
		public ItemTier itemTier;

		// Token: 0x040018B9 RID: 6329
		[Tooltip("The velocity with which the drop will be emitted. Rotates with this object.")]
		public Vector3 dropVelocity;

		// Token: 0x040018BA RID: 6330
		public Animator animator;
	}
}
