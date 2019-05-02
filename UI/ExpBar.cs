using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D6 RID: 1494
	[RequireComponent(typeof(RectTransform))]
	public class ExpBar : MonoBehaviour
	{
		// Token: 0x06002184 RID: 8580 RVA: 0x00018600 File Offset: 0x00016800
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x000A208C File Offset: 0x000A028C
		public void Update()
		{
			TeamIndex teamIndex = this.source ? this.source.teamIndex : TeamIndex.Neutral;
			float x = 0f;
			if (this.source && TeamManager.instance)
			{
				x = Mathf.InverseLerp(TeamManager.instance.GetTeamCurrentLevelExperience(teamIndex), TeamManager.instance.GetTeamNextLevelExperience(teamIndex), TeamManager.instance.GetTeamExperience(teamIndex));
			}
			if (this.fillRectTransform)
			{
				Rect rect = this.rectTransform.rect;
				Rect rect2 = this.fillRectTransform.rect;
				this.fillRectTransform.anchorMin = new Vector2(0f, 0f);
				this.fillRectTransform.anchorMax = new Vector2(x, 1f);
				this.fillRectTransform.sizeDelta = new Vector2(1f, 1f);
			}
		}

		// Token: 0x04002432 RID: 9266
		public CharacterMaster source;

		// Token: 0x04002433 RID: 9267
		public RectTransform fillRectTransform;

		// Token: 0x04002434 RID: 9268
		private RectTransform rectTransform;
	}
}
