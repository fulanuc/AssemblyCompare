using System;
using System.Collections.Generic;
using EntityStates;
using Facepunch.Steamworks;
using RoR2.UI.MainMenu;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000597 RID: 1431
	public class SteamLobbyFinder : MonoBehaviour
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600206D RID: 8301 RVA: 0x00013D37 File Offset: 0x00011F37
		private static Client steamClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x00017A03 File Offset: 0x00015C03
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnStartup()
		{
			SteamworksLobbyManager.onLobbiesUpdated += delegate()
			{
				SteamLobbyFinder.awaitingLobbyRefresh = false;
			};
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x00017A29 File Offset: 0x00015C29
		private void Awake()
		{
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = new SerializableEntityStateType(typeof(SteamLobbyFinder.LobbyStateStart));
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x00017A56 File Offset: 0x00015C56
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.stateMachine);
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x00017A63 File Offset: 0x00015C63
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbiesUpdated += this.OnLobbiesUpdated;
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x00017A76 File Offset: 0x00015C76
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbiesUpdated -= this.OnLobbiesUpdated;
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x0009CCC8 File Offset: 0x0009AEC8
		private void Update()
		{
			if (SteamLobbyFinder.steamClient.Lobby.IsValid && !SteamworksLobbyManager.ownsLobby)
			{
				if (this.stateMachine.state.GetType() != typeof(SteamLobbyFinder.LobbyStateNonLeader))
				{
					this.stateMachine.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
				}
				return;
			}
			this.refreshTimer -= Time.unscaledDeltaTime;
			this.age += Time.unscaledDeltaTime;
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x0009CD44 File Offset: 0x0009AF44
		private static void RequestLobbyListRefresh()
		{
			if (SteamLobbyFinder.awaitingLobbyRefresh)
			{
				return;
			}
			SteamLobbyFinder.awaitingLobbyRefresh = true;
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.StringFilters["appid"] = TextSerialization.ToStringInvariant(SteamLobbyFinder.steamClient.AppId);
			filter.StringFilters["build_id"] = RoR2Application.GetBuildId();
			filter.StringFilters["qp"] = "1";
			filter.StringFilters["total_max_players"] = TextSerialization.ToStringInvariant(RoR2Application.maxPlayers);
			LobbyList.Filter filter2 = filter;
			SteamLobbyFinder.steamClient.LobbyList.Refresh(filter2);
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00017A89 File Offset: 0x00015C89
		private void OnLobbiesUpdated()
		{
			SteamLobbyFinder.LobbyStateBase lobbyStateBase = this.stateMachine.state as SteamLobbyFinder.LobbyStateBase;
			if (lobbyStateBase == null)
			{
				return;
			}
			lobbyStateBase.OnLobbiesUpdated();
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00017AA5 File Offset: 0x00015CA5
		private static bool CanJoinLobby(int currentLobbySize, LobbyList.Lobby lobby)
		{
			return currentLobbySize + SteamLobbyFinder.GetRealLobbyPlayerCount(lobby) < lobby.MemberLimit;
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x0009CDD8 File Offset: 0x0009AFD8
		private static int GetRealLobbyPlayerCount(LobbyList.Lobby lobby)
		{
			string data = lobby.GetData("player_count");
			int result;
			if (data != null && TextSerialization.TryParseInvariant(data, out result))
			{
				return result;
			}
			return SteamLobbyFinder.steamClient.Lobby.MaxMembers;
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x00017AB7 File Offset: 0x00015CB7
		private static int GetCurrentLobbyRealPlayerCount()
		{
			return SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06002079 RID: 8313 RVA: 0x00017AC3 File Offset: 0x00015CC3
		// (set) Token: 0x0600207A RID: 8314 RVA: 0x00017ACA File Offset: 0x00015CCA
		public static bool userRequestedQuickplayQueue
		{
			get
			{
				return SteamLobbyFinder._userRequestedQuickplayQueue;
			}
			set
			{
				if (SteamLobbyFinder._userRequestedQuickplayQueue != value)
				{
					SteamLobbyFinder._userRequestedQuickplayQueue = value;
					SteamLobbyFinder.running = (SteamLobbyFinder._lobbyIsInQuickplayQueue || SteamLobbyFinder._userRequestedQuickplayQueue);
				}
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x0600207B RID: 8315 RVA: 0x00017AEE File Offset: 0x00015CEE
		// (set) Token: 0x0600207C RID: 8316 RVA: 0x00017AF5 File Offset: 0x00015CF5
		private static bool lobbyIsInQuickplayQueue
		{
			get
			{
				return SteamLobbyFinder._lobbyIsInQuickplayQueue;
			}
			set
			{
				if (SteamLobbyFinder._lobbyIsInQuickplayQueue != value)
				{
					SteamLobbyFinder._lobbyIsInQuickplayQueue = value;
					SteamLobbyFinder.running = (SteamLobbyFinder._lobbyIsInQuickplayQueue || SteamLobbyFinder._userRequestedQuickplayQueue);
				}
			}
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x00017B19 File Offset: 0x00015D19
		[ConCommand(commandName = "steam_quickplay_start")]
		private static void CCSteamQuickplayStart(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = true;
			SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(true);
		}

		// Token: 0x0600207E RID: 8318 RVA: 0x00017B2C File Offset: 0x00015D2C
		[ConCommand(commandName = "steam_quickplay_stop")]
		private static void CCSteamQuickplayStop(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = false;
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x0600207F RID: 8319 RVA: 0x0009CE10 File Offset: 0x0009B010
		static SteamLobbyFinder()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += SteamLobbyFinder.OnLobbyDataUpdated;
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient client)
			{
				SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(false);
				SteamLobbyFinder.userRequestedQuickplayQueue = false;
			};
			SteamworksLobbyManager.onLobbyOwnershipGained += delegate()
			{
				if (SteamworksLobbyManager.newestLobbyData.quickplayQueued)
				{
					SteamLobbyFinder.userRequestedQuickplayQueue = true;
				}
			};
			SteamworksLobbyManager.onLobbyOwnershipLost += delegate()
			{
				SteamLobbyFinder.userRequestedQuickplayQueue = false;
			};
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x00017B3E File Offset: 0x00015D3E
		private static void OnLobbyDataUpdated()
		{
			SteamLobbyFinder.lobbyIsInQuickplayQueue = SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002081 RID: 8321 RVA: 0x0009CE70 File Offset: 0x0009B070
		public static string GetResolvedStateString()
		{
			if (!SteamLobbyFinder.steamClient.Lobby.IsValid)
			{
				return Language.GetString("STEAM_LOBBY_STATUS_NOT_IN_LOBBY");
			}
			bool flag = SteamLobbyFinder.steamClient.Lobby.LobbyType == Lobby.Type.Public;
			if (SteamLobbyFinder.instance)
			{
				bool flag2 = SteamLobbyFinder.instance.stateMachine.state is SteamLobbyFinder.LobbyStateSingleSearch;
			}
			int totalPlayerCount = SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
			int totalMaxPlayers = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers;
			bool isFull = SteamworksLobbyManager.isFull;
			string token = string.Empty;
			object[] args = Array.Empty<object>();
			if (GameNetworkManager.singleton.isHost || (MultiplayerMenuController.instance && MultiplayerMenuController.instance.isInHostingState))
			{
				token = "STEAM_LOBBY_STATUS_STARTING_SERVER";
			}
			else if (SteamworksLobbyManager.newestLobbyData.starting)
			{
				token = "STEAM_LOBBY_STATUS_GAME_STARTING";
			}
			else if (SteamworksLobbyManager.newestLobbyData.shouldConnect)
			{
				token = "STEAM_LOBBY_STATUS_CONNECTING_TO_HOST";
			}
			else if (SteamLobbyFinder.instance && SteamLobbyFinder.instance.stateMachine.state is SteamLobbyFinder.LobbyStateStart)
			{
				token = "STEAM_LOBBY_STATUS_LAUNCHING_QUICKPLAY";
			}
			else if (SteamworksLobbyManager.isInLobby)
			{
				if (SteamworksLobbyManager.newestLobbyData.quickplayQueued)
				{
					if (!flag)
					{
						token = "STEAM_LOBBY_STATUS_QUICKPLAY_SEARCHING_FOR_EXISTING_LOBBY";
					}
					else
					{
						DateTime d = Util.UnixTimeStampToDateTimeUtc(SteamLobbyFinder.steamClient.Utils.GetServerRealTime());
						DateTime? quickplayCutoffTime = SteamworksLobbyManager.newestLobbyData.quickplayCutoffTime;
						if (quickplayCutoffTime == null)
						{
							token = "STEAM_LOBBY_STATUS_QUICKPLAY_WAITING_BELOW_MINIMUM_PLAYERS";
							args = new object[]
							{
								SteamworksLobbyManager.newestLobbyData.totalPlayerCount,
								SteamworksLobbyManager.newestLobbyData.totalMaxPlayers
							};
						}
						else
						{
							TimeSpan timeSpan = quickplayCutoffTime.Value - d;
							token = "STEAM_LOBBY_STATUS_QUICKPLAY_WAITING_ABOVE_MINIMUM_PLAYERS";
							args = new object[]
							{
								SteamworksLobbyManager.newestLobbyData.totalPlayerCount,
								SteamworksLobbyManager.newestLobbyData.totalMaxPlayers,
								Math.Max(0.0, timeSpan.TotalSeconds)
							};
						}
					}
				}
				else
				{
					token = "STEAM_LOBBY_STATUS_OUT_OF_QUICKPLAY";
				}
			}
			return Language.GetStringFormatted(token, args);
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06002082 RID: 8322 RVA: 0x00017B4F File Offset: 0x00015D4F
		// (set) Token: 0x06002083 RID: 8323 RVA: 0x0009D078 File Offset: 0x0009B278
		public static bool running
		{
			get
			{
				return SteamLobbyFinder.instance;
			}
			private set
			{
				if (SteamLobbyFinder.instance != value)
				{
					if (value)
					{
						SteamLobbyFinder.instance = new GameObject("SteamLobbyFinder", new Type[]
						{
							typeof(SteamLobbyFinder),
							typeof(SetDontDestroyOnLoad)
						}).GetComponent<SteamLobbyFinder>();
						return;
					}
					UnityEngine.Object.Destroy(SteamLobbyFinder.instance.gameObject);
					SteamLobbyFinder.instance = null;
				}
			}
		}

		// Token: 0x0400226F RID: 8815
		private float age;

		// Token: 0x04002270 RID: 8816
		public float joinOnlyDuration = 5f;

		// Token: 0x04002271 RID: 8817
		public float waitForFullDuration = 20f;

		// Token: 0x04002272 RID: 8818
		public float startDelayDuration = 1f;

		// Token: 0x04002273 RID: 8819
		public float refreshInterval = 2f;

		// Token: 0x04002274 RID: 8820
		private float refreshTimer;

		// Token: 0x04002275 RID: 8821
		private EntityStateMachine stateMachine;

		// Token: 0x04002276 RID: 8822
		private static bool awaitingLobbyRefresh;

		// Token: 0x04002277 RID: 8823
		private static SteamLobbyFinder instance;

		// Token: 0x04002278 RID: 8824
		private static bool _userRequestedQuickplayQueue;

		// Token: 0x04002279 RID: 8825
		private static bool _lobbyIsInQuickplayQueue;

		// Token: 0x02000598 RID: 1432
		private class LobbyStateBase : BaseState
		{
			// Token: 0x06002085 RID: 8325 RVA: 0x00017B8F File Offset: 0x00015D8F
			public override void OnEnter()
			{
				base.OnEnter();
				this.lobbyFinder = base.GetComponent<SteamLobbyFinder>();
				SteamworksLobbyManager.onLobbyOwnershipGained += this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost += this.OnLobbyOwnershipLost;
			}

			// Token: 0x06002086 RID: 8326 RVA: 0x00017BC5 File Offset: 0x00015DC5
			public override void OnExit()
			{
				SteamworksLobbyManager.onLobbyOwnershipGained -= this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost -= this.OnLobbyOwnershipLost;
				base.OnExit();
			}

			// Token: 0x06002087 RID: 8327 RVA: 0x000025DA File Offset: 0x000007DA
			public virtual void OnLobbiesUpdated()
			{
			}

			// Token: 0x06002088 RID: 8328 RVA: 0x00017BEF File Offset: 0x00015DEF
			private void OnLobbyOwnershipGained()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateStart());
			}

			// Token: 0x06002089 RID: 8329 RVA: 0x00017C01 File Offset: 0x00015E01
			private void OnLobbyOwnershipLost()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
			}

			// Token: 0x0400227A RID: 8826
			protected SteamLobbyFinder lobbyFinder;
		}

		// Token: 0x02000599 RID: 1433
		private class LobbyStateNonLeader : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x0600208B RID: 8331 RVA: 0x00017C13 File Offset: 0x00015E13
			public override void Update()
			{
				base.Update();
				if (SteamworksLobbyManager.ownsLobby)
				{
					if (SteamworksLobbyManager.hasMinimumPlayerCount)
					{
						this.outer.SetNextState(new SteamLobbyFinder.LobbyStateMultiSearch());
						return;
					}
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x0200059A RID: 1434
		private class LobbyStateStart : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x0600208D RID: 8333 RVA: 0x00017C52 File Offset: 0x00015E52
			public override void Update()
			{
				base.Update();
				if (this.lobbyFinder.startDelayDuration <= base.age)
				{
					this.outer.SetNextState(SteamworksLobbyManager.hasMinimumPlayerCount ? new SteamLobbyFinder.LobbyStateMultiSearch() : new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x0200059B RID: 1435
		private class LobbyStateSingleSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x0600208F RID: 8335 RVA: 0x0009D0E0 File Offset: 0x0009B2E0
			public override void OnEnter()
			{
				base.OnEnter();
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
			}

			// Token: 0x06002090 RID: 8336 RVA: 0x0009D104 File Offset: 0x0009B304
			public override void Update()
			{
				base.Update();
				if (SteamworksLobbyManager.hasMinimumPlayerCount)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateMultiSearch());
					return;
				}
				if (this.lobbyFinder.refreshTimer <= 0f)
				{
					if (base.age >= this.lobbyFinder.joinOnlyDuration && SteamLobbyFinder.steamClient.Lobby.LobbyType != Lobby.Type.Public)
					{
						Debug.LogFormat("Unable to find joinable lobby after {0} seconds. Setting lobby to public.", new object[]
						{
							this.lobbyFinder.age
						});
						SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
					}
					this.lobbyFinder.refreshTimer = this.lobbyFinder.refreshInterval;
					SteamLobbyFinder.RequestLobbyListRefresh();
				}
			}

			// Token: 0x06002091 RID: 8337 RVA: 0x0009D1B4 File Offset: 0x0009B3B4
			public override void OnLobbiesUpdated()
			{
				base.OnLobbiesUpdated();
				if (SteamLobbyFinder.steamClient.IsValid)
				{
					List<LobbyList.Lobby> lobbies = SteamLobbyFinder.steamClient.LobbyList.Lobbies;
					List<LobbyList.Lobby> list = new List<LobbyList.Lobby>();
					ulong currentLobby = SteamLobbyFinder.steamClient.Lobby.CurrentLobby;
					bool isValid = SteamLobbyFinder.steamClient.Lobby.IsValid;
					int currentLobbySize = isValid ? SteamLobbyFinder.GetCurrentLobbyRealPlayerCount() : LocalUserManager.readOnlyLocalUsersList.Count;
					if (SteamworksLobbyManager.ownsLobby || !isValid)
					{
						for (int i = 0; i < lobbies.Count; i++)
						{
							if ((!isValid || lobbies[i].LobbyID < currentLobby) && SteamLobbyFinder.CanJoinLobby(currentLobbySize, lobbies[i]))
							{
								list.Add(lobbies[i]);
							}
						}
						if (list.Count > 0)
						{
							SteamworksLobbyManager.JoinLobby(new CSteamID(list[0].LobbyID));
						}
					}
					Debug.LogFormat("Found {0} lobbies, {1} joinable.", new object[]
					{
						lobbies.Count,
						list.Count
					});
				}
			}
		}

		// Token: 0x0200059C RID: 1436
		private class LobbyStateMultiSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002093 RID: 8339 RVA: 0x0009D2C0 File Offset: 0x0009B4C0
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(new uint?((uint)(Util.UnixTimeStampToDateTimeUtc(SteamLobbyFinder.steamClient.Utils.GetServerRealTime()) + TimeSpan.FromSeconds((double)this.lobbyFinder.waitForFullDuration) - Util.dateZero).TotalSeconds));
			}

			// Token: 0x06002094 RID: 8340 RVA: 0x0009D32C File Offset: 0x0009B52C
			public override void OnExit()
			{
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
				base.OnExit();
			}

			// Token: 0x06002095 RID: 8341 RVA: 0x0009D350 File Offset: 0x0009B550
			public override void Update()
			{
				base.Update();
				if (!SteamworksLobbyManager.hasMinimumPlayerCount)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateSingleSearch());
					return;
				}
				if (this.lobbyFinder.waitForFullDuration <= base.age)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateBeginGame());
				}
			}
		}

		// Token: 0x0200059D RID: 1437
		private class LobbyStateBeginGame : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002097 RID: 8343 RVA: 0x0009D3A0 File Offset: 0x0009B5A0
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
				SteamworksLobbyManager.SetStartingIfOwner(true);
				string arg = "ClassicRun";
				Console.instance.SubmitCmd(null, string.Format("transition_command \"gamemode {0}; host 1;\"", arg), false);
			}
		}
	}
}
