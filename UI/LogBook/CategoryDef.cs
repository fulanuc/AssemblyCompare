﻿using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000666 RID: 1638
	public class CategoryDef
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06002493 RID: 9363 RVA: 0x0001AAEC File Offset: 0x00018CEC
		// (set) Token: 0x06002494 RID: 9364 RVA: 0x0001AAF4 File Offset: 0x00018CF4
		public GameObject iconPrefab
		{
			get
			{
				return this._iconPrefab;
			}
			[NotNull]
			set
			{
				this._iconPrefab = value;
				this.iconSize = ((RectTransform)this._iconPrefab.transform).sizeDelta;
			}
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x000AD324 File Offset: 0x000AB524
		public static void InitializeDefault(GameObject gameObject, Entry entry, EntryStatus status, UserProfile userProfile)
		{
			Texture texture = null;
			Color color = Color.white;
			Texture texture2;
			switch (status)
			{
			case EntryStatus.Unimplemented:
				texture2 = Resources.Load<Texture2D>("Textures/MiscIcons/texWIPIcon");
				break;
			case EntryStatus.Locked:
				texture2 = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.gray;
				break;
			case EntryStatus.Unencountered:
				texture2 = entry.iconTexture;
				color = Color.black;
				break;
			case EntryStatus.Available:
				texture2 = entry.iconTexture;
				texture = entry.bgTexture;
				color = Color.white;
				break;
			case EntryStatus.New:
				texture2 = entry.iconTexture;
				texture = entry.bgTexture;
				color = new Color(1f, 0.8f, 0.5f, 1f);
				break;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
			RawImage rawImage = null;
			ChildLocator component = gameObject.GetComponent<ChildLocator>();
			RawImage rawImage2;
			if (component)
			{
				rawImage2 = component.FindChild("Icon").GetComponent<RawImage>();
				rawImage = component.FindChild("BG").GetComponent<RawImage>();
			}
			else
			{
				rawImage2 = gameObject.GetComponentInChildren<RawImage>();
			}
			rawImage2.texture = texture2;
			rawImage2.color = color;
			if (rawImage)
			{
				if (texture != null)
				{
					rawImage.texture = texture;
				}
				else
				{
					rawImage.enabled = false;
				}
			}
			TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren)
			{
				if (status >= EntryStatus.Available)
				{
					componentInChildren.text = Language.GetString(entry.nameToken);
					return;
				}
				componentInChildren.text = Language.GetString("UNIDENTIFIED");
			}
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x000AD48C File Offset: 0x000AB68C
		public static void InitializeChallenge(GameObject gameObject, Entry entry, EntryStatus status, UserProfile userProfile)
		{
			TextMeshProUGUI textMeshProUGUI = null;
			TextMeshProUGUI textMeshProUGUI2 = null;
			RawImage rawImage = null;
			AchievementDef achievementDef = (AchievementDef)entry.extraData;
			float achievementProgress = AchievementManager.GetUserAchievementManager(LocalUserManager.readOnlyLocalUsersList.FirstOrDefault((LocalUser v) => v.userProfile == userProfile)).GetAchievementProgress(achievementDef);
			ChildLocator component = gameObject.GetComponent<ChildLocator>();
			if (component)
			{
				textMeshProUGUI = component.FindChild("DescriptionLabel").GetComponent<TextMeshProUGUI>();
				textMeshProUGUI2 = component.FindChild("NameLabel").GetComponent<TextMeshProUGUI>();
				rawImage = component.FindChild("RewardImage").GetComponent<RawImage>();
				textMeshProUGUI2.text = Language.GetString(achievementDef.nameToken);
				textMeshProUGUI.text = Language.GetString(achievementDef.descriptionToken);
			}
			Texture texture = null;
			Color color = Color.white;
			switch (status)
			{
			case EntryStatus.None:
				component.FindChild("RewardImageContainer").gameObject.SetActive(true);
				textMeshProUGUI2.text = "";
				textMeshProUGUI.text = "";
				break;
			case EntryStatus.Unimplemented:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texWIPIcon");
				break;
			case EntryStatus.Locked:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.black;
				textMeshProUGUI2.text = Language.GetString("UNIDENTIFIED");
				textMeshProUGUI.text = Language.GetString("UNIDENTIFIED");
				component.FindChild("CantBeAchieved").gameObject.SetActive(true);
				break;
			case EntryStatus.Unencountered:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.gray;
				component.FindChild("ProgressTowardsUnlocking").GetComponent<Image>().fillAmount = achievementProgress;
				break;
			case EntryStatus.Available:
				texture = entry.iconTexture;
				color = Color.white;
				component.FindChild("HasBeenUnlocked").gameObject.SetActive(true);
				break;
			case EntryStatus.New:
				texture = entry.iconTexture;
				color = new Color(1f, 0.8f, 0.5f, 1f);
				component.FindChild("HasBeenUnlocked").gameObject.SetActive(true);
				break;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
			if (texture != null)
			{
				rawImage.texture = texture;
				rawImage.color = color;
				return;
			}
			rawImage.enabled = false;
		}

		// Token: 0x0400278B RID: 10123
		[NotNull]
		public string nameToken = string.Empty;

		// Token: 0x0400278C RID: 10124
		[NotNull]
		public Entry[] entries = Array.Empty<Entry>();

		// Token: 0x0400278D RID: 10125
		private GameObject _iconPrefab;

		// Token: 0x0400278E RID: 10126
		public Vector2 iconSize = Vector2.one;

		// Token: 0x0400278F RID: 10127
		public bool fullWidth;

		// Token: 0x04002790 RID: 10128
		public Action<GameObject, Entry, EntryStatus, UserProfile> initializeElementGraphics = new Action<GameObject, Entry, EntryStatus, UserProfile>(CategoryDef.InitializeDefault);

		// Token: 0x04002791 RID: 10129
		public ViewablesCatalog.Node viewableNode;
	}
}
