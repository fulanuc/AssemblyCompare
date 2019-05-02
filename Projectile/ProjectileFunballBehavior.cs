using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000555 RID: 1365
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileFunballBehavior : NetworkBehaviour
	{
		// Token: 0x06001EA5 RID: 7845 RVA: 0x0001663C File Offset: 0x0001483C
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001EA6 RID: 7846 RVA: 0x0001664A File Offset: 0x0001484A
		private void Start()
		{
			this.Networktimer = -1f;
		}

		// Token: 0x06001EA7 RID: 7847 RVA: 0x00095E60 File Offset: 0x00094060
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

		// Token: 0x06001EA8 RID: 7848 RVA: 0x00016657 File Offset: 0x00014857
		private void OnCollisionEnter(Collision collision)
		{
			this.fuseStarted = true;
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001EAB RID: 7851 RVA: 0x00095F90 File Offset: 0x00094190
		// (set) Token: 0x06001EAC RID: 7852 RVA: 0x00016689 File Offset: 0x00014889
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

		// Token: 0x06001EAD RID: 7853 RVA: 0x00095FA4 File Offset: 0x000941A4
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

		// Token: 0x06001EAE RID: 7854 RVA: 0x00096010 File Offset: 0x00094210
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

		// Token: 0x040020E9 RID: 8425
		[Tooltip("The effect to use for the explosion.")]
		public GameObject explosionPrefab;

		// Token: 0x040020EA RID: 8426
		[Tooltip("How many seconds until detonation.")]
		public float duration;

		// Token: 0x040020EB RID: 8427
		[Tooltip("Radius of blast in meters.")]
		public float blastRadius = 1f;

		// Token: 0x040020EC RID: 8428
		[Tooltip("Maximum damage of blast.")]
		public float blastDamage = 1f;

		// Token: 0x040020ED RID: 8429
		[Tooltip("Force of blast.")]
		public float blastForce = 1f;

		// Token: 0x040020EE RID: 8430
		private ProjectileController projectileController;

		// Token: 0x040020EF RID: 8431
		[SyncVar]
		private float timer;

		// Token: 0x040020F0 RID: 8432
		private bool fuseStarted;
	}
}
