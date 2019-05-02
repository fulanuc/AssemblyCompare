using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000570 RID: 1392
	public class CharacterNetworkTransformManager : MonoBehaviour
	{
		// Token: 0x06001F21 RID: 7969 RVA: 0x00016C99 File Offset: 0x00014E99
		private void Awake()
		{
			CharacterNetworkTransformManager.instance = this;
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x00016CA1 File Offset: 0x00014EA1
		[NetworkMessageHandler(msgType = 51, client = true, server = true)]
		private static void HandleTransformUpdates(NetworkMessage netMsg)
		{
			if (CharacterNetworkTransformManager.instance)
			{
				CharacterNetworkTransformManager.instance.HandleTransformUpdatesInternal(netMsg);
			}
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x00098B24 File Offset: 0x00096D24
		private void HandleTransformUpdatesInternal(NetworkMessage netMsg)
		{
			uint num = (uint)netMsg.reader.ReadByte();
			float filteredClientRTT = GameNetworkManager.singleton.filteredClientRTT;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				netMsg.ReadMessage<CharacterNetworkTransformManager.CharacterUpdateMessage>(this.currentInMessage);
				GameObject gameObject = this.currentInMessage.gameObject;
				if (gameObject && (!NetworkServer.active || gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner == netMsg.conn))
				{
					CharacterNetworkTransform component = gameObject.GetComponent<CharacterNetworkTransform>();
					if (component && !component.hasEffectiveAuthority)
					{
						CharacterNetworkTransform.Snapshot snapshot = new CharacterNetworkTransform.Snapshot
						{
							serverTime = this.currentInMessage.timestamp,
							position = this.currentInMessage.newPosition,
							moveVector = this.currentInMessage.moveVector,
							aimDirection = this.currentInMessage.aimDirection,
							rotation = this.currentInMessage.rotation,
							isGrounded = this.currentInMessage.isGrounded
						};
						if (NetworkClient.active)
						{
							snapshot.serverTime += filteredClientRTT;
						}
						component.PushSnapshot(snapshot);
						if (NetworkServer.active)
						{
							this.snapshotQueue.Enqueue(new CharacterNetworkTransformManager.NetSnapshot
							{
								gameObject = component.gameObject,
								snapshot = snapshot
							});
						}
					}
				}
				num2++;
			}
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x00098C84 File Offset: 0x00096E84
		private void ProcessQueue()
		{
			if (this.snapshotQueue.Count == 0)
			{
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(51);
			int num = Mathf.Min(Mathf.FloorToInt((float)(1000 - networkWriter.Position) / 61f), this.snapshotQueue.Count);
			networkWriter.Write((byte)num);
			for (int i = 0; i < num; i++)
			{
				CharacterNetworkTransformManager.NetSnapshot netSnapshot = this.snapshotQueue.Dequeue();
				this.currentOutMessage.gameObject = netSnapshot.gameObject;
				this.currentOutMessage.newPosition = netSnapshot.snapshot.position;
				this.currentOutMessage.aimDirection = netSnapshot.snapshot.aimDirection;
				this.currentOutMessage.moveVector = netSnapshot.snapshot.moveVector;
				this.currentOutMessage.rotation = netSnapshot.snapshot.rotation;
				this.currentOutMessage.isGrounded = netSnapshot.snapshot.isGrounded;
				this.currentOutMessage.timestamp = netSnapshot.snapshot.serverTime;
				networkWriter.Write(this.currentOutMessage);
			}
			networkWriter.FinishMessage();
			if (NetworkServer.active)
			{
				NetworkServer.SendWriterToReady(null, networkWriter, QosChannelIndex.characterTransformUnreliable.intVal);
				return;
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendWriter(networkWriter, QosChannelIndex.characterTransformUnreliable.intVal);
			}
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x00098DDC File Offset: 0x00096FDC
		private void FixedUpdate()
		{
			if (!NetworkManager.singleton)
			{
				return;
			}
			ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList = CharacterNetworkTransform.readOnlyInstancesList;
			float fixedTime = Time.fixedTime;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CharacterNetworkTransform characterNetworkTransform = readOnlyInstancesList[i];
				if (characterNetworkTransform.hasEffectiveAuthority && fixedTime - characterNetworkTransform.lastPositionTransmitTime > characterNetworkTransform.positionTransmitInterval)
				{
					characterNetworkTransform.lastPositionTransmitTime = fixedTime;
					this.snapshotQueue.Enqueue(new CharacterNetworkTransformManager.NetSnapshot
					{
						gameObject = characterNetworkTransform.gameObject,
						snapshot = characterNetworkTransform.newestNetSnapshot
					});
				}
			}
			while (this.snapshotQueue.Count > 0)
			{
				this.ProcessQueue();
			}
		}

		// Token: 0x040021BC RID: 8636
		private static CharacterNetworkTransformManager instance;

		// Token: 0x040021BD RID: 8637
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentInMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021BE RID: 8638
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentOutMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021BF RID: 8639
		private readonly Queue<CharacterNetworkTransformManager.NetSnapshot> snapshotQueue = new Queue<CharacterNetworkTransformManager.NetSnapshot>();

		// Token: 0x02000571 RID: 1393
		private class CharacterUpdateMessage : MessageBase
		{
			// Token: 0x06001F28 RID: 7976 RVA: 0x00098E80 File Offset: 0x00097080
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.aimDirection);
				writer.Write(this.moveVector);
				writer.Write(this.rotation);
				writer.Write(this.timestamp);
				writer.Write(this.isGrounded);
			}

			// Token: 0x06001F29 RID: 7977 RVA: 0x00098EE4 File Offset: 0x000970E4
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.aimDirection = reader.ReadVector3();
				this.moveVector = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
				this.timestamp = reader.ReadSingle();
				this.isGrounded = reader.ReadBoolean();
			}

			// Token: 0x040021C0 RID: 8640
			public GameObject gameObject;

			// Token: 0x040021C1 RID: 8641
			public Vector3 newPosition;

			// Token: 0x040021C2 RID: 8642
			public Vector3 aimDirection;

			// Token: 0x040021C3 RID: 8643
			public Vector3 moveVector;

			// Token: 0x040021C4 RID: 8644
			public Quaternion rotation;

			// Token: 0x040021C5 RID: 8645
			public float timestamp;

			// Token: 0x040021C6 RID: 8646
			public bool isGrounded;

			// Token: 0x040021C7 RID: 8647
			public const int maxNetworkSize = 61;
		}

		// Token: 0x02000572 RID: 1394
		public struct NetSnapshot
		{
			// Token: 0x040021C8 RID: 8648
			public GameObject gameObject;

			// Token: 0x040021C9 RID: 8649
			public CharacterNetworkTransform.Snapshot snapshot;

			// Token: 0x040021CA RID: 8650
			public const int maxNetworkSize = 61;
		}
	}
}
