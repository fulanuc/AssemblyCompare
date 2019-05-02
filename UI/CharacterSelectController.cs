using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B8 RID: 1464
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CharacterSelectController : MonoBehaviour
	{
		// Token: 0x170002DB RID: 731
		// (get) Token: 0x060020D3 RID: 8403 RVA: 0x00017E6A File Offset: 0x0001606A
		private NetworkUser networkUser
		{
			get
			{
				LocalUser localUser = this.localUser;
				if (localUser == null)
				{
					return null;
				}
				return localUser.currentNetworkUser;
			}
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x00017E7D File Offset: 0x0001607D
		private void SetEventSystem(MPEventSystem newEventSystem)
		{
			if (newEventSystem == this.eventSystem)
			{
				return;
			}
			this.eventSystem = newEventSystem;
			this.localUser = LocalUserManager.FindLocalUser(newEventSystem.player);
		}

		// Token: 0x060020D5 RID: 8405 RVA: 0x00017EA6 File Offset: 0x000160A6
		public void SelectSurvivor(SurvivorIndex survivor)
		{
			this.selectedSurvivorIndex = survivor;
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x0009EFE8 File Offset: 0x0009D1E8
		private void RebuildLocal()
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(this.selectedSurvivorIndex);
			this.survivorName.text = survivorDef.displayNameToken;
			if (survivorDef.descriptionToken != null)
			{
				this.survivorDescription.text = Language.GetString(survivorDef.descriptionToken);
			}
			SkillLocator component = survivorDef.bodyPrefab.GetComponent<SkillLocator>();
			if (component)
			{
				this.RebuildStrip(this.primarySkillStrip, component.primary);
				this.RebuildStrip(this.secondarySkillStrip, component.secondary);
				this.RebuildStrip(this.utilitySkillStrip, component.utility);
				this.RebuildStrip(this.specialSkillStrip, component.special);
				this.RebuildStrip(this.passiveSkillStrip, component.passiveSkill);
			}
			Image[] array = this.primaryColorImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = survivorDef.primaryColor;
			}
			TextMeshProUGUI[] array2 = this.primaryColorTexts;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].color = survivorDef.primaryColor;
			}
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x0009F0E8 File Offset: 0x0009D2E8
		private void RebuildStrip(CharacterSelectController.SkillStrip skillStrip, GenericSkill skill)
		{
			if (skill)
			{
				skillStrip.stripRoot.SetActive(true);
				skillStrip.skillIcon.sprite = skill.icon;
				skillStrip.skillDescription.text = Language.GetString(skill.skillDescriptionToken);
				skillStrip.skillName.text = Language.GetString(skill.skillNameToken);
				return;
			}
			skillStrip.stripRoot.SetActive(false);
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x0009F154 File Offset: 0x0009D354
		private void RebuildStrip(CharacterSelectController.SkillStrip skillStrip, SkillLocator.PassiveSkill skill)
		{
			if (skill.enabled)
			{
				skillStrip.stripRoot.SetActive(true);
				skillStrip.skillIcon.sprite = skill.icon;
				skillStrip.skillDescription.text = Language.GetString(skill.skillDescriptionToken);
				skillStrip.skillName.text = Language.GetString(skill.skillNameToken);
				return;
			}
			skillStrip.stripRoot.SetActive(false);
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x00017EAF File Offset: 0x000160AF
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			this.selectedSurvivorIndex = this.GetSelectedSurvivorIndexFromBodyPreference();
			this.RebuildLocal();
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x0009F1C0 File Offset: 0x0009D3C0
		private SurvivorIndex GetSelectedSurvivorIndexFromBodyPreference()
		{
			if (this.networkUser)
			{
				SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(BodyCatalog.GetBodyPrefab(this.networkUser.bodyIndexPreference));
				if (survivorDef != null && survivorDef.survivorIndex != SurvivorIndex.None)
				{
					return survivorDef.survivorIndex;
				}
			}
			return SurvivorIndex.Commando;
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x0009F204 File Offset: 0x0009D404
		private void Update()
		{
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			if (this.previousSurvivorIndex != this.selectedSurvivorIndex)
			{
				this.RebuildLocal();
				this.previousSurvivorIndex = this.selectedSurvivorIndex;
			}
			if (this.characterDisplayPads.Length != 0)
			{
				for (int i = 0; i < this.characterDisplayPads.Length; i++)
				{
					CharacterSelectController.CharacterPad characterPad = this.characterDisplayPads[i];
					NetworkUser networkUser = null;
					List<NetworkUser> list = new List<NetworkUser>(NetworkUser.readOnlyInstancesList.Count);
					list.AddRange(NetworkUser.readOnlyLocalPlayersList);
					for (int j = 0; j < NetworkUser.readOnlyInstancesList.Count; j++)
					{
						NetworkUser item = NetworkUser.readOnlyInstancesList[j];
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
					if (i < list.Count)
					{
						networkUser = list[i];
					}
					if (networkUser)
					{
						GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(networkUser.bodyIndexPreference);
						SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
						if (survivorDef != null)
						{
							SurvivorDef survivorDef2 = SurvivorCatalog.GetSurvivorDef(characterPad.displaySurvivorIndex);
							bool flag = true;
							if (survivorDef2 != null && survivorDef2.bodyPrefab == bodyPrefab)
							{
								flag = false;
							}
							if (flag)
							{
								GameObject displayPrefab = survivorDef.displayPrefab;
								this.ClearPadDisplay(characterPad);
								if (displayPrefab)
								{
									characterPad.displayInstance = UnityEngine.Object.Instantiate<GameObject>(displayPrefab, characterPad.padTransform.position, characterPad.padTransform.rotation, characterPad.padTransform);
								}
								characterPad.displaySurvivorIndex = survivorDef.survivorIndex;
							}
						}
						else
						{
							this.ClearPadDisplay(characterPad);
						}
					}
					else
					{
						this.ClearPadDisplay(characterPad);
					}
					if (!characterPad.padTransform)
					{
						return;
					}
					this.characterDisplayPads[i] = characterPad;
					if (this.characterDisplayPads[i].padTransform)
					{
						this.characterDisplayPads[i].padTransform.gameObject.SetActive(this.characterDisplayPads[i].displayInstance != null);
					}
				}
			}
			if (!RoR2Application.isInSinglePlayer)
			{
				bool flag2 = this.IsClientReady();
				this.readyButton.gameObject.SetActive(!flag2);
				this.unreadyButton.gameObject.SetActive(flag2);
			}
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x00017EE0 File Offset: 0x000160E0
		private void ClearPadDisplay(CharacterSelectController.CharacterPad characterPad)
		{
			if (characterPad.displayInstance)
			{
				UnityEngine.Object.Destroy(characterPad.displayInstance);
			}
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x0009F42C File Offset: 0x0009D62C
		private static bool InputPlayerIsAssigned(Player inputPlayer)
		{
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				if (readOnlyInstancesList[i].inputPlayer == inputPlayer)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x0009F464 File Offset: 0x0009D664
		public bool IsClientReady()
		{
			int num = 0;
			if (!PreGameController.instance)
			{
				return false;
			}
			VoteController component = PreGameController.instance.GetComponent<VoteController>();
			if (!component)
			{
				return false;
			}
			int i = 0;
			int voteCount = component.GetVoteCount();
			while (i < voteCount)
			{
				VoteController.UserVote vote = component.GetVote(i);
				if (vote.networkUserObject && vote.receivedVote)
				{
					NetworkUser component2 = vote.networkUserObject.GetComponent<NetworkUser>();
					if (component2 && component2.isLocalPlayer)
					{
						num++;
					}
				}
				i++;
			}
			return num == NetworkUser.readOnlyLocalPlayersList.Count;
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x0009F4FC File Offset: 0x0009D6FC
		public void ClientSetReady()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				if (networkUser)
				{
					networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, 0);
				}
				else
				{
					Debug.Log("Null network user in readonly local player list!!");
				}
			}
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x0009F568 File Offset: 0x0009D768
		public void ClientSetUnready()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, -1);
			}
		}

		// Token: 0x0400234D RID: 9037
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400234E RID: 9038
		private MPEventSystem eventSystem;

		// Token: 0x0400234F RID: 9039
		private LocalUser localUser;

		// Token: 0x04002350 RID: 9040
		public SurvivorIndex selectedSurvivorIndex;

		// Token: 0x04002351 RID: 9041
		private SurvivorIndex previousSurvivorIndex = SurvivorIndex.None;

		// Token: 0x04002352 RID: 9042
		public TextMeshProUGUI survivorName;

		// Token: 0x04002353 RID: 9043
		public CharacterSelectController.SkillStrip primarySkillStrip;

		// Token: 0x04002354 RID: 9044
		public CharacterSelectController.SkillStrip secondarySkillStrip;

		// Token: 0x04002355 RID: 9045
		public CharacterSelectController.SkillStrip utilitySkillStrip;

		// Token: 0x04002356 RID: 9046
		public CharacterSelectController.SkillStrip specialSkillStrip;

		// Token: 0x04002357 RID: 9047
		public CharacterSelectController.SkillStrip passiveSkillStrip;

		// Token: 0x04002358 RID: 9048
		public TextMeshProUGUI survivorDescription;

		// Token: 0x04002359 RID: 9049
		public CharacterSelectController.CharacterPad[] characterDisplayPads;

		// Token: 0x0400235A RID: 9050
		public Image[] primaryColorImages;

		// Token: 0x0400235B RID: 9051
		public TextMeshProUGUI[] primaryColorTexts;

		// Token: 0x0400235C RID: 9052
		public MPButton readyButton;

		// Token: 0x0400235D RID: 9053
		public MPButton unreadyButton;

		// Token: 0x020005B9 RID: 1465
		[Serializable]
		public struct SkillStrip
		{
			// Token: 0x0400235E RID: 9054
			public GameObject stripRoot;

			// Token: 0x0400235F RID: 9055
			public Image skillIcon;

			// Token: 0x04002360 RID: 9056
			public TextMeshProUGUI skillName;

			// Token: 0x04002361 RID: 9057
			public TextMeshProUGUI skillDescription;
		}

		// Token: 0x020005BA RID: 1466
		[Serializable]
		public struct CharacterPad
		{
			// Token: 0x04002362 RID: 9058
			public Transform padTransform;

			// Token: 0x04002363 RID: 9059
			public GameObject displayInstance;

			// Token: 0x04002364 RID: 9060
			public SurvivorIndex displaySurvivorIndex;
		}
	}
}
