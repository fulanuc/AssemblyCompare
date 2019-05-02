using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000653 RID: 1619
	public class VoteInfoPanelController : MonoBehaviour
	{
		// Token: 0x06002443 RID: 9283 RVA: 0x0001A67F File Offset: 0x0001887F
		private void Awake()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x000AC574 File Offset: 0x000AA774
		private void AllocateIndicators(int desiredIndicatorCount)
		{
			while (this.indicators.Count > desiredIndicatorCount)
			{
				int index = this.indicators.Count - 1;
				UnityEngine.Object.Destroy(this.indicators[index].gameObject);
				this.indicators.RemoveAt(index);
			}
			while (this.indicators.Count < desiredIndicatorCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.indicatorPrefab, this.container);
				gameObject.SetActive(true);
				this.indicators.Add(new VoteInfoPanelController.IndicatorInfo
				{
					gameObject = gameObject,
					image = gameObject.GetComponentInChildren<Image>(),
					tooltipProvider = gameObject.GetComponentInChildren<TooltipProvider>()
				});
			}
			this.timerPanelObject.transform.SetAsLastSibling();
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x000AC630 File Offset: 0x000AA830
		public void UpdateElements()
		{
			int num = 0;
			if (this.voteController)
			{
				num = this.voteController.GetVoteCount();
			}
			this.AllocateIndicators(num);
			for (int i = 0; i < num; i++)
			{
				VoteController.UserVote vote = this.voteController.GetVote(i);
				this.indicators[i].image.sprite = (vote.receivedVote ? this.hasVotedSprite : this.hasNotVotedSprite);
				string userName;
				if (vote.networkUserObject)
				{
					NetworkUser component = vote.networkUserObject.GetComponent<NetworkUser>();
					if (component)
					{
						userName = component.GetNetworkPlayerName().GetResolvedName();
					}
					else
					{
						userName = Language.GetString("PLAYER_NAME_UNAVAILABLE");
					}
				}
				else
				{
					userName = Language.GetString("PLAYER_NAME_DISCONNECTED");
				}
				this.indicators[i].tooltipProvider.SetContent(TooltipProvider.GetPlayerNameTooltipContent(userName));
			}
			bool flag = this.voteController && this.voteController.timerStartCondition != VoteController.TimerStartCondition.Never;
			this.timerPanelObject.SetActive(flag);
			if (flag)
			{
				float num2 = this.voteController.timer;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				int num3 = Mathf.FloorToInt(num2 * 0.0166666675f);
				int num4 = (int)num2 - num3 * 60;
				this.timerLabel.text = string.Format("{0}:{1:00}", num3, num4);
			}
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x0001A694 File Offset: 0x00018894
		private void Update()
		{
			this.UpdateElements();
		}

		// Token: 0x0400272A RID: 10026
		public GameObject indicatorPrefab;

		// Token: 0x0400272B RID: 10027
		public Sprite hasNotVotedSprite;

		// Token: 0x0400272C RID: 10028
		public Sprite hasVotedSprite;

		// Token: 0x0400272D RID: 10029
		public RectTransform container;

		// Token: 0x0400272E RID: 10030
		public GameObject timerPanelObject;

		// Token: 0x0400272F RID: 10031
		public TextMeshProUGUI timerLabel;

		// Token: 0x04002730 RID: 10032
		public VoteController voteController;

		// Token: 0x04002731 RID: 10033
		private readonly List<VoteInfoPanelController.IndicatorInfo> indicators = new List<VoteInfoPanelController.IndicatorInfo>();

		// Token: 0x02000654 RID: 1620
		private struct IndicatorInfo
		{
			// Token: 0x04002732 RID: 10034
			public GameObject gameObject;

			// Token: 0x04002733 RID: 10035
			public Image image;

			// Token: 0x04002734 RID: 10036
			public TooltipProvider tooltipProvider;
		}
	}
}
