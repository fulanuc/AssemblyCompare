using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000311 RID: 785
	public class Highlight : MonoBehaviour
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06001063 RID: 4195 RVA: 0x0000C879 File Offset: 0x0000AA79
		public static ReadOnlyCollection<Highlight> readonlyHighlightList
		{
			get
			{
				return Highlight._readonlyHighlightList;
			}
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0000C880 File Offset: 0x0000AA80
		private void Awake()
		{
			this.displayNameProvider = base.GetComponent<IDisplayNameProvider>();
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0000C88E File Offset: 0x0000AA8E
		public void OnEnable()
		{
			Highlight.highlightList.Add(this);
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0000C89B File Offset: 0x0000AA9B
		public void OnDisable()
		{
			Highlight.highlightList.Remove(this);
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0006226C File Offset: 0x0006046C
		public Color GetColor()
		{
			switch (this.highlightColor)
			{
			case Highlight.HighlightColor.interactive:
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Interactable);
			case Highlight.HighlightColor.teleporter:
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			case Highlight.HighlightColor.pickup:
				return this.pickupIndex.GetPickupColor();
			default:
				return Color.magenta;
			}
		}

		// Token: 0x04001444 RID: 5188
		private static List<Highlight> highlightList = new List<Highlight>();

		// Token: 0x04001445 RID: 5189
		private static ReadOnlyCollection<Highlight> _readonlyHighlightList = new ReadOnlyCollection<Highlight>(Highlight.highlightList);

		// Token: 0x04001446 RID: 5190
		private IDisplayNameProvider displayNameProvider;

		// Token: 0x04001447 RID: 5191
		[HideInInspector]
		public PickupIndex pickupIndex;

		// Token: 0x04001448 RID: 5192
		public Renderer targetRenderer;

		// Token: 0x04001449 RID: 5193
		public float strength = 1f;

		// Token: 0x0400144A RID: 5194
		public Highlight.HighlightColor highlightColor;

		// Token: 0x0400144B RID: 5195
		public bool isOn;

		// Token: 0x02000312 RID: 786
		public enum HighlightColor
		{
			// Token: 0x0400144D RID: 5197
			interactive,
			// Token: 0x0400144E RID: 5198
			teleporter,
			// Token: 0x0400144F RID: 5199
			pickup,
			// Token: 0x04001450 RID: 5200
			unavailable
		}
	}
}
