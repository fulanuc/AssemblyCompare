using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033C RID: 828
	public class Interactor : NetworkBehaviour
	{
		// Token: 0x060010FD RID: 4349 RVA: 0x00064720 File Offset: 0x00062920
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

		// Token: 0x060010FE RID: 4350 RVA: 0x0000CF5D File Offset: 0x0000B15D
		[Command]
		public void CmdInteract(GameObject interactableObject)
		{
			this.PerformInteraction(interactableObject);
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00064864 File Offset: 0x00062A64
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

		// Token: 0x06001100 RID: 4352 RVA: 0x0000CF66 File Offset: 0x0000B166
		[ClientRpc]
		private void RpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!anyInteractionSucceeded && CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				Util.PlaySound("Play_UI_insufficient_funds", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0000CF8D File Offset: 0x0000B18D
		public void AttemptInteraction(GameObject interactableObject)
		{
			if (NetworkServer.active)
			{
				this.PerformInteraction(interactableObject);
				return;
			}
			this.CallCmdInteract(interactableObject);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0000CFB8 File Offset: 0x0000B1B8
		protected static void InvokeCmdCmdInteract(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdInteract called on client.");
				return;
			}
			((Interactor)obj).CmdInteract(reader.ReadGameObject());
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x000648E8 File Offset: 0x00062AE8
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

		// Token: 0x06001106 RID: 4358 RVA: 0x0000CFE1 File Offset: 0x0000B1E1
		protected static void InvokeRpcRpcInteractionResult(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcInteractionResult called on server.");
				return;
			}
			((Interactor)obj).RpcInteractionResult(reader.ReadBoolean());
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00064974 File Offset: 0x00062B74
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

		// Token: 0x06001108 RID: 4360 RVA: 0x000649E8 File Offset: 0x00062BE8
		static Interactor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(Interactor), Interactor.kCmdCmdInteract, new NetworkBehaviour.CmdDelegate(Interactor.InvokeCmdCmdInteract));
			Interactor.kRpcRpcInteractionResult = 804118976;
			NetworkBehaviour.RegisterRpcDelegate(typeof(Interactor), Interactor.kRpcRpcInteractionResult, new NetworkBehaviour.CmdDelegate(Interactor.InvokeRpcRpcInteractionResult));
			NetworkCRC.RegisterBehaviour("Interactor", 0);
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400152F RID: 5423
		public float maxInteractionDistance = 1f;

		// Token: 0x04001530 RID: 5424
		private static int kCmdCmdInteract = 591229007;

		// Token: 0x04001531 RID: 5425
		private static int kRpcRpcInteractionResult;
	}
}
