using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CA RID: 1482
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CharacterSelectController : MonoBehaviour
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06002164 RID: 8548 RVA: 0x00018564 File Offset: 0x00016764
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

		// Token: 0x06002165 RID: 8549 RVA: 0x00018577 File Offset: 0x00016777
		private void SetEventSystem(MPEventSystem newEventSystem)
		{
			if (newEventSystem == this.eventSystem)
			{
				return;
			}
			this.eventSystem = newEventSystem;
			this.localUser = LocalUserManager.FindLocalUser(newEventSystem.player);
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000185A0 File Offset: 0x000167A0
		public void SelectSurvivor(SurvivorIndex survivor)
		{
			this.selectedSurvivorIndex = survivor;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000A05BC File Offset: 0x0009E7BC
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

		// Token: 0x06002168 RID: 8552 RVA: 0x000A06BC File Offset: 0x0009E8BC
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

		// Token: 0x06002169 RID: 8553 RVA: 0x000A0728 File Offset: 0x0009E928
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

		// Token: 0x0600216A RID: 8554 RVA: 0x000185A9 File Offset: 0x000167A9
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.SetEventSystem(this.eventSystemLocator.eventSystem);
			this.selectedSurvivorIndex = this.GetSelectedSurvivorIndexFromBodyPreference();
			this.RebuildLocal();
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x000A0794 File Offset: 0x0009E994
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

		// Token: 0x0600216C RID: 8556 RVA: 0x000A07D8 File Offset: 0x0009E9D8
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

		// Token: 0x0600216D RID: 8557 RVA: 0x000185DA File Offset: 0x000167DA
		private void ClearPadDisplay(CharacterSelectController.CharacterPad characterPad)
		{
			if (characterPad.displayInstance)
			{
				UnityEngine.Object.Destroy(characterPad.displayInstance);
			}
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x000A0A00 File Offset: 0x0009EC00
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

		// Token: 0x0600216F RID: 8559 RVA: 0x000A0A38 File Offset: 0x0009EC38
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

		// Token: 0x06002170 RID: 8560 RVA: 0x000A0AD0 File Offset: 0x0009ECD0
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

		// Token: 0x06002171 RID: 8561 RVA: 0x000A0B3C File Offset: 0x0009ED3C
		public void ClientSetUnready()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
			{
				networkUser.CallCmdSubmitVote(PreGameController.instance.gameObject, -1);
			}
		}

		// Token: 0x040023A1 RID: 9121
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040023A2 RID: 9122
		private MPEventSystem eventSystem;

		// Token: 0x040023A3 RID: 9123
		private LocalUser localUser;

		// Token: 0x040023A4 RID: 9124
		public SurvivorIndex selectedSurvivorIndex;

		// Token: 0x040023A5 RID: 9125
		private SurvivorIndex previousSurvivorIndex = SurvivorIndex.None;

		// Token: 0x040023A6 RID: 9126
		public TextMeshProUGUI survivorName;

		// Token: 0x040023A7 RID: 9127
		public CharacterSelectController.SkillStrip primarySkillStrip;

		// Token: 0x040023A8 RID: 9128
		public CharacterSelectController.SkillStrip secondarySkillStrip;

		// Token: 0x040023A9 RID: 9129
		public CharacterSelectController.SkillStrip utilitySkillStrip;

		// Token: 0x040023AA RID: 9130
		public CharacterSelectController.SkillStrip specialSkillStrip;

		// Token: 0x040023AB RID: 9131
		public CharacterSelectController.SkillStrip passiveSkillStrip;

		// Token: 0x040023AC RID: 9132
		public TextMeshProUGUI survivorDescription;

		// Token: 0x040023AD RID: 9133
		public CharacterSelectController.CharacterPad[] characterDisplayPads;

		// Token: 0x040023AE RID: 9134
		public Image[] primaryColorImages;

		// Token: 0x040023AF RID: 9135
		public TextMeshProUGUI[] primaryColorTexts;

		// Token: 0x040023B0 RID: 9136
		public MPButton readyButton;

		// Token: 0x040023B1 RID: 9137
		public MPButton unreadyButton;

		// Token: 0x020005CB RID: 1483
		[Serializable]
		public struct SkillStrip
		{
			// Token: 0x040023B2 RID: 9138
			public GameObject stripRoot;

			// Token: 0x040023B3 RID: 9139
			public Image skillIcon;

			// Token: 0x040023B4 RID: 9140
			public TextMeshProUGUI skillName;

			// Token: 0x040023B5 RID: 9141
			public TextMeshProUGUI skillDescription;
		}

		// Token: 0x020005CC RID: 1484
		[Serializable]
		public struct CharacterPad
		{
			// Token: 0x040023B6 RID: 9142
			public Transform padTransform;

			// Token: 0x040023B7 RID: 9143
			public GameObject displayInstance;

			// Token: 0x040023B8 RID: 9144
			public SurvivorIndex displaySurvivorIndex;
		}
	}
}
