using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000249 RID: 585
	public static class FadeToBlackManager
	{
		// Token: 0x06000AF9 RID: 2809 RVA: 0x0004A6C8 File Offset: 0x000488C8
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ScreenTintCanvas"));
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			FadeToBlackManager.alpha = 0f;
			FadeToBlackManager.image = gameObject.transform.GetChild(0).GetComponent<Image>();
			FadeToBlackManager.UpdateImageAlpha();
			RoR2Application.onUpdate += FadeToBlackManager.Update;
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000AFA RID: 2810 RVA: 0x00008D88 File Offset: 0x00006F88
		public static bool fullyFaded
		{
			get
			{
				return FadeToBlackManager.alpha == 2f;
			}
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00008D96 File Offset: 0x00006F96
		private static void Update()
		{
			FadeToBlackManager.alpha = Mathf.MoveTowards(FadeToBlackManager.alpha, (FadeToBlackManager.fadeCount > 0) ? 2f : 0f, Time.deltaTime * 4f);
			FadeToBlackManager.UpdateImageAlpha();
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0004A720 File Offset: 0x00048920
		private static void UpdateImageAlpha()
		{
			Color color = FadeToBlackManager.image.color;
			color.a = FadeToBlackManager.alpha;
			FadeToBlackManager.image.color = color;
		}

		// Token: 0x04000EE6 RID: 3814
		private static Image image;

		// Token: 0x04000EE7 RID: 3815
		public static int fadeCount;

		// Token: 0x04000EE8 RID: 3816
		private static float alpha;

		// Token: 0x04000EE9 RID: 3817
		private const float fadeDuration = 0.25f;

		// Token: 0x04000EEA RID: 3818
		private const float inversefadeDuration = 4f;
	}
}
