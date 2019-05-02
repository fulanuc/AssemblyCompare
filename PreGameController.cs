using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038F RID: 911
	[RequireComponent(typeof(NetworkRuleChoiceMask))]
	[RequireComponent(typeof(NetworkRuleBook))]
	public class PreGameController : NetworkBehaviour
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x0000EA7F File Offset: 0x0000CC7F
		// (set) Token: 0x06001328 RID: 4904 RVA: 0x0000EA86 File Offset: 0x0000CC86
		public static PreGameController instance { get; private set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001329 RID: 4905 RVA: 0x0000EA8E File Offset: 0x0000CC8E
		public RuleChoiceMask resolvedRuleChoiceMask
		{
			get
			{
				return this.networkRuleChoiceMaskComponent.ruleChoiceMask;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600132A RID: 4906 RVA: 0x0000EA9B File Offset: 0x0000CC9B
		public RuleBook readOnlyRuleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0006BD9C File Offset: 0x00069F9C
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

		// Token: 0x0600132C RID: 4908 RVA: 0x0000EAA8 File Offset: 0x0000CCA8
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.GenerateRuleVoteController;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0006BE5C File Offset: 0x0006A05C
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

		// Token: 0x0600132E RID: 4910 RVA: 0x0006BEAC File Offset: 0x0006A0AC
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

		// Token: 0x0600132F RID: 4911 RVA: 0x0000EABB File Offset: 0x0000CCBB
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

		// Token: 0x06001330 RID: 4912 RVA: 0x0006BF20 File Offset: 0x0006A120
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

		// Token: 0x06001331 RID: 4913 RVA: 0x0006BFA8 File Offset: 0x0006A1A8
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

		// Token: 0x06001332 RID: 4914 RVA: 0x0006C0EC File Offset: 0x0006A2EC
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

		// Token: 0x06001333 RID: 4915 RVA: 0x0006C164 File Offset: 0x0006A364
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

		// Token: 0x06001334 RID: 4916 RVA: 0x0000EAF3 File Offset: 0x0000CCF3
		private void OnDisable()
		{
			PreGameController.instance = SingletonHelper.Unassign<PreGameController>(PreGameController.instance, this);
			NetworkUser.OnNetworkUserUnlockablesUpdated -= this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStartCallback;
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x0000EB27 File Offset: 0x0000CD27
		// (set) Token: 0x06001336 RID: 4918 RVA: 0x0000EB2F File Offset: 0x0000CD2F
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

		// Token: 0x06001337 RID: 4919 RVA: 0x0000EB38 File Offset: 0x0000CD38
		public bool IsCharacterSwitchingCurrentlyAllowed()
		{
			return this.pregameState == PreGameController.PregameState.Idle;
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0000EB43 File Offset: 0x0000CD43
		private void Update()
		{
			if (this.pregameState == PreGameController.PregameState.Launching)
			{
				if (GameNetworkManager.singleton.unpredictedServerFixedTime - this.launchStartTime >= 0.5f)
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

		// Token: 0x06001339 RID: 4921 RVA: 0x0000EB76 File Offset: 0x0000CD76
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

		// Token: 0x0600133A RID: 4922 RVA: 0x0006C1F4 File Offset: 0x0006A3F4
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

		// Token: 0x0600133B RID: 4923 RVA: 0x0000EBAC File Offset: 0x0000CDAC
		[ConCommand(commandName = "pregame_start_run", flags = ConVarFlags.SenderMustBeServer, helpText = "Begins a run out of pregame.")]
		private static void CCPregameStartRun(ConCommandArgs args)
		{
			if (PreGameController.instance)
			{
				PreGameController.instance.StartRun();
			}
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x0006C248 File Offset: 0x0006A448
		[ConCommand(commandName = "pregame_set_seed", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Sets the random seed for the run.")]
		private static void CCPregameSetSeed(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (!PreGameController.instance)
			{
				throw new ConCommandException("Pregame controller does not currently exist to set the seed for.");
			}
			ulong num;
			if (!TextSerialization.TryParseInvariant(args[0], out num))
			{
				throw new ConCommandException("Specified seed is not a parsable uint64.");
			}
			PreGameController.instance.runSeed = num;
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x0006C29C File Offset: 0x0006A49C
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

		// Token: 0x0600133E RID: 4926 RVA: 0x0006C2D8 File Offset: 0x0006A4D8
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

		// Token: 0x0600133F RID: 4927 RVA: 0x0006C35C File Offset: 0x0006A55C
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

		// Token: 0x06001340 RID: 4928 RVA: 0x0000EBC4 File Offset: 0x0000CDC4
		private void OnNetworkUserUnlockablesUpdatedCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001341 RID: 4929 RVA: 0x0006C428 File Offset: 0x0006A628
		// (remove) Token: 0x06001342 RID: 4930 RVA: 0x0006C45C File Offset: 0x0006A65C
		public static event Action<PreGameController> onServerRecalculatedModifierAvailability;

		// Token: 0x06001343 RID: 4931 RVA: 0x0000EBD3 File Offset: 0x0000CDD3
		private void OnPostNetworkUserStartCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				networkUser.ServerRequestUnlockables();
			}
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x0006C490 File Offset: 0x0006A690
		// (set) Token: 0x06001348 RID: 4936 RVA: 0x0000EC21 File Offset: 0x0000CE21
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

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x0006C4A4 File Offset: 0x0006A6A4
		// (set) Token: 0x0600134A RID: 4938 RVA: 0x0000EC35 File Offset: 0x0000CE35
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

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x0006C4B8 File Offset: 0x0006A6B8
		// (set) Token: 0x0600134C RID: 4940 RVA: 0x0000EC49 File Offset: 0x0000CE49
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

		// Token: 0x0600134D RID: 4941 RVA: 0x0006C4CC File Offset: 0x0006A6CC
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

		// Token: 0x0600134E RID: 4942 RVA: 0x0006C5B8 File Offset: 0x0006A7B8
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

		// Token: 0x040016DA RID: 5850
		private NetworkRuleChoiceMask networkRuleChoiceMaskComponent;

		// Token: 0x040016DB RID: 5851
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x040016DC RID: 5852
		private readonly RuleChoiceMask serverAvailableChoiceMask = new RuleChoiceMask();

		// Token: 0x040016DD RID: 5853
		public ulong runSeed;

		// Token: 0x040016DE RID: 5854
		[SyncVar]
		public int gameModeIndex;

		// Token: 0x040016DF RID: 5855
		private readonly RuleBook ruleBookBuffer = new RuleBook();

		// Token: 0x040016E0 RID: 5856
		private static RuleBook persistentRuleBook;

		// Token: 0x040016E1 RID: 5857
		[SyncVar]
		private int pregameStateInternal;

		// Token: 0x040016E2 RID: 5858
		private const float launchTransitionDuration = 0f;

		// Token: 0x040016E3 RID: 5859
		private GameObject gameModePrefab;

		// Token: 0x040016E4 RID: 5860
		[SyncVar]
		private float launchStartTime = float.PositiveInfinity;

		// Token: 0x040016E5 RID: 5861
		private readonly RuleChoiceMask unlockedChoiceMask = new RuleChoiceMask();

		// Token: 0x040016E6 RID: 5862
		private readonly RuleChoiceMask choiceMaskBuffer = new RuleChoiceMask();

		// Token: 0x02000390 RID: 912
		private enum PregameState
		{
			// Token: 0x040016E9 RID: 5865
			Idle,
			// Token: 0x040016EA RID: 5866
			Launching,
			// Token: 0x040016EB RID: 5867
			Launched
		}

		// Token: 0x02000391 RID: 913
		private class GameModeConVar : BaseConVar
		{
			// Token: 0x0600134F RID: 4943 RVA: 0x000090A8 File Offset: 0x000072A8
			public GameModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001350 RID: 4944 RVA: 0x0000EC5D File Offset: 0x0000CE5D
			static GameModeConVar()
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					PreGameController.GameModeConVar.instance.runPrefabComponent = GameModeCatalog.FindGameModePrefabComponent(PreGameController.GameModeConVar.instance.GetString());
				});
			}

			// Token: 0x06001351 RID: 4945 RVA: 0x0006C644 File Offset: 0x0006A844
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

			// Token: 0x06001352 RID: 4946 RVA: 0x0000EC93 File Offset: 0x0000CE93
			public override string GetString()
			{
				if (!this.runPrefabComponent)
				{
					return "ClassicRun";
				}
				return this.runPrefabComponent.gameObject.name;
			}

			// Token: 0x040016EC RID: 5868
			public static readonly PreGameController.GameModeConVar instance = new PreGameController.GameModeConVar("gamemode", ConVarFlags.None, "", "Sets the specified game mode as the one to use in the next run.");

			// Token: 0x040016ED RID: 5869
			public Run runPrefabComponent;
		}
	}
}
