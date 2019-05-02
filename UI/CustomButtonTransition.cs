using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CC RID: 1484
	public class CustomButtonTransition : MPButton
	{
		// Token: 0x0600214E RID: 8526 RVA: 0x0001841B File Offset: 0x0001661B
		protected override void Awake()
		{
			base.Awake();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x000A0C2C File Offset: 0x0009EE2C
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

		// Token: 0x06002150 RID: 8528 RVA: 0x000A0C94 File Offset: 0x0009EE94
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

		// Token: 0x06002151 RID: 8529 RVA: 0x0001842F File Offset: 0x0001662F
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000A0DA0 File Offset: 0x0009EFA0
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

		// Token: 0x040023CD RID: 9165
		private TextMeshProUGUI textMeshProUGui;

		// Token: 0x040023CE RID: 9166
		private Vector3 originalPosition;

		// Token: 0x040023CF RID: 9167
		private Vector3 newPosition;

		// Token: 0x040023D0 RID: 9168
		private float newButtonScale = 1f;

		// Token: 0x040023D1 RID: 9169
		private float stopwatch;

		// Token: 0x040023D2 RID: 9170
		private Color originalColor;

		// Token: 0x040023D3 RID: 9171
		public bool scaleButtonOnHover = true;

		// Token: 0x040023D4 RID: 9172
		public bool showImageOnHover;

		// Token: 0x040023D5 RID: 9173
		public Image imageOnHover;

		// Token: 0x040023D6 RID: 9174
		public Image imageOnInteractable;

		// Token: 0x040023D7 RID: 9175
		private bool hovering;

		// Token: 0x040023D8 RID: 9176
		private float buttonScaleVelocity;

		// Token: 0x040023D9 RID: 9177
		private float imageOnHoverAlphaVelocity;

		// Token: 0x040023DA RID: 9178
		private float imageOnHoverScaleVelocity;
	}
}
