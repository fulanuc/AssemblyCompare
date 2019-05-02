using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000558 RID: 1368
	public class ProjectileNetworkTransform : NetworkBehaviour
	{
		// Token: 0x06001E93 RID: 7827 RVA: 0x000165D1 File Offset: 0x000147D1
		public void SetValuesFromTransform()
		{
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001E94 RID: 7828 RVA: 0x000165F5 File Offset: 0x000147F5
		private bool isPrediction
		{
			get
			{
				return this.projectileController && this.projectileController.isPrediction;
			}
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x0009685C File Offset: 0x00094A5C
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.transform = base.transform;
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x000968B0 File Offset: 0x00094AB0
		private void Start()
		{
			this.interpolatedPosition.interpDelay = this.GetNetworkSendInterval() * this.interpolationFactor;
			this.interpolatedPosition.SetValueImmediate(this.serverPosition);
			this.interpolatedRotation.SetValueImmediate(this.serverRotation);
			if (this.isPrediction)
			{
				base.enabled = false;
			}
			if (this.rb && !this.isPrediction && !NetworkServer.active)
			{
				this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				this.rb.detectCollisions = false;
				this.rb.isKinematic = true;
			}
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x00016611 File Offset: 0x00014811
		private void OnSyncPosition(Vector3 newPosition)
		{
			this.interpolatedPosition.PushValue(newPosition);
			this.NetworkserverPosition = newPosition;
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x00016626 File Offset: 0x00014826
		private void OnSyncRotation(Quaternion newRotation)
		{
			this.interpolatedRotation.PushValue(newRotation);
			this.NetworkserverRotation = newRotation;
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x0001663B File Offset: 0x0001483B
		public override float GetNetworkSendInterval()
		{
			return this.positionTransmitInterval;
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x00096948 File Offset: 0x00094B48
		private void FixedUpdate()
		{
			if (base.isServer)
			{
				this.interpolatedPosition.interpDelay = this.GetNetworkSendInterval() * this.interpolationFactor;
				this.NetworkserverPosition = this.transform.position;
				this.NetworkserverRotation = this.transform.rotation;
				this.interpolatedPosition.SetValueImmediate(this.serverPosition);
				this.interpolatedRotation.SetValueImmediate(this.serverRotation);
				return;
			}
			Vector3 currentValue = this.interpolatedPosition.GetCurrentValue(false);
			Quaternion currentValue2 = this.interpolatedRotation.GetCurrentValue(false);
			this.ApplyPositionAndRotation(currentValue, currentValue2);
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x00016643 File Offset: 0x00014843
		private void ApplyPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			if (this.rb)
			{
				this.rb.MovePosition(position);
				this.rb.MoveRotation(rotation);
				return;
			}
			this.transform.position = position;
			this.transform.rotation = rotation;
		}

		// Token: 0x06001E9D RID: 7837 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06001E9E RID: 7838 RVA: 0x000969DC File Offset: 0x00094BDC
		// (set) Token: 0x06001E9F RID: 7839 RVA: 0x000166A1 File Offset: 0x000148A1
		public Vector3 NetworkserverPosition
		{
			get
			{
				return this.serverPosition;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPosition(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Vector3>(value, ref this.serverPosition, dirtyBit);
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06001EA0 RID: 7840 RVA: 0x000969F0 File Offset: 0x00094BF0
		// (set) Token: 0x06001EA1 RID: 7841 RVA: 0x000166E0 File Offset: 0x000148E0
		public Quaternion NetworkserverRotation
		{
			get
			{
				return this.serverRotation;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncRotation(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Quaternion>(value, ref this.serverRotation, dirtyBit);
			}
		}

		// Token: 0x06001EA2 RID: 7842 RVA: 0x00096A04 File Offset: 0x00094C04
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.serverPosition);
				writer.Write(this.serverRotation);
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
				writer.Write(this.serverPosition);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.serverRotation);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001EA3 RID: 7843 RVA: 0x00096AB0 File Offset: 0x00094CB0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.serverPosition = reader.ReadVector3();
				this.serverRotation = reader.ReadQuaternion();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncPosition(reader.ReadVector3());
			}
			if ((num & 2) != 0)
			{
				this.OnSyncRotation(reader.ReadQuaternion());
			}
		}

		// Token: 0x0400211E RID: 8478
		private ProjectileController projectileController;

		// Token: 0x0400211F RID: 8479
		private new Transform transform;

		// Token: 0x04002120 RID: 8480
		private Rigidbody rb;

		// Token: 0x04002121 RID: 8481
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.0333333351f;

		// Token: 0x04002122 RID: 8482
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 1f;

		// Token: 0x04002123 RID: 8483
		[SyncVar(hook = "OnSyncPosition")]
		private Vector3 serverPosition;

		// Token: 0x04002124 RID: 8484
		[SyncVar(hook = "OnSyncRotation")]
		private Quaternion serverRotation;

		// Token: 0x04002125 RID: 8485
		private NetworkLerpedVector3 interpolatedPosition;

		// Token: 0x04002126 RID: 8486
		private NetworkLerpedQuaternion interpolatedRotation;
	}
}
