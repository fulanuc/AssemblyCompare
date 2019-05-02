using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005BB RID: 1467
	public class ChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x0009F5BC File Offset: 0x0009D7BC
		private void Update()
		{
			Color color = this.spriteBaseColor;
			Color color2 = this.textBaseColor;
			if (!this.isCharged)
			{
				if (this.flashWhenNotCharging)
				{
					this.flashStopwatch += Time.deltaTime;
					color = (((int)(this.flashStopwatch * this.flashFrequency) % 2 == 0) ? this.spriteFlashColor : this.spriteBaseColor);
				}
				if (this.isCharging)
				{
					color = this.spriteChargingColor;
					color2 = this.textChargingColor;
				}
				if (this.disableTextWhenNotCharging)
				{
					this.chargingText.enabled = this.isCharging;
				}
				else
				{
					this.chargingText.enabled = true;
				}
			}
			else
			{
				color = this.spriteChargedColor;
				if (this.disableTextWhenCharged)
				{
					this.chargingText.enabled = false;
				}
			}
			SpriteRenderer[] array = this.iconSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
			this.chargingText.color = color2;
		}

		// Token: 0x04002365 RID: 9061
		public SpriteRenderer[] iconSprites;

		// Token: 0x04002366 RID: 9062
		public TextMeshPro chargingText;

		// Token: 0x04002367 RID: 9063
		public Color spriteBaseColor;

		// Token: 0x04002368 RID: 9064
		public Color spriteFlashColor;

		// Token: 0x04002369 RID: 9065
		public Color spriteChargingColor;

		// Token: 0x0400236A RID: 9066
		public Color spriteChargedColor;

		// Token: 0x0400236B RID: 9067
		public Color textBaseColor;

		// Token: 0x0400236C RID: 9068
		public Color textChargingColor;

		// Token: 0x0400236D RID: 9069
		public bool isCharging;

		// Token: 0x0400236E RID: 9070
		public bool isCharged;

		// Token: 0x0400236F RID: 9071
		public bool disableTextWhenNotCharging;

		// Token: 0x04002370 RID: 9072
		public bool disableTextWhenCharged;

		// Token: 0x04002371 RID: 9073
		public bool flashWhenNotCharging;

		// Token: 0x04002372 RID: 9074
		public float flashFrequency;

		// Token: 0x04002373 RID: 9075
		private float flashStopwatch;
	}
}
