using System;
using System.Collections.Generic;
using EntityStates;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000374 RID: 884
	[DisallowMultipleComponent]
	public class NetworkStateMachine : NetworkBehaviour
	{
		// Token: 0x06001251 RID: 4689 RVA: 0x000689F0 File Offset: 0x00066BF0
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				this.stateMachines[i].networkIndex = i;
			}
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00068A2C File Offset: 0x00066C2C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			if (initialState)
			{
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					EntityStateMachine entityStateMachine = this.stateMachines[i];
					writer.WritePackedUInt32((uint)(StateIndexTable.TypeToIndex(entityStateMachine.state.GetType()) + 1));
					entityStateMachine.state.OnSerialize(writer);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00068A80 File Offset: 0x00066C80
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					EntityStateMachine entityStateMachine = this.stateMachines[i];
					short stateTypeIndex = (short)reader.ReadPackedUInt32() - 1;
					if (!base.hasAuthority)
					{
						EntityState entityState = EntityState.Instantiate(stateTypeIndex);
						if (entityState != null)
						{
							entityState.OnDeserialize(reader);
							if (!this.stateMachines[i])
							{
								Debug.LogErrorFormat("State machine [{0}] on object {1} is not set! incoming state = {2}", new object[]
								{
									i,
									base.gameObject,
									entityState.GetType()
								});
							}
							entityStateMachine.SetNextState(entityState);
						}
					}
				}
			}
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x00068B14 File Offset: 0x00066D14
		[NetworkMessageHandler(msgType = 48, client = true, server = true)]
		public static void HandleSetEntityState(NetworkMessage netMsg)
		{
			NetworkIdentity networkIdentity = netMsg.reader.ReadNetworkIdentity();
			byte b = netMsg.reader.ReadByte();
			short stateTypeIndex = netMsg.reader.ReadInt16();
			if (networkIdentity == null)
			{
				return;
			}
			NetworkStateMachine component = networkIdentity.gameObject.GetComponent<NetworkStateMachine>();
			if (component == null || (int)b >= component.stateMachines.Length)
			{
				return;
			}
			EntityStateMachine entityStateMachine = component.stateMachines[(int)b];
			if (entityStateMachine == null)
			{
				return;
			}
			if (networkIdentity.isServer)
			{
				HashSet<NetworkInstanceId> clientOwnedObjects = netMsg.conn.clientOwnedObjects;
				if (clientOwnedObjects == null || !clientOwnedObjects.Contains(networkIdentity.netId))
				{
					return;
				}
			}
			else if (networkIdentity.hasAuthority)
			{
				return;
			}
			EntityState entityState = EntityState.Instantiate(stateTypeIndex);
			if (entityState == null)
			{
				return;
			}
			entityState.OnDeserialize(netMsg.reader);
			entityStateMachine.SetState(entityState);
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00068BDC File Offset: 0x00066DDC
		public void SendSetEntityState(int stateMachineIndex)
		{
			if (!NetworkServer.active && !base.hasAuthority)
			{
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			EntityStateMachine entityStateMachine = this.stateMachines[stateMachineIndex];
			short value = StateIndexTable.TypeToIndex(entityStateMachine.state.GetType());
			networkWriter.StartMessage(48);
			networkWriter.Write(this.networkIdentity);
			networkWriter.Write((byte)stateMachineIndex);
			networkWriter.Write(value);
			entityStateMachine.state.OnSerialize(networkWriter);
			networkWriter.FinishMessage();
			if (NetworkServer.active)
			{
				NetworkServer.SendWriterToReady(base.gameObject, networkWriter, this.GetNetworkChannel());
				return;
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendWriter(networkWriter, this.GetNetworkChannel());
			}
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x00068C80 File Offset: 0x00066E80
		private void OnValidate()
		{
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				if (!this.stateMachines[i])
				{
					Debug.LogErrorFormat("{0} has a blank entry for NetworkStateMachine!", new object[]
					{
						base.gameObject
					});
				}
			}
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0400162D RID: 5677
		[Tooltip("The sibling state machine components to network.")]
		[SerializeField]
		private EntityStateMachine[] stateMachines;

		// Token: 0x0400162E RID: 5678
		private NetworkIdentity networkIdentity;
	}
}
