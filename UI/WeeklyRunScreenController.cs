using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000667 RID: 1639
	public class WeeklyRunScreenController : MonoBehaviour
	{
		// Token: 0x060024D8 RID: 9432 RVA: 0x0001AD87 File Offset: 0x00018F87
		private void OnEnable()
		{
			this.currentCycle = WeeklyRun.GetCurrentSeedCycle();
			this.UpdateLeaderboard();
		}

		// Token: 0x060024D9 RID: 9433 RVA: 0x0001AD9A File Offset: 0x00018F9A
		private void UpdateLeaderboard()
		{
			if (this.leaderboard)
			{
				this.leaderboard.SetRequestedInfo(WeeklyRun.GetLeaderboardName(1, this.currentCycle), this.leaderboard.currentRequestType, this.leaderboard.currentPage);
			}
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x0001ADD6 File Offset: 0x00018FD6
		public void SetCurrentLeaderboard(GameObject leaderboardGameObject)
		{
			this.leaderboard = leaderboardGameObject.GetComponent<LeaderboardController>();
			this.UpdateLeaderboard();
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x000ADE24 File Offset: 0x000AC024
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

		// Token: 0x04002790 RID: 10128
		public LeaderboardController leaderboard;

		// Token: 0x04002791 RID: 10129
		public TextMeshProUGUI countdownLabel;

		// Token: 0x04002792 RID: 10130
		private uint currentCycle;

		// Token: 0x04002793 RID: 10131
		private TimeSpan lastCountdown;

		// Token: 0x04002794 RID: 10132
		private float labelFadeValue;
	}
}
