using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000308 RID: 776
	[RequireComponent(typeof(ItemFollower))]
	public class GravCubeController : MonoBehaviour
	{
		// Token: 0x06001009 RID: 4105 RVA: 0x0000C499 File Offset: 0x0000A699
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
			if (this.itemFollower)
			{
				this.itemFollowerAnimator = this.itemFollower.followerInstance.GetComponentInChildren<Animator>();
			}
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0000C4CA File Offset: 0x0000A6CA
		public void ActivateCube(float duration)
		{
			this.activeTimer = duration;
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x00060738 File Offset: 0x0005E938
		private void Update()
		{
			this.activeTimer -= Time.deltaTime;
			if (this.activeTimer > 0f)
			{
				this.itemFollowerAnimator.SetBool("active", true);
				return;
			}
			this.itemFollowerAnimator.SetBool("active", false);
		}

		// Token: 0x040013FF RID: 5119
		private ItemFollower itemFollower;

		// Token: 0x04001400 RID: 5120
		private float activeTimer;

		// Token: 0x04001401 RID: 5121
		private Animator itemFollowerAnimator;
	}
}
