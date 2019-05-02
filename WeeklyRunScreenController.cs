using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000655 RID: 1621
	public class WeeklyRunScreenController : MonoBehaviour
	{
		// Token: 0x06002448 RID: 9288 RVA: 0x0001A6AF File Offset: 0x000188AF
		private void OnEnable()
		{
			this.currentCycle = WeeklyRun.GetCurrentSeedCycle();
			this.UpdateLeaderboard();
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x0001A6C2 File Offset: 0x000188C2
		private void UpdateLeaderboard()
		{
			this.leaderboard.SetRequestedInfo(WeeklyRun.GetLeaderboardName(1, this.currentCycle), this.leaderboard.currentRequestType, this.leaderboard.currentPage);
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x000AC7A4 File Offset: 0x000AA9A4
		private void Update()
		{
			uint currentSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			if (currentSeedCycle != this.currentCycle)
			{
				this.currentCycle = currentSeedCycle;
				this.UpdateLeaderboard();
			}
			TimeSpan t = WeeklyRun.GetSeedCycleStartDateTime(this.currentCycle + 1u) - WeeklyRun.now;
			string @string = Language.GetString("WEEKLY_RUN_NEXT_CYCLE_COUNTDOWN_FORMAT");
			this.countdownLabel.text = string.Format(@string, t.Hours + t.Days * 24, t.Minutes, t.Seconds);
			if (t != this.lastCountdown)
			{
				this.lastCountdown = t;
				this.labelFadeValue = 0f;
			}
			this.labelFadeValue = Mathf.Max(this.labelFadeValue + Time.deltaTime * 1f, 0f);
			Color white = Color.white;
			if (t.Days == 0 && t.Hours == 0)
			{
				white.g = this.labelFadeValue;
				white.b = this.labelFadeValue;
			}
			this.countdownLabel.color = white;
		}

		// Token: 0x04002735 RID: 10037
		public LeaderboardController leaderboard;

		// Token: 0x04002736 RID: 10038
		public TextMeshProUGUI countdownLabel;

		// Token: 0x04002737 RID: 10039
		private uint currentCycle;

		// Token: 0x04002738 RID: 10040
		private TimeSpan lastCountdown;

		// Token: 0x04002739 RID: 10041
		private float labelFadeValue;
	}
}
