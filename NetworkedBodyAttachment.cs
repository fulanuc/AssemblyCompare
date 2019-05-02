using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036C RID: 876
	public sealed class NetworkedBodyAttachment : NetworkBehaviour
	{
		// Token: 0x0600120F RID: 4623 RVA: 0x0000DBEF File Offset: 0x0000BDEF
		private void OnSyncAttachedBodyObject(GameObject value)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.Network_attachedBodyObject = value;
			this.OnAttachedBodyObjectAssigned();
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0000DC06 File Offset: 0x0000BE06
		public GameObject attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001211 RID: 4625 RVA: 0x0000DC0E File Offset: 0x0000BE0E
		// (set) Token: 0x06001212 RID: 4626 RVA: 0x0000DC16 File Offset: 0x0000BE16
		public CharacterBody attachedBody { get; private set; }

		// Token: 0x06001213 RID: 4627 RVA: 0x00068324 File Offset: 0x00066524
		[Server]
		public void AttachToGameObjectAndSpawn([NotNull] GameObject newAttachedBodyObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkedBodyAttachment::AttachToGameObjectAndSpawn(UnityEngine.GameObject)' called on client");
				return;
			}
			if (this.attached)
			{
				Debug.LogErrorFormat("Can't attach object '{0}' to object '{1}', it's already been assigned to object '{2}'.", new object[]
				{
					base.gameObject,
					newAttachedBodyObject,
					this.attachedBodyObject
				});
				return;
			}
			this.Network_attachedBodyObject = newAttachedBodyObject;
			this.OnAttachedBodyObjectAssigned();
			NetworkConnection clientAuthorityOwner = newAttachedBodyObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
			if (clientAuthorityOwner == null)
			{
				NetworkServer.Spawn(base.gameObject);
				return;
			}
			NetworkServer.SpawnWithClientAuthority(base.gameObject, clientAuthorityOwner);
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x000683AC File Offset: 0x000665AC
		private void OnAttachedBodyObjectAssigned()
		{
			if (this.attached)
			{
				return;
			}
			this.attached = true;
			if (this._attachedBodyObject)
			{
				this.attachedBody = this._attachedBodyObject.GetComponent<CharacterBody>();
				base.transform.SetParent(this._attachedBodyObject.transform);
				base.transform.localPosition = Vector3.zero;
			}
			INetworkedBodyAttachmentListener[] components = base.GetComponents<INetworkedBodyAttachmentListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnAttachedBodyAssigned(this);
			}
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0000DC1F File Offset: 0x0000BE1F
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.OnSyncAttachedBodyObject(this.attachedBodyObject);
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0000DC33 File Offset: 0x0000BE33
		private void FixedUpdate()
		{
			if (!this.attachedBodyObject)
			{
				if (NetworkServer.active)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x0000DC55 File Offset: 0x0000BE55
		private void OnValidate()
		{
			if (!base.GetComponent<NetworkIdentity>().localPlayerAuthority)
			{
				Debug.LogWarningFormat("NetworkedBodyAttachment: Object {0} NetworkIdentity needs localPlayerAuthority=true", new object[]
				{
					base.gameObject.name
				});
			}
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x0006842C File Offset: 0x0006662C
		// (set) Token: 0x0600121B RID: 4635 RVA: 0x00068440 File Offset: 0x00066640
		public GameObject Network_attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncAttachedBodyObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this._attachedBodyObject, dirtyBit, ref this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00068490 File Offset: 0x00066690
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this._attachedBodyObject);
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
				writer.Write(this._attachedBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x000684FC File Offset: 0x000666FC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.____attachedBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncAttachedBodyObject(reader.ReadGameObject());
			}
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0000DC82 File Offset: 0x0000BE82
		public override void PreStartClient()
		{
			if (!this.____attachedBodyObjectNetId.IsEmpty())
			{
				this.Network_attachedBodyObject = ClientScene.FindLocalObject(this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x0400161E RID: 5662
		[SyncVar(hook = "OnSyncAttachedBodyObject")]
		private GameObject _attachedBodyObject;

		// Token: 0x04001620 RID: 5664
		private bool attached;

		// Token: 0x04001621 RID: 5665
		private NetworkInstanceId ____attachedBodyObjectNetId;
	}
}
