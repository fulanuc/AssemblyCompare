using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E3 RID: 1507
	public class DisplayStock : MonoBehaviour
	{
		// Token: 0x060021FE RID: 8702 RVA: 0x00018C25 File Offset: 0x00016E25
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x000A305C File Offset: 0x000A125C
		private void Update()
		{
			if (this.hudElement.targetCharacterBody)
			{
				if (!this.skillLocator)
				{
					this.skillLocator = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
				}
				if (this.skillLocator)
				{
					GenericSkill skill = this.skillLocator.GetSkill(this.skillSlot);
					if (skill)
					{
						for (int i = 0; i < this.stockImages.Length; i++)
						{
							if (skill.stock > i)
							{
								this.stockImages[i].sprite = this.fullStockSprite;
								this.stockImages[i].color = this.fullStockColor;
							}
							else
							{
								this.stockImages[i].sprite = this.emptyStockSprite;
								this.stockImages[i].color = this.emptyStockColor;
							}
						}
					}
				}
			}
		}

		// Token: 0x04002462 RID: 9314
		public SkillSlot skillSlot;

		// Token: 0x04002463 RID: 9315
		public Image[] stockImages;

		// Token: 0x04002464 RID: 9316
		public Sprite fullStockSprite;

		// Token: 0x04002465 RID: 9317
		public Color fullStockColor;

		// Token: 0x04002466 RID: 9318
		public Sprite emptyStockSprite;

		// Token: 0x04002467 RID: 9319
		public Color emptyStockColor;

		// Token: 0x04002468 RID: 9320
		private HudElement hudElement;

		// Token: 0x04002469 RID: 9321
		private SkillLocator skillLocator;
	}
}
