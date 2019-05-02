using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033E RID: 830
	public class Interactor : NetworkBehaviour
	{
		// Token: 0x06001111 RID: 4369 RVA: 0x00064960 File Offset: 0x00062B60
		public GameObject FindBestInteractableObject(Ray raycastRay, float maxRaycastDistance, Vector3 overlapPosition, float overlapRadius)
		{
			LayerMask mask = LayerIndex.defaultLayer.mask | LayerIndex.world.mask | LayerIndex.pickups.mask;
			RaycastHit raycastHit;
			if (Physics.Raycast(raycastRay, out raycastHit, maxRaycastDistance, mask, QueryTriggerInteraction.Collide))
			{
				GameObject entity = EntityLocator.GetEntity(raycastHit.collider.gameObject);
				if (entity)
				{
					IInteractable component = entity.GetComponent<IInteractable>();
					if (component != null && component.GetInteractability(this) != Interactability.Disabled)
					{
						return entity;
					}
				}
			}
			Collider[] array = Physics.OverlapSphere(overlapPosition, overlapRadius, mask, QueryTriggerInteraction.Collide);
			int num = array.Length;
			GameObject result = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				Collider collider = array[i];
				GameObject entity2 = EntityLocator.GetEntity(collider.gameObject);
				if (entity2)
				{
					IInteractable component2 = entity2.GetComponent<IInteractable>();
					if (component2 != null && component2.GetInteractability(this) != Interactability.Disabled && !component2.ShouldIgnoreSpherecastForInteractibility(this))
					{
						float num3 = Vector3.Dot((collider.transform.position - overlapPosition).normalized, raycastRay.direction);
						if (num3 > num2)
						{
							num2 = num3;
							result = entity2.gameObject;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0000D046 File Offset: 0x0000B246
		[Command]
		public void CmdInteract(GameObject interactableObject)
		{
			this.PerformInteraction(interactableObject);
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00064AA4 File Offset: 0x00062CA4
		[Server]
		private void PerformInteraction(GameObject interactableObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Interactor::PerformInteraction(UnityEngine.GameObject)' called on client");
				return;
			}
			if (!interactableObject)
			{
				return;
			}
			bool flag = false;
			bool anyInteractionSucceeded = false;
			foreach (IInteractable interactable in interactableObject.GetComponents<IInteractable>())
			{
				Interactability interactability = interactable.GetInteractability(this);
				if (interactability == Interactability.Available)
				{
					interactable.OnInteractionBegin(this);
					GlobalEventManager.instance.OnInteractionBegin(this, interactable, interactableObject);
					anyInteractionSucceeded = true;
				}
				flag |= (interactability > Interactability.Disabled);
			}
			if (flag)
			{
				this.CallRpcInteractionResult(anyInteractionSucceeded);
			}
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x0000D04F File Offset: 0x0000B24F
		[ClientRpc]
		private void RpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!anyInteractionSucceeded && CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				Util.PlaySound("Play_UI_insufficient_funds", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0000D076 File Offset: 0x0000B276
		public void AttemptInteraction(GameObject interactableObject)
		{
			if (NetworkServer.active)
			{
				this.PerformInteraction(interactableObject);
				return;
			}
			this.CallCmdInteract(interactableObject);
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0000D0A1 File Offset: 0x0000B2A1
		protected static void InvokeCmdCmdInteract(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdInteract called on client.");
				return;
			}
			((Interactor)obj).CmdInteract(reader.ReadGameObject());
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00064B28 File Offset: 0x00062D28
		public void CallCmdInteract(GameObject interactableObject)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdInteract called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdInteract(interactableObject);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)Interactor.kCmdCmdInteract);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(interactableObject);
			base.SendCommandInternal(networkWriter, 0, "CmdInteract");
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0000D0CA File Offset: 0x0000B2CA
		protected static void InvokeRpcRpcInteractionResult(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcInteractionResult called on server.");
				return;
			}
			((Interactor)obj).RpcInteractionResult(reader.ReadBoolean());
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00064BB4 File Offset: 0x00062DB4
		public void CallRpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcInteractionResult called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)Interactor.kRpcRpcInteractionResult);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(anyInteractionSucceeded);
			this.SendRPCInternal(networkWriter, 0, "RpcInteractionResult");
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x00064C28 File Offset: 0x00062E28
		static Interactor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(Interactor), Interactor.kCmdCmdInteract, new NetworkBehaviour.CmdDelegate(Interactor.InvokeCmdCmdInteract));
			Interactor.kRpcRpcInteractionResult = 804118976;
			NetworkBehaviour.RegisterRpcDelegate(typeof(Interactor), Interactor.kRpcRpcInteractionResult, new NetworkBehaviour.CmdDelegate(Interactor.InvokeRpcRpcInteractionResult));
			NetworkCRC.RegisterBehaviour("Interactor", 0);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001543 RID: 5443
		public float maxInteractionDistance = 1f;

		// Token: 0x04001544 RID: 5444
		private static int kCmdCmdInteract = 591229007;

		// Token: 0x04001545 RID: 5445
		private static int kRpcRpcInteractionResult;
	}
}
