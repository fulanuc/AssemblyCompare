using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D1 RID: 1489
	public class DisplayStock : MonoBehaviour
	{
		// Token: 0x0600216D RID: 8557 RVA: 0x0001852B File Offset: 0x0001672B
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x000A1A88 File Offset: 0x0009FC88
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

		// Token: 0x0400240E RID: 9230
		public SkillSlot skillSlot;

		// Token: 0x0400240F RID: 9231
		public Image[] stockImages;

		// Token: 0x04002410 RID: 9232
		public Sprite fullStockSprite;

		// Token: 0x04002411 RID: 9233
		public Color fullStockColor;

		// Token: 0x04002412 RID: 9234
		public Sprite emptyStockSprite;

		// Token: 0x04002413 RID: 9235
		public Color emptyStockColor;

		// Token: 0x04002414 RID: 9236
		private HudElement hudElement;

		// Token: 0x04002415 RID: 9237
		private SkillLocator skillLocator;
	}
}
