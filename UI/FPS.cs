using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A1 RID: 1441
	public class FPS : MonoBehaviour
	{
		// Token: 0x0600204F RID: 8271 RVA: 0x0009D9D8 File Offset: 0x0009BBD8
		private void Update()
		{
			this.stopwatch += Time.unscaledDeltaTime;
			this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
			if (this.stopwatch >= 1f)
			{
				this.stopwatch -= 1f;
				float num = this.deltaTime * 1000f;
				float num2 = 1f / this.deltaTime;
				string text = string.Format("{0:0.0} ms \n{1:0.} fps", num, num2);
				this.targetText.text = text;
			}
		}

		// Token: 0x040022DC RID: 8924
		public TextMeshProUGUI targetText;

		// Token: 0x040022DD RID: 8925
		private float deltaTime;

		// Token: 0x040022DE RID: 8926
		private float stopwatch;

		// Token: 0x040022DF RID: 8927
		private const float updateTime = 1f;
	}
}
