using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054E RID: 1358
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileController : NetworkBehaviour
	{
		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001E7E RID: 7806 RVA: 0x00016443 File Offset: 0x00014643
		// (set) Token: 0x06001E7F RID: 7807 RVA: 0x0001644B File Offset: 0x0001464B
		public TeamFilter teamFilter { get; private set; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06001E80 RID: 7808 RVA: 0x00016454 File Offset: 0x00014654
		// (set) Token: 0x06001E81 RID: 7809 RVA: 0x0001645C File Offset: 0x0001465C
		public NetworkConnection clientAuthorityOwner { get; private set; }

		// Token: 0x06001E82 RID: 7810 RVA: 0x000953F8 File Offset: 0x000935F8
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

		// Token: 0x06001E83 RID: 7811 RVA: 0x0009544C File Offset: 0x0009364C
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

		// Token: 0x06001E84 RID: 7812 RVA: 0x00016465 File Offset: 0x00014665
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

		// Token: 0x06001E85 RID: 7813 RVA: 0x0001649E File Offset: 0x0001469E
		private void OnEnable()
		{
			this.IgnoreCollisionsWithOwner();
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x00095564 File Offset: 0x00093764
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

		// Token: 0x06001E87 RID: 7815 RVA: 0x000164A6 File Offset: 0x000146A6
		private static Vector3 EstimateContactPoint(ContactPoint[] contacts, Collider collider)
		{
			if (contacts.Length == 0)
			{
				return collider.transform.position;
			}
			return contacts[0].point;
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x000164C4 File Offset: 0x000146C4
		private static Vector3 EstimateContactNormal(ContactPoint[] contacts)
		{
			if (contacts.Length == 0)
			{
				return Vector3.zero;
			}
			return contacts[0].normal;
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x00095620 File Offset: 0x00093820
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

		// Token: 0x06001E8B RID: 7819 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001E8C RID: 7820 RVA: 0x000956A0 File Offset: 0x000938A0
		// (set) Token: 0x06001E8D RID: 7821 RVA: 0x000164F6 File Offset: 0x000146F6
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

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001E8E RID: 7822 RVA: 0x000956B4 File Offset: 0x000938B4
		// (set) Token: 0x06001E8F RID: 7823 RVA: 0x0001650A File Offset: 0x0001470A
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

		// Token: 0x06001E90 RID: 7824 RVA: 0x000956C8 File Offset: 0x000938C8
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

		// Token: 0x06001E91 RID: 7825 RVA: 0x00095774 File Offset: 0x00093974
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

		// Token: 0x06001E92 RID: 7826 RVA: 0x00016524 File Offset: 0x00014724
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x040020A8 RID: 8360
		[Tooltip("The prefab to instantiate as the visual representation of this projectile. The prefab must have a ProjectileGhostController attached.")]
		public GameObject ghostPrefab;

		// Token: 0x040020A9 RID: 8361
		private Rigidbody rigidbody;

		// Token: 0x040020AB RID: 8363
		[HideInInspector]
		public ProjectileGhostController ghost;

		// Token: 0x040020AC RID: 8364
		[HideInInspector]
		public bool isPrediction;

		// Token: 0x040020AD RID: 8365
		public bool allowPrediction = true;

		// Token: 0x040020AE RID: 8366
		[SyncVar]
		[NonSerialized]
		public ushort predictionId;

		// Token: 0x040020AF RID: 8367
		[SyncVar]
		[HideInInspector]
		public GameObject owner;

		// Token: 0x040020B0 RID: 8368
		[HideInInspector]
		public ProcChainMask procChainMask;

		// Token: 0x040020B2 RID: 8370
		public float procCoefficient = 1f;

		// Token: 0x040020B3 RID: 8371
		public GameObject authorityEffect;

		// Token: 0x040020B4 RID: 8372
		public GameObject predictionEffect;

		// Token: 0x040020B5 RID: 8373
		private Collider[] myColliders;

		// Token: 0x040020B6 RID: 8374
		private NetworkInstanceId ___ownerNetId;
	}
}
