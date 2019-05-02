using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000249 RID: 585
	public static class FadeToBlackManager
	{
		// Token: 0x06000AFC RID: 2812 RVA: 0x0004A8CC File Offset: 0x00048ACC
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ScreenTintCanvas"), RoR2Application.instance.mainCanvas.transform);
			FadeToBlackManager.alpha = 0f;
			FadeToBlackManager.image = gameObject.transform.GetChild(0).GetComponent<Image>();
			FadeToBlackManager.UpdateImageAlpha();
			RoR2Application.onUpdate += FadeToBlackManager.Update;
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000AFD RID: 2813 RVA: 0x00008DAD File Offset: 0x00006FAD
		public static bool fullyFaded
		{
			get
			{
				return FadeToBlackManager.alpha == 2f;
			}
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x00008DBB File Offset: 0x00006FBB
		private static void Update()
		{
			FadeToBlackManager.alpha = Mathf.MoveTowards(FadeToBlackManager.alpha, (FadeToBlackManager.fadeCount > 0) ? 2f : 0f, Time.unscaledDeltaTime * 4f);
			FadeToBlackManager.UpdateImageAlpha();
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x0004A92C File Offset: 0x00048B2C
		private static void UpdateImageAlpha()
		{
			Color color = FadeToBlackManager.image.color;
			color.a = FadeToBlackManager.alpha;
			FadeToBlackManager.image.color = color;
		}

		// Token: 0x04000EEC RID: 3820
		private static Image image;

		// Token: 0x04000EED RID: 3821
		public static int fadeCount;

		// Token: 0x04000EEE RID: 3822
		private static float alpha;

		// Token: 0x04000EEF RID: 3823
		private const float fadeDuration = 0.25f;

		// Token: 0x04000EF0 RID: 3824
		private const float inversefadeDuration = 4f;
	}
}
