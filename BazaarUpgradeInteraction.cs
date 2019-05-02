using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000267 RID: 615
	public class BazaarUpgradeInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x06000B7B RID: 2939 RVA: 0x0004BE70 File Offset: 0x0004A070
		private void Awake()
		{
			this.unlockableProgressionDefs = new UnlockableDef[this.unlockableProgression.Length];
			for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
			{
				this.unlockableProgressionDefs[i] = UnlockableCatalog.GetUnlockableDef(this.unlockableProgression[i]);
			}
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x000091DB File Offset: 0x000073DB
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

		// Token: 0x06000B7D RID: 2941 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0004BEB8 File Offset: 0x0004A0B8
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

		// Token: 0x06000B7F RID: 2943 RVA: 0x0004BF44 File Offset: 0x0004A144
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

		// Token: 0x06000B80 RID: 2944 RVA: 0x00009212 File Offset: 0x00007412
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0000921F File Offset: 0x0000741F
		private string GetCostString()
		{
			return string.Format(" (<color=#{1}>{0}</color>)", this.cost, BazaarUpgradeInteraction.lunarCoinColorString);
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0000923B File Offset: 0x0000743B
		public string GetContextString(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return null;
			}
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0000925E File Offset: 0x0000745E
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

		// Token: 0x06000B84 RID: 2948 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnInteractionBegin(Interactor activator)
		{
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x00009284 File Offset: 0x00007484
		private int GetCostForInteractor(Interactor activator)
		{
			return this.cost;
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0004BF90 File Offset: 0x0004A190
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.GetCostForInteractor(activator));
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0000928C File Offset: 0x0000748C
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.GetInteractorNextUnlockable(viewer) != null;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x00009298 File Offset: 0x00007498
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0004BFC8 File Offset: 0x0004A1C8
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = CostType.Lunar;
			}
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x0004BFF8 File Offset: 0x0004A1F8
		// (set) Token: 0x06000B8E RID: 2958 RVA: 0x000092ED File Offset: 0x000074ED
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

		// Token: 0x06000B8F RID: 2959 RVA: 0x0004C00C File Offset: 0x0004A20C
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

		// Token: 0x06000B90 RID: 2960 RVA: 0x0004C078 File Offset: 0x0004A278
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

		// Token: 0x04000F6C RID: 3948
		[SyncVar]
		public bool available = true;

		// Token: 0x04000F6D RID: 3949
		public string displayNameToken;

		// Token: 0x04000F6E RID: 3950
		public int cost;

		// Token: 0x04000F6F RID: 3951
		public string contextToken;

		// Token: 0x04000F70 RID: 3952
		public string[] unlockableProgression;

		// Token: 0x04000F71 RID: 3953
		private UnlockableDef[] unlockableProgressionDefs;

		// Token: 0x04000F72 RID: 3954
		public float activationCooldownDuration = 1f;

		// Token: 0x04000F73 RID: 3955
		private float activationTimer;

		// Token: 0x04000F74 RID: 3956
		public GameObject purchaseEffect;

		// Token: 0x04000F75 RID: 3957
		private static readonly Color32 lunarCoinColor = new Color32(198, 173, 250, byte.MaxValue);

		// Token: 0x04000F76 RID: 3958
		private static readonly string lunarCoinColorString = Util.RGBToHex(BazaarUpgradeInteraction.lunarCoinColor);
	}
}
