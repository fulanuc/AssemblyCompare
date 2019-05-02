using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000522 RID: 1314
	[RequireComponent(typeof(EffectComponent))]
	public class ItemTakenOrbEffect : MonoBehaviour
	{
		// Token: 0x06001DBF RID: 7615 RVA: 0x00090D44 File Offset: 0x0008EF44
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

		// Token: 0x04001FB4 RID: 8116
		public TrailRenderer trailToColor;

		// Token: 0x04001FB5 RID: 8117
		public SpriteRenderer iconSpriteRenderer;
	}
}
