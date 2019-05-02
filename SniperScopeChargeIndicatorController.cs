using System;
using EntityStates.Sniper.Scope;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000638 RID: 1592
	[RequireComponent(typeof(HudElement))]
	public class SniperScopeChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x060023B8 RID: 9144 RVA: 0x0001A08B File Offset: 0x0001828B
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x000AA878 File Offset: 0x000A8A78
		private void FixedUpdate()
		{
			float fillAmount = 0f;
			if (this.hudElement.targetCharacterBody)
			{
				SkillLocator component = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
				if (component && component.secondary)
				{
					EntityStateMachine stateMachine = component.secondary.stateMachine;
					if (stateMachine)
					{
						ScopeSniper scopeSniper = stateMachine.state as ScopeSniper;
						if (scopeSniper != null)
						{
							fillAmount = scopeSniper.charge;
						}
					}
				}
			}
			if (this.image)
			{
				this.image.fillAmount = fillAmount;
			}
		}

		// Token: 0x0400269A RID: 9882
		private GameObject sourceGameObject;

		// Token: 0x0400269B RID: 9883
		private HudElement hudElement;

		// Token: 0x0400269C RID: 9884
		public Image image;
	}
}
