using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D8 RID: 728
	[RequireComponent(typeof(SphereCollider))]
	public class DisableCollisionsIfInTrigger : MonoBehaviour
	{
		// Token: 0x06000E98 RID: 3736 RVA: 0x0000B422 File Offset: 0x00009622
		public void Awake()
		{
			this.trigger = base.GetComponent<SphereCollider>();
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x0005955C File Offset: 0x0005775C
		private void OnTriggerEnter(Collider other)
		{
			if (this.trigger)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = other.transform.position;
				float num = this.trigger.radius * this.trigger.radius;
				if ((position - position2).sqrMagnitude < num)
				{
					Physics.IgnoreCollision(this.colliderToIgnore, other);
				}
			}
		}

		// Token: 0x0400129E RID: 4766
		public Collider colliderToIgnore;

		// Token: 0x0400129F RID: 4767
		private SphereCollider trigger;
	}
}
