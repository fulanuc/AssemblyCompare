using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200065C RID: 1628
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(EventFunctions))]
	public class UITransitionController : MonoBehaviour
	{
		// Token: 0x060024AB RID: 9387 RVA: 0x0001AB1A File Offset: 0x00018D1A
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.PushMecanimTransitionInParameters();
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x000AD6E0 File Offset: 0x000AB8E0
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

		// Token: 0x060024AD RID: 9389 RVA: 0x000AD764 File Offset: 0x000AB964
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

		// Token: 0x060024AE RID: 9390 RVA: 0x0001AB2E File Offset: 0x00018D2E
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

		// Token: 0x0400275C RID: 10076
		public UITransitionController.TransitionStyle transitionIn;

		// Token: 0x0400275D RID: 10077
		public UITransitionController.TransitionStyle transitionOut;

		// Token: 0x0400275E RID: 10078
		public float transitionInSpeed = 1f;

		// Token: 0x0400275F RID: 10079
		public float transitionOutSpeed = 1f;

		// Token: 0x04002760 RID: 10080
		public bool transitionOutAtEndOfLifetime;

		// Token: 0x04002761 RID: 10081
		public float lifetime;

		// Token: 0x04002762 RID: 10082
		private float stopwatch;

		// Token: 0x04002763 RID: 10083
		private Animator animator;

		// Token: 0x04002764 RID: 10084
		private bool done;

		// Token: 0x0200065D RID: 1629
		public enum TransitionStyle
		{
			// Token: 0x04002766 RID: 10086
			Instant,
			// Token: 0x04002767 RID: 10087
			CanvasGroupAlphaFade,
			// Token: 0x04002768 RID: 10088
			SwipeYScale,
			// Token: 0x04002769 RID: 10089
			SwipeXScale
		}
	}
}
