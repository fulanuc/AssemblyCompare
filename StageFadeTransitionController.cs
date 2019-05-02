using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063B RID: 1595
	[RequireComponent(typeof(Image))]
	public class StageFadeTransitionController : MonoBehaviour
	{
		// Token: 0x060023C1 RID: 9153 RVA: 0x000AA984 File Offset: 0x000A8B84
		private void Awake()
		{
			this.fadeImage = base.GetComponent<Image>();
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
		}

		// Token: 0x060023C2 RID: 9154 RVA: 0x000AA9C4 File Offset: 0x000A8BC4
		private void Start()
		{
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
			this.fadeImage.CrossFadeColor(Color.black, 0.5f, false, true);
			this.startEngineTime = Time.time;
		}

		// Token: 0x060023C3 RID: 9155 RVA: 0x000AAA18 File Offset: 0x000A8C18
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

		// Token: 0x040026A4 RID: 9892
		private Image fadeImage;

		// Token: 0x040026A5 RID: 9893
		private float startEngineTime;

		// Token: 0x040026A6 RID: 9894
		private const float transitionDuration = 0.5f;
	}
}
