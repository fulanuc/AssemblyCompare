using System;
using EntityStates;
using EntityStates.SurvivorPod;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F8 RID: 1016
	[RequireComponent(typeof(EntityStateMachine))]
	public sealed class SurvivorPodController : NetworkBehaviour, ICameraStateProvider, IInteractable
	{
		// Token: 0x17000201 RID: 513
		// (get) Token: 0x0600164D RID: 5709 RVA: 0x00010C06 File Offset: 0x0000EE06
		// (set) Token: 0x0600164E RID: 5710 RVA: 0x00010C0E File Offset: 0x0000EE0E
		public EntityStateMachine characterStateMachine { get; private set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600164F RID: 5711 RVA: 0x00010C17 File Offset: 0x0000EE17
		// (set) Token: 0x06001650 RID: 5712 RVA: 0x00010C1F File Offset: 0x0000EE1F
		public Transform characterTransform { get; private set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001651 RID: 5713 RVA: 0x00010C28 File Offset: 0x0000EE28
		// (set) Token: 0x06001652 RID: 5714 RVA: 0x00010C30 File Offset: 0x0000EE30
		public InputBankTest characterInputBank { get; private set; }

		// Token: 0x06001653 RID: 5715 RVA: 0x00076644 File Offset: 0x00074844
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

		// Token: 0x06001654 RID: 5716 RVA: 0x000766B0 File Offset: 0x000748B0
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

		// Token: 0x06001655 RID: 5717 RVA: 0x00010C39 File Offset: 0x0000EE39
		private void Awake()
		{
			this.stateMachine = base.GetComponent<EntityStateMachine>();
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x00010C47 File Offset: 0x0000EE47
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.NetworkcharacterBodyObject = this.characterBodyObject;
			}
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x00010C5C File Offset: 0x0000EE5C
		private void Update()
		{
			this.UpdateCameras(this.characterBodyObject);
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00010C6A File Offset: 0x0000EE6A
		private void FixedUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x00010C6A File Offset: 0x0000EE6A
		private void LateUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00076730 File Offset: 0x00074930
		private void UpdatePassengerPosition()
		{
			if (this.characterTransform && this.characterStateMachine && this.characterStateMachine.state is GenericCharacterPod)
			{
				this.characterTransform.position = this.holdingPosition.position;
			}
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x00076780 File Offset: 0x00074980
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

		// Token: 0x0600165C RID: 5724 RVA: 0x00010C72 File Offset: 0x0000EE72
		Interactability IInteractable.GetInteractability(Interactor interactor)
		{
			if (!(interactor.gameObject == this.characterBodyObject) || !(this.stateMachine.state is Landed))
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x00010C9C File Offset: 0x0000EE9C
		void IInteractable.OnInteractionBegin(Interactor interactor)
		{
			this.stateMachine.SetNextState(new PreRelease());
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x00010CAE File Offset: 0x0000EEAE
		string IInteractable.GetContextString(Interactor activator)
		{
			return Language.GetString("SURVIVOR_POD_HATCH_OPEN_CONTEXT");
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x00010CBA File Offset: 0x0000EEBA
		private void OnEnable()
		{
			InstanceTracker.Add<SurvivorPodController>(this);
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00010CC2 File Offset: 0x0000EEC2
		private void OnDisable()
		{
			InstanceTracker.Remove<SurvivorPodController>(this);
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldShowOnScanner()
		{
			return false;
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001665 RID: 5733 RVA: 0x00076830 File Offset: 0x00074A30
		// (set) Token: 0x06001666 RID: 5734 RVA: 0x00076844 File Offset: 0x00074A44
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

		// Token: 0x06001667 RID: 5735 RVA: 0x00076894 File Offset: 0x00074A94
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

		// Token: 0x06001668 RID: 5736 RVA: 0x00076900 File Offset: 0x00074B00
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

		// Token: 0x06001669 RID: 5737 RVA: 0x00010CCA File Offset: 0x0000EECA
		public override void PreStartClient()
		{
			if (!this.___characterBodyObjectNetId.IsEmpty())
			{
				this.NetworkcharacterBodyObject = ClientScene.FindLocalObject(this.___characterBodyObjectNetId);
			}
		}

		// Token: 0x04001991 RID: 6545
		[SyncVar(hook = "OnSyncBodyObject")]
		[NonSerialized]
		public GameObject characterBodyObject;

		// Token: 0x04001995 RID: 6549
		private EntityStateMachine stateMachine;

		// Token: 0x04001996 RID: 6550
		[Tooltip("The bone which controls the camera during the entry animation.")]
		public Transform cameraBone;

		// Token: 0x04001997 RID: 6551
		[Tooltip("The transform at which the survivor will be held until they exit the pod.")]
		public Transform holdingPosition;

		// Token: 0x04001998 RID: 6552
		[Tooltip("The transform at which the survivor will be placed upon exiting the pod.")]
		public Transform exitPosition;

		// Token: 0x04001999 RID: 6553
		[Tooltip("The animator for the pod.")]
		public Animator animator;

		// Token: 0x0400199A RID: 6554
		private NetworkInstanceId ___characterBodyObjectNetId;
	}
}
