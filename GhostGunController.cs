using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000304 RID: 772
	public class GhostGunController : MonoBehaviour
	{
		// Token: 0x06000FDD RID: 4061 RVA: 0x0000C311 File Offset: 0x0000A511
		private void Start()
		{
			this.fireTimer = 0f;
			this.ammo = 6;
			this.kills = 0;
			this.timeoutTimer = this.timeout;
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0005E098 File Offset: 0x0005C298
		private void Fire(Vector3 origin, Vector3 aimDirection)
		{
			CharacterBody component = this.owner.GetComponent<CharacterBody>();
			int killCount = component.killCount;
			new BulletAttack
			{
				aimVector = aimDirection,
				bulletCount = 1u,
				damage = this.CalcDamage(),
				force = 2400f,
				maxSpread = 0f,
				minSpread = 0f,
				muzzleName = "muzzle",
				origin = origin,
				owner = this.owner,
				procCoefficient = 0f,
				tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerSmokeChase"),
				hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Hitspark1"),
				damageColorIndex = DamageColorIndex.Item
			}.Fire();
			this.kills += component.killCount - killCount;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0005E164 File Offset: 0x0005C364
		private float CalcDamage()
		{
			float damage = this.owner.GetComponent<CharacterBody>().damage;
			return 5f * Mathf.Pow(2f, (float)this.kills) * damage;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0005E19C File Offset: 0x0005C39C
		private bool HasLoS(GameObject target)
		{
			Ray ray = new Ray(base.transform.position, target.transform.position - base.transform.position);
			RaycastHit raycastHit = default(RaycastHit);
			return !Physics.Raycast(ray, out raycastHit, this.maxRange, LayerIndex.defaultLayer.mask | LayerIndex.world.mask, QueryTriggerInteraction.Ignore) || raycastHit.collider.gameObject == target;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0005E228 File Offset: 0x0005C428
		private bool WillHit(GameObject target)
		{
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(ray, out raycastHit, this.maxRange, LayerIndex.entityPrecise.mask | LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				HurtBox component = raycastHit.collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						return healthComponent.gameObject == target;
					}
				}
			}
			return false;
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0005E2C0 File Offset: 0x0005C4C0
		private GameObject FindTarget()
		{
			TeamIndex teamA = TeamIndex.Neutral;
			TeamComponent component = this.owner.GetComponent<TeamComponent>();
			if (component)
			{
				teamA = component.teamIndex;
			}
			Vector3 position = base.transform.position;
			float num = this.CalcDamage();
			float num2 = this.maxRange * this.maxRange;
			GameObject gameObject = null;
			GameObject result = null;
			float num3 = 0f;
			float num4 = float.PositiveInfinity;
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if (TeamManager.IsTeamEnemy(teamA, teamIndex))
				{
					ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
					for (int i = 0; i < teamMembers.Count; i++)
					{
						GameObject gameObject2 = teamMembers[i].gameObject;
						if ((gameObject2.transform.position - position).sqrMagnitude <= num2)
						{
							HealthComponent component2 = teamMembers[i].GetComponent<HealthComponent>();
							if (component2)
							{
								if (component2.health <= num)
								{
									if (component2.health > num3 && this.HasLoS(gameObject2))
									{
										gameObject = gameObject2;
										num3 = component2.health;
									}
								}
								else if (component2.health < num4 && this.HasLoS(gameObject2))
								{
									result = gameObject2;
									num4 = component2.health;
								}
							}
						}
					}
				}
			}
			if (!gameObject)
			{
				return result;
			}
			return gameObject;
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0005E410 File Offset: 0x0005C610
		private void FixedUpdate()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (!this.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			InputBankTest component = this.owner.GetComponent<InputBankTest>();
			Vector3 vector = component ? component.aimDirection : base.transform.forward;
			if (this.target)
			{
				vector = (this.target.transform.position - base.transform.position).normalized;
			}
			base.transform.forward = Vector3.RotateTowards(base.transform.forward, vector, 0.0174532924f * this.turnSpeed * Time.fixedDeltaTime, 0f);
			Vector3 vector2 = this.owner.transform.position + base.transform.rotation * this.localOffset;
			base.transform.position = Vector3.SmoothDamp(base.transform.position, vector2, ref this.velocity, this.positionSmoothTime, float.PositiveInfinity, Time.fixedDeltaTime);
			this.fireTimer -= Time.fixedDeltaTime;
			this.timeoutTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.target = this.FindTarget();
				this.fireTimer = this.interval;
			}
			if (this.target && this.WillHit(this.target))
			{
				Vector3 normalized = (this.target.transform.position - base.transform.position).normalized;
				this.Fire(base.transform.position, normalized);
				this.ammo--;
				this.target = null;
				this.timeoutTimer = this.timeout;
			}
			if (this.ammo <= 0 || this.timeoutTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x040013D4 RID: 5076
		public GameObject owner;

		// Token: 0x040013D5 RID: 5077
		public float interval;

		// Token: 0x040013D6 RID: 5078
		public float maxRange = 20f;

		// Token: 0x040013D7 RID: 5079
		public float turnSpeed = 180f;

		// Token: 0x040013D8 RID: 5080
		public Vector3 localOffset = Vector3.zero;

		// Token: 0x040013D9 RID: 5081
		public float positionSmoothTime = 0.05f;

		// Token: 0x040013DA RID: 5082
		public float timeout = 2f;

		// Token: 0x040013DB RID: 5083
		private float fireTimer;

		// Token: 0x040013DC RID: 5084
		private float timeoutTimer;

		// Token: 0x040013DD RID: 5085
		private int ammo;

		// Token: 0x040013DE RID: 5086
		private int kills;

		// Token: 0x040013DF RID: 5087
		private GameObject target;

		// Token: 0x040013E0 RID: 5088
		private Vector3 velocity;
	}
}
