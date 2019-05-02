using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030A RID: 778
	[RequireComponent(typeof(TeamFilter))]
	public class HealingWard : NetworkBehaviour
	{
		// Token: 0x06001011 RID: 4113 RVA: 0x0000C520 File Offset: 0x0000A720
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x000607F8 File Offset: 0x0005E9F8
		private void Start()
		{
			RaycastHit raycastHit;
			if (this.floorWard && Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				base.transform.up = raycastHit.normal;
			}
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x00060864 File Offset: 0x0005EA64
		private void Update()
		{
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(this.rangeIndicator.localScale.x, this.radius, ref this.rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0000C52E File Offset: 0x0000A72E
		private void FixedUpdate()
		{
			this.healTimer -= Time.fixedDeltaTime;
			if (this.healTimer <= 0f && NetworkServer.active)
			{
				this.healTimer = this.interval;
				this.HealOccupants();
			}
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x000608B8 File Offset: 0x0005EAB8
		private void HealOccupants()
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(this.teamFilter.teamIndex);
			float num = this.radius * this.radius;
			Vector3 position = base.transform.position;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
				{
					HealthComponent component = teamMembers[i].GetComponent<HealthComponent>();
					if (component)
					{
						float num2 = this.healPoints + component.fullHealth * this.healFraction;
						if (num2 > 0f)
						{
							component.Heal(num2, default(ProcChainMask), true);
						}
					}
				}
			}
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06001018 RID: 4120 RVA: 0x00060970 File Offset: 0x0005EB70
		// (set) Token: 0x06001019 RID: 4121 RVA: 0x0000C57B File Offset: 0x0000A77B
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

		// Token: 0x0600101A RID: 4122 RVA: 0x00060984 File Offset: 0x0005EB84
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

		// Token: 0x0600101B RID: 4123 RVA: 0x000609F0 File Offset: 0x0005EBF0
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

		// Token: 0x04001407 RID: 5127
		[SyncVar]
		[Tooltip("The area of effect.")]
		public float radius;

		// Token: 0x04001408 RID: 5128
		[Tooltip("How long between heal pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x04001409 RID: 5129
		[Tooltip("How many hit points to restore each pulse.")]
		public float healPoints;

		// Token: 0x0400140A RID: 5130
		[Tooltip("What fraction of the healee max health to restore each pulse.")]
		public float healFraction;

		// Token: 0x0400140B RID: 5131
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x0400140C RID: 5132
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x0400140D RID: 5133
		private TeamFilter teamFilter;

		// Token: 0x0400140E RID: 5134
		private float healTimer;

		// Token: 0x0400140F RID: 5135
		private float rangeIndicatorScaleVelocity;
	}
}
