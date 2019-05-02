using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000273 RID: 627
	[RequireComponent(typeof(TeamFilter))]
	public class BuffWard : NetworkBehaviour
	{
		// Token: 0x06000BCE RID: 3022 RVA: 0x00009549 File Offset: 0x00007749
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x0004CFB4 File Offset: 0x0004B1B4
		private void Start()
		{
			RaycastHit raycastHit;
			if (this.floorWard && Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				base.transform.up = raycastHit.normal;
			}
			if (this.rangeIndicator && this.expires)
			{
				ScaleParticleSystemDuration component = this.rangeIndicator.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.expireDuration;
				}
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0004D054 File Offset: 0x0004B254
		private void Update()
		{
			this.calculatedRadius = (this.animateRadius ? (this.radius * this.radiusCoefficientCurve.Evaluate(this.stopwatch / this.expireDuration)) : this.radius);
			this.stopwatch += Time.deltaTime;
			if (this.expires && NetworkServer.active && this.expireDuration <= this.stopwatch)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(this.rangeIndicator.localScale.x, this.calculatedRadius, ref this.rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0004D118 File Offset: 0x0004B318
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.buffTimer -= Time.fixedDeltaTime;
				if (this.buffTimer <= 0f)
				{
					this.buffTimer = this.interval;
					float radiusSqr = this.calculatedRadius * this.calculatedRadius;
					Vector3 position = base.transform.position;
					if (this.invertTeamFilter)
					{
						for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
						{
							if (teamIndex != this.teamFilter.teamIndex)
							{
								this.BuffTeam(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
							}
						}
						return;
					}
					this.BuffTeam(TeamComponent.GetTeamMembers(this.teamFilter.teamIndex), radiusSqr, position);
				}
			}
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0004D1C0 File Offset: 0x0004B3C0
		private void BuffTeam(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			foreach (TeamComponent teamComponent in recipients)
			{
				if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
				{
					CharacterBody component = teamComponent.GetComponent<CharacterBody>();
					if (component)
					{
						component.AddTimedBuff(this.buffType, this.buffDuration);
					}
				}
			}
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x0004D248 File Offset: 0x0004B448
		// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x0000956A File Offset: 0x0000776A
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1u);
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0004D25C File Offset: 0x0004B45C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
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
				writer.Write(this.radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0004D2C8 File Offset: 0x0004B4C8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.radius = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.radius = reader.ReadSingle();
			}
		}

		// Token: 0x04000FB3 RID: 4019
		[SyncVar]
		[Tooltip("The area of effect.")]
		public float radius;

		// Token: 0x04000FB4 RID: 4020
		[Tooltip("How long between buff pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x04000FB5 RID: 4021
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x04000FB6 RID: 4022
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04000FB7 RID: 4023
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04000FB8 RID: 4024
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x04000FB9 RID: 4025
		[Tooltip("Does the ward disappear over time?")]
		public bool expires;

		// Token: 0x04000FBA RID: 4026
		[Tooltip("If set, applies to all teams BUT the one selected.")]
		public bool invertTeamFilter;

		// Token: 0x04000FBB RID: 4027
		public float expireDuration;

		// Token: 0x04000FBC RID: 4028
		public bool animateRadius;

		// Token: 0x04000FBD RID: 4029
		public AnimationCurve radiusCoefficientCurve;

		// Token: 0x04000FBE RID: 4030
		private TeamFilter teamFilter;

		// Token: 0x04000FBF RID: 4031
		private float buffTimer;

		// Token: 0x04000FC0 RID: 4032
		private float rangeIndicatorScaleVelocity;

		// Token: 0x04000FC1 RID: 4033
		private float stopwatch;

		// Token: 0x04000FC2 RID: 4034
		private float calculatedRadius;
	}
}
