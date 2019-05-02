using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200059B RID: 1435
	[CreateAssetMenu]
	public class UISkinData : ScriptableObject
	{
		// Token: 0x040022AF RID: 8879
		[Header("Main Panel Style")]
		public UISkinData.PanelStyle mainPanelStyle;

		// Token: 0x040022B0 RID: 8880
		[Header("Header Style")]
		public UISkinData.PanelStyle headerPanelStyle;

		// Token: 0x040022B1 RID: 8881
		public UISkinData.TextStyle headerTextStyle;

		// Token: 0x040022B2 RID: 8882
		[Header("Detail Style")]
		public UISkinData.PanelStyle detailPanelStyle;

		// Token: 0x040022B3 RID: 8883
		public UISkinData.TextStyle detailTextStyle;

		// Token: 0x040022B4 RID: 8884
		[Header("Body Style")]
		public UISkinData.TextStyle bodyTextStyle;

		// Token: 0x040022B5 RID: 8885
		[Header("Button Style")]
		public UISkinData.ButtonStyle buttonStyle;

		// Token: 0x040022B6 RID: 8886
		[Header("Scroll Rect Style")]
		public UISkinData.ScrollRectStyle scrollRectStyle;

		// Token: 0x0200059C RID: 1436
		[Serializable]
		public struct TextStyle
		{
			// Token: 0x06002046 RID: 8262 RVA: 0x0001776B File Offset: 0x0001596B
			public void Apply(TextMeshProUGUI label, bool useAlignment = true)
			{
				label.font = this.font;
				label.fontSize = this.fontSize;
				label.color = this.color;
				if (useAlignment)
				{
					label.alignment = this.alignment;
				}
			}

			// Token: 0x040022B7 RID: 8887
			public TMP_FontAsset font;

			// Token: 0x040022B8 RID: 8888
			public float fontSize;

			// Token: 0x040022B9 RID: 8889
			public TextAlignmentOptions alignment;

			// Token: 0x040022BA RID: 8890
			public Color color;
		}

		// Token: 0x0200059D RID: 1437
		[Serializable]
		public struct PanelStyle
		{
			// Token: 0x06002047 RID: 8263 RVA: 0x000177A0 File Offset: 0x000159A0
			public void Apply(Image image)
			{
				image.material = this.material;
				image.sprite = this.sprite;
				image.color = this.color;
			}

			// Token: 0x040022BB RID: 8891
			public Material material;

			// Token: 0x040022BC RID: 8892
			public Sprite sprite;

			// Token: 0x040022BD RID: 8893
			public Color color;
		}

		// Token: 0x0200059E RID: 1438
		[Serializable]
		public struct ButtonStyle
		{
			// Token: 0x040022BE RID: 8894
			public Material material;

			// Token: 0x040022BF RID: 8895
			public Sprite sprite;

			// Token: 0x040022C0 RID: 8896
			public ColorBlock colors;

			// Token: 0x040022C1 RID: 8897
			public UISkinData.TextStyle interactableTextStyle;

			// Token: 0x040022C2 RID: 8898
			public UISkinData.TextStyle disabledTextStyle;

			// Token: 0x040022C3 RID: 8899
			public float recommendedWidth;

			// Token: 0x040022C4 RID: 8900
			public float recommendedHeight;
		}

		// Token: 0x0200059F RID: 1439
		[Serializable]
		public struct ScrollRectStyle
		{
			// Token: 0x040022C5 RID: 8901
			[FormerlySerializedAs("viewportPanelStyle")]
			public UISkinData.PanelStyle backgroundPanelStyle;

			// Token: 0x040022C6 RID: 8902
			public UISkinData.PanelStyle scrollbarBackgroundStyle;

			// Token: 0x040022C7 RID: 8903
			public ColorBlock scrollbarHandleColors;

			// Token: 0x040022C8 RID: 8904
			public Sprite scrollbarHandleImage;
		}
	}
}
