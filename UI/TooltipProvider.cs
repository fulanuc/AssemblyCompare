using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000659 RID: 1625
	public class TooltipProvider : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600248A RID: 9354 RVA: 0x0001A987 File Offset: 0x00018B87
		private bool tooltipIsAvailable
		{
			get
			{
				return this.titleColor != Color.clear;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x0600248B RID: 9355 RVA: 0x0001A999 File Offset: 0x00018B99
		public string titleText
		{
			get
			{
				if (!string.IsNullOrEmpty(this.overrideTitleText))
				{
					return this.overrideTitleText;
				}
				if (this.titleToken == null)
				{
					return null;
				}
				return Language.GetString(this.titleToken);
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600248C RID: 9356 RVA: 0x0001A9C4 File Offset: 0x00018BC4
		public string bodyText
		{
			get
			{
				if (!string.IsNullOrEmpty(this.overrideBodyText))
				{
					return this.overrideBodyText;
				}
				if (this.bodyToken == null)
				{
					return null;
				}
				return Language.GetString(this.bodyToken);
			}
		}

		// Token: 0x0600248D RID: 9357 RVA: 0x000ACE00 File Offset: 0x000AB000
		public void SetContent(TooltipContent tooltipContent)
		{
			this.titleToken = tooltipContent.titleToken;
			this.overrideTitleText = tooltipContent.overrideTitleText;
			this.titleColor = tooltipContent.titleColor;
			this.bodyToken = tooltipContent.bodyToken;
			this.overrideBodyText = tooltipContent.overrideBodyText;
			this.bodyColor = tooltipContent.bodyColor;
			this.disableTitleRichText = tooltipContent.disableTitleRichText;
			this.disableBodyRichText = tooltipContent.disableBodyRichText;
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x0001A9EF File Offset: 0x00018BEF
		private void OnDisable()
		{
			TooltipController.RemoveTooltip(this);
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x000ACE70 File Offset: 0x000AB070
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.SetTooltip(mpeventSystem, this, eventData.position);
			}
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x000ACEA8 File Offset: 0x000AB0A8
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.RemoveTooltip(mpeventSystem, this);
			}
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x000ACED8 File Offset: 0x000AB0D8
		public static TooltipContent GetPlayerNameTooltipContent(string userName)
		{
			string stringFormatted = Language.GetStringFormatted("PLAYER_NAME_TOOLTIP_FORMAT", new object[]
			{
				userName
			});
			return new TooltipContent
			{
				overrideTitleText = stringFormatted,
				disableTitleRichText = true,
				titleColor = TooltipProvider.playerColor
			};
		}

		// Token: 0x04002732 RID: 10034
		public string titleToken = "";

		// Token: 0x04002733 RID: 10035
		public Color titleColor = Color.clear;

		// Token: 0x04002734 RID: 10036
		public string bodyToken = "";

		// Token: 0x04002735 RID: 10037
		public Color bodyColor;

		// Token: 0x04002736 RID: 10038
		public string overrideTitleText = "";

		// Token: 0x04002737 RID: 10039
		public string overrideBodyText = "";

		// Token: 0x04002738 RID: 10040
		public bool disableTitleRichText;

		// Token: 0x04002739 RID: 10041
		public bool disableBodyRichText;

		// Token: 0x0400273A RID: 10042
		[NonSerialized]
		public int userCount;

		// Token: 0x0400273B RID: 10043
		private static readonly Color playerColor = new Color32(242, 65, 65, byte.MaxValue);
	}
}
