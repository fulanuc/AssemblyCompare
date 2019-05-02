using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DA RID: 1498
	public class CursorIndicatorController : MonoBehaviour
	{
		// Token: 0x060021D2 RID: 8658 RVA: 0x000A2014 File Offset: 0x000A0214
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

		// Token: 0x060021D3 RID: 8659 RVA: 0x000A2090 File Offset: 0x000A0290
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

		// Token: 0x060021D4 RID: 8660 RVA: 0x00018A65 File Offset: 0x00016C65
		public void SetPosition(Vector2 position)
		{
			this.containerTransform.position = position;
		}

		// Token: 0x04002412 RID: 9234
		[NonSerialized]
		public CursorIndicatorController.CursorSet noneCursorSet;

		// Token: 0x04002413 RID: 9235
		public CursorIndicatorController.CursorSet mouseCursorSet;

		// Token: 0x04002414 RID: 9236
		public CursorIndicatorController.CursorSet gamepadCursorSet;

		// Token: 0x04002415 RID: 9237
		private GameObject currentChildIndicator;

		// Token: 0x04002416 RID: 9238
		public RectTransform containerTransform;

		// Token: 0x04002417 RID: 9239
		private Color cachedIndicatorColor = Color.clear;

		// Token: 0x020005DB RID: 1499
		public enum CursorImage
		{
			// Token: 0x04002419 RID: 9241
			None,
			// Token: 0x0400241A RID: 9242
			Pointer,
			// Token: 0x0400241B RID: 9243
			Hover
		}

		// Token: 0x020005DC RID: 1500
		[Serializable]
		public struct CursorSet
		{
			// Token: 0x060021D6 RID: 8662 RVA: 0x00018A8B File Offset: 0x00016C8B
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

			// Token: 0x0400241C RID: 9244
			public GameObject pointerObject;

			// Token: 0x0400241D RID: 9245
			public GameObject hoverObject;
		}
	}
}
