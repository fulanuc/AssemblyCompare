using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using RoR2.ConVar;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003C3 RID: 963
	[DisallowMultipleComponent]
	[RequireComponent(typeof(NetworkRuleBook))]
	public class Run : NetworkBehaviour
	{
		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600147B RID: 5243 RVA: 0x0000F8B1 File Offset: 0x0000DAB1
		// (set) Token: 0x0600147C RID: 5244 RVA: 0x0000F8B8 File Offset: 0x0000DAB8
		public static Run instance { get; private set; }

		// Token: 0x0600147D RID: 5245 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
		private void OnEnable()
		{
			Run.instance = SingletonHelper.Assign<Run>(Run.instance, this);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0000F8D2 File Offset: 0x0000DAD2
		private void OnDisable()
		{
			Run.instance = SingletonHelper.Unassign<Run>(Run.instance, this);
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0000F8E4 File Offset: 0x0000DAE4
		protected void Awake()
		{
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06001480 RID: 5248 RVA: 0x0000F8F2 File Offset: 0x0000DAF2
		public RuleBook ruleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x00070784 File Offset: 0x0006E984
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

		// Token: 0x06001482 RID: 5250 RVA: 0x000707E4 File Offset: 0x0006E9E4
		[Server]
		private void SetRunStopwatchPaused(bool isPaused)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetRunStopwatchPaused(System.Boolean)' called on client");
				return;
			}
			if (isPaused != this.runStopwatch.isPaused)
			{
				Run.RunStopwatch networkrunStopwatch = this.runStopwatch;
				networkrunStopwatch.isPaused = isPaused;
				float num = this.GetRunStopwatch();
				if (isPaused)
				{
					networkrunStopwatch.offsetFromFixedTime = num;
				}
				else
				{
					networkrunStopwatch.offsetFromFixedTime = num - this.fixedTime;
				}
				this.NetworkrunStopwatch = networkrunStopwatch;
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x0000F8FF File Offset: 0x0000DAFF
		public float GetRunStopwatch()
		{
			if (this.runStopwatch.isPaused)
			{
				return this.runStopwatch.offsetFromFixedTime;
			}
			return this.fixedTime + this.runStopwatch.offsetFromFixedTime;
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06001484 RID: 5252 RVA: 0x0000F92C File Offset: 0x0000DB2C
		public bool isRunStopwatchPaused
		{
			get
			{
				return this.runStopwatch.isPaused;
			}
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00070850 File Offset: 0x0006EA50
		private void GenerateStageRNG()
		{
			this.stageRng = new Xoroshiro128Plus(this.stageRngGenerator.nextUlong);
			this.bossRewardRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.treasureRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.spawnRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001486 RID: 5254 RVA: 0x0000F939 File Offset: 0x0000DB39
		// (set) Token: 0x06001487 RID: 5255 RVA: 0x0000F941 File Offset: 0x0000DB41
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

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06001488 RID: 5256 RVA: 0x0000F94A File Offset: 0x0000DB4A
		// (set) Token: 0x06001489 RID: 5257 RVA: 0x0000F952 File Offset: 0x0000DB52
		public int livingPlayerCount { get; private set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600148A RID: 5258 RVA: 0x0000F95B File Offset: 0x0000DB5B
		// (set) Token: 0x0600148B RID: 5259 RVA: 0x0000F963 File Offset: 0x0000DB63
		public int participatingPlayerCount { get; private set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600148C RID: 5260 RVA: 0x0000F96C File Offset: 0x0000DB6C
		// (set) Token: 0x0600148D RID: 5261 RVA: 0x0000F974 File Offset: 0x0000DB74
		public float targetMonsterLevel { get; private set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600148E RID: 5262 RVA: 0x0000F97D File Offset: 0x0000DB7D
		public float teamlessDamageCoefficient
		{
			get
			{
				return this.difficultyCoefficient;
			}
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x000708B8 File Offset: 0x0006EAB8
		protected void FixedUpdate()
		{
			this.NetworkfixedTime = this.fixedTime + Time.fixedDeltaTime;
			Run.FixedTimeStamp.Update();
			if (NetworkServer.active)
			{
				this.SetRunStopwatchPaused(!this.ShouldUpdateRunStopwatch());
			}
			this.livingPlayerCount = PlayerCharacterMasterController.instances.Count((PlayerCharacterMasterController v) => v.master.alive);
			this.participatingPlayerCount = PlayerCharacterMasterController.instances.Count;
			this.OnFixedUpdate();
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00070938 File Offset: 0x0006EB38
		protected virtual void OnFixedUpdate()
		{
			float num = this.GetRunStopwatch();
			DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(this.selectedDifficulty);
			float num2 = Mathf.Floor(num * 0.0166666675f);
			float num3 = (float)this.participatingPlayerCount * 0.3f;
			float num4 = 0.7f + num3;
			float num5 = 0.7f + num3;
			float num6 = Mathf.Pow((float)this.participatingPlayerCount, 0.2f);
			float num7 = 0.046f * difficultyDef.scalingValue * num6;
			float num8 = 0.046f * difficultyDef.scalingValue * num6;
			float num9 = Mathf.Pow(1.15f, (float)this.stageClearCount);
			this.compensatedDifficultyCoefficient = (num5 + num8 * num2) * num9;
			this.difficultyCoefficient = (num4 + num7 * num2) * num9;
			float num10 = (num4 + num7 * (num * 0.0166666675f)) * Mathf.Pow(1.15f, (float)this.stageClearCount);
			if (TeamManager.instance)
			{
				this.targetMonsterLevel = Mathf.Min((num10 - num4) / 0.33f + 1f, TeamManager.naturalLevelCap);
				if (NetworkServer.active)
				{
					uint num11 = (uint)Mathf.FloorToInt(this.targetMonsterLevel);
					uint teamLevel = TeamManager.instance.GetTeamLevel(TeamIndex.Monster);
					if (num11 > teamLevel)
					{
						TeamManager.instance.SetTeamLevel(TeamIndex.Monster, num11);
					}
				}
			}
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x0000F985 File Offset: 0x0000DB85
		protected void Update()
		{
			this.time = Mathf.Clamp(this.time + Time.deltaTime, this.fixedTime, this.fixedTime + Time.fixedDeltaTime);
			Run.TimeStamp.Update();
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x0000F9B5 File Offset: 0x0000DBB5
		protected virtual bool ShouldUpdateRunStopwatch()
		{
			return SceneCatalog.mostRecentSceneDef.sceneType == SceneType.Stage && this.livingPlayerCount > 0;
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x0000F9CF File Offset: 0x0000DBCF
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

		// Token: 0x06001494 RID: 5268 RVA: 0x00070A74 File Offset: 0x0006EC74
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

		// Token: 0x06001495 RID: 5269 RVA: 0x00070B04 File Offset: 0x0006ED04
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

		// Token: 0x06001496 RID: 5270 RVA: 0x0000F9F6 File Offset: 0x0000DBF6
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

		// Token: 0x06001497 RID: 5271 RVA: 0x0000FA1A File Offset: 0x0000DC1A
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

		// Token: 0x06001498 RID: 5272 RVA: 0x0000FA3E File Offset: 0x0000DC3E
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

		// Token: 0x06001499 RID: 5273 RVA: 0x00070B50 File Offset: 0x0006ED50
		private static void PopulateValidStages()
		{
			Run.validStages = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage
			select sceneDef.sceneField).ToArray<SceneField>();
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00070BB4 File Offset: 0x0006EDB4
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

		// Token: 0x0600149B RID: 5275 RVA: 0x000025DA File Offset: 0x000007DA
		protected virtual void OverrideSeed()
		{
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00070C2C File Offset: 0x0006EE2C
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

		// Token: 0x0600149D RID: 5277 RVA: 0x00070D98 File Offset: 0x0006EF98
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

		// Token: 0x0600149E RID: 5278 RVA: 0x00070EA8 File Offset: 0x0006F0A8
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
			if (!this.shutdown && GameNetworkManager.singleton.isNetworkActive)
			{
				this.HandlePostRunDestination();
			}
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0000FA62 File Offset: 0x0000DC62
		protected virtual void HandlePostRunDestination()
		{
			if (NetworkServer.active)
			{
				NetworkManager.singleton.ServerChangeScene("lobby");
			}
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x0000FA7A File Offset: 0x0000DC7A
		protected void OnApplicationQuit()
		{
			this.shutdown = true;
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x00070F70 File Offset: 0x0006F170
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

		// Token: 0x060014A2 RID: 5282 RVA: 0x0000FA83 File Offset: 0x0000DC83
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

		// Token: 0x060014A3 RID: 5283 RVA: 0x0000FAA7 File Offset: 0x0000DCA7
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

		// Token: 0x060014A4 RID: 5284 RVA: 0x0000FAD2 File Offset: 0x0000DCD2
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

		// Token: 0x060014A5 RID: 5285 RVA: 0x0000FAFF File Offset: 0x0000DCFF
		public void OnUserAdded(NetworkUser user)
		{
			if (NetworkServer.active)
			{
				this.SetupUserCharacterMaster(user);
			}
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnUserRemoved(NetworkUser user)
		{
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x00070FB0 File Offset: 0x0006F1B0
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
		// (add) Token: 0x060014A8 RID: 5288 RVA: 0x000710C4 File Offset: 0x0006F2C4
		// (remove) Token: 0x060014A9 RID: 5289 RVA: 0x000710F8 File Offset: 0x0006F2F8
		public static event Action<Run, PlayerCharacterMasterController> onPlayerFirstCreatedServer;

		// Token: 0x060014AA RID: 5290 RVA: 0x0007112C File Offset: 0x0006F32C
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

		// Token: 0x060014AB RID: 5291 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnServerBossKilled(bool bossGroupDefeated)
		{
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnServerCharacterBodySpawned(CharacterBody characterBody)
		{
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0000FB0F File Offset: 0x0000DD0F
		public virtual GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x0000FB1B File Offset: 0x0000DD1B
		public int GetDifficultyScaledCost(int baseCost)
		{
			return (int)((float)baseCost * Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f));
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00071180 File Offset: 0x0006F380
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

		// Token: 0x060014B3 RID: 5299 RVA: 0x0000FB35 File Offset: 0x0000DD35
		[ConCommand(commandName = "run_end", flags = ConVarFlags.SenderMustBeServer, helpText = "Ends the current run.")]
		private static void CCRunEnd(ConCommandArgs args)
		{
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x00071320 File Offset: 0x0006F520
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

		// Token: 0x060014B5 RID: 5301 RVA: 0x0000FB52 File Offset: 0x0000DD52
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

		// Token: 0x060014B6 RID: 5302 RVA: 0x000713AC File Offset: 0x0006F5AC
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

		// Token: 0x060014B7 RID: 5303 RVA: 0x0000FB8D File Offset: 0x0000DD8D
		public virtual void AdvanceStage(string nextSceneName)
		{
			if (Stage.instance)
			{
				Stage.instance.CompleteServer();
			}
			this.GenerateStageRNG();
			NetworkManager.singleton.ServerChangeScene(nextSceneName);
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0000FBB6 File Offset: 0x0000DDB6
		// (set) Token: 0x060014B9 RID: 5305 RVA: 0x0000FBBE File Offset: 0x0000DDBE
		public bool isGameOverServer { get; private set; }

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x060014BA RID: 5306 RVA: 0x00071418 File Offset: 0x0006F618
		// (remove) Token: 0x060014BB RID: 5307 RVA: 0x0007144C File Offset: 0x0006F64C
		public static event Action<Run, GameResultType> OnServerGameOver;

		// Token: 0x060014BC RID: 5308 RVA: 0x00071480 File Offset: 0x0006F680
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
			if (gameResultType == GameResultType.Unknown)
			{
				for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
				{
					NetworkUser networkUser = NetworkUser.readOnlyInstancesList[i];
					if (networkUser && networkUser.isParticipating)
					{
						networkUser.AwardLunarCoins(5u);
					}
				}
			}
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

		// Token: 0x060014BD RID: 5309 RVA: 0x0000FBC7 File Offset: 0x0000DDC7
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
		// (add) Token: 0x060014BE RID: 5310 RVA: 0x00071534 File Offset: 0x0006F734
		// (remove) Token: 0x060014BF RID: 5311 RVA: 0x00071568 File Offset: 0x0006F768
		public static event Action<Run, RunReport> onClientGameOverGlobal;

		// Token: 0x060014C0 RID: 5312 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0007159C File Offset: 0x0006F79C
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

		// Token: 0x060014C2 RID: 5314 RVA: 0x0000FBE6 File Offset: 0x0000DDE6
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, string choiceDefGlobalName)
		{
			this.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindChoiceDef(choiceDefGlobalName));
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060014C3 RID: 5315 RVA: 0x00071624 File Offset: 0x0006F824
		// (remove) Token: 0x060014C4 RID: 5316 RVA: 0x00071658 File Offset: 0x0006F858
		public static event Action<Run> onRunStartGlobal;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x060014C5 RID: 5317 RVA: 0x0007168C File Offset: 0x0006F88C
		// (remove) Token: 0x060014C6 RID: 5318 RVA: 0x000716C0 File Offset: 0x0006F8C0
		public static event Action<Run> onRunDestroyGlobal;

		// Token: 0x060014C9 RID: 5321 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060014CA RID: 5322 RVA: 0x000717C4 File Offset: 0x0006F9C4
		// (set) Token: 0x060014CB RID: 5323 RVA: 0x0000FC2C File Offset: 0x0000DE2C
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

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x000717D8 File Offset: 0x0006F9D8
		// (set) Token: 0x060014CD RID: 5325 RVA: 0x0000FC40 File Offset: 0x0000DE40
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

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060014CE RID: 5326 RVA: 0x000717EC File Offset: 0x0006F9EC
		// (set) Token: 0x060014CF RID: 5327 RVA: 0x0000FC54 File Offset: 0x0000DE54
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

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00071800 File Offset: 0x0006FA00
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x0000FC68 File Offset: 0x0000DE68
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

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x00071814 File Offset: 0x0006FA14
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public Run.RunStopwatch NetworkrunStopwatch
		{
			get
			{
				return this.runStopwatch;
			}
			set
			{
				base.SetSyncVar<Run.RunStopwatch>(value, ref this.runStopwatch, 16u);
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x00071828 File Offset: 0x0006FA28
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x0000FC90 File Offset: 0x0000DE90
		public int NetworkstageClearCount
		{
			get
			{
				return this.stageClearCount;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.stageClearCount, 32u);
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060014D6 RID: 5334 RVA: 0x0007183C File Offset: 0x0006FA3C
		// (set) Token: 0x060014D7 RID: 5335 RVA: 0x0000FCA4 File Offset: 0x0000DEA4
		public int NetworkselectedDifficultyInternal
		{
			get
			{
				return this.selectedDifficultyInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.selectedDifficultyInternal, 64u);
			}
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00071850 File Offset: 0x0006FA50
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
				writer.Write(this.fixedTime);
				GeneratedNetworkCode._WriteRunStopwatch_Run(writer, this.runStopwatch);
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
				GeneratedNetworkCode._WriteRunStopwatch_Run(writer, this.runStopwatch);
			}
			if ((base.syncVarDirtyBits & 32u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.stageClearCount);
			}
			if ((base.syncVarDirtyBits & 64u) != 0u)
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

		// Token: 0x060014D9 RID: 5337 RVA: 0x00071A38 File Offset: 0x0006FC38
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
				this.fixedTime = reader.ReadSingle();
				this.runStopwatch = GeneratedNetworkCode._ReadRunStopwatch_Run(reader);
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
				this.runStopwatch = GeneratedNetworkCode._ReadRunStopwatch_Run(reader);
			}
			if ((num & 32) != 0)
			{
				this.stageClearCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 64) != 0)
			{
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x0400181A RID: 6170
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x0400181B RID: 6171
		public string nameToken = "";

		// Token: 0x0400181C RID: 6172
		[Tooltip("The pool of scenes to select the first scene of the run from.")]
		public SceneField[] startingScenes = Array.Empty<SceneField>();

		// Token: 0x0400181D RID: 6173
		[SyncVar]
		public ItemMask availableItems;

		// Token: 0x0400181E RID: 6174
		[SyncVar]
		public EquipmentMask availableEquipment;

		// Token: 0x0400181F RID: 6175
		[SyncVar]
		public ArtifactMask enabledArtifacts;

		// Token: 0x04001820 RID: 6176
		[SyncVar]
		public float fixedTime;

		// Token: 0x04001821 RID: 6177
		public float time;

		// Token: 0x04001822 RID: 6178
		[SyncVar]
		private Run.RunStopwatch runStopwatch;

		// Token: 0x04001823 RID: 6179
		[SyncVar]
		public int stageClearCount;

		// Token: 0x04001824 RID: 6180
		public SceneField nextStageScene;

		// Token: 0x04001825 RID: 6181
		public ulong seed;

		// Token: 0x04001826 RID: 6182
		public Xoroshiro128Plus runRNG;

		// Token: 0x04001827 RID: 6183
		public Xoroshiro128Plus nextStageRng;

		// Token: 0x04001828 RID: 6184
		public Xoroshiro128Plus stageRngGenerator;

		// Token: 0x04001829 RID: 6185
		public Xoroshiro128Plus stageRng;

		// Token: 0x0400182A RID: 6186
		public Xoroshiro128Plus bossRewardRng;

		// Token: 0x0400182B RID: 6187
		public Xoroshiro128Plus treasureRng;

		// Token: 0x0400182C RID: 6188
		public Xoroshiro128Plus spawnRng;

		// Token: 0x0400182D RID: 6189
		public float difficultyCoefficient = 1f;

		// Token: 0x0400182E RID: 6190
		public float compensatedDifficultyCoefficient = 1f;

		// Token: 0x0400182F RID: 6191
		[SyncVar]
		private int selectedDifficultyInternal = 1;

		// Token: 0x04001833 RID: 6195
		public int shopPortalCount;

		// Token: 0x04001834 RID: 6196
		private static readonly StringConVar cvRunSceneOverride = new StringConVar("run_scene_override", ConVarFlags.Cheat, "", "Overrides the first scene to enter in a run.");

		// Token: 0x04001835 RID: 6197
		private readonly HashSet<string> unlockablesUnlockedByAnyUser = new HashSet<string>();

		// Token: 0x04001836 RID: 6198
		private readonly HashSet<string> unlockablesUnlockedByAllUsers = new HashSet<string>();

		// Token: 0x04001837 RID: 6199
		private readonly HashSet<string> unlockablesAlreadyFullyObtained = new HashSet<string>();

		// Token: 0x04001838 RID: 6200
		private static SceneField[] validStages;

		// Token: 0x04001839 RID: 6201
		private bool shutdown;

		// Token: 0x0400183A RID: 6202
		private Dictionary<NetworkUserId, CharacterMaster> userMasters = new Dictionary<NetworkUserId, CharacterMaster>();

		// Token: 0x0400183B RID: 6203
		private bool allowNewParticipants;

		// Token: 0x0400183D RID: 6205
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x0400183E RID: 6206
		public readonly List<PickupIndex> availableTier1DropList = new List<PickupIndex>();

		// Token: 0x0400183F RID: 6207
		public readonly List<PickupIndex> availableTier2DropList = new List<PickupIndex>();

		// Token: 0x04001840 RID: 6208
		public readonly List<PickupIndex> availableTier3DropList = new List<PickupIndex>();

		// Token: 0x04001841 RID: 6209
		public readonly List<PickupIndex> availableLunarDropList = new List<PickupIndex>();

		// Token: 0x04001842 RID: 6210
		public readonly List<PickupIndex> availableEquipmentDropList = new List<PickupIndex>();

		// Token: 0x04001843 RID: 6211
		public WeightedSelection<List<PickupIndex>> smallChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x04001844 RID: 6212
		public WeightedSelection<List<PickupIndex>> mediumChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x04001845 RID: 6213
		public WeightedSelection<List<PickupIndex>> largeChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x020003C4 RID: 964
		[Serializable]
		public struct RunStopwatch : IEquatable<Run.RunStopwatch>
		{
			// Token: 0x060014DA RID: 5338 RVA: 0x0000FCB8 File Offset: 0x0000DEB8
			public bool Equals(Run.RunStopwatch other)
			{
				return this.offsetFromFixedTime.Equals(other.offsetFromFixedTime) && this.isPaused == other.isPaused;
			}

			// Token: 0x060014DB RID: 5339 RVA: 0x00071B58 File Offset: 0x0006FD58
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is Run.RunStopwatch)
				{
					Run.RunStopwatch other = (Run.RunStopwatch)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060014DC RID: 5340 RVA: 0x0000FCDD File Offset: 0x0000DEDD
			public override int GetHashCode()
			{
				return this.offsetFromFixedTime.GetHashCode() * 397 ^ this.isPaused.GetHashCode();
			}

			// Token: 0x0400184B RID: 6219
			public float offsetFromFixedTime;

			// Token: 0x0400184C RID: 6220
			public bool isPaused;
		}

		// Token: 0x020003C5 RID: 965
		[Serializable]
		public struct TimeStamp : IEquatable<Run.TimeStamp>, IComparable<Run.TimeStamp>
		{
			// Token: 0x170001DB RID: 475
			// (get) Token: 0x060014DD RID: 5341 RVA: 0x0000FCFC File Offset: 0x0000DEFC
			public float timeUntil
			{
				get
				{
					return this.t - Run.TimeStamp.tNow;
				}
			}

			// Token: 0x170001DC RID: 476
			// (get) Token: 0x060014DE RID: 5342 RVA: 0x0000FD0A File Offset: 0x0000DF0A
			public float timeSince
			{
				get
				{
					return Run.TimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x060014DF RID: 5343 RVA: 0x0000FD18 File Offset: 0x0000DF18
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001DE RID: 478
			// (get) Token: 0x060014E0 RID: 5344 RVA: 0x0000FD2A File Offset: 0x0000DF2A
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x060014E1 RID: 5345 RVA: 0x0000FD3C File Offset: 0x0000DF3C
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.TimeStamp.tNow;
				}
			}

			// Token: 0x060014E2 RID: 5346 RVA: 0x00071B84 File Offset: 0x0006FD84
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x060014E3 RID: 5347 RVA: 0x0000FD4E File Offset: 0x0000DF4E
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x060014E4 RID: 5348 RVA: 0x0000FD5B File Offset: 0x0000DF5B
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x060014E5 RID: 5349 RVA: 0x0000FD68 File Offset: 0x0000DF68
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x060014E6 RID: 5350 RVA: 0x0000FD75 File Offset: 0x0000DF75
			public static void Update()
			{
				Run.TimeStamp.tNow = Run.instance.time;
			}

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x060014E7 RID: 5351 RVA: 0x0000FD86 File Offset: 0x0000DF86
			public static Run.TimeStamp now
			{
				get
				{
					return new Run.TimeStamp(Run.TimeStamp.tNow);
				}
			}

			// Token: 0x060014E8 RID: 5352 RVA: 0x0000FD92 File Offset: 0x0000DF92
			private TimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x060014E9 RID: 5353 RVA: 0x00071BA0 File Offset: 0x0006FDA0
			public bool Equals(Run.TimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x060014EA RID: 5354 RVA: 0x0000FD9B File Offset: 0x0000DF9B
			public override bool Equals(object obj)
			{
				return obj is Run.TimeStamp && this.Equals((Run.TimeStamp)obj);
			}

			// Token: 0x060014EB RID: 5355 RVA: 0x00071BC4 File Offset: 0x0006FDC4
			public int CompareTo(Run.TimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x060014EC RID: 5356 RVA: 0x0000FDB3 File Offset: 0x0000DFB3
			public static Run.TimeStamp operator +(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t + b);
			}

			// Token: 0x060014ED RID: 5357 RVA: 0x0000FDC2 File Offset: 0x0000DFC2
			public static Run.TimeStamp operator -(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t - b);
			}

			// Token: 0x060014EE RID: 5358 RVA: 0x0000FDD1 File Offset: 0x0000DFD1
			public static float operator -(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x060014EF RID: 5359 RVA: 0x0000FDE0 File Offset: 0x0000DFE0
			public static bool operator <(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x060014F0 RID: 5360 RVA: 0x0000FDF0 File Offset: 0x0000DFF0
			public static bool operator >(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x060014F1 RID: 5361 RVA: 0x0000FE00 File Offset: 0x0000E000
			public static bool operator <=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x060014F2 RID: 5362 RVA: 0x0000FE13 File Offset: 0x0000E013
			public static bool operator >=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x060014F3 RID: 5363 RVA: 0x0000FE26 File Offset: 0x0000E026
			public static bool operator ==(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x060014F4 RID: 5364 RVA: 0x0000FE30 File Offset: 0x0000E030
			public static bool operator !=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060014F5 RID: 5365 RVA: 0x0000FE3D File Offset: 0x0000E03D
			public static Run.TimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.TimeStamp(reader.ReadSingle());
			}

			// Token: 0x060014F6 RID: 5366 RVA: 0x0000FE4A File Offset: 0x0000E04A
			public static void Serialize(NetworkWriter writer, Run.TimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x060014F7 RID: 5367 RVA: 0x0000FE59 File Offset: 0x0000E059
			public static void ToXml(XElement element, Run.TimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x060014F8 RID: 5368 RVA: 0x00071BE8 File Offset: 0x0006FDE8
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

			// Token: 0x060014F9 RID: 5369 RVA: 0x0000FE6C File Offset: 0x0000E06C
			[RuntimeInitializeOnLoadMethod]
			private static void Init()
			{
				HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			}

			// Token: 0x0400184D RID: 6221
			public readonly float t;

			// Token: 0x0400184E RID: 6222
			private static float tNow;

			// Token: 0x0400184F RID: 6223
			public static readonly Run.TimeStamp zero = new Run.TimeStamp(0f);

			// Token: 0x04001850 RID: 6224
			public static readonly Run.TimeStamp positiveInfinity = new Run.TimeStamp(float.PositiveInfinity);

			// Token: 0x04001851 RID: 6225
			public static readonly Run.TimeStamp negativeInfinity = new Run.TimeStamp(float.NegativeInfinity);
		}

		// Token: 0x020003C6 RID: 966
		[Serializable]
		public struct FixedTimeStamp : IEquatable<Run.FixedTimeStamp>, IComparable<Run.FixedTimeStamp>
		{
			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x060014FB RID: 5371 RVA: 0x0000FEBA File Offset: 0x0000E0BA
			public float timeUntil
			{
				get
				{
					return this.t - Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x060014FC RID: 5372 RVA: 0x0000FEC8 File Offset: 0x0000E0C8
			public float timeSince
			{
				get
				{
					return Run.FixedTimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001E6 RID: 486
			// (get) Token: 0x060014FD RID: 5373 RVA: 0x0000FED6 File Offset: 0x0000E0D6
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x060014FE RID: 5374 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001E8 RID: 488
			// (get) Token: 0x060014FF RID: 5375 RVA: 0x0000FEFA File Offset: 0x0000E0FA
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x06001500 RID: 5376 RVA: 0x00071C14 File Offset: 0x0006FE14
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x06001501 RID: 5377 RVA: 0x0000FF0C File Offset: 0x0000E10C
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001EA RID: 490
			// (get) Token: 0x06001502 RID: 5378 RVA: 0x0000FF19 File Offset: 0x0000E119
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x06001503 RID: 5379 RVA: 0x0000FF26 File Offset: 0x0000E126
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x06001504 RID: 5380 RVA: 0x0000FF33 File Offset: 0x0000E133
			public static void Update()
			{
				Run.FixedTimeStamp.tNow = Run.instance.fixedTime;
			}

			// Token: 0x170001EC RID: 492
			// (get) Token: 0x06001505 RID: 5381 RVA: 0x0000FF44 File Offset: 0x0000E144
			public static Run.FixedTimeStamp now
			{
				get
				{
					return new Run.FixedTimeStamp(Run.FixedTimeStamp.tNow);
				}
			}

			// Token: 0x06001506 RID: 5382 RVA: 0x0000FF50 File Offset: 0x0000E150
			private FixedTimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x06001507 RID: 5383 RVA: 0x00071C30 File Offset: 0x0006FE30
			public bool Equals(Run.FixedTimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x06001508 RID: 5384 RVA: 0x0000FF59 File Offset: 0x0000E159
			public override bool Equals(object obj)
			{
				return obj is Run.FixedTimeStamp && this.Equals((Run.FixedTimeStamp)obj);
			}

			// Token: 0x06001509 RID: 5385 RVA: 0x00071C54 File Offset: 0x0006FE54
			public int CompareTo(Run.FixedTimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x0600150A RID: 5386 RVA: 0x0000FF71 File Offset: 0x0000E171
			public static Run.FixedTimeStamp operator +(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t + b);
			}

			// Token: 0x0600150B RID: 5387 RVA: 0x0000FF80 File Offset: 0x0000E180
			public static Run.FixedTimeStamp operator -(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t - b);
			}

			// Token: 0x0600150C RID: 5388 RVA: 0x0000FF8F File Offset: 0x0000E18F
			public static float operator -(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x0600150D RID: 5389 RVA: 0x0000FF9E File Offset: 0x0000E19E
			public static bool operator <(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x0600150E RID: 5390 RVA: 0x0000FFAE File Offset: 0x0000E1AE
			public static bool operator >(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x0600150F RID: 5391 RVA: 0x0000FFBE File Offset: 0x0000E1BE
			public static bool operator <=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x06001510 RID: 5392 RVA: 0x0000FFD1 File Offset: 0x0000E1D1
			public static bool operator >=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x06001511 RID: 5393 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
			public static bool operator ==(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x06001512 RID: 5394 RVA: 0x0000FFEE File Offset: 0x0000E1EE
			public static bool operator !=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x06001513 RID: 5395 RVA: 0x0000FFFB File Offset: 0x0000E1FB
			public static Run.FixedTimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.FixedTimeStamp(reader.ReadSingle());
			}

			// Token: 0x06001514 RID: 5396 RVA: 0x00010008 File Offset: 0x0000E208
			public static void Serialize(NetworkWriter writer, Run.FixedTimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x06001515 RID: 5397 RVA: 0x00010017 File Offset: 0x0000E217
			public static void ToXml(XElement element, Run.FixedTimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x06001516 RID: 5398 RVA: 0x00071C78 File Offset: 0x0006FE78
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

			// Token: 0x04001852 RID: 6226
			public readonly float t;

			// Token: 0x04001853 RID: 6227
			private static float tNow;

			// Token: 0x04001854 RID: 6228
			public static readonly Run.FixedTimeStamp zero = new Run.FixedTimeStamp(0f);

			// Token: 0x04001855 RID: 6229
			public static readonly Run.FixedTimeStamp positiveInfinity = new Run.FixedTimeStamp(float.PositiveInfinity);

			// Token: 0x04001856 RID: 6230
			public static readonly Run.FixedTimeStamp negativeInfinity = new Run.FixedTimeStamp(float.NegativeInfinity);
		}
	}
}
