using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using RoR2.ConVar;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003BE RID: 958
	[RequireComponent(typeof(NetworkRuleBook))]
	[DisallowMultipleComponent]
	public class Run : NetworkBehaviour
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x0000F6F1 File Offset: 0x0000D8F1
		// (set) Token: 0x0600145D RID: 5213 RVA: 0x0000F6F8 File Offset: 0x0000D8F8
		public static Run instance { get; private set; }

		// Token: 0x0600145E RID: 5214 RVA: 0x0000F700 File Offset: 0x0000D900
		private void OnEnable()
		{
			Run.instance = SingletonHelper.Assign<Run>(Run.instance, this);
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0000F712 File Offset: 0x0000D912
		private void OnDisable()
		{
			Run.instance = SingletonHelper.Unassign<Run>(Run.instance, this);
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0000F724 File Offset: 0x0000D924
		protected void Awake()
		{
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x0000F732 File Offset: 0x0000D932
		public RuleBook ruleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00070548 File Offset: 0x0006E748
		[Server]
		public void SetRuleBook(RuleBook newRuleBook)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetRuleBook(RoR2.RuleBook)' called on client");
				return;
			}
			this.networkRuleBookComponent.SetRuleBook(newRuleBook);
			this.selectedDifficulty = newRuleBook.FindDifficulty();
			this.NetworkenabledArtifacts = newRuleBook.GenerateArtifactMask();
			this.NetworkavailableItems = newRuleBook.GenerateItemMask();
			this.NetworkavailableEquipment = newRuleBook.GenerateEquipmentMask();
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x000705A8 File Offset: 0x0006E7A8
		private void GenerateStageRNG()
		{
			this.stageRng = new Xoroshiro128Plus(this.stageRngGenerator.nextUlong);
			this.bossRewardRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.treasureRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.spawnRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x0000F73F File Offset: 0x0000D93F
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0000F747 File Offset: 0x0000D947
		public DifficultyIndex selectedDifficulty
		{
			get
			{
				return (DifficultyIndex)this.selectedDifficultyInternal;
			}
			set
			{
				this.NetworkselectedDifficultyInternal = (int)value;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x0000F750 File Offset: 0x0000D950
		// (set) Token: 0x06001467 RID: 5223 RVA: 0x0000F758 File Offset: 0x0000D958
		public int livingPlayerCount { get; private set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06001468 RID: 5224 RVA: 0x0000F761 File Offset: 0x0000D961
		// (set) Token: 0x06001469 RID: 5225 RVA: 0x0000F769 File Offset: 0x0000D969
		public int participatingPlayerCount { get; private set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600146A RID: 5226 RVA: 0x0000F772 File Offset: 0x0000D972
		// (set) Token: 0x0600146B RID: 5227 RVA: 0x0000F77A File Offset: 0x0000D97A
		public float targetMonsterLevel { get; private set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600146C RID: 5228 RVA: 0x0000F783 File Offset: 0x0000D983
		public float teamlessDamageCoefficient
		{
			get
			{
				return this.difficultyCoefficient;
			}
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00070610 File Offset: 0x0006E810
		protected void FixedUpdate()
		{
			this.NetworkfixedTime = this.fixedTime + Time.fixedDeltaTime;
			Run.FixedTimeStamp.Update();
			this.livingPlayerCount = PlayerCharacterMasterController.instances.Count((PlayerCharacterMasterController v) => v.master.alive);
			this.participatingPlayerCount = PlayerCharacterMasterController.instances.Count;
			this.OnFixedUpdate();
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0007067C File Offset: 0x0006E87C
		protected virtual void OnFixedUpdate()
		{
			DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(this.selectedDifficulty);
			float num = Mathf.Floor(this.fixedTime * 0.0166666675f);
			float num2 = (float)this.participatingPlayerCount * 0.3f;
			float num3 = 0.7f + num2;
			float num4 = 0.7f + num2;
			float num5 = Mathf.Pow((float)this.participatingPlayerCount, 0.2f);
			float num6 = 0.046f * difficultyDef.scalingValue * num5;
			float num7 = 0.046f * difficultyDef.scalingValue * num5;
			float num8 = Mathf.Pow(1.15f, (float)this.stageClearCount);
			this.compensatedDifficultyCoefficient = (num4 + num7 * num) * num8;
			this.difficultyCoefficient = (num3 + num6 * num) * num8;
			float num9 = (num3 + num6 * (this.fixedTime * 0.0166666675f)) * Mathf.Pow(1.15f, (float)this.stageClearCount);
			if (TeamManager.instance)
			{
				this.targetMonsterLevel = Mathf.Min((num9 - num3) / 0.33f + 1f, TeamManager.naturalLevelCap);
				if (NetworkServer.active)
				{
					uint num10 = (uint)Mathf.FloorToInt(this.targetMonsterLevel);
					uint teamLevel = TeamManager.instance.GetTeamLevel(TeamIndex.Monster);
					if (num10 > teamLevel)
					{
						TeamManager.instance.SetTeamLevel(TeamIndex.Monster, num10);
					}
				}
			}
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0000F78B File Offset: 0x0000D98B
		protected void Update()
		{
			this.time = Mathf.Clamp(this.time + Time.deltaTime, this.fixedTime, this.fixedTime + Time.fixedDeltaTime);
			Run.TimeStamp.Update();
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x0000F7BB File Offset: 0x0000D9BB
		[Server]
		public virtual bool CanUnlockableBeGrantedThisRun(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::CanUnlockableBeGrantedThisRun(System.String)' called on client");
				return false;
			}
			return !this.unlockablesAlreadyFullyObtained.Contains(unlockableName);
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x000707B8 File Offset: 0x0006E9B8
		[Server]
		public void GrantUnlockToAllParticipatingPlayers(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::GrantUnlockToAllParticipatingPlayers(System.String)' called on client");
				return;
			}
			if (this.unlockablesAlreadyFullyObtained.Contains(unlockableName))
			{
				return;
			}
			this.unlockablesAlreadyFullyObtained.Add(unlockableName);
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return;
			}
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				if (networkUser.isParticipating)
				{
					networkUser.ServerHandleUnlock(unlockableDef);
				}
			}
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x00070848 File Offset: 0x0006EA48
		[Server]
		public void GrantUnlockToSinglePlayer(string unlockableName, CharacterBody body)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::GrantUnlockToSinglePlayer(System.String,RoR2.CharacterBody)' called on client");
				return;
			}
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return;
			}
			if (body)
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(body);
				if (networkUser)
				{
					networkUser.ServerHandleUnlock(unlockableDef);
				}
			}
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x0000F7E2 File Offset: 0x0000D9E2
		[Server]
		public virtual bool IsUnlockableUnlocked(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::IsUnlockableUnlocked(System.String)' called on client");
				return false;
			}
			return this.unlockablesUnlockedByAnyUser.Contains(unlockableName);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0000F806 File Offset: 0x0000DA06
		[Server]
		public virtual bool DoesEveryoneHaveThisUnlockableUnlocked(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::DoesEveryoneHaveThisUnlockableUnlocked(System.String)' called on client");
				return false;
			}
			return this.unlockablesUnlockedByAllUsers.Contains(unlockableName);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x0000F82A File Offset: 0x0000DA2A
		[Server]
		public void ForceUnlockImmediate(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::ForceUnlockImmediate(System.String)' called on client");
				return;
			}
			this.unlockablesUnlockedByAnyUser.Add(unlockableName);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00070894 File Offset: 0x0006EA94
		private static void PopulateValidStages()
		{
			Run.validStages = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage
			select sceneDef.sceneField).ToArray<SceneField>();
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000708F8 File Offset: 0x0006EAF8
		public void PickNextStageScene(SceneField[] choices)
		{
			if (this.ruleBook.stageOrder == StageOrder.Normal)
			{
				this.nextStageScene = choices[this.nextStageRng.RangeInt(0, choices.Length)];
				return;
			}
			SceneField[] array = (from v in Run.validStages
			where v.SceneName != SceneManager.GetActiveScene().name
			select v).ToArray<SceneField>();
			this.nextStageScene = array[this.nextStageRng.RangeInt(0, array.Length)];
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x000025F6 File Offset: 0x000007F6
		protected virtual void OverrideSeed()
		{
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00070970 File Offset: 0x0006EB70
		protected virtual void BuildUnlockAvailability()
		{
			this.unlockablesUnlockedByAnyUser.Clear();
			this.unlockablesUnlockedByAllUsers.Clear();
			this.unlockablesAlreadyFullyObtained.Clear();
			int num = 0;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				if (networkUser.isParticipating)
				{
					num++;
					foreach (UnlockableDef unlockableDef in networkUser.unlockables)
					{
						string name = unlockableDef.name;
						this.unlockablesUnlockedByAnyUser.Add(name);
						if (!dictionary.ContainsKey(name))
						{
							dictionary.Add(name, 0);
						}
						Dictionary<string, int> dictionary2 = dictionary;
						string key = name;
						int value = dictionary2[key] + 1;
						dictionary2[key] = value;
					}
				}
			}
			if (num > 0)
			{
				foreach (KeyValuePair<string, int> keyValuePair in dictionary)
				{
					if (keyValuePair.Value == num)
					{
						this.unlockablesUnlockedByAllUsers.Add(keyValuePair.Key);
						this.unlockablesAlreadyFullyObtained.Add(keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00070ADC File Offset: 0x0006ECDC
		protected void Start()
		{
			if (NetworkServer.active)
			{
				this.OverrideSeed();
				this.runRNG = new Xoroshiro128Plus(this.seed);
				this.nextStageRng = new Xoroshiro128Plus(this.runRNG.nextUlong);
				this.stageRngGenerator = new Xoroshiro128Plus(this.runRNG.nextUlong);
				this.GenerateStageRNG();
				Run.PopulateValidStages();
			}
			this.allowNewParticipants = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				this.OnUserAdded(readOnlyInstancesList[i]);
			}
			this.allowNewParticipants = false;
			if (NetworkServer.active)
			{
				SceneField[] choices = this.startingScenes;
				string @string = Run.cvRunSceneOverride.GetString();
				if (@string != "")
				{
					choices = new SceneField[]
					{
						new SceneField(@string)
					};
				}
				this.PickNextStageScene(choices);
				NetworkManager.singleton.ServerChangeScene(this.nextStageScene);
			}
			this.BuildUnlockAvailability();
			this.BuildDropTable();
			Action<Run> action = Run.onRunStartGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x00070BEC File Offset: 0x0006EDEC
		protected void OnDestroy()
		{
			Action<Run> action = Run.onRunDestroyGlobal;
			if (action != null)
			{
				action(this);
			}
			ReadOnlyCollection<CharacterBody> readOnlyInstancesList = CharacterBody.readOnlyInstancesList;
			for (int i = readOnlyInstancesList.Count - 1; i >= 0; i--)
			{
				if (readOnlyInstancesList[i])
				{
					UnityEngine.Object.Destroy(readOnlyInstancesList[i].gameObject);
				}
			}
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList2 = CharacterMaster.readOnlyInstancesList;
			for (int j = readOnlyInstancesList2.Count - 1; j >= 0; j--)
			{
				if (readOnlyInstancesList2[j])
				{
					UnityEngine.Object.Destroy(readOnlyInstancesList2[j].gameObject);
				}
			}
			if (Stage.instance)
			{
				UnityEngine.Object.Destroy(Stage.instance.gameObject);
			}
			Chat.Clear();
			if (!this.shutdown)
			{
				this.HandlePostRunDestination();
			}
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0000F84E File Offset: 0x0000DA4E
		protected virtual void HandlePostRunDestination()
		{
			if (NetworkServer.active)
			{
				NetworkManager.singleton.ServerChangeScene("lobby");
			}
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0000F866 File Offset: 0x0000DA66
		protected void OnApplicationQuit()
		{
			this.shutdown = true;
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00070CA8 File Offset: 0x0006EEA8
		[Server]
		public CharacterMaster GetUserMaster(NetworkUserId networkUserId)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.Run::GetUserMaster(RoR2.NetworkUserId)' called on client");
				return null;
			}
			CharacterMaster result = null;
			this.userMasters.TryGetValue(networkUserId, out result);
			return result;
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0000F86F File Offset: 0x0000DA6F
		[Server]
		public void OnServerSceneChanged(string sceneName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::OnServerSceneChanged(System.String)' called on client");
				return;
			}
			this.BeginStage();
			this.isGameOverServer = false;
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0000F893 File Offset: 0x0000DA93
		[Server]
		private void BeginStage()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::BeginStage()' called on client");
				return;
			}
			NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Stage")));
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0000F8BE File Offset: 0x0000DABE
		[Server]
		private void EndStage()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::EndStage()' called on client");
				return;
			}
			if (Stage.instance)
			{
				UnityEngine.Object.Destroy(Stage.instance);
			}
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0000F8EB File Offset: 0x0000DAEB
		public void OnUserAdded(NetworkUser user)
		{
			if (NetworkServer.active)
			{
				this.SetupUserCharacterMaster(user);
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnUserRemoved(NetworkUser user)
		{
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00070CE8 File Offset: 0x0006EEE8
		[Server]
		private void SetupUserCharacterMaster(NetworkUser user)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetupUserCharacterMaster(RoR2.NetworkUser)' called on client");
				return;
			}
			if (user.masterObject)
			{
				return;
			}
			CharacterMaster characterMaster = this.GetUserMaster(user.id);
			bool flag = !characterMaster && this.allowNewParticipants;
			if (flag)
			{
				characterMaster = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMaster"), Vector3.zero, Quaternion.identity).GetComponent<CharacterMaster>();
				this.userMasters[user.id] = characterMaster;
				characterMaster.GiveMoney(this.ruleBook.startingMoney);
				if (this.selectedDifficulty == DifficultyIndex.Easy)
				{
					characterMaster.inventory.GiveItem(ItemIndex.DrizzlePlayerHelper, 1);
				}
				NetworkServer.Spawn(characterMaster.gameObject);
			}
			PlayerCharacterMasterController playerCharacterMasterController = null;
			if (characterMaster)
			{
				user.masterObject = characterMaster.gameObject;
				playerCharacterMasterController = characterMaster.GetComponent<PlayerCharacterMasterController>();
				if (playerCharacterMasterController)
				{
					playerCharacterMasterController.networkUserObject = user.gameObject;
				}
				characterMaster.GetComponent<NetworkIdentity>().AssignClientAuthority(user.connectionToClient);
			}
			if (flag && playerCharacterMasterController)
			{
				Action<Run, PlayerCharacterMasterController> action = Run.onPlayerFirstCreatedServer;
				if (action == null)
				{
					return;
				}
				action(this, playerCharacterMasterController);
			}
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06001485 RID: 5253 RVA: 0x00070DFC File Offset: 0x0006EFFC
		// (remove) Token: 0x06001486 RID: 5254 RVA: 0x00070E30 File Offset: 0x0006F030
		public static event Action<Run, PlayerCharacterMasterController> onPlayerFirstCreatedServer;

		// Token: 0x06001487 RID: 5255 RVA: 0x00070E64 File Offset: 0x0006F064
		[Server]
		public virtual void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::HandlePlayerFirstEntryAnimation(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"), body.transform.position, spawnRotation);
			gameObject.GetComponent<SurvivorPodController>().NetworkcharacterBodyObject = body.gameObject;
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnServerBossKilled(bool bossGroupDefeated)
		{
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnServerCharacterBodySpawned(CharacterBody characterBody)
		{
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0000F8FB File Offset: 0x0000DAFB
		public virtual GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x0000F907 File Offset: 0x0000DB07
		public int GetDifficultyScaledCost(int baseCost)
		{
			return (int)((float)baseCost * Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f));
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00070EB8 File Offset: 0x0006F0B8
		public void BuildDropTable()
		{
			this.availableTier1DropList.Clear();
			this.availableTier2DropList.Clear();
			this.availableTier3DropList.Clear();
			this.availableLunarDropList.Clear();
			this.availableEquipmentDropList.Clear();
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (this.availableItems.HasItem(itemIndex))
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					List<PickupIndex> list = null;
					switch (itemDef.tier)
					{
					case ItemTier.Tier1:
						list = this.availableTier1DropList;
						break;
					case ItemTier.Tier2:
						list = this.availableTier2DropList;
						break;
					case ItemTier.Tier3:
						list = this.availableTier3DropList;
						break;
					case ItemTier.Lunar:
						list = this.availableLunarDropList;
						break;
					}
					if (list != null)
					{
						list.Add(new PickupIndex(itemIndex));
					}
				}
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				if (this.availableEquipment.HasEquipment(equipmentIndex))
				{
					EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
					if (equipmentDef.canDrop)
					{
						if (!equipmentDef.isLunar)
						{
							this.availableEquipmentDropList.Add(new PickupIndex(equipmentIndex));
						}
						else
						{
							this.availableLunarDropList.Add(new PickupIndex(equipmentIndex));
						}
					}
				}
			}
			this.smallChestDropTierSelector.Clear();
			this.smallChestDropTierSelector.AddChoice(this.availableTier1DropList, 0.8f);
			this.smallChestDropTierSelector.AddChoice(this.availableTier2DropList, 0.2f);
			this.smallChestDropTierSelector.AddChoice(this.availableTier3DropList, 0.01f);
			this.mediumChestDropTierSelector.Clear();
			this.mediumChestDropTierSelector.AddChoice(this.availableTier2DropList, 0.8f);
			this.mediumChestDropTierSelector.AddChoice(this.availableTier3DropList, 0.2f);
			this.largeChestDropTierSelector.Clear();
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0000F921 File Offset: 0x0000DB21
		[ConCommand(commandName = "run_end", flags = ConVarFlags.SenderMustBeServer, helpText = "Ends the current run.")]
		private static void CCRunEnd(ConCommandArgs args)
		{
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00071058 File Offset: 0x0006F258
		[ConCommand(commandName = "run_set_time", flags = (ConVarFlags.SenderMustBeServer | ConVarFlags.Cheat), helpText = "Sets the time of the current run.")]
		private static void CCRunSetTime(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			args.CheckArgumentCount(1);
			float networkfixedTime;
			if (TextSerialization.TryParseInvariant(args[0], out networkfixedTime))
			{
				Run.instance.NetworkfixedTime = networkfixedTime;
			}
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x000710A0 File Offset: 0x0006F2A0
		[ConCommand(commandName = "run_print_unlockables", flags = ConVarFlags.SenderMustBeServer, helpText = "Prints all unlockables available in this run.")]
		private static void CCRunPrintUnlockables(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			List<string> list = new List<string>();
			foreach (string item in Run.instance.unlockablesUnlockedByAnyUser)
			{
				list.Add(item);
			}
			Debug.Log(string.Join("\n", list.ToArray()));
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x0000F93E File Offset: 0x0000DB3E
		[ConCommand(commandName = "run_print_seed", flags = ConVarFlags.None, helpText = "Prints the seed of the current run.")]
		private static void CCRunPrintSeed(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			Debug.LogFormat("Current run seed: {0}", new object[]
			{
				Run.instance.seed
			});
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x0007112C File Offset: 0x0006F32C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Stage.onServerStageComplete += delegate(Stage stage)
			{
				if (Run.instance && SceneInfo.instance && SceneInfo.instance.countsAsStage)
				{
					Run instance = Run.instance;
					instance.NetworkstageClearCount = instance.stageClearCount + 1;
				}
			};
			HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			HGXml.Register<Run.FixedTimeStamp>(new HGXml.Serializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.ToXml), new HGXml.Deserializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.FromXml));
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0000F979 File Offset: 0x0000DB79
		public virtual void AdvanceStage(string nextSceneName)
		{
			if (Stage.instance)
			{
				Stage.instance.CompleteServer();
			}
			this.GenerateStageRNG();
			NetworkManager.singleton.ServerChangeScene(nextSceneName);
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x0000F9A2 File Offset: 0x0000DBA2
		// (set) Token: 0x06001497 RID: 5271 RVA: 0x0000F9AA File Offset: 0x0000DBAA
		public bool isGameOverServer { get; private set; }

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06001498 RID: 5272 RVA: 0x00071198 File Offset: 0x0006F398
		// (remove) Token: 0x06001499 RID: 5273 RVA: 0x000711CC File Offset: 0x0006F3CC
		public static event Action<Run, GameResultType> OnServerGameOver;

		// Token: 0x0600149A RID: 5274 RVA: 0x00071200 File Offset: 0x0006F400
		public void BeginGameOver(GameResultType gameResultType)
		{
			if (this.isGameOverServer)
			{
				return;
			}
			if (Stage.instance && gameResultType != GameResultType.Lost)
			{
				Stage.instance.CompleteServer();
			}
			this.isGameOverServer = true;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GameOverController"));
			GameOverController component = gameObject.GetComponent<GameOverController>();
			component.SetRunReport(RunReport.Generate(this, gameResultType));
			Action<Run, GameResultType> onServerGameOver = Run.OnServerGameOver;
			if (onServerGameOver != null)
			{
				onServerGameOver(this, gameResultType);
			}
			NetworkServer.Spawn(gameObject);
			component.CallRpcClientGameOver();
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x0000F9B3 File Offset: 0x0000DBB3
		public virtual void OnClientGameOver(RunReport runReport)
		{
			RunReport.Save(runReport, "PreviousRun");
			Action<Run, RunReport> action = Run.onClientGameOverGlobal;
			if (action == null)
			{
				return;
			}
			action(this, runReport);
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x0600149C RID: 5276 RVA: 0x00071278 File Offset: 0x0006F478
		// (remove) Token: 0x0600149D RID: 5277 RVA: 0x000712AC File Offset: 0x0006F4AC
		public static event Action<Run, RunReport> onClientGameOverGlobal;

		// Token: 0x0600149E RID: 5278 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x000712E0 File Offset: 0x0006F4E0
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, RuleChoiceDef choiceDef)
		{
			foreach (RuleChoiceDef ruleChoiceDef in choiceDef.ruleDef.choices)
			{
				mustInclude[ruleChoiceDef.globalIndex] = false;
				mustExclude[ruleChoiceDef.globalIndex] = true;
			}
			mustInclude[choiceDef.globalIndex] = true;
			mustExclude[choiceDef.globalIndex] = false;
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x0000F9D2 File Offset: 0x0000DBD2
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, string choiceDefGlobalName)
		{
			this.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindChoiceDef(choiceDefGlobalName));
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060014A1 RID: 5281 RVA: 0x00071368 File Offset: 0x0006F568
		// (remove) Token: 0x060014A2 RID: 5282 RVA: 0x0007139C File Offset: 0x0006F59C
		public static event Action<Run> onRunStartGlobal;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x060014A3 RID: 5283 RVA: 0x000713D0 File Offset: 0x0006F5D0
		// (remove) Token: 0x060014A4 RID: 5284 RVA: 0x00071404 File Offset: 0x0006F604
		public static event Action<Run> onRunDestroyGlobal;

		// Token: 0x060014A7 RID: 5287 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x00071508 File Offset: 0x0006F708
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x0000FA18 File Offset: 0x0000DC18
		public ItemMask NetworkavailableItems
		{
			get
			{
				return this.availableItems;
			}
			set
			{
				base.SetSyncVar<ItemMask>(value, ref this.availableItems, 1u);
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x0007151C File Offset: 0x0006F71C
		// (set) Token: 0x060014AB RID: 5291 RVA: 0x0000FA2C File Offset: 0x0000DC2C
		public EquipmentMask NetworkavailableEquipment
		{
			get
			{
				return this.availableEquipment;
			}
			set
			{
				base.SetSyncVar<EquipmentMask>(value, ref this.availableEquipment, 2u);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x00071530 File Offset: 0x0006F730
		// (set) Token: 0x060014AD RID: 5293 RVA: 0x0000FA40 File Offset: 0x0000DC40
		public ArtifactMask NetworkenabledArtifacts
		{
			get
			{
				return this.enabledArtifacts;
			}
			set
			{
				base.SetSyncVar<ArtifactMask>(value, ref this.enabledArtifacts, 4u);
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x00071544 File Offset: 0x0006F744
		// (set) Token: 0x060014AF RID: 5295 RVA: 0x0000FA54 File Offset: 0x0000DC54
		public float NetworkfixedTime
		{
			get
			{
				return this.fixedTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.fixedTime, 8u);
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060014B0 RID: 5296 RVA: 0x00071558 File Offset: 0x0006F758
		// (set) Token: 0x060014B1 RID: 5297 RVA: 0x0000FA68 File Offset: 0x0000DC68
		public int NetworkstageClearCount
		{
			get
			{
				return this.stageClearCount;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.stageClearCount, 16u);
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x0007156C File Offset: 0x0006F76C
		// (set) Token: 0x060014B3 RID: 5299 RVA: 0x0000FA7C File Offset: 0x0000DC7C
		public int NetworkselectedDifficultyInternal
		{
			get
			{
				return this.selectedDifficultyInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.selectedDifficultyInternal, 32u);
			}
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x00071580 File Offset: 0x0006F780
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
				writer.Write(this.fixedTime);
				writer.WritePackedUInt32((uint)this.stageClearCount);
				writer.WritePackedUInt32((uint)this.selectedDifficultyInternal);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.fixedTime);
			}
			if ((base.syncVarDirtyBits & 16u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.stageClearCount);
			}
			if ((base.syncVarDirtyBits & 32u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.selectedDifficultyInternal);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00071728 File Offset: 0x0006F928
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
				this.fixedTime = reader.ReadSingle();
				this.stageClearCount = (int)reader.ReadPackedUInt32();
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
			}
			if ((num & 2) != 0)
			{
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
			}
			if ((num & 4) != 0)
			{
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
			}
			if ((num & 8) != 0)
			{
				this.fixedTime = reader.ReadSingle();
			}
			if ((num & 16) != 0)
			{
				this.stageClearCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 32) != 0)
			{
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x040017FE RID: 6142
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x040017FF RID: 6143
		public string nameToken = "";

		// Token: 0x04001800 RID: 6144
		[Tooltip("The pool of scenes to select the first scene of the run from.")]
		public SceneField[] startingScenes = Array.Empty<SceneField>();

		// Token: 0x04001801 RID: 6145
		[SyncVar]
		public ItemMask availableItems;

		// Token: 0x04001802 RID: 6146
		[SyncVar]
		public EquipmentMask availableEquipment;

		// Token: 0x04001803 RID: 6147
		[SyncVar]
		public ArtifactMask enabledArtifacts;

		// Token: 0x04001804 RID: 6148
		[SyncVar]
		public float fixedTime;

		// Token: 0x04001805 RID: 6149
		public float time;

		// Token: 0x04001806 RID: 6150
		[SyncVar]
		public int stageClearCount;

		// Token: 0x04001807 RID: 6151
		public SceneField nextStageScene;

		// Token: 0x04001808 RID: 6152
		public ulong seed;

		// Token: 0x04001809 RID: 6153
		public Xoroshiro128Plus runRNG;

		// Token: 0x0400180A RID: 6154
		public Xoroshiro128Plus nextStageRng;

		// Token: 0x0400180B RID: 6155
		public Xoroshiro128Plus stageRngGenerator;

		// Token: 0x0400180C RID: 6156
		public Xoroshiro128Plus stageRng;

		// Token: 0x0400180D RID: 6157
		public Xoroshiro128Plus bossRewardRng;

		// Token: 0x0400180E RID: 6158
		public Xoroshiro128Plus treasureRng;

		// Token: 0x0400180F RID: 6159
		public Xoroshiro128Plus spawnRng;

		// Token: 0x04001810 RID: 6160
		public float difficultyCoefficient = 1f;

		// Token: 0x04001811 RID: 6161
		public float compensatedDifficultyCoefficient = 1f;

		// Token: 0x04001812 RID: 6162
		[SyncVar]
		private int selectedDifficultyInternal = 1;

		// Token: 0x04001816 RID: 6166
		public int shopPortalCount;

		// Token: 0x04001817 RID: 6167
		private static readonly StringConVar cvRunSceneOverride = new StringConVar("run_scene_override", ConVarFlags.Cheat, "", "Overrides the first scene to enter in a run.");

		// Token: 0x04001818 RID: 6168
		private readonly HashSet<string> unlockablesUnlockedByAnyUser = new HashSet<string>();

		// Token: 0x04001819 RID: 6169
		private readonly HashSet<string> unlockablesUnlockedByAllUsers = new HashSet<string>();

		// Token: 0x0400181A RID: 6170
		private readonly HashSet<string> unlockablesAlreadyFullyObtained = new HashSet<string>();

		// Token: 0x0400181B RID: 6171
		private static SceneField[] validStages;

		// Token: 0x0400181C RID: 6172
		private bool shutdown;

		// Token: 0x0400181D RID: 6173
		private Dictionary<NetworkUserId, CharacterMaster> userMasters = new Dictionary<NetworkUserId, CharacterMaster>();

		// Token: 0x0400181E RID: 6174
		private bool allowNewParticipants;

		// Token: 0x04001820 RID: 6176
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x04001821 RID: 6177
		public readonly List<PickupIndex> availableTier1DropList = new List<PickupIndex>();

		// Token: 0x04001822 RID: 6178
		public readonly List<PickupIndex> availableTier2DropList = new List<PickupIndex>();

		// Token: 0x04001823 RID: 6179
		public readonly List<PickupIndex> availableTier3DropList = new List<PickupIndex>();

		// Token: 0x04001824 RID: 6180
		public readonly List<PickupIndex> availableLunarDropList = new List<PickupIndex>();

		// Token: 0x04001825 RID: 6181
		public readonly List<PickupIndex> availableEquipmentDropList = new List<PickupIndex>();

		// Token: 0x04001826 RID: 6182
		public WeightedSelection<List<PickupIndex>> smallChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x04001827 RID: 6183
		public WeightedSelection<List<PickupIndex>> mediumChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x04001828 RID: 6184
		public WeightedSelection<List<PickupIndex>> largeChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x020003BF RID: 959
		[Serializable]
		public struct TimeStamp : IEquatable<Run.TimeStamp>, IComparable<Run.TimeStamp>
		{
			// Token: 0x170001D3 RID: 467
			// (get) Token: 0x060014B6 RID: 5302 RVA: 0x0000FA90 File Offset: 0x0000DC90
			public float timeUntil
			{
				get
				{
					return this.t - Run.TimeStamp.tNow;
				}
			}

			// Token: 0x170001D4 RID: 468
			// (get) Token: 0x060014B7 RID: 5303 RVA: 0x0000FA9E File Offset: 0x0000DC9E
			public float timeSince
			{
				get
				{
					return Run.TimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0000FAAC File Offset: 0x0000DCAC
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001D6 RID: 470
			// (get) Token: 0x060014B9 RID: 5305 RVA: 0x0000FABE File Offset: 0x0000DCBE
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x060014BA RID: 5306 RVA: 0x0000FAD0 File Offset: 0x0000DCD0
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.TimeStamp.tNow;
				}
			}

			// Token: 0x060014BB RID: 5307 RVA: 0x00071824 File Offset: 0x0006FA24
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x060014BC RID: 5308 RVA: 0x0000FAE2 File Offset: 0x0000DCE2
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x060014BD RID: 5309 RVA: 0x0000FAEF File Offset: 0x0000DCEF
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001DA RID: 474
			// (get) Token: 0x060014BE RID: 5310 RVA: 0x0000FAFC File Offset: 0x0000DCFC
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x060014BF RID: 5311 RVA: 0x0000FB09 File Offset: 0x0000DD09
			public static void Update()
			{
				Run.TimeStamp.tNow = Run.instance.time;
			}

			// Token: 0x170001DB RID: 475
			// (get) Token: 0x060014C0 RID: 5312 RVA: 0x0000FB1A File Offset: 0x0000DD1A
			public static Run.TimeStamp now
			{
				get
				{
					return new Run.TimeStamp(Run.TimeStamp.tNow);
				}
			}

			// Token: 0x060014C1 RID: 5313 RVA: 0x0000FB26 File Offset: 0x0000DD26
			private TimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x060014C2 RID: 5314 RVA: 0x00071840 File Offset: 0x0006FA40
			public bool Equals(Run.TimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x060014C3 RID: 5315 RVA: 0x0000FB2F File Offset: 0x0000DD2F
			public override bool Equals(object obj)
			{
				return obj is Run.TimeStamp && this.Equals((Run.TimeStamp)obj);
			}

			// Token: 0x060014C4 RID: 5316 RVA: 0x00071864 File Offset: 0x0006FA64
			public int CompareTo(Run.TimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x060014C5 RID: 5317 RVA: 0x0000FB47 File Offset: 0x0000DD47
			public static Run.TimeStamp operator +(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t + b);
			}

			// Token: 0x060014C6 RID: 5318 RVA: 0x0000FB56 File Offset: 0x0000DD56
			public static Run.TimeStamp operator -(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t - b);
			}

			// Token: 0x060014C7 RID: 5319 RVA: 0x0000FB65 File Offset: 0x0000DD65
			public static float operator -(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x060014C8 RID: 5320 RVA: 0x0000FB74 File Offset: 0x0000DD74
			public static bool operator <(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x060014C9 RID: 5321 RVA: 0x0000FB84 File Offset: 0x0000DD84
			public static bool operator >(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x060014CA RID: 5322 RVA: 0x0000FB94 File Offset: 0x0000DD94
			public static bool operator <=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x060014CB RID: 5323 RVA: 0x0000FBA7 File Offset: 0x0000DDA7
			public static bool operator >=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x060014CC RID: 5324 RVA: 0x0000FBBA File Offset: 0x0000DDBA
			public static bool operator ==(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x060014CD RID: 5325 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
			public static bool operator !=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060014CE RID: 5326 RVA: 0x0000FBD1 File Offset: 0x0000DDD1
			public static Run.TimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.TimeStamp(reader.ReadSingle());
			}

			// Token: 0x060014CF RID: 5327 RVA: 0x0000FBDE File Offset: 0x0000DDDE
			public static void Serialize(NetworkWriter writer, Run.TimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x060014D0 RID: 5328 RVA: 0x0000FBED File Offset: 0x0000DDED
			public static void ToXml(XElement element, Run.TimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x060014D1 RID: 5329 RVA: 0x00071888 File Offset: 0x0006FA88
			public static bool FromXml(XElement element, ref Run.TimeStamp dest)
			{
				float num;
				if (TextSerialization.TryParseInvariant(element.Value, out num))
				{
					dest = new Run.TimeStamp(num);
					return true;
				}
				return false;
			}

			// Token: 0x060014D2 RID: 5330 RVA: 0x0000FC00 File Offset: 0x0000DE00
			[RuntimeInitializeOnLoadMethod]
			private static void Init()
			{
				HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			}

			// Token: 0x0400182E RID: 6190
			public readonly float t;

			// Token: 0x0400182F RID: 6191
			private static float tNow;

			// Token: 0x04001830 RID: 6192
			public static readonly Run.TimeStamp zero = new Run.TimeStamp(0f);

			// Token: 0x04001831 RID: 6193
			public static readonly Run.TimeStamp positiveInfinity = new Run.TimeStamp(float.PositiveInfinity);

			// Token: 0x04001832 RID: 6194
			public static readonly Run.TimeStamp negativeInfinity = new Run.TimeStamp(float.NegativeInfinity);
		}

		// Token: 0x020003C0 RID: 960
		[Serializable]
		public struct FixedTimeStamp : IEquatable<Run.FixedTimeStamp>, IComparable<Run.FixedTimeStamp>
		{
			// Token: 0x170001DC RID: 476
			// (get) Token: 0x060014D4 RID: 5332 RVA: 0x0000FC4E File Offset: 0x0000DE4E
			public float timeUntil
			{
				get
				{
					return this.t - Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x060014D5 RID: 5333 RVA: 0x0000FC5C File Offset: 0x0000DE5C
			public float timeSince
			{
				get
				{
					return Run.FixedTimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001DE RID: 478
			// (get) Token: 0x060014D6 RID: 5334 RVA: 0x0000FC6A File Offset: 0x0000DE6A
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x060014D7 RID: 5335 RVA: 0x0000FC7C File Offset: 0x0000DE7C
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x060014D8 RID: 5336 RVA: 0x0000FC8E File Offset: 0x0000DE8E
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x060014D9 RID: 5337 RVA: 0x000718B4 File Offset: 0x0006FAB4
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x060014DA RID: 5338 RVA: 0x0000FCA0 File Offset: 0x0000DEA0
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x060014DB RID: 5339 RVA: 0x0000FCAD File Offset: 0x0000DEAD
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x060014DC RID: 5340 RVA: 0x0000FCBA File Offset: 0x0000DEBA
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x060014DD RID: 5341 RVA: 0x0000FCC7 File Offset: 0x0000DEC7
			public static void Update()
			{
				Run.FixedTimeStamp.tNow = Run.instance.fixedTime;
			}

			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x060014DE RID: 5342 RVA: 0x0000FCD8 File Offset: 0x0000DED8
			public static Run.FixedTimeStamp now
			{
				get
				{
					return new Run.FixedTimeStamp(Run.FixedTimeStamp.tNow);
				}
			}

			// Token: 0x060014DF RID: 5343 RVA: 0x0000FCE4 File Offset: 0x0000DEE4
			private FixedTimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x060014E0 RID: 5344 RVA: 0x000718D0 File Offset: 0x0006FAD0
			public bool Equals(Run.FixedTimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x060014E1 RID: 5345 RVA: 0x0000FCED File Offset: 0x0000DEED
			public override bool Equals(object obj)
			{
				return obj is Run.FixedTimeStamp && this.Equals((Run.FixedTimeStamp)obj);
			}

			// Token: 0x060014E2 RID: 5346 RVA: 0x000718F4 File Offset: 0x0006FAF4
			public int CompareTo(Run.FixedTimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x060014E3 RID: 5347 RVA: 0x0000FD05 File Offset: 0x0000DF05
			public static Run.FixedTimeStamp operator +(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t + b);
			}

			// Token: 0x060014E4 RID: 5348 RVA: 0x0000FD14 File Offset: 0x0000DF14
			public static Run.FixedTimeStamp operator -(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t - b);
			}

			// Token: 0x060014E5 RID: 5349 RVA: 0x0000FD23 File Offset: 0x0000DF23
			public static float operator -(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x060014E6 RID: 5350 RVA: 0x0000FD32 File Offset: 0x0000DF32
			public static bool operator <(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x060014E7 RID: 5351 RVA: 0x0000FD42 File Offset: 0x0000DF42
			public static bool operator >(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x060014E8 RID: 5352 RVA: 0x0000FD52 File Offset: 0x0000DF52
			public static bool operator <=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x060014E9 RID: 5353 RVA: 0x0000FD65 File Offset: 0x0000DF65
			public static bool operator >=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x060014EA RID: 5354 RVA: 0x0000FD78 File Offset: 0x0000DF78
			public static bool operator ==(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x060014EB RID: 5355 RVA: 0x0000FD82 File Offset: 0x0000DF82
			public static bool operator !=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060014EC RID: 5356 RVA: 0x0000FD8F File Offset: 0x0000DF8F
			public static Run.FixedTimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.FixedTimeStamp(reader.ReadSingle());
			}

			// Token: 0x060014ED RID: 5357 RVA: 0x0000FD9C File Offset: 0x0000DF9C
			public static void Serialize(NetworkWriter writer, Run.FixedTimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x060014EE RID: 5358 RVA: 0x0000FDAB File Offset: 0x0000DFAB
			public static void ToXml(XElement element, Run.FixedTimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x060014EF RID: 5359 RVA: 0x00071918 File Offset: 0x0006FB18
			public static bool FromXml(XElement element, ref Run.FixedTimeStamp dest)
			{
				float num;
				if (TextSerialization.TryParseInvariant(element.Value, out num))
				{
					dest = new Run.FixedTimeStamp(num);
					return true;
				}
				return false;
			}

			// Token: 0x04001833 RID: 6195
			public readonly float t;

			// Token: 0x04001834 RID: 6196
			private static float tNow;

			// Token: 0x04001835 RID: 6197
			public static readonly Run.FixedTimeStamp zero = new Run.FixedTimeStamp(0f);

			// Token: 0x04001836 RID: 6198
			public static readonly Run.FixedTimeStamp positiveInfinity = new Run.FixedTimeStamp(float.PositiveInfinity);

			// Token: 0x04001837 RID: 6199
			public static readonly Run.FixedTimeStamp negativeInfinity = new Run.FixedTimeStamp(float.NegativeInfinity);
		}
	}
}
