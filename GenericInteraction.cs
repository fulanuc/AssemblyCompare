using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FD RID: 765
	public class GenericInteraction : NetworkBehaviour, IInteractable
	{
		// Token: 0x06000F83 RID: 3971 RVA: 0x0000BECD File Offset: 0x0000A0CD
		[Server]
		public void SetInteractabilityAvailable()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityAvailable()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.Available;
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0000BEEB File Offset: 0x0000A0EB
		[Server]
		public void SetInteractabilityConditionsNotMet()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityConditionsNotMet()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.ConditionsNotMet;
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x0000BF09 File Offset: 0x0000A109
		[Server]
		public void SetInteractabilityDisabled()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericInteraction::SetInteractabilityDisabled()' called on client");
				return;
			}
			this.Networkinteractability = Interactability.Disabled;
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x0000BF27 File Offset: 0x0000A127
		string IInteractable.GetContextString(Interactor activator)
		{
			if (this.contextToken == "")
			{
				return null;
			}
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0000BF48 File Offset: 0x0000A148
		Interactability IInteractable.GetInteractability(Interactor activator)
		{
			return this.interactability;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0000BF50 File Offset: 0x0000A150
		void IInteractable.OnInteractionBegin(Interactor activator)
		{
			this.onActivation.Invoke();
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x0005D384 File Offset: 0x0005B584
		// (set) Token: 0x06000F8D RID: 3981 RVA: 0x0000BF6C File Offset: 0x0000A16C
		public Interactability Networkinteractability
		{
			get
			{
				return this.interactability;
			}
			set
			{
				base.SetSyncVar<Interactability>(value, ref this.interactability, 1u);
			}
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0005D398 File Offset: 0x0005B598
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.interactability);
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
				writer.Write((int)this.interactability);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x0005D404 File Offset: 0x0005B604
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.interactability = (Interactability)reader.ReadInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.interactability = (Interactability)reader.ReadInt32();
			}
		}

		// Token: 0x04001392 RID: 5010
		[SyncVar]
		public Interactability interactability = Interactability.Available;

		// Token: 0x04001393 RID: 5011
		public string contextToken;

		// Token: 0x04001394 RID: 5012
		public UnityEvent onActivation;
	}
}
