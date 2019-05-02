using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200030F RID: 783
	public class Highlight : MonoBehaviour
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x0000C795 File Offset: 0x0000A995
		public static ReadOnlyCollection<Highlight> readonlyHighlightList
		{
			get
			{
				return Highlight._readonlyHighlightList;
			}
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x0000C79C File Offset: 0x0000A99C
		private void Awake()
		{
			this.displayNameProvider = base.GetComponent<IDisplayNameProvider>();
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0000C7AA File Offset: 0x0000A9AA
		public void OnEnable()
		{
			Highlight.highlightList.Add(this);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x0000C7B7 File Offset: 0x0000A9B7
		public void OnDisable()
		{
			Highlight.highlightList.Remove(this);
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00061FC8 File Offset: 0x000601C8
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

		// Token: 0x04001430 RID: 5168
		private static List<Highlight> highlightList = new List<Highlight>();

		// Token: 0x04001431 RID: 5169
		private static ReadOnlyCollection<Highlight> _readonlyHighlightList = new ReadOnlyCollection<Highlight>(Highlight.highlightList);

		// Token: 0x04001432 RID: 5170
		private IDisplayNameProvider displayNameProvider;

		// Token: 0x04001433 RID: 5171
		[HideInInspector]
		public PickupIndex pickupIndex;

		// Token: 0x04001434 RID: 5172
		public Renderer targetRenderer;

		// Token: 0x04001435 RID: 5173
		public float strength = 1f;

		// Token: 0x04001436 RID: 5174
		public Highlight.HighlightColor highlightColor;

		// Token: 0x04001437 RID: 5175
		public bool isOn;

		// Token: 0x02000310 RID: 784
		public enum HighlightColor
		{
			// Token: 0x04001439 RID: 5177
			interactive,
			// Token: 0x0400143A RID: 5178
			teleporter,
			// Token: 0x0400143B RID: 5179
			pickup,
			// Token: 0x0400143C RID: 5180
			unavailable
		}
	}
}
