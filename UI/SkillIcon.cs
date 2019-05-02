using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000648 RID: 1608
	public class SkillIcon : MonoBehaviour
	{
		// Token: 0x06002442 RID: 9282 RVA: 0x000ABAB0 File Offset: 0x000A9CB0
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

		// Token: 0x040026E4 RID: 9956
		public SkillSlot targetSkillSlot;

		// Token: 0x040026E5 RID: 9957
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x040026E6 RID: 9958
		public GenericSkill targetSkill;

		// Token: 0x040026E7 RID: 9959
		public Image iconImage;

		// Token: 0x040026E8 RID: 9960
		public RawImage cooldownRemapPanel;

		// Token: 0x040026E9 RID: 9961
		public TextMeshProUGUI cooldownText;

		// Token: 0x040026EA RID: 9962
		public TextMeshProUGUI stockText;

		// Token: 0x040026EB RID: 9963
		public GameObject flashPanelObject;

		// Token: 0x040026EC RID: 9964
		public GameObject isReadyPanelObject;

		// Token: 0x040026ED RID: 9965
		public Animator animator;

		// Token: 0x040026EE RID: 9966
		public string animatorStackString;

		// Token: 0x040026EF RID: 9967
		public bool wasReady;

		// Token: 0x040026F0 RID: 9968
		public int previousStock;

		// Token: 0x040026F1 RID: 9969
		public TooltipProvider tooltipProvider;

		// Token: 0x040026F2 RID: 9970
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
