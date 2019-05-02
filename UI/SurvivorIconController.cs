using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000654 RID: 1620
	[RequireComponent(typeof(MPButton))]
	public class SurvivorIconController : MonoBehaviour
	{
		// Token: 0x06002473 RID: 9331 RVA: 0x0001A8E4 File Offset: 0x00018AE4
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x000AC7C0 File Offset: 0x000AA9C0
		public void PushSurvivorIndexToCharacterSelect(CharacterSelectController chararcterSelectController)
		{
			if (!PreGameController.instance || !PreGameController.instance.IsCharacterSwitchingCurrentlyAllowed())
			{
				return;
			}
			chararcterSelectController.SelectSurvivor(this.survivorIndex);
			LocalUser localUser = ((MPEventSystem)EventSystem.current).localUser;
			if (localUser.eventSystem == EventSystem.current)
			{
				NetworkUser currentNetworkUser = localUser.currentNetworkUser;
				if (currentNetworkUser == null)
				{
					return;
				}
				currentNetworkUser.CallCmdSetBodyPreference(BodyCatalog.FindBodyIndex(SurvivorCatalog.GetSurvivorDef(this.survivorIndex).bodyPrefab));
			}
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x000AC83C File Offset: 0x000AAA3C
		private void Update()
		{
			if (this.shouldRebuild)
			{
				this.shouldRebuild = false;
				this.Rebuild();
			}
			if (this.characterSelectController)
			{
				this.isSelected = false;
				bool flag = SurvivorIconController.SurvivorIsUnlockedOnThisClient(this.survivorIndex);
				this.button.interactable = flag;
				Color color = Color.gray;
				if (!flag)
				{
					color = Color.black;
				}
				if (this.characterSelectController.selectedSurvivorIndex == this.survivorIndex)
				{
					color = Color.white;
					this.isSelected = true;
				}
				this.survivorIcon.color = color;
			}
			this.survivorIsSelectedEffect.SetActive(this.isSelected);
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000AC8D8 File Offset: 0x000AAAD8
		private void Rebuild()
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(this.survivorIndex);
			if (survivorDef != null)
			{
				GameObject bodyPrefab = survivorDef.bodyPrefab;
				if (bodyPrefab)
				{
					CharacterBody component = bodyPrefab.GetComponent<CharacterBody>();
					if (component)
					{
						if (this.survivorIcon)
						{
							this.survivorIcon.texture = component.portraitIcon;
						}
						string viewableName = string.Format(CultureInfo.InvariantCulture, "/Survivors/{0}", this.survivorIndex.ToString());
						if (this.viewableTag)
						{
							this.viewableTag.viewableName = viewableName;
							this.viewableTag.Refresh();
						}
						if (this.viewableTrigger)
						{
							this.viewableTrigger.viewableName = viewableName;
						}
					}
				}
			}
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x000AC994 File Offset: 0x000AAB94
		private static bool SurvivorIsUnlockedOnThisClient(SurvivorIndex survivorIndex)
		{
			return LocalUserManager.readOnlyLocalUsersList.Any((LocalUser localUser) => localUser.userProfile.HasSurvivorUnlocked(survivorIndex));
		}

		// Token: 0x04002714 RID: 10004
		public CharacterSelectController characterSelectController;

		// Token: 0x04002715 RID: 10005
		public SurvivorIndex survivorIndex;

		// Token: 0x04002716 RID: 10006
		public RawImage survivorIcon;

		// Token: 0x04002717 RID: 10007
		public GameObject survivorIsSelectedEffect;

		// Token: 0x04002718 RID: 10008
		private bool shouldRebuild = true;

		// Token: 0x04002719 RID: 10009
		private bool isSelected;

		// Token: 0x0400271A RID: 10010
		private MPButton button;

		// Token: 0x0400271B RID: 10011
		public ViewableTag viewableTag;

		// Token: 0x0400271C RID: 10012
		public ViewableTrigger viewableTrigger;
	}
}
