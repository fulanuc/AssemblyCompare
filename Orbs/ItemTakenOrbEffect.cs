using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000513 RID: 1299
	[RequireComponent(typeof(EffectComponent))]
	public class ItemTakenOrbEffect : MonoBehaviour
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x0008FFD0 File Offset: 0x0008E1D0
		private void Start()
		{
			ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)(base.GetComponent<EffectComponent>().effectData.genericUInt - 1u));
			ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.Error;
			Sprite sprite = null;
			if (itemDef != null)
			{
				colorIndex = itemDef.colorIndex;
				sprite = itemDef.pickupIconSprite;
			}
			this.trailToColor.endColor = (this.trailToColor.startColor = ColorCatalog.GetColor(colorIndex));
			this.iconSpriteRenderer.sprite = sprite;
		}

		// Token: 0x04001F76 RID: 8054
		public TrailRenderer trailToColor;

		// Token: 0x04001F77 RID: 8055
		public SpriteRenderer iconSpriteRenderer;
	}
}
