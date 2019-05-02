using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000306 RID: 774
	public class GravitatePickup : MonoBehaviour
	{
		// Token: 0x06000FF7 RID: 4087 RVA: 0x0000C3E9 File Offset: 0x0000A5E9
		private void Start()
		{
			this.teamFilter.teamIndex = TeamIndex.Player;
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x0000C3F7 File Offset: 0x0000A5F7
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active && !this.gravitateTarget && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				this.gravitateTarget = other.gameObject.transform;
			}
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x00060504 File Offset: 0x0005E704
		private void FixedUpdate()
		{
			if (this.gravitateTarget)
			{
				this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, (this.gravitateTarget.transform.position - base.transform.position).normalized * this.maxSpeed, this.acceleration);
			}
		}

		// Token: 0x040013EA RID: 5098
		private Transform gravitateTarget;

		// Token: 0x040013EB RID: 5099
		[Tooltip("The rigidbody to set the velocity of.")]
		public Rigidbody rigidbody;

		// Token: 0x040013EC RID: 5100
		[Tooltip("The TeamFilter which controls which team can activate this trigger.")]
		public TeamFilter teamFilter;

		// Token: 0x040013ED RID: 5101
		public float acceleration;

		// Token: 0x040013EE RID: 5102
		public float maxSpeed;
	}
}
