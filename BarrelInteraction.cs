using System;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000265 RID: 613
	public sealed class BarrelInteraction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000B62 RID: 2914 RVA: 0x00009155 File Offset: 0x00007355
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0000916E File Offset: 0x0000736E
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.opened)
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0004BCA8 File Offset: 0x00049EA8
		private void Start()
		{
			if (Run.instance)
			{
				this.goldReward = (int)((float)this.goldReward * Run.instance.difficultyCoefficient);
				this.expReward = (uint)(this.expReward * Run.instance.difficultyCoefficient);
			}
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0004BCF4 File Offset: 0x00049EF4
		[Server]
		public void OnInteractionBegin(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BarrelInteraction::OnInteractionBegin(RoR2.Interactor)' called on client");
				return;
			}
			if (!this.opened)
			{
				this.Networkopened = true;
				EntityStateMachine component = base.GetComponent<EntityStateMachine>();
				if (component)
				{
					component.SetNextState(new Opening());
				}
				CharacterBody component2 = activator.GetComponent<CharacterBody>();
				if (component2)
				{
					TeamIndex objectTeam = TeamComponent.GetObjectTeam(component2.gameObject);
					TeamManager.instance.GiveTeamMoney(objectTeam, (uint)this.goldReward);
				}
				this.CoinDrop();
				ExperienceManager.instance.AwardExperience(base.transform.position, activator.GetComponent<CharacterBody>(), (ulong)this.expReward);
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0004BD94 File Offset: 0x00049F94
		[Server]
		private void CoinDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BarrelInteraction::CoinDrop()' called on client");
				return;
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
			{
				origin = base.transform.position,
				genericFloat = (float)this.goldReward
			}, true);
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0000917B File Offset: 0x0000737B
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x00009188 File Offset: 0x00007388
		public void OnEnable()
		{
			InstanceTracker.Add<BarrelInteraction>(this);
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00009190 File Offset: 0x00007390
		public void OnDisable()
		{
			InstanceTracker.Remove<BarrelInteraction>(this);
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00009198 File Offset: 0x00007398
		public bool ShouldShowOnScanner()
		{
			return !this.opened;
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000B6F RID: 2927 RVA: 0x0004BDF0 File Offset: 0x00049FF0
		// (set) Token: 0x06000B70 RID: 2928 RVA: 0x000091B6 File Offset: 0x000073B6
		public bool Networkopened
		{
			get
			{
				return this.opened;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.opened, 1u);
			}
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0004BE04 File Offset: 0x0004A004
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.opened);
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
				writer.Write(this.opened);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0004BE70 File Offset: 0x0004A070
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.opened = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.opened = reader.ReadBoolean();
			}
		}

		// Token: 0x04000F65 RID: 3941
		public int goldReward;

		// Token: 0x04000F66 RID: 3942
		public uint expReward;

		// Token: 0x04000F67 RID: 3943
		public string displayNameToken = "BARREL1_NAME";

		// Token: 0x04000F68 RID: 3944
		public string contextToken;

		// Token: 0x04000F69 RID: 3945
		[SyncVar]
		private bool opened;
	}
}
