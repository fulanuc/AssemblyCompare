using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E0 RID: 1504
	[ExecuteInEditMode]
	public class DifficultyBarController : MonoBehaviour
	{
		// Token: 0x060021EB RID: 8683 RVA: 0x000A265C File Offset: 0x000A085C
		private static Color ColorMultiplySaturationAndValue(ref Color col, float saturationMultiplier, float valueMultiplier)
		{
			float h;
			float num;
			float num2;
			Color.RGBToHSV(col, out h, out num, out num2);
			return Color.HSVToRGB(h, num * saturationMultiplier, num2 * valueMultiplier);
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x000A2688 File Offset: 0x000A0888
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

		// Token: 0x060021ED RID: 8685 RVA: 0x000A2954 File Offset: 0x000A0B54
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

		// Token: 0x060021EE RID: 8686 RVA: 0x00018B90 File Offset: 0x00016D90
		private float CalcSegmentStartX(int i)
		{
			return (float)i * this.elementWidth;
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x00018B9B File Offset: 0x00016D9B
		private float CalcSegmentEndX(int i)
		{
			return (float)(i + 1) * this.elementWidth;
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x000A2B38 File Offset: 0x000A0D38
		private void SetLabelDefaultDimensions(RectTransform labelRectTransform)
		{
			labelRectTransform.anchorMin = new Vector2(0f, 0f);
			labelRectTransform.anchorMax = new Vector2(1f, 1f);
			labelRectTransform.pivot = new Vector2(0.5f, 0.5f);
			labelRectTransform.offsetMin = new Vector2(0f, 0f);
			labelRectTransform.offsetMax = new Vector2(0f, 0f);
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x000A2BB0 File Offset: 0x000A0DB0
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

		// Token: 0x060021F2 RID: 8690 RVA: 0x000A2CE4 File Offset: 0x000A0EE4
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

		// Token: 0x060021F3 RID: 8691 RVA: 0x000A2D78 File Offset: 0x000A0F78
		private static void ScaleLabelToWidth(TextMeshProUGUI label, float width)
		{
			RectTransform rectTransform = (RectTransform)label.transform;
			float x = label.textBounds.size.x;
			Vector3 localScale = rectTransform.localScale;
			localScale.x = width / x;
			rectTransform.localScale = localScale;
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x000A2DBC File Offset: 0x000A0FBC
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

		// Token: 0x060021F5 RID: 8693 RVA: 0x000A2EB8 File Offset: 0x000A10B8
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

		// Token: 0x060021F6 RID: 8694 RVA: 0x00018BA8 File Offset: 0x00016DA8
		private void Awake()
		{
			this.SetupSegments();
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x000A2F30 File Offset: 0x000A1130
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

		// Token: 0x060021F8 RID: 8696 RVA: 0x000A2F80 File Offset: 0x000A1180
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

		// Token: 0x060021F9 RID: 8697 RVA: 0x000A2FEC File Offset: 0x000A11EC
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

		// Token: 0x0400243A RID: 9274
		[Header("Component References")]
		public RectTransform viewPort;

		// Token: 0x0400243B RID: 9275
		public RectTransform segmentContainer;

		// Token: 0x0400243C RID: 9276
		[Header("Layout")]
		[Tooltip("How wide each segment should be.")]
		public float elementWidth;

		// Token: 0x0400243D RID: 9277
		public float levelsPerSegment;

		// Token: 0x0400243E RID: 9278
		public float debugTime;

		// Token: 0x0400243F RID: 9279
		[Header("Segment Parameters")]
		public DifficultyBarController.SegmentDef[] segmentDefs;

		// Token: 0x04002440 RID: 9280
		[Tooltip("The prefab to instantiate for each segment.")]
		public GameObject segmentPrefab;

		// Token: 0x04002441 RID: 9281
		[Header("Colors")]
		public float pastSaturationMultiplier;

		// Token: 0x04002442 RID: 9282
		public float pastValueMultiplier;

		// Token: 0x04002443 RID: 9283
		public Color pastLabelColor;

		// Token: 0x04002444 RID: 9284
		public float currentSaturationMultiplier;

		// Token: 0x04002445 RID: 9285
		public float currentValueMultiplier;

		// Token: 0x04002446 RID: 9286
		public Color currentLabelColor;

		// Token: 0x04002447 RID: 9287
		public float upcomingSaturationMultiplier;

		// Token: 0x04002448 RID: 9288
		public float upcomingValueMultiplier;

		// Token: 0x04002449 RID: 9289
		public Color upcomingLabelColor;

		// Token: 0x0400244A RID: 9290
		[Header("Animations")]
		public AnimationCurve fadeAnimationCurve;

		// Token: 0x0400244B RID: 9291
		public float fadeAnimationDuration = 1f;

		// Token: 0x0400244C RID: 9292
		public AnimationCurve flashAnimationCurve;

		// Token: 0x0400244D RID: 9293
		public float flashAnimationDuration = 0.5f;

		// Token: 0x0400244E RID: 9294
		private int currentSegmentIndex = -1;

		// Token: 0x0400244F RID: 9295
		private static readonly Color labelFadedColor = Color.Lerp(Color.gray, Color.white, 0.5f);

		// Token: 0x04002450 RID: 9296
		[Header("Final Segment")]
		public Sprite finalSegmentSprite;

		// Token: 0x04002451 RID: 9297
		private float scrollX;

		// Token: 0x04002452 RID: 9298
		private float scrollXRaw;

		// Token: 0x04002453 RID: 9299
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public Image[] images;

		// Token: 0x04002454 RID: 9300
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public TextMeshProUGUI[] labels;

		// Token: 0x04002455 RID: 9301
		public RawImage[] wormGearImages;

		// Token: 0x04002456 RID: 9302
		public float UVScaleToScrollX;

		// Token: 0x04002457 RID: 9303
		public float gearUVOffset;

		// Token: 0x04002458 RID: 9304
		private readonly List<DifficultyBarController.SegmentImageAnimation> playingAnimations = new List<DifficultyBarController.SegmentImageAnimation>();

		// Token: 0x020005E1 RID: 1505
		[Serializable]
		public struct SegmentDef
		{
			// Token: 0x04002459 RID: 9305
			[Tooltip("The default English string to use for the element at design time.")]
			public string debugString;

			// Token: 0x0400245A RID: 9306
			[Tooltip("The final language token to use for this element at runtime.")]
			public string token;

			// Token: 0x0400245B RID: 9307
			[Tooltip("The color to use for the panel.")]
			public Color color;
		}

		// Token: 0x020005E2 RID: 1506
		private class SegmentImageAnimation
		{
			// Token: 0x060021FC RID: 8700 RVA: 0x00018BFB File Offset: 0x00016DFB
			public void Update(float t)
			{
				this.segmentImage.color = Color.Lerp(this.color0, this.color1, this.colorCurve.Evaluate(t));
			}

			// Token: 0x0400245C RID: 9308
			public Image segmentImage;

			// Token: 0x0400245D RID: 9309
			public float age;

			// Token: 0x0400245E RID: 9310
			public float duration;

			// Token: 0x0400245F RID: 9311
			public AnimationCurve colorCurve;

			// Token: 0x04002460 RID: 9312
			public Color color0;

			// Token: 0x04002461 RID: 9313
			public Color color1;
		}
	}
}
