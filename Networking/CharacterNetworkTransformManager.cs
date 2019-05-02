using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200057F RID: 1407
	public class CharacterNetworkTransformManager : MonoBehaviour
	{
		// Token: 0x06001F8B RID: 8075 RVA: 0x00017178 File Offset: 0x00015378
		private void Awake()
		{
			CharacterNetworkTransformManager.instance = this;
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00017180 File Offset: 0x00015380
		[NetworkMessageHandler(msgType = 51, client = true, server = true)]
		private static void HandleTransformUpdates(NetworkMessage netMsg)
		{
			if (CharacterNetworkTransformManager.instance)
			{
				CharacterNetworkTransformManager.instance.HandleTransformUpdatesInternal(netMsg);
			}
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x00099840 File Offset: 0x00097A40
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

		// Token: 0x06001F8E RID: 8078 RVA: 0x000999A0 File Offset: 0x00097BA0
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

		// Token: 0x06001F8F RID: 8079 RVA: 0x00099AF8 File Offset: 0x00097CF8
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

		// Token: 0x040021FA RID: 8698
		private static CharacterNetworkTransformManager instance;

		// Token: 0x040021FB RID: 8699
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentInMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021FC RID: 8700
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentOutMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021FD RID: 8701
		private readonly Queue<CharacterNetworkTransformManager.NetSnapshot> snapshotQueue = new Queue<CharacterNetworkTransformManager.NetSnapshot>();

		// Token: 0x02000580 RID: 1408
		private class CharacterUpdateMessage : MessageBase
		{
			// Token: 0x06001F92 RID: 8082 RVA: 0x00099B9C File Offset: 0x00097D9C
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

			// Token: 0x06001F93 RID: 8083 RVA: 0x00099C00 File Offset: 0x00097E00
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

			// Token: 0x040021FE RID: 8702
			public GameObject gameObject;

			// Token: 0x040021FF RID: 8703
			public Vector3 newPosition;

			// Token: 0x04002200 RID: 8704
			public Vector3 aimDirection;

			// Token: 0x04002201 RID: 8705
			public Vector3 moveVector;

			// Token: 0x04002202 RID: 8706
			public Quaternion rotation;

			// Token: 0x04002203 RID: 8707
			public float timestamp;

			// Token: 0x04002204 RID: 8708
			public bool isGrounded;

			// Token: 0x04002205 RID: 8709
			public const int maxNetworkSize = 61;
		}

		// Token: 0x02000581 RID: 1409
		public struct NetSnapshot
		{
			// Token: 0x04002206 RID: 8710
			public GameObject gameObject;

			// Token: 0x04002207 RID: 8711
			public CharacterNetworkTransform.Snapshot snapshot;

			// Token: 0x04002208 RID: 8712
			public const int maxNetworkSize = 61;
		}
	}
}
