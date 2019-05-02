using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000665 RID: 1637
	public class VoteInfoPanelController : MonoBehaviour
	{
		// Token: 0x060024D3 RID: 9427 RVA: 0x0001AD57 File Offset: 0x00018F57
		private void Awake()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x000ADBF4 File Offset: 0x000ABDF4
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

		// Token: 0x060024D5 RID: 9429 RVA: 0x000ADCB0 File Offset: 0x000ABEB0
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

		// Token: 0x060024D6 RID: 9430 RVA: 0x0001AD6C File Offset: 0x00018F6C
		private void Update()
		{
			this.UpdateElements();
		}

		// Token: 0x04002785 RID: 10117
		public GameObject indicatorPrefab;

		// Token: 0x04002786 RID: 10118
		public Sprite hasNotVotedSprite;

		// Token: 0x04002787 RID: 10119
		public Sprite hasVotedSprite;

		// Token: 0x04002788 RID: 10120
		public RectTransform container;

		// Token: 0x04002789 RID: 10121
		public GameObject timerPanelObject;

		// Token: 0x0400278A RID: 10122
		public TextMeshProUGUI timerLabel;

		// Token: 0x0400278B RID: 10123
		public VoteController voteController;

		// Token: 0x0400278C RID: 10124
		private readonly List<VoteInfoPanelController.IndicatorInfo> indicators = new List<VoteInfoPanelController.IndicatorInfo>();

		// Token: 0x02000666 RID: 1638
		private struct IndicatorInfo
		{
			// Token: 0x0400278D RID: 10125
			public GameObject gameObject;

			// Token: 0x0400278E RID: 10126
			public Image image;

			// Token: 0x0400278F RID: 10127
			public TooltipProvider tooltipProvider;
		}
	}
}
