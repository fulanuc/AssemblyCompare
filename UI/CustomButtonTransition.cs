using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DE RID: 1502
	public class CustomButtonTransition : MPButton
	{
		// Token: 0x060021DF RID: 8671 RVA: 0x00018B15 File Offset: 0x00016D15
		protected override void Awake()
		{
			base.Awake();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x000A2200 File Offset: 0x000A0400
		protected override void Start()
		{
			base.Start();
			base.onClick.AddListener(new UnityAction(this.OnClickCustom));
			if (this.textMeshProUGui)
			{
				this.originalPosition = new Vector3(this.textMeshProUGui.margin.x, 0f, 0f);
			}
			this.newPosition = this.originalPosition;
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x000A2268 File Offset: 0x000A0468
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			switch (state)
			{
			case Selectable.SelectionState.Normal:
				this.newPosition.x = this.originalPosition.x;
				this.newButtonScale = 1f;
				this.hovering = false;
				break;
			case Selectable.SelectionState.Highlighted:
				this.newPosition.x = this.originalPosition.x + 4f;
				Util.PlaySound("Play_UI_menuHover", RoR2Application.instance.gameObject);
				this.newButtonScale = 1.05f;
				this.hovering = true;
				break;
			case Selectable.SelectionState.Pressed:
				this.newPosition.x = this.originalPosition.x + 6f;
				this.newButtonScale = 0.95f;
				this.hovering = true;
				break;
			case Selectable.SelectionState.Disabled:
				this.newPosition.x = this.originalPosition.x;
				this.newButtonScale = 1f;
				this.hovering = false;
				break;
			}
			this.originalColor = base.targetGraphic.color;
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x00018B29 File Offset: 0x00016D29
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x000A2374 File Offset: 0x000A0574
		private void LateUpdate()
		{
			this.stopwatch += Time.deltaTime;
			if (Application.isPlaying)
			{
				if (this.textMeshProUGui)
				{
					Vector3 position = this.textMeshProUGui.transform.position;
					this.textMeshProUGui.margin = this.newPosition;
				}
				if (base.image && this.scaleButtonOnHover)
				{
					float num = Mathf.SmoothDamp(base.image.transform.localScale.x, this.newButtonScale, ref this.buttonScaleVelocity, base.colors.fadeDuration * 0.2f, Time.unscaledDeltaTime);
					base.image.transform.localScale = new Vector3(num, num, num);
				}
				if (this.showImageOnHover)
				{
					float target = this.hovering ? 1f : 0f;
					float target2 = this.hovering ? 1f : 1.1f;
					Color color = this.imageOnHover.color;
					float x = this.imageOnHover.transform.localScale.x;
					float a = Mathf.SmoothDamp(color.a, target, ref this.imageOnHoverAlphaVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					float num2 = Mathf.SmoothDamp(x, target2, ref this.imageOnHoverScaleVelocity, 0.03f, 100f, Time.unscaledDeltaTime);
					Color color2 = new Color(color.r, color.g, color.g, a);
					new Vector3(num2, num2, num2);
					this.imageOnHover.color = color2;
				}
				if (this.imageOnInteractable)
				{
					this.imageOnInteractable.enabled = base.interactable;
				}
			}
		}

		// Token: 0x04002421 RID: 9249
		private TextMeshProUGUI textMeshProUGui;

		// Token: 0x04002422 RID: 9250
		private Vector3 originalPosition;

		// Token: 0x04002423 RID: 9251
		private Vector3 newPosition;

		// Token: 0x04002424 RID: 9252
		private float newButtonScale = 1f;

		// Token: 0x04002425 RID: 9253
		private float stopwatch;

		// Token: 0x04002426 RID: 9254
		private Color originalColor;

		// Token: 0x04002427 RID: 9255
		public bool scaleButtonOnHover = true;

		// Token: 0x04002428 RID: 9256
		public bool showImageOnHover;

		// Token: 0x04002429 RID: 9257
		public Image imageOnHover;

		// Token: 0x0400242A RID: 9258
		public Image imageOnInteractable;

		// Token: 0x0400242B RID: 9259
		private bool hovering;

		// Token: 0x0400242C RID: 9260
		private float buttonScaleVelocity;

		// Token: 0x0400242D RID: 9261
		private float imageOnHoverAlphaVelocity;

		// Token: 0x0400242E RID: 9262
		private float imageOnHoverScaleVelocity;
	}
}
