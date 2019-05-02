using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000305 RID: 773
	[RequireComponent(typeof(ItemFollower))]
	public class GravCubeController : MonoBehaviour
	{
		// Token: 0x06000FF3 RID: 4083 RVA: 0x0000C3AF File Offset: 0x0000A5AF
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
			if (this.itemFollower)
			{
				this.itemFollowerAnimator = this.itemFollower.followerInstance.GetComponentInChildren<Animator>();
			}
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x0000C3E0 File Offset: 0x0000A5E0
		public void ActivateCube(float duration)
		{
			this.activeTimer = duration;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x000604B4 File Offset: 0x0005E6B4
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

		// Token: 0x040013E7 RID: 5095
		private ItemFollower itemFollower;

		// Token: 0x040013E8 RID: 5096
		private float activeTimer;

		// Token: 0x040013E9 RID: 5097
		private Animator itemFollowerAnimator;
	}
}
