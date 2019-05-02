using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000610 RID: 1552
	[RequireComponent(typeof(RectTransform))]
	public class MoneyText : MonoBehaviour
	{
		// Token: 0x0600230F RID: 8975 RVA: 0x000A8428 File Offset: 0x000A6628
		public void Update()
		{
			this.coinSoundCooldown -= Time.deltaTime;
			if (this.targetText)
			{
				if (this.updateTimer <= 0f)
				{
					int num = 0;
					if (this.displayAmount != this.targetValue)
					{
						int num2;
						num = Math.DivRem(this.targetValue - this.displayAmount, 3, out num2);
						if (num2 != 0)
						{
							num += Math.Sign(num2);
						}
						if (num > 0)
						{
							if (this.coinSoundCooldown <= 0f)
							{
								this.coinSoundCooldown = 0.025f;
								Util.PlaySound(this.sound, RoR2Application.instance.gameObject);
							}
							if (this.flashPanel)
							{
								this.flashPanel.Flash();
							}
						}
						this.displayAmount += num;
					}
					float num3 = Mathf.Min(0.5f / (float)num, 0.25f);
					this.targetText.text = this.displayAmount.ToString();
					this.updateTimer = num3;
				}
				this.updateTimer -= Time.deltaTime;
			}
		}

		// Token: 0x040025E9 RID: 9705
		public TextMeshProUGUI targetText;

		// Token: 0x040025EA RID: 9706
		public FlashPanel flashPanel;

		// Token: 0x040025EB RID: 9707
		private int displayAmount;

		// Token: 0x040025EC RID: 9708
		private float updateTimer;

		// Token: 0x040025ED RID: 9709
		private float coinSoundCooldown;

		// Token: 0x040025EE RID: 9710
		public int targetValue;

		// Token: 0x040025EF RID: 9711
		public string sound = "Play_UI_coin";
	}
}
