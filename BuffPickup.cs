using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000272 RID: 626
	public class BuffPickup : MonoBehaviour
	{
		// Token: 0x06000BCC RID: 3020 RVA: 0x0004CF38 File Offset: 0x0004B138
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

		// Token: 0x04000FAD RID: 4013
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000FAE RID: 4014
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000FAF RID: 4015
		public GameObject pickupEffect;

		// Token: 0x04000FB0 RID: 4016
		public BuffIndex buffIndex;

		// Token: 0x04000FB1 RID: 4017
		public float buffDuration;

		// Token: 0x04000FB2 RID: 4018
		private bool alive = true;
	}
}
