using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FE RID: 1534
	[RequireComponent(typeof(RectTransform))]
	public class MoneyText : MonoBehaviour
	{
		// Token: 0x0600227F RID: 8831 RVA: 0x000A6DAC File Offset: 0x000A4FAC
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

		// Token: 0x0400258E RID: 9614
		public TextMeshProUGUI targetText;

		// Token: 0x0400258F RID: 9615
		public FlashPanel flashPanel;

		// Token: 0x04002590 RID: 9616
		private int displayAmount;

		// Token: 0x04002591 RID: 9617
		private float updateTimer;

		// Token: 0x04002592 RID: 9618
		private float coinSoundCooldown;

		// Token: 0x04002593 RID: 9619
		public int targetValue;

		// Token: 0x04002594 RID: 9620
		public string sound = "Play_UI_coin";
	}
}
