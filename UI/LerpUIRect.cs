using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000605 RID: 1541
	[RequireComponent(typeof(RectTransform))]
	public class LerpUIRect : MonoBehaviour
	{
		// Token: 0x060022C0 RID: 8896 RVA: 0x0001953F File Offset: 0x0001773F
		private void Start()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x0001954D File Offset: 0x0001774D
		private void OnDisable()
		{
			this.lerpState = LerpUIRect.LerpState.Entering;
			this.stopwatch = 0f;
			this.UpdateLerp();
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x00019567 File Offset: 0x00017767
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdateLerp();
		}

		// Token: 0x060022C3 RID: 8899 RVA: 0x000A6C44 File Offset: 0x000A4E44
		private void UpdateLerp()
		{
			LerpUIRect.LerpState lerpState = this.lerpState;
			if (lerpState != LerpUIRect.LerpState.Entering)
			{
				if (lerpState != LerpUIRect.LerpState.Leaving)
				{
					return;
				}
				float num = this.stopwatch / this.enterDuration;
				float t = this.leavingCurve.Evaluate(num);
				this.rectTransform.anchoredPosition = Vector3.LerpUnclamped(this.finalLocalPosition, this.startLocalPosition, t);
				if (num >= 1f)
				{
					this.lerpState = LerpUIRect.LerpState.Holding;
					this.stopwatch = 0f;
				}
			}
			else
			{
				float num = this.stopwatch / this.enterDuration;
				float t = this.enterCurve.Evaluate(num);
				this.rectTransform.anchoredPosition = Vector3.LerpUnclamped(this.startLocalPosition, this.finalLocalPosition, t);
				if (num >= 1f)
				{
					this.lerpState = LerpUIRect.LerpState.Holding;
					this.stopwatch = 0f;
					return;
				}
			}
		}

		// Token: 0x0400258C RID: 9612
		public Vector3 startLocalPosition;

		// Token: 0x0400258D RID: 9613
		public Vector3 finalLocalPosition;

		// Token: 0x0400258E RID: 9614
		public LerpUIRect.LerpState lerpState;

		// Token: 0x0400258F RID: 9615
		public AnimationCurve enterCurve;

		// Token: 0x04002590 RID: 9616
		public float enterDuration;

		// Token: 0x04002591 RID: 9617
		public AnimationCurve leavingCurve;

		// Token: 0x04002592 RID: 9618
		public float leaveDuration;

		// Token: 0x04002593 RID: 9619
		private float stopwatch;

		// Token: 0x04002594 RID: 9620
		private RectTransform rectTransform;

		// Token: 0x02000606 RID: 1542
		public enum LerpState
		{
			// Token: 0x04002596 RID: 9622
			Entering,
			// Token: 0x04002597 RID: 9623
			Holding,
			// Token: 0x04002598 RID: 9624
			Leaving
		}
	}
}
