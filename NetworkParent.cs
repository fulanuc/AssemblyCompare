using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036B RID: 875
	public class NetworkParent : NetworkBehaviour
	{
		// Token: 0x06001209 RID: 4617 RVA: 0x0000DBBD File Offset: 0x0000BDBD
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0000DBCB File Offset: 0x0000BDCB
		public override void OnStartServer()
		{
			this.ServerUpdateParent();
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x0000DBD3 File Offset: 0x0000BDD3
		private void OnTransformParentChanged()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdateParent();
			}
			this.transform.localPosition = Vector3.zero;
			this.transform.localRotation = Quaternion.identity;
			this.transform.localScale = Vector3.one;
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00068208 File Offset: 0x00066408
		[Server]
		private void ServerUpdateParent()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkParent::ServerUpdateParent()' called on client");
				return;
			}
			Transform transform = this.transform.parent;
			if (transform == this.cachedServerParentTransform)
			{
				return;
			}
			if (!transform)
			{
				transform = null;
			}
			this.cachedServerParentTransform = transform;
			this.SetParentIdentifier(new NetworkParent.ParentIdentifier(transform));
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0000DC12 File Offset: 0x0000BE12
		private void SetParentIdentifier(NetworkParent.ParentIdentifier newParentIdentifier)
		{
			this.NetworkparentIdentifier = newParentIdentifier;
			if (!NetworkServer.active)
			{
				this.transform.parent = this.parentIdentifier.Resolve();
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x00068260 File Offset: 0x00066460
		// (set) Token: 0x06001211 RID: 4625 RVA: 0x0000DC38 File Offset: 0x0000BE38
		public NetworkParent.ParentIdentifier NetworkparentIdentifier
		{
			get
			{
				return this.parentIdentifier;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetParentIdentifier(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkParent.ParentIdentifier>(value, ref this.parentIdentifier, dirtyBit);
			}
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x00068274 File Offset: 0x00066474
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
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
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x000682E0 File Offset: 0x000664E0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.parentIdentifier = GeneratedNetworkCode._ReadParentIdentifier_NetworkParent(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetParentIdentifier(GeneratedNetworkCode._ReadParentIdentifier_NetworkParent(reader));
			}
		}

		// Token: 0x04001609 RID: 5641
		private Transform cachedServerParentTransform;

		// Token: 0x0400160A RID: 5642
		private new Transform transform;

		// Token: 0x0400160B RID: 5643
		[SyncVar(hook = "SetParentIdentifier")]
		private NetworkParent.ParentIdentifier parentIdentifier;

		// Token: 0x0200036C RID: 876
		[Serializable]
		private struct ParentIdentifier : IEquatable<NetworkParent.ParentIdentifier>
		{
			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06001214 RID: 4628 RVA: 0x0000DC77 File Offset: 0x0000BE77
			// (set) Token: 0x06001215 RID: 4629 RVA: 0x0000DC81 File Offset: 0x0000BE81
			public int indexInParentChildLocator
			{
				get
				{
					return (int)(this.indexInParentChildLocatorPlusOne - 1);
				}
				set
				{
					this.indexInParentChildLocatorPlusOne = (byte)(value + 1);
				}
			}

			// Token: 0x06001216 RID: 4630 RVA: 0x00068324 File Offset: 0x00066524
			private static ChildLocator LookUpChildLocator(Transform rootObject)
			{
				ModelLocator component = rootObject.GetComponent<ModelLocator>();
				if (!component)
				{
					return null;
				}
				Transform modelTransform = component.modelTransform;
				if (!modelTransform)
				{
					return null;
				}
				return modelTransform.GetComponent<ChildLocator>();
			}

			// Token: 0x06001217 RID: 4631 RVA: 0x0006835C File Offset: 0x0006655C
			public ParentIdentifier(Transform parent)
			{
				this.parentNetworkIdentity = null;
				this.indexInParentChildLocatorPlusOne = 0;
				if (!parent)
				{
					return;
				}
				this.parentNetworkIdentity = parent.GetComponentInParent<NetworkIdentity>();
				if (!this.parentNetworkIdentity)
				{
					Debug.LogWarningFormat("NetworkParent cannot accept a non-null parent without a NetworkIdentity! parent={0}", new object[]
					{
						parent
					});
					return;
				}
				if (this.parentNetworkIdentity.gameObject == parent.gameObject)
				{
					return;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(this.parentNetworkIdentity.transform);
				if (!childLocator)
				{
					Debug.LogWarningFormat("NetworkParent can only be parented directly to another object with a NetworkIdentity or an object registered in the ChildLocator of a a model of an object with a NetworkIdentity. parent={0}", new object[]
					{
						parent
					});
					return;
				}
				this.indexInParentChildLocator = childLocator.FindChildIndex(parent);
				if (this.indexInParentChildLocatorPlusOne == 0)
				{
					Debug.LogWarningFormat("NetowrkParent parent={0} is not registered in a ChildLocator.", new object[]
					{
						parent
					});
					return;
				}
			}

			// Token: 0x06001218 RID: 4632 RVA: 0x0000DC8D File Offset: 0x0000BE8D
			public bool Equals(NetworkParent.ParentIdentifier other)
			{
				return object.Equals(this.parentNetworkIdentity, other.parentNetworkIdentity) && this.indexInParentChildLocatorPlusOne == other.indexInParentChildLocatorPlusOne;
			}

			// Token: 0x06001219 RID: 4633 RVA: 0x0000DCB2 File Offset: 0x0000BEB2
			public override bool Equals(object obj)
			{
				return obj != null && obj is NetworkParent.ParentIdentifier && this.Equals((NetworkParent.ParentIdentifier)obj);
			}

			// Token: 0x0600121A RID: 4634 RVA: 0x0000DCCF File Offset: 0x0000BECF
			public override int GetHashCode()
			{
				return ((this.parentNetworkIdentity != null) ? this.parentNetworkIdentity.GetHashCode() : 0) * 397 ^ this.indexInParentChildLocatorPlusOne.GetHashCode();
			}

			// Token: 0x0600121B RID: 4635 RVA: 0x0006841C File Offset: 0x0006661C
			public Transform Resolve()
			{
				if (!this.parentNetworkIdentity)
				{
					return null;
				}
				if (this.indexInParentChildLocatorPlusOne == 0)
				{
					return this.parentNetworkIdentity.transform;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(this.parentNetworkIdentity.transform);
				if (childLocator)
				{
					return childLocator.FindChild(this.indexInParentChildLocator);
				}
				return null;
			}

			// Token: 0x0400160C RID: 5644
			public NetworkIdentity parentNetworkIdentity;

			// Token: 0x0400160D RID: 5645
			public byte indexInParentChildLocatorPlusOne;
		}
	}
}
