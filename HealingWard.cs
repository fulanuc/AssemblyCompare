using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000307 RID: 775
	[RequireComponent(typeof(TeamFilter))]
	public class HealingWard : NetworkBehaviour
	{
		// Token: 0x06000FFB RID: 4091 RVA: 0x0000C436 File Offset: 0x0000A636
		private void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00060574 File Offset: 0x0005E774
		private void Start()
		{
			RaycastHit raycastHit;
			if (this.floorWard && Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				base.transform.position = raycastHit.point;
				base.transform.up = raycastHit.normal;
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x000605E0 File Offset: 0x0005E7E0
		private void Update()
		{
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(this.rangeIndicator.localScale.x, this.radius, ref this.rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0000C444 File Offset: 0x0000A644
		private void FixedUpdate()
		{
			this.healTimer -= Time.fixedDeltaTime;
			if (this.healTimer <= 0f && NetworkServer.active)
			{
				this.healTimer = this.interval;
				this.HealOccupants();
			}
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x00060634 File Offset: 0x0005E834
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

		// Token: 0x06001001 RID: 4097 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x000606EC File Offset: 0x0005E8EC
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x0000C491 File Offset: 0x0000A691
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

		// Token: 0x06001004 RID: 4100 RVA: 0x00060700 File Offset: 0x0005E900
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

		// Token: 0x06001005 RID: 4101 RVA: 0x0006076C File Offset: 0x0005E96C
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

		// Token: 0x040013EF RID: 5103
		[Tooltip("The area of effect.")]
		[SyncVar]
		public float radius;

		// Token: 0x040013F0 RID: 5104
		[Tooltip("How long between heal pulses in the area of effect.")]
		public float interval = 1f;

		// Token: 0x040013F1 RID: 5105
		[Tooltip("How many hit points to restore each pulse.")]
		public float healPoints;

		// Token: 0x040013F2 RID: 5106
		[Tooltip("What fraction of the healee max health to restore each pulse.")]
		public float healFraction;

		// Token: 0x040013F3 RID: 5107
		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		// Token: 0x040013F4 RID: 5108
		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		// Token: 0x040013F5 RID: 5109
		private TeamFilter teamFilter;

		// Token: 0x040013F6 RID: 5110
		private float healTimer;

		// Token: 0x040013F7 RID: 5111
		private float rangeIndicatorScaleVelocity;
	}
}
