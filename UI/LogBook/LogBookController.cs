using System;
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
	// Token: 0x0200066B RID: 1643
	public class LogBookController : MonoBehaviour
	{
		// Token: 0x060024A0 RID: 9376 RVA: 0x000AD764 File Offset: 0x000AB964
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

		// Token: 0x060024A1 RID: 9377 RVA: 0x0001AB6F File Offset: 0x00018D6F
		private void Start()
		{
			this.BuildCategoriesButtons();
			this.GeneratePages();
			this.stateMachine.SetNextState(new LogBookController.ChangeCategoryState
			{
				newCategoryIndex = 0
			});
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000AD854 File Offset: 0x000ABA54
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

		// Token: 0x060024A3 RID: 9379 RVA: 0x0001AB94 File Offset: 0x00018D94
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(delegate
			{
				SceneCatalog.availability.CallWhenAvailable(new Action(LogBookController.BuildStaticData));
			});
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000AD96C File Offset: 0x000ABB6C
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

		// Token: 0x060024A5 RID: 9381 RVA: 0x000AD9D8 File Offset: 0x000ABBD8
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

		// Token: 0x060024A6 RID: 9382 RVA: 0x000ADACC File Offset: 0x000ABCCC
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

		// Token: 0x060024A7 RID: 9383 RVA: 0x000ADB40 File Offset: 0x000ABD40
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

		// Token: 0x060024A8 RID: 9384 RVA: 0x000ADBB4 File Offset: 0x000ABDB4
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

		// Token: 0x060024A9 RID: 9385 RVA: 0x000ADC5C File Offset: 0x000ABE5C
		private static TooltipContent GetWIPTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			return new TooltipContent
			{
				titleColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.WIP),
				titleToken = "TOOLTIP_WIP_CONTENT_NAME",
				bodyToken = "TOOLTIP_WIP_CONTENT_DESCRIPTION"
			};
		}

		// Token: 0x060024AA RID: 9386 RVA: 0x00003C44 File Offset: 0x00001E44
		private static EntryStatus GetAlwaysAvailable(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Available;
		}

		// Token: 0x060024AB RID: 9387 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static EntryStatus GetUnimplemented(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Unimplemented;
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x000ADCA0 File Offset: 0x000ABEA0
		private static EntryStatus GetStageStatus(UserProfile userProfile, Entry entry)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(SceneCatalog.GetUnlockableLogFromSceneName((entry.extraData as SceneDef).sceneName));
			if (unlockableDef != null && userProfile.HasUnlockable(unlockableDef))
			{
				return EntryStatus.Available;
			}
			return EntryStatus.Unencountered;
		}

		// Token: 0x060024AD RID: 9389 RVA: 0x000ADCD8 File Offset: 0x000ABED8
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

		// Token: 0x060024AE RID: 9390 RVA: 0x000ADD3C File Offset: 0x000ABF3C
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

		// Token: 0x060024AF RID: 9391 RVA: 0x000ADD9C File Offset: 0x000ABF9C
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

		// Token: 0x060024B0 RID: 9392 RVA: 0x0001ABBF File Offset: 0x00018DBF
		private static void BuildStaticData()
		{
			LogBookController.categories = LogBookController.BuildCategories();
			LogBookController.RegisterViewables(LogBookController.categories);
			LogBookController.availability.MakeAvailable();
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060024B2 RID: 9394 RVA: 0x0001ABF6 File Offset: 0x00018DF6
		// (set) Token: 0x060024B3 RID: 9395 RVA: 0x000ADDD4 File Offset: 0x000ABFD4
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

		// Token: 0x060024B4 RID: 9396 RVA: 0x0001ABFE File Offset: 0x00018DFE
		private LogBookController.NavigationPageInfo[] GetCategoryPages(int categoryIndex)
		{
			return this.navigationPagesByCategory[categoryIndex];
		}

		// Token: 0x060024B5 RID: 9397 RVA: 0x0001AC08 File Offset: 0x00018E08
		private void OnLeftButton()
		{
			this.desiredPageIndex--;
		}

		// Token: 0x060024B6 RID: 9398 RVA: 0x0001AC18 File Offset: 0x00018E18
		private void OnRightButton()
		{
			this.desiredPageIndex++;
		}

		// Token: 0x060024B7 RID: 9399 RVA: 0x0001AC28 File Offset: 0x00018E28
		private void OnCategoryClicked(int categoryIndex)
		{
			this.desiredCategoryIndex = categoryIndex;
			this.goToEndOfNextCategory = false;
		}

		// Token: 0x060024B8 RID: 9400 RVA: 0x000ADE6C File Offset: 0x000AC06C
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

		// Token: 0x060024B9 RID: 9401 RVA: 0x000ADFF4 File Offset: 0x000AC1F4
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

		// Token: 0x060024BA RID: 9402 RVA: 0x0001AC38 File Offset: 0x00018E38
		private UserProfile LookUpUserProfile()
		{
			LocalUser localUser = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault((LocalUser v) => v != null);
			if (localUser == null)
			{
				return null;
			}
			return localUser.userProfile;
		}

		// Token: 0x060024BB RID: 9403 RVA: 0x000AE200 File Offset: 0x000AC400
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

		// Token: 0x060024BC RID: 9404 RVA: 0x000AE49C File Offset: 0x000AC69C
		private void ViewEntry(Entry entry)
		{
			this.OnViewEntry.Invoke();
			LogBookPage component = this.pageViewerPanel.GetComponent<LogBookPage>();
			component.SetEntry(this.LookUpUserProfile(), entry);
			component.modelPanel.SetAnglesForCharacterThumbnailForSeconds(0.5f, false);
			ViewablesCatalog.Node viewableNode = entry.viewableNode;
			ViewableTrigger.TriggerView((viewableNode != null) ? viewableNode.fullName : null);
		}

		// Token: 0x060024BD RID: 9405 RVA: 0x0001AC6E File Offset: 0x00018E6E
		private void ReturnToNavigation()
		{
			this.navigationPanel.SetActive(true);
			this.pageViewerPanel.SetActive(false);
		}

		// Token: 0x060024BE RID: 9406 RVA: 0x000AE4F4 File Offset: 0x000AC6F4
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

		// Token: 0x060024BF RID: 9407 RVA: 0x000AE6B4 File Offset: 0x000AC8B4
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

		// Token: 0x060024C0 RID: 9408 RVA: 0x000AE924 File Offset: 0x000ACB24
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

		// Token: 0x040027AB RID: 10155
		[Header("Navigation")]
		public GameObject navigationPanel;

		// Token: 0x040027AC RID: 10156
		public RectTransform categoryContainer;

		// Token: 0x040027AD RID: 10157
		public GameObject categorySpaceFiller;

		// Token: 0x040027AE RID: 10158
		public int categorySpaceFillerCount;

		// Token: 0x040027AF RID: 10159
		public Color spaceFillerColor;

		// Token: 0x040027B0 RID: 10160
		private UIElementAllocator<MPButton> navigationCategoryButtonAllocator;

		// Token: 0x040027B1 RID: 10161
		public RectTransform entryPageContainer;

		// Token: 0x040027B2 RID: 10162
		public GameObject entryPagePrefab;

		// Token: 0x040027B3 RID: 10163
		public RectTransform navigationPageIndicatorContainer;

		// Token: 0x040027B4 RID: 10164
		public GameObject navigationPageIndicatorPrefab;

		// Token: 0x040027B5 RID: 10165
		private UIElementAllocator<MPButton> navigationPageIndicatorAllocator;

		// Token: 0x040027B6 RID: 10166
		public MPButton previousPageButton;

		// Token: 0x040027B7 RID: 10167
		public MPButton nextPageButton;

		// Token: 0x040027B8 RID: 10168
		public LanguageTextMeshController currentCategoryLabel;

		// Token: 0x040027B9 RID: 10169
		private RectTransform categoryHightlightRect;

		// Token: 0x040027BA RID: 10170
		[Header("PageViewer")]
		public UnityEvent OnViewEntry;

		// Token: 0x040027BB RID: 10171
		public GameObject pageViewerPanel;

		// Token: 0x040027BC RID: 10172
		public MPButton pageViewerBackButton;

		// Token: 0x040027BD RID: 10173
		private EntityStateMachine stateMachine;

		// Token: 0x040027BE RID: 10174
		public static CategoryDef[] categories = Array.Empty<CategoryDef>();

		// Token: 0x040027BF RID: 10175
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x040027C0 RID: 10176
		private LogBookController.NavigationPageInfo[] _availableNavigationPages = Array.Empty<LogBookController.NavigationPageInfo>();

		// Token: 0x040027C1 RID: 10177
		private GameObject currentEntriesPageObject;

		// Token: 0x040027C2 RID: 10178
		private int currentCategoryIndex;

		// Token: 0x040027C3 RID: 10179
		private int desiredCategoryIndex;

		// Token: 0x040027C4 RID: 10180
		private int currentPageIndex;

		// Token: 0x040027C5 RID: 10181
		private int desiredPageIndex;

		// Token: 0x040027C6 RID: 10182
		private bool goToEndOfNextCategory;

		// Token: 0x040027C7 RID: 10183
		private LogBookController.NavigationPageInfo[] allNavigationPages;

		// Token: 0x040027C8 RID: 10184
		private LogBookController.NavigationPageInfo[][] navigationPagesByCategory;

		// Token: 0x0200066C RID: 1644
		private class NavigationPageInfo
		{
			// Token: 0x040027C9 RID: 10185
			public CategoryDef categoryDef;

			// Token: 0x040027CA RID: 10186
			public Entry[] entries;

			// Token: 0x040027CB RID: 10187
			public int index;

			// Token: 0x040027CC RID: 10188
			public int indexInCategory;
		}

		// Token: 0x0200066D RID: 1645
		private class LogBookState : EntityState
		{
			// Token: 0x060024C3 RID: 9411 RVA: 0x0001AC9B File Offset: 0x00018E9B
			public override void OnEnter()
			{
				base.OnEnter();
				this.logBookController = base.GetComponent<LogBookController>();
			}

			// Token: 0x060024C4 RID: 9412 RVA: 0x0001ACAF File Offset: 0x00018EAF
			public override void Update()
			{
				base.Update();
				this.unscaledAge += Time.unscaledDeltaTime;
			}

			// Token: 0x040027CD RID: 10189
			protected LogBookController logBookController;

			// Token: 0x040027CE RID: 10190
			protected float unscaledAge;
		}

		// Token: 0x0200066E RID: 1646
		private class FadeState : LogBookController.LogBookState
		{
			// Token: 0x060024C6 RID: 9414 RVA: 0x0001ACC9 File Offset: 0x00018EC9
			public override void OnEnter()
			{
				base.OnEnter();
				this.canvasGroup = base.GetComponent<CanvasGroup>();
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = 0f;
				}
			}

			// Token: 0x060024C7 RID: 9415 RVA: 0x0001ACFA File Offset: 0x00018EFA
			public override void OnExit()
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.endValue;
				}
				base.OnExit();
			}

			// Token: 0x060024C8 RID: 9416 RVA: 0x000AEAC0 File Offset: 0x000ACCC0
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

			// Token: 0x040027CF RID: 10191
			private CanvasGroup canvasGroup;

			// Token: 0x040027D0 RID: 10192
			public float duration = 0.5f;

			// Token: 0x040027D1 RID: 10193
			public float endValue;
		}

		// Token: 0x0200066F RID: 1647
		private class ChangeEntriesPageState : LogBookController.LogBookState
		{
			// Token: 0x060024CA RID: 9418 RVA: 0x000AEB1C File Offset: 0x000ACD1C
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

			// Token: 0x060024CB RID: 9419 RVA: 0x000AEBA0 File Offset: 0x000ACDA0
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

			// Token: 0x060024CC RID: 9420 RVA: 0x000AEBF4 File Offset: 0x000ACDF4
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

			// Token: 0x060024CD RID: 9421 RVA: 0x000AEC8C File Offset: 0x000ACE8C
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

			// Token: 0x040027D2 RID: 10194
			private int oldPageIndex;

			// Token: 0x040027D3 RID: 10195
			public LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x040027D4 RID: 10196
			public float duration = 0.1f;

			// Token: 0x040027D5 RID: 10197
			public Vector2 moveDirection;

			// Token: 0x040027D6 RID: 10198
			private GameObject oldPage;

			// Token: 0x040027D7 RID: 10199
			private GameObject newPage;

			// Token: 0x040027D8 RID: 10200
			private Vector2 oldPageTargetPosition;

			// Token: 0x040027D9 RID: 10201
			private Vector2 newPageTargetPosition;

			// Token: 0x040027DA RID: 10202
			private Vector2 containerSize = Vector2.zero;
		}

		// Token: 0x02000670 RID: 1648
		private class ChangeCategoryState : LogBookController.LogBookState
		{
			// Token: 0x060024CF RID: 9423 RVA: 0x000AECD4 File Offset: 0x000ACED4
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

			// Token: 0x060024D0 RID: 9424 RVA: 0x000AEE00 File Offset: 0x000AD000
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

			// Token: 0x060024D1 RID: 9425 RVA: 0x000AEF0C File Offset: 0x000AD10C
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

			// Token: 0x060024D2 RID: 9426 RVA: 0x000AEFC0 File Offset: 0x000AD1C0
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

			// Token: 0x040027DB RID: 10203
			private int oldCategoryIndex;

			// Token: 0x040027DC RID: 10204
			public int newCategoryIndex;

			// Token: 0x040027DD RID: 10205
			public bool goToLastPage;

			// Token: 0x040027DE RID: 10206
			public float duration = 0.1f;

			// Token: 0x040027DF RID: 10207
			private GameObject oldPage;

			// Token: 0x040027E0 RID: 10208
			private GameObject newPage;

			// Token: 0x040027E1 RID: 10209
			private Vector2 oldPageTargetPosition;

			// Token: 0x040027E2 RID: 10210
			private Vector2 newPageTargetPosition;

			// Token: 0x040027E3 RID: 10211
			private Vector2 moveDirection;

			// Token: 0x040027E4 RID: 10212
			private Vector2 containerSize = Vector2.zero;

			// Token: 0x040027E5 RID: 10213
			private LogBookController.NavigationPageInfo[] newNavigationPages;

			// Token: 0x040027E6 RID: 10214
			private int destinationPageIndex;

			// Token: 0x040027E7 RID: 10215
			private LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x040027E8 RID: 10216
			private int frame;
		}

		// Token: 0x02000671 RID: 1649
		private class EnterLogViewState : LogBookController.LogBookState
		{
			// Token: 0x060024D4 RID: 9428 RVA: 0x000AF014 File Offset: 0x000AD214
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

			// Token: 0x060024D5 RID: 9429 RVA: 0x0001AD6F File Offset: 0x00018F6F
			private void SetIconRect(Rect rect)
			{
				this.flyingIconTransform.position = rect.position;
				this.flyingIconTransform.offsetMin = rect.min;
				this.flyingIconTransform.offsetMax = rect.max;
			}

			// Token: 0x060024D6 RID: 9430 RVA: 0x000AF13C File Offset: 0x000AD33C
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

			// Token: 0x060024D7 RID: 9431 RVA: 0x000AF1C8 File Offset: 0x000AD3C8
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

			// Token: 0x060024D8 RID: 9432 RVA: 0x000AF22C File Offset: 0x000AD42C
			private static Rect LerpRect(Rect a, Rect b, float t)
			{
				return new Rect
				{
					min = Vector2.LerpUnclamped(a.min, b.min, t),
					max = Vector2.LerpUnclamped(a.max, b.max, t)
				};
			}

			// Token: 0x060024D9 RID: 9433 RVA: 0x0001ADAC File Offset: 0x00018FAC
			public override void OnExit()
			{
				EntityState.Destroy(this.flyingIcon);
				base.OnExit();
			}

			// Token: 0x060024DA RID: 9434 RVA: 0x000AF278 File Offset: 0x000AD478
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

			// Token: 0x040027E9 RID: 10217
			public Texture iconTexture;

			// Token: 0x040027EA RID: 10218
			public RectTransform startRectTransform;

			// Token: 0x040027EB RID: 10219
			public RectTransform endRectTransform;

			// Token: 0x040027EC RID: 10220
			public Entry entry;

			// Token: 0x040027ED RID: 10221
			private GameObject flyingIcon;

			// Token: 0x040027EE RID: 10222
			private RectTransform flyingIconTransform;

			// Token: 0x040027EF RID: 10223
			private RawImage flyingIconImage;

			// Token: 0x040027F0 RID: 10224
			private float duration = 0.75f;

			// Token: 0x040027F1 RID: 10225
			private Rect startRect;

			// Token: 0x040027F2 RID: 10226
			private Rect midRect;

			// Token: 0x040027F3 RID: 10227
			private Rect endRect;

			// Token: 0x040027F4 RID: 10228
			private bool submittedViewEntry;
		}
	}
}
