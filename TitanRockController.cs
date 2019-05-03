using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000403 RID: 1027
	public class TitanRockController : NetworkBehaviour
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x000112AE File Offset: 0x0000F4AE
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.SetOwner(this.owner);
				return;
			}
			this.fireTimer = this.startDelay;
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x0007886C File Offset: 0x00076A6C
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

		// Token: 0x060016EB RID: 5867 RVA: 0x0007899C File Offset: 0x00076B9C
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

		// Token: 0x060016EC RID: 5868 RVA: 0x00078AE4 File Offset: 0x00076CE4
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

		// Token: 0x060016ED RID: 5869 RVA: 0x00078B40 File Offset: 0x00076D40
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

		// Token: 0x060016F0 RID: 5872 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x00078C4C File Offset: 0x00076E4C
		// (set) Token: 0x060016F2 RID: 5874 RVA: 0x00078C60 File Offset: 0x00076E60
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

		// Token: 0x060016F3 RID: 5875 RVA: 0x00078CB0 File Offset: 0x00076EB0
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

		// Token: 0x060016F4 RID: 5876 RVA: 0x00078D1C File Offset: 0x00076F1C
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

		// Token: 0x060016F5 RID: 5877 RVA: 0x00011309 File Offset: 0x0000F509
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x040019FC RID: 6652
		[Tooltip("The child transform from which projectiles will be fired.")]
		public Transform fireTransform;

		// Token: 0x040019FD RID: 6653
		[Tooltip("How long it takes to start firing.")]
		public float startDelay = 4f;

		// Token: 0x040019FE RID: 6654
		[Tooltip("Firing interval.")]
		public float fireInterval = 1f;

		// Token: 0x040019FF RID: 6655
		[Tooltip("The prefab to fire as a projectile.")]
		public GameObject projectilePrefab;

		// Token: 0x04001A00 RID: 6656
		[Tooltip("The damage coefficient to multiply by the owner's damage stat for the projectile's final damage value.")]
		public float damageCoefficient;

		// Token: 0x04001A01 RID: 6657
		[Tooltip("The force of the projectile's damage.")]
		public float damageForce;

		// Token: 0x04001A02 RID: 6658
		[SyncVar(hook = "SetOwner")]
		private GameObject owner;

		// Token: 0x04001A03 RID: 6659
		private Transform targetTransform;

		// Token: 0x04001A04 RID: 6660
		private Vector3 velocity;

		// Token: 0x04001A05 RID: 6661
		private static readonly Vector3 targetLocalPosition = new Vector3(0f, 12f, -3f);

		// Token: 0x04001A06 RID: 6662
		private float fireTimer;

		// Token: 0x04001A07 RID: 6663
		private InputBankTest ownerInputBank;

		// Token: 0x04001A08 RID: 6664
		private CharacterBody ownerCharacterBody;

		// Token: 0x04001A09 RID: 6665
		private bool isCrit;

		// Token: 0x04001A0A RID: 6666
		private bool foundOwner;

		// Token: 0x04001A0B RID: 6667
		private NetworkInstanceId ___ownerNetId;
	}
}
