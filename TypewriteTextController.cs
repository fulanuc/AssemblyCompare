using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000648 RID: 1608
	public class TypewriteTextController : MonoBehaviour
	{
		// Token: 0x06002404 RID: 9220 RVA: 0x000AB8A0 File Offset: 0x000A9AA0
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

		// Token: 0x06002405 RID: 9221 RVA: 0x0001A387 File Offset: 0x00018587
		private void Start()
		{
			this.SetupTypewriter();
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x000AB924 File Offset: 0x000A9B24
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

		// Token: 0x040026E1 RID: 9953
		public float initialDelay;

		// Token: 0x040026E2 RID: 9954
		public float delayBetweenKeys = 0.1f;

		// Token: 0x040026E3 RID: 9955
		public float delayBetweenTexts = 1f;

		// Token: 0x040026E4 RID: 9956
		public TextMeshProUGUI[] textMeshProUGui;

		// Token: 0x040026E5 RID: 9957
		public string soundString;

		// Token: 0x040026E6 RID: 9958
		public bool fadeOutAfterCompletion;

		// Token: 0x040026E7 RID: 9959
		public float fadeOutDelay;

		// Token: 0x040026E8 RID: 9960
		public float fadeOutDuration;

		// Token: 0x040026E9 RID: 9961
		private string[] initialTextStrings;

		// Token: 0x040026EA RID: 9962
		private float totalStopwatch;

		// Token: 0x040026EB RID: 9963
		private float keyStopwatch;

		// Token: 0x040026EC RID: 9964
		private float fadeOutStopwatch;

		// Token: 0x040026ED RID: 9965
		private int[] keyCount;

		// Token: 0x040026EE RID: 9966
		private int textArrayIndex;

		// Token: 0x040026EF RID: 9967
		private bool done;

		// Token: 0x040026F0 RID: 9968
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
