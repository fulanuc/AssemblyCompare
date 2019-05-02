﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002BA RID: 698
	[RequireComponent(typeof(CharacterBody))]
	public class ContactDamage : MonoBehaviour
	{
		// Token: 0x06000E38 RID: 3640 RVA: 0x0000AF9F File Offset: 0x0000919F
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00057B64 File Offset: 0x00055D64
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f)
				{
					this.overlapAttack = new OverlapAttack
					{
						attacker = base.gameObject,
						inflictor = base.gameObject,
						hitBoxGroup = this.hitBoxGroup,
						teamIndex = TeamComponent.GetObjectTeam(base.gameObject)
					};
					this.refreshTimer = this.damageInterval;
				}
				this.overlapAttack.damage = this.characterBody.damage * this.damagePerSecondCoefficient * this.damageInterval;
				this.overlapAttack.pushAwayForce = this.pushForcePerSecond * this.damageInterval;
				this.overlapAttack.damageType = this.damageType;
				this.overlapAttack.Fire(null);
			}
		}

		// Token: 0x0400120B RID: 4619
		public float damagePerSecondCoefficient = 2f;

		// Token: 0x0400120C RID: 4620
		public float damageInterval = 0.25f;

		// Token: 0x0400120D RID: 4621
		public float pushForcePerSecond = 4000f;

		// Token: 0x0400120E RID: 4622
		public HitBoxGroup hitBoxGroup;

		// Token: 0x0400120F RID: 4623
		public DamageType damageType;

		// Token: 0x04001210 RID: 4624
		private OverlapAttack overlapAttack;

		// Token: 0x04001211 RID: 4625
		private CharacterBody characterBody;

		// Token: 0x04001212 RID: 4626
		private float refreshTimer;
	}
}
