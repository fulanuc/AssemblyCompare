using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C8 RID: 712
	[RequireComponent(typeof(TeamFilter))]
	public class DelayBlast : MonoBehaviour
	{
		// Token: 0x06000E77 RID: 3703 RVA: 0x0000B2B3 File Offset: 0x000094B3
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x00058E88 File Offset: 0x00057088
		private void Start()
		{
			if (this.delayEffect)
			{
				EffectManager.instance.SpawnEffect(this.delayEffect, new EffectData
				{
					origin = base.transform.position,
					rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
					scale = this.radius
				}, true);
			}
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00058EEC File Offset: 0x000570EC
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer >= this.maxTimer)
				{
					EffectManager.instance.SpawnEffect(this.explosionEffect, new EffectData
					{
						origin = base.transform.position,
						rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
						scale = this.radius
					}, true);
					new BlastAttack
					{
						position = this.position,
						baseDamage = this.baseDamage,
						baseForce = this.baseForce,
						bonusForce = this.bonusForce,
						radius = this.radius,
						attacker = this.attacker,
						inflictor = this.inflictor,
						teamIndex = this.teamFilter.teamIndex,
						crit = this.crit,
						damageColorIndex = this.damageColorIndex,
						damageType = this.damageType
					}.Fire();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04001259 RID: 4697
		[HideInInspector]
		public Vector3 position;

		// Token: 0x0400125A RID: 4698
		[HideInInspector]
		public GameObject attacker;

		// Token: 0x0400125B RID: 4699
		[HideInInspector]
		public GameObject inflictor;

		// Token: 0x0400125C RID: 4700
		[HideInInspector]
		public float baseDamage;

		// Token: 0x0400125D RID: 4701
		[HideInInspector]
		public bool crit;

		// Token: 0x0400125E RID: 4702
		[HideInInspector]
		public float baseForce;

		// Token: 0x0400125F RID: 4703
		[HideInInspector]
		public float radius;

		// Token: 0x04001260 RID: 4704
		[HideInInspector]
		public Vector3 bonusForce;

		// Token: 0x04001261 RID: 4705
		[HideInInspector]
		public float maxTimer;

		// Token: 0x04001262 RID: 4706
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001263 RID: 4707
		[HideInInspector]
		public BlastAttack.FalloffModel falloffModel;

		// Token: 0x04001264 RID: 4708
		[HideInInspector]
		public DamageType damageType;

		// Token: 0x04001265 RID: 4709
		public GameObject explosionEffect;

		// Token: 0x04001266 RID: 4710
		public GameObject delayEffect;

		// Token: 0x04001267 RID: 4711
		private float timer;

		// Token: 0x04001268 RID: 4712
		private TeamFilter teamFilter;
	}
}
