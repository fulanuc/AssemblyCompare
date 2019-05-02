using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200039E RID: 926
	[RequireComponent(typeof(Highlight))]
	public class PurchaseInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x06001395 RID: 5013 RVA: 0x0006D5EC File Offset: 0x0006B7EC
		private void Awake()
		{
			if (this.automaticallyScaleCostWithDifficulty)
			{
				this.Networkcost = Run.instance.GetDifficultyScaledCost(this.cost);
			}
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x0000EF29 File Offset: 0x0000D129
		[Server]
		public void SetAvailable(bool newAvailable)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::SetAvailable(System.Boolean)' called on client");
				return;
			}
			this.Networkavailable = newAvailable;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0000EF47 File Offset: 0x0000D147
		[Server]
		public void SetUnavailableTemporarily(float time)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::SetUnavailableTemporarily(System.Single)' called on client");
				return;
			}
			this.Networkavailable = false;
			base.Invoke("SetAvailableTrue", time);
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0000EF71 File Offset: 0x0000D171
		private void SetAvailableTrue()
		{
			this.Networkavailable = true;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x0000EF7A File Offset: 0x0000D17A
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x0006D638 File Offset: 0x0006B838
		private string GetCostString()
		{
			switch (this.costType)
			{
			case CostType.None:
				return "";
			case CostType.Money:
				return string.Format(" (<nobr><style=cShrine>${0}</style></nobr>)", this.cost);
			case CostType.PercentHealth:
				return string.Format(" (<nobr><style=cDeath>{0}% HP</style></nobr>)", this.cost);
			case CostType.Lunar:
				return string.Format(" (<nobr><color=#{1}>{0}</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarCoin));
			case CostType.WhiteItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			case CostType.GreenItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			case CostType.RedItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			default:
				return "";
			}
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x0006D73C File Offset: 0x0006B93C
		private static bool ActivatorHasUnlockable(Interactor activator, string unlockableName)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			if (networkUser)
			{
				LocalUser localUser = networkUser.localUser;
				if (localUser != null)
				{
					return localUser.userProfile.HasUnlockable(unlockableName);
				}
			}
			return true;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x0000EF87 File Offset: 0x0000D187
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0006D778 File Offset: 0x0006B978
		public Interactability GetInteractability(Interactor activator)
		{
			if (!string.IsNullOrEmpty(this.requiredUnlockable) && !PurchaseInteraction.ActivatorHasUnlockable(activator, this.requiredUnlockable))
			{
				return Interactability.Disabled;
			}
			if (!this.available || this.lockGameObject)
			{
				return Interactability.Disabled;
			}
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x0000EF9F File Offset: 0x0000D19F
		public static ItemTier CostTypeToItemTier(CostType costType)
		{
			switch (costType)
			{
			case CostType.WhiteItem:
				return ItemTier.Tier1;
			case CostType.GreenItem:
				return ItemTier.Tier2;
			case CostType.RedItem:
				return ItemTier.Tier3;
			default:
				return ItemTier.NoTier;
			}
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x0000EFBE File Offset: 0x0000D1BE
		public static ColorCatalog.ColorIndex CostTypeToColorIndex(CostType costType)
		{
			switch (costType)
			{
			case CostType.WhiteItem:
				return ColorCatalog.ColorIndex.Tier1Item;
			case CostType.GreenItem:
				return ColorCatalog.ColorIndex.Tier2Item;
			case CostType.RedItem:
				return ColorCatalog.ColorIndex.Tier3Item;
			default:
				return ColorCatalog.ColorIndex.Error;
			}
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x0006D7C8 File Offset: 0x0006B9C8
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			switch (this.costType)
			{
			case CostType.None:
				return true;
			case CostType.Money:
			{
				CharacterBody component = activator.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						return (ulong)master.money >= (ulong)((long)this.cost);
					}
				}
				return false;
			}
			case CostType.PercentHealth:
			{
				HealthComponent component2 = activator.GetComponent<HealthComponent>();
				return component2 && component2.health / component2.fullHealth * 100f >= (float)this.cost;
			}
			case CostType.Lunar:
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
				return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.cost);
			}
			case CostType.WhiteItem:
			case CostType.GreenItem:
			case CostType.RedItem:
			{
				ItemTier itemTier = PurchaseInteraction.CostTypeToItemTier(this.costType);
				CharacterBody component3 = activator.gameObject.GetComponent<CharacterBody>();
				if (component3)
				{
					Inventory inventory = component3.inventory;
					if (inventory)
					{
						return inventory.HasAtLeastXTotalItemsOfTier(itemTier, this.cost);
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060013A1 RID: 5025 RVA: 0x0006D8E0 File Offset: 0x0006BAE0
		// (remove) Token: 0x060013A2 RID: 5026 RVA: 0x0006D914 File Offset: 0x0006BB14
		public static event Action<PurchaseInteraction, Interactor> onItemSpentOnPurchase;

		// Token: 0x060013A3 RID: 5027 RVA: 0x0006D948 File Offset: 0x0006BB48
		public void OnInteractionBegin(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return;
			}
			CharacterBody component = activator.GetComponent<CharacterBody>();
			switch (this.costType)
			{
			case CostType.Money:
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.money -= (uint)this.cost;
					}
				}
				break;
			case CostType.PercentHealth:
			{
				HealthComponent component2 = activator.GetComponent<HealthComponent>();
				if (component2)
				{
					float health = component2.health;
					float num = component2.fullHealth * (float)this.cost / 100f;
					if (health > num)
					{
						component2.TakeDamage(new DamageInfo
						{
							damage = num,
							attacker = base.gameObject,
							position = base.transform.position,
							damageType = DamageType.BypassArmor
						});
					}
				}
				break;
			}
			case CostType.Lunar:
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
				if (networkUser)
				{
					networkUser.DeductLunarCoins((uint)this.cost);
				}
				break;
			}
			case CostType.WhiteItem:
			case CostType.GreenItem:
			case CostType.RedItem:
			{
				ItemTier itemTier = PurchaseInteraction.CostTypeToItemTier(this.costType);
				if (component)
				{
					Inventory inventory = component.inventory;
					if (inventory)
					{
						ItemIndex itemIndex = ItemIndex.None;
						ShopTerminalBehavior component3 = base.GetComponent<ShopTerminalBehavior>();
						if (component3)
						{
							itemIndex = component3.CurrentPickupIndex().itemIndex;
						}
						WeightedSelection<ItemIndex> weightedSelection = new WeightedSelection<ItemIndex>(8);
						foreach (ItemIndex itemIndex2 in ItemCatalog.allItems)
						{
							if (itemIndex2 != itemIndex)
							{
								int itemCount = inventory.GetItemCount(itemIndex2);
								if (itemCount > 0 && ItemCatalog.GetItemDef(itemIndex2).tier == itemTier)
								{
									weightedSelection.AddChoice(itemIndex2, (float)itemCount);
								}
							}
						}
						List<ItemIndex> list = new List<ItemIndex>();
						int num2 = 0;
						while (weightedSelection.Count > 0 && num2 < this.cost)
						{
							int num3 = weightedSelection.EvaluteToChoiceIndex(this.rng.nextNormalizedFloat);
							WeightedSelection<ItemIndex>.ChoiceInfo choice = weightedSelection.GetChoice(num3);
							ItemIndex value = choice.value;
							int num4 = (int)choice.weight;
							num4--;
							if (num4 <= 0)
							{
								weightedSelection.RemoveChoice(num3);
							}
							else
							{
								weightedSelection.ModifyChoiceWeight(num3, (float)num4);
							}
							list.Add(value);
							num2++;
						}
						for (int i = num2; i < this.cost; i++)
						{
							list.Add(itemIndex);
						}
						for (int j = 0; j < list.Count; j++)
						{
							ItemIndex itemIndex3 = list[j];
							PurchaseInteraction.CreateItemTakenOrb(component.corePosition, base.gameObject, itemIndex3);
							inventory.RemoveItem(itemIndex3, 1);
							if (itemIndex3 != itemIndex)
							{
								Action<PurchaseInteraction, Interactor> action = PurchaseInteraction.onItemSpentOnPurchase;
								if (action != null)
								{
									action(this, activator);
								}
							}
						}
					}
				}
				break;
			}
			}
			IEnumerable<StatDef> statDefsToIncrement = this.purchaseStatNames.Select(new Func<string, StatDef>(StatDef.Find));
			StatManager.OnPurchase<IEnumerable<StatDef>>(component, this.costType, statDefsToIncrement);
			this.onPurchase.Invoke(activator);
			this.lastActivator = activator;
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x0006DC68 File Offset: 0x0006BE68
		[Server]
		private static void CreateItemTakenOrb(Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::CreateItemTakenOrb(UnityEngine.Vector3,UnityEngine.GameObject,RoR2.ItemIndex)' called on client");
				return;
			}
			GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTakenOrbEffect");
			EffectData effectData = new EffectData
			{
				origin = effectOrigin,
				genericFloat = 1.5f,
				genericUInt = (uint)(itemIndex + 1)
			};
			effectData.SetNetworkedObjectReference(targetObject);
			EffectManager.instance.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0000EFDE File Offset: 0x0000D1DE
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00009298 File Offset: 0x00007498
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x0006DCCC File Offset: 0x0006BECC
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x0000EFE6 File Offset: 0x0000D1E6
		private void OnEnable()
		{
			PurchaseInteraction.instancesList.Add(this);
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0000EFF3 File Offset: 0x0000D1F3
		private void OnDisable()
		{
			PurchaseInteraction.instancesList.Remove(this);
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060013AE RID: 5038 RVA: 0x0006DD00 File Offset: 0x0006BF00
		// (set) Token: 0x060013AF RID: 5039 RVA: 0x0000F036 File Offset: 0x0000D236
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

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060013B0 RID: 5040 RVA: 0x0006DD14 File Offset: 0x0006BF14
		// (set) Token: 0x060013B1 RID: 5041 RVA: 0x0000F04A File Offset: 0x0000D24A
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

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060013B2 RID: 5042 RVA: 0x0006DD28 File Offset: 0x0006BF28
		// (set) Token: 0x060013B3 RID: 5043 RVA: 0x0000F05E File Offset: 0x0000D25E
		public GameObject NetworklockGameObject
		{
			get
			{
				return this.lockGameObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.lockGameObject, 4u, ref this.___lockGameObjectNetId);
			}
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x0006DD3C File Offset: 0x0006BF3C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
				writer.WritePackedUInt32((uint)this.cost);
				writer.Write(this.lockGameObject);
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
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.lockGameObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x0006DE28 File Offset: 0x0006C028
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				this.cost = (int)reader.ReadPackedUInt32();
				this.___lockGameObjectNetId = reader.ReadNetworkId();
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
			if ((num & 4) != 0)
			{
				this.lockGameObject = reader.ReadGameObject();
			}
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0000F078 File Offset: 0x0000D278
		public override void PreStartClient()
		{
			if (!this.___lockGameObjectNetId.IsEmpty())
			{
				this.NetworklockGameObject = ClientScene.FindLocalObject(this.___lockGameObjectNetId);
			}
		}

		// Token: 0x04001739 RID: 5945
		private static readonly List<PurchaseInteraction> instancesList = new List<PurchaseInteraction>();

		// Token: 0x0400173A RID: 5946
		public static readonly ReadOnlyCollection<PurchaseInteraction> readOnlyInstancesList = PurchaseInteraction.instancesList.AsReadOnly();

		// Token: 0x0400173B RID: 5947
		public string displayNameToken;

		// Token: 0x0400173C RID: 5948
		public string contextToken;

		// Token: 0x0400173D RID: 5949
		public CostType costType;

		// Token: 0x0400173E RID: 5950
		[SyncVar]
		public bool available = true;

		// Token: 0x0400173F RID: 5951
		[SyncVar]
		public int cost;

		// Token: 0x04001740 RID: 5952
		public bool automaticallyScaleCostWithDifficulty;

		// Token: 0x04001741 RID: 5953
		[Tooltip("The unlockable that a player must have to be able to interact with this terminal.")]
		public string requiredUnlockable = "";

		// Token: 0x04001742 RID: 5954
		public bool ignoreSpherecastForInteractability;

		// Token: 0x04001743 RID: 5955
		public string[] purchaseStatNames;

		// Token: 0x04001744 RID: 5956
		[HideInInspector]
		public Interactor lastActivator;

		// Token: 0x04001745 RID: 5957
		[SyncVar]
		public GameObject lockGameObject;

		// Token: 0x04001746 RID: 5958
		private Xoroshiro128Plus rng;

		// Token: 0x04001748 RID: 5960
		public PurchaseEvent onPurchase;

		// Token: 0x04001749 RID: 5961
		private NetworkInstanceId ___lockGameObjectNetId;
	}
}
