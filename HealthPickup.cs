using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030F RID: 783
	public class HealthPickup : MonoBehaviour
	{
		// Token: 0x06001053 RID: 4179 RVA: 0x00061F58 File Offset: 0x00060158
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						component.healthComponent.Heal(this.flatHealing + healthComponent.fullHealth * this.fractionalHealing, default(ProcChainMask), true);
						EffectManager.instance.SpawnEffect(this.pickupEffect, new EffectData
						{
							origin = base.transform.position
						}, true);
					}
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04001435 RID: 5173
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04001436 RID: 5174
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04001437 RID: 5175
		public GameObject pickupEffect;

		// Token: 0x04001438 RID: 5176
		public float flatHealing;

		// Token: 0x04001439 RID: 5177
		public float fractionalHealing;

		// Token: 0x0400143A RID: 5178
		private bool alive = true;
	}
}
