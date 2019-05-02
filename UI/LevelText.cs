using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000607 RID: 1543
	[RequireComponent(typeof(RectTransform))]
	public class LevelText : MonoBehaviour
	{
		// Token: 0x060022C5 RID: 8901 RVA: 0x000A6D10 File Offset: 0x000A4F10
		private void SetDisplayData(uint newDisplayData)
		{
			if (this.displayData == newDisplayData)
			{
				return;
			}
			this.displayData = newDisplayData;
			uint value = this.displayData;
			LevelText.sharedStringBuilder.Clear();
			LevelText.sharedStringBuilder.AppendUint(value, 0u, uint.MaxValue);
			this.targetText.SetText(LevelText.sharedStringBuilder);
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x00019581 File Offset: 0x00017781
		private void Update()
		{
			if (this.source && TeamManager.instance)
			{
				this.SetDisplayData(TeamManager.instance.GetTeamLevel(this.source.teamIndex));
			}
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x000195B7 File Offset: 0x000177B7
		private void OnValidate()
		{
			if (!this.targetText)
			{
				Debug.LogError("targetText must be assigned.");
			}
		}

		// Token: 0x04002599 RID: 9625
		public CharacterMaster source;

		// Token: 0x0400259A RID: 9626
		public TextMeshProUGUI targetText;

		// Token: 0x0400259B RID: 9627
		private uint displayData;

		// Token: 0x0400259C RID: 9628
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
