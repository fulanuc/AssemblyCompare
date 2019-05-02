using System;
using EntityStates;
using EntityStates.SurvivorPod;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F2 RID: 1010
	[RequireComponent(typeof(EntityStateMachine))]
	public class SurvivorPodController : NetworkBehaviour, ICameraStateProvider, IInteractable
	{
		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x000107FD File Offset: 0x0000E9FD
		// (set) Token: 0x06001611 RID: 5649 RVA: 0x00010805 File Offset: 0x0000EA05
		public EntityStateMachine characterStateMachine { get; private set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06001612 RID: 5650 RVA: 0x0001080E File Offset: 0x0000EA0E
		// (set) Token: 0x06001613 RID: 5651 RVA: 0x00010816 File Offset: 0x0000EA16
		public Transform characterTransform { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06001614 RID: 5652 RVA: 0x0001081F File Offset: 0x0000EA1F
		// (set) Token: 0x06001615 RID: 5653 RVA: 0x00010827 File Offset: 0x0000EA27
		public InputBankTest characterInputBank { get; private set; }

		// Token: 0x06001616 RID: 5654 RVA: 0x0007600C File Offset: 0x0007420C
		private void OnSyncBodyObject(GameObject newCharacterBodyObject)
		{
			this.characterStateMachine = (newCharacterBodyObject ? newCharacterBodyObject.GetComponent<EntityStateMachine>() : null);
			this.characterTransform = (newCharacterBodyObject ? newCharacterBodyObject.transform : null);
			if (this.characterStateMachine)
			{
				this.characterStateMachine.SetState(new GenericCharacterPod());
			}
			this.UpdateCameras(newCharacterBodyObject);
			base.enabled = newCharacterBodyObject;
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x00076078 File Offset: 0x00074278
		private void UpdateCameras(GameObject characterBodyObject)
		{
			foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
			{
				if (characterBodyObject && cameraRigController.target == characterBodyObject)
				{
					cameraRigController.SetOverrideCam(this, 0f);
				}
				else if (cameraRigController.IsOverrideCam(this))
				{
					cameraRigController.SetOverrideCam(null, 0.05f);
				}
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x00010830 File Offset: 0x0000EA30
		private void Awake()
		{
			this.stateMachine = base.GetComponent<EntityStateMachine>();
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x0001083E File Offset: 0x0000EA3E
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.NetworkcharacterBodyObject = this.characterBodyObject;
			}
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00010853 File Offset: 0x0000EA53
		private void Update()
		{
			this.UpdateCameras(this.characterBodyObject);
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00010861 File Offset: 0x0000EA61
		private void FixedUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00010861 File Offset: 0x0000EA61
		private void LateUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x000760F8 File Offset: 0x000742F8
		private void UpdatePassengerPosition()
		{
			if (this.characterTransform && this.characterStateMachine && this.characterStateMachine.state is GenericCharacterPod)
			{
				this.characterTransform.position = this.holdingPosition.position;
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x00076148 File Offset: 0x00074348
		CameraState ICameraStateProvider.GetCameraState(CameraRigController cameraRigController)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.cameraBone.position;
			Vector3 direction = position2 - position;
			Ray ray = new Ray(position, direction);
			Vector3 position3 = position2;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, direction.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				position3 = ray.GetPoint(Mathf.Max(raycastHit.distance - 0.25f, 0.25f));
			}
			return new CameraState
			{
				position = position3,
				rotation = this.cameraBone.rotation,
				fov = 60f
			};
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x00010869 File Offset: 0x0000EA69
		Interactability IInteractable.GetInteractability(Interactor interactor)
		{
			if (!(interactor.gameObject == this.characterBodyObject) || !(this.stateMachine.state is Landed))
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00010893 File Offset: 0x0000EA93
		void IInteractable.OnInteractionBegin(Interactor interactor)
		{
			this.stateMachine.SetNextState(new PreRelease());
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000108A5 File Offset: 0x0000EAA5
		string IInteractable.GetContextString(Interactor activator)
		{
			return Language.GetString("SURVIVOR_POD_HATCH_OPEN_CONTEXT");
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001625 RID: 5669 RVA: 0x000761F8 File Offset: 0x000743F8
		// (set) Token: 0x06001626 RID: 5670 RVA: 0x0007620C File Offset: 0x0007440C
		public GameObject NetworkcharacterBodyObject
		{
			get
			{
				return this.characterBodyObject;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncBodyObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.characterBodyObject, dirtyBit, ref this.___characterBodyObjectNetId);
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0007625C File Offset: 0x0007445C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.characterBodyObject);
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
				writer.Write(this.characterBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x000762C8 File Offset: 0x000744C8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___characterBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncBodyObject(reader.ReadGameObject());
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x000108B1 File Offset: 0x0000EAB1
		public override void PreStartClient()
		{
			if (!this.___characterBodyObjectNetId.IsEmpty())
			{
				this.NetworkcharacterBodyObject = ClientScene.FindLocalObject(this.___characterBodyObjectNetId);
			}
		}

		// Token: 0x04001968 RID: 6504
		[SyncVar(hook = "OnSyncBodyObject")]
		[NonSerialized]
		public GameObject characterBodyObject;

		// Token: 0x0400196C RID: 6508
		private EntityStateMachine stateMachine;

		// Token: 0x0400196D RID: 6509
		[Tooltip("The bone which controls the camera during the entry animation.")]
		public Transform cameraBone;

		// Token: 0x0400196E RID: 6510
		[Tooltip("The transform at which the survivor will be held until they exit the pod.")]
		public Transform holdingPosition;

		// Token: 0x0400196F RID: 6511
		[Tooltip("The transform at which the survivor will be placed upon exiting the pod.")]
		public Transform exitPosition;

		// Token: 0x04001970 RID: 6512
		[Tooltip("The animator for the pod.")]
		public Animator animator;

		// Token: 0x04001971 RID: 6513
		private NetworkInstanceId ___characterBodyObjectNetId;
	}
}
