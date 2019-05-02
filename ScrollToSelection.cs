using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062D RID: 1581
	[RequireComponent(typeof(MPEventSystemLocator))]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollToSelection : MonoBehaviour
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06002387 RID: 9095 RVA: 0x00019EB7 File Offset: 0x000180B7
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x00019EC4 File Offset: 0x000180C4
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x000A9E60 File Offset: 0x000A8060
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

		// Token: 0x0600238A RID: 9098 RVA: 0x000A9ECC File Offset: 0x000A80CC
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

		// Token: 0x0400266C RID: 9836
		private ScrollRect scrollRect;

		// Token: 0x0400266D RID: 9837
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400266E RID: 9838
		private Vector3[] targetWorldCorners = new Vector3[4];

		// Token: 0x0400266F RID: 9839
		private Vector3[] viewPortWorldCorners = new Vector3[4];

		// Token: 0x04002670 RID: 9840
		private GameObject lastSelectedObject;
	}
}
