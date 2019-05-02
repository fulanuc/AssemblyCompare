using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C3 RID: 1475
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(HudElement))]
	public class CrosshairController : MonoBehaviour
	{
		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600212D RID: 8493 RVA: 0x000182E9 File Offset: 0x000164E9
		// (set) Token: 0x0600212E RID: 8494 RVA: 0x000182F1 File Offset: 0x000164F1
		public RectTransform rectTransform { get; private set; }

		// Token: 0x0600212F RID: 8495 RVA: 0x000182FA File Offset: 0x000164FA
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.hudElement = base.GetComponent<HudElement>();
			this.SetCrosshairSpread();
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x000A0648 File Offset: 0x0009E848
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

		// Token: 0x06002131 RID: 8497 RVA: 0x0001831A File Offset: 0x0001651A
		private void LateUpdate()
		{
			this.SetCrosshairSpread();
		}

		// Token: 0x040023AA RID: 9130
		private HudElement hudElement;

		// Token: 0x040023AB RID: 9131
		public CrosshairController.SpritePosition[] spriteSpreadPositions;

		// Token: 0x040023AC RID: 9132
		public RawImage[] remapSprites;

		// Token: 0x040023AD RID: 9133
		public float minSpreadAlpha;

		// Token: 0x040023AE RID: 9134
		public float maxSpreadAlpha;

		// Token: 0x040023AF RID: 9135
		[Tooltip("The angle the crosshair represents when alpha = 1")]
		public float maxSpreadAngle;

		// Token: 0x040023B0 RID: 9136
		private MaterialPropertyBlock _propBlock;

		// Token: 0x020005C4 RID: 1476
		[Serializable]
		public struct SpritePosition
		{
			// Token: 0x040023B1 RID: 9137
			public RectTransform target;

			// Token: 0x040023B2 RID: 9138
			public Vector3 zeroPosition;

			// Token: 0x040023B3 RID: 9139
			public Vector3 onePosition;
		}
	}
}
