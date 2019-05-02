using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DC RID: 988
	public class SetStateOnHurt : NetworkBehaviour
	{
		// Token: 0x0600157F RID: 5503 RVA: 0x00010414 File Offset: 0x0000E614
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00010422 File Offset: 0x0000E622
		public override void OnStopAuthority()
		{
			base.OnStopAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x00010430 File Offset: 0x0000E630
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x00010443 File Offset: 0x0000E643
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.UpdateAuthority();
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x00073438 File Offset: 0x00071638
		private void OnTakeDamage(DamageInfo damageInfo)
		{
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if (this.canBeFrozen && (damageInfo.damageType & DamageType.Freeze2s) != DamageType.Generic)
				{
					this.SetFrozen(2f * damageInfo.procCoefficient);
					return;
				}
				if (!this.characterBody.healthComponent.isInFrozenState)
				{
					if (this.canBeStunned && (damageInfo.damageType & DamageType.Stun1s) != DamageType.Generic)
					{
						this.SetStun(1f);
						return;
					}
					if (this.canBeHitStunned && num > this.characterBody.maxHealth * this.hitThreshold)
					{
						this.SetPain();
					}
				}
			}
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00010457 File Offset: 0x0000E657
		[Server]
		public void SetStun(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetStun(System.Single)' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetStunInternal(duration);
				return;
			}
			this.CallRpcSetStun(duration);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x00010485 File Offset: 0x0000E685
		[ClientRpc]
		private void RpcSetStun(float duration)
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetStunInternal(duration);
			}
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x0007350C File Offset: 0x0007170C
		private void SetStunInternal(float duration)
		{
			if (this.targetStateMachine)
			{
				StunState stunState = new StunState();
				stunState.stunDuration = duration;
				this.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x00010496 File Offset: 0x0000E696
		[Server]
		public void SetFrozen(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetFrozen(System.Single)' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetFrozenInternal(duration);
				return;
			}
			this.CallRpcSetFrozen(duration);
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x000104C4 File Offset: 0x0000E6C4
		[ClientRpc]
		private void RpcSetFrozen(float duration)
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetFrozenInternal(duration);
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x00073564 File Offset: 0x00071764
		private void SetFrozenInternal(float duration)
		{
			if (this.targetStateMachine)
			{
				FrozenState frozenState = new FrozenState();
				frozenState.freezeDuration = duration;
				this.targetStateMachine.SetInterruptState(frozenState, InterruptPriority.Frozen);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x000104D5 File Offset: 0x0000E6D5
		[Server]
		public void SetPain()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetPain()' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetPainInternal();
				return;
			}
			this.CallRpcSetPain();
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x00010501 File Offset: 0x0000E701
		[ClientRpc]
		private void RpcSetPain()
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetPainInternal();
			}
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x000735BC File Offset: 0x000717BC
		private void SetPainInternal()
		{
			if (this.targetStateMachine)
			{
				this.targetStateMachine.SetInterruptState(EntityState.Instantiate(this.hurtState), InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x00010540 File Offset: 0x0000E740
		protected static void InvokeRpcRpcSetStun(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetStun called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetStun(reader.ReadSingle());
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0001056A File Offset: 0x0000E76A
		protected static void InvokeRpcRpcSetFrozen(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetFrozen called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetFrozen(reader.ReadSingle());
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x00010594 File Offset: 0x0000E794
		protected static void InvokeRpcRpcSetPain(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetPain called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetPain();
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x00073610 File Offset: 0x00071810
		public void CallRpcSetStun(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetStun called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetStun);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(duration);
			this.SendRPCInternal(networkWriter, 0, "RpcSetStun");
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x00073684 File Offset: 0x00071884
		public void CallRpcSetFrozen(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetFrozen called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetFrozen);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(duration);
			this.SendRPCInternal(networkWriter, 0, "RpcSetFrozen");
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x000736F8 File Offset: 0x000718F8
		public void CallRpcSetPain()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetPain called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetPain);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcSetPain");
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x00073764 File Offset: 0x00071964
		static SetStateOnHurt()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetStun, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetStun));
			SetStateOnHurt.kRpcRpcSetFrozen = 1781279215;
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetFrozen, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetFrozen));
			SetStateOnHurt.kRpcRpcSetPain = 788726245;
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetPain, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetPain));
			NetworkCRC.RegisterBehaviour("SetStateOnHurt", 0);
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018B2 RID: 6322
		[Tooltip("The percentage of their max HP they need to take to get stunned. Ranges from 0-1.")]
		public float hitThreshold = 0.1f;

		// Token: 0x040018B3 RID: 6323
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x040018B4 RID: 6324
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x040018B5 RID: 6325
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x040018B6 RID: 6326
		public bool canBeHitStunned = true;

		// Token: 0x040018B7 RID: 6327
		public bool canBeStunned = true;

		// Token: 0x040018B8 RID: 6328
		public bool canBeFrozen = true;

		// Token: 0x040018B9 RID: 6329
		private bool hasEffectiveAuthority = true;

		// Token: 0x040018BA RID: 6330
		private CharacterBody characterBody;

		// Token: 0x040018BB RID: 6331
		private static int kRpcRpcSetStun = 788834249;

		// Token: 0x040018BC RID: 6332
		private static int kRpcRpcSetFrozen;

		// Token: 0x040018BD RID: 6333
		private static int kRpcRpcSetPain;
	}
}
