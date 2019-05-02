using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000267 RID: 615
	public sealed class BazaarUpgradeInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x06000B81 RID: 2945 RVA: 0x0004C07C File Offset: 0x0004A27C
		private void Awake()
		{
			this.unlockableProgressionDefs = new UnlockableDef[this.unlockableProgression.Length];
			for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
			{
				this.unlockableProgressionDefs[i] = UnlockableCatalog.GetUnlockableDef(this.unlockableProgression[i]);
			}
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0000921B File Offset: 0x0000741B
		private void FixedUpdate()
		{
			if (NetworkServer.active && !this.available)
			{
				this.activationTimer -= Time.fixedDeltaTime;
				if (this.activationTimer <= 0f)
				{
					this.Networkavailable = true;
				}
			}
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0004C0C4 File Offset: 0x0004A2C4
		private UnlockableDef GetInteractorNextUnlockable(GameObject activatorGameObject)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activatorGameObject);
			if (networkUser)
			{
				LocalUser localUser = networkUser.localUser;
				if (localUser != null)
				{
					for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
					{
						UnlockableDef unlockableDef = this.unlockableProgressionDefs[i];
						if (!localUser.userProfile.HasUnlockable(unlockableDef))
						{
							return unlockableDef;
						}
					}
				}
				else
				{
					for (int j = 0; j < this.unlockableProgressionDefs.Length; j++)
					{
						UnlockableDef unlockableDef2 = this.unlockableProgressionDefs[j];
						if (!networkUser.unlockables.Contains(unlockableDef2))
						{
							return unlockableDef2;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0004C150 File Offset: 0x0004A350
		private static bool ActivatorHasUnlockable(Interactor activator, string unlockableName)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			if (!networkUser)
			{
				return true;
			}
			LocalUser localUser = networkUser.localUser;
			if (localUser != null)
			{
				return localUser.userProfile.HasUnlockable(unlockableName);
			}
			return networkUser.unlockables.Contains(UnlockableCatalog.GetUnlockableDef(unlockableName));
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x00009252 File Offset: 0x00007452
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0000925F File Offset: 0x0000745F
		private string GetCostString()
		{
			return string.Format(" (<color=#{1}>{0}</color>)", this.cost, BazaarUpgradeInteraction.lunarCoinColorString);
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0000927B File Offset: 0x0000747B
		public string GetContextString(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return null;
			}
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0000929E File Offset: 0x0000749E
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.GetInteractorNextUnlockable(activator.gameObject) == null || !this.available)
			{
				return Interactability.Disabled;
			}
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnInteractionBegin(Interactor activator)
		{
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x000092C4 File Offset: 0x000074C4
		private int GetCostForInteractor(Interactor activator)
		{
			return this.cost;
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0004C19C File Offset: 0x0004A39C
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.GetCostForInteractor(activator));
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x000092CC File Offset: 0x000074CC
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.GetInteractorNextUnlockable(viewer) != null;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x000092D8 File Offset: 0x000074D8
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0004C1D4 File Offset: 0x0004A3D4
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = CostType.Lunar;
			}
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x000092E4 File Offset: 0x000074E4
		private void OnEnable()
		{
			InstanceTracker.Add<BazaarUpgradeInteraction>(this);
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x000092EC File Offset: 0x000074EC
		private void OnDisable()
		{
			InstanceTracker.Remove<BazaarUpgradeInteraction>(this);
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x000092F4 File Offset: 0x000074F4
		public bool ShouldShowOnScanner()
		{
			return this.available;
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0004C204 File Offset: 0x0004A404
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x00009345 File Offset: 0x00007545
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

		// Token: 0x06000B98 RID: 2968 RVA: 0x0004C218 File Offset: 0x0004A418
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
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
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0004C284 File Offset: 0x0004A484
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.available = reader.ReadBoolean();
			}
		}

		// Token: 0x04000F72 RID: 3954
		[SyncVar]
		public bool available = true;

		// Token: 0x04000F73 RID: 3955
		public string displayNameToken;

		// Token: 0x04000F74 RID: 3956
		public int cost;

		// Token: 0x04000F75 RID: 3957
		public string contextToken;

		// Token: 0x04000F76 RID: 3958
		public string[] unlockableProgression;

		// Token: 0x04000F77 RID: 3959
		private UnlockableDef[] unlockableProgressionDefs;

		// Token: 0x04000F78 RID: 3960
		public float activationCooldownDuration = 1f;

		// Token: 0x04000F79 RID: 3961
		private float activationTimer;

		// Token: 0x04000F7A RID: 3962
		public GameObject purchaseEffect;

		// Token: 0x04000F7B RID: 3963
		private static readonly Color32 lunarCoinColor = new Color32(198, 173, 250, byte.MaxValue);

		// Token: 0x04000F7C RID: 3964
		private static readonly string lunarCoinColorString = Util.RGBToHex(BazaarUpgradeInteraction.lunarCoinColor);
	}
}
