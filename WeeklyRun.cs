using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000427 RID: 1063
	public class WeeklyRun : Run
	{
		// Token: 0x060017CD RID: 6093 RVA: 0x0007B07C File Offset: 0x0007927C
		public static uint GetCurrentSeedCycle()
		{
			return (uint)((WeeklyRun.now - WeeklyRun.startDate).Days / 3);
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x0007B0A4 File Offset: 0x000792A4
		public static DateTime GetSeedCycleStartDateTime(uint seedCycle)
		{
			return WeeklyRun.startDate.AddDays(seedCycle * 3u);
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060017CF RID: 6095 RVA: 0x00011DAB File Offset: 0x0000FFAB
		public static DateTime now
		{
			get
			{
				return Util.UnixTimeStampToDateTimeUtc(Client.Instance.Utils.GetServerRealTime());
			}
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x00011DC1 File Offset: 0x0000FFC1
		protected new void Start()
		{
			base.Start();
			this.bossAffixRng = new Xoroshiro128Plus(this.runRNG.nextUlong);
			if (NetworkServer.active)
			{
				this.NetworkserverSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			}
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x0007B0C4 File Offset: 0x000792C4
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			if (TeleporterInteraction.instance)
			{
				bool flag = this.crystalsRequiredToKill > this.crystalsKilled;
				if (flag != TeleporterInteraction.instance.locked)
				{
					if (flag)
					{
						if (NetworkServer.active)
						{
							TeleporterInteraction.instance.Networklocked = true;
							return;
						}
					}
					else
					{
						if (NetworkServer.active)
						{
							TeleporterInteraction.instance.Networklocked = false;
						}
						ChildLocator component = TeleporterInteraction.instance.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
						if (component)
						{
							Transform transform = component.FindChild("TimeCrystalBeaconBlocker");
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/TimeCrystalDeath"), new EffectData
							{
								origin = transform.transform.position
							}, false);
							transform.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00011DF1 File Offset: 0x0000FFF1
		protected override void OverrideSeed()
		{
			this.seed = (ulong)WeeklyRun.GetCurrentSeedCycle();
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x000025DA File Offset: 0x000007DA
		public override void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00011DFF File Offset: 0x0000FFFF
		public override void AdvanceStage(string nextSceneName)
		{
			if (this.stageClearCount == 1 && SceneInfo.instance.countsAsStage)
			{
				base.BeginGameOver(GameResultType.Won);
				return;
			}
			base.AdvanceStage(nextSceneName);
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00011E25 File Offset: 0x00010025
		public override void OnClientGameOver(RunReport runReport)
		{
			base.OnClientGameOver(runReport);
			this.ClientSubmitLeaderboardScore(runReport);
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x0007B190 File Offset: 0x00079390
		public override void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
			base.OnServerBossAdded(bossGroup, characterMaster);
			if (this.stageClearCount >= 1)
			{
				if (characterMaster.inventory.GetEquipmentIndex() == EquipmentIndex.None)
				{
					characterMaster.inventory.SetEquipmentIndex(WeeklyRun.affixes[this.bossAffixRng.RangeInt(0, WeeklyRun.affixes.Length)]);
				}
				characterMaster.inventory.GiveItem(ItemIndex.BoostHp, 5);
				characterMaster.inventory.GiveItem(ItemIndex.BoostDamage, 1);
			}
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x00011E35 File Offset: 0x00010035
		public override void OnServerBossKilled(bool bossGroupDefeated)
		{
			base.OnServerBossKilled(bossGroupDefeated);
			if (TeleporterInteraction.instance && bossGroupDefeated)
			{
				TeleporterInteraction.instance.remainingChargeTimer = 0f;
			}
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x00011E5B File Offset: 0x0001005B
		public override GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutCrystalBoom");
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060017D9 RID: 6105 RVA: 0x00011E67 File Offset: 0x00010067
		public uint crystalsKilled
		{
			get
			{
				return (uint)((ulong)this.crystalCount - (ulong)((long)this.crystalActiveList.Count));
			}
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x0007B1FC File Offset: 0x000793FC
		public override void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
			base.OnServerTeleporterPlaced(sceneDirector, teleporter);
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
			directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
			int num = 0;
			while ((long)num < (long)((ulong)this.crystalCount))
			{
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(this.crystalSpawnCard, directorPlacementRule, this.stageRng);
				if (gameObject)
				{
					DeathRewards component3 = gameObject.GetComponent<DeathRewards>();
					if (component3)
					{
						component3.goldReward = this.crystalRewardValue;
					}
				}
				this.crystalActiveList.Add(OnDestroyCallback.AddCallback(gameObject, delegate(OnDestroyCallback component)
				{
					this.crystalActiveList.Remove(component);
				}));
				num++;
			}
			if (TeleporterInteraction.instance)
			{
				ChildLocator component2 = TeleporterInteraction.instance.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					component2.FindChild("TimeCrystalProps").gameObject.SetActive(true);
					component2.FindChild("TimeCrystalBeaconBlocker").gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x0007B2E4 File Offset: 0x000794E4
		public override void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
			if (this.stageClearCount == 0)
			{
				SpawnPoint spawnPoint = SpawnPoint.readOnlyInstancesList[0];
				if (spawnPoint)
				{
					float num = 360f / this.equipmentBarrelCount;
					int num2 = 0;
					while ((long)num2 < (long)((ulong)this.equipmentBarrelCount))
					{
						Vector3 b = Quaternion.AngleAxis(num * (float)num2, Vector3.up) * (Vector3.forward * this.equipmentBarrelRadius);
						DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule();
						directorPlacementRule.minDistance = 0f;
						directorPlacementRule.maxDistance = 3f;
						directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
						directorPlacementRule.position = spawnPoint.transform.position + b;
						DirectorCore.instance.TrySpawnObject(this.equipmentBarrelSpawnCard, directorPlacementRule, this.stageRng);
						num2++;
					}
				}
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x00011E7E File Offset: 0x0001007E
		public static string GetLeaderboardName(int playerCount, uint seedCycle)
		{
			return string.Format(CultureInfo.InvariantCulture, "weekly{0}p{1}", playerCount, seedCycle);
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x0007B3B8 File Offset: 0x000795B8
		protected void ClientSubmitLeaderboardScore(RunReport runReport)
		{
			if (runReport.gameResultType != GameResultType.Won)
			{
				return;
			}
			bool flag = false;
			using (IEnumerator<NetworkUser> enumerator = NetworkUser.readOnlyLocalPlayersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isParticipating)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			int num = PlayerCharacterMasterController.instances.Count;
			if (num <= 0)
			{
				return;
			}
			if (num >= 3)
			{
				if (num > 4)
				{
					return;
				}
				num = 4;
			}
			string name = WeeklyRun.GetLeaderboardName(num, this.serverSeedCycle);
			int[] subScores = new int[64];
			GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(NetworkUser.readOnlyLocalPlayersList[0].bodyIndexPreference);
			if (!bodyPrefab)
			{
				return;
			}
			SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
			if (survivorDef == null)
			{
				return;
			}
			subScores[1] = (int)survivorDef.survivorIndex;
			Leaderboard leaderboard = Client.Instance.GetLeaderboard(name, Client.LeaderboardSortMethod.Ascending, Client.LeaderboardDisplayType.TimeMilliSeconds);
			leaderboard.OnBoardInformation = delegate()
			{
				leaderboard.AddScore(true, (int)Math.Ceiling((double)runReport.runStopwatchValue * 1000.0), subScores);
			};
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0007B4CC File Offset: 0x000796CC
		public override void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
			base.OverrideRuleChoices(mustInclude, mustExclude);
			base.ForceChoice(mustInclude, mustExclude, "Difficulty.Normal");
			base.ForceChoice(mustInclude, mustExclude, "Misc.StartingMoney.50");
			base.ForceChoice(mustInclude, mustExclude, "Misc.StageOrder.Random");
			base.ForceChoice(mustInclude, mustExclude, "Misc.KeepMoneyBetweenStages.Off");
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				RuleDef ruleDef = RuleCatalog.FindRuleDef(artifactIndex.ToString());
				RuleChoiceDef ruleChoiceDef = (ruleDef != null) ? ruleDef.FindChoice("Off") : null;
				if (ruleChoiceDef != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef);
				}
			}
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				RuleDef ruleDef2 = RuleCatalog.FindRuleDef("Items." + itemIndex.ToString());
				RuleChoiceDef ruleChoiceDef2 = (ruleDef2 != null) ? ruleDef2.FindChoice("On") : null;
				if (ruleChoiceDef2 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef2);
				}
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				RuleDef ruleDef3 = RuleCatalog.FindRuleDef("Equipment." + equipmentIndex.ToString());
				RuleChoiceDef ruleChoiceDef3 = (ruleDef3 != null) ? ruleDef3.FindChoice("On") : null;
				if (ruleChoiceDef3 != null)
				{
					base.ForceChoice(mustInclude, mustExclude, ruleChoiceDef3);
				}
			}
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override bool IsUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00003696 File Offset: 0x00001896
		public override bool CanUnlockableBeGrantedThisRun(string unlockableName)
		{
			return false;
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override bool DoesEveryoneHaveThisUnlockableUnlocked(string unlockableName)
		{
			return true;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00011E9B File Offset: 0x0001009B
		protected override void HandlePostRunDestination()
		{
			Console.instance.SubmitCmd(null, "transition_command \"disconnect\";", false);
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00011EAE File Offset: 0x000100AE
		protected override bool ShouldUpdateRunStopwatch()
		{
			return base.livingPlayerCount > 0;
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060017E8 RID: 6120 RVA: 0x0007B5E8 File Offset: 0x000797E8
		// (set) Token: 0x060017E9 RID: 6121 RVA: 0x00011F2F File Offset: 0x0001012F
		public uint NetworkserverSeedCycle
		{
			get
			{
				return this.serverSeedCycle;
			}
			set
			{
				base.SetSyncVar<uint>(value, ref this.serverSeedCycle, 128u);
			}
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x0007B5FC File Offset: 0x000797FC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			if (forceAll)
			{
				writer.WritePackedUInt32(this.serverSeedCycle);
				return true;
			}
			bool flag2 = false;
			if ((base.syncVarDirtyBits & 128u) != 0u)
			{
				if (!flag2)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag2 = true;
				}
				writer.WritePackedUInt32(this.serverSeedCycle);
			}
			if (!flag2)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag2 || flag;
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x0007B674 File Offset: 0x00079874
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 128) != 0)
			{
				this.serverSeedCycle = reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001AC8 RID: 6856
		private Xoroshiro128Plus bossAffixRng;

		// Token: 0x04001AC9 RID: 6857
		public static readonly DateTime startDate = new DateTime(2018, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x04001ACA RID: 6858
		public const int cycleLength = 3;

		// Token: 0x04001ACB RID: 6859
		private string leaderboardName;

		// Token: 0x04001ACC RID: 6860
		[SyncVar]
		private uint serverSeedCycle;

		// Token: 0x04001ACD RID: 6861
		private static readonly EquipmentIndex[] affixes = new EquipmentIndex[]
		{
			EquipmentIndex.AffixBlue,
			EquipmentIndex.AffixRed
		};

		// Token: 0x04001ACE RID: 6862
		public SpawnCard crystalSpawnCard;

		// Token: 0x04001ACF RID: 6863
		public uint crystalCount = 3u;

		// Token: 0x04001AD0 RID: 6864
		public uint crystalRewardValue = 50u;

		// Token: 0x04001AD1 RID: 6865
		public uint crystalsRequiredToKill = 3u;

		// Token: 0x04001AD2 RID: 6866
		private List<OnDestroyCallback> crystalActiveList = new List<OnDestroyCallback>();

		// Token: 0x04001AD3 RID: 6867
		public SpawnCard equipmentBarrelSpawnCard;

		// Token: 0x04001AD4 RID: 6868
		public uint equipmentBarrelCount = 3u;

		// Token: 0x04001AD5 RID: 6869
		public float equipmentBarrelRadius = 10f;
	}
}
