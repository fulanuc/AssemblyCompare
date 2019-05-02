using System;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000265 RID: 613
	public class BarrelInteraction : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000B5F RID: 2911 RVA: 0x00009130 File Offset: 0x00007330
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00009149 File Offset: 0x00007349
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.opened)
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0004BA9C File Offset: 0x00049C9C
		private void Start()
		{
			if (Run.instance)
			{
				this.goldReward = (int)((float)this.goldReward * Run.instance.difficultyCoefficient);
				this.expReward = (uint)(this.expReward * Run.instance.difficultyCoefficient);
			}
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0004BAE8 File Offset: 0x00049CE8
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

		// Token: 0x06000B64 RID: 2916 RVA: 0x0004BB88 File Offset: 0x00049D88
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

		// Token: 0x06000B65 RID: 2917 RVA: 0x00009156 File Offset: 0x00007356
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x0004BBE4 File Offset: 0x00049DE4
		// (set) Token: 0x06000B6A RID: 2922 RVA: 0x00009176 File Offset: 0x00007376
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

		// Token: 0x06000B6B RID: 2923 RVA: 0x0004BBF8 File Offset: 0x00049DF8
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

		// Token: 0x06000B6C RID: 2924 RVA: 0x0004BC64 File Offset: 0x00049E64
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

		// Token: 0x04000F5F RID: 3935
		public int goldReward;

		// Token: 0x04000F60 RID: 3936
		public uint expReward;

		// Token: 0x04000F61 RID: 3937
		public string displayNameToken = "BARREL1_NAME";

		// Token: 0x04000F62 RID: 3938
		public string contextToken;

		// Token: 0x04000F63 RID: 3939
		[SyncVar]
		private bool opened;
	}
}
