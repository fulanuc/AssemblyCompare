using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200056D RID: 1389
	[RequireComponent(typeof(ProjectileNetworkTransform))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileStickOnImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001F2A RID: 7978 RVA: 0x00016D53 File Offset: 0x00014F53
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x00016D6D File Offset: 0x00014F6D
		// (set) Token: 0x06001F2C RID: 7980 RVA: 0x00016D75 File Offset: 0x00014F75
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

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06001F2D RID: 7981 RVA: 0x00016D85 File Offset: 0x00014F85
		public bool stuck
		{
			get
			{
				return this.hitHurtboxIndex != -1;
			}
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x0009831C File Offset: 0x0009651C
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

		// Token: 0x06001F2F RID: 7983 RVA: 0x000984B0 File Offset: 0x000966B0
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

		// Token: 0x06001F31 RID: 7985 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001F32 RID: 7986 RVA: 0x000985D0 File Offset: 0x000967D0
		// (set) Token: 0x06001F33 RID: 7987 RVA: 0x00016DA2 File Offset: 0x00014FA2
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

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001F34 RID: 7988 RVA: 0x000985E4 File Offset: 0x000967E4
		// (set) Token: 0x06001F35 RID: 7989 RVA: 0x00016DBC File Offset: 0x00014FBC
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

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001F36 RID: 7990 RVA: 0x000985F8 File Offset: 0x000967F8
		// (set) Token: 0x06001F37 RID: 7991 RVA: 0x00016DD0 File Offset: 0x00014FD0
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

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001F38 RID: 7992 RVA: 0x0009860C File Offset: 0x0009680C
		// (set) Token: 0x06001F39 RID: 7993 RVA: 0x00016DE4 File Offset: 0x00014FE4
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

		// Token: 0x06001F3A RID: 7994 RVA: 0x00098620 File Offset: 0x00096820
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

		// Token: 0x06001F3B RID: 7995 RVA: 0x00098748 File Offset: 0x00096948
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

		// Token: 0x06001F3C RID: 7996 RVA: 0x00016DF8 File Offset: 0x00014FF8
		public override void PreStartClient()
		{
			if (!this.___syncVictimNetId.IsEmpty())
			{
				this.NetworksyncVictim = ClientScene.FindLocalObject(this.___syncVictimNetId);
			}
		}

		// Token: 0x04002199 RID: 8601
		public string stickSoundString;

		// Token: 0x0400219A RID: 8602
		public ParticleSystem[] stickParticleSystem;

		// Token: 0x0400219B RID: 8603
		private ProjectileController projectileController;

		// Token: 0x0400219C RID: 8604
		private Rigidbody rigidbody;

		// Token: 0x0400219D RID: 8605
		private Transform stuckTransform;

		// Token: 0x0400219E RID: 8606
		public bool ignoreCharacters;

		// Token: 0x0400219F RID: 8607
		public bool ignoreWorld;

		// Token: 0x040021A0 RID: 8608
		public UnityEvent stickEvent;

		// Token: 0x040021A1 RID: 8609
		private GameObject _victim;

		// Token: 0x040021A2 RID: 8610
		[SyncVar]
		private GameObject syncVictim;

		// Token: 0x040021A3 RID: 8611
		[SyncVar]
		private sbyte hitHurtboxIndex = -1;

		// Token: 0x040021A4 RID: 8612
		[SyncVar]
		private Vector3 localPosition;

		// Token: 0x040021A5 RID: 8613
		[SyncVar]
		private Quaternion localRotation;

		// Token: 0x040021A6 RID: 8614
		private NetworkInstanceId ___syncVictimNetId;
	}
}
