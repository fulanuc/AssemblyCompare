using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000366 RID: 870
	public class MultiShopController : NetworkBehaviour, IHologramContentProvider
	{
		// Token: 0x060011ED RID: 4589 RVA: 0x0000DA91 File Offset: 0x0000BC91
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.CreateTerminals();
			}
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00067858 File Offset: 0x00065A58
		private void Start()
		{
			if (Run.instance && NetworkServer.active)
			{
				this.Networkcost = Run.instance.GetDifficultyScaledCost(this.baseCost);
				if (this.terminalGameObjects != null)
				{
					GameObject[] array = this.terminalGameObjects;
					for (int i = 0; i < array.Length; i++)
					{
						PurchaseInteraction component = array[i].GetComponent<PurchaseInteraction>();
						component.Networkcost = this.cost;
						component.costType = this.costType;
					}
				}
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x000678CC File Offset: 0x00065ACC
		private void OnDestroy()
		{
			if (this.terminalGameObjects != null)
			{
				for (int i = this.terminalGameObjects.Length - 1; i >= 0; i--)
				{
					UnityEngine.Object.Destroy(this.terminalGameObjects[i]);
				}
				this.terminalGameObjects = null;
			}
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x0006790C File Offset: 0x00065B0C
		private void CreateTerminals()
		{
			this.terminalGameObjects = new GameObject[this.terminalPositions.Length];
			for (int i = 0; i < this.terminalPositions.Length; i++)
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
				bool newHidden = this.hideDisplayContent && Run.instance.treasureRng.nextNormalizedFloat < 0.2f;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.terminalPrefab, this.terminalPositions[i].position, this.terminalPositions[i].rotation);
				this.terminalGameObjects[i] = gameObject;
				gameObject.GetComponent<ShopTerminalBehavior>().SetPickupIndex(newPickupIndex, newHidden);
				NetworkServer.Spawn(gameObject);
			}
			GameObject[] array = this.terminalGameObjects;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].GetComponent<PurchaseInteraction>().onPurchase.AddListener(new UnityAction<Interactor>(this.DisableAllTerminals));
			}
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x00067AD4 File Offset: 0x00065CD4
		private void DisableAllTerminals(Interactor interactor)
		{
			foreach (GameObject gameObject in this.terminalGameObjects)
			{
				gameObject.GetComponent<PurchaseInteraction>().Networkavailable = false;
				gameObject.GetComponent<ShopTerminalBehavior>().SetNoPickup();
			}
			this.Networkavailable = false;
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x0000DAA0 File Offset: 0x0000BCA0
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x000092D8 File Offset: 0x000074D8
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00067B18 File Offset: 0x00065D18
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x00067B4C File Offset: 0x00065D4C
		// (set) Token: 0x060011F8 RID: 4600 RVA: 0x0000DABE File Offset: 0x0000BCBE
		public bool Networkavailable
		{
			get
			{
				return this.available;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.available, 1u);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x00067B60 File Offset: 0x00065D60
		// (set) Token: 0x060011FA RID: 4602 RVA: 0x0000DAD2 File Offset: 0x0000BCD2
		public int Networkcost
		{
			get
			{
				return this.cost;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.cost, 2u);
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x00067B74 File Offset: 0x00065D74
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
				writer.WritePackedUInt32((uint)this.cost);
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
				writer.Write(this.available);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.cost);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00067C20 File Offset: 0x00065E20
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				this.cost = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.available = reader.ReadBoolean();
			}
			if ((num & 2) != 0)
			{
				this.cost = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x040015FA RID: 5626
		[Tooltip("The shop terminal prefab to instantiate.")]
		public GameObject terminalPrefab;

		// Token: 0x040015FB RID: 5627
		[Tooltip("The positions at which to instantiate shop terminals.")]
		public Transform[] terminalPositions;

		// Token: 0x040015FC RID: 5628
		[Tooltip("The tier of items to drop")]
		public ItemTier itemTier;

		// Token: 0x040015FD RID: 5629
		[Tooltip("Whether or not there's a chance the item contents are replaced with a '?'")]
		private bool hideDisplayContent = true;

		// Token: 0x040015FE RID: 5630
		private GameObject[] terminalGameObjects;

		// Token: 0x040015FF RID: 5631
		[SyncVar]
		private bool available = true;

		// Token: 0x04001600 RID: 5632
		public int baseCost;

		// Token: 0x04001601 RID: 5633
		public CostType costType;

		// Token: 0x04001602 RID: 5634
		[SyncVar]
		private int cost;
	}
}
