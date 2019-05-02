using System;
using System.Collections.Generic;
using EntityStates;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000371 RID: 881
	[DisallowMultipleComponent]
	public class NetworkStateMachine : NetworkBehaviour
	{
		// Token: 0x0600123A RID: 4666 RVA: 0x000686B8 File Offset: 0x000668B8
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				this.stateMachines[i].networkIndex = i;
			}
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x000686F4 File Offset: 0x000668F4
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

		// Token: 0x0600123C RID: 4668 RVA: 0x00068748 File Offset: 0x00066948
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

		// Token: 0x0600123D RID: 4669 RVA: 0x000687DC File Offset: 0x000669DC
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

		// Token: 0x0600123E RID: 4670 RVA: 0x000688A4 File Offset: 0x00066AA4
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

		// Token: 0x0600123F RID: 4671 RVA: 0x00068948 File Offset: 0x00066B48
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

		// Token: 0x06001241 RID: 4673 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x04001614 RID: 5652
		[Tooltip("The sibling state machine components to network.")]
		[SerializeField]
		private EntityStateMachine[] stateMachines;

		// Token: 0x04001615 RID: 5653
		private NetworkIdentity networkIdentity;
	}
}
