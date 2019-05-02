using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Stats;
using TMPro;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200068D RID: 1677
	public class PageBuilder
	{
		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06002597 RID: 9623 RVA: 0x0001B657 File Offset: 0x00019857
		private StatSheet statSheet
		{
			get
			{
				return this.userProfile.statSheet;
			}
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x000B1068 File Offset: 0x000AF268
		public void Destroy()
		{
			foreach (GameObject obj in this.managedObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x06002599 RID: 9625 RVA: 0x000B10B8 File Offset: 0x000AF2B8
		public void AddSimpleTextPanel(string text)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Logbook/SimpleTextPanel"), this.container);
			gameObject.GetComponent<ChildLocator>().FindChild("MainLabel").GetComponent<TextMeshProUGUI>().text = text;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x0600259A RID: 9626 RVA: 0x0001B664 File Offset: 0x00019864
		public void AddSimpleTextPanel(params string[] textLines)
		{
			this.AddSimpleTextPanel(string.Join("\n", textLines));
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x000B1104 File Offset: 0x000AF304
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

		// Token: 0x0600259C RID: 9628 RVA: 0x0001B677 File Offset: 0x00019877
		public void AddDescriptionPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("DESCRIPTION_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x0001B693 File Offset: 0x00019893
		public void AddNotesPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("NOTES_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x000B1284 File Offset: 0x000AF484
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

		// Token: 0x0600259F RID: 9631 RVA: 0x000B1354 File Offset: 0x000AF554
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

		// Token: 0x060025A0 RID: 9632 RVA: 0x000B1420 File Offset: 0x000AF620
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

		// Token: 0x060025A1 RID: 9633 RVA: 0x0001B6AF File Offset: 0x000198AF
		public void AddSimpleBody(CharacterBody bodyPrefabComponent)
		{
			this.AddBodyStatsPanel(bodyPrefabComponent);
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x000B1518 File Offset: 0x000AF718
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

		// Token: 0x060025A3 RID: 9635 RVA: 0x000B15D4 File Offset: 0x000AF7D4
		public static void Stage(PageBuilder builder)
		{
			SceneDef sceneDef = (SceneDef)builder.entry.extraData;
			builder.AddStagePanel(sceneDef);
			builder.AddNotesPanel(Language.IsTokenInvalid(sceneDef.loreToken) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(sceneDef.loreToken));
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x0001B6B8 File Offset: 0x000198B8
		public static void SimplePickup(PageBuilder builder)
		{
			builder.AddSimplePickup((PickupIndex)builder.entry.extraData);
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x0001B6D0 File Offset: 0x000198D0
		public static void SimpleBody(PageBuilder builder)
		{
			builder.AddSimpleBody((CharacterBody)builder.entry.extraData);
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x000B1624 File Offset: 0x000AF824
		public static void MonsterBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddMonsterPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x000B1660 File Offset: 0x000AF860
		public static void SurvivorBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddSurvivorPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x000B169C File Offset: 0x000AF89C
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

		// Token: 0x060025A9 RID: 9641 RVA: 0x0001B6E8 File Offset: 0x000198E8
		public static void RunReportPanel(PageBuilder builder)
		{
			builder.AddRunReportPanel(RunReport.Load("PreviousRun"));
		}

		// Token: 0x0400287D RID: 10365
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x0400287E RID: 10366
		public UserProfile userProfile;

		// Token: 0x0400287F RID: 10367
		public RectTransform container;

		// Token: 0x04002880 RID: 10368
		public Entry entry;

		// Token: 0x04002881 RID: 10369
		public readonly List<GameObject> managedObjects = new List<GameObject>();
	}
}
