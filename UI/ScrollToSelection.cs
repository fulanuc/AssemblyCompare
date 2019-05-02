using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063F RID: 1599
	[RequireComponent(typeof(ScrollRect))]
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ScrollToSelection : MonoBehaviour
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06002417 RID: 9239 RVA: 0x0001A585 File Offset: 0x00018785
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x0001A592 File Offset: 0x00018792
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x000AB4DC File Offset: 0x000A96DC
		private void Update()
		{
			GameObject gameObject = this.eventSystem ? this.eventSystem.currentSelectedGameObject : null;
			if (this.lastSelectedObject != gameObject)
			{
				if (gameObject && gameObject.transform.IsChildOf(base.transform))
				{
					this.ScrollToRect((RectTransform)gameObject.transform);
				}
				this.lastSelectedObject = gameObject;
			}
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x000AB548 File Offset: 0x000A9748
		private void ScrollToRect(RectTransform targetRectTransform)
		{
			targetRectTransform.GetWorldCorners(this.targetWorldCorners);
			((RectTransform)base.transform).GetWorldCorners(this.viewPortWorldCorners);
			if (this.scrollRect.vertical && this.scrollRect.verticalScrollbar)
			{
				float y = this.targetWorldCorners[1].y;
				float y2 = this.targetWorldCorners[0].y;
				float y3 = this.viewPortWorldCorners[1].y;
				float y4 = this.viewPortWorldCorners[0].y;
				float num = y - y3;
				float num2 = y2 - y4;
				float num3 = y3 - y4;
				if (num > 0f)
				{
					this.scrollRect.verticalScrollbar.value += num / num3;
				}
				if (num2 < 0f)
				{
					this.scrollRect.verticalScrollbar.value += num2 / num3;
				}
			}
			if (this.scrollRect.horizontal && this.scrollRect.horizontalScrollbar)
			{
				float y5 = this.targetWorldCorners[2].y;
				float y6 = this.targetWorldCorners[0].y;
				float y7 = this.viewPortWorldCorners[2].y;
				float y8 = this.viewPortWorldCorners[0].y;
				float num4 = y5 - y7;
				float num5 = y6 - y8;
				float num6 = y7 - y8;
				if (num4 > 0f)
				{
					this.scrollRect.horizontalScrollbar.value += num4 / num6;
				}
				if (num5 < 0f)
				{
					this.scrollRect.horizontalScrollbar.value += num5 / num6;
				}
			}
		}

		// Token: 0x040026C7 RID: 9927
		private ScrollRect scrollRect;

		// Token: 0x040026C8 RID: 9928
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040026C9 RID: 9929
		private Vector3[] targetWorldCorners = new Vector3[4];

		// Token: 0x040026CA RID: 9930
		private Vector3[] viewPortWorldCorners = new Vector3[4];

		// Token: 0x040026CB RID: 9931
		private GameObject lastSelectedObject;
	}
}
