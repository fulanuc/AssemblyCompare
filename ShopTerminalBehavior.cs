using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E2 RID: 994
	public class ShopTerminalBehavior : NetworkBehaviour
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060015B1 RID: 5553 RVA: 0x000106D5 File Offset: 0x0000E8D5
		public bool pickupIndexIsHidden
		{
			get
			{
				return this.hidden;
			}
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x00073D54 File Offset: 0x00071F54
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

		// Token: 0x060015B3 RID: 5555 RVA: 0x000106DD File Offset: 0x0000E8DD
		private void OnSyncHidden(bool newHidden)
		{
			this.SetPickupIndex(this.pickupIndex, newHidden);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x000106EC File Offset: 0x0000E8EC
		private void OnSyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.SetPickupIndex(newPickupIndex, this.hidden);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x00073DA4 File Offset: 0x00071FA4
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

		// Token: 0x060015B6 RID: 5558 RVA: 0x000106FB File Offset: 0x0000E8FB
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex != newPickupIndex || this.hidden != newHidden)
			{
				this.NetworkpickupIndex = newPickupIndex;
				this.Networkhidden = newHidden;
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00073EC8 File Offset: 0x000720C8
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

		// Token: 0x060015B8 RID: 5560 RVA: 0x00010728 File Offset: 0x0000E928
		public PickupIndex CurrentPickupIndex()
		{
			return this.pickupIndex;
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x00010730 File Offset: 0x0000E930
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

		// Token: 0x060015BA RID: 5562 RVA: 0x00073F5C File Offset: 0x0007215C
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

		// Token: 0x060015BC RID: 5564 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060015BD RID: 5565 RVA: 0x00073FC4 File Offset: 0x000721C4
		// (set) Token: 0x060015BE RID: 5566 RVA: 0x00010766 File Offset: 0x0000E966
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

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x00073FD8 File Offset: 0x000721D8
		// (set) Token: 0x060015C0 RID: 5568 RVA: 0x000107A5 File Offset: 0x0000E9A5
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

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x00073FEC File Offset: 0x000721EC
		// (set) Token: 0x060015C2 RID: 5570 RVA: 0x000107E4 File Offset: 0x0000E9E4
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

		// Token: 0x060015C3 RID: 5571 RVA: 0x00074000 File Offset: 0x00072200
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

		// Token: 0x060015C4 RID: 5572 RVA: 0x000740EC File Offset: 0x000722EC
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

		// Token: 0x040018DB RID: 6363
		[SyncVar(hook = "OnSyncPickupIndex")]
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040018DC RID: 6364
		[SyncVar(hook = "OnSyncHidden")]
		private bool hidden;

		// Token: 0x040018DD RID: 6365
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x040018DE RID: 6366
		[Tooltip("The PickupDisplay component that should show which item this shop terminal is offering.")]
		public PickupDisplay pickupDisplay;

		// Token: 0x040018DF RID: 6367
		[Tooltip("The position from which the drop will be emitted")]
		public Transform dropTransform;

		// Token: 0x040018E0 RID: 6368
		[Tooltip("Whether or not the shop terminal shouldd drive itself")]
		public bool selfGeneratePickup;

		// Token: 0x040018E1 RID: 6369
		[Tooltip("The tier of items to drop - only works if the pickup generates itself")]
		public ItemTier itemTier;

		// Token: 0x040018E2 RID: 6370
		[Tooltip("The velocity with which the drop will be emitted. Rotates with this object.")]
		public Vector3 dropVelocity;

		// Token: 0x040018E3 RID: 6371
		public Animator animator;
	}
}
