using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000647 RID: 1607
	public class TooltipProvider : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060023FA RID: 9210 RVA: 0x0001A2B9 File Offset: 0x000184B9
		private bool tooltipIsAvailable
		{
			get
			{
				return this.titleColor != Color.clear;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060023FB RID: 9211 RVA: 0x0001A2CB File Offset: 0x000184CB
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

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060023FC RID: 9212 RVA: 0x0001A2F6 File Offset: 0x000184F6
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

		// Token: 0x060023FD RID: 9213 RVA: 0x000AB780 File Offset: 0x000A9980
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

		// Token: 0x060023FE RID: 9214 RVA: 0x0001A321 File Offset: 0x00018521
		private void OnDisable()
		{
			TooltipController.RemoveTooltip(this);
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x000AB7F0 File Offset: 0x000A99F0
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.SetTooltip(mpeventSystem, this, eventData.position);
			}
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x000AB828 File Offset: 0x000A9A28
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			MPEventSystem mpeventSystem = EventSystem.current as MPEventSystem;
			if (mpeventSystem != null && this.tooltipIsAvailable)
			{
				TooltipController.RemoveTooltip(mpeventSystem, this);
			}
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x000AB858 File Offset: 0x000A9A58
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

		// Token: 0x040026D7 RID: 9943
		public string titleToken = "";

		// Token: 0x040026D8 RID: 9944
		public Color titleColor = Color.clear;

		// Token: 0x040026D9 RID: 9945
		public string bodyToken = "";

		// Token: 0x040026DA RID: 9946
		public Color bodyColor;

		// Token: 0x040026DB RID: 9947
		public string overrideTitleText = "";

		// Token: 0x040026DC RID: 9948
		public string overrideBodyText = "";

		// Token: 0x040026DD RID: 9949
		public bool disableTitleRichText;

		// Token: 0x040026DE RID: 9950
		public bool disableBodyRichText;

		// Token: 0x040026DF RID: 9951
		[NonSerialized]
		public int userCount;

		// Token: 0x040026E0 RID: 9952
		private static readonly Color playerColor = new Color32(242, 65, 65, byte.MaxValue);
	}
}
