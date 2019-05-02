using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000567 RID: 1383
	public class ProjectileNetworkTransform : NetworkBehaviour
	{
		// Token: 0x06001EFD RID: 7933 RVA: 0x00016AB0 File Offset: 0x00014CB0
		public void SetValuesFromTransform()
		{
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001EFE RID: 7934 RVA: 0x00016AD4 File Offset: 0x00014CD4
		private bool isPrediction
		{
			get
			{
				return this.projectileController && this.projectileController.isPrediction;
			}
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x00097578 File Offset: 0x00095778
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.transform = base.transform;
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x000975CC File Offset: 0x000957CC
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

		// Token: 0x06001F01 RID: 7937 RVA: 0x00016AF0 File Offset: 0x00014CF0
		private void OnSyncPosition(Vector3 newPosition)
		{
			this.interpolatedPosition.PushValue(newPosition);
			this.NetworkserverPosition = newPosition;
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x00016B05 File Offset: 0x00014D05
		private void OnSyncRotation(Quaternion newRotation)
		{
			this.interpolatedRotation.PushValue(newRotation);
			this.NetworkserverRotation = newRotation;
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x00016B1A File Offset: 0x00014D1A
		public override float GetNetworkSendInterval()
		{
			return this.positionTransmitInterval;
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x00097664 File Offset: 0x00095864
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

		// Token: 0x06001F05 RID: 7941 RVA: 0x00016B22 File Offset: 0x00014D22
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

		// Token: 0x06001F07 RID: 7943 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001F08 RID: 7944 RVA: 0x000976F8 File Offset: 0x000958F8
		// (set) Token: 0x06001F09 RID: 7945 RVA: 0x00016B80 File Offset: 0x00014D80
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

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06001F0A RID: 7946 RVA: 0x0009770C File Offset: 0x0009590C
		// (set) Token: 0x06001F0B RID: 7947 RVA: 0x00016BBF File Offset: 0x00014DBF
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

		// Token: 0x06001F0C RID: 7948 RVA: 0x00097720 File Offset: 0x00095920
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

		// Token: 0x06001F0D RID: 7949 RVA: 0x000977CC File Offset: 0x000959CC
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

		// Token: 0x0400215C RID: 8540
		private ProjectileController projectileController;

		// Token: 0x0400215D RID: 8541
		private new Transform transform;

		// Token: 0x0400215E RID: 8542
		private Rigidbody rb;

		// Token: 0x0400215F RID: 8543
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.0333333351f;

		// Token: 0x04002160 RID: 8544
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 1f;

		// Token: 0x04002161 RID: 8545
		[SyncVar(hook = "OnSyncPosition")]
		private Vector3 serverPosition;

		// Token: 0x04002162 RID: 8546
		[SyncVar(hook = "OnSyncRotation")]
		private Quaternion serverRotation;

		// Token: 0x04002163 RID: 8547
		private NetworkLerpedVector3 interpolatedPosition;

		// Token: 0x04002164 RID: 8548
		private NetworkLerpedQuaternion interpolatedRotation;
	}
}
