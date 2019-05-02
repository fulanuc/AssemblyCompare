using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000546 RID: 1350
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFunballBehavior : NetworkBehaviour
	{
		// Token: 0x06001E3B RID: 7739 RVA: 0x0001615D File Offset: 0x0001435D
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E3C RID: 7740 RVA: 0x0001616B File Offset: 0x0001436B
		private void Start()
		{
			this.Networktimer = -1f;
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x00095144 File Offset: 0x00093344
		private void FixedUpdate()
		{
			if (NetworkServer.active && this.fuseStarted)
			{
				this.Networktimer = this.timer + Time.fixedDeltaTime;
				if (this.timer >= this.duration)
				{
					EffectManager.instance.SpawnEffect(this.explosionPrefab, new EffectData
					{
						origin = base.transform.position,
						scale = this.blastRadius
					}, true);
					new BlastAttack
					{
						attacker = this.projectileController.owner,
						inflictor = base.gameObject,
						teamIndex = this.projectileController.teamFilter.teamIndex,
						position = base.transform.position,
						procChainMask = this.projectileController.procChainMask,
						procCoefficient = this.projectileController.procCoefficient,
						radius = this.blastRadius,
						baseDamage = this.blastDamage,
						baseForce = this.blastForce,
						bonusForce = Vector3.zero,
						crit = false,
						damageType = DamageType.Generic
					}.Fire();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x00016178 File Offset: 0x00014378
		private void OnCollisionEnter(Collision collision)
		{
			this.fuseStarted = true;
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06001E41 RID: 7745 RVA: 0x00095274 File Offset: 0x00093474
		// (set) Token: 0x06001E42 RID: 7746 RVA: 0x000161AA File Offset: 0x000143AA
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 1u);
			}
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x00095288 File Offset: 0x00093488
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.timer);
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
				writer.Write(this.timer);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x000952F4 File Offset: 0x000934F4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.timer = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.timer = reader.ReadSingle();
			}
		}

		// Token: 0x040020AB RID: 8363
		[Tooltip("The effect to use for the explosion.")]
		public GameObject explosionPrefab;

		// Token: 0x040020AC RID: 8364
		[Tooltip("How many seconds until detonation.")]
		public float duration;

		// Token: 0x040020AD RID: 8365
		[Tooltip("Radius of blast in meters.")]
		public float blastRadius = 1f;

		// Token: 0x040020AE RID: 8366
		[Tooltip("Maximum damage of blast.")]
		public float blastDamage = 1f;

		// Token: 0x040020AF RID: 8367
		[Tooltip("Force of blast.")]
		public float blastForce = 1f;

		// Token: 0x040020B0 RID: 8368
		private ProjectileController projectileController;

		// Token: 0x040020B1 RID: 8369
		[SyncVar]
		private float timer;

		// Token: 0x040020B2 RID: 8370
		private bool fuseStarted;
	}
}
