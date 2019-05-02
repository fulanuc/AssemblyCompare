using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055E RID: 1374
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileNetworkTransform))]
	public class ProjectileStickOnImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EC0 RID: 7872 RVA: 0x00016874 File Offset: 0x00014A74
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001EC1 RID: 7873 RVA: 0x0001688E File Offset: 0x00014A8E
		// (set) Token: 0x06001EC2 RID: 7874 RVA: 0x00016896 File Offset: 0x00014A96
		public GameObject victim
		{
			get
			{
				return this._victim;
			}
			private set
			{
				this._victim = value;
				this.NetworksyncVictim = value;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001EC3 RID: 7875 RVA: 0x000168A6 File Offset: 0x00014AA6
		public bool stuck
		{
			get
			{
				return this.hitHurtboxIndex != -1;
			}
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x00097600 File Offset: 0x00095800
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.victim)
			{
				bool flag = false;
				this.NetworkhitHurtboxIndex = -1;
				HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
				GameObject gameObject = null;
				if (component)
				{
					flag = true;
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						gameObject = healthComponent.gameObject;
					}
					this.NetworkhitHurtboxIndex = (sbyte)component.indexInGroup;
				}
				if (!gameObject && !this.ignoreWorld)
				{
					gameObject = impactInfo.collider.gameObject;
					this.NetworkhitHurtboxIndex = -2;
				}
				if (gameObject == this.projectileController.owner)
				{
					this.victim = null;
					this.NetworkhitHurtboxIndex = -1;
					return;
				}
				if (this.ignoreCharacters && flag)
				{
					gameObject = null;
					this.NetworkhitHurtboxIndex = -1;
				}
				if (gameObject)
				{
					this.stickEvent.Invoke();
					ParticleSystem[] array = this.stickParticleSystem;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
					if (this.stickSoundString.Length > 0)
					{
						Util.PlaySound(this.stickSoundString, base.gameObject);
					}
				}
				Vector3 estimatedImpactNormal = impactInfo.estimatedImpactNormal;
				if (estimatedImpactNormal != Vector3.zero)
				{
					base.transform.rotation = Util.QuaternionSafeLookRotation(estimatedImpactNormal, base.transform.up);
				}
				Transform transform = impactInfo.collider.transform;
				this.NetworklocalPosition = transform.InverseTransformPoint(base.transform.position);
				this.NetworklocalRotation = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
				this.victim = gameObject;
			}
		}

		// Token: 0x06001EC5 RID: 7877 RVA: 0x00097794 File Offset: 0x00095994
		public void FixedUpdate()
		{
			bool flag = this.stuckTransform;
			if (flag)
			{
				base.transform.SetPositionAndRotation(this.stuckTransform.TransformPoint(this.localPosition), this.stuckTransform.rotation * this.localRotation);
			}
			else
			{
				GameObject gameObject = NetworkServer.active ? this.victim : this.syncVictim;
				if (gameObject)
				{
					this.stuckTransform = gameObject.transform;
					flag = true;
					if (this.hitHurtboxIndex >= 0)
					{
						CharacterBody component = this.stuckTransform.GetComponent<CharacterBody>();
						if (component)
						{
							this.stuckTransform = component.hurtBoxGroup.hurtBoxes[(int)this.hitHurtboxIndex].transform;
						}
					}
				}
				else if (this.hitHurtboxIndex == -2 && !NetworkServer.active)
				{
					flag = true;
				}
			}
			if (NetworkServer.active)
			{
				if (this.rigidbody.isKinematic != flag)
				{
					if (flag)
					{
						this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
						this.rigidbody.isKinematic = true;
					}
					else
					{
						this.rigidbody.isKinematic = false;
						this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
					}
				}
				if (!flag)
				{
					this.NetworkhitHurtboxIndex = -1;
				}
			}
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x000978B4 File Offset: 0x00095AB4
		// (set) Token: 0x06001EC9 RID: 7881 RVA: 0x000168C3 File Offset: 0x00014AC3
		public GameObject NetworksyncVictim
		{
			get
			{
				return this.syncVictim;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.syncVictim, 1u, ref this.___syncVictimNetId);
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06001ECA RID: 7882 RVA: 0x000978C8 File Offset: 0x00095AC8
		// (set) Token: 0x06001ECB RID: 7883 RVA: 0x000168DD File Offset: 0x00014ADD
		public sbyte NetworkhitHurtboxIndex
		{
			get
			{
				return this.hitHurtboxIndex;
			}
			set
			{
				base.SetSyncVar<sbyte>(value, ref this.hitHurtboxIndex, 2u);
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001ECC RID: 7884 RVA: 0x000978DC File Offset: 0x00095ADC
		// (set) Token: 0x06001ECD RID: 7885 RVA: 0x000168F1 File Offset: 0x00014AF1
		public Vector3 NetworklocalPosition
		{
			get
			{
				return this.localPosition;
			}
			set
			{
				base.SetSyncVar<Vector3>(value, ref this.localPosition, 4u);
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001ECE RID: 7886 RVA: 0x000978F0 File Offset: 0x00095AF0
		// (set) Token: 0x06001ECF RID: 7887 RVA: 0x00016905 File Offset: 0x00014B05
		public Quaternion NetworklocalRotation
		{
			get
			{
				return this.localRotation;
			}
			set
			{
				base.SetSyncVar<Quaternion>(value, ref this.localRotation, 8u);
			}
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x00097904 File Offset: 0x00095B04
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.syncVictim);
				writer.WritePackedUInt32((uint)this.hitHurtboxIndex);
				writer.Write(this.localPosition);
				writer.Write(this.localRotation);
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
				writer.Write(this.syncVictim);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.hitHurtboxIndex);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.localPosition);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.localRotation);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x00097A2C File Offset: 0x00095C2C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___syncVictimNetId = reader.ReadNetworkId();
				this.hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
				this.localPosition = reader.ReadVector3();
				this.localRotation = reader.ReadQuaternion();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.syncVictim = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.localPosition = reader.ReadVector3();
			}
			if ((num & 8) != 0)
			{
				this.localRotation = reader.ReadQuaternion();
			}
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x00016919 File Offset: 0x00014B19
		public override void PreStartClient()
		{
			if (!this.___syncVictimNetId.IsEmpty())
			{
				this.NetworksyncVictim = ClientScene.FindLocalObject(this.___syncVictimNetId);
			}
		}

		// Token: 0x0400215B RID: 8539
		public string stickSoundString;

		// Token: 0x0400215C RID: 8540
		public ParticleSystem[] stickParticleSystem;

		// Token: 0x0400215D RID: 8541
		private ProjectileController projectileController;

		// Token: 0x0400215E RID: 8542
		private Rigidbody rigidbody;

		// Token: 0x0400215F RID: 8543
		private Transform stuckTransform;

		// Token: 0x04002160 RID: 8544
		public bool ignoreCharacters;

		// Token: 0x04002161 RID: 8545
		public bool ignoreWorld;

		// Token: 0x04002162 RID: 8546
		public UnityEvent stickEvent;

		// Token: 0x04002163 RID: 8547
		private GameObject _victim;

		// Token: 0x04002164 RID: 8548
		[SyncVar]
		private GameObject syncVictim;

		// Token: 0x04002165 RID: 8549
		[SyncVar]
		private sbyte hitHurtboxIndex = -1;

		// Token: 0x04002166 RID: 8550
		[SyncVar]
		private Vector3 localPosition;

		// Token: 0x04002167 RID: 8551
		[SyncVar]
		private Quaternion localRotation;

		// Token: 0x04002168 RID: 8552
		private NetworkInstanceId ___syncVictimNetId;
	}
}
