using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000649 RID: 1609
	public class UIJuice : MonoBehaviour
	{
		// Token: 0x06002409 RID: 9225 RVA: 0x0001A3B9 File Offset: 0x000185B9
		private void Awake()
		{
			this.InitializeFirstTimeInfo();
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x0001A3C1 File Offset: 0x000185C1
		private void Update()
		{
			this.transitionStopwatch = Mathf.Min(this.transitionStopwatch + Time.unscaledDeltaTime, this.transitionDuration);
			this.ProcessTransition();
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x000ABB50 File Offset: 0x000A9D50
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

		// Token: 0x0600240C RID: 9228 RVA: 0x000ABCE0 File Offset: 0x000A9EE0
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

		// Token: 0x0600240D RID: 9229 RVA: 0x000ABD38 File Offset: 0x000A9F38
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

		// Token: 0x0600240E RID: 9230 RVA: 0x000ABD8C File Offset: 0x000A9F8C
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

		// Token: 0x0600240F RID: 9231 RVA: 0x000ABDE0 File Offset: 0x000A9FE0
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

		// Token: 0x06002410 RID: 9232 RVA: 0x000ABE34 File Offset: 0x000AA034
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

		// Token: 0x06002411 RID: 9233 RVA: 0x000ABE88 File Offset: 0x000AA088
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

		// Token: 0x06002412 RID: 9234 RVA: 0x000ABEDC File Offset: 0x000AA0DC
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

		// Token: 0x06002413 RID: 9235 RVA: 0x000ABF30 File Offset: 0x000AA130
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

		// Token: 0x06002414 RID: 9236 RVA: 0x000ABF84 File Offset: 0x000AA184
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

		// Token: 0x06002415 RID: 9237 RVA: 0x0001A3E6 File Offset: 0x000185E6
		public void TransitionAlphaFadeIn()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = 0f;
			this.transitionEndAlpha = this.originalAlpha;
			this.BeginTransition();
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x0001A40B File Offset: 0x0001860B
		public void TransitionAlphaFadeOut()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = this.originalAlpha;
			this.transitionEndAlpha = 0f;
			this.BeginTransition();
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x0001A430 File Offset: 0x00018630
		public void DestroyOnEndOfTransition(bool set)
		{
			this.destroyOnEndOfTransition = set;
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x0001A439 File Offset: 0x00018639
		private void BeginTransition()
		{
			this.transitionStopwatch = 0f;
			this.ProcessTransition();
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x000ABFD8 File Offset: 0x000AA1D8
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

		// Token: 0x040026F1 RID: 9969
		[Header("Transition Settings")]
		public CanvasGroup canvasGroup;

		// Token: 0x040026F2 RID: 9970
		public RectTransform panningRect;

		// Token: 0x040026F3 RID: 9971
		public float transitionDuration;

		// Token: 0x040026F4 RID: 9972
		public float panningMagnitude;

		// Token: 0x040026F5 RID: 9973
		public bool destroyOnEndOfTransition;

		// Token: 0x040026F6 RID: 9974
		private float transitionStopwatch;

		// Token: 0x040026F7 RID: 9975
		private float transitionEndAlpha;

		// Token: 0x040026F8 RID: 9976
		private float transitionStartAlpha;

		// Token: 0x040026F9 RID: 9977
		private float originalAlpha;

		// Token: 0x040026FA RID: 9978
		private Vector2 transitionStartPosition;

		// Token: 0x040026FB RID: 9979
		private Vector2 transitionEndPosition;

		// Token: 0x040026FC RID: 9980
		private Vector2 originalPosition;

		// Token: 0x040026FD RID: 9981
		private Vector2 transitionStartSize;

		// Token: 0x040026FE RID: 9982
		private Vector2 transitionEndSize;

		// Token: 0x040026FF RID: 9983
		private Vector3 originalSize;

		// Token: 0x04002700 RID: 9984
		private bool hasInitialized;
	}
}
