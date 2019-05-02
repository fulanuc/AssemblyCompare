using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B3 RID: 1459
	public class FPS : MonoBehaviour
	{
		// Token: 0x060020E0 RID: 8416 RVA: 0x0009EFAC File Offset: 0x0009D1AC
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

		// Token: 0x04002330 RID: 9008
		public TextMeshProUGUI targetText;

		// Token: 0x04002331 RID: 9009
		private float deltaTime;

		// Token: 0x04002332 RID: 9010
		private float stopwatch;

		// Token: 0x04002333 RID: 9011
		private const float updateTime = 1f;
	}
}
