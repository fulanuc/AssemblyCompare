using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using RoR2.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EA RID: 1514
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class GameEndReportPanelController : MonoBehaviour
	{
		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600221C RID: 8732 RVA: 0x00018D6A File Offset: 0x00016F6A
		// (set) Token: 0x0600221D RID: 8733 RVA: 0x00018D72 File Offset: 0x00016F72
		public GameEndReportPanelController.DisplayData displayData { get; private set; }

		// Token: 0x0600221E RID: 8734 RVA: 0x00018D7B File Offset: 0x00016F7B
		private void Awake()
		{
			this.playerNavigationController.onPageChangeSubmitted += this.OnPlayerNavigationControllerPageChangeSubmitted;
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x000A38A4 File Offset: 0x000A1AA4
		private void SetFlashAnimationValue(float t)
		{
			if (t == this.lastFlashAnimationValue)
			{
				return;
			}
			this.lastFlashAnimationValue = t;
			this.flashOverlay.color = new Color(1f, 1f, 1f, this.flashCurve.Evaluate(t));
			this.canvasGroup.alpha = this.alphaCurve.Evaluate(t);
			if (t >= 1f)
			{
				this.flashOverlay.enabled = false;
			}
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x00018D94 File Offset: 0x00016F94
		private void Update()
		{
			this.flashStopwatch += Time.deltaTime;
			this.SetFlashAnimationValue(Mathf.Clamp01(this.flashStopwatch / this.flashDuration));
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x000A3918 File Offset: 0x000A1B18
		private void AllocateStatStrips(int count)
		{
			while (this.statStrips.Count > count)
			{
				int index = this.statStrips.Count - 1;
				UnityEngine.Object.Destroy(this.statStrips[index].gameObject);
				this.statStrips.RemoveAt(index);
			}
			while (this.statStrips.Count < count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.statStripPrefab, this.statContentArea);
				gameObject.SetActive(true);
				this.statStrips.Add(gameObject);
			}
			if (this.statsAnimateImageAlpha)
			{
				Image[] array = new Image[this.statStrips.Count];
				for (int i = 0; i < this.statStrips.Count; i++)
				{
					array[i] = this.statStrips[i].GetComponent<Image>();
				}
				this.statsAnimateImageAlpha.ResetStopwatch();
				this.statsAnimateImageAlpha.images = array;
			}
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x000A39FC File Offset: 0x000A1BFC
		private void AllocateUnlockStrips(int count)
		{
			while (this.unlockStrips.Count > count)
			{
				int index = this.unlockStrips.Count - 1;
				UnityEngine.Object.Destroy(this.unlockStrips[index].gameObject);
				this.unlockStrips.RemoveAt(index);
			}
			while (this.unlockStrips.Count < count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.unlockStripPrefab, this.unlockContentArea);
				gameObject.SetActive(true);
				this.unlockStrips.Add(gameObject);
			}
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x000A3A80 File Offset: 0x000A1C80
		public void SetDisplayData(GameEndReportPanelController.DisplayData newDisplayData)
		{
			if (this.displayData.Equals(newDisplayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			if (this.resultLabel)
			{
				GameResultType gameResultType = GameResultType.Unknown;
				if (this.displayData.runReport != null)
				{
					gameResultType = this.displayData.runReport.gameResultType;
				}
				string token;
				if (gameResultType != GameResultType.Lost)
				{
					if (gameResultType != GameResultType.Won)
					{
						token = "GAME_RESULT_UNKNOWN";
					}
					else
					{
						token = "GAME_RESULT_WON";
					}
				}
				else
				{
					token = "GAME_RESULT_LOST";
				}
				this.resultLabel.text = Language.GetString(token);
			}
			RunReport runReport = this.displayData.runReport;
			RunReport.PlayerInfo playerInfo = (runReport != null) ? runReport.GetPlayerInfoSafe(this.displayData.playerIndex) : null;
			this.SetPlayerInfo(playerInfo);
			RunReport runReport2 = this.displayData.runReport;
			int num = (runReport2 != null) ? runReport2.playerInfoCount : 0;
			this.playerNavigationController.gameObject.SetActive(num > 1);
			this.playerNavigationController.SetDisplayData(new CarouselNavigationController.DisplayData(num, this.displayData.playerIndex));
			ReadOnlyCollection<MPButton> elements = this.playerNavigationController.buttonAllocator.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				MPButton mpbutton = elements[i];
				RunReport.PlayerInfo playerInfo2 = this.displayData.runReport.GetPlayerInfo(i);
				CharacterBody bodyPrefabBodyComponent = BodyCatalog.GetBodyPrefabBodyComponent(playerInfo2.bodyIndex);
				Texture texture = bodyPrefabBodyComponent ? bodyPrefabBodyComponent.portraitIcon : null;
				mpbutton.GetComponentInChildren<RawImage>().texture = texture;
				mpbutton.GetComponent<TooltipProvider>().SetContent(TooltipProvider.GetPlayerNameTooltipContent(playerInfo2.name));
			}
			this.selectedPlayerEffectRoot.transform.SetParent(this.playerNavigationController.buttonAllocator.elements[this.displayData.playerIndex].transform);
			this.selectedPlayerEffectRoot.gameObject.SetActive(false);
			this.selectedPlayerEffectRoot.gameObject.SetActive(true);
			this.selectedPlayerEffectRoot.offsetMin = Vector2.zero;
			this.selectedPlayerEffectRoot.offsetMax = Vector2.zero;
			this.selectedPlayerEffectRoot.localScale = Vector3.one;
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x000A3C8C File Offset: 0x000A1E8C
		private void OnPlayerNavigationControllerPageChangeSubmitted(int newPage)
		{
			GameEndReportPanelController.DisplayData displayData = this.displayData;
			displayData.playerIndex = newPage;
			this.SetDisplayData(displayData);
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x000A3CB0 File Offset: 0x000A1EB0
		private void SetPlayerInfo([CanBeNull] RunReport.PlayerInfo playerInfo)
		{
			ulong num = 0UL;
			if (playerInfo != null)
			{
				StatSheet statSheet = playerInfo.statSheet;
				this.AllocateStatStrips(this.statsToDisplay.Length);
				for (int i = 0; i < this.statsToDisplay.Length; i++)
				{
					string text = this.statsToDisplay[i];
					StatDef statDef = StatDef.Find(text);
					if (statDef == null)
					{
						Debug.LogWarningFormat("GameEndReportPanelController.SetStatSheet: Could not find stat def \"{0}\".", new object[]
						{
							text
						});
					}
					else
					{
						this.AssignStatToStrip(statSheet, statDef, this.statStrips[i]);
						num += statSheet.GetStatPointValue(statDef);
					}
				}
				int unlockableCount = statSheet.GetUnlockableCount();
				int num2 = 0;
				for (int j = 0; j < unlockableCount; j++)
				{
					if (!statSheet.GetUnlockable(j).hidden)
					{
						num2++;
					}
				}
				this.AllocateUnlockStrips(num2);
				int num3 = 0;
				for (int k = 0; k < unlockableCount; k++)
				{
					UnlockableDef unlockable = statSheet.GetUnlockable(k);
					if (!unlockable.hidden)
					{
						this.AssignUnlockToStrip(unlockable, this.unlockStrips[num3]);
						num3++;
					}
				}
				if (this.itemInventoryDisplay)
				{
					this.itemInventoryDisplay.SetItems(playerInfo.itemAcquisitionOrder, playerInfo.itemAcquisitionOrder.Length, playerInfo.itemStacks);
					this.itemInventoryDisplay.UpdateDisplay();
				}
			}
			else
			{
				this.AllocateStatStrips(0);
				this.AllocateUnlockStrips(0);
				if (this.itemInventoryDisplay)
				{
					this.itemInventoryDisplay.ResetItems();
				}
			}
			string @string = Language.GetString("STAT_POINTS_FORMAT");
			this.totalPointsLabel.text = string.Format(@string, TextSerialization.ToStringNumeric(num));
			GameObject gameObject = null;
			if (playerInfo != null)
			{
				gameObject = BodyCatalog.GetBodyPrefab(playerInfo.bodyIndex);
			}
			string arg = "";
			Texture texture = null;
			if (gameObject)
			{
				texture = gameObject.GetComponent<CharacterBody>().portraitIcon;
				arg = Language.GetString(gameObject.GetComponent<CharacterBody>().baseNameToken);
			}
			string string2 = Language.GetString("STAT_CLASS_NAME_FORMAT");
			this.playerBodyLabel.text = string.Format(string2, arg);
			this.playerBodyPortraitImage.texture = texture;
			GameObject gameObject2 = null;
			if (playerInfo != null)
			{
				gameObject2 = BodyCatalog.GetBodyPrefab(playerInfo.killerBodyIndex);
			}
			string arg2 = "";
			Texture texture2 = null;
			if (gameObject2)
			{
				texture2 = gameObject2.GetComponent<CharacterBody>().portraitIcon;
				arg2 = Language.GetString(gameObject2.GetComponent<CharacterBody>().baseNameToken);
			}
			string string3 = Language.GetString("STAT_KILLER_NAME_FORMAT");
			this.killerBodyLabel.text = string.Format(string3, arg2);
			this.killerBodyPortraitImage.texture = texture2;
			this.killerPanelObject.SetActive(gameObject2);
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x000A3F38 File Offset: 0x000A2138
		private void AssignStatToStrip([CanBeNull] StatSheet srcStatSheet, [NotNull] StatDef statDef, GameObject destStatStrip)
		{
			string arg = "0";
			ulong value = 0UL;
			if (srcStatSheet != null)
			{
				arg = srcStatSheet.GetStatDisplayValue(statDef);
				value = srcStatSheet.GetStatPointValue(statDef);
			}
			string @string = Language.GetString(statDef.displayToken);
			string text = string.Format(Language.GetString("STAT_NAME_VALUE_FORMAT"), @string, arg);
			destStatStrip.transform.Find("StatNameLabel").GetComponent<TextMeshProUGUI>().text = text;
			string string2 = Language.GetString("STAT_POINTS_FORMAT");
			destStatStrip.transform.Find("PointValueLabel").GetComponent<TextMeshProUGUI>().text = string.Format(string2, TextSerialization.ToStringNumeric(value));
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x000A3FD0 File Offset: 0x000A21D0
		private void AssignUnlockToStrip(UnlockableDef unlockableDef, GameObject destUnlockableStrip)
		{
			AchievementDef achievementDefFromUnlockable = AchievementManager.GetAchievementDefFromUnlockable(unlockableDef.name);
			Texture texture = null;
			string @string = Language.GetString("TOOLTIP_UNLOCK_GENERIC_NAME");
			string string2 = Language.GetString("TOOLTIP_UNLOCK_GENERIC_NAME");
			if (unlockableDef.name.Contains("Items."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_ITEM_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_ITEM_DESCRIPTION");
			}
			else if (unlockableDef.name.Contains("Logs."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_LOG_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_LOG_DESCRIPTION");
			}
			else if (unlockableDef.name.Contains("Characters."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_SURVIVOR_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_SURVIVOR_DESCRIPTION");
			}
			string string3;
			if (achievementDefFromUnlockable != null)
			{
				texture = Resources.Load<Texture>(achievementDefFromUnlockable.iconPath);
				string3 = Language.GetString(achievementDefFromUnlockable.nameToken);
			}
			else
			{
				string3 = Language.GetString(unlockableDef.nameToken);
			}
			if (texture != null)
			{
				destUnlockableStrip.transform.Find("IconImage").GetComponent<RawImage>().texture = texture;
			}
			destUnlockableStrip.transform.Find("NameLabel").GetComponent<TextMeshProUGUI>().text = string3;
			destUnlockableStrip.GetComponent<TooltipProvider>().overrideTitleText = @string;
			destUnlockableStrip.GetComponent<TooltipProvider>().overrideBodyText = string2;
		}

		// Token: 0x04002492 RID: 9362
		[Tooltip("The TextMeshProUGUI component to use to display the result of the game: Win or Loss")]
		[Header("Result")]
		public TextMeshProUGUI resultLabel;

		// Token: 0x04002493 RID: 9363
		[Tooltip("A list of StatDef names to display in the stats section.")]
		[Header("Stats")]
		public string[] statsToDisplay;

		// Token: 0x04002494 RID: 9364
		[Tooltip("Prefab to be used for stat display.")]
		public GameObject statStripPrefab;

		// Token: 0x04002495 RID: 9365
		[Tooltip("The RectTransform in which to build the stat strips.")]
		public RectTransform statContentArea;

		// Token: 0x04002496 RID: 9366
		[Tooltip("The TextMeshProUGUI component used to display the total points.")]
		public TextMeshProUGUI totalPointsLabel;

		// Token: 0x04002497 RID: 9367
		[Tooltip("The component in charge of swiping over all elements over time.")]
		public AnimateImageAlpha statsAnimateImageAlpha;

		// Token: 0x04002498 RID: 9368
		[Tooltip("Prefab to be used for unlock display.")]
		[Header("Unlocks")]
		public GameObject unlockStripPrefab;

		// Token: 0x04002499 RID: 9369
		[Tooltip("The RectTransform in which to build the unlock strips.")]
		public RectTransform unlockContentArea;

		// Token: 0x0400249A RID: 9370
		[Header("Items")]
		[Tooltip("The inventory display controller.")]
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x0400249B RID: 9371
		[Header("Intro Flash Animation")]
		[Tooltip("How long the flash animation takes.")]
		public float flashDuration;

		// Token: 0x0400249C RID: 9372
		[Tooltip("A white panel whose alpha will be animated from 0->1->0 when this panel is shown to simulate a flash effect.")]
		public Image flashOverlay;

		// Token: 0x0400249D RID: 9373
		[Tooltip("The alpha curve for flashOverlay")]
		public AnimationCurve flashCurve;

		// Token: 0x0400249E RID: 9374
		[Tooltip("The CanvasGroup which controls the alpha of this entire panel.")]
		public CanvasGroup canvasGroup;

		// Token: 0x0400249F RID: 9375
		[Tooltip("The alpha curve for this panel during its appearance animation.")]
		public AnimationCurve alphaCurve;

		// Token: 0x040024A0 RID: 9376
		[Header("Player Info")]
		[Tooltip("The RawImage component to use to display the player character's portrait.")]
		public RawImage playerBodyPortraitImage;

		// Token: 0x040024A1 RID: 9377
		[Tooltip("The TextMeshProUGUI component to use to display the player character's body name.")]
		public TextMeshProUGUI playerBodyLabel;

		// Token: 0x040024A2 RID: 9378
		[Header("Killer Info")]
		[Tooltip("The RawImage component to use to display the killer character's portrait.")]
		public RawImage killerBodyPortraitImage;

		// Token: 0x040024A3 RID: 9379
		[Tooltip("The TextMeshProUGUI component to use to display the killer character's body name.")]
		public TextMeshProUGUI killerBodyLabel;

		// Token: 0x040024A4 RID: 9380
		[Tooltip("The GameObject used as the panel for the killer information. This is used to disable the killer panel when the player has won the game.")]
		public GameObject killerPanelObject;

		// Token: 0x040024A5 RID: 9381
		[Header("Navigation")]
		public MPButton continueButton;

		// Token: 0x040024A6 RID: 9382
		public CarouselNavigationController playerNavigationController;

		// Token: 0x040024A7 RID: 9383
		public RectTransform selectedPlayerEffectRoot;

		// Token: 0x040024A8 RID: 9384
		private float lastFlashAnimationValue = -1f;

		// Token: 0x040024A9 RID: 9385
		private float flashStopwatch;

		// Token: 0x040024AA RID: 9386
		private readonly List<GameObject> statStrips = new List<GameObject>();

		// Token: 0x040024AB RID: 9387
		private readonly List<GameObject> unlockStrips = new List<GameObject>();

		// Token: 0x020005EB RID: 1515
		public struct DisplayData : IEquatable<GameEndReportPanelController.DisplayData>
		{
			// Token: 0x06002229 RID: 8745 RVA: 0x00018DE9 File Offset: 0x00016FE9
			public bool Equals(GameEndReportPanelController.DisplayData other)
			{
				return object.Equals(this.runReport, other.runReport) && this.playerIndex == other.playerIndex;
			}

			// Token: 0x0600222A RID: 8746 RVA: 0x000A4104 File Offset: 0x000A2304
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is GameEndReportPanelController.DisplayData)
				{
					GameEndReportPanelController.DisplayData other = (GameEndReportPanelController.DisplayData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x0600222B RID: 8747 RVA: 0x000A4130 File Offset: 0x000A2330
			public override int GetHashCode()
			{
				return ((-1418150836 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<RunReport>.Default.GetHashCode(this.runReport)) * -1521134295 + this.playerIndex.GetHashCode();
			}

			// Token: 0x040024AC RID: 9388
			[CanBeNull]
			public RunReport runReport;

			// Token: 0x040024AD RID: 9389
			public int playerIndex;
		}
	}
}
