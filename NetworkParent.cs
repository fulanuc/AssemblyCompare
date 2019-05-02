using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036E RID: 878
	public class NetworkParent : NetworkBehaviour
	{
		// Token: 0x06001220 RID: 4640 RVA: 0x0000DCA6 File Offset: 0x0000BEA6
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0000DCB4 File Offset: 0x0000BEB4
		public override void OnStartServer()
		{
			this.ServerUpdateParent();
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0000DCBC File Offset: 0x0000BEBC
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

		// Token: 0x06001223 RID: 4643 RVA: 0x00068540 File Offset: 0x00066740
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

		// Token: 0x06001224 RID: 4644 RVA: 0x0000DCFB File Offset: 0x0000BEFB
		private void SetParentIdentifier(NetworkParent.ParentIdentifier newParentIdentifier)
		{
			this.NetworkparentIdentifier = newParentIdentifier;
			if (!NetworkServer.active)
			{
				this.transform.parent = this.parentIdentifier.Resolve();
			}
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x00068598 File Offset: 0x00066798
		// (set) Token: 0x06001228 RID: 4648 RVA: 0x0000DD21 File Offset: 0x0000BF21
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

		// Token: 0x06001229 RID: 4649 RVA: 0x000685AC File Offset: 0x000667AC
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

		// Token: 0x0600122A RID: 4650 RVA: 0x00068618 File Offset: 0x00066818
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

		// Token: 0x04001622 RID: 5666
		private Transform cachedServerParentTransform;

		// Token: 0x04001623 RID: 5667
		private new Transform transform;

		// Token: 0x04001624 RID: 5668
		[SyncVar(hook = "SetParentIdentifier")]
		private NetworkParent.ParentIdentifier parentIdentifier;

		// Token: 0x0200036F RID: 879
		[Serializable]
		private struct ParentIdentifier : IEquatable<NetworkParent.ParentIdentifier>
		{
			// Token: 0x17000193 RID: 403
			// (get) Token: 0x0600122B RID: 4651 RVA: 0x0000DD60 File Offset: 0x0000BF60
			// (set) Token: 0x0600122C RID: 4652 RVA: 0x0000DD6A File Offset: 0x0000BF6A
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

			// Token: 0x0600122D RID: 4653 RVA: 0x0006865C File Offset: 0x0006685C
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

			// Token: 0x0600122E RID: 4654 RVA: 0x00068694 File Offset: 0x00066894
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

			// Token: 0x0600122F RID: 4655 RVA: 0x0000DD76 File Offset: 0x0000BF76
			public bool Equals(NetworkParent.ParentIdentifier other)
			{
				return object.Equals(this.parentNetworkIdentity, other.parentNetworkIdentity) && this.indexInParentChildLocatorPlusOne == other.indexInParentChildLocatorPlusOne;
			}

			// Token: 0x06001230 RID: 4656 RVA: 0x0000DD9B File Offset: 0x0000BF9B
			public override bool Equals(object obj)
			{
				return obj != null && obj is NetworkParent.ParentIdentifier && this.Equals((NetworkParent.ParentIdentifier)obj);
			}

			// Token: 0x06001231 RID: 4657 RVA: 0x0000DDB8 File Offset: 0x0000BFB8
			public override int GetHashCode()
			{
				return ((this.parentNetworkIdentity != null) ? this.parentNetworkIdentity.GetHashCode() : 0) * 397 ^ this.indexInParentChildLocatorPlusOne.GetHashCode();
			}

			// Token: 0x06001232 RID: 4658 RVA: 0x00068754 File Offset: 0x00066954
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

			// Token: 0x04001625 RID: 5669
			public NetworkIdentity parentNetworkIdentity;

			// Token: 0x04001626 RID: 5670
			public byte indexInParentChildLocatorPlusOne;
		}
	}
}
