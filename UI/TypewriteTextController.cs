using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200065A RID: 1626
	public class TypewriteTextController : MonoBehaviour
	{
		// Token: 0x06002494 RID: 9364 RVA: 0x000ACF20 File Offset: 0x000AB120
		private void SetupTypewriter()
		{
			this.totalStopwatch = -this.initialDelay;
			this.keyStopwatch = -this.initialDelay;
			this.textArrayIndex = 0;
			this.initialTextStrings = new string[this.textMeshProUGui.Length];
			this.keyCount = new int[this.textMeshProUGui.Length];
			for (int i = 0; i < this.textMeshProUGui.Length; i++)
			{
				this.initialTextStrings[i] = this.textMeshProUGui[i].text;
			}
			this.Update();
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x0001AA55 File Offset: 0x00018C55
		private void Start()
		{
			this.SetupTypewriter();
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x000ACFA4 File Offset: 0x000AB1A4
		private void Update()
		{
			if (this.done)
			{
				if (this.fadeOutAfterCompletion)
				{
					this.fadeOutStopwatch += Time.deltaTime;
					float t = (this.fadeOutStopwatch - this.fadeOutDelay) / this.fadeOutDuration;
					for (int i = 0; i < this.textMeshProUGui.Length; i++)
					{
						Color color = this.textMeshProUGui[i].color;
						this.textMeshProUGui[i].color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, t));
					}
					if (this.fadeOutStopwatch >= this.fadeOutDelay + this.fadeOutDuration)
					{
						base.gameObject.SetActive(false);
					}
				}
				return;
			}
			this.totalStopwatch += Time.deltaTime;
			this.keyStopwatch += Time.deltaTime;
			if (this.keyStopwatch >= this.delayBetweenKeys)
			{
				this.keyStopwatch -= this.delayBetweenKeys;
				this.keyCount[this.textArrayIndex]++;
				if (this.soundString.Length > 0)
				{
					Util.PlaySound(this.soundString, RoR2Application.instance.gameObject);
				}
				if (this.keyCount[this.textArrayIndex] > this.initialTextStrings[this.textArrayIndex].Length)
				{
					if (this.textArrayIndex + 1 == this.textMeshProUGui.Length)
					{
						this.done = true;
					}
					else
					{
						this.textArrayIndex++;
						this.keyCount[this.textArrayIndex] = 0;
						this.keyStopwatch -= this.delayBetweenTexts;
					}
				}
			}
			for (int j = 0; j < this.textMeshProUGui.Length; j++)
			{
				if (this.keyCount[j] <= 0)
				{
					this.textMeshProUGui[j].text = "";
				}
				else
				{
					TypewriteTextController.sharedStringBuilder.Clear();
					TypewriteTextController.sharedStringBuilder.Append(this.initialTextStrings[j], 0, Mathf.Min(this.keyCount[j], this.initialTextStrings[j].Length));
					this.textMeshProUGui[j].SetText(TypewriteTextController.sharedStringBuilder);
				}
			}
		}

		// Token: 0x0400273C RID: 10044
		public float initialDelay;

		// Token: 0x0400273D RID: 10045
		public float delayBetweenKeys = 0.1f;

		// Token: 0x0400273E RID: 10046
		public float delayBetweenTexts = 1f;

		// Token: 0x0400273F RID: 10047
		public TextMeshProUGUI[] textMeshProUGui;

		// Token: 0x04002740 RID: 10048
		public string soundString;

		// Token: 0x04002741 RID: 10049
		public bool fadeOutAfterCompletion;

		// Token: 0x04002742 RID: 10050
		public float fadeOutDelay;

		// Token: 0x04002743 RID: 10051
		public float fadeOutDuration;

		// Token: 0x04002744 RID: 10052
		private string[] initialTextStrings;

		// Token: 0x04002745 RID: 10053
		private float totalStopwatch;

		// Token: 0x04002746 RID: 10054
		private float keyStopwatch;

		// Token: 0x04002747 RID: 10055
		private float fadeOutStopwatch;

		// Token: 0x04002748 RID: 10056
		private int[] keyCount;

		// Token: 0x04002749 RID: 10057
		private int textArrayIndex;

		// Token: 0x0400274A RID: 10058
		private bool done;

		// Token: 0x0400274B RID: 10059
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
