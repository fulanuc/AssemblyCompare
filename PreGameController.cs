using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000394 RID: 916
	[RequireComponent(typeof(NetworkRuleChoiceMask))]
	[RequireComponent(typeof(NetworkRuleBook))]
	public class PreGameController : NetworkBehaviour
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x0000EC35 File Offset: 0x0000CE35
		// (set) Token: 0x06001346 RID: 4934 RVA: 0x0000EC3C File Offset: 0x0000CE3C
		public static PreGameController instance { get; private set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x0000EC44 File Offset: 0x0000CE44
		public RuleChoiceMask resolvedRuleChoiceMask
		{
			get
			{
				return this.networkRuleChoiceMaskComponent.ruleChoiceMask;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001348 RID: 4936 RVA: 0x0000EC51 File Offset: 0x0000CE51
		public RuleBook readOnlyRuleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x0006C008 File Offset: 0x0006A208
		private void Awake()
		{
			this.networkRuleChoiceMaskComponent = base.GetComponent<NetworkRuleChoiceMask>();
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
			if (NetworkServer.active)
			{
				this.NetworkgameModeIndex = GameModeCatalog.FindGameModeIndex(PreGameController.GameModeConVar.instance.GetString());
				this.runSeed = RoR2Application.rng.nextUlong;
			}
			bool isInSinglePlayer = RoR2Application.isInSinglePlayer;
			for (int i = 0; i < this.serverAvailableChoiceMask.length; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				this.serverAvailableChoiceMask[i] = (isInSinglePlayer ? choiceDef.availableInSinglePlayer : choiceDef.availableInMultiPlayer);
			}
			if (PreGameController.persistentRuleBook != null && NetworkServer.active)
			{
				this.networkRuleBookComponent.SetRuleBook(PreGameController.persistentRuleBook);
			}
			NetworkUser.OnPostNetworkUserStart += this.GenerateRuleVoteController;
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0000EC5E File Offset: 0x0000CE5E
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.GenerateRuleVoteController;
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x0006C0C8 File Offset: 0x0006A2C8
		private void GenerateRuleVoteController(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				if (PreGameRuleVoteController.FindForUser(networkUser))
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PreGameRuleVoteController"), base.transform);
				gameObject.GetComponent<PreGameRuleVoteController>().networkUserNetworkIdentity = networkUser.GetComponent<NetworkIdentity>();
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x0006C118 File Offset: 0x0006A318
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.ResolveChoiceMask();
				foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
				{
					Debug.LogFormat("Attempting to generate PreGameVoteController for {0}", new object[]
					{
						networkUser.userName
					});
					this.GenerateRuleVoteController(networkUser);
				}
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x0000EC71 File Offset: 0x0000CE71
		[Server]
		public void UpdatePersistentRulebook()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::UpdatePersistentRulebook()' called on client");
				return;
			}
			if (PreGameController.persistentRuleBook == null)
			{
				PreGameController.persistentRuleBook = new RuleBook();
			}
			PreGameController.persistentRuleBook.Copy(this.readOnlyRuleBook);
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0006C18C File Offset: 0x0006A38C
		[Server]
		public void ApplyChoice(int ruleChoiceIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::ApplyChoice(System.Int32)' called on client");
				return;
			}
			if (!this.resolvedRuleChoiceMask[ruleChoiceIndex])
			{
				return;
			}
			RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(ruleChoiceIndex);
			if (this.readOnlyRuleBook.GetRuleChoice(choiceDef.ruleDef.globalIndex) == choiceDef)
			{
				return;
			}
			this.ruleBookBuffer.Copy(this.readOnlyRuleBook);
			this.ruleBookBuffer.ApplyChoice(choiceDef);
			this.networkRuleBookComponent.SetRuleBook(this.ruleBookBuffer);
			this.UpdatePersistentRulebook();
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x0006C214 File Offset: 0x0006A414
		[Server]
		public void EnforceValidRuleChoices()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::EnforceValidRuleChoices()' called on client");
				return;
			}
			this.ruleBookBuffer.Copy(this.readOnlyRuleBook);
			for (int i = 0; i < RuleCatalog.ruleCount; i++)
			{
				if (!this.resolvedRuleChoiceMask[this.ruleBookBuffer.GetRuleChoice(i)])
				{
					RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
					RuleChoiceDef choiceDef = ruleDef.choices[ruleDef.defaultChoiceIndex];
					int num = 0;
					int j = 0;
					int count = ruleDef.choices.Count;
					while (j < count)
					{
						if (this.resolvedRuleChoiceMask[ruleDef.choices[j]])
						{
							num++;
						}
						j++;
					}
					if (this.resolvedRuleChoiceMask[choiceDef] || num == 0)
					{
						this.ruleBookBuffer.ApplyChoice(choiceDef);
					}
					else
					{
						int k = 0;
						int count2 = ruleDef.choices.Count;
						while (k < count2)
						{
							if (this.resolvedRuleChoiceMask[ruleDef.choices[k]])
							{
								this.ruleBookBuffer.ApplyChoice(ruleDef.choices[k]);
								break;
							}
							k++;
						}
					}
				}
			}
			this.networkRuleBookComponent.SetRuleBook(this.ruleBookBuffer);
			this.UpdatePersistentRulebook();
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0006C358 File Offset: 0x0006A558
		private void TestRuleValues()
		{
			RuleBook ruleBook = new RuleBook();
			ruleBook.Copy(this.networkRuleBookComponent.ruleBook);
			RuleDef ruleDef = RuleCatalog.GetRuleDef(UnityEngine.Random.Range(0, RuleCatalog.ruleCount));
			RuleChoiceDef choiceDef = ruleDef.choices[UnityEngine.Random.Range(0, ruleDef.choices.Count)];
			ruleBook.ApplyChoice(choiceDef);
			this.networkRuleBookComponent.SetRuleBook(ruleBook);
			base.Invoke("TestRuleValues", 0.5f);
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0006C3D0 File Offset: 0x0006A5D0
		private void OnEnable()
		{
			PreGameController.instance = SingletonHelper.Assign<PreGameController>(PreGameController.instance, this);
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
			NetworkUser.OnNetworkUserUnlockablesUpdated += this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStartCallback;
			if (NetworkClient.active)
			{
				foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
				{
					networkUser.SendServerUnlockables();
				}
			}
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x0000ECA9 File Offset: 0x0000CEA9
		private void OnDisable()
		{
			PreGameController.instance = SingletonHelper.Unassign<PreGameController>(PreGameController.instance, this);
			NetworkUser.OnNetworkUserUnlockablesUpdated -= this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStartCallback;
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001353 RID: 4947 RVA: 0x0000ECDD File Offset: 0x0000CEDD
		// (set) Token: 0x06001354 RID: 4948 RVA: 0x0000ECE5 File Offset: 0x0000CEE5
		private PreGameController.PregameState pregameState
		{
			get
			{
				return (PreGameController.PregameState)this.pregameStateInternal;
			}
			set
			{
				this.NetworkpregameStateInternal = (int)value;
			}
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0000ECEE File Offset: 0x0000CEEE
		public bool IsCharacterSwitchingCurrentlyAllowed()
		{
			return this.pregameState == PreGameController.PregameState.Idle;
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x0000ECF9 File Offset: 0x0000CEF9
		private void Update()
		{
			if (this.pregameState == PreGameController.PregameState.Launching)
			{
				if (GameNetworkManager.singleton.unpredictedServerFixedTime - this.launchStartTime >= 0.5f && NetworkServer.active)
				{
					this.StartRun();
					return;
				}
			}
			else
			{
				PreGameController.PregameState pregameState = this.pregameState;
			}
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x0000ED33 File Offset: 0x0000CF33
		[Server]
		public void StartLaunch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::StartLaunch()' called on client");
				return;
			}
			if (this.pregameState == PreGameController.PregameState.Idle)
			{
				this.pregameState = PreGameController.PregameState.Launching;
				this.NetworklaunchStartTime = GameNetworkManager.singleton.unpredictedServerFixedTime;
			}
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x0006C460 File Offset: 0x0006A660
		[Server]
		private void StartRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::StartRun()' called on client");
				return;
			}
			this.pregameState = PreGameController.PregameState.Launched;
			Run run = NetworkSession.instance.BeginRun(PreGameController.GameModeConVar.instance.runPrefabComponent);
			run.SetRuleBook(this.readOnlyRuleBook);
			run.seed = this.runSeed;
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x0000ED69 File Offset: 0x0000CF69
		[ConCommand(commandName = "pregame_start_run", flags = ConVarFlags.SenderMustBeServer, helpText = "Begins a run out of pregame.")]
		private static void CCPregameStartRun(ConCommandArgs args)
		{
			if (PreGameController.instance)
			{
				PreGameController.instance.StartRun();
			}
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x0006C4B4 File Offset: 0x0006A6B4
		private static bool AnyUserHasUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				if (readOnlyInstancesList[i].unlockables.Contains(unlockableDef))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x0006C4F0 File Offset: 0x0006A6F0
		[Server]
		private void RecalculateModifierAvailability()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::RecalculateModifierAvailability()' called on client");
				return;
			}
			for (int i = 0; i < RuleCatalog.choiceCount; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				bool flag = string.IsNullOrEmpty(choiceDef.unlockableName);
				if (!flag)
				{
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(choiceDef.unlockableName);
					if (unlockableDef != null)
					{
						flag = PreGameController.AnyUserHasUnlockable(unlockableDef);
					}
				}
				this.unlockedChoiceMask[i] = flag;
			}
			this.ResolveChoiceMask();
			Action<PreGameController> action = PreGameController.onServerRecalculatedModifierAvailability;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x0006C574 File Offset: 0x0006A774
		[Server]
		private void ResolveChoiceMask()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::ResolveChoiceMask()' called on client");
				return;
			}
			RuleChoiceMask ruleChoiceMask = new RuleChoiceMask();
			RuleChoiceMask ruleChoiceMask2 = new RuleChoiceMask();
			Run gameModePrefabComponent = GameModeCatalog.GetGameModePrefabComponent(this.gameModeIndex);
			if (gameModePrefabComponent)
			{
				gameModePrefabComponent.OverrideRuleChoices(ruleChoiceMask, ruleChoiceMask2);
			}
			for (int i = 0; i < this.choiceMaskBuffer.length; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				this.choiceMaskBuffer[i] = (ruleChoiceMask[i] || (!ruleChoiceMask2[i] && this.serverAvailableChoiceMask[i] && this.unlockedChoiceMask[i] && !choiceDef.excludeByDefault));
			}
			this.networkRuleChoiceMaskComponent.SetRuleChoiceMask(this.choiceMaskBuffer);
			this.EnforceValidRuleChoices();
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x0000ED81 File Offset: 0x0000CF81
		private void OnNetworkUserUnlockablesUpdatedCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x0600135E RID: 4958 RVA: 0x0006C640 File Offset: 0x0006A840
		// (remove) Token: 0x0600135F RID: 4959 RVA: 0x0006C674 File Offset: 0x0006A874
		public static event Action<PreGameController> onServerRecalculatedModifierAvailability;

		// Token: 0x06001360 RID: 4960 RVA: 0x0000ED90 File Offset: 0x0000CF90
		private void OnPostNetworkUserStartCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				networkUser.ServerRequestUnlockables();
			}
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0006C6A8 File Offset: 0x0006A8A8
		// (set) Token: 0x06001365 RID: 4965 RVA: 0x0000EDDE File Offset: 0x0000CFDE
		public int NetworkgameModeIndex
		{
			get
			{
				return this.gameModeIndex;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.gameModeIndex, 1u);
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x0006C6BC File Offset: 0x0006A8BC
		// (set) Token: 0x06001367 RID: 4967 RVA: 0x0000EDF2 File Offset: 0x0000CFF2
		public int NetworkpregameStateInternal
		{
			get
			{
				return this.pregameStateInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.pregameStateInternal, 2u);
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x0006C6D0 File Offset: 0x0006A8D0
		// (set) Token: 0x06001369 RID: 4969 RVA: 0x0000EE06 File Offset: 0x0000D006
		public float NetworklaunchStartTime
		{
			get
			{
				return this.launchStartTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.launchStartTime, 4u);
			}
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0006C6E4 File Offset: 0x0006A8E4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.gameModeIndex);
				writer.WritePackedUInt32((uint)this.pregameStateInternal);
				writer.Write(this.launchStartTime);
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
				writer.WritePackedUInt32((uint)this.gameModeIndex);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.pregameStateInternal);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.launchStartTime);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0006C7D0 File Offset: 0x0006A9D0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.gameModeIndex = (int)reader.ReadPackedUInt32();
				this.pregameStateInternal = (int)reader.ReadPackedUInt32();
				this.launchStartTime = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.gameModeIndex = (int)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.pregameStateInternal = (int)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.launchStartTime = reader.ReadSingle();
			}
		}

		// Token: 0x040016F6 RID: 5878
		private NetworkRuleChoiceMask networkRuleChoiceMaskComponent;

		// Token: 0x040016F7 RID: 5879
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x040016F8 RID: 5880
		private readonly RuleChoiceMask serverAvailableChoiceMask = new RuleChoiceMask();

		// Token: 0x040016F9 RID: 5881
		public ulong runSeed;

		// Token: 0x040016FA RID: 5882
		[SyncVar]
		public int gameModeIndex;

		// Token: 0x040016FB RID: 5883
		private readonly RuleBook ruleBookBuffer = new RuleBook();

		// Token: 0x040016FC RID: 5884
		private static RuleBook persistentRuleBook;

		// Token: 0x040016FD RID: 5885
		[SyncVar]
		private int pregameStateInternal;

		// Token: 0x040016FE RID: 5886
		private const float launchTransitionDuration = 0f;

		// Token: 0x040016FF RID: 5887
		private GameObject gameModePrefab;

		// Token: 0x04001700 RID: 5888
		[SyncVar]
		private float launchStartTime = float.PositiveInfinity;

		// Token: 0x04001701 RID: 5889
		private readonly RuleChoiceMask unlockedChoiceMask = new RuleChoiceMask();

		// Token: 0x04001702 RID: 5890
		private readonly RuleChoiceMask choiceMaskBuffer = new RuleChoiceMask();

		// Token: 0x02000395 RID: 917
		private enum PregameState
		{
			// Token: 0x04001705 RID: 5893
			Idle,
			// Token: 0x04001706 RID: 5894
			Launching,
			// Token: 0x04001707 RID: 5895
			Launched
		}

		// Token: 0x02000396 RID: 918
		private class GameModeConVar : BaseConVar
		{
			// Token: 0x0600136C RID: 4972 RVA: 0x000090CD File Offset: 0x000072CD
			public GameModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600136D RID: 4973 RVA: 0x0000EE1A File Offset: 0x0000D01A
			static GameModeConVar()
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					PreGameController.GameModeConVar.instance.runPrefabComponent = GameModeCatalog.FindGameModePrefabComponent(PreGameController.GameModeConVar.instance.GetString());
				});
			}

			// Token: 0x0600136E RID: 4974 RVA: 0x0006C85C File Offset: 0x0006AA5C
			public override void SetString(string newValue)
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					Run exists = GameModeCatalog.FindGameModePrefabComponent(newValue);
					if (!exists)
					{
						Debug.LogFormat("GameMode \"{0}\" does not exist.", new object[]
						{
							newValue
						});
						return;
					}
					this.runPrefabComponent = exists;
				});
			}

			// Token: 0x0600136F RID: 4975 RVA: 0x0000EE50 File Offset: 0x0000D050
			public override string GetString()
			{
				if (!this.runPrefabComponent)
				{
					return "ClassicRun";
				}
				return this.runPrefabComponent.gameObject.name;
			}

			// Token: 0x04001708 RID: 5896
			public static readonly PreGameController.GameModeConVar instance = new PreGameController.GameModeConVar("gamemode", ConVarFlags.None, "", "Sets the specified game mode as the one to use in the next run.");

			// Token: 0x04001709 RID: 5897
			public Run runPrefabComponent;
		}
	}
}
