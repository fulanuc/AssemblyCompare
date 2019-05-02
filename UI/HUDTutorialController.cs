using System;
using EntityStates;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E7 RID: 1511
	[RequireComponent(typeof(HUD))]
	public class HUDTutorialController : MonoBehaviour
	{
		// Token: 0x060021E7 RID: 8679 RVA: 0x000A4600 File Offset: 0x000A2800
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

		// Token: 0x060021E8 RID: 8680 RVA: 0x00018ACB File Offset: 0x00016CCB
		private UserProfile GetUserProfile()
		{
			return this.hud.localUserViewer.userProfile;
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x00018ADD File Offset: 0x00016CDD
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

		// Token: 0x060021EA RID: 8682 RVA: 0x000A4664 File Offset: 0x000A2864
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

		// Token: 0x040024DE RID: 9438
		private HUD hud;

		// Token: 0x040024DF RID: 9439
		[Header("Equipment Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject equipmentTutorialObject;

		// Token: 0x040024E0 RID: 9440
		[Tooltip("The equipment icon to monitor for a change to trigger the tutorial popup.")]
		public EquipmentIcon equipmentIcon;

		// Token: 0x040024E1 RID: 9441
		[Tooltip("The tutorial popup object.")]
		[Header("Difficulty Tutorial")]
		public GameObject difficultyTutorialObject;

		// Token: 0x040024E2 RID: 9442
		[Tooltip("The time at which to trigger the tutorial popup.")]
		public float difficultyTutorialTriggerTime = 60f;

		// Token: 0x040024E3 RID: 9443
		[Tooltip("The tutorial popup object.")]
		[Header("Sprint Tutorial")]
		public GameObject sprintTutorialObject;

		// Token: 0x040024E4 RID: 9444
		[Tooltip("How long to wait for the player to sprint before showing the tutorial popup.")]
		public float sprintTutorialTriggerTime = 30f;

		// Token: 0x040024E5 RID: 9445
		private float sprintTutorialStopwatch;
	}
}
