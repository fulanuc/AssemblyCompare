using System;
using EntityStates.Sniper.Scope;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200064A RID: 1610
	[RequireComponent(typeof(HudElement))]
	public class SniperScopeChargeIndicatorController : MonoBehaviour
	{
		// Token: 0x06002448 RID: 9288 RVA: 0x0001A759 File Offset: 0x00018959
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x000ABEF4 File Offset: 0x000AA0F4
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

		// Token: 0x040026F5 RID: 9973
		private GameObject sourceGameObject;

		// Token: 0x040026F6 RID: 9974
		private HudElement hudElement;

		// Token: 0x040026F7 RID: 9975
		public Image image;
	}
}
