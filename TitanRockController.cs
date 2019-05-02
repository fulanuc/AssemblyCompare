using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000409 RID: 1033
	public class TitanRockController : NetworkBehaviour
	{
		// Token: 0x0600172C RID: 5932 RVA: 0x000116DA File Offset: 0x0000F8DA
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.SetOwner(this.owner);
				return;
			}
			this.fireTimer = this.startDelay;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x00078E2C File Offset: 0x0007702C
		public void SetOwner(GameObject newOwner)
		{
			this.ownerInputBank = null;
			this.ownerCharacterBody = null;
			this.isCrit = false;
			this.Networkowner = newOwner;
			if (this.owner)
			{
				this.ownerInputBank = this.owner.GetComponent<InputBankTest>();
				this.ownerCharacterBody = this.owner.GetComponent<CharacterBody>();
				ModelLocator component = this.owner.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
						if (component2)
						{
							this.targetTransform = component2.FindChild("Chest");
							if (this.targetTransform)
							{
								base.transform.rotation = this.targetTransform.rotation;
							}
						}
					}
				}
				base.transform.position = this.owner.transform.position + Vector3.down * 20f;
				if (NetworkServer.active && this.ownerCharacterBody)
				{
					CharacterMaster master = this.ownerCharacterBody.master;
					if (master)
					{
						this.isCrit = Util.CheckRoll(this.ownerCharacterBody.crit, master);
					}
				}
			}
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x00078F5C File Offset: 0x0007715C
		public void FixedUpdate()
		{
			if (this.targetTransform)
			{
				this.foundOwner = true;
				base.transform.position = Vector3.SmoothDamp(base.transform.position, this.targetTransform.TransformPoint(TitanRockController.targetLocalPosition), ref this.velocity, 1f);
				base.transform.rotation = this.targetTransform.rotation;
			}
			else if (this.foundOwner)
			{
				this.foundOwner = false;
				foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
				{
					particleSystem.main.gravityModifier = 1f;
					particleSystem.emission.enabled = false;
					particleSystem.noise.enabled = false;
					particleSystem.limitVelocityOverLifetime.enabled = false;
				}
				base.transform.Find("Debris").GetComponent<ParticleSystem>().collision.enabled = true;
				Light[] componentsInChildren2 = base.GetComponentsInChildren<Light>();
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].enabled = false;
				}
				Util.PlaySound("Stop_titanboss_shift_loop", base.gameObject);
			}
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x000790A4 File Offset: 0x000772A4
		[Server]
		private void FixedUpdateServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TitanRockController::FixedUpdateServer()' called on client");
				return;
			}
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.Fire();
				this.fireTimer += this.fireInterval;
			}
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x00079100 File Offset: 0x00077300
		[Server]
		private void Fire()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TitanRockController::Fire()' called on client");
				return;
			}
			if (this.ownerInputBank)
			{
				Vector3 position = this.fireTransform.position;
				Vector3 forward = this.ownerInputBank.aimDirection;
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(this.owner, new Ray(this.ownerInputBank.aimOrigin, this.ownerInputBank.aimDirection), out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					forward = raycastHit.point - position;
				}
				float num = this.ownerCharacterBody ? this.ownerCharacterBody.damage : 1f;
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, position, Util.QuaternionSafeLookRotation(forward), this.owner, this.damageCoefficient * num, this.damageForce, this.isCrit, DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06001734 RID: 5940 RVA: 0x0007920C File Offset: 0x0007740C
		// (set) Token: 0x06001735 RID: 5941 RVA: 0x00079220 File Offset: 0x00077420
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetOwner(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.owner, dirtyBit, ref this.___ownerNetId);
			}
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x00079270 File Offset: 0x00077470
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
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
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x000792DC File Offset: 0x000774DC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetOwner(reader.ReadGameObject());
			}
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x00011735 File Offset: 0x0000F935
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04001A25 RID: 6693
		[Tooltip("The child transform from which projectiles will be fired.")]
		public Transform fireTransform;

		// Token: 0x04001A26 RID: 6694
		[Tooltip("How long it takes to start firing.")]
		public float startDelay = 4f;

		// Token: 0x04001A27 RID: 6695
		[Tooltip("Firing interval.")]
		public float fireInterval = 1f;

		// Token: 0x04001A28 RID: 6696
		[Tooltip("The prefab to fire as a projectile.")]
		public GameObject projectilePrefab;

		// Token: 0x04001A29 RID: 6697
		[Tooltip("The damage coefficient to multiply by the owner's damage stat for the projectile's final damage value.")]
		public float damageCoefficient;

		// Token: 0x04001A2A RID: 6698
		[Tooltip("The force of the projectile's damage.")]
		public float damageForce;

		// Token: 0x04001A2B RID: 6699
		[SyncVar(hook = "SetOwner")]
		private GameObject owner;

		// Token: 0x04001A2C RID: 6700
		private Transform targetTransform;

		// Token: 0x04001A2D RID: 6701
		private Vector3 velocity;

		// Token: 0x04001A2E RID: 6702
		private static readonly Vector3 targetLocalPosition = new Vector3(0f, 12f, -3f);

		// Token: 0x04001A2F RID: 6703
		private float fireTimer;

		// Token: 0x04001A30 RID: 6704
		private InputBankTest ownerInputBank;

		// Token: 0x04001A31 RID: 6705
		private CharacterBody ownerCharacterBody;

		// Token: 0x04001A32 RID: 6706
		private bool isCrit;

		// Token: 0x04001A33 RID: 6707
		private bool foundOwner;

		// Token: 0x04001A34 RID: 6708
		private NetworkInstanceId ___ownerNetId;
	}
}
