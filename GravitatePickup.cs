using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000309 RID: 777
	public class GravitatePickup : MonoBehaviour
	{
		// Token: 0x0600100D RID: 4109 RVA: 0x0000C4D3 File Offset: 0x0000A6D3
		private void Start()
		{
			this.teamFilter.teamIndex = TeamIndex.Player;
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0000C4E1 File Offset: 0x0000A6E1
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active && !this.gravitateTarget && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				this.gravitateTarget = other.gameObject.transform;
			}
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x00060788 File Offset: 0x0005E988
		private void FixedUpdate()
		{
			if (this.gravitateTarget)
			{
				this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, (this.gravitateTarget.transform.position - base.transform.position).normalized * this.maxSpeed, this.acceleration);
			}
		}

		// Token: 0x04001402 RID: 5122
		private Transform gravitateTarget;

		// Token: 0x04001403 RID: 5123
		[Tooltip("The rigidbody to set the velocity of.")]
		public Rigidbody rigidbody;

		// Token: 0x04001404 RID: 5124
		[Tooltip("The TeamFilter which controls which team can activate this trigger.")]
		public TeamFilter teamFilter;

		// Token: 0x04001405 RID: 5125
		public float acceleration;

		// Token: 0x04001406 RID: 5126
		public float maxSpeed;
	}
}
