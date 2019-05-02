using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CE RID: 1486
	[ExecuteInEditMode]
	public class DifficultyBarController : MonoBehaviour
	{
		// Token: 0x0600215A RID: 8538 RVA: 0x000A1088 File Offset: 0x0009F288
		private static Color ColorMultiplySaturationAndValue(ref Color col, float saturationMultiplier, float valueMultiplier)
		{
			float h;
			float num;
			float num2;
			Color.RGBToHSV(col, out h, out num, out num2);
			return Color.HSVToRGB(h, num * saturationMultiplier, num2 * valueMultiplier);
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x000A10B4 File Offset: 0x0009F2B4
		private void OnCurrentSegmentIndexChanged(int newSegmentIndex)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			int num = newSegmentIndex - 1;
			float width = this.viewPort.rect.width;
			int i = 0;
			int num2 = this.images.Length - 1;
			while (i < num2)
			{
				Image image = this.images[i];
				RectTransform rectTransform = image.rectTransform;
				bool enabled = rectTransform.offsetMax.x + this.scrollX >= 0f && rectTransform.offsetMin.x + this.scrollX <= width;
				image.enabled = enabled;
				i++;
			}
			int num3 = this.images.Length - 1;
			Image image2 = this.images[num3];
			bool enabled2 = image2.rectTransform.offsetMax.x + this.scrollX >= 0f;
			image2.enabled = enabled2;
			for (int j = 0; j <= num; j++)
			{
				this.images[j].color = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[j].color, this.pastSaturationMultiplier, this.pastValueMultiplier);
				this.labels[j].color = this.pastLabelColor;
			}
			for (int k = newSegmentIndex + 1; k < this.images.Length; k++)
			{
				this.images[k].color = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[k].color, this.upcomingSaturationMultiplier, this.upcomingValueMultiplier);
				this.labels[k].color = this.upcomingLabelColor;
			}
			Image image3 = (num != -1) ? this.images[num] : null;
			Image image4 = (newSegmentIndex != -1) ? this.images[newSegmentIndex] : null;
			TextMeshProUGUI textMeshProUGUI = (newSegmentIndex != -1) ? this.labels[newSegmentIndex] : null;
			if (image3)
			{
				this.playingAnimations.Add(new DifficultyBarController.SegmentImageAnimation
				{
					age = 0f,
					duration = this.fadeAnimationDuration,
					segmentImage = image3,
					colorCurve = this.fadeAnimationCurve,
					color0 = this.segmentDefs[num].color,
					color1 = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[num].color, this.pastSaturationMultiplier, this.pastValueMultiplier)
				});
			}
			if (image4)
			{
				this.playingAnimations.Add(new DifficultyBarController.SegmentImageAnimation
				{
					age = 0f,
					duration = this.flashAnimationDuration,
					segmentImage = image4,
					colorCurve = this.flashAnimationCurve,
					color0 = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[newSegmentIndex].color, this.currentSaturationMultiplier, this.currentValueMultiplier),
					color1 = Color.white
				});
			}
			if (textMeshProUGUI)
			{
				textMeshProUGUI.color = this.currentLabelColor;
			}
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000A1380 File Offset: 0x0009F580
		private void SetSegmentScroll(float segmentScroll)
		{
			float num = (float)(this.segmentDefs.Length + 2);
			if (segmentScroll > num)
			{
				segmentScroll = num - 1f + (segmentScroll - Mathf.Floor(segmentScroll));
			}
			this.scrollXRaw = (segmentScroll - 1f) * -this.elementWidth;
			this.scrollX = Mathf.Floor(this.scrollXRaw);
			int num2 = this.currentSegmentIndex;
			this.currentSegmentIndex = Mathf.Clamp(Mathf.FloorToInt(segmentScroll), 0, this.segmentContainer.childCount - 1);
			if (num2 != this.currentSegmentIndex)
			{
				this.OnCurrentSegmentIndexChanged(this.currentSegmentIndex);
			}
			Vector2 offsetMin = this.segmentContainer.offsetMin;
			offsetMin.x = this.scrollX;
			this.segmentContainer.offsetMin = offsetMin;
			if (this.segmentContainer && this.segmentContainer.childCount > 0)
			{
				int num3 = this.segmentContainer.childCount - 1;
				RectTransform rectTransform = (RectTransform)this.segmentContainer.GetChild(num3);
				RectTransform rectTransform2 = (RectTransform)rectTransform.Find("Label");
				TextMeshProUGUI textMeshProUGUI = this.labels[num3];
				if (segmentScroll >= (float)(num3 - 1))
				{
					float num4 = this.elementWidth;
					Vector2 offsetMin2 = rectTransform.offsetMin;
					offsetMin2.x = this.CalcSegmentStartX(num3);
					rectTransform.offsetMin = offsetMin2;
					Vector2 offsetMax = rectTransform.offsetMax;
					offsetMax.x = offsetMin2.x + num4;
					rectTransform.offsetMax = offsetMax;
					rectTransform2.anchorMin = new Vector2(0f, 0f);
					rectTransform2.anchorMax = new Vector2(0f, 1f);
					rectTransform2.offsetMin = new Vector2(0f, 0f);
					rectTransform2.offsetMax = new Vector2(this.elementWidth, 0f);
					return;
				}
				rectTransform.offsetMax = rectTransform.offsetMin + new Vector2(this.elementWidth, 0f);
				this.SetLabelDefaultDimensions(rectTransform2);
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x00018496 File Offset: 0x00016696
		private float CalcSegmentStartX(int i)
		{
			return (float)i * this.elementWidth;
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x000184A1 File Offset: 0x000166A1
		private float CalcSegmentEndX(int i)
		{
			return (float)(i + 1) * this.elementWidth;
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x000A1564 File Offset: 0x0009F764
		private void SetLabelDefaultDimensions(RectTransform labelRectTransform)
		{
			labelRectTransform.anchorMin = new Vector2(0f, 0f);
			labelRectTransform.anchorMax = new Vector2(1f, 1f);
			labelRectTransform.pivot = new Vector2(0.5f, 0.5f);
			labelRectTransform.offsetMin = new Vector2(0f, 0f);
			labelRectTransform.offsetMax = new Vector2(0f, 0f);
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000A15DC File Offset: 0x0009F7DC
		private void SetSegmentCount(uint desiredCount)
		{
			if (!this.segmentContainer || !this.segmentPrefab)
			{
				return;
			}
			uint num = (uint)this.segmentContainer.childCount;
			if (this.images == null || (long)this.images.Length != (long)((ulong)desiredCount))
			{
				this.images = new Image[desiredCount];
				this.labels = new TextMeshProUGUI[desiredCount];
			}
			int i = 0;
			int num2 = Mathf.Min(this.images.Length, this.segmentContainer.childCount);
			while (i < num2)
			{
				Transform child = this.segmentContainer.GetChild(i);
				this.images[i] = child.GetComponent<Image>();
				this.labels[i] = child.Find("Label").GetComponent<TextMeshProUGUI>();
				i++;
			}
			while (num > desiredCount)
			{
				UnityEngine.Object.DestroyImmediate(this.segmentContainer.GetChild((int)(num - 1u)).gameObject);
				num -= 1u;
			}
			while (num < desiredCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.segmentPrefab, this.segmentContainer);
				gameObject.SetActive(true);
				this.images[i] = gameObject.GetComponent<Image>();
				this.labels[i] = gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				i++;
				num += 1u;
			}
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000A1710 File Offset: 0x0009F910
		private void SetupSegments()
		{
			if (!this.segmentContainer || !this.segmentPrefab)
			{
				return;
			}
			this.SetSegmentCount((uint)this.segmentDefs.Length);
			for (int i = 0; i < this.segmentContainer.childCount; i++)
			{
				this.SetupSegment((RectTransform)this.segmentContainer.GetChild(i), ref this.segmentDefs[i], i);
			}
			this.SetupFinalSegment((RectTransform)this.segmentContainer.GetChild(this.segmentContainer.childCount - 1));
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x000A17A4 File Offset: 0x0009F9A4
		private static void ScaleLabelToWidth(TextMeshProUGUI label, float width)
		{
			RectTransform rectTransform = (RectTransform)label.transform;
			float x = label.textBounds.size.x;
			Vector3 localScale = rectTransform.localScale;
			localScale.x = width / x;
			rectTransform.localScale = localScale;
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x000A17E8 File Offset: 0x0009F9E8
		private void SetupFinalSegment(RectTransform segmentTransform)
		{
			TextMeshProUGUI[] array = segmentTransform.GetComponentsInChildren<TextMeshProUGUI>();
			int num = 4;
			if (array.Length < num)
			{
				TextMeshProUGUI[] array2 = new TextMeshProUGUI[num];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i];
				}
				for (int j = array.Length; j < num; j++)
				{
					array2[j] = UnityEngine.Object.Instantiate<GameObject>(array[0].gameObject, segmentTransform).GetComponent<TextMeshProUGUI>();
				}
				array = array2;
			}
			int k = 0;
			int num2 = array.Length;
			while (k < num2)
			{
				TextMeshProUGUI textMeshProUGUI = array[k];
				textMeshProUGUI.enableWordWrapping = false;
				textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
				textMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
				textMeshProUGUI.text = Language.GetString(this.segmentDefs[this.segmentDefs.Length - 1].token);
				textMeshProUGUI.enableAutoSizing = true;
				Vector3 localPosition = textMeshProUGUI.transform.localPosition;
				localPosition.x = (float)k * this.elementWidth;
				textMeshProUGUI.transform.localPosition = localPosition;
				k++;
			}
			segmentTransform.GetComponent<Image>().sprite = this.finalSegmentSprite;
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x000A18E4 File Offset: 0x0009FAE4
		private void SetupSegment(RectTransform segmentTransform, ref DifficultyBarController.SegmentDef segmentDef, int i)
		{
			Vector2 offsetMin = segmentTransform.offsetMin;
			Vector2 offsetMax = segmentTransform.offsetMax;
			offsetMin.x = this.CalcSegmentStartX(i);
			offsetMax.x = this.CalcSegmentEndX(i);
			segmentTransform.offsetMin = offsetMin;
			segmentTransform.offsetMax = offsetMax;
			segmentTransform.GetComponent<Image>().color = segmentDef.color;
			((RectTransform)segmentTransform.Find("Label")).GetComponent<LanguageTextMeshController>().token = segmentDef.token;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x000184AE File Offset: 0x000166AE
		private void Awake()
		{
			this.SetupSegments();
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000A195C File Offset: 0x0009FB5C
		private void Update()
		{
			if (Run.instance)
			{
				this.SetSegmentScroll((Run.instance.targetMonsterLevel - 1f) / this.levelsPerSegment);
			}
			if (Application.isPlaying)
			{
				this.RunAnimations(Time.deltaTime);
			}
			this.UpdateGears();
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000A19AC File Offset: 0x0009FBAC
		private void UpdateGears()
		{
			foreach (RawImage rawImage in this.wormGearImages)
			{
				Rect uvRect = rawImage.uvRect;
				float num = Mathf.Sign(uvRect.width);
				uvRect.x = this.scrollXRaw * this.UVScaleToScrollX * num + ((num < 0f) ? this.gearUVOffset : 0f);
				rawImage.uvRect = uvRect;
			}
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x000A1A18 File Offset: 0x0009FC18
		private void RunAnimations(float deltaTime)
		{
			for (int i = this.playingAnimations.Count - 1; i >= 0; i--)
			{
				DifficultyBarController.SegmentImageAnimation segmentImageAnimation = this.playingAnimations[i];
				segmentImageAnimation.age += deltaTime;
				float num = Mathf.Clamp01(segmentImageAnimation.age / segmentImageAnimation.duration);
				segmentImageAnimation.Update(num);
				if (num >= 1f)
				{
					this.playingAnimations.RemoveAt(i);
				}
			}
		}

		// Token: 0x040023E6 RID: 9190
		[Header("Component References")]
		public RectTransform viewPort;

		// Token: 0x040023E7 RID: 9191
		public RectTransform segmentContainer;

		// Token: 0x040023E8 RID: 9192
		[Header("Layout")]
		[Tooltip("How wide each segment should be.")]
		public float elementWidth;

		// Token: 0x040023E9 RID: 9193
		public float levelsPerSegment;

		// Token: 0x040023EA RID: 9194
		public float debugTime;

		// Token: 0x040023EB RID: 9195
		[Header("Segment Parameters")]
		public DifficultyBarController.SegmentDef[] segmentDefs;

		// Token: 0x040023EC RID: 9196
		[Tooltip("The prefab to instantiate for each segment.")]
		public GameObject segmentPrefab;

		// Token: 0x040023ED RID: 9197
		[Header("Colors")]
		public float pastSaturationMultiplier;

		// Token: 0x040023EE RID: 9198
		public float pastValueMultiplier;

		// Token: 0x040023EF RID: 9199
		public Color pastLabelColor;

		// Token: 0x040023F0 RID: 9200
		public float currentSaturationMultiplier;

		// Token: 0x040023F1 RID: 9201
		public float currentValueMultiplier;

		// Token: 0x040023F2 RID: 9202
		public Color currentLabelColor;

		// Token: 0x040023F3 RID: 9203
		public float upcomingSaturationMultiplier;

		// Token: 0x040023F4 RID: 9204
		public float upcomingValueMultiplier;

		// Token: 0x040023F5 RID: 9205
		public Color upcomingLabelColor;

		// Token: 0x040023F6 RID: 9206
		[Header("Animations")]
		public AnimationCurve fadeAnimationCurve;

		// Token: 0x040023F7 RID: 9207
		public float fadeAnimationDuration = 1f;

		// Token: 0x040023F8 RID: 9208
		public AnimationCurve flashAnimationCurve;

		// Token: 0x040023F9 RID: 9209
		public float flashAnimationDuration = 0.5f;

		// Token: 0x040023FA RID: 9210
		private int currentSegmentIndex = -1;

		// Token: 0x040023FB RID: 9211
		private static readonly Color labelFadedColor = Color.Lerp(Color.gray, Color.white, 0.5f);

		// Token: 0x040023FC RID: 9212
		[Header("Final Segment")]
		public Sprite finalSegmentSprite;

		// Token: 0x040023FD RID: 9213
		private float scrollX;

		// Token: 0x040023FE RID: 9214
		private float scrollXRaw;

		// Token: 0x040023FF RID: 9215
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public Image[] images;

		// Token: 0x04002400 RID: 9216
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public TextMeshProUGUI[] labels;

		// Token: 0x04002401 RID: 9217
		public RawImage[] wormGearImages;

		// Token: 0x04002402 RID: 9218
		public float UVScaleToScrollX;

		// Token: 0x04002403 RID: 9219
		public float gearUVOffset;

		// Token: 0x04002404 RID: 9220
		private readonly List<DifficultyBarController.SegmentImageAnimation> playingAnimations = new List<DifficultyBarController.SegmentImageAnimation>();

		// Token: 0x020005CF RID: 1487
		[Serializable]
		public struct SegmentDef
		{
			// Token: 0x04002405 RID: 9221
			[Tooltip("The default English string to use for the element at design time.")]
			public string debugString;

			// Token: 0x04002406 RID: 9222
			[Tooltip("The final language token to use for this element at runtime.")]
			public string token;

			// Token: 0x04002407 RID: 9223
			[Tooltip("The color to use for the panel.")]
			public Color color;
		}

		// Token: 0x020005D0 RID: 1488
		private class SegmentImageAnimation
		{
			// Token: 0x0600216B RID: 8555 RVA: 0x00018501 File Offset: 0x00016701
			public void Update(float t)
			{
				this.segmentImage.color = Color.Lerp(this.color0, this.color1, this.colorCurve.Evaluate(t));
			}

			// Token: 0x04002408 RID: 9224
			public Image segmentImage;

			// Token: 0x04002409 RID: 9225
			public float age;

			// Token: 0x0400240A RID: 9226
			public float duration;

			// Token: 0x0400240B RID: 9227
			public AnimationCurve colorCurve;

			// Token: 0x0400240C RID: 9228
			public Color color0;

			// Token: 0x0400240D RID: 9229
			public Color color1;
		}
	}
}
