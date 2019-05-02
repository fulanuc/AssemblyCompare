using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FF RID: 1535
	public class LeaderboardController : MonoBehaviour
	{
		// Token: 0x060022A9 RID: 8873 RVA: 0x000A6720 File Offset: 0x000A4920
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
			if (this.noEntryObject)
			{
				this.noEntryObject.SetActive(this.leaderboardInfoList.Count == 0);
			}
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x000A6790 File Offset: 0x000A4990
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

		// Token: 0x060022AB RID: 8875 RVA: 0x000A681C File Offset: 0x000A4A1C
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
				this.stripList[i].isMeImage.enabled = (leaderboardInfo.userSteamID == Client.Instance.SteamId);
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

		// Token: 0x060022AC RID: 8876 RVA: 0x000A69C8 File Offset: 0x000A4BC8
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

		// Token: 0x060022AD RID: 8877 RVA: 0x000A6A34 File Offset: 0x000A4C34
		private void SetLeaderboardInfo(LeaderboardController.LeaderboardInfo[] leaderboardInfos)
		{
			this.leaderboardInfoList.Clear();
			foreach (LeaderboardController.LeaderboardInfo item in leaderboardInfos)
			{
				this.leaderboardInfoList.Add(item);
			}
			this.Rebuild();
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060022AE RID: 8878 RVA: 0x00019470 File Offset: 0x00017670
		// (set) Token: 0x060022AF RID: 8879 RVA: 0x00019478 File Offset: 0x00017678
		public int currentPage { get; private set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060022B0 RID: 8880 RVA: 0x00019481 File Offset: 0x00017681
		// (set) Token: 0x060022B1 RID: 8881 RVA: 0x00019489 File Offset: 0x00017689
		public string currentLeaderboardName { get; private set; }

		// Token: 0x060022B2 RID: 8882 RVA: 0x000A6A78 File Offset: 0x000A4C78
		public void SetRequestType(string requestTypeName)
		{
			LeaderboardController.RequestType requestType;
			if (Enum.TryParse<LeaderboardController.RequestType>(requestTypeName, false, out requestType))
			{
				this.currentRequestType = requestType;
			}
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x000A6A98 File Offset: 0x000A4C98
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

		// Token: 0x060022B4 RID: 8884 RVA: 0x000A6B10 File Offset: 0x000A4D10
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

		// Token: 0x060022B5 RID: 8885 RVA: 0x00019492 File Offset: 0x00017692
		private Action GenerateRequest(Leaderboard leaderboard, LeaderboardController.RequestType callRequestType, int page)
		{
			Leaderboard.FetchScoresCallback <>9__1;
			return delegate()
			{
				if (this.currentLeaderboard != leaderboard)
				{
					return;
				}
				int num = page * this.entriesPerPage - this.entriesPerPage / 2;
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

		// Token: 0x060022B6 RID: 8886 RVA: 0x000194C0 File Offset: 0x000176C0
		private void OrderLeaderboardListByTime(ref List<LeaderboardController.LeaderboardInfo> leaderboardInfoList)
		{
			leaderboardInfoList.Sort(new Comparison<LeaderboardController.LeaderboardInfo>(LeaderboardController.SortByTime));
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x000194D5 File Offset: 0x000176D5
		private static int SortByTime(LeaderboardController.LeaderboardInfo p1, LeaderboardController.LeaderboardInfo p2)
		{
			return p1.timeInSeconds.CompareTo(p2.timeInSeconds);
		}

		// Token: 0x04002568 RID: 9576
		public GameObject stripPrefab;

		// Token: 0x04002569 RID: 9577
		public RectTransform container;

		// Token: 0x0400256A RID: 9578
		public GameObject noEntryObject;

		// Token: 0x0400256B RID: 9579
		public AnimateImageAlpha animateImageAlpha;

		// Token: 0x0400256C RID: 9580
		private List<LeaderboardStrip> stripList = new List<LeaderboardStrip>();

		// Token: 0x0400256D RID: 9581
		private List<LeaderboardController.LeaderboardInfo> leaderboardInfoList = new List<LeaderboardController.LeaderboardInfo>();

		// Token: 0x0400256E RID: 9582
		public MPButton nextPageButton;

		// Token: 0x0400256F RID: 9583
		public MPButton previousPageButton;

		// Token: 0x04002570 RID: 9584
		public MPButton resetPageButton;

		// Token: 0x04002571 RID: 9585
		public int entriesPerPage = 16;

		// Token: 0x04002574 RID: 9588
		public LeaderboardController.RequestType currentRequestType;

		// Token: 0x04002575 RID: 9589
		private Leaderboard currentLeaderboard;

		// Token: 0x04002576 RID: 9590
		private Action queuedRequest;

		// Token: 0x02000600 RID: 1536
		private struct LeaderboardInfo
		{
			// Token: 0x04002577 RID: 9591
			public int rank;

			// Token: 0x04002578 RID: 9592
			public ulong userSteamID;

			// Token: 0x04002579 RID: 9593
			public SurvivorIndex survivorIndex;

			// Token: 0x0400257A RID: 9594
			public float timeInSeconds;

			// Token: 0x0400257B RID: 9595
			public bool isMe;
		}

		// Token: 0x02000601 RID: 1537
		public enum RequestType
		{
			// Token: 0x0400257D RID: 9597
			Global,
			// Token: 0x0400257E RID: 9598
			GlobalAroundUser,
			// Token: 0x0400257F RID: 9599
			Friends
		}
	}
}
