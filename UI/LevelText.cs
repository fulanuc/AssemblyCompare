using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F5 RID: 1525
	[RequireComponent(typeof(RectTransform))]
	public class LevelText : MonoBehaviour
	{
		// Token: 0x06002235 RID: 8757 RVA: 0x000A5694 File Offset: 0x000A3894
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

		// Token: 0x06002236 RID: 8758 RVA: 0x00018ED4 File Offset: 0x000170D4
		private void Update()
		{
			if (this.source && TeamManager.instance)
			{
				this.SetDisplayData(TeamManager.instance.GetTeamLevel(this.source.teamIndex));
			}
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x00018F0A File Offset: 0x0001710A
		private void OnValidate()
		{
			if (!this.targetText)
			{
				Debug.LogError("targetText must be assigned.");
			}
		}

		// Token: 0x0400253E RID: 9534
		public CharacterMaster source;

		// Token: 0x0400253F RID: 9535
		public TextMeshProUGUI targetText;

		// Token: 0x04002540 RID: 9536
		private uint displayData;

		// Token: 0x04002541 RID: 9537
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
