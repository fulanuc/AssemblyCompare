using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DF RID: 1503
	public class CustomScrollbar : MPScrollbar
	{
		// Token: 0x060021E5 RID: 8677 RVA: 0x00018B5A File Offset: 0x00016D5A
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x00018B62 File Offset: 0x00016D62
		protected override void Start()
		{
			base.Start();
			this.newPosition = this.originalPosition;
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x000A252C File Offset: 0x000A072C
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			switch (state)
			{
			case Selectable.SelectionState.Normal:
				this.hovering = false;
				break;
			case Selectable.SelectionState.Highlighted:
				Util.PlaySound("Play_UI_menuHover", RoR2Application.instance.gameObject);
				this.hovering = true;
				break;
			case Selectable.SelectionState.Pressed:
				this.hovering = true;
				break;
			case Selectable.SelectionState.Disabled:
				this.hovering = false;
				break;
			}
			this.originalColor = base.targetGraphic.color;
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x00018B29 File Offset: 0x00016D29
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x000A25A4 File Offset: 0x000A07A4
		private void LateUpdate()
		{
			this.stopwatch += Time.deltaTime;
			if (Application.isPlaying)
			{
				if (this.showImageOnHover)
				{
					float target = this.hovering ? 1f : 0f;
					Color color = this.imageOnHover.color;
					float a = Mathf.SmoothDamp(color.a, target, ref this.imageOnHoverAlphaVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					Color color2 = new Color(color.r, color.g, color.g, a);
					this.imageOnHover.color = color2;
				}
				if (this.imageOnInteractable)
				{
					this.imageOnInteractable.enabled = base.interactable;
				}
			}
		}

		// Token: 0x0400242F RID: 9263
		private Vector3 originalPosition;

		// Token: 0x04002430 RID: 9264
		private Vector3 newPosition;

		// Token: 0x04002431 RID: 9265
		private float newButtonScale = 1f;

		// Token: 0x04002432 RID: 9266
		private float stopwatch;

		// Token: 0x04002433 RID: 9267
		private Color originalColor;

		// Token: 0x04002434 RID: 9268
		public bool scaleButtonOnHover = true;

		// Token: 0x04002435 RID: 9269
		public bool showImageOnHover;

		// Token: 0x04002436 RID: 9270
		public Image imageOnHover;

		// Token: 0x04002437 RID: 9271
		public Image imageOnInteractable;

		// Token: 0x04002438 RID: 9272
		private bool hovering;

		// Token: 0x04002439 RID: 9273
		private float imageOnHoverAlphaVelocity;
	}
}
