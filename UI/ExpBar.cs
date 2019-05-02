using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E8 RID: 1512
	[RequireComponent(typeof(RectTransform))]
	public class ExpBar : MonoBehaviour
	{
		// Token: 0x06002215 RID: 8725 RVA: 0x00018CFA File Offset: 0x00016EFA
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x000A3660 File Offset: 0x000A1860
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

		// Token: 0x04002486 RID: 9350
		public CharacterMaster source;

		// Token: 0x04002487 RID: 9351
		public RectTransform fillRectTransform;

		// Token: 0x04002488 RID: 9352
		private RectTransform rectTransform;
	}
}
