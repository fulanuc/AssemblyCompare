using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x0200050D RID: 1293
	internal class StatManager
	{
		// Token: 0x06001D68 RID: 7528 RVA: 0x0008F424 File Offset: 0x0008D624
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onServerDamageDealt += StatManager.OnDamageDealt;
			GlobalEventManager.onCharacterDeathGlobal += StatManager.OnCharacterDeath;
			HealthComponent.onCharacterHealServer += StatManager.OnCharacterHeal;
			Run.onPlayerFirstCreatedServer += StatManager.OnPlayerFirstCreatedServer;
			Run.OnServerGameOver += StatManager.OnServerGameOver;
			Stage.onServerStageComplete += StatManager.OnServerStageComplete;
			Stage.onServerStageBegin += StatManager.OnServerStageBegin;
			Inventory.onServerItemGiven += StatManager.OnServerItemGiven;
			RoR2Application.onFixedUpdate += StatManager.ProcessEvents;
			EquipmentSlot.onServerEquipmentActivated += StatManager.OnEquipmentActivated;
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x0008F4DC File Offset: 0x0008D6DC
		private static void OnServerGameOver(Run run, GameResultType result)
		{
			if (result != GameResultType.Lost)
			{
				foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
				{
					if (playerStatsComponent.playerCharacterMasterController.isConnected)
					{
						StatSheet currentStats = playerStatsComponent.currentStats;
						PerBodyStatDef totalWins = PerBodyStatDef.totalWins;
						GameObject bodyPrefab = playerStatsComponent.characterMaster.bodyPrefab;
						currentStats.PushStatValue(totalWins.FindStatDef(((bodyPrefab != null) ? bodyPrefab.name : null) ?? ""), 1UL);
					}
				}
			}
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x0001592B File Offset: 0x00013B2B
		private static void OnPlayerFirstCreatedServer(Run run, PlayerCharacterMasterController playerCharacterMasterController)
		{
			playerCharacterMasterController.master.onBodyStart += StatManager.OnBodyFirstStart;
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x0008F570 File Offset: 0x0008D770
		private static void OnBodyFirstStart(CharacterBody body)
		{
			CharacterMaster master = body.master;
			if (master)
			{
				master.onBodyStart -= StatManager.OnBodyFirstStart;
				PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					PlayerStatsComponent component2 = component.GetComponent<PlayerStatsComponent>();
					if (component2)
					{
						StatSheet currentStats = component2.currentStats;
						currentStats.PushStatValue(PerBodyStatDef.timesPicked.FindStatDef(body.name), 1UL);
						currentStats.PushStatValue(StatDef.totalGamesPlayed, 1UL);
					}
				}
			}
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x00015944 File Offset: 0x00013B44
		private static void ProcessEvents()
		{
			StatManager.ProcessDamageEvents();
			StatManager.ProcessDeathEvents();
			StatManager.ProcessHealingEvents();
			StatManager.ProcessGoldEvents();
			StatManager.ProcessItemCollectedEvents();
			StatManager.ProcessCharacterUpdateEvents();
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x0008F5E8 File Offset: 0x0008D7E8
		public static void OnCharacterHeal(HealthComponent healthComponent, float amount)
		{
			StatManager.healingEvents.Enqueue(new StatManager.HealingEvent
			{
				healee = healthComponent.gameObject,
				healAmount = amount
			});
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x0008F620 File Offset: 0x0008D820
		public static void OnDamageDealt(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			StatManager.damageEvents.Enqueue(new StatManager.DamageEvent
			{
				attacker = damageInfo.attacker,
				attackerName = (damageInfo.attacker ? damageInfo.attacker.name : null),
				victim = damageReport.victim.gameObject,
				victimName = damageReport.victim.gameObject.name,
				victimIsElite = (damageReport.victimBody && damageReport.victimBody.isElite),
				damageDealt = damageReport.damageInfo.damage
			});
		}

		// Token: 0x06001D6F RID: 7535 RVA: 0x0008F6D4 File Offset: 0x0008D8D4
		public static void OnCharacterDeath(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			StatManager.deathEvents.Enqueue(new StatManager.DamageEvent
			{
				attacker = damageInfo.attacker,
				attackerName = (damageInfo.attacker ? damageInfo.attacker.name : null),
				victim = damageReport.victim.gameObject,
				victimName = damageReport.victim.gameObject.name,
				victimIsElite = (damageReport.victimBody && damageReport.victimBody.isElite),
				damageDealt = damageReport.damageInfo.damage
			});
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x0008F788 File Offset: 0x0008D988
		private static void ProcessHealingEvents()
		{
			while (StatManager.healingEvents.Count > 0)
			{
				StatManager.HealingEvent healingEvent = StatManager.healingEvents.Dequeue();
				ulong statValue = (ulong)healingEvent.healAmount;
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(healingEvent.healee);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalHealthHealed, statValue);
				}
			}
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x0008F7D0 File Offset: 0x0008D9D0
		private static void ProcessDamageEvents()
		{
			while (StatManager.damageEvents.Count > 0)
			{
				StatManager.DamageEvent damageEvent = StatManager.damageEvents.Dequeue();
				ulong statValue = (ulong)damageEvent.damageDealt;
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(damageEvent.victim);
				StatSheet statSheet2 = PlayerStatsComponent.FindBodyStatSheet(damageEvent.attacker);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDamageTaken, statValue);
					if (damageEvent.attackerName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenFrom, damageEvent.attackerName, statValue);
					}
					if (damageEvent.victimName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenAs, damageEvent.victimName, statValue);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalDamageDealt, statValue);
					statSheet2.PushStatValue(StatDef.highestDamageDealt, statValue);
					if (damageEvent.attackerName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtAs, damageEvent.attackerName, statValue);
					}
					if (damageEvent.victimName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtTo, damageEvent.victimName, statValue);
					}
				}
			}
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x0008F8B0 File Offset: 0x0008DAB0
		private static void ProcessDeathEvents()
		{
			while (StatManager.deathEvents.Count > 0)
			{
				StatManager.DamageEvent damageEvent = StatManager.deathEvents.Dequeue();
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(damageEvent.victim);
				StatSheet statSheet2 = PlayerStatsComponent.FindBodyStatSheet(damageEvent.attacker);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDeaths, 1UL);
					statSheet.PushStatValue(PerBodyStatDef.deathsAs, damageEvent.victimName, 1UL);
					if (damageEvent.attackerName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.deathsFrom, damageEvent.attackerName, 1UL);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalKills, 1UL);
					statSheet2.PushStatValue(PerBodyStatDef.killsAs, damageEvent.attackerName, 1UL);
					if (damageEvent.victimName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.killsAgainst, damageEvent.victimName, 1UL);
						if (damageEvent.victimIsElite)
						{
							statSheet2.PushStatValue(PerBodyStatDef.killsAgainstElite, damageEvent.victimName, 1UL);
						}
					}
				}
			}
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x0008F98C File Offset: 0x0008DB8C
		public static void OnGoldCollected(CharacterMaster characterMaster, ulong amount)
		{
			StatManager.goldCollectedEvents.Enqueue(new StatManager.GoldEvent
			{
				characterMaster = characterMaster,
				amount = amount
			});
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x0008F9BC File Offset: 0x0008DBBC
		private static void ProcessGoldEvents()
		{
			while (StatManager.goldCollectedEvents.Count > 0)
			{
				StatManager.GoldEvent goldEvent = StatManager.goldCollectedEvents.Dequeue();
				CharacterMaster characterMaster = goldEvent.characterMaster;
				StatSheet statSheet;
				if (characterMaster == null)
				{
					statSheet = null;
				}
				else
				{
					PlayerStatsComponent component = characterMaster.GetComponent<PlayerStatsComponent>();
					statSheet = ((component != null) ? component.currentStats : null);
				}
				StatSheet statSheet2 = statSheet;
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.goldCollected, goldEvent.amount);
					statSheet2.PushStatValue(StatDef.maxGoldCollected, statSheet2.GetStatValueULong(StatDef.goldCollected));
				}
			}
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x0008FA2C File Offset: 0x0008DC2C
		public static void OnPurchase<T>(CharacterBody characterBody, CostType costType, T statDefsToIncrement) where T : IEnumerable<StatDef>
		{
			StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(characterBody);
			if (statSheet == null)
			{
				return;
			}
			StatDef statDef = null;
			StatDef statDef2 = null;
			switch (costType)
			{
			case CostType.Money:
				statDef = StatDef.totalGoldPurchases;
				statDef2 = StatDef.highestGoldPurchases;
				break;
			case CostType.PercentHealth:
				statDef = StatDef.totalBloodPurchases;
				statDef2 = StatDef.highestBloodPurchases;
				break;
			case CostType.Lunar:
				statDef = StatDef.totalLunarPurchases;
				statDef2 = StatDef.highestLunarPurchases;
				break;
			case CostType.WhiteItem:
				statDef = StatDef.totalTier1Purchases;
				statDef2 = StatDef.highestTier1Purchases;
				break;
			case CostType.GreenItem:
				statDef = StatDef.totalTier2Purchases;
				statDef2 = StatDef.highestTier2Purchases;
				break;
			case CostType.RedItem:
				statDef = StatDef.totalTier3Purchases;
				statDef2 = StatDef.highestTier3Purchases;
				break;
			}
			statSheet.PushStatValue(StatDef.totalPurchases, 1UL);
			statSheet.PushStatValue(StatDef.highestPurchases, statSheet.GetStatValueULong(StatDef.totalPurchases));
			if (statDef != null)
			{
				statSheet.PushStatValue(statDef, 1UL);
				if (statDef2 != null)
				{
					statSheet.PushStatValue(statDef2, statSheet.GetStatValueULong(statDef));
				}
			}
			if (statDefsToIncrement != null)
			{
				foreach (StatDef statDef3 in statDefsToIncrement)
				{
					if (statDef3 != null)
					{
						statSheet.PushStatValue(statDef3, 1UL);
					}
				}
			}
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x00015964 File Offset: 0x00013B64
		public static void OnEquipmentActivated(EquipmentSlot activator, EquipmentIndex equipmentIndex)
		{
			StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(activator.characterBody);
			if (statSheet == null)
			{
				return;
			}
			statSheet.PushStatValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex), 1UL);
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00015988 File Offset: 0x00013B88
		public static void PushCharacterUpdateEvent(StatManager.CharacterUpdateEvent e)
		{
			StatManager.characterUpdateEvents.Enqueue(e);
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x0008FB50 File Offset: 0x0008DD50
		private static void ProcessCharacterUpdateEvents()
		{
			while (StatManager.characterUpdateEvents.Count > 0)
			{
				StatManager.CharacterUpdateEvent characterUpdateEvent = StatManager.characterUpdateEvents.Dequeue();
				if (characterUpdateEvent.statsComponent)
				{
					StatSheet currentStats = characterUpdateEvent.statsComponent.currentStats;
					if (currentStats != null)
					{
						GameObject bodyObject = characterUpdateEvent.statsComponent.characterMaster.GetBodyObject();
						string text = null;
						if (bodyObject)
						{
							text = bodyObject.name;
						}
						currentStats.PushStatValue(StatDef.totalTimeAlive, (double)characterUpdateEvent.additionalTimeAlive);
						currentStats.PushStatValue(StatDef.highestLevel, (ulong)((long)characterUpdateEvent.level));
						currentStats.PushStatValue(StatDef.totalDistanceTraveled, (double)characterUpdateEvent.additionalDistanceTraveled);
						if (text != null)
						{
							currentStats.PushStatValue(PerBodyStatDef.totalTimeAlive, text, (double)characterUpdateEvent.additionalTimeAlive);
							currentStats.PushStatValue(PerBodyStatDef.longestRun, text, (double)characterUpdateEvent.runTime);
						}
						EquipmentIndex currentEquipmentIndex = characterUpdateEvent.statsComponent.characterMaster.inventory.currentEquipmentIndex;
						if (currentEquipmentIndex != EquipmentIndex.None)
						{
							currentStats.PushStatValue(PerEquipmentStatDef.totalTimeHeld.FindStatDef(currentEquipmentIndex), (double)characterUpdateEvent.additionalTimeAlive);
						}
					}
				}
			}
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0008FC58 File Offset: 0x0008DE58
		private static void OnServerItemGiven(Inventory inventory, ItemIndex itemIndex, int quantity)
		{
			StatManager.itemCollectedEvents.Enqueue(new StatManager.ItemCollectedEvent
			{
				inventory = inventory,
				itemIndex = itemIndex,
				quantity = quantity,
				newCount = inventory.GetItemCount(itemIndex)
			});
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0008FCA0 File Offset: 0x0008DEA0
		private static void ProcessItemCollectedEvents()
		{
			while (StatManager.itemCollectedEvents.Count > 0)
			{
				StatManager.ItemCollectedEvent itemCollectedEvent = StatManager.itemCollectedEvents.Dequeue();
				if (itemCollectedEvent.inventory)
				{
					PlayerStatsComponent component = itemCollectedEvent.inventory.GetComponent<PlayerStatsComponent>();
					StatSheet statSheet = (component != null) ? component.currentStats : null;
					if (statSheet != null)
					{
						statSheet.PushStatValue(StatDef.totalItemsCollected, (ulong)((long)itemCollectedEvent.quantity));
						statSheet.PushStatValue(StatDef.highestItemsCollected, statSheet.GetStatValueULong(StatDef.totalItemsCollected));
						statSheet.PushStatValue(PerItemStatDef.totalCollected.FindStatDef(itemCollectedEvent.itemIndex), (ulong)((long)itemCollectedEvent.quantity));
						statSheet.PushStatValue(PerItemStatDef.highestCollected.FindStatDef(itemCollectedEvent.itemIndex), (ulong)((long)itemCollectedEvent.newCount));
					}
				}
			}
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x0008FD58 File Offset: 0x0008DF58
		private static void OnServerStageBegin(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					StatDef statDef = PerStageStatDef.totalTimesVisited.FindStatDef(stage.sceneDef ? stage.sceneDef.sceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x0008FDF0 File Offset: 0x0008DFF0
		private static void OnServerStageComplete(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					currentStats.PushStatValue(StatDef.totalStagesCompleted, 1UL);
					currentStats.PushStatValue(StatDef.highestStagesCompleted, currentStats.GetStatValueULong(StatDef.totalStagesCompleted));
					StatDef statDef = PerStageStatDef.totalTimesCleared.FindStatDef(stage.sceneDef ? stage.sceneDef.sceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x04001F60 RID: 8032
		private static readonly Queue<StatManager.DamageEvent> damageEvents = new Queue<StatManager.DamageEvent>();

		// Token: 0x04001F61 RID: 8033
		private static readonly Queue<StatManager.DamageEvent> deathEvents = new Queue<StatManager.DamageEvent>();

		// Token: 0x04001F62 RID: 8034
		private static readonly Queue<StatManager.HealingEvent> healingEvents = new Queue<StatManager.HealingEvent>();

		// Token: 0x04001F63 RID: 8035
		private static readonly Queue<StatManager.GoldEvent> goldCollectedEvents = new Queue<StatManager.GoldEvent>();

		// Token: 0x04001F64 RID: 8036
		private static readonly Queue<StatManager.PurchaseStatEvent> purchaseStatEvents = new Queue<StatManager.PurchaseStatEvent>();

		// Token: 0x04001F65 RID: 8037
		private static readonly Queue<StatManager.CharacterUpdateEvent> characterUpdateEvents = new Queue<StatManager.CharacterUpdateEvent>();

		// Token: 0x04001F66 RID: 8038
		private static readonly Queue<StatManager.ItemCollectedEvent> itemCollectedEvents = new Queue<StatManager.ItemCollectedEvent>();

		// Token: 0x0200050E RID: 1294
		private struct DamageEvent
		{
			// Token: 0x04001F67 RID: 8039
			[CanBeNull]
			public GameObject attacker;

			// Token: 0x04001F68 RID: 8040
			[CanBeNull]
			public string attackerName;

			// Token: 0x04001F69 RID: 8041
			[CanBeNull]
			public GameObject victim;

			// Token: 0x04001F6A RID: 8042
			[NotNull]
			public string victimName;

			// Token: 0x04001F6B RID: 8043
			public bool victimIsElite;

			// Token: 0x04001F6C RID: 8044
			public float damageDealt;
		}

		// Token: 0x0200050F RID: 1295
		private struct HealingEvent
		{
			// Token: 0x04001F6D RID: 8045
			[CanBeNull]
			public GameObject healee;

			// Token: 0x04001F6E RID: 8046
			public float healAmount;
		}

		// Token: 0x02000510 RID: 1296
		private struct GoldEvent
		{
			// Token: 0x04001F6F RID: 8047
			[CanBeNull]
			public CharacterMaster characterMaster;

			// Token: 0x04001F70 RID: 8048
			public ulong amount;
		}

		// Token: 0x02000511 RID: 1297
		private struct PurchaseStatEvent
		{
		}

		// Token: 0x02000512 RID: 1298
		public struct CharacterUpdateEvent
		{
			// Token: 0x04001F71 RID: 8049
			public PlayerStatsComponent statsComponent;

			// Token: 0x04001F72 RID: 8050
			public float additionalDistanceTraveled;

			// Token: 0x04001F73 RID: 8051
			public float additionalTimeAlive;

			// Token: 0x04001F74 RID: 8052
			public int level;

			// Token: 0x04001F75 RID: 8053
			public float runTime;
		}

		// Token: 0x02000513 RID: 1299
		private struct ItemCollectedEvent
		{
			// Token: 0x04001F76 RID: 8054
			[CanBeNull]
			public Inventory inventory;

			// Token: 0x04001F77 RID: 8055
			public ItemIndex itemIndex;

			// Token: 0x04001F78 RID: 8056
			public int quantity;

			// Token: 0x04001F79 RID: 8057
			public int newCount;
		}
	}
}
