using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A1 RID: 929
	[RequireComponent(typeof(PurchaseInteraction))]
	public class PurchaseAvailabilityIndicator : MonoBehaviour
	{
		// Token: 0x060013AF RID: 5039 RVA: 0x0000F0E5 File Offset: 0x0000D2E5
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0006D7A8 File Offset: 0x0006B9A8
		private void FixedUpdate()
		{
			this.indicatorObject.SetActive(this.purchaseInteraction.available);
			if (this.animator)
			{
				this.animator.SetBool(this.mecanimBool, this.purchaseInteraction.available);
			}
		}

		// Token: 0x04001749 RID: 5961
		public GameObject indicatorObject;

		// Token: 0x0400174A RID: 5962
		public Animator animator;

		// Token: 0x0400174B RID: 5963
		public string mecanimBool;

		// Token: 0x0400174C RID: 5964
		private PurchaseInteraction purchaseInteraction;
	}
}
