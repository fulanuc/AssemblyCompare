using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020004C0 RID: 1216
	public static class TeleportHelper
	{
		// Token: 0x06001B6E RID: 7022 RVA: 0x000889C8 File Offset: 0x00086BC8
		public static void TeleportGameObject(GameObject gameObject, Vector3 newPosition)
		{
			bool hasEffectiveAuthority = Util.HasEffectiveAuthority(gameObject);
			TeleportHelper.TeleportGameObject(gameObject, newPosition, newPosition - gameObject.transform.position, hasEffectiveAuthority);
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x000889F8 File Offset: 0x00086BF8
		private static void TeleportGameObject(GameObject gameObject, Vector3 newPosition, Vector3 delta, bool hasEffectiveAuthority)
		{
			TeleportHelper.OnTeleport(gameObject, newPosition, delta);
			if (NetworkServer.active || hasEffectiveAuthority)
			{
				TeleportHelper.TeleportMessage msg = new TeleportHelper.TeleportMessage
				{
					gameObject = gameObject,
					newPosition = newPosition,
					delta = delta
				};
				QosChannelIndex defaultReliable = QosChannelIndex.defaultReliable;
				if (NetworkServer.active)
				{
					NetworkServer.SendByChannelToAll(68, msg, defaultReliable.intVal);
					return;
				}
				NetworkManager.singleton.client.connection.SendByChannel(68, msg, defaultReliable.intVal);
			}
		}

		// Token: 0x06001B70 RID: 7024 RVA: 0x00088A6C File Offset: 0x00086C6C
		private static void OnTeleport(GameObject gameObject, Vector3 newPosition, Vector3 delta)
		{
			CharacterMotor component = gameObject.GetComponent<CharacterMotor>();
			if (component)
			{
				component.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
				component.velocity = Vector3.zero;
			}
			else
			{
				gameObject.transform.position = newPosition;
			}
			ITeleportHandler[] componentsInChildren = gameObject.GetComponentsInChildren<ITeleportHandler>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnTeleport(newPosition - delta, newPosition);
			}
		}

		// Token: 0x06001B71 RID: 7025 RVA: 0x00088AD8 File Offset: 0x00086CD8
		public static void TeleportBody(CharacterBody body, Vector3 targetFootPosition)
		{
			Vector3 b = body.footPosition - body.transform.position;
			TeleportHelper.TeleportGameObject(body.gameObject, targetFootPosition - b);
		}

		// Token: 0x06001B72 RID: 7026 RVA: 0x00088B10 File Offset: 0x00086D10
		[NetworkMessageHandler(client = true, server = true, msgType = 68)]
		private static void HandleTeleport(NetworkMessage netMsg)
		{
			if (Util.ConnectionIsLocal(netMsg.conn))
			{
				return;
			}
			netMsg.ReadMessage<TeleportHelper.TeleportMessage>(TeleportHelper.messageBuffer);
			if (!TeleportHelper.messageBuffer.gameObject)
			{
				return;
			}
			bool flag = Util.HasEffectiveAuthority(TeleportHelper.messageBuffer.gameObject);
			if (flag)
			{
				return;
			}
			TeleportHelper.TeleportGameObject(TeleportHelper.messageBuffer.gameObject, TeleportHelper.messageBuffer.newPosition, TeleportHelper.messageBuffer.delta, flag);
		}

		// Token: 0x04001DF9 RID: 7673
		private static readonly TeleportHelper.TeleportMessage messageBuffer = new TeleportHelper.TeleportMessage();

		// Token: 0x020004C1 RID: 1217
		private class TeleportMessage : MessageBase
		{
			// Token: 0x06001B75 RID: 7029 RVA: 0x00014520 File Offset: 0x00012720
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.delta);
			}

			// Token: 0x06001B76 RID: 7030 RVA: 0x00014546 File Offset: 0x00012746
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.delta = reader.ReadVector3();
			}

			// Token: 0x04001DFA RID: 7674
			public GameObject gameObject;

			// Token: 0x04001DFB RID: 7675
			public Vector3 newPosition;

			// Token: 0x04001DFC RID: 7676
			public Vector3 delta;
		}
	}
}
