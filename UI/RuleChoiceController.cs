using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063B RID: 1595
	public class RuleChoiceController : MonoBehaviour
	{
		// Token: 0x060023FF RID: 9215 RVA: 0x0001A466 File Offset: 0x00018666
		private void OnEnable()
		{
			RuleChoiceController.instancesList.Add(this);
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x0001A473 File Offset: 0x00018673
		private void OnDisable()
		{
			RuleChoiceController.instancesList.Remove(this);
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x0001A481 File Offset: 0x00018681
		static RuleChoiceController()
		{
			PreGameRuleVoteController.onVotesUpdated += delegate()
			{
				foreach (RuleChoiceController ruleChoiceController in RuleChoiceController.instancesList)
				{
					ruleChoiceController.UpdateFromVotes();
				}
			};
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x0001A4A2 File Offset: 0x000186A2
		private void Start()
		{
			this.UpdateFromVotes();
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x000AB0D0 File Offset: 0x000A92D0
		public void UpdateFromVotes()
		{
			int num = PreGameRuleVoteController.votesForEachChoice[this.currentChoiceDef.globalIndex];
			bool isInSinglePlayer = RoR2Application.isInSinglePlayer;
			if (num > 0 && !isInSinglePlayer)
			{
				this.voteCounter.enabled = true;
				this.voteCounter.text = num.ToString();
			}
			else
			{
				this.voteCounter.enabled = false;
			}
			bool enabled = false;
			NetworkUser networkUser = this.FindNetworkUser();
			if (networkUser)
			{
				PreGameRuleVoteController preGameRuleVoteController = PreGameRuleVoteController.FindForUser(networkUser);
				if (preGameRuleVoteController)
				{
					enabled = preGameRuleVoteController.IsChoiceVoted(this.currentChoiceDef);
				}
			}
			this.selectionDisplayPanel.enabled = enabled;
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x000AB164 File Offset: 0x000A9364
		public void SetChoice([NotNull] RuleChoiceDef newChoiceDef)
		{
			if (newChoiceDef == this.currentChoiceDef)
			{
				return;
			}
			this.currentChoiceDef = newChoiceDef;
			base.gameObject.name = "Choice (" + this.currentChoiceDef.globalName + ")";
			this.image.material = ((this.currentChoiceDef.materialPath == null) ? null : Resources.Load<Material>(this.currentChoiceDef.materialPath));
			this.image.sprite = Resources.Load<Sprite>(this.currentChoiceDef.spritePath);
			this.tooltipProvider.titleToken = this.currentChoiceDef.tooltipNameToken;
			this.tooltipProvider.titleColor = this.currentChoiceDef.tooltipNameColor;
			this.tooltipProvider.bodyToken = this.currentChoiceDef.tooltipBodyToken;
			this.tooltipProvider.bodyColor = this.currentChoiceDef.tooltipBodyColor;
			this.UpdateFromVotes();
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x0001A4AA File Offset: 0x000186AA
		private NetworkUser FindNetworkUser()
		{
			return ((MPEventSystem)EventSystem.current).localUser.currentNetworkUser;
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x000AB24C File Offset: 0x000A944C
		public void OnClick()
		{
			if (!this.canVote)
			{
				return;
			}
			NetworkUser networkUser = this.FindNetworkUser();
			Debug.Log(networkUser);
			if (networkUser)
			{
				PreGameRuleVoteController preGameRuleVoteController = PreGameRuleVoteController.FindForUser(networkUser);
				if (preGameRuleVoteController)
				{
					int choiceValue = this.currentChoiceDef.localIndex;
					if (preGameRuleVoteController.IsChoiceVoted(this.currentChoiceDef))
					{
						choiceValue = -1;
					}
					preGameRuleVoteController.SetVote(this.currentChoiceDef.ruleDef.globalIndex, choiceValue);
					Debug.LogFormat("voteController.SetVote({0}, {1})", new object[]
					{
						this.currentChoiceDef.ruleDef.globalIndex,
						this.currentChoiceDef.localIndex
					});
					return;
				}
				Debug.Log("voteController=null");
			}
		}

		// Token: 0x040026B2 RID: 9906
		private static readonly List<RuleChoiceController> instancesList = new List<RuleChoiceController>();

		// Token: 0x040026B3 RID: 9907
		[HideInInspector]
		public RuleBookViewerStrip strip;

		// Token: 0x040026B4 RID: 9908
		public Image image;

		// Token: 0x040026B5 RID: 9909
		public TooltipProvider tooltipProvider;

		// Token: 0x040026B6 RID: 9910
		public TextMeshProUGUI voteCounter;

		// Token: 0x040026B7 RID: 9911
		public Image selectionDisplayPanel;

		// Token: 0x040026B8 RID: 9912
		public bool canVote;

		// Token: 0x040026B9 RID: 9913
		private RuleChoiceDef currentChoiceDef;
	}
}
