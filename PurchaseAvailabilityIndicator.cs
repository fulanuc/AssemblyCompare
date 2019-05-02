using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039C RID: 924
	[RequireComponent(typeof(PurchaseInteraction))]
	public class PurchaseAvailabilityIndicator : MonoBehaviour
	{
		// Token: 0x06001392 RID: 5010 RVA: 0x0000EF1B File Offset: 0x0000D11B
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0006D5A0 File Offset: 0x0006B7A0
		private void FixedUpdate()
		{
			this.indicatorObject.SetActive(this.purchaseInteraction.available);
			if (this.animator)
			{
				this.animator.SetBool(this.mecanimBool, this.purchaseInteraction.available);
			}
		}

		// Token: 0x0400172D RID: 5933
		public GameObject indicatorObject;

		// Token: 0x0400172E RID: 5934
		public Animator animator;

		// Token: 0x0400172F RID: 5935
		public string mecanimBool;

		// Token: 0x04001730 RID: 5936
		private PurchaseInteraction purchaseInteraction;
	}
}
