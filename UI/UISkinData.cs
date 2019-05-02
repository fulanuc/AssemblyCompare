using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AD RID: 1453
	[CreateAssetMenu]
	public class UISkinData : ScriptableObject
	{
		// Token: 0x04002303 RID: 8963
		[Header("Main Panel Style")]
		public UISkinData.PanelStyle mainPanelStyle;

		// Token: 0x04002304 RID: 8964
		[Header("Header Style")]
		public UISkinData.PanelStyle headerPanelStyle;

		// Token: 0x04002305 RID: 8965
		public UISkinData.TextStyle headerTextStyle;

		// Token: 0x04002306 RID: 8966
		[Header("Detail Style")]
		public UISkinData.PanelStyle detailPanelStyle;

		// Token: 0x04002307 RID: 8967
		public UISkinData.TextStyle detailTextStyle;

		// Token: 0x04002308 RID: 8968
		[Header("Body Style")]
		public UISkinData.TextStyle bodyTextStyle;

		// Token: 0x04002309 RID: 8969
		[Header("Button Style")]
		public UISkinData.ButtonStyle buttonStyle;

		// Token: 0x0400230A RID: 8970
		[Header("Scroll Rect Style")]
		public UISkinData.ScrollRectStyle scrollRectStyle;

		// Token: 0x020005AE RID: 1454
		[Serializable]
		public struct TextStyle
		{
			// Token: 0x060020D7 RID: 8407 RVA: 0x0009EA00 File Offset: 0x0009CC00
			public void Apply(TextMeshProUGUI label, bool useAlignment = true)
			{
				if (label.font != this.font)
				{
					label.font = this.font;
				}
				if (label.fontSize != this.fontSize)
				{
					label.fontSize = this.fontSize;
				}
				if (label.color != this.color)
				{
					label.color = this.color;
				}
				if (useAlignment && label.alignment != this.alignment)
				{
					label.alignment = this.alignment;
				}
			}

			// Token: 0x0400230B RID: 8971
			public TMP_FontAsset font;

			// Token: 0x0400230C RID: 8972
			public float fontSize;

			// Token: 0x0400230D RID: 8973
			public TextAlignmentOptions alignment;

			// Token: 0x0400230E RID: 8974
			public Color color;
		}

		// Token: 0x020005AF RID: 1455
		[Serializable]
		public struct PanelStyle
		{
			// Token: 0x060020D8 RID: 8408 RVA: 0x00017E9A File Offset: 0x0001609A
			public void Apply(Image image)
			{
				image.material = this.material;
				image.sprite = this.sprite;
				image.color = this.color;
			}

			// Token: 0x0400230F RID: 8975
			public Material material;

			// Token: 0x04002310 RID: 8976
			public Sprite sprite;

			// Token: 0x04002311 RID: 8977
			public Color color;
		}

		// Token: 0x020005B0 RID: 1456
		[Serializable]
		public struct ButtonStyle
		{
			// Token: 0x04002312 RID: 8978
			public Material material;

			// Token: 0x04002313 RID: 8979
			public Sprite sprite;

			// Token: 0x04002314 RID: 8980
			public ColorBlock colors;

			// Token: 0x04002315 RID: 8981
			public UISkinData.TextStyle interactableTextStyle;

			// Token: 0x04002316 RID: 8982
			public UISkinData.TextStyle disabledTextStyle;

			// Token: 0x04002317 RID: 8983
			public float recommendedWidth;

			// Token: 0x04002318 RID: 8984
			public float recommendedHeight;
		}

		// Token: 0x020005B1 RID: 1457
		[Serializable]
		public struct ScrollRectStyle
		{
			// Token: 0x04002319 RID: 8985
			[FormerlySerializedAs("viewportPanelStyle")]
			public UISkinData.PanelStyle backgroundPanelStyle;

			// Token: 0x0400231A RID: 8986
			public UISkinData.PanelStyle scrollbarBackgroundStyle;

			// Token: 0x0400231B RID: 8987
			public ColorBlock scrollbarHandleColors;

			// Token: 0x0400231C RID: 8988
			public Sprite scrollbarHandleImage;
		}
	}
}
