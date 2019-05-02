using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200065B RID: 1627
	public class UIJuice : MonoBehaviour
	{
		// Token: 0x06002499 RID: 9369 RVA: 0x0001AA87 File Offset: 0x00018C87
		private void Awake()
		{
			this.InitializeFirstTimeInfo();
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x0001AA8F File Offset: 0x00018C8F
		private void Update()
		{
			this.transitionStopwatch = Mathf.Min(this.transitionStopwatch + Time.unscaledDeltaTime, this.transitionDuration);
			this.ProcessTransition();
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x000AD1D0 File Offset: 0x000AB3D0
		private void ProcessTransition()
		{
			this.InitializeFirstTimeInfo();
			if (this.transitionStopwatch < this.transitionDuration)
			{
				AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, this.transitionStartAlpha, 1f, this.transitionEndAlpha);
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = animationCurve.Evaluate(this.transitionStopwatch / this.transitionDuration);
				}
				AnimationCurve animationCurve2 = new AnimationCurve();
				Keyframe key = new Keyframe(0f, 0f, 3f, 3f);
				Keyframe key2 = new Keyframe(1f, 1f, 0f, 0f);
				animationCurve2.AddKey(key);
				animationCurve2.AddKey(key2);
				Vector2 anchoredPosition = Vector2.Lerp(this.transitionStartPosition, this.transitionEndPosition, animationCurve2.Evaluate(this.transitionStopwatch / this.transitionDuration));
				Vector2 sizeDelta = Vector2.Lerp(this.transitionStartSize, this.transitionEndSize, animationCurve2.Evaluate(this.transitionStopwatch / this.transitionDuration));
				if (this.panningRect)
				{
					this.panningRect.anchoredPosition = anchoredPosition;
					this.panningRect.sizeDelta = sizeDelta;
					return;
				}
			}
			else
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.transitionEndAlpha;
				}
				if (this.panningRect)
				{
					this.panningRect.anchoredPosition = this.transitionEndPosition;
					this.panningRect.sizeDelta = this.transitionEndSize;
				}
				if (this.destroyOnEndOfTransition)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x000AD360 File Offset: 0x000AB560
		public void TransitionScaleUpWidth()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartSize = new Vector2(0f, this.transitionEndSize.y * 0.8f);
				this.transitionEndSize = this.originalSize;
			}
			this.BeginTransition();
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000AD3B8 File Offset: 0x000AB5B8
		public void TransitionPanFromLeft()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(-1f, 0f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x000AD40C File Offset: 0x000AB60C
		public void TransitionPanToLeft()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(-1f, 0f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x000AD460 File Offset: 0x000AB660
		public void TransitionPanFromRight()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(1f, 0f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x000AD4B4 File Offset: 0x000AB6B4
		public void TransitionPanToRight()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(1f, 0f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x000AD508 File Offset: 0x000AB708
		public void TransitionPanFromTop()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(0f, 1f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000AD55C File Offset: 0x000AB75C
		public void TransitionPanToTop()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(0f, 1f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000AD5B0 File Offset: 0x000AB7B0
		public void TransitionPanFromBottom()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(0f, -1f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000AD604 File Offset: 0x000AB804
		public void TransitionPanToBottom()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(0f, -1f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x0001AAB4 File Offset: 0x00018CB4
		public void TransitionAlphaFadeIn()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = 0f;
			this.transitionEndAlpha = this.originalAlpha;
			this.BeginTransition();
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x0001AAD9 File Offset: 0x00018CD9
		public void TransitionAlphaFadeOut()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = this.originalAlpha;
			this.transitionEndAlpha = 0f;
			this.BeginTransition();
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x0001AAFE File Offset: 0x00018CFE
		public void DestroyOnEndOfTransition(bool set)
		{
			this.destroyOnEndOfTransition = set;
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x0001AB07 File Offset: 0x00018D07
		private void BeginTransition()
		{
			this.transitionStopwatch = 0f;
			this.ProcessTransition();
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x000AD658 File Offset: 0x000AB858
		private void InitializeFirstTimeInfo()
		{
			if (this.hasInitialized)
			{
				return;
			}
			if (this.panningRect)
			{
				this.originalPosition = this.panningRect.anchoredPosition;
				this.originalSize = this.panningRect.sizeDelta;
			}
			if (this.canvasGroup)
			{
				this.originalAlpha = this.canvasGroup.alpha;
				this.transitionEndAlpha = this.originalAlpha;
				this.transitionStartAlpha = this.originalAlpha;
			}
			this.hasInitialized = true;
		}

		// Token: 0x0400274C RID: 10060
		[Header("Transition Settings")]
		public CanvasGroup canvasGroup;

		// Token: 0x0400274D RID: 10061
		public RectTransform panningRect;

		// Token: 0x0400274E RID: 10062
		public float transitionDuration;

		// Token: 0x0400274F RID: 10063
		public float panningMagnitude;

		// Token: 0x04002750 RID: 10064
		public bool destroyOnEndOfTransition;

		// Token: 0x04002751 RID: 10065
		private float transitionStopwatch;

		// Token: 0x04002752 RID: 10066
		private float transitionEndAlpha;

		// Token: 0x04002753 RID: 10067
		private float transitionStartAlpha;

		// Token: 0x04002754 RID: 10068
		private float originalAlpha;

		// Token: 0x04002755 RID: 10069
		private Vector2 transitionStartPosition;

		// Token: 0x04002756 RID: 10070
		private Vector2 transitionEndPosition;

		// Token: 0x04002757 RID: 10071
		private Vector2 originalPosition;

		// Token: 0x04002758 RID: 10072
		private Vector2 transitionStartSize;

		// Token: 0x04002759 RID: 10073
		private Vector2 transitionEndSize;

		// Token: 0x0400275A RID: 10074
		private Vector3 originalSize;

		// Token: 0x0400275B RID: 10075
		private bool hasInitialized;
	}
}
