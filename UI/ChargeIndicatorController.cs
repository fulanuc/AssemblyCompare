using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005CD RID: 1485
	public class ChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x06002173 RID: 8563 RVA: 0x000A0B90 File Offset: 0x0009ED90
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

		// Token: 0x040023B9 RID: 9145
		public SpriteRenderer[] iconSprites;

		// Token: 0x040023BA RID: 9146
		public TextMeshPro chargingText;

		// Token: 0x040023BB RID: 9147
		public Color spriteBaseColor;

		// Token: 0x040023BC RID: 9148
		public Color spriteFlashColor;

		// Token: 0x040023BD RID: 9149
		public Color spriteChargingColor;

		// Token: 0x040023BE RID: 9150
		public Color spriteChargedColor;

		// Token: 0x040023BF RID: 9151
		public Color textBaseColor;

		// Token: 0x040023C0 RID: 9152
		public Color textChargingColor;

		// Token: 0x040023C1 RID: 9153
		public bool isCharging;

		// Token: 0x040023C2 RID: 9154
		public bool isCharged;

		// Token: 0x040023C3 RID: 9155
		public bool disableTextWhenNotCharging;

		// Token: 0x040023C4 RID: 9156
		public bool disableTextWhenCharged;

		// Token: 0x040023C5 RID: 9157
		public bool flashWhenNotCharging;

		// Token: 0x040023C6 RID: 9158
		public float flashFrequency;

		// Token: 0x040023C7 RID: 9159
		private float flashStopwatch;
	}
}
