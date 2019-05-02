using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000629 RID: 1577
	public class RuleChoiceController : MonoBehaviour
	{
		// Token: 0x0600236F RID: 9071 RVA: 0x00019D98 File Offset: 0x00017F98
		private void OnEnable()
		{
			RuleChoiceController.instancesList.Add(this);
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x00019DA5 File Offset: 0x00017FA5
		private void OnDisable()
		{
			RuleChoiceController.instancesList.Remove(this);
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x00019DB3 File Offset: 0x00017FB3
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

		// Token: 0x06002372 RID: 9074 RVA: 0x00019DD4 File Offset: 0x00017FD4
		private void Start()
		{
			this.UpdateFromVotes();
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x000A9A54 File Offset: 0x000A7C54
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

		// Token: 0x06002374 RID: 9076 RVA: 0x000A9AE8 File Offset: 0x000A7CE8
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

		// Token: 0x06002375 RID: 9077 RVA: 0x00019DDC File Offset: 0x00017FDC
		private NetworkUser FindNetworkUser()
		{
			return ((MPEventSystem)EventSystem.current).localUser.currentNetworkUser;
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x000A9BD0 File Offset: 0x000A7DD0
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

		// Token: 0x04002657 RID: 9815
		private static readonly List<RuleChoiceController> instancesList = new List<RuleChoiceController>();

		// Token: 0x04002658 RID: 9816
		[HideInInspector]
		public RuleBookViewerStrip strip;

		// Token: 0x04002659 RID: 9817
		public Image image;

		// Token: 0x0400265A RID: 9818
		public TooltipProvider tooltipProvider;

		// Token: 0x0400265B RID: 9819
		public TextMeshProUGUI voteCounter;

		// Token: 0x0400265C RID: 9820
		public Image selectionDisplayPanel;

		// Token: 0x0400265D RID: 9821
		public bool canVote;

		// Token: 0x0400265E RID: 9822
		private RuleChoiceDef currentChoiceDef;
	}
}
