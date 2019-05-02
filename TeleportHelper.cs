using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020004CE RID: 1230
	public static class TeleportHelper
	{
		// Token: 0x06001BD2 RID: 7122 RVA: 0x00089540 File Offset: 0x00087740
		public static void TeleportGameObject(GameObject gameObject, Vector3 newPosition)
		{
			bool hasEffectiveAuthority = Util.HasEffectiveAuthority(gameObject);
			TeleportHelper.TeleportGameObject(gameObject, newPosition, newPosition - gameObject.transform.position, hasEffectiveAuthority);
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x00089570 File Offset: 0x00087770
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

		// Token: 0x06001BD4 RID: 7124 RVA: 0x000895E4 File Offset: 0x000877E4
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

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00089650 File Offset: 0x00087850
		public static void TeleportBody(CharacterBody body, Vector3 targetFootPosition)
		{
			Vector3 b = body.footPosition - body.transform.position;
			TeleportHelper.TeleportGameObject(body.gameObject, targetFootPosition - b);
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00089688 File Offset: 0x00087888
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

		// Token: 0x04001E33 RID: 7731
		private static readonly TeleportHelper.TeleportMessage messageBuffer = new TeleportHelper.TeleportMessage();

		// Token: 0x020004CF RID: 1231
		private class TeleportMessage : MessageBase
		{
			// Token: 0x06001BD9 RID: 7129 RVA: 0x000149ED File Offset: 0x00012BED
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.delta);
			}

			// Token: 0x06001BDA RID: 7130 RVA: 0x00014A13 File Offset: 0x00012C13
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.delta = reader.ReadVector3();
			}

			// Token: 0x04001E34 RID: 7732
			public GameObject gameObject;

			// Token: 0x04001E35 RID: 7733
			public Vector3 newPosition;

			// Token: 0x04001E36 RID: 7734
			public Vector3 delta;
		}
	}
}
