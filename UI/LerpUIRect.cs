using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F3 RID: 1523
	[RequireComponent(typeof(RectTransform))]
	public class LerpUIRect : MonoBehaviour
	{
		// Token: 0x06002230 RID: 8752 RVA: 0x00018E92 File Offset: 0x00017092
		private void Start()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x00018EA0 File Offset: 0x000170A0
		private void OnDisable()
		{
			this.lerpState = LerpUIRect.LerpState.Entering;
			this.stopwatch = 0f;
			this.UpdateLerp();
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x00018EBA File Offset: 0x000170BA
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdateLerp();
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x000A55C8 File Offset: 0x000A37C8
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

		// Token: 0x04002531 RID: 9521
		public Vector3 startLocalPosition;

		// Token: 0x04002532 RID: 9522
		public Vector3 finalLocalPosition;

		// Token: 0x04002533 RID: 9523
		public LerpUIRect.LerpState lerpState;

		// Token: 0x04002534 RID: 9524
		public AnimationCurve enterCurve;

		// Token: 0x04002535 RID: 9525
		public float enterDuration;

		// Token: 0x04002536 RID: 9526
		public AnimationCurve leavingCurve;

		// Token: 0x04002537 RID: 9527
		public float leaveDuration;

		// Token: 0x04002538 RID: 9528
		private float stopwatch;

		// Token: 0x04002539 RID: 9529
		private RectTransform rectTransform;

		// Token: 0x020005F4 RID: 1524
		public enum LerpState
		{
			// Token: 0x0400253B RID: 9531
			Entering,
			// Token: 0x0400253C RID: 9532
			Holding,
			// Token: 0x0400253D RID: 9533
			Leaving
		}
	}
}
