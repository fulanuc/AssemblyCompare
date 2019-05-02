using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000300 RID: 768
	public sealed class GenericInteraction : NetworkBehaviour, IInteractable
	{
		// Token: 0x06000F93 RID: 3987 RVA: 0x0000BF7B File Offset: 0x0000A17B
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

		// Token: 0x06000F94 RID: 3988 RVA: 0x0000BF99 File Offset: 0x0000A199
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

		// Token: 0x06000F95 RID: 3989 RVA: 0x0000BFB7 File Offset: 0x0000A1B7
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

		// Token: 0x06000F96 RID: 3990 RVA: 0x0000BFD5 File Offset: 0x0000A1D5
		string IInteractable.GetContextString(Interactor activator)
		{
			if (this.contextToken == "")
			{
				return null;
			}
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0000BFF6 File Offset: 0x0000A1F6
		Interactability IInteractable.GetInteractability(Interactor activator)
		{
			return this.interactability;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0000BFFE File Offset: 0x0000A1FE
		void IInteractable.OnInteractionBegin(Interactor activator)
		{
			this.onActivation.Invoke();
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0000C00B File Offset: 0x0000A20B
		private void OnEnable()
		{
			InstanceTracker.Add<GenericInteraction>(this);
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0000C013 File Offset: 0x0000A213
		private void OnDisable()
		{
			InstanceTracker.Remove<GenericInteraction>(this);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0000C01B File Offset: 0x0000A21B
		public bool ShouldShowOnScanner()
		{
			return this.shouldShowOnScanner && this.interactability > Interactability.Disabled;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x0005D5A4 File Offset: 0x0005B7A4
		// (set) Token: 0x06000FA0 RID: 4000 RVA: 0x0000C046 File Offset: 0x0000A246
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

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0005D5B8 File Offset: 0x0005B7B8
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

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0005D624 File Offset: 0x0005B824
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

		// Token: 0x040013A9 RID: 5033
		[SyncVar]
		public Interactability interactability = Interactability.Available;

		// Token: 0x040013AA RID: 5034
		public string contextToken;

		// Token: 0x040013AB RID: 5035
		public UnityEvent onActivation;

		// Token: 0x040013AC RID: 5036
		public bool shouldShowOnScanner = true;
	}
}
