using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D5 RID: 1493
	[RequireComponent(typeof(HudElement))]
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairController : MonoBehaviour
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060021BE RID: 8638 RVA: 0x000189E3 File Offset: 0x00016BE3
		// (set) Token: 0x060021BF RID: 8639 RVA: 0x000189EB File Offset: 0x00016BEB
		public RectTransform rectTransform { get; private set; }

		// Token: 0x060021C0 RID: 8640 RVA: 0x000189F4 File Offset: 0x00016BF4
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.hudElement = base.GetComponent<HudElement>();
			this.SetCrosshairSpread();
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x000A1C1C File Offset: 0x0009FE1C
		private void SetCrosshairSpread()
		{
			float num = 0f;
			if (this.hudElement.targetCharacterBody)
			{
				num = this.hudElement.targetCharacterBody.spreadBloomAngle;
			}
			for (int i = 0; i < this.spriteSpreadPositions.Length; i++)
			{
				CrosshairController.SpritePosition spritePosition = this.spriteSpreadPositions[i];
				spritePosition.target.localPosition = Vector3.Lerp(spritePosition.zeroPosition, spritePosition.onePosition, num / this.maxSpreadAngle);
			}
			for (int j = 0; j < this.remapSprites.Length; j++)
			{
				this.remapSprites[j].color = new Color(1f, 1f, 1f, Util.Remap(num / this.maxSpreadAngle, 0f, 1f, this.minSpreadAlpha, this.maxSpreadAlpha));
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x00018A14 File Offset: 0x00016C14
		private void LateUpdate()
		{
			this.SetCrosshairSpread();
		}

		// Token: 0x040023FE RID: 9214
		private HudElement hudElement;

		// Token: 0x040023FF RID: 9215
		public CrosshairController.SpritePosition[] spriteSpreadPositions;

		// Token: 0x04002400 RID: 9216
		public RawImage[] remapSprites;

		// Token: 0x04002401 RID: 9217
		public float minSpreadAlpha;

		// Token: 0x04002402 RID: 9218
		public float maxSpreadAlpha;

		// Token: 0x04002403 RID: 9219
		[Tooltip("The angle the crosshair represents when alpha = 1")]
		public float maxSpreadAngle;

		// Token: 0x04002404 RID: 9220
		private MaterialPropertyBlock _propBlock;

		// Token: 0x020005D6 RID: 1494
		[Serializable]
		public struct SpritePosition
		{
			// Token: 0x04002405 RID: 9221
			public RectTransform target;

			// Token: 0x04002406 RID: 9222
			public Vector3 zeroPosition;

			// Token: 0x04002407 RID: 9223
			public Vector3 onePosition;
		}
	}
}
