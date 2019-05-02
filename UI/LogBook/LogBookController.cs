﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntityStates;
using RoR2.Stats;
using RoR2.UI.SkinControllers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200067D RID: 1661
	public class LogBookController : MonoBehaviour
	{
		// Token: 0x06002537 RID: 9527 RVA: 0x000AEE54 File Offset: 0x000AD054
		private void Awake()
		{
			this.navigationCategoryButtonAllocator = new UIElementAllocator<MPButton>(this.categoryContainer, Resources.Load<GameObject>("Prefabs/UI/Logbook/CategoryButton"));
			this.navigationPageIndicatorAllocator = new UIElementAllocator<MPButton>(this.navigationPageIndicatorContainer, this.navigationPageIndicatorPrefab);
			this.previousPageButton.onClick.AddListener(new UnityAction(this.OnLeftButton));
			this.nextPageButton.onClick.AddListener(new UnityAction(this.OnRightButton));
			this.pageViewerBackButton.onClick.AddListener(new UnityAction(this.ReturnToNavigation));
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = default(SerializableEntityStateType);
			this.categoryHightlightRect = (RectTransform)UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ButtonSelectionHighlight"), base.transform.parent).transform;
			this.categoryHightlightRect.gameObject.SetActive(false);
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x0001B2A2 File Offset: 0x000194A2
		private void Start()
		{
			this.BuildCategoriesButtons();
			this.GeneratePages();
			this.stateMachine.SetNextState(new LogBookController.ChangeCategoryState
			{
				newCategoryIndex = 0
			});
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x000AEF44 File Offset: 0x000AD144
		private void BuildCategoriesButtons()
		{
			this.navigationCategoryButtonAllocator.AllocateElements(LogBookController.categories.Length);
			ReadOnlyCollection<MPButton> elements = this.navigationCategoryButtonAllocator.elements;
			for (int i = 0; i < LogBookController.categories.Length; i++)
			{
				MPButton mpbutton = elements[i];
				CategoryDef categoryDef = LogBookController.categories[i];
				mpbutton.GetComponentInChildren<TextMeshProUGUI>().text = Language.GetString(categoryDef.nameToken);
				mpbutton.onClick.RemoveAllListeners();
				int categoryIndex = i;
				mpbutton.onClick.AddListener(delegate()
				{
					this.OnCategoryClicked(categoryIndex);
				});
				ViewableTag viewableTag = mpbutton.gameObject.GetComponent<ViewableTag>();
				if (!viewableTag)
				{
					viewableTag = mpbutton.gameObject.AddComponent<ViewableTag>();
				}
				viewableTag.viewableName = categoryDef.viewableNode.fullName;
			}
			if (this.categorySpaceFiller)
			{
				for (int j = 0; j < this.categorySpaceFillerCount; j++)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.categorySpaceFiller, this.categoryContainer).gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x0001B2C7 File Offset: 0x000194C7
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(delegate
			{
				SceneCatalog.availability.CallWhenAvailable(new Action(LogBookController.BuildStaticData));
			});
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x000AF05C File Offset: 0x000AD25C
		private static EntryStatus GetPickupStatus(UserProfile userProfile, Entry entry)
		{
			PickupIndex pickupIndex = (PickupIndex)entry.extraData;
			ItemIndex itemIndex = pickupIndex.itemIndex;
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			string unlockableName;
			if (itemIndex != ItemIndex.None)
			{
				unlockableName = ItemCatalog.GetItemDef(itemIndex).unlockableName;
			}
			else
			{
				if (equipmentIndex == EquipmentIndex.None)
				{
					return EntryStatus.Unimplemented;
				}
				unlockableName = EquipmentCatalog.GetEquipmentDef(equipmentIndex).unlockableName;
			}
			if (!userProfile.HasUnlockable(unlockableName))
			{
				return EntryStatus.Locked;
			}
			if (!userProfile.HasDiscoveredPickup(pickupIndex))
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x000AF0C8 File Offset: 0x000AD2C8
		private static TooltipContent GetPickupTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(((PickupIndex)entry.extraData).GetUnlockableName());
			TooltipContent result = default(TooltipContent);
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				if (unlockableDef != null)
				{
					result.overrideBodyText = unlockableDef.getUnlockedString();
				}
				result.bodyToken = "LOGBOOK_CATEGORY_ITEM";
				result.bodyColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable);
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				if (status == EntryStatus.Unimplemented)
				{
					result.titleToken = "TOOLTIP_WIP_CONTENT_NAME";
					result.bodyToken = "TOOLTIP_WIP_CONTENT_DESCRIPTION";
				}
				else if (status == EntryStatus.Unencountered)
				{
					result.overrideBodyText = Language.GetString("LOGBOOK_UNLOCK_ITEM_LOG");
				}
				else if (status == EntryStatus.Locked)
				{
					result.overrideBodyText = unlockableDef.getHowToUnlockString();
				}
				result.bodyColor = Color.white;
			}
			return result;
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x000AF1BC File Offset: 0x000AD3BC
		private static TooltipContent GetMonsterTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			result.titleColor = entry.color;
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_MONSTER";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				result.bodyToken = "LOGBOOK_UNLOCK_ITEM_MONSTER";
			}
			return result;
		}

		// Token: 0x0600253E RID: 9534 RVA: 0x000AF230 File Offset: 0x000AD430
		private static TooltipContent GetStageTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			result.titleColor = entry.color;
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_STAGE";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				result.bodyToken = "LOGBOOK_UNLOCK_ITEM_STAGE";
			}
			return result;
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x000AF2A4 File Offset: 0x000AD4A4
		private static TooltipContent GetSurvivorTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(SurvivorCatalog.FindSurvivorDefFromBody(((CharacterBody)entry.extraData).gameObject).unlockableName);
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_SURVIVOR";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				if (status == EntryStatus.Unencountered)
				{
					result.overrideBodyText = Language.GetString("LOGBOOK_UNLOCK_ITEM_SURVIVOR");
				}
				else if (status == EntryStatus.Locked)
				{
					result.overrideBodyText = unlockableDef.getHowToUnlockString();
				}
			}
			return result;
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x000AF34C File Offset: 0x000AD54C
		private static TooltipContent GetWIPTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			return new TooltipContent
			{
				titleColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.WIP),
				titleToken = "TOOLTIP_WIP_CONTENT_NAME",
				bodyToken = "TOOLTIP_WIP_CONTENT_DESCRIPTION"
			};
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x00003C44 File Offset: 0x00001E44
		private static EntryStatus GetAlwaysAvailable(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Available;
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static EntryStatus GetUnimplemented(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Unimplemented;
		}

		// Token: 0x06002543 RID: 9539 RVA: 0x000AF390 File Offset: 0x000AD590
		private static EntryStatus GetStageStatus(UserProfile userProfile, Entry entry)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(SceneCatalog.GetUnlockableLogFromSceneName((entry.extraData as SceneDef).sceneName));
			if (unlockableDef != null && userProfile.HasUnlockable(unlockableDef))
			{
				return EntryStatus.Available;
			}
			return EntryStatus.Unencountered;
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x000AF3C8 File Offset: 0x000AD5C8
		private static EntryStatus GetMonsterStatus(UserProfile userProfile, Entry entry)
		{
			CharacterBody characterBody = (CharacterBody)entry.extraData;
			DeathRewards component = characterBody.GetComponent<DeathRewards>();
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef((component != null) ? component.logUnlockableName : null);
			if (unlockableDef == null)
			{
				return EntryStatus.None;
			}
			if (userProfile.HasUnlockable(unlockableDef))
			{
				return EntryStatus.Available;
			}
			if (userProfile.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainst, characterBody.gameObject.name) > 0UL)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Locked;
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x000AF42C File Offset: 0x000AD62C
		private static EntryStatus GetSurvivorStatus(UserProfile userProfile, Entry entry)
		{
			CharacterBody characterBody = (CharacterBody)entry.extraData;
			SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(characterBody.gameObject);
			if (!userProfile.HasUnlockable(survivorDef.unlockableName))
			{
				return EntryStatus.Locked;
			}
			if (userProfile.statSheet.GetStatValueDouble(PerBodyStatDef.totalTimeAlive, characterBody.gameObject.name) == 0.0)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x06002546 RID: 9542 RVA: 0x000AF48C File Offset: 0x000AD68C
		private static EntryStatus GetAchievementStatus(UserProfile userProfile, Entry entry)
		{
			string identifier = ((AchievementDef)entry.extraData).identifier;
			bool flag = userProfile.HasAchievement(identifier);
			if (!userProfile.CanSeeAchievement(identifier))
			{
				return EntryStatus.Locked;
			}
			if (!flag)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x0001B2F2 File Offset: 0x000194F2
		private static void BuildStaticData()
		{
			LogBookController.categories = LogBookController.BuildCategories();
			LogBookController.RegisterViewables(LogBookController.categories);
			LogBookController.availability.MakeAvailable();
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06002549 RID: 9545 RVA: 0x0001B329 File Offset: 0x00019529
		// (set) Token: 0x0600254A RID: 9546 RVA: 0x000AF4C4 File Offset: 0x000AD6C4
		private LogBookController.NavigationPageInfo[] availableNavigationPages
		{
			get
			{
				return this._availableNavigationPages;
			}
			set
			{
				int num = this._availableNavigationPages.Length;
				this._availableNavigationPages = value;
				if (num != this.availableNavigationPages.Length)
				{
					this.navigationPageIndicatorAllocator.AllocateElements(this.availableNavigationPages.Length);
					ReadOnlyCollection<MPButton> elements = this.navigationPageIndicatorAllocator.elements;
					for (int i = 0; i < elements.Count; i++)
					{
						elements[i].onClick.RemoveAllListeners();
						int pageIndex = i;
						elements[i].onClick.AddListener(delegate()
						{
							this.desiredPageIndex = pageIndex;
						});
					}
				}
			}
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x0001B331 File Offset: 0x00019531
		private LogBookController.NavigationPageInfo[] GetCategoryPages(int categoryIndex)
		{
			return this.navigationPagesByCategory[categoryIndex];
		}

		// Token: 0x0600254C RID: 9548 RVA: 0x0001B33B File Offset: 0x0001953B
		private void OnLeftButton()
		{
			this.desiredPageIndex--;
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x0001B34B File Offset: 0x0001954B
		private void OnRightButton()
		{
			this.desiredPageIndex++;
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x0001B35B File Offset: 0x0001955B
		private void OnCategoryClicked(int categoryIndex)
		{
			this.desiredCategoryIndex = categoryIndex;
			this.goToEndOfNextCategory = false;
		}

		// Token: 0x0600254F RID: 9551 RVA: 0x000AF55C File Offset: 0x000AD75C
		private void GeneratePages()
		{
			this.navigationPagesByCategory = new LogBookController.NavigationPageInfo[LogBookController.categories.Length][];
			IEnumerable<LogBookController.NavigationPageInfo> enumerable = Array.Empty<LogBookController.NavigationPageInfo>();
			int num = 0;
			for (int i = 0; i < LogBookController.categories.Length; i++)
			{
				CategoryDef categoryDef = LogBookController.categories[i];
				bool fullWidth = categoryDef.fullWidth;
				Vector2 size = this.entryPageContainer.rect.size;
				if (fullWidth)
				{
					categoryDef.iconSize.x = size.x;
				}
				int num2 = Mathf.FloorToInt(Mathf.Max(size.x / categoryDef.iconSize.x, 1f));
				int num3 = Mathf.FloorToInt(Mathf.Max(size.y / categoryDef.iconSize.y, 1f));
				int num4 = num2 * num3;
				int num5 = Mathf.CeilToInt((float)categoryDef.entries.Length / (float)num4);
				if (num5 <= 0)
				{
					num5 = 1;
				}
				LogBookController.NavigationPageInfo[] array = new LogBookController.NavigationPageInfo[num5];
				for (int j = 0; j < num5; j++)
				{
					int num6 = num4;
					int num7 = j * num4;
					int num8 = categoryDef.entries.Length - num7;
					int num9 = num6;
					if (num9 > num8)
					{
						num9 = num8;
					}
					Entry[] array2 = new Entry[num6];
					Array.Copy(categoryDef.entries, num7, array2, 0, num9);
					array[j] = new LogBookController.NavigationPageInfo
					{
						categoryDef = categoryDef,
						entries = array2,
						index = num++,
						indexInCategory = j
					};
				}
				this.navigationPagesByCategory[i] = array;
				enumerable = enumerable.Concat(array);
			}
			this.allNavigationPages = enumerable.ToArray<LogBookController.NavigationPageInfo>();
		}

		// Token: 0x06002550 RID: 9552 RVA: 0x000AF6E4 File Offset: 0x000AD8E4
		private void Update()
		{
			if (this.desiredPageIndex > this.availableNavigationPages.Length - 1)
			{
				this.desiredPageIndex = this.availableNavigationPages.Length - 1;
				this.desiredCategoryIndex++;
				this.goToEndOfNextCategory = false;
			}
			if (this.desiredPageIndex < 0)
			{
				this.desiredCategoryIndex--;
				this.desiredPageIndex = 0;
				this.goToEndOfNextCategory = true;
			}
			if (this.desiredCategoryIndex > LogBookController.categories.Length - 1)
			{
				this.desiredCategoryIndex = LogBookController.categories.Length - 1;
			}
			if (this.desiredCategoryIndex < 0)
			{
				this.desiredCategoryIndex = 0;
			}
			foreach (MPButton mpbutton in this.navigationPageIndicatorAllocator.elements)
			{
				ColorBlock colors = mpbutton.colors;
				colors.colorMultiplier = 1f;
				mpbutton.colors = colors;
			}
			if (this.currentPageIndex < this.navigationPageIndicatorAllocator.elements.Count)
			{
				MPButton mpbutton2 = this.navigationPageIndicatorAllocator.elements[this.currentPageIndex];
				ColorBlock colors2 = mpbutton2.colors;
				colors2.colorMultiplier = 2f;
				mpbutton2.colors = colors2;
			}
			if (this.desiredCategoryIndex != this.currentCategoryIndex)
			{
				if (this.stateMachine.state is Idle)
				{
					int num = (this.desiredCategoryIndex > this.currentCategoryIndex) ? 1 : -1;
					this.stateMachine.SetNextState(new LogBookController.ChangeCategoryState
					{
						newCategoryIndex = this.currentCategoryIndex + num,
						goToLastPage = this.goToEndOfNextCategory
					});
					return;
				}
			}
			else if (this.desiredPageIndex != this.currentPageIndex && this.stateMachine.state is Idle)
			{
				int num2 = (this.desiredPageIndex > this.currentPageIndex) ? 1 : -1;
				this.stateMachine.SetNextState(new LogBookController.ChangeEntriesPageState
				{
					newNavigationPageInfo = this.GetCategoryPages(this.currentCategoryIndex)[this.currentPageIndex + num2],
					moveDirection = new Vector2((float)num2, 0f)
				});
			}
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x0001B36B File Offset: 0x0001956B
		private UserProfile LookUpUserProfile()
		{
			LocalUser localUser = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault((LocalUser v) => v != null);
			if (localUser == null)
			{
				return null;
			}
			return localUser.userProfile;
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x000AF8F0 File Offset: 0x000ADAF0
		private GameObject BuildEntriesPage(LogBookController.NavigationPageInfo navigationPageInfo)
		{
			Entry[] entries = navigationPageInfo.entries;
			CategoryDef categoryDef = navigationPageInfo.categoryDef;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.entryPagePrefab, this.entryPageContainer);
			gameObject.GetComponent<GridLayoutGroup>().cellSize = categoryDef.iconSize;
			UIElementAllocator<RectTransform> uielementAllocator = new UIElementAllocator<RectTransform>((RectTransform)gameObject.transform, categoryDef.iconPrefab);
			uielementAllocator.AllocateElements(entries.Length);
			UserProfile userProfile = this.LookUpUserProfile();
			ReadOnlyCollection<RectTransform> elements = uielementAllocator.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				EntryStatus entryStatus = EntryStatus.None;
				RectTransform rectTransform = elements[i];
				MPButton component = rectTransform.GetComponent<MPButton>();
				Entry entry = (i < entries.Length) ? entries[i] : null;
				if (entry != null)
				{
					entryStatus = entry.getStatus(userProfile, entry);
					rectTransform.gameObject.AddComponent<TooltipProvider>().SetContent(entry.getTooltipContent(userProfile, entry, entryStatus));
					Action<GameObject, Entry, EntryStatus, UserProfile> initializeElementGraphics = categoryDef.initializeElementGraphics;
					if (initializeElementGraphics != null)
					{
						initializeElementGraphics(rectTransform.gameObject, entry, entryStatus, userProfile);
					}
					if (component)
					{
						component.interactable = (entryStatus >= EntryStatus.Available);
					}
					if (entry.viewableNode != null && !rectTransform.gameObject.GetComponent<ViewableTag>() && !(entry.extraData is AchievementDef))
					{
						ViewableTag viewableTag = rectTransform.gameObject.AddComponent<ViewableTag>();
						viewableTag.viewableVisualStyle = ViewableTag.ViewableVisualStyle.Icon;
						viewableTag.viewableName = entry.viewableNode.fullName;
					}
				}
				if (entryStatus >= EntryStatus.Available && component)
				{
					component.onClick.AddListener(delegate()
					{
						this.ViewEntry(entry);
					});
				}
				if (entryStatus == EntryStatus.None)
				{
					if (component)
					{
						component.enabled = false;
						component.targetGraphic.color = this.spaceFillerColor;
						component.GetComponent<ButtonSkinController>().enabled = false;
					}
					else
					{
						rectTransform.GetComponent<Image>().color = this.spaceFillerColor;
					}
					for (int j = rectTransform.childCount - 1; j >= 0; j--)
					{
						UnityEngine.Object.Destroy(rectTransform.GetChild(j).gameObject);
					}
				}
			}
			gameObject.gameObject.SetActive(true);
			GridLayoutGroup gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();
			Action destroyLayoutGroup = null;
			int frameTimer = 2;
			destroyLayoutGroup = delegate()
			{
				int frameTimer;
				frameTimer--;
				frameTimer = frameTimer;
				if (frameTimer <= 0)
				{
					gridLayoutGroup.enabled = false;
					RoR2Application.onLateUpdate -= destroyLayoutGroup;
				}
			};
			RoR2Application.onLateUpdate += destroyLayoutGroup;
			return gameObject;
		}

		// Token: 0x06002553 RID: 9555 RVA: 0x000AFB8C File Offset: 0x000ADD8C
		private void ViewEntry(Entry entry)
		{
			this.OnViewEntry.Invoke();
			LogBookPage component = this.pageViewerPanel.GetComponent<LogBookPage>();
			component.SetEntry(this.LookUpUserProfile(), entry);
			component.modelPanel.SetAnglesForCharacterThumbnailForSeconds(0.5f, false);
			ViewablesCatalog.Node viewableNode = entry.viewableNode;
			ViewableTrigger.TriggerView((viewableNode != null) ? viewableNode.fullName : null);
		}

		// Token: 0x06002554 RID: 9556 RVA: 0x0001B3A1 File Offset: 0x000195A1
		private void ReturnToNavigation()
		{
			this.navigationPanel.SetActive(true);
			this.pageViewerPanel.SetActive(false);
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x000AFBE4 File Offset: 0x000ADDE4
		private static Entry[] BuildPickupEntries()
		{
			Entry entry = new Entry();
			entry.nameToken = "TOOLTIP_WIP_CONTENT_NAME";
			entry.color = Color.white;
			entry.iconTexture = Resources.Load<Texture>("Textures/MiscIcons/texWIPIcon");
			entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetUnimplemented);
			entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetWIPTooltipContent);
			IEnumerable<Entry> first = from pickupIndex in PickupIndex.allPickups
			select ItemCatalog.GetItemDef(pickupIndex.itemIndex) into itemDef
			where itemDef != null && itemDef.inDroppableTier
			orderby (int)(itemDef.tier + ((itemDef.tier == ItemTier.Lunar) ? 100 : 0))
			select new Entry
			{
				nameToken = itemDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_ITEM",
				color = ColorCatalog.GetColor(itemDef.darkColorIndex),
				iconTexture = itemDef.pickupIconTexture,
				bgTexture = itemDef.bgIconTexture,
				extraData = new PickupIndex(itemDef.itemIndex),
				modelPrefab = Resources.Load<GameObject>(itemDef.pickupModelPath),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetPickupStatus),
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetPickupTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.SimplePickup),
				isWIP = Language.IsTokenInvalid(itemDef.loreToken)
			};
			IEnumerable<Entry> second = from pickupIndex in PickupIndex.allPickups
			select EquipmentCatalog.GetEquipmentDef(pickupIndex.equipmentIndex) into equipmentDef
			where equipmentDef != null && equipmentDef.canDrop
			orderby !equipmentDef.isLunar
			select new Entry
			{
				nameToken = equipmentDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_EQUIPMENT",
				color = ColorCatalog.GetColor(equipmentDef.colorIndex),
				iconTexture = equipmentDef.pickupIconTexture,
				bgTexture = equipmentDef.bgIconTexture,
				extraData = new PickupIndex(equipmentDef.equipmentIndex),
				modelPrefab = Resources.Load<GameObject>(equipmentDef.pickupModelPath),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetPickupStatus),
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetPickupTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.SimplePickup),
				isWIP = Language.IsTokenInvalid(equipmentDef.loreToken)
			};
			IEnumerable<Entry> enumerable = first.Concat(second);
			int count = Math.Max(120 - enumerable.Count<Entry>(), 0);
			IEnumerable<Entry> second2 = Enumerable.Repeat<Entry>(entry, count);
			enumerable = enumerable.Concat(second2);
			return enumerable.ToArray<Entry>();
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x000AFDA4 File Offset: 0x000ADFA4
		private static CategoryDef[] BuildCategories()
		{
			CategoryDef[] array = new CategoryDef[5];
			array[0] = new CategoryDef
			{
				nameToken = "LOGBOOK_CATEGORY_ITEMANDEQUIPMENT",
				iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/ItemEntryIcon"),
				entries = LogBookController.BuildPickupEntries()
			};
			int num = 1;
			CategoryDef categoryDef = new CategoryDef();
			categoryDef.nameToken = "LOGBOOK_CATEGORY_MONSTER";
			categoryDef.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/MonsterEntryIcon");
			categoryDef.entries = (from characterBody in BodyCatalog.allBodyPrefabBodyBodyComponents.Where(delegate(CharacterBody characterBody)
			{
				if (characterBody)
				{
					DeathRewards component = characterBody.GetComponent<DeathRewards>();
					return !string.IsNullOrEmpty((component != null) ? component.logUnlockableName : null);
				}
				return false;
			})
			orderby characterBody.baseMaxHealth
			select characterBody).Select(delegate(CharacterBody characterBody)
			{
				Entry entry = new Entry();
				entry.nameToken = characterBody.baseNameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_MONSTER";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty);
				entry.iconTexture = characterBody.portraitIcon;
				entry.extraData = characterBody;
				ModelLocator component = characterBody.GetComponent<ModelLocator>();
				GameObject modelPrefab;
				if (component == null)
				{
					modelPrefab = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					modelPrefab = ((modelTransform != null) ? modelTransform.gameObject : null);
				}
				entry.modelPrefab = modelPrefab;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetMonsterStatus);
				entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetMonsterTooltipContent);
				entry.addEntries = new Action<PageBuilder>(PageBuilder.MonsterBody);
				entry.bgTexture = (characterBody.isChampion ? Resources.Load<Texture>("Textures/ItemIcons/BG/texTier3BGIcon") : Resources.Load<Texture>("Textures/ItemIcons/BG/texTier1BGIcon"));
				entry.isWIP = false;
				return entry;
			}).ToArray<Entry>();
			array[num] = categoryDef;
			int num2 = 2;
			CategoryDef categoryDef2 = new CategoryDef();
			categoryDef2.nameToken = "LOGBOOK_CATEGORY_STAGE";
			categoryDef2.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/StageEntryIcon");
			categoryDef2.entries = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage || sceneDef.sceneType == SceneType.Intermission
			orderby sceneDef.stageOrder
			select new Entry
			{
				nameToken = sceneDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_STAGE",
				iconTexture = sceneDef.previewTexture,
				color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Interactable),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetStageStatus),
				modelPrefab = sceneDef.dioramaPrefab,
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetStageTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.Stage),
				extraData = sceneDef,
				isWIP = Language.IsTokenInvalid(sceneDef.loreToken)
			}).ToArray<Entry>();
			array[num2] = categoryDef2;
			int num3 = 3;
			CategoryDef categoryDef3 = new CategoryDef();
			categoryDef3.nameToken = "LOGBOOK_CATEGORY_SURVIVOR";
			categoryDef3.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/SurvivorEntryIcon");
			categoryDef3.entries = (from characterBody in BodyCatalog.allBodyPrefabBodyBodyComponents
			where SurvivorCatalog.FindSurvivorDefFromBody(characterBody.gameObject) != null
			select characterBody).Select(delegate(CharacterBody characterBody)
			{
				Entry entry = new Entry();
				entry.nameToken = characterBody.baseNameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_SURVIVOR";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty);
				entry.iconTexture = characterBody.portraitIcon;
				entry.extraData = characterBody;
				ModelLocator component = characterBody.GetComponent<ModelLocator>();
				GameObject modelPrefab;
				if (component == null)
				{
					modelPrefab = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					modelPrefab = ((modelTransform != null) ? modelTransform.gameObject : null);
				}
				entry.modelPrefab = modelPrefab;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetSurvivorStatus);
				entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetSurvivorTooltipContent);
				entry.addEntries = new Action<PageBuilder>(PageBuilder.SurvivorBody);
				entry.isWIP = false;
				return entry;
			}).ToArray<Entry>();
			array[num3] = categoryDef3;
			int num4 = 4;
			CategoryDef categoryDef4 = new CategoryDef();
			categoryDef4.nameToken = "LOGBOOK_CATEGORY_ACHIEVEMENTS";
			categoryDef4.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/AchievementEntryIcon");
			categoryDef4.entries = AchievementManager.allAchievementDefs.Select(delegate(AchievementDef achievementDef)
			{
				Entry entry = new Entry();
				entry.nameToken = achievementDef.nameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_ACHIEVEMENT";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty);
				Sprite achievedIcon = achievementDef.GetAchievedIcon();
				entry.iconTexture = ((achievedIcon != null) ? achievedIcon.texture : null);
				entry.extraData = achievementDef;
				entry.modelPrefab = null;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetAchievementStatus);
				return entry;
			}).ToArray<Entry>();
			categoryDef4.initializeElementGraphics = new Action<GameObject, Entry, EntryStatus, UserProfile>(CategoryDef.InitializeChallenge);
			categoryDef4.fullWidth = true;
			array[num4] = categoryDef4;
			return array;
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x000B0014 File Offset: 0x000AE214
		private static void RegisterViewables(CategoryDef[] categoriesToGenerateFrom)
		{
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Logbook", true, null);
			foreach (CategoryDef categoryDef in LogBookController.categories)
			{
				ViewablesCatalog.Node node2 = new ViewablesCatalog.Node(categoryDef.nameToken, true, node);
				categoryDef.viewableNode = node2;
				Entry[] entries = categoryDef.entries;
				for (int j = 0; j < entries.Length; j++)
				{
					LogBookController.<>c__DisplayClass69_0 CS$<>8__locals1 = new LogBookController.<>c__DisplayClass69_0();
					CS$<>8__locals1.entry = entries[j];
					bool flag = CS$<>8__locals1.entry.extraData is AchievementDef;
					string nameToken = CS$<>8__locals1.entry.nameToken;
					if (!CS$<>8__locals1.entry.isWIP && !(nameToken == "TOOLTIP_WIP_CONTENT_NAME"))
					{
						ViewablesCatalog.Node entryNode = new ViewablesCatalog.Node(nameToken, false, node2);
						if (!flag)
						{
							entryNode.shouldShowUnviewed = ((UserProfile userProfile) => CS$<>8__locals1.entry.getStatus(userProfile, CS$<>8__locals1.entry) == EntryStatus.Available && !userProfile.HasViewedViewable(entryNode.fullName));
						}
						else
						{
							AchievementDef achievementDef = (AchievementDef)CS$<>8__locals1.entry.extraData;
							bool hasPrereq = !string.IsNullOrEmpty(achievementDef.prerequisiteAchievementIdentifier);
							entryNode.shouldShowUnviewed = ((UserProfile userProfile) => (CS$<>8__locals1.entry.getStatus(userProfile, CS$<>8__locals1.entry) == EntryStatus.Available && userProfile.HasAchievement(achievementDef.prerequisiteAchievementIdentifier)) & hasPrereq);
						}
						CS$<>8__locals1.entry.viewableNode = entryNode;
					}
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x04002807 RID: 10247
		[Header("Navigation")]
		public GameObject navigationPanel;

		// Token: 0x04002808 RID: 10248
		public RectTransform categoryContainer;

		// Token: 0x04002809 RID: 10249
		public GameObject categorySpaceFiller;

		// Token: 0x0400280A RID: 10250
		public int categorySpaceFillerCount;

		// Token: 0x0400280B RID: 10251
		public Color spaceFillerColor;

		// Token: 0x0400280C RID: 10252
		private UIElementAllocator<MPButton> navigationCategoryButtonAllocator;

		// Token: 0x0400280D RID: 10253
		public RectTransform entryPageContainer;

		// Token: 0x0400280E RID: 10254
		public GameObject entryPagePrefab;

		// Token: 0x0400280F RID: 10255
		public RectTransform navigationPageIndicatorContainer;

		// Token: 0x04002810 RID: 10256
		public GameObject navigationPageIndicatorPrefab;

		// Token: 0x04002811 RID: 10257
		private UIElementAllocator<MPButton> navigationPageIndicatorAllocator;

		// Token: 0x04002812 RID: 10258
		public MPButton previousPageButton;

		// Token: 0x04002813 RID: 10259
		public MPButton nextPageButton;

		// Token: 0x04002814 RID: 10260
		public LanguageTextMeshController currentCategoryLabel;

		// Token: 0x04002815 RID: 10261
		private RectTransform categoryHightlightRect;

		// Token: 0x04002816 RID: 10262
		[Header("PageViewer")]
		public UnityEvent OnViewEntry;

		// Token: 0x04002817 RID: 10263
		public GameObject pageViewerPanel;

		// Token: 0x04002818 RID: 10264
		public MPButton pageViewerBackButton;

		// Token: 0x04002819 RID: 10265
		private EntityStateMachine stateMachine;

		// Token: 0x0400281A RID: 10266
		public static CategoryDef[] categories = Array.Empty<CategoryDef>();

		// Token: 0x0400281B RID: 10267
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x0400281C RID: 10268
		private LogBookController.NavigationPageInfo[] _availableNavigationPages = Array.Empty<LogBookController.NavigationPageInfo>();

		// Token: 0x0400281D RID: 10269
		private GameObject currentEntriesPageObject;

		// Token: 0x0400281E RID: 10270
		private int currentCategoryIndex;

		// Token: 0x0400281F RID: 10271
		private int desiredCategoryIndex;

		// Token: 0x04002820 RID: 10272
		private int currentPageIndex;

		// Token: 0x04002821 RID: 10273
		private int desiredPageIndex;

		// Token: 0x04002822 RID: 10274
		private bool goToEndOfNextCategory;

		// Token: 0x04002823 RID: 10275
		private LogBookController.NavigationPageInfo[] allNavigationPages;

		// Token: 0x04002824 RID: 10276
		private LogBookController.NavigationPageInfo[][] navigationPagesByCategory;

		// Token: 0x0200067E RID: 1662
		private class NavigationPageInfo
		{
			// Token: 0x04002825 RID: 10277
			public CategoryDef categoryDef;

			// Token: 0x04002826 RID: 10278
			public Entry[] entries;

			// Token: 0x04002827 RID: 10279
			public int index;

			// Token: 0x04002828 RID: 10280
			public int indexInCategory;
		}

		// Token: 0x0200067F RID: 1663
		private class LogBookState : EntityState
		{
			// Token: 0x0600255A RID: 9562 RVA: 0x0001B3CE File Offset: 0x000195CE
			public override void OnEnter()
			{
				base.OnEnter();
				this.logBookController = base.GetComponent<LogBookController>();
			}

			// Token: 0x0600255B RID: 9563 RVA: 0x0001B3E2 File Offset: 0x000195E2
			public override void Update()
			{
				base.Update();
				this.unscaledAge += Time.unscaledDeltaTime;
			}

			// Token: 0x04002829 RID: 10281
			protected LogBookController logBookController;

			// Token: 0x0400282A RID: 10282
			protected float unscaledAge;
		}

		// Token: 0x02000680 RID: 1664
		private class FadeState : LogBookController.LogBookState
		{
			// Token: 0x0600255D RID: 9565 RVA: 0x0001B3FC File Offset: 0x000195FC
			public override void OnEnter()
			{
				base.OnEnter();
				this.canvasGroup = base.GetComponent<CanvasGroup>();
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = 0f;
				}
			}

			// Token: 0x0600255E RID: 9566 RVA: 0x0001B42D File Offset: 0x0001962D
			public override void OnExit()
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.endValue;
				}
				base.OnExit();
			}

			// Token: 0x0600255F RID: 9567 RVA: 0x000B01B0 File Offset: 0x000AE3B0
			public override void Update()
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.unscaledAge / this.duration;
					if (this.canvasGroup.alpha >= 1f)
					{
						this.outer.SetNextState(new Idle());
					}
				}
				base.Update();
			}

			// Token: 0x0400282B RID: 10283
			private CanvasGroup canvasGroup;

			// Token: 0x0400282C RID: 10284
			public float duration = 0.5f;

			// Token: 0x0400282D RID: 10285
			public float endValue;
		}

		// Token: 0x02000681 RID: 1665
		private class ChangeEntriesPageState : LogBookController.LogBookState
		{
			// Token: 0x06002561 RID: 9569 RVA: 0x000B020C File Offset: 0x000AE40C
			public override void OnEnter()
			{
				base.OnEnter();
				if (this.logBookController)
				{
					this.oldPageIndex = this.logBookController.currentPageIndex;
					this.oldPage = this.logBookController.currentEntriesPageObject;
					this.newPage = this.logBookController.BuildEntriesPage(this.newNavigationPageInfo);
					this.containerSize = this.logBookController.entryPageContainer.rect.size;
				}
				this.SetPagePositions(0f);
			}

			// Token: 0x06002562 RID: 9570 RVA: 0x000B0290 File Offset: 0x000AE490
			public override void OnExit()
			{
				base.OnExit();
				EntityState.Destroy(this.oldPage);
				if (this.logBookController)
				{
					this.logBookController.currentEntriesPageObject = this.newPage;
					this.logBookController.currentPageIndex = this.newNavigationPageInfo.indexInCategory;
				}
			}

			// Token: 0x06002563 RID: 9571 RVA: 0x000B02E4 File Offset: 0x000AE4E4
			private void SetPagePositions(float t)
			{
				Vector2 vector = new Vector2(this.containerSize.x * -this.moveDirection.x, this.containerSize.y * this.moveDirection.y);
				Vector2 vector2 = vector * t;
				if (this.oldPage)
				{
					this.oldPage.transform.localPosition = vector2;
				}
				if (this.newPage)
				{
					this.newPage.transform.localPosition = vector2 - vector;
				}
			}

			// Token: 0x06002564 RID: 9572 RVA: 0x000B037C File Offset: 0x000AE57C
			public override void Update()
			{
				base.Update();
				float num = Mathf.Clamp01(this.unscaledAge / this.duration);
				this.SetPagePositions(num);
				if (num == 1f)
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x0400282E RID: 10286
			private int oldPageIndex;

			// Token: 0x0400282F RID: 10287
			public LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x04002830 RID: 10288
			public float duration = 0.1f;

			// Token: 0x04002831 RID: 10289
			public Vector2 moveDirection;

			// Token: 0x04002832 RID: 10290
			private GameObject oldPage;

			// Token: 0x04002833 RID: 10291
			private GameObject newPage;

			// Token: 0x04002834 RID: 10292
			private Vector2 oldPageTargetPosition;

			// Token: 0x04002835 RID: 10293
			private Vector2 newPageTargetPosition;

			// Token: 0x04002836 RID: 10294
			private Vector2 containerSize = Vector2.zero;
		}

		// Token: 0x02000682 RID: 1666
		private class ChangeCategoryState : LogBookController.LogBookState
		{
			// Token: 0x06002566 RID: 9574 RVA: 0x000B03C4 File Offset: 0x000AE5C4
			public override void OnEnter()
			{
				base.OnEnter();
				if (this.logBookController)
				{
					this.oldCategoryIndex = this.logBookController.currentCategoryIndex;
					this.oldPage = this.logBookController.currentEntriesPageObject;
					this.newNavigationPages = this.logBookController.GetCategoryPages(this.newCategoryIndex);
					this.destinationPageIndex = this.newNavigationPages[0].index;
					if (this.goToLastPage)
					{
						this.destinationPageIndex = this.newNavigationPages[this.newNavigationPages.Length - 1].index;
						Debug.LogFormat("goToLastPage=true destinationPageIndex={0}", new object[]
						{
							this.destinationPageIndex
						});
					}
					this.newNavigationPageInfo = this.logBookController.allNavigationPages[this.destinationPageIndex];
					this.newPage = this.logBookController.BuildEntriesPage(this.newNavigationPageInfo);
					this.containerSize = this.logBookController.entryPageContainer.rect.size;
					this.moveDirection = new Vector2(Mathf.Sign((float)(this.newCategoryIndex - this.oldCategoryIndex)), 0f);
				}
				this.SetPagePositions(0f);
			}

			// Token: 0x06002567 RID: 9575 RVA: 0x000B04F0 File Offset: 0x000AE6F0
			public override void OnExit()
			{
				EntityState.Destroy(this.oldPage);
				if (this.logBookController)
				{
					this.logBookController.currentEntriesPageObject = this.newPage;
					this.logBookController.currentPageIndex = this.newNavigationPageInfo.indexInCategory;
					this.logBookController.desiredPageIndex = this.newNavigationPageInfo.indexInCategory;
					this.logBookController.currentCategoryIndex = this.newCategoryIndex;
					this.logBookController.availableNavigationPages = this.newNavigationPages;
					this.logBookController.currentCategoryLabel.token = LogBookController.categories[this.newCategoryIndex].nameToken;
					this.logBookController.categoryHightlightRect.SetParent(this.logBookController.navigationCategoryButtonAllocator.elements[this.newCategoryIndex].transform, false);
					this.logBookController.categoryHightlightRect.gameObject.SetActive(false);
					this.logBookController.categoryHightlightRect.gameObject.SetActive(true);
				}
				base.OnExit();
			}

			// Token: 0x06002568 RID: 9576 RVA: 0x000B05FC File Offset: 0x000AE7FC
			private void SetPagePositions(float t)
			{
				Vector2 vector = new Vector2(this.containerSize.x * -this.moveDirection.x, this.containerSize.y * this.moveDirection.y);
				Vector2 vector2 = vector * t;
				if (this.oldPage)
				{
					this.oldPage.transform.localPosition = vector2;
				}
				if (this.newPage)
				{
					this.newPage.transform.localPosition = vector2 - vector;
					if (this.frame == 4)
					{
						this.newPage.GetComponent<GridLayoutGroup>().enabled = false;
					}
				}
			}

			// Token: 0x06002569 RID: 9577 RVA: 0x000B06B0 File Offset: 0x000AE8B0
			public override void Update()
			{
				base.Update();
				this.frame++;
				float num = Mathf.Clamp01(this.unscaledAge / this.duration);
				this.SetPagePositions(num);
				if (num == 1f)
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x04002837 RID: 10295
			private int oldCategoryIndex;

			// Token: 0x04002838 RID: 10296
			public int newCategoryIndex;

			// Token: 0x04002839 RID: 10297
			public bool goToLastPage;

			// Token: 0x0400283A RID: 10298
			public float duration = 0.1f;

			// Token: 0x0400283B RID: 10299
			private GameObject oldPage;

			// Token: 0x0400283C RID: 10300
			private GameObject newPage;

			// Token: 0x0400283D RID: 10301
			private Vector2 oldPageTargetPosition;

			// Token: 0x0400283E RID: 10302
			private Vector2 newPageTargetPosition;

			// Token: 0x0400283F RID: 10303
			private Vector2 moveDirection;

			// Token: 0x04002840 RID: 10304
			private Vector2 containerSize = Vector2.zero;

			// Token: 0x04002841 RID: 10305
			private LogBookController.NavigationPageInfo[] newNavigationPages;

			// Token: 0x04002842 RID: 10306
			private int destinationPageIndex;

			// Token: 0x04002843 RID: 10307
			private LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x04002844 RID: 10308
			private int frame;
		}

		// Token: 0x02000683 RID: 1667
		private class EnterLogViewState : LogBookController.LogBookState
		{
			// Token: 0x0600256B RID: 9579 RVA: 0x000B0704 File Offset: 0x000AE904
			public override void OnEnter()
			{
				base.OnEnter();
				this.flyingIcon = new GameObject("FlyingIcon", new Type[]
				{
					typeof(RectTransform),
					typeof(CanvasRenderer),
					typeof(RawImage)
				});
				this.flyingIconTransform = (RectTransform)this.flyingIcon.transform;
				this.flyingIconTransform.SetParent(this.logBookController.transform, false);
				this.flyingIconTransform.localScale = Vector3.one;
				this.flyingIconImage = this.flyingIconTransform.GetComponent<RawImage>();
				this.flyingIconImage.texture = this.iconTexture;
				Vector3[] array = new Vector3[4];
				this.startRectTransform.GetWorldCorners(array);
				this.startRect = this.GetRectRelativeToParent(array);
				this.midRect = new Rect(((RectTransform)this.logBookController.transform).rect.center, this.startRect.size);
				this.endRectTransform.GetWorldCorners(array);
				this.endRect = this.GetRectRelativeToParent(array);
				this.SetIconRect(this.startRect);
			}

			// Token: 0x0600256C RID: 9580 RVA: 0x0001B4A2 File Offset: 0x000196A2
			private void SetIconRect(Rect rect)
			{
				this.flyingIconTransform.position = rect.position;
				this.flyingIconTransform.offsetMin = rect.min;
				this.flyingIconTransform.offsetMax = rect.max;
			}

			// Token: 0x0600256D RID: 9581 RVA: 0x000B082C File Offset: 0x000AEA2C
			private Rect GetRectRelativeToParent(Vector3[] corners)
			{
				for (int i = 0; i < 4; i++)
				{
					corners[i] = this.logBookController.transform.InverseTransformPoint(corners[i]);
				}
				return new Rect
				{
					xMin = corners[0].x,
					xMax = corners[2].x,
					yMin = corners[0].y,
					yMax = corners[2].y
				};
			}

			// Token: 0x0600256E RID: 9582 RVA: 0x000B08B8 File Offset: 0x000AEAB8
			private static Rect RectFromWorldCorners(Vector3[] corners)
			{
				return new Rect
				{
					xMin = corners[0].x,
					xMax = corners[2].x,
					yMin = corners[0].y,
					yMax = corners[2].y
				};
			}

			// Token: 0x0600256F RID: 9583 RVA: 0x000B091C File Offset: 0x000AEB1C
			private static Rect LerpRect(Rect a, Rect b, float t)
			{
				return new Rect
				{
					min = Vector2.LerpUnclamped(a.min, b.min, t),
					max = Vector2.LerpUnclamped(a.max, b.max, t)
				};
			}

			// Token: 0x06002570 RID: 9584 RVA: 0x0001B4DF File Offset: 0x000196DF
			public override void OnExit()
			{
				EntityState.Destroy(this.flyingIcon);
				base.OnExit();
			}

			// Token: 0x06002571 RID: 9585 RVA: 0x000B0968 File Offset: 0x000AEB68
			public override void Update()
			{
				base.Update();
				float num = Mathf.Min(this.unscaledAge / this.duration, 1f);
				if (num < 0.1f)
				{
					Util.Remap(num, 0f, 0.1f, 0f, 1f);
					this.SetIconRect(this.startRect);
				}
				if (num < 0.2f)
				{
					float t = Util.Remap(num, 0.1f, 0.2f, 0f, 1f);
					this.SetIconRect(LogBookController.EnterLogViewState.LerpRect(this.startRect, this.midRect, t));
					return;
				}
				if (num < 0.4f)
				{
					Util.Remap(num, 0.2f, 0.4f, 0f, 1f);
					this.SetIconRect(this.midRect);
					return;
				}
				if (num < 0.6f)
				{
					float t2 = Util.Remap(num, 0.4f, 0.6f, 0f, 1f);
					this.SetIconRect(LogBookController.EnterLogViewState.LerpRect(this.midRect, this.endRect, t2));
					return;
				}
				if (num < 1f)
				{
					float num2 = Util.Remap(num, 0.6f, 1f, 0f, 1f);
					this.flyingIconImage.color = new Color(1f, 1f, 1f, 1f - num2);
					this.SetIconRect(this.endRect);
					if (!this.submittedViewEntry)
					{
						this.submittedViewEntry = true;
						this.logBookController.ViewEntry(this.entry);
						return;
					}
				}
				else
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x04002845 RID: 10309
			public Texture iconTexture;

			// Token: 0x04002846 RID: 10310
			public RectTransform startRectTransform;

			// Token: 0x04002847 RID: 10311
			public RectTransform endRectTransform;

			// Token: 0x04002848 RID: 10312
			public Entry entry;

			// Token: 0x04002849 RID: 10313
			private GameObject flyingIcon;

			// Token: 0x0400284A RID: 10314
			private RectTransform flyingIconTransform;

			// Token: 0x0400284B RID: 10315
			private RawImage flyingIconImage;

			// Token: 0x0400284C RID: 10316
			private float duration = 0.75f;

			// Token: 0x0400284D RID: 10317
			private Rect startRect;

			// Token: 0x0400284E RID: 10318
			private Rect midRect;

			// Token: 0x0400284F RID: 10319
			private Rect endRect;

			// Token: 0x04002850 RID: 10320
			private bool submittedViewEntry;
		}
	}
}
