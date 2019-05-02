using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Stats;
using TMPro;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200067B RID: 1659
	public class PageBuilder
	{
		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06002500 RID: 9472 RVA: 0x0001AF24 File Offset: 0x00019124
		private StatSheet statSheet
		{
			get
			{
				return this.userProfile.statSheet;
			}
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x000AF978 File Offset: 0x000ADB78
		public void Destroy()
		{
			foreach (GameObject obj in this.managedObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x000AF9C8 File Offset: 0x000ADBC8
		public void AddSimpleTextPanel(string text)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Logbook/SimpleTextPanel"), this.container);
			gameObject.GetComponent<ChildLocator>().FindChild("MainLabel").GetComponent<TextMeshProUGUI>().text = text;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0001AF31 File Offset: 0x00019131
		public void AddSimpleTextPanel(params string[] textLines)
		{
			this.AddSimpleTextPanel(string.Join("\n", textLines));
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x000AFA14 File Offset: 0x000ADC14
		public void AddSimplePickup(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			string token = null;
			if (itemIndex != ItemIndex.None)
			{
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				this.AddDescriptionPanel(Language.GetString(itemDef.descriptionToken));
				token = itemDef.loreToken;
				ulong statValueULong = this.statSheet.GetStatValueULong(PerItemStatDef.totalCollected.FindStatDef(itemIndex));
				ulong statValueULong2 = this.statSheet.GetStatValueULong(PerItemStatDef.highestCollected.FindStatDef(itemIndex));
				string stringFormatted = Language.GetStringFormatted("GENERIC_PREFIX_FOUND", new object[]
				{
					statValueULong
				});
				string stringFormatted2 = Language.GetStringFormatted("ITEM_PREFIX_STACKCOUNT", new object[]
				{
					statValueULong2
				});
				this.AddSimpleTextPanel(new string[]
				{
					stringFormatted,
					stringFormatted2
				});
			}
			else if (equipmentIndex != EquipmentIndex.None)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
				this.AddDescriptionPanel(Language.GetString(equipmentDef.descriptionToken));
				token = equipmentDef.loreToken;
				string stringFormatted3 = Language.GetStringFormatted("EQUIPMENT_PREFIX_TOTALTIMEHELD", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimeHeld.FindStatDef(equipmentIndex))
				});
				string stringFormatted4 = Language.GetStringFormatted("EQUIPMENT_PREFIX_USECOUNT", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex))
				});
				this.AddSimpleTextPanel(new string[]
				{
					stringFormatted3,
					stringFormatted4
				});
			}
			this.AddNotesPanel(Language.IsTokenInvalid(token) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(token));
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x0001AF44 File Offset: 0x00019144
		public void AddDescriptionPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("DESCRIPTION_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0001AF60 File Offset: 0x00019160
		public void AddNotesPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("NOTES_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x000AFB94 File Offset: 0x000ADD94
		public void AddBodyStatsPanel(CharacterBody bodyPrefabComponent)
		{
			float baseMaxHealth = bodyPrefabComponent.baseMaxHealth;
			float levelMaxHealth = bodyPrefabComponent.levelMaxHealth;
			float baseDamage = bodyPrefabComponent.baseDamage;
			float levelDamage = bodyPrefabComponent.levelDamage;
			float baseMoveSpeed = bodyPrefabComponent.baseMoveSpeed;
			this.AddSimpleTextPanel(new string[]
			{
				Language.GetStringFormatted("BODY_HEALTH_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseMaxHealth.ToString(),
						levelMaxHealth.ToString()
					})
				}),
				Language.GetStringFormatted("BODY_DAMAGE_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseDamage.ToString(),
						levelDamage.ToString()
					})
				}),
				Language.GetStringFormatted("BODY_MOVESPEED_FORMAT", new object[]
				{
					baseMoveSpeed
				})
			});
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x000AFC64 File Offset: 0x000ADE64
		public void AddMonsterPanel(CharacterBody bodyPrefabComponent)
		{
			ulong statValueULong = this.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainst, bodyPrefabComponent.gameObject.name);
			ulong statValueULong2 = this.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainstElite, bodyPrefabComponent.gameObject.name);
			ulong statValueULong3 = this.statSheet.GetStatValueULong(PerBodyStatDef.deathsFrom, bodyPrefabComponent.gameObject.name);
			string stringFormatted = Language.GetStringFormatted("MONSTER_PREFIX_KILLED", new object[]
			{
				statValueULong
			});
			string stringFormatted2 = Language.GetStringFormatted("MONSTER_PREFIX_ELITESKILLED", new object[]
			{
				statValueULong2
			});
			string stringFormatted3 = Language.GetStringFormatted("MONSTER_PREFIX_DEATH", new object[]
			{
				statValueULong3
			});
			this.AddSimpleTextPanel(new string[]
			{
				stringFormatted,
				stringFormatted2,
				stringFormatted3
			});
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x000AFD30 File Offset: 0x000ADF30
		public void AddSurvivorPanel(CharacterBody bodyPrefabComponent)
		{
			string statDisplayValue = this.statSheet.GetStatDisplayValue(PerBodyStatDef.longestRun.FindStatDef(bodyPrefabComponent.name));
			ulong statValueULong = this.statSheet.GetStatValueULong(PerBodyStatDef.timesPicked.FindStatDef(bodyPrefabComponent.name));
			ulong statValueULong2 = this.statSheet.GetStatValueULong(StatDef.totalGamesPlayed);
			double num = 0.0;
			if (statValueULong2 != 0UL)
			{
				num = statValueULong / statValueULong2 * 100.0;
			}
			PageBuilder.sharedStringBuilder.Clear();
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_LONGESTRUN", new object[]
			{
				statDisplayValue
			}));
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_TIMESPICKED", new object[]
			{
				statValueULong
			}));
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_PICKPERCENTAGE", new object[]
			{
				num
			}));
			this.AddSimpleTextPanel(PageBuilder.sharedStringBuilder.ToString());
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x0001AF7C File Offset: 0x0001917C
		public void AddSimpleBody(CharacterBody bodyPrefabComponent)
		{
			this.AddBodyStatsPanel(bodyPrefabComponent);
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x000AFE28 File Offset: 0x000AE028
		public void AddStagePanel(SceneDef sceneDef)
		{
			string statDisplayValue = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesVisited.FindStatDef(sceneDef.sceneName));
			string statDisplayValue2 = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesCleared.FindStatDef(sceneDef.sceneName));
			string stringFormatted = Language.GetStringFormatted("STAGE_PREFIX_TOTALTIMESVISITED", new object[]
			{
				statDisplayValue
			});
			string stringFormatted2 = Language.GetStringFormatted("STAGE_PREFIX_TOTALTIMESCLEARED", new object[]
			{
				statDisplayValue2
			});
			PageBuilder.sharedStringBuilder.Clear();
			PageBuilder.sharedStringBuilder.Append(stringFormatted);
			PageBuilder.sharedStringBuilder.Append("\n");
			PageBuilder.sharedStringBuilder.Append(stringFormatted2);
			this.AddSimpleTextPanel(PageBuilder.sharedStringBuilder.ToString());
		}

		// Token: 0x0600250C RID: 9484 RVA: 0x000AFEE4 File Offset: 0x000AE0E4
		public static void Stage(PageBuilder builder)
		{
			SceneDef sceneDef = (SceneDef)builder.entry.extraData;
			builder.AddStagePanel(sceneDef);
			builder.AddNotesPanel(Language.IsTokenInvalid(sceneDef.loreToken) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(sceneDef.loreToken));
		}

		// Token: 0x0600250D RID: 9485 RVA: 0x0001AF85 File Offset: 0x00019185
		public static void SimplePickup(PageBuilder builder)
		{
			builder.AddSimplePickup((PickupIndex)builder.entry.extraData);
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0001AF9D File Offset: 0x0001919D
		public static void SimpleBody(PageBuilder builder)
		{
			builder.AddSimpleBody((CharacterBody)builder.entry.extraData);
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x000AFF34 File Offset: 0x000AE134
		public static void MonsterBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddMonsterPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x000AFF70 File Offset: 0x000AE170
		public static void SurvivorBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddSurvivorPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x000AFFAC File Offset: 0x000AE1AC
		public void AddRunReportPanel(RunReport runReport)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/GameEndReportPanel"), this.container);
			gameObject.GetComponent<GameEndReportPanelController>().SetDisplayData(new GameEndReportPanelController.DisplayData
			{
				runReport = runReport,
				playerIndex = 0
			});
			gameObject.GetComponent<MPEventSystemProvider>().fallBackToMainEventSystem = true;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0001AFB5 File Offset: 0x000191B5
		public static void RunReportPanel(PageBuilder builder)
		{
			builder.AddRunReportPanel(RunReport.Load("PreviousRun"));
		}

		// Token: 0x04002821 RID: 10273
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x04002822 RID: 10274
		public UserProfile userProfile;

		// Token: 0x04002823 RID: 10275
		public RectTransform container;

		// Token: 0x04002824 RID: 10276
		public Entry entry;

		// Token: 0x04002825 RID: 10277
		public readonly List<GameObject> managedObjects = new List<GameObject>();
	}
}
