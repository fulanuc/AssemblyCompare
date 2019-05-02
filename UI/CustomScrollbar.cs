using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CD RID: 1485
	public class CustomScrollbar : MPScrollbar
	{
		// Token: 0x06002154 RID: 8532 RVA: 0x00018460 File Offset: 0x00016660
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x00018468 File Offset: 0x00016668
		protected override void Start()
		{
			base.Start();
			this.newPosition = this.originalPosition;
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x000A0F58 File Offset: 0x0009F158
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

		// Token: 0x06002157 RID: 8535 RVA: 0x0001842F File Offset: 0x0001662F
		public void OnClickCustom()
		{
			Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x000A0FD0 File Offset: 0x0009F1D0
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

		// Token: 0x040023DB RID: 9179
		private Vector3 originalPosition;

		// Token: 0x040023DC RID: 9180
		private Vector3 newPosition;

		// Token: 0x040023DD RID: 9181
		private float newButtonScale = 1f;

		// Token: 0x040023DE RID: 9182
		private float stopwatch;

		// Token: 0x040023DF RID: 9183
		private Color originalColor;

		// Token: 0x040023E0 RID: 9184
		public bool scaleButtonOnHover = true;

		// Token: 0x040023E1 RID: 9185
		public bool showImageOnHover;

		// Token: 0x040023E2 RID: 9186
		public Image imageOnHover;

		// Token: 0x040023E3 RID: 9187
		public Image imageOnInteractable;

		// Token: 0x040023E4 RID: 9188
		private bool hovering;

		// Token: 0x040023E5 RID: 9189
		private float imageOnHoverAlphaVelocity;
	}
}
