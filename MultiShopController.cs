using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000363 RID: 867
	public class MultiShopController : NetworkBehaviour, IHologramContentProvider
	{
		// Token: 0x060011D6 RID: 4566 RVA: 0x0000D9A8 File Offset: 0x0000BBA8
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.CreateTerminals();
			}
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x00067520 File Offset: 0x00065720
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

		// Token: 0x060011D8 RID: 4568 RVA: 0x00067594 File Offset: 0x00065794
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

		// Token: 0x060011D9 RID: 4569 RVA: 0x000675D4 File Offset: 0x000657D4
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

		// Token: 0x060011DA RID: 4570 RVA: 0x0006779C File Offset: 0x0006599C
		private void DisableAllTerminals(Interactor interactor)
		{
			foreach (GameObject gameObject in this.terminalGameObjects)
			{
				gameObject.GetComponent<PurchaseInteraction>().Networkavailable = false;
				gameObject.GetComponent<ShopTerminalBehavior>().SetNoPickup();
			}
			this.Networkavailable = false;
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0000D9B7 File Offset: 0x0000BBB7
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x00009298 File Offset: 0x00007498
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x000677E0 File Offset: 0x000659E0
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060011E0 RID: 4576 RVA: 0x00067814 File Offset: 0x00065A14
		// (set) Token: 0x060011E1 RID: 4577 RVA: 0x0000D9D5 File Offset: 0x0000BBD5
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

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x00067828 File Offset: 0x00065A28
		// (set) Token: 0x060011E3 RID: 4579 RVA: 0x0000D9E9 File Offset: 0x0000BBE9
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

		// Token: 0x060011E4 RID: 4580 RVA: 0x0006783C File Offset: 0x00065A3C
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

		// Token: 0x060011E5 RID: 4581 RVA: 0x000678E8 File Offset: 0x00065AE8
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

		// Token: 0x040015E1 RID: 5601
		[Tooltip("The shop terminal prefab to instantiate.")]
		public GameObject terminalPrefab;

		// Token: 0x040015E2 RID: 5602
		[Tooltip("The positions at which to instantiate shop terminals.")]
		public Transform[] terminalPositions;

		// Token: 0x040015E3 RID: 5603
		[Tooltip("The tier of items to drop")]
		public ItemTier itemTier;

		// Token: 0x040015E4 RID: 5604
		[Tooltip("Whether or not there's a chance the item contents are replaced with a '?'")]
		private bool hideDisplayContent = true;

		// Token: 0x040015E5 RID: 5605
		private GameObject[] terminalGameObjects;

		// Token: 0x040015E6 RID: 5606
		[SyncVar]
		private bool available = true;

		// Token: 0x040015E7 RID: 5607
		public int baseCost;

		// Token: 0x040015E8 RID: 5608
		public CostType costType;

		// Token: 0x040015E9 RID: 5609
		[SyncVar]
		private int cost;
	}
}
