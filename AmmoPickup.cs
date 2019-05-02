using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000256 RID: 598
	public class AmmoPickup : MonoBehaviour
	{
		// Token: 0x06000B28 RID: 2856 RVA: 0x0004B238 File Offset: 0x00049438
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

		// Token: 0x04000F2C RID: 3884
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000F2D RID: 3885
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000F2E RID: 3886
		public GameObject pickupEffect;

		// Token: 0x04000F2F RID: 3887
		private bool alive = true;
	}
}
