using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000636 RID: 1590
	public class SkillIcon : MonoBehaviour
	{
		// Token: 0x060023B2 RID: 9138 RVA: 0x000AA434 File Offset: 0x000A8634
		private void Update()
		{
			if (this.targetSkill)
			{
				if (this.tooltipProvider)
				{
					this.tooltipProvider.titleToken = this.targetSkill.skillNameToken;
					this.tooltipProvider.bodyToken = this.targetSkill.skillDescriptionToken;
				}
				float cooldownRemaining = this.targetSkill.cooldownRemaining;
				float baseRechargeInterval = this.targetSkill.baseRechargeInterval;
				int stock = this.targetSkill.stock;
				bool flag = stock > 0 || cooldownRemaining == 0f;
				bool flag2 = this.targetSkill.CanExecute();
				if (this.previousStock < stock)
				{
					Util.PlaySound("Play_UI_cooldownRefresh", RoR2Application.instance.gameObject);
				}
				if (this.animator)
				{
					if (this.targetSkill.maxStock > 1)
					{
						this.animator.SetBool(this.animatorStackString, true);
					}
					else
					{
						this.animator.SetBool(this.animatorStackString, false);
					}
				}
				if (this.isReadyPanelObject)
				{
					this.isReadyPanelObject.SetActive(flag2);
				}
				if (!this.wasReady && flag && this.flashPanelObject)
				{
					this.flashPanelObject.SetActive(true);
				}
				if (this.cooldownText)
				{
					if (flag)
					{
						this.cooldownText.gameObject.SetActive(false);
					}
					else
					{
						SkillIcon.sharedStringBuilder.Clear();
						SkillIcon.sharedStringBuilder.AppendInt(Mathf.CeilToInt(cooldownRemaining), 0u, uint.MaxValue);
						this.cooldownText.SetText(SkillIcon.sharedStringBuilder);
						this.cooldownText.gameObject.SetActive(true);
					}
				}
				if (this.iconImage)
				{
					this.iconImage.enabled = true;
					this.iconImage.color = (flag2 ? Color.white : Color.gray);
					this.iconImage.sprite = this.targetSkill.icon;
				}
				if (this.cooldownRemapPanel)
				{
					float num = (baseRechargeInterval >= Mathf.Epsilon) ? (1f - cooldownRemaining / baseRechargeInterval) : 1f;
					this.cooldownRemapPanel.enabled = (num < 1f);
					this.cooldownRemapPanel.color = new Color(1f, 1f, 1f, 1f - cooldownRemaining / baseRechargeInterval);
				}
				if (this.stockText)
				{
					if (this.targetSkill.maxStock > 1)
					{
						this.stockText.gameObject.SetActive(true);
						SkillIcon.sharedStringBuilder.Clear();
						SkillIcon.sharedStringBuilder.AppendInt(this.targetSkill.stock, 0u, uint.MaxValue);
						this.stockText.SetText(SkillIcon.sharedStringBuilder);
					}
					else
					{
						this.stockText.gameObject.SetActive(false);
					}
				}
				this.wasReady = flag;
				this.previousStock = stock;
				return;
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = "";
				this.tooltipProvider.bodyToken = "";
			}
			if (this.cooldownText)
			{
				this.cooldownText.gameObject.SetActive(false);
			}
			if (this.stockText)
			{
				this.stockText.gameObject.SetActive(false);
			}
			if (this.iconImage)
			{
				this.iconImage.enabled = false;
				this.iconImage.sprite = null;
			}
		}

		// Token: 0x04002689 RID: 9865
		public SkillSlot targetSkillSlot;

		// Token: 0x0400268A RID: 9866
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x0400268B RID: 9867
		public GenericSkill targetSkill;

		// Token: 0x0400268C RID: 9868
		public Image iconImage;

		// Token: 0x0400268D RID: 9869
		public RawImage cooldownRemapPanel;

		// Token: 0x0400268E RID: 9870
		public TextMeshProUGUI cooldownText;

		// Token: 0x0400268F RID: 9871
		public TextMeshProUGUI stockText;

		// Token: 0x04002690 RID: 9872
		public GameObject flashPanelObject;

		// Token: 0x04002691 RID: 9873
		public GameObject isReadyPanelObject;

		// Token: 0x04002692 RID: 9874
		public Animator animator;

		// Token: 0x04002693 RID: 9875
		public string animatorStackString;

		// Token: 0x04002694 RID: 9876
		public bool wasReady;

		// Token: 0x04002695 RID: 9877
		public int previousStock;

		// Token: 0x04002696 RID: 9878
		public TooltipProvider tooltipProvider;

		// Token: 0x04002697 RID: 9879
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
