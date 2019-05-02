using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000272 RID: 626
	public class BuffPickup : MonoBehaviour
	{
		// Token: 0x06000BC3 RID: 3011 RVA: 0x0004CD2C File Offset: 0x0004AF2C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					component.AddTimedBuff(this.buffIndex, this.buffDuration);
					UnityEngine.Object.Instantiate<GameObject>(this.pickupEffect, other.transform.position, Quaternion.identity);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000FA7 RID: 4007
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000FA8 RID: 4008
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000FA9 RID: 4009
		public GameObject pickupEffect;

		// Token: 0x04000FAA RID: 4010
		public BuffIndex buffIndex;

		// Token: 0x04000FAB RID: 4011
		public float buffDuration;

		// Token: 0x04000FAC RID: 4012
		private bool alive = true;
	}
}
