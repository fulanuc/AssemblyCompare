using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000642 RID: 1602
	[RequireComponent(typeof(MPButton))]
	public class SurvivorIconController : MonoBehaviour
	{
		// Token: 0x060023E3 RID: 9187 RVA: 0x0001A216 File Offset: 0x00018416
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
		}

		// Token: 0x060023E4 RID: 9188 RVA: 0x000AB140 File Offset: 0x000A9340
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

		// Token: 0x060023E5 RID: 9189 RVA: 0x000AB1BC File Offset: 0x000A93BC
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

		// Token: 0x060023E6 RID: 9190 RVA: 0x000AB258 File Offset: 0x000A9458
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

		// Token: 0x060023E7 RID: 9191 RVA: 0x000AB314 File Offset: 0x000A9514
		private static bool SurvivorIsUnlockedOnThisClient(SurvivorIndex survivorIndex)
		{
			return LocalUserManager.readOnlyLocalUsersList.Any((LocalUser localUser) => localUser.userProfile.HasSurvivorUnlocked(survivorIndex));
		}

		// Token: 0x040026B9 RID: 9913
		public CharacterSelectController characterSelectController;

		// Token: 0x040026BA RID: 9914
		public SurvivorIndex survivorIndex;

		// Token: 0x040026BB RID: 9915
		public RawImage survivorIcon;

		// Token: 0x040026BC RID: 9916
		public GameObject survivorIsSelectedEffect;

		// Token: 0x040026BD RID: 9917
		private bool shouldRebuild = true;

		// Token: 0x040026BE RID: 9918
		private bool isSelected;

		// Token: 0x040026BF RID: 9919
		private MPButton button;

		// Token: 0x040026C0 RID: 9920
		public ViewableTag viewableTag;

		// Token: 0x040026C1 RID: 9921
		public ViewableTrigger viewableTrigger;
	}
}
