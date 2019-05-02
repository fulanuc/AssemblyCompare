using System;
using System.Collections.Generic;
using EntityStates;
using Facepunch.Steamworks;
using RoR2.UI.MainMenu;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000584 RID: 1412
	public class SteamLobbyFinder : MonoBehaviour
	{
		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06001FDC RID: 8156 RVA: 0x00013821 File Offset: 0x00011A21
		private static Client steamClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x000172F3 File Offset: 0x000154F3
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnStartup()
		{
			SteamworksLobbyManager.onLobbiesUpdated += delegate()
			{
				SteamLobbyFinder.awaitingLobbyRefresh = false;
			};
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x00017319 File Offset: 0x00015519
		private void Awake()
		{
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = new SerializableEntityStateType(typeof(SteamLobbyFinder.LobbyStateStart));
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x00017346 File Offset: 0x00015546
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.stateMachine);
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x00017353 File Offset: 0x00015553
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbiesUpdated += this.OnLobbiesUpdated;
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x00017366 File Offset: 0x00015566
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbiesUpdated -= this.OnLobbiesUpdated;
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x0009B7C0 File Offset: 0x000999C0
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

		// Token: 0x06001FE3 RID: 8163 RVA: 0x0009B83C File Offset: 0x00099A3C
		private static void RequestLobbyListRefresh()
		{
			if (SteamLobbyFinder.awaitingLobbyRefresh)
			{
				return;
			}
			SteamLobbyFinder.awaitingLobbyRefresh = true;
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.StringFilters["appid"] = TextSerialization.ToStringInvariant(SteamLobbyFinder.steamClient.AppId).ToString();
			filter.StringFilters["build_id"] = RoR2Application.GetBuildId();
			filter.StringFilters["qp"] = "1";
			LobbyList.Filter filter2 = filter;
			SteamLobbyFinder.steamClient.LobbyList.Refresh(filter2);
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x00017379 File Offset: 0x00015579
		private void OnLobbiesUpdated()
		{
			SteamLobbyFinder.LobbyStateBase lobbyStateBase = this.stateMachine.state as SteamLobbyFinder.LobbyStateBase;
			if (lobbyStateBase == null)
			{
				return;
			}
			lobbyStateBase.OnLobbiesUpdated();
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x00017395 File Offset: 0x00015595
		private static bool CanJoinLobby(int currentLobbySize, LobbyList.Lobby lobby)
		{
			return currentLobbySize + SteamLobbyFinder.GetRealLobbyPlayerCount(lobby) < lobby.MemberLimit;
		}

		// Token: 0x06001FE6 RID: 8166 RVA: 0x0009B8BC File Offset: 0x00099ABC
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

		// Token: 0x06001FE7 RID: 8167 RVA: 0x000173A7 File Offset: 0x000155A7
		private static int GetCurrentLobbyRealPlayerCount()
		{
			return SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06001FE8 RID: 8168 RVA: 0x000173B3 File Offset: 0x000155B3
		// (set) Token: 0x06001FE9 RID: 8169 RVA: 0x000173BA File Offset: 0x000155BA
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

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06001FEA RID: 8170 RVA: 0x000173DE File Offset: 0x000155DE
		// (set) Token: 0x06001FEB RID: 8171 RVA: 0x000173E5 File Offset: 0x000155E5
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

		// Token: 0x06001FEC RID: 8172 RVA: 0x00017409 File Offset: 0x00015609
		[ConCommand(commandName = "steam_quickplay_start")]
		private static void CCSteamQuickplayStart(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = true;
			SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(true);
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x0001741C File Offset: 0x0001561C
		[ConCommand(commandName = "steam_quickplay_stop")]
		private static void CCSteamQuickplayStop(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = false;
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x0009B8F4 File Offset: 0x00099AF4
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

		// Token: 0x06001FEF RID: 8175 RVA: 0x0001742E File Offset: 0x0001562E
		private static void OnLobbyDataUpdated()
		{
			SteamLobbyFinder.lobbyIsInQuickplayQueue = SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0009B954 File Offset: 0x00099B54
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
						DateTime d = Util.UnixTimeStampToDateTime(SteamLobbyFinder.steamClient.Utils.GetServerRealTime());
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

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06001FF1 RID: 8177 RVA: 0x0001743F File Offset: 0x0001563F
		// (set) Token: 0x06001FF2 RID: 8178 RVA: 0x0009BB5C File Offset: 0x00099D5C
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

		// Token: 0x04002218 RID: 8728
		private float age;

		// Token: 0x04002219 RID: 8729
		public float joinOnlyDuration = 5f;

		// Token: 0x0400221A RID: 8730
		public float waitForFullDuration = 20f;

		// Token: 0x0400221B RID: 8731
		public float startDelayDuration = 1f;

		// Token: 0x0400221C RID: 8732
		public float refreshInterval = 2f;

		// Token: 0x0400221D RID: 8733
		private float refreshTimer;

		// Token: 0x0400221E RID: 8734
		private EntityStateMachine stateMachine;

		// Token: 0x0400221F RID: 8735
		private static bool awaitingLobbyRefresh;

		// Token: 0x04002220 RID: 8736
		private static SteamLobbyFinder instance;

		// Token: 0x04002221 RID: 8737
		private static bool _userRequestedQuickplayQueue;

		// Token: 0x04002222 RID: 8738
		private static bool _lobbyIsInQuickplayQueue;

		// Token: 0x02000585 RID: 1413
		private class LobbyStateBase : BaseState
		{
			// Token: 0x06001FF4 RID: 8180 RVA: 0x0001747F File Offset: 0x0001567F
			public override void OnEnter()
			{
				base.OnEnter();
				this.lobbyFinder = base.GetComponent<SteamLobbyFinder>();
				SteamworksLobbyManager.onLobbyOwnershipGained += this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost += this.OnLobbyOwnershipLost;
			}

			// Token: 0x06001FF5 RID: 8181 RVA: 0x000174B5 File Offset: 0x000156B5
			public override void OnExit()
			{
				SteamworksLobbyManager.onLobbyOwnershipGained -= this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost -= this.OnLobbyOwnershipLost;
				base.OnExit();
			}

			// Token: 0x06001FF6 RID: 8182 RVA: 0x000025F6 File Offset: 0x000007F6
			public virtual void OnLobbiesUpdated()
			{
			}

			// Token: 0x06001FF7 RID: 8183 RVA: 0x000174DF File Offset: 0x000156DF
			private void OnLobbyOwnershipGained()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateStart());
			}

			// Token: 0x06001FF8 RID: 8184 RVA: 0x000174F1 File Offset: 0x000156F1
			private void OnLobbyOwnershipLost()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
			}

			// Token: 0x04002223 RID: 8739
			protected SteamLobbyFinder lobbyFinder;
		}

		// Token: 0x02000586 RID: 1414
		private class LobbyStateNonLeader : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06001FFA RID: 8186 RVA: 0x00017503 File Offset: 0x00015703
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

		// Token: 0x02000587 RID: 1415
		private class LobbyStateStart : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06001FFC RID: 8188 RVA: 0x00017542 File Offset: 0x00015742
			public override void Update()
			{
				base.Update();
				if (this.lobbyFinder.startDelayDuration <= base.age)
				{
					this.outer.SetNextState(SteamworksLobbyManager.hasMinimumPlayerCount ? new SteamLobbyFinder.LobbyStateMultiSearch() : new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x02000588 RID: 1416
		private class LobbyStateSingleSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06001FFE RID: 8190 RVA: 0x0009BBC4 File Offset: 0x00099DC4
			public override void OnEnter()
			{
				base.OnEnter();
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
			}

			// Token: 0x06001FFF RID: 8191 RVA: 0x0009BBE8 File Offset: 0x00099DE8
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

			// Token: 0x06002000 RID: 8192 RVA: 0x0009BC98 File Offset: 0x00099E98
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

		// Token: 0x02000589 RID: 1417
		private class LobbyStateMultiSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002002 RID: 8194 RVA: 0x0009BDA4 File Offset: 0x00099FA4
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(new uint?((uint)(Util.UnixTimeStampToDateTime(SteamLobbyFinder.steamClient.Utils.GetServerRealTime()) + TimeSpan.FromSeconds((double)this.lobbyFinder.waitForFullDuration) - Util.dateZero).TotalSeconds));
			}

			// Token: 0x06002003 RID: 8195 RVA: 0x0009BE10 File Offset: 0x0009A010
			public override void OnExit()
			{
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
				base.OnExit();
			}

			// Token: 0x06002004 RID: 8196 RVA: 0x0009BE34 File Offset: 0x0009A034
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

		// Token: 0x0200058A RID: 1418
		private class LobbyStateBeginGame : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002006 RID: 8198 RVA: 0x0009BE84 File Offset: 0x0009A084
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
