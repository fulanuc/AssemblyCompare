using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055E RID: 1374
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileIntervalOverlapAttack : MonoBehaviour
	{
		// Token: 0x06001EC7 RID: 7879 RVA: 0x00016802 File Offset: 0x00014A02
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x0001681C File Offset: 0x00014A1C
		private void Start()
		{
			this.countdown = 0f;
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x00096834 File Offset: 0x00094A34
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

		// Token: 0x04002122 RID: 8482
		public HitBoxGroup hitBoxGroup;

		// Token: 0x04002123 RID: 8483
		public float interval;

		// Token: 0x04002124 RID: 8484
		public float damageCoefficient = 1f;

		// Token: 0x04002125 RID: 8485
		private float countdown;

		// Token: 0x04002126 RID: 8486
		private ProjectileController projectileController;

		// Token: 0x04002127 RID: 8487
		private ProjectileDamage projectileDamage;
	}
}
