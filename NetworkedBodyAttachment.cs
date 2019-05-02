using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000369 RID: 873
	public sealed class NetworkedBodyAttachment : NetworkBehaviour
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x0000DB06 File Offset: 0x0000BD06
		private void OnSyncAttachedBodyObject(GameObject value)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.Network_attachedBodyObject = value;
			this.OnAttachedBodyObjectAssigned();
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x0000DB1D File Offset: 0x0000BD1D
		public GameObject attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060011FA RID: 4602 RVA: 0x0000DB25 File Offset: 0x0000BD25
		// (set) Token: 0x060011FB RID: 4603 RVA: 0x0000DB2D File Offset: 0x0000BD2D
		public CharacterBody attachedBody { get; private set; }

		// Token: 0x060011FC RID: 4604 RVA: 0x00067FEC File Offset: 0x000661EC
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

		// Token: 0x060011FD RID: 4605 RVA: 0x00068074 File Offset: 0x00066274
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

		// Token: 0x060011FE RID: 4606 RVA: 0x0000DB36 File Offset: 0x0000BD36
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.OnSyncAttachedBodyObject(this.attachedBodyObject);
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x0000DB4A File Offset: 0x0000BD4A
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

		// Token: 0x06001200 RID: 4608 RVA: 0x0000DB6C File Offset: 0x0000BD6C
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

		// Token: 0x06001202 RID: 4610 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x000680F4 File Offset: 0x000662F4
		// (set) Token: 0x06001204 RID: 4612 RVA: 0x00068108 File Offset: 0x00066308
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

		// Token: 0x06001205 RID: 4613 RVA: 0x00068158 File Offset: 0x00066358
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

		// Token: 0x06001206 RID: 4614 RVA: 0x000681C4 File Offset: 0x000663C4
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

		// Token: 0x06001207 RID: 4615 RVA: 0x0000DB99 File Offset: 0x0000BD99
		public override void PreStartClient()
		{
			if (!this.____attachedBodyObjectNetId.IsEmpty())
			{
				this.Network_attachedBodyObject = ClientScene.FindLocalObject(this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x04001605 RID: 5637
		[SyncVar(hook = "OnSyncAttachedBodyObject")]
		private GameObject _attachedBodyObject;

		// Token: 0x04001607 RID: 5639
		private bool attached;

		// Token: 0x04001608 RID: 5640
		private NetworkInstanceId ____attachedBodyObjectNetId;
	}
}
