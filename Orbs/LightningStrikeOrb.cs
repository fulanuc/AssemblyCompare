using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000525 RID: 1317
	public class LightningStrikeOrb : Orb, IOrbFixedUpdateBehavior
	{
		// Token: 0x06001DC6 RID: 7622 RVA: 0x00015C83 File Offset: 0x00013E83
		public override void Begin()
		{
			base.duration = 0.5f;
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x06001DC7 RID: 7623 RVA: 0x0009115C File Offset: 0x0008F35C
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

		// Token: 0x06001DC8 RID: 7624 RVA: 0x00015CB3 File Offset: 0x00013EB3
		public void FixedUpdate()
		{
			if (this.target)
			{
				this.lastKnownTargetPosition = this.target.transform.position;
			}
		}

		// Token: 0x04001FCC RID: 8140
		private const float speed = 30f;

		// Token: 0x04001FCD RID: 8141
		public float damageValue;

		// Token: 0x04001FCE RID: 8142
		public GameObject attacker;

		// Token: 0x04001FCF RID: 8143
		public TeamIndex teamIndex;

		// Token: 0x04001FD0 RID: 8144
		public bool isCrit;

		// Token: 0x04001FD1 RID: 8145
		public float scale;

		// Token: 0x04001FD2 RID: 8146
		public ProcChainMask procChainMask;

		// Token: 0x04001FD3 RID: 8147
		public float procCoefficient = 0.2f;

		// Token: 0x04001FD4 RID: 8148
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001FD5 RID: 8149
		private Vector3 lastKnownTargetPosition;
	}
}
