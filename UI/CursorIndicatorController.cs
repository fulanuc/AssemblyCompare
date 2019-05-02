using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C8 RID: 1480
	public class CursorIndicatorController : MonoBehaviour
	{
		// Token: 0x06002141 RID: 8513 RVA: 0x000A0A40 File Offset: 0x0009EC40
		public void SetCursor(CursorIndicatorController.CursorSet cursorSet, CursorIndicatorController.CursorImage cursorImage, Color color)
		{
			GameObject gameObject = cursorSet.GetGameObject(cursorImage);
			bool flag = color != this.cachedIndicatorColor;
			if (gameObject != this.currentChildIndicator)
			{
				if (this.currentChildIndicator)
				{
					this.currentChildIndicator.SetActive(false);
				}
				this.currentChildIndicator = gameObject;
				if (this.currentChildIndicator)
				{
					this.currentChildIndicator.SetActive(true);
				}
				flag = true;
			}
			if (flag)
			{
				this.cachedIndicatorColor = color;
				this.ApplyIndicatorColor();
			}
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x000A0ABC File Offset: 0x0009ECBC
		private void ApplyIndicatorColor()
		{
			if (!this.currentChildIndicator)
			{
				return;
			}
			Image[] componentsInChildren = this.currentChildIndicator.GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].color = this.cachedIndicatorColor;
			}
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x0001836B File Offset: 0x0001656B
		public void SetPosition(Vector2 position)
		{
			this.containerTransform.position = position;
		}

		// Token: 0x040023BE RID: 9150
		[NonSerialized]
		public CursorIndicatorController.CursorSet noneCursorSet;

		// Token: 0x040023BF RID: 9151
		public CursorIndicatorController.CursorSet mouseCursorSet;

		// Token: 0x040023C0 RID: 9152
		public CursorIndicatorController.CursorSet gamepadCursorSet;

		// Token: 0x040023C1 RID: 9153
		private GameObject currentChildIndicator;

		// Token: 0x040023C2 RID: 9154
		public RectTransform containerTransform;

		// Token: 0x040023C3 RID: 9155
		private Color cachedIndicatorColor = Color.clear;

		// Token: 0x020005C9 RID: 1481
		public enum CursorImage
		{
			// Token: 0x040023C5 RID: 9157
			None,
			// Token: 0x040023C6 RID: 9158
			Pointer,
			// Token: 0x040023C7 RID: 9159
			Hover
		}

		// Token: 0x020005CA RID: 1482
		[Serializable]
		public struct CursorSet
		{
			// Token: 0x06002145 RID: 8517 RVA: 0x00018391 File Offset: 0x00016591
			public GameObject GetGameObject(CursorIndicatorController.CursorImage cursorImage)
			{
				switch (cursorImage)
				{
				case CursorIndicatorController.CursorImage.None:
					return null;
				case CursorIndicatorController.CursorImage.Pointer:
					return this.pointerObject;
				case CursorIndicatorController.CursorImage.Hover:
					return this.hoverObject;
				default:
					return null;
				}
			}

			// Token: 0x040023C8 RID: 9160
			public GameObject pointerObject;

			// Token: 0x040023C9 RID: 9161
			public GameObject hoverObject;
		}
	}
}
