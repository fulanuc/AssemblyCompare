using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200064D RID: 1613
	[RequireComponent(typeof(Image))]
	public class StageFadeTransitionController : MonoBehaviour
	{
		// Token: 0x06002451 RID: 9297 RVA: 0x000AC000 File Offset: 0x000AA200
		private void Awake()
		{
			this.fadeImage = base.GetComponent<Image>();
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x000AC040 File Offset: 0x000AA240
		private void Start()
		{
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
			this.fadeImage.CrossFadeColor(Color.black, 0.5f, false, true);
			this.startEngineTime = Time.time;
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x000AC094 File Offset: 0x000AA294
		private void Update()
		{
			if (Stage.instance)
			{
				float stageAdvanceTime = Stage.instance.stageAdvanceTime;
				float num = Time.time - this.startEngineTime;
				float a = 0f;
				float b = 0f;
				if (num < 0.5f)
				{
					a = 1f - Mathf.Clamp01((Time.time - this.startEngineTime) / 0.5f);
				}
				if (!float.IsInfinity(stageAdvanceTime))
				{
					float num2 = Stage.instance.stageAdvanceTime - 0.25f - Run.instance.fixedTime;
					b = 1f - Mathf.Clamp01(num2 / 0.5f);
				}
				Color color = this.fadeImage.color;
				color.a = Mathf.Max(a, b);
				this.fadeImage.color = color;
			}
		}

		// Token: 0x040026FF RID: 9983
		private Image fadeImage;

		// Token: 0x04002700 RID: 9984
		private float startEngineTime;

		// Token: 0x04002701 RID: 9985
		private const float transitionDuration = 0.5f;
	}
}
