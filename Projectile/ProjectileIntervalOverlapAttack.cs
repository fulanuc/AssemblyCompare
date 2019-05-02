using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054F RID: 1359
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileIntervalOverlapAttack : MonoBehaviour
	{
		// Token: 0x06001E5D RID: 7773 RVA: 0x00016323 File Offset: 0x00014523
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E5E RID: 7774 RVA: 0x0001633D File Offset: 0x0001453D
		private void Start()
		{
			this.countdown = 0f;
		}

		// Token: 0x06001E5F RID: 7775 RVA: 0x00095B18 File Offset: 0x00093D18
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.countdown -= Time.fixedDeltaTime;
				if (this.countdown <= 0f)
				{
					this.countdown += this.interval;
					if (this.hitBoxGroup)
					{
						new OverlapAttack
						{
							attacker = this.projectileController.owner,
							inflictor = base.gameObject,
							teamIndex = this.projectileController.teamFilter.teamIndex,
							damage = this.projectileDamage.damage * this.damageCoefficient,
							hitBoxGroup = this.hitBoxGroup,
							isCrit = this.projectileDamage.crit,
							procCoefficient = 0f,
							damageType = this.projectileDamage.damageType
						}.Fire(null);
					}
				}
			}
		}

		// Token: 0x040020E4 RID: 8420
		public HitBoxGroup hitBoxGroup;

		// Token: 0x040020E5 RID: 8421
		public float interval;

		// Token: 0x040020E6 RID: 8422
		public float damageCoefficient = 1f;

		// Token: 0x040020E7 RID: 8423
		private float countdown;

		// Token: 0x040020E8 RID: 8424
		private ProjectileController projectileController;

		// Token: 0x040020E9 RID: 8425
		private ProjectileDamage projectileDamage;
	}
}
