using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000256 RID: 598
	public class AmmoPickup : MonoBehaviour
	{
		// Token: 0x06000B25 RID: 2853 RVA: 0x0004B02C File Offset: 0x0004922C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				SkillLocator component = other.GetComponent<SkillLocator>();
				if (component)
				{
					this.alive = false;
					component.ApplyAmmoPack();
					EffectManager.instance.SimpleEffect(this.pickupEffect, base.transform.position, Quaternion.identity, true);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000F26 RID: 3878
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000F27 RID: 3879
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000F28 RID: 3880
		public GameObject pickupEffect;

		// Token: 0x04000F29 RID: 3881
		private bool alive = true;
	}
}
