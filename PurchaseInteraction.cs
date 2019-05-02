using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003A3 RID: 931
	[RequireComponent(typeof(Highlight))]
	public sealed class PurchaseInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x060013B2 RID: 5042 RVA: 0x0006D7F4 File Offset: 0x0006B9F4
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

		// Token: 0x060013B3 RID: 5043 RVA: 0x0000F0F3 File Offset: 0x0000D2F3
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

		// Token: 0x060013B4 RID: 5044 RVA: 0x0000F111 File Offset: 0x0000D311
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

		// Token: 0x060013B5 RID: 5045 RVA: 0x0000F13B File Offset: 0x0000D33B
		private void SetAvailableTrue()
		{
			this.Networkavailable = true;
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0000F144 File Offset: 0x0000D344
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0006D840 File Offset: 0x0006BA40
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

		// Token: 0x060013B8 RID: 5048 RVA: 0x0006D944 File Offset: 0x0006BB44
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

		// Token: 0x060013B9 RID: 5049 RVA: 0x0000F151 File Offset: 0x0000D351
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0006D980 File Offset: 0x0006BB80
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

		// Token: 0x060013BB RID: 5051 RVA: 0x0000F169 File Offset: 0x0000D369
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

		// Token: 0x060013BC RID: 5052 RVA: 0x0000F188 File Offset: 0x0000D388
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

		// Token: 0x060013BD RID: 5053 RVA: 0x0006D9D0 File Offset: 0x0006BBD0
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
		// (add) Token: 0x060013BE RID: 5054 RVA: 0x0006DAE8 File Offset: 0x0006BCE8
		// (remove) Token: 0x060013BF RID: 5055 RVA: 0x0006DB1C File Offset: 0x0006BD1C
		public static event Action<PurchaseInteraction, Interactor> onItemSpentOnPurchase;

		// Token: 0x060013C0 RID: 5056 RVA: 0x0006DB50 File Offset: 0x0006BD50
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

		// Token: 0x060013C1 RID: 5057 RVA: 0x0006DE70 File Offset: 0x0006C070
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

		// Token: 0x060013C2 RID: 5058 RVA: 0x0000F1A8 File Offset: 0x0000D3A8
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x000092D8 File Offset: 0x000074D8
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x0006DED4 File Offset: 0x0006C0D4
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x0000F1A8 File Offset: 0x0000D3A8
		public bool ShouldShowOnScanner()
		{
			return this.available;
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0000F1B0 File Offset: 0x0000D3B0
		private void OnEnable()
		{
			InstanceTracker.Add<PurchaseInteraction>(this);
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0000F1B8 File Offset: 0x0000D3B8
		private void OnDisable()
		{
			InstanceTracker.Remove<PurchaseInteraction>(this);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x0006DF08 File Offset: 0x0006C108
		// (set) Token: 0x060013CC RID: 5068 RVA: 0x0000F1DA File Offset: 0x0000D3DA
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

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x0006DF1C File Offset: 0x0006C11C
		// (set) Token: 0x060013CE RID: 5070 RVA: 0x0000F1EE File Offset: 0x0000D3EE
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

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x0006DF30 File Offset: 0x0006C130
		// (set) Token: 0x060013D0 RID: 5072 RVA: 0x0000F202 File Offset: 0x0000D402
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

		// Token: 0x060013D1 RID: 5073 RVA: 0x0006DF44 File Offset: 0x0006C144
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

		// Token: 0x060013D2 RID: 5074 RVA: 0x0006E030 File Offset: 0x0006C230
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

		// Token: 0x060013D3 RID: 5075 RVA: 0x0000F21C File Offset: 0x0000D41C
		public override void PreStartClient()
		{
			if (!this.___lockGameObjectNetId.IsEmpty())
			{
				this.NetworklockGameObject = ClientScene.FindLocalObject(this.___lockGameObjectNetId);
			}
		}

		// Token: 0x04001755 RID: 5973
		public string displayNameToken;

		// Token: 0x04001756 RID: 5974
		public string contextToken;

		// Token: 0x04001757 RID: 5975
		public CostType costType;

		// Token: 0x04001758 RID: 5976
		[SyncVar]
		public bool available = true;

		// Token: 0x04001759 RID: 5977
		[SyncVar]
		public int cost;

		// Token: 0x0400175A RID: 5978
		public bool automaticallyScaleCostWithDifficulty;

		// Token: 0x0400175B RID: 5979
		[Tooltip("The unlockable that a player must have to be able to interact with this terminal.")]
		public string requiredUnlockable = "";

		// Token: 0x0400175C RID: 5980
		public bool ignoreSpherecastForInteractability;

		// Token: 0x0400175D RID: 5981
		public string[] purchaseStatNames;

		// Token: 0x0400175E RID: 5982
		[HideInInspector]
		public Interactor lastActivator;

		// Token: 0x0400175F RID: 5983
		[SyncVar]
		public GameObject lockGameObject;

		// Token: 0x04001760 RID: 5984
		private Xoroshiro128Plus rng;

		// Token: 0x04001762 RID: 5986
		public PurchaseEvent onPurchase;

		// Token: 0x04001763 RID: 5987
		private NetworkInstanceId ___lockGameObjectNetId;
	}
}
