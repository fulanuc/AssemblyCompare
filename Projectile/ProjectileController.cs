using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053F RID: 1343
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileController : NetworkBehaviour
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001E14 RID: 7700 RVA: 0x00015F64 File Offset: 0x00014164
		// (set) Token: 0x06001E15 RID: 7701 RVA: 0x00015F6C File Offset: 0x0001416C
		public TeamFilter teamFilter { get; private set; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001E16 RID: 7702 RVA: 0x00015F75 File Offset: 0x00014175
		// (set) Token: 0x06001E17 RID: 7703 RVA: 0x00015F7D File Offset: 0x0001417D
		public NetworkConnection clientAuthorityOwner { get; private set; }

		// Token: 0x06001E18 RID: 7704 RVA: 0x000946DC File Offset: 0x000928DC
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.myColliders = base.GetComponents<Collider>();
			for (int i = 0; i < this.myColliders.Length; i++)
			{
				this.myColliders[i].enabled = false;
			}
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x00094730 File Offset: 0x00092930
		private void Start()
		{
			for (int i = 0; i < this.myColliders.Length; i++)
			{
				this.myColliders[i].enabled = true;
			}
			this.IgnoreCollisionsWithOwner();
			if (!this.isPrediction && !NetworkServer.active)
			{
				ProjectileManager.instance.OnClientProjectileReceived(this);
			}
			if (this.ghostPrefab && (this.isPrediction || !this.allowPrediction || !base.hasAuthority))
			{
				this.ghost = UnityEngine.Object.Instantiate<GameObject>(this.ghostPrefab, base.transform.position, base.transform.rotation).GetComponent<ProjectileGhostController>();
				if (this.isPrediction)
				{
					this.ghost.predictionTransform = base.transform;
				}
				else
				{
					this.ghost.authorityTransform = base.transform;
				}
			}
			if (this.isPrediction)
			{
				if (this.predictionEffect)
				{
					this.predictionEffect.SetActive(true);
				}
			}
			else if (this.authorityEffect)
			{
				this.authorityEffect.SetActive(true);
			}
			this.clientAuthorityOwner = base.GetComponent<NetworkIdentity>().clientAuthorityOwner;
		}

		// Token: 0x06001E1A RID: 7706 RVA: 0x00015F86 File Offset: 0x00014186
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				ProjectileManager.instance.OnServerProjectileDestroyed(this);
			}
			if (this.ghost && this.isPrediction)
			{
				UnityEngine.Object.Destroy(this.ghost.gameObject);
			}
		}

		// Token: 0x06001E1B RID: 7707 RVA: 0x00015FBF File Offset: 0x000141BF
		private void OnEnable()
		{
			this.IgnoreCollisionsWithOwner();
		}

		// Token: 0x06001E1C RID: 7708 RVA: 0x00094848 File Offset: 0x00092A48
		private void IgnoreCollisionsWithOwner()
		{
			if (this.owner)
			{
				ModelLocator component = this.owner.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
						if (component2)
						{
							HurtBox[] hurtBoxes = component2.hurtBoxes;
							for (int i = 0; i < hurtBoxes.Length; i++)
							{
								foreach (Collider collider in hurtBoxes[i].GetComponents<Collider>())
								{
									for (int k = 0; k < this.myColliders.Length; k++)
									{
										Collider collider2 = this.myColliders[k];
										Physics.IgnoreCollision(collider, collider2);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001E1D RID: 7709 RVA: 0x00015FC7 File Offset: 0x000141C7
		private static Vector3 EstimateContactPoint(ContactPoint[] contacts, Collider collider)
		{
			if (contacts.Length == 0)
			{
				return collider.transform.position;
			}
			return contacts[0].point;
		}

		// Token: 0x06001E1E RID: 7710 RVA: 0x00015FE5 File Offset: 0x000141E5
		private static Vector3 EstimateContactNormal(ContactPoint[] contacts)
		{
			if (contacts.Length == 0)
			{
				return Vector3.zero;
			}
			return contacts[0].normal;
		}

		// Token: 0x06001E1F RID: 7711 RVA: 0x00094904 File Offset: 0x00092B04
		public void OnCollisionEnter(Collision collision)
		{
			if (NetworkServer.active || this.isPrediction)
			{
				ContactPoint[] contacts = collision.contacts;
				ProjectileImpactInfo impactInfo = new ProjectileImpactInfo
				{
					collider = collision.collider,
					estimatedPointOfImpact = ProjectileController.EstimateContactPoint(contacts, collision.collider),
					estimatedImpactNormal = ProjectileController.EstimateContactNormal(contacts)
				};
				IProjectileImpactBehavior[] components = base.GetComponents<IProjectileImpactBehavior>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnProjectileImpact(impactInfo);
				}
			}
		}

		// Token: 0x06001E21 RID: 7713 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06001E22 RID: 7714 RVA: 0x00094984 File Offset: 0x00092B84
		// (set) Token: 0x06001E23 RID: 7715 RVA: 0x00016017 File Offset: 0x00014217
		public ushort NetworkpredictionId
		{
			get
			{
				return this.predictionId;
			}
			set
			{
				base.SetSyncVar<ushort>(value, ref this.predictionId, 1u);
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001E24 RID: 7716 RVA: 0x00094998 File Offset: 0x00092B98
		// (set) Token: 0x06001E25 RID: 7717 RVA: 0x0001602B File Offset: 0x0001422B
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 2u, ref this.___ownerNetId);
			}
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x000949AC File Offset: 0x00092BAC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
				writer.Write(this.owner);
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
				writer.WritePackedUInt32((uint)this.predictionId);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E27 RID: 7719 RVA: 0x00094A58 File Offset: 0x00092C58
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x06001E28 RID: 7720 RVA: 0x00016045 File Offset: 0x00014245
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x0400206A RID: 8298
		[Tooltip("The prefab to instantiate as the visual representation of this projectile. The prefab must have a ProjectileGhostController attached.")]
		public GameObject ghostPrefab;

		// Token: 0x0400206B RID: 8299
		private Rigidbody rigidbody;

		// Token: 0x0400206D RID: 8301
		[HideInInspector]
		public ProjectileGhostController ghost;

		// Token: 0x0400206E RID: 8302
		[HideInInspector]
		public bool isPrediction;

		// Token: 0x0400206F RID: 8303
		public bool allowPrediction = true;

		// Token: 0x04002070 RID: 8304
		[SyncVar]
		[NonSerialized]
		public ushort predictionId;

		// Token: 0x04002071 RID: 8305
		[HideInInspector]
		[SyncVar]
		public GameObject owner;

		// Token: 0x04002072 RID: 8306
		[HideInInspector]
		public ProcChainMask procChainMask;

		// Token: 0x04002074 RID: 8308
		public float procCoefficient = 1f;

		// Token: 0x04002075 RID: 8309
		public GameObject authorityEffect;

		// Token: 0x04002076 RID: 8310
		public GameObject predictionEffect;

		// Token: 0x04002077 RID: 8311
		private Collider[] myColliders;

		// Token: 0x04002078 RID: 8312
		private NetworkInstanceId ___ownerNetId;
	}
}
