using System;
using EntityStates;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F9 RID: 1529
	[RequireComponent(typeof(HUD))]
	public class HUDTutorialController : MonoBehaviour
	{
		// Token: 0x06002278 RID: 8824 RVA: 0x000A5BB4 File Offset: 0x000A3DB4
		private void Awake()
		{
			this.hud = base.GetComponent<HUD>();
			if (this.equipmentTutorialObject)
			{
				this.equipmentTutorialObject.SetActive(false);
			}
			if (this.difficultyTutorialObject)
			{
				this.difficultyTutorialObject.SetActive(false);
			}
			if (this.sprintTutorialObject)
			{
				this.sprintTutorialObject.SetActive(false);
			}
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000191C5 File Offset: 0x000173C5
		private UserProfile GetUserProfile()
		{
			return this.hud.localUserViewer.userProfile;
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000191D7 File Offset: 0x000173D7
		private void HandleTutorial(GameObject tutorialPopup, ref UserProfile.TutorialProgression tutorialProgression, bool dismiss = false, bool progress = true)
		{
			if (tutorialPopup && !dismiss)
			{
				tutorialPopup.SetActive(true);
			}
			tutorialProgression.shouldShow = false;
			if (progress)
			{
				tutorialProgression.showCount += 1u;
			}
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x000A5C18 File Offset: 0x000A3E18
		private void Update()
		{
			UserProfile userProfile = this.GetUserProfile();
			CharacterBody cachedBody = this.hud.localUserViewer.cachedBody;
			if (userProfile != null && cachedBody)
			{
				if (userProfile.tutorialEquipment.shouldShow && this.equipmentIcon.hasEquipment)
				{
					this.HandleTutorial(this.equipmentTutorialObject, ref userProfile.tutorialEquipment, false, true);
				}
				if (userProfile.tutorialDifficulty.shouldShow && Run.instance && Run.instance.fixedTime >= this.difficultyTutorialTriggerTime)
				{
					this.HandleTutorial(this.difficultyTutorialObject, ref userProfile.tutorialDifficulty, false, true);
				}
				if (userProfile.tutorialSprint.shouldShow)
				{
					if (cachedBody.isSprinting)
					{
						this.HandleTutorial(null, ref userProfile.tutorialSprint, true, true);
						return;
					}
					EntityStateMachine component = cachedBody.GetComponent<EntityStateMachine>();
					if (((component != null) ? component.state : null) is GenericCharacterMain)
					{
						this.sprintTutorialStopwatch += Time.deltaTime;
					}
					if (this.sprintTutorialStopwatch >= this.sprintTutorialTriggerTime)
					{
						this.HandleTutorial(this.sprintTutorialObject, ref userProfile.tutorialSprint, false, false);
						return;
					}
				}
				else if (this.sprintTutorialObject && this.sprintTutorialObject.activeInHierarchy && cachedBody.isSprinting)
				{
					UnityEngine.Object.Destroy(this.sprintTutorialObject);
					this.sprintTutorialObject = null;
					UserProfile userProfile2 = userProfile;
					userProfile2.tutorialSprint.showCount = userProfile2.tutorialSprint.showCount + 1u;
				}
			}
		}

		// Token: 0x04002533 RID: 9523
		private HUD hud;

		// Token: 0x04002534 RID: 9524
		[Header("Equipment Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject equipmentTutorialObject;

		// Token: 0x04002535 RID: 9525
		[Tooltip("The equipment icon to monitor for a change to trigger the tutorial popup.")]
		public EquipmentIcon equipmentIcon;

		// Token: 0x04002536 RID: 9526
		[Tooltip("The tutorial popup object.")]
		[Header("Difficulty Tutorial")]
		public GameObject difficultyTutorialObject;

		// Token: 0x04002537 RID: 9527
		[Tooltip("The time at which to trigger the tutorial popup.")]
		public float difficultyTutorialTriggerTime = 60f;

		// Token: 0x04002538 RID: 9528
		[Tooltip("The tutorial popup object.")]
		[Header("Sprint Tutorial")]
		public GameObject sprintTutorialObject;

		// Token: 0x04002539 RID: 9529
		[Tooltip("How long to wait for the player to sprint before showing the tutorial popup.")]
		public float sprintTutorialTriggerTime = 30f;

		// Token: 0x0400253A RID: 9530
		private float sprintTutorialStopwatch;
	}
}
