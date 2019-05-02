using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064A RID: 1610
	[RequireComponent(typeof(EventFunctions))]
	[RequireComponent(typeof(Animator))]
	public class UITransitionController : MonoBehaviour
	{
		// Token: 0x0600241B RID: 9243 RVA: 0x0001A44C File Offset: 0x0001864C
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.PushMecanimTransitionInParameters();
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x000AC060 File Offset: 0x000AA260
		private void PushMecanimTransitionInParameters()
		{
			this.animator.SetFloat("transitionInSpeed", this.transitionInSpeed);
			switch (this.transitionIn)
			{
			case UITransitionController.TransitionStyle.Instant:
				this.animator.SetTrigger("InstantIn");
				return;
			case UITransitionController.TransitionStyle.CanvasGroupAlphaFade:
				this.animator.SetTrigger("CanvasGroupAlphaFadeIn");
				return;
			case UITransitionController.TransitionStyle.SwipeYScale:
				this.animator.SetTrigger("SwipeYScaleIn");
				return;
			case UITransitionController.TransitionStyle.SwipeXScale:
				this.animator.SetTrigger("SwipeXScaleIn");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x000AC0E4 File Offset: 0x000AA2E4
		private void PushMecanimTransitionOutParameters()
		{
			this.animator.SetFloat("transitionOutSpeed", this.transitionOutSpeed);
			switch (this.transitionOut)
			{
			case UITransitionController.TransitionStyle.Instant:
				this.animator.SetTrigger("InstantOut");
				return;
			case UITransitionController.TransitionStyle.CanvasGroupAlphaFade:
				this.animator.SetTrigger("CanvasGroupAlphaFadeOut");
				return;
			case UITransitionController.TransitionStyle.SwipeYScale:
				this.animator.SetTrigger("SwipeYScaleOut");
				return;
			case UITransitionController.TransitionStyle.SwipeXScale:
				this.animator.SetTrigger("SwipeXScaleOut");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x0001A460 File Offset: 0x00018660
		private void Update()
		{
			if (this.transitionOutAtEndOfLifetime && !this.done)
			{
				this.stopwatch += Time.deltaTime;
				if (this.stopwatch >= this.lifetime)
				{
					this.PushMecanimTransitionOutParameters();
					this.done = true;
				}
			}
		}

		// Token: 0x04002701 RID: 9985
		public UITransitionController.TransitionStyle transitionIn;

		// Token: 0x04002702 RID: 9986
		public UITransitionController.TransitionStyle transitionOut;

		// Token: 0x04002703 RID: 9987
		public float transitionInSpeed = 1f;

		// Token: 0x04002704 RID: 9988
		public float transitionOutSpeed = 1f;

		// Token: 0x04002705 RID: 9989
		public bool transitionOutAtEndOfLifetime;

		// Token: 0x04002706 RID: 9990
		public float lifetime;

		// Token: 0x04002707 RID: 9991
		private float stopwatch;

		// Token: 0x04002708 RID: 9992
		private Animator animator;

		// Token: 0x04002709 RID: 9993
		private bool done;

		// Token: 0x0200064B RID: 1611
		public enum TransitionStyle
		{
			// Token: 0x0400270B RID: 9995
			Instant,
			// Token: 0x0400270C RID: 9996
			CanvasGroupAlphaFade,
			// Token: 0x0400270D RID: 9997
			SwipeYScale,
			// Token: 0x0400270E RID: 9998
			SwipeXScale
		}
	}
}
