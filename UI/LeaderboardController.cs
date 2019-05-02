using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005ED RID: 1517
	public class LeaderboardController : MonoBehaviour
	{
		// Token: 0x06002218 RID: 8728 RVA: 0x00018D76 File Offset: 0x00016F76
		private void Update()
		{
			if (this.currentLeaderboard != null && this.currentLeaderboard.IsValid && !this.currentLeaderboard.IsQuerying)
			{
				Action action = this.queuedRequest;
				if (action != null)
				{
					action();
				}
				this.queuedRequest = null;
			}
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000A516C File Offset: 0x000A336C
		private void SetStripCount(int newCount)
		{
			while (this.stripList.Count > newCount)
			{
				UnityEngine.Object.Destroy(this.stripList[this.stripList.Count - 1].gameObject);
				this.stripList.RemoveAt(this.stripList.Count - 1);
			}
			while (this.stripList.Count < newCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.stripPrefab, this.container);
				this.stripList.Add(gameObject.GetComponent<LeaderboardStrip>());
			}
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x000A51F8 File Offset: 0x000A33F8
		private void Rebuild()
		{
			this.SetStripCount(this.leaderboardInfoList.Count);
			for (int i = 0; i < this.leaderboardInfoList.Count; i++)
			{
				LeaderboardController.LeaderboardInfo leaderboardInfo = this.leaderboardInfoList[i];
				int num = Mathf.FloorToInt(leaderboardInfo.timeInSeconds / 60f);
				float num2 = leaderboardInfo.timeInSeconds - (float)(num * 60);
				string text = string.Format("{0:0}:{1:00.00}", num, num2);
				this.stripList[i].rankLabel.text = leaderboardInfo.rank.ToString();
				this.stripList[i].usernameLabel.userSteamId = leaderboardInfo.userSteamID;
				this.stripList[i].timeLabel.text = text;
				this.stripList[i].classIcon.texture = SurvivorCatalog.GetSurvivorPortrait(leaderboardInfo.survivorIndex);
				this.stripList[i].usernameLabel.Refresh();
			}
			if (this.animateImageAlpha)
			{
				UnityEngine.UI.Image[] array = new UnityEngine.UI.Image[this.stripList.Count];
				for (int j = 0; j < this.stripList.Count; j++)
				{
					array[this.stripList.Count - 1 - j] = this.stripList[j].GetComponent<UnityEngine.UI.Image>();
				}
				this.animateImageAlpha.ResetStopwatch();
				this.animateImageAlpha.images = array;
			}
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x000A537C File Offset: 0x000A357C
		private void GenerateFakeLeaderboardList(int count)
		{
			this.leaderboardInfoList.Clear();
			for (int i = 1; i <= count; i++)
			{
				LeaderboardController.LeaderboardInfo item = default(LeaderboardController.LeaderboardInfo);
				item.userSteamID = 76561197995890564UL;
				item.survivorIndex = (SurvivorIndex)UnityEngine.Random.Range(0, 6);
				item.timeInSeconds = UnityEngine.Random.Range(120f, 600f);
				this.leaderboardInfoList.Add(item);
			}
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x000A53E8 File Offset: 0x000A35E8
		private void SetLeaderboardInfo(LeaderboardController.LeaderboardInfo[] leaderboardInfos)
		{
			this.leaderboardInfoList.Clear();
			foreach (LeaderboardController.LeaderboardInfo item in leaderboardInfos)
			{
				this.leaderboardInfoList.Add(item);
			}
			this.Rebuild();
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600221D RID: 8733 RVA: 0x00018DB2 File Offset: 0x00016FB2
		// (set) Token: 0x0600221E RID: 8734 RVA: 0x00018DBA File Offset: 0x00016FBA
		public int currentPage { get; private set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x0600221F RID: 8735 RVA: 0x00018DC3 File Offset: 0x00016FC3
		// (set) Token: 0x06002220 RID: 8736 RVA: 0x00018DCB File Offset: 0x00016FCB
		public string currentLeaderboardName { get; private set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06002221 RID: 8737 RVA: 0x00018DD4 File Offset: 0x00016FD4
		// (set) Token: 0x06002222 RID: 8738 RVA: 0x00018DDC File Offset: 0x00016FDC
		public LeaderboardController.RequestType currentRequestType { get; private set; }

		// Token: 0x06002223 RID: 8739 RVA: 0x000A542C File Offset: 0x000A362C
		private static LeaderboardController.LeaderboardInfo LeaderboardInfoFromSteamLeaderboardEntry(Leaderboard.Entry entry)
		{
			SurvivorIndex survivorIndex = SurvivorIndex.None;
			int num = (entry.SubScores != null && entry.SubScores.Length >= 1) ? entry.SubScores[1] : 0;
			if (num >= 0 && num < 7)
			{
				survivorIndex = (SurvivorIndex)num;
			}
			return new LeaderboardController.LeaderboardInfo
			{
				timeInSeconds = (float)entry.Score * 0.001f,
				survivorIndex = survivorIndex,
				userSteamID = entry.SteamId,
				rank = entry.GlobalRank
			};
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x000A54A4 File Offset: 0x000A36A4
		public void SetRequestedInfo(string newLeaderboardName, LeaderboardController.RequestType newRequestType, int newPage)
		{
			bool flag = this.currentLeaderboardName != newLeaderboardName;
			if (flag)
			{
				this.currentLeaderboardName = newLeaderboardName;
				this.currentLeaderboard = Client.Instance.GetLeaderboard(this.currentLeaderboardName, Client.LeaderboardSortMethod.None, Client.LeaderboardDisplayType.None);
				newPage = 0;
			}
			bool flag2 = this.currentRequestType != newRequestType || flag;
			bool flag3 = this.currentPage != newPage || flag;
			if (flag2)
			{
				this.currentRequestType = newRequestType;
			}
			if (flag3)
			{
				this.currentPage = newPage;
			}
			if (flag || flag2 || flag3)
			{
				this.queuedRequest = this.GenerateRequest(this.currentLeaderboard, newRequestType, newPage);
			}
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x00018DE5 File Offset: 0x00016FE5
		private Action GenerateRequest(Leaderboard leaderboard, LeaderboardController.RequestType callRequestType, int page)
		{
			Leaderboard.FetchScoresCallback <>9__1;
			return delegate()
			{
				if (this.currentLeaderboard != leaderboard)
				{
					return;
				}
				int num = page * this.entriesPerPage;
				Leaderboard leaderboard2 = this.currentLeaderboard;
				Leaderboard.RequestType callRequestType2 = (Leaderboard.RequestType)callRequestType;
				int start = num;
				int end = num + this.entriesPerPage;
				Leaderboard.FetchScoresCallback onSuccess;
				if ((onSuccess = <>9__1) == null)
				{
					onSuccess = (<>9__1 = delegate(Leaderboard.Entry[] entries)
					{
						this.SetLeaderboardInfo(entries.Select(new Func<Leaderboard.Entry, LeaderboardController.LeaderboardInfo>(LeaderboardController.LeaderboardInfoFromSteamLeaderboardEntry)).ToArray<LeaderboardController.LeaderboardInfo>());
					});
				}
				leaderboard2.FetchScores(callRequestType2, start, end, onSuccess, delegate(Result reason)
				{
				});
			};
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x00018E13 File Offset: 0x00017013
		private void OrderLeaderboardListByTime(ref List<LeaderboardController.LeaderboardInfo> leaderboardInfoList)
		{
			leaderboardInfoList.Sort(new Comparison<LeaderboardController.LeaderboardInfo>(LeaderboardController.SortByTime));
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x00018E28 File Offset: 0x00017028
		private static int SortByTime(LeaderboardController.LeaderboardInfo p1, LeaderboardController.LeaderboardInfo p2)
		{
			return p1.timeInSeconds.CompareTo(p2.timeInSeconds);
		}

		// Token: 0x04002513 RID: 9491
		public GameObject stripPrefab;

		// Token: 0x04002514 RID: 9492
		public RectTransform container;

		// Token: 0x04002515 RID: 9493
		public AnimateImageAlpha animateImageAlpha;

		// Token: 0x04002516 RID: 9494
		private List<LeaderboardStrip> stripList = new List<LeaderboardStrip>();

		// Token: 0x04002517 RID: 9495
		private List<LeaderboardController.LeaderboardInfo> leaderboardInfoList = new List<LeaderboardController.LeaderboardInfo>();

		// Token: 0x04002518 RID: 9496
		public int entriesPerPage = 16;

		// Token: 0x0400251C RID: 9500
		private Leaderboard currentLeaderboard;

		// Token: 0x0400251D RID: 9501
		private Action queuedRequest;

		// Token: 0x020005EE RID: 1518
		private struct LeaderboardInfo
		{
			// Token: 0x0400251E RID: 9502
			public int rank;

			// Token: 0x0400251F RID: 9503
			public ulong userSteamID;

			// Token: 0x04002520 RID: 9504
			public SurvivorIndex survivorIndex;

			// Token: 0x04002521 RID: 9505
			public float timeInSeconds;
		}

		// Token: 0x020005EF RID: 1519
		public enum RequestType
		{
			// Token: 0x04002523 RID: 9507
			Global,
			// Token: 0x04002524 RID: 9508
			GlobalAroundUser,
			// Token: 0x04002525 RID: 9509
			Friends
		}
	}
}
