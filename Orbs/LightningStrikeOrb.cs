using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000516 RID: 1302
	public class LightningStrikeOrb : Orb, IOrbFixedUpdateBehavior
	{
		// Token: 0x06001D5E RID: 7518 RVA: 0x000157BA File Offset: 0x000139BA
		public override void Begin()
		{
			base.duration = 0.5f;
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x000903E8 File Offset: 0x0008E5E8
		public override void OnArrival()
		{
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningStrikeImpact"), new EffectData
			{
				origin = this.lastKnownTargetPosition
			}, true);
			if (this.attacker)
			{
				new BlastAttack
				{
					attacker = this.attacker,
					baseDamage = this.damageValue,
					baseForce = 0f,
					bonusForce = Vector3.down * 3000f,
					canHurtAttacker = false,
					crit = this.isCrit,
					damageColorIndex = DamageColorIndex.Item,
					damageType = DamageType.Stun1s,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = null,
					position = this.lastKnownTargetPosition,
					procChainMask = default(ProcChainMask),
					procCoefficient = 1f,
					radius = 3f,
					teamIndex = TeamComponent.GetObjectTeam(this.attacker)
				}.Fire();
			}
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x000157EA File Offset: 0x000139EA
		public void FixedUpdate()
		{
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x04001F8E RID: 8078
		private const float speed = 30f;

		// Token: 0x04001F8F RID: 8079
		public float damageValue;

		// Token: 0x04001F90 RID: 8080
		public GameObject attacker;

		// Token: 0x04001F91 RID: 8081
		public TeamIndex teamIndex;

		// Token: 0x04001F92 RID: 8082
		public bool isCrit;

		// Token: 0x04001F93 RID: 8083
		public float scale;

		// Token: 0x04001F94 RID: 8084
		public ProcChainMask procChainMask;

		// Token: 0x04001F95 RID: 8085
		public float procCoefficient = 0.2f;

		// Token: 0x04001F96 RID: 8086
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001F97 RID: 8087
		private Vector3 lastKnownTargetPosition;
	}
}
