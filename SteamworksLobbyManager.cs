using System;
using System.Collections.Generic;
using System.Reflection;
using Facepunch.Steamworks;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200049F RID: 1183
	public class SteamworksLobbyManager
	{
		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06001A56 RID: 6742 RVA: 0x00013821 File Offset: 0x00011A21
		private static Client client
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06001A57 RID: 6743 RVA: 0x00013828 File Offset: 0x00011A28
		// (set) Token: 0x06001A58 RID: 6744 RVA: 0x0001382F File Offset: 0x00011A2F
		public static bool isInLobby { get; private set; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06001A59 RID: 6745 RVA: 0x00013837 File Offset: 0x00011A37
		// (set) Token: 0x06001A5A RID: 6746 RVA: 0x0001383E File Offset: 0x00011A3E
		public static bool ownsLobby
		{
			get
			{
				return SteamworksLobbyManager._ownsLobby;
			}
			private set
			{
				if (value != SteamworksLobbyManager._ownsLobby)
				{
					SteamworksLobbyManager._ownsLobby = value;
					if (SteamworksLobbyManager._ownsLobby)
					{
						SteamworksLobbyManager.OnLobbyOwnershipGained();
						SteamworksLobbyManager.UpdatePlayerCount();
						return;
					}
					SteamworksLobbyManager.OnLobbyOwnershipLost();
				}
			}
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x00013865 File Offset: 0x00011A65
		private static void UpdateOwnsLobby()
		{
			SteamworksLobbyManager.ownsLobby = SteamworksLobbyManager.client.Lobby.IsOwner;
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06001A5C RID: 6748 RVA: 0x0001387B File Offset: 0x00011A7B
		public static bool hasMinimumPlayerCount
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.totalPlayerCount >= SteamworksLobbyManager.minimumPlayerCount;
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06001A5D RID: 6749 RVA: 0x00013891 File Offset: 0x00011A91
		// (set) Token: 0x06001A5E RID: 6750 RVA: 0x00013898 File Offset: 0x00011A98
		public static bool isFull { get; private set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x000138A0 File Offset: 0x00011AA0
		public static CSteamID serverId
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.serverId;
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06001A60 RID: 6752 RVA: 0x000138AC File Offset: 0x00011AAC
		// (set) Token: 0x06001A61 RID: 6753 RVA: 0x000138B3 File Offset: 0x00011AB3
		public static SteamworksLobbyManager.LobbyData newestLobbyData { get; private set; }

		// Token: 0x06001A62 RID: 6754 RVA: 0x00085E94 File Offset: 0x00084094
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			SteamworksLobbyManager.newestLobbyData = new SteamworksLobbyManager.LobbyData();
			LocalUserManager.onLocalUsersUpdated += SteamworksLobbyManager.UpdatePlayerCount;
			GameNetworkManager.onStopClientGlobal += delegate()
			{
				NetworkConnection connection = GameNetworkManager.singleton.client.connection;
				bool flag = Util.ConnectionIsLocal(connection);
				bool flag2;
				if (connection is SteamNetworkConnection)
				{
					flag2 = (((SteamNetworkConnection)connection).steamId == SteamworksLobbyManager.newestLobbyData.serverId);
				}
				else
				{
					flag2 = (connection.address == SteamworksLobbyManager.newestLobbyData.serverAddressPortPair.address);
				}
				if (flag && SteamworksLobbyManager.ownsLobby)
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobbyData.RemoveData("server_id");
				}
				if (!flag && flag2)
				{
					SteamworksLobbyManager.client.Lobby.Leave();
				}
			};
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient networkClient)
			{
				if (SteamworksLobbyManager.ownsLobby)
				{
					SteamworksLobbyManager.client.Lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
				}
			};
			SteamworksLobbyManager.onLobbyOwnershipGained += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
			GameNetworkManager.onStopClientGlobal += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x00085F4C File Offset: 0x0008414C
		public static void SetupCallbacks(Client client)
		{
			client.Lobby.OnChatMessageRecieved = new Action<ulong, byte[], int>(SteamworksLobbyManager.OnChatMessageReceived);
			client.Lobby.OnLobbyCreated = new Action<bool>(SteamworksLobbyManager.OnLobbyCreated);
			client.Lobby.OnLobbyDataUpdated = new Action(SteamworksLobbyManager.OnLobbyDataUpdated);
			client.Lobby.OnLobbyJoined = new Action<bool>(SteamworksLobbyManager.OnLobbyJoined);
			client.Lobby.OnLobbyMemberDataUpdated = new Action<ulong>(SteamworksLobbyManager.OnLobbyMemberDataUpdated);
			client.Lobby.OnLobbyStateChanged = new Action<Lobby.MemberStateChange, ulong, ulong>(SteamworksLobbyManager.OnLobbyStateChanged);
			client.Lobby.OnLobbyKicked = new Action<bool, ulong, ulong>(SteamworksLobbyManager.OnLobbyKicked);
			client.Lobby.OnLobbyLeave = new Action<ulong>(SteamworksLobbyManager.OnLobbyLeave);
			client.Lobby.OnUserInvitedToLobby = new Action<ulong, ulong>(SteamworksLobbyManager.OnUserInvitedToLobby);
			client.Lobby.OnLobbyJoinRequested = new Action<ulong>(SteamworksLobbyManager.OnLobbyJoinRequested);
			client.LobbyList.OnLobbiesUpdated = new Action(SteamworksLobbyManager.OnLobbiesUpdated);
			RoR2Application.onUpdate += SteamworksLobbyManager.StaticUpdate;
			SteamworksLobbyManager.SetStartingIfOwner(false);
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x000138BB File Offset: 0x00011ABB
		public static int GetLobbyMemberPlayerCountByIndex(int memberIndex)
		{
			if (memberIndex >= SteamworksLobbyManager.playerCountsList.Count)
			{
				return 0;
			}
			return SteamworksLobbyManager.playerCountsList[memberIndex];
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06001A65 RID: 6757 RVA: 0x000138D7 File Offset: 0x00011AD7
		// (set) Token: 0x06001A66 RID: 6758 RVA: 0x000138DE File Offset: 0x00011ADE
		public static int calculatedTotalPlayerCount { get; private set; }

		// Token: 0x06001A67 RID: 6759 RVA: 0x00086070 File Offset: 0x00084270
		private static void UpdatePlayerCount()
		{
			if (SteamworksLobbyManager.client != null && SteamworksLobbyManager.client.Lobby.IsValid)
			{
				int count = LocalUserManager.readOnlyLocalUsersList.Count;
				string memberData = SteamworksLobbyManager.client.Lobby.GetMemberData(SteamworksLobbyManager.client.SteamId, "player_count");
				string text = TextSerialization.ToStringInvariant(Math.Max(1, count));
				if (memberData != text)
				{
					SteamworksLobbyManager.client.Lobby.SetMemberData("player_count", text);
				}
				SteamworksLobbyManager.playerCountsList.Clear();
				SteamworksLobbyManager.calculatedTotalPlayerCount = 0;
				ulong steamId = SteamworksLobbyManager.client.SteamId;
				int num = 0;
				foreach (ulong num2 in SteamworksLobbyManager.client.Lobby.GetMemberIDs())
				{
					int num3 = TextSerialization.TryParseInvariant(SteamworksLobbyManager.client.Lobby.GetMemberData(num2, "player_count"), out num3) ? Math.Min(Math.Max(1, num3), 4) : 1;
					if (num2 == steamId)
					{
						num3 = Math.Max(1, count);
					}
					SteamworksLobbyManager.playerCountsList.Add(num3);
					SteamworksLobbyManager.calculatedTotalPlayerCount += num3;
					if (num3 > 1)
					{
						num += num3 - 1;
					}
				}
				if (SteamworksLobbyManager.ownsLobby)
				{
					string data = SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetData("player_count");
					string b = TextSerialization.ToStringInvariant(SteamworksLobbyManager.calculatedTotalPlayerCount);
					if (data != b)
					{
						SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("player_count", b);
					}
					int maxMembers = SteamworksLobbyManager.client.Lobby.MaxMembers;
					int num4 = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers - num;
					if (maxMembers != num4)
					{
						SteamworksLobbyManager.client.Lobby.MaxMembers = num4;
					}
				}
			}
			Action action = SteamworksLobbyManager.onPlayerCountUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x000138E6 File Offset: 0x00011AE6
		[ConCommand(commandName = "steam_lobby_update_player_count", flags = ConVarFlags.None, helpText = "Forces a refresh of the steam lobby player count.")]
		private static void CCSteamLobbyUpdatePlayerCount(ConCommandArgs args)
		{
			SteamworksLobbyManager.UpdatePlayerCount();
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06001A69 RID: 6761 RVA: 0x00086230 File Offset: 0x00084430
		// (remove) Token: 0x06001A6A RID: 6762 RVA: 0x00086264 File Offset: 0x00084464
		public static event Action onPlayerCountUpdated;

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001A6B RID: 6763 RVA: 0x00086298 File Offset: 0x00084498
		// (remove) Token: 0x06001A6C RID: 6764 RVA: 0x000862CC File Offset: 0x000844CC
		public static event Action onLobbyChanged;

		// Token: 0x06001A6D RID: 6765 RVA: 0x00086300 File Offset: 0x00084500
		private static void OnLobbyChanged()
		{
			SteamworksLobbyManager.isInLobby = SteamworksLobbyManager.client.Lobby.IsValid;
			SteamworksLobbyManager.UpdateOwnsLobby();
			if (SteamworksLobbyManager.client.Lobby.CurrentLobbyData != null)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("total_max_players", TextSerialization.ToStringInvariant(4));
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("build_id", RoR2Application.GetBuildId());
			}
			SteamworksLobbyManager.UpdatePlayerCount();
			Action action = SteamworksLobbyManager.onLobbyChanged;
			if (action != null)
			{
				action();
			}
			SteamworksLobbyManager.OnLobbyDataUpdated();
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x000138ED File Offset: 0x00011AED
		public static void CreateLobby()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.client.Lobby.Create(SteamworksLobbyManager.preferredLobbyType, 4);
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06001A6F RID: 6767 RVA: 0x0001391B File Offset: 0x00011B1B
		// (set) Token: 0x06001A70 RID: 6768 RVA: 0x00013922 File Offset: 0x00011B22
		public static bool awaitingJoin { get; private set; }

		// Token: 0x06001A71 RID: 6769 RVA: 0x0001392A File Offset: 0x00011B2A
		public static void JoinLobby(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.awaitingJoin = true;
			SteamworksLobbyManager.client.Lobby.Join(newLobbyId.value);
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0001394F File Offset: 0x00011B4F
		public static void LeaveLobby()
		{
			Client client = SteamworksLobbyManager.client;
			if (client == null)
			{
				return;
			}
			client.Lobby.Leave();
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x00086390 File Offset: 0x00084590
		private static void Update()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.startingFadeSet != (SteamworksLobbyManager.newestLobbyData.starting && !ClientScene.ready))
			{
				if (SteamworksLobbyManager.startingFadeSet)
				{
					FadeToBlackManager.fadeCount--;
				}
				else
				{
					FadeToBlackManager.fadeCount++;
				}
				SteamworksLobbyManager.startingFadeSet = !SteamworksLobbyManager.startingFadeSet;
			}
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x00013965 File Offset: 0x00011B65
		private static void StaticUpdate()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.UpdateOwnsLobby();
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x00013974 File Offset: 0x00011B74
		[ConCommand(commandName = "steam_lobby_create")]
		private static void CCSteamLobbyCreate(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x00013980 File Offset: 0x00011B80
		[ConCommand(commandName = "steam_lobby_create_if_none")]
		private static void CCSteamLobbyCreateIfNone(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!SteamworksLobbyManager.client.Lobby.IsValid)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0001399D File Offset: 0x00011B9D
		[ConCommand(commandName = "steam_lobby_find")]
		private static void CCSteamLobbyFind(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.awaitingLobbyListUpdate = true;
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x000863F4 File Offset: 0x000845F4
		[ConCommand(commandName = "steam_lobby_join")]
		private static void CCSteamLobbyJoin(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			Debug.LogFormat("Joining lobby {0}...", new object[]
			{
				args[0]
			});
			CSteamID newLobbyId;
			if (CSteamID.TryParse(args[0], out newLobbyId))
			{
				SteamworksLobbyManager.JoinLobby(newLobbyId);
				return;
			}
			throw new ConCommandException("Could not parse lobby id.");
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x000139AA File Offset: 0x00011BAA
		[ConCommand(commandName = "steam_lobby_leave")]
		private static void CCSteamLobbyLeave(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.LeaveLobby();
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0008644C File Offset: 0x0008464C
		[ConCommand(commandName = "steam_lobby_assign_owner")]
		private static void CCSteamLobbyAssignOwner(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			Debug.LogFormat("Promoting {0} to lobby leader...", new object[]
			{
				args[0]
			});
			CSteamID csteamID;
			if (CSteamID.TryParse(args[0], out csteamID))
			{
				SteamworksLobbyManager.client.Lobby.Owner = csteamID.value;
				return;
			}
			throw new ConCommandException("Could not parse steamID.");
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x000864B4 File Offset: 0x000846B4
		[ConCommand(commandName = "steam_lobby_invite", flags = ConVarFlags.None, helpText = "Invites the player with the specified steam id to the current lobby.")]
		private static void CCSteamLobbyInvite(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID csteamID;
			if (CSteamID.TryParse(args[0], out csteamID))
			{
				SteamworksLobbyManager.client.Lobby.InviteUserToLobby(csteamID.value);
				return;
			}
			throw new ConCommandException("Could not parse user id.");
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x000139B6 File Offset: 0x00011BB6
		[ConCommand(commandName = "steam_lobby_open_invite_overlay", flags = ConVarFlags.None, helpText = "Opens the steam overlay to the friend invite dialog.")]
		private static void CCSteamLobbyOpenInviteOverlay(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.client.Overlay.OpenInviteDialog(SteamworksLobbyManager.client.Lobby.CurrentLobby);
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x000139DB File Offset: 0x00011BDB
		[ConCommand(commandName = "steam_lobby_copy_to_clipboard", flags = ConVarFlags.None, helpText = "Copies the currently active lobby to the clipboard if applicable.")]
		private static void CCSteamLobbyCopyToClipboard(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			GUIUtility.systemCopyBuffer = TextSerialization.ToStringInvariant(SteamworksLobbyManager.client.Lobby.CurrentLobby);
			Chat.AddMessage(Language.GetString("STEAM_COPY_LOBBY_TO_CLIPBOARD_MESSAGE"));
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x00086500 File Offset: 0x00084700
		[ConCommand(commandName = "steam_lobby_print_data", flags = ConVarFlags.None, helpText = "Prints all data about the current steam lobby.")]
		private static void CCSteamLobbyPrintData(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (SteamworksLobbyManager.client.Lobby.IsValid)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, string> keyValuePair in SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetAllData())
				{
					list.Add(string.Format("\"{0}\" = \"{1}\"", keyValuePair.Key, keyValuePair.Value));
				}
				Debug.Log(string.Join("\n", list.ToArray()));
			}
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x00013A0A File Offset: 0x00011C0A
		[ConCommand(commandName = "steam_id", flags = ConVarFlags.None, helpText = "Displays your steam id.")]
		private static void CCSteamId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Steam id = {0}", new object[]
			{
				SteamworksLobbyManager.client.SteamId
			});
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x00013A33 File Offset: 0x00011C33
		[ConCommand(commandName = "steam_lobby_id", flags = ConVarFlags.None, helpText = "Displays the steam id of the current lobby.")]
		private static void CCSteamLobbyId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Lobby id = {0}", new object[]
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobby
			});
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x000865AC File Offset: 0x000847AC
		[ConCommand(commandName = "steam_lobby_print_members", flags = ConVarFlags.None, helpText = "Displays the members current lobby.")]
		private static void CCSteamLobbyPrintMembers(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			ulong[] memberIDs = SteamworksLobbyManager.client.Lobby.GetMemberIDs();
			string[] array = new string[memberIDs.Length];
			for (int i = 0; i < memberIDs.Length; i++)
			{
				array[i] = string.Format("[{0}]{1} id={2} name={3}", new object[]
				{
					i,
					(SteamworksLobbyManager.client.Lobby.Owner == memberIDs[i]) ? "*" : " ",
					memberIDs[i],
					SteamworksLobbyManager.client.Friends.GetName(memberIDs[i])
				});
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x00086654 File Offset: 0x00084854
		[ConCommand(commandName = "steam_lobby_print_list", flags = ConVarFlags.None, helpText = "Displays a list of lobbies from the last search.")]
		private static void CCSteamLobbyPrintList(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			List<LobbyList.Lobby> lobbies = SteamworksLobbyManager.client.LobbyList.Lobbies;
			string[] array = new string[lobbies.Count];
			for (int i = 0; i < lobbies.Count; i++)
			{
				array[i] = string.Format("[{0}] id={1}\n      players={2}/{3},\n      owner=\"{4}\"", new object[]
				{
					i,
					lobbies[i].LobbyID,
					lobbies[i].NumMembers,
					lobbies[i].MemberLimit,
					SteamworksLobbyManager.client.Friends.GetName(lobbies[i].Owner)
				});
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x00013A61 File Offset: 0x00011C61
		[ConCommand(commandName = "steam_lobby_force_update_data", flags = ConVarFlags.Cheat, helpText = "Forces Facepunch.Steamworks lobby data to be re-pulled from the Steamworks API.")]
		private static void CCSteamLobbyForceUpdateData(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.ForceLobbyDataUpdate();
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x0008671C File Offset: 0x0008491C
		public static void ForceLobbyDataUpdate()
		{
			Client client = SteamworksLobbyManager.client;
			Lobby lobby = (client != null) ? client.Lobby : null;
			if (lobby != null)
			{
				SteamworksLobbyManager.updateLobbyDataMethodInfo.Invoke(lobby, Array.Empty<object>());
			}
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x00086750 File Offset: 0x00084950
		private static void OnChatMessageReceived(ulong senderId, byte[] buffer, int byteCount)
		{
			NetworkReader networkReader = new NetworkReader(buffer);
			if (byteCount >= 1 && networkReader.ReadByte() == 0)
			{
				Chat.AddMessage(string.Format("{0}: {1}", Client.Instance.Friends.Get(senderId), networkReader.ReadString()));
			}
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x00013A6D File Offset: 0x00011C6D
		public static void JoinOrStartMigrate(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.ownsLobby)
			{
				SteamworksLobbyManager.StartMigrateLobby(newLobbyId);
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.JoinLobby(newLobbyId);
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x00013A92 File Offset: 0x00011C92
		public static void StartMigrateLobby(CSteamID newLobbyId)
		{
			SteamworksLobbyManager.client.Lobby.Joinable = false;
			SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("migration_id", TextSerialization.ToStringInvariant(newLobbyId.value));
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x00013AC9 File Offset: 0x00011CC9
		private static void OnLobbyCreated(bool success)
		{
			if (success)
			{
				Debug.LogFormat("Steamworks lobby creation succeeded. Lobby id = {0}", new object[]
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobby
				});
				SteamworksLobbyManager.OnLobbyChanged();
				return;
			}
			Debug.Log("Steamworks lobby creation failed.");
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001A89 RID: 6793 RVA: 0x00086798 File Offset: 0x00084998
		// (remove) Token: 0x06001A8A RID: 6794 RVA: 0x000867CC File Offset: 0x000849CC
		public static event Action onLobbyDataUpdated;

		// Token: 0x06001A8B RID: 6795 RVA: 0x00086800 File Offset: 0x00084A00
		private static void OnLobbyDataUpdated()
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			SteamworksLobbyManager.newestLobbyData = (lobby.IsValid ? new SteamworksLobbyManager.LobbyData(lobby.CurrentLobbyData) : new SteamworksLobbyManager.LobbyData());
			SteamworksLobbyManager.UpdateOwnsLobby();
			SteamworksLobbyManager.UpdatePlayerCount();
			if (lobby.IsValid && !SteamworksLobbyManager.ownsLobby)
			{
				if (SteamworksLobbyManager.newestLobbyData.serverId.isValid)
				{
					if (!GameNetworkManager.singleton.IsConnectedToServer(SteamworksLobbyManager.newestLobbyData.serverId) && RoR2Application.GetBuildId() == SteamworksLobbyManager.newestLobbyData.buildId)
					{
						Console.instance.SubmitCmd(null, "connect_steamworks_p2p " + SteamworksLobbyManager.newestLobbyData.serverId, false);
					}
				}
				else if (GameNetworkManager.singleton.isNetworkActive)
				{
					Console.instance.SubmitCmd(null, "disconnect", false);
				}
			}
			Action action = SteamworksLobbyManager.onLobbyDataUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06001A8C RID: 6796 RVA: 0x000868EC File Offset: 0x00084AEC
		// (remove) Token: 0x06001A8D RID: 6797 RVA: 0x00086920 File Offset: 0x00084B20
		public static event Action<bool> onLobbyJoined;

		// Token: 0x06001A8E RID: 6798 RVA: 0x00086954 File Offset: 0x00084B54
		private static void OnLobbyJoined(bool success)
		{
			SteamworksLobbyManager.awaitingJoin = false;
			if (success)
			{
				if (SteamworksLobbyManager.client.Lobby.CurrentLobbyData != null)
				{
					string buildId = RoR2Application.GetBuildId();
					string data = SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetData("build_id");
					if (buildId != data)
					{
						Debug.LogFormat("Lobby build_id mismatch, leaving lobby. Ours=\"{0}\" Theirs=\"{1}\"", new object[]
						{
							buildId,
							data
						});
						SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
						simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
						simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "STEAM_LOBBY_VERSION_MISMATCH_DIALOG_TITLE",
							formatParams = Array.Empty<object>()
						};
						SimpleDialogBox.TokenParamsPair descriptionToken = default(SimpleDialogBox.TokenParamsPair);
						descriptionToken.token = "STEAM_LOBBY_VERSION_MISMATCH_DIALOG_DESCRIPTION";
						object[] formatParams = new string[]
						{
							buildId,
							data
						};
						descriptionToken.formatParams = formatParams;
						simpleDialogBox.descriptionToken = descriptionToken;
						SteamworksLobbyManager.client.Lobby.Leave();
						return;
					}
				}
				Debug.LogFormat("Steamworks lobby join succeeded. Lobby id = {0}", new object[]
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobby
				});
				SteamworksLobbyManager.OnLobbyChanged();
			}
			else
			{
				Debug.Log("Steamworks lobby join failed.");
				Console.instance.SubmitCmd(null, "steam_lobby_create_if_none", true);
			}
			Action<bool> action = SteamworksLobbyManager.onLobbyJoined;
			if (action == null)
			{
				return;
			}
			action(success);
		}

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06001A8F RID: 6799 RVA: 0x00086AA0 File Offset: 0x00084CA0
		// (remove) Token: 0x06001A90 RID: 6800 RVA: 0x00086AD4 File Offset: 0x00084CD4
		public static event Action<ulong> onLobbyLeave;

		// Token: 0x06001A91 RID: 6801 RVA: 0x00013B05 File Offset: 0x00011D05
		private static void OnLobbyLeave(ulong lobbyId)
		{
			Debug.LogFormat("Left lobby {0}.", new object[]
			{
				lobbyId
			});
			Action<ulong> action = SteamworksLobbyManager.onLobbyLeave;
			if (action != null)
			{
				action(lobbyId);
			}
			SteamworksLobbyManager.OnLobbyChanged();
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x00013B36 File Offset: 0x00011D36
		private static void OnLobbyKicked(bool kickedDueToDisconnect, ulong lobbyId, ulong adminId)
		{
			Debug.LogFormat("Kicked from lobby. kickedDueToDisconnect={0} lobbyId={1} adminId={2}", new object[]
			{
				kickedDueToDisconnect,
				lobbyId,
				adminId
			});
			SteamworksLobbyManager.OnLobbyChanged();
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001A93 RID: 6803 RVA: 0x00086B08 File Offset: 0x00084D08
		// (remove) Token: 0x06001A94 RID: 6804 RVA: 0x00086B3C File Offset: 0x00084D3C
		public static event Action<ulong> onLobbyMemberDataUpdated;

		// Token: 0x06001A95 RID: 6805 RVA: 0x00013B68 File Offset: 0x00011D68
		private static void OnLobbyMemberDataUpdated(ulong memberId)
		{
			SteamworksLobbyManager.UpdateOwnsLobby();
			Action<ulong> action = SteamworksLobbyManager.onLobbyMemberDataUpdated;
			if (action == null)
			{
				return;
			}
			action(memberId);
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06001A96 RID: 6806 RVA: 0x00086B70 File Offset: 0x00084D70
		// (remove) Token: 0x06001A97 RID: 6807 RVA: 0x00086BA4 File Offset: 0x00084DA4
		public static event Action<Lobby.MemberStateChange, ulong, ulong> onLobbyStateChanged;

		// Token: 0x06001A98 RID: 6808 RVA: 0x00086BD8 File Offset: 0x00084DD8
		private static void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			Debug.LogFormat("OnLobbyStateChanged memberStateChange={0} initiatorUserId={1} affectedUserId={2}", new object[]
			{
				memberStateChange,
				initiatorUserId,
				affectedUserId
			});
			SteamworksLobbyManager.OnLobbyChanged();
			Action<Lobby.MemberStateChange, ulong, ulong> action = SteamworksLobbyManager.onLobbyStateChanged;
			if (action == null)
			{
				return;
			}
			action(memberStateChange, initiatorUserId, affectedUserId);
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x00013B7F File Offset: 0x00011D7F
		private static void OnLobbyJoinRequested(ulong lobbyId)
		{
			Debug.LogFormat("Request to join lobby {0} received. Attempting to join lobby.", new object[]
			{
				lobbyId
			});
			SteamworksLobbyManager.JoinLobby(new CSteamID(lobbyId));
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x00013BA5 File Offset: 0x00011DA5
		private static void OnUserInvitedToLobby(ulong lobbyId, ulong senderId)
		{
			Debug.LogFormat("Received invitation to lobby {0} from sender {1}.", new object[]
			{
				lobbyId,
				senderId
			});
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x00086C28 File Offset: 0x00084E28
		[ConCommand(commandName = "dump_lobbies", flags = ConVarFlags.None, helpText = "")]
		private static void DumpLobbies(ConCommandArgs args)
		{
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.MaxResults = new int?(50);
			filter.DistanceFilter = LobbyList.Filter.Distance.Worldwide;
			SteamworksLobbyManager.client.LobbyList.Refresh(filter);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x00086C60 File Offset: 0x00084E60
		private static void OnLobbiesUpdated()
		{
			if (SteamworksLobbyManager.awaitingLobbyListUpdate)
			{
				SteamworksLobbyManager.awaitingLobbyListUpdate = false;
				List<LobbyList.Lobby> lobbies = SteamworksLobbyManager.client.LobbyList.Lobbies;
				Debug.LogFormat("Found {0} lobbies.", new object[]
				{
					lobbies.Count
				});
				if (lobbies.Count > 0)
				{
					Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", lobbies[0].LobbyID), false);
				}
			}
			Action action = SteamworksLobbyManager.onLobbiesUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x00086CE8 File Offset: 0x00084EE8
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnStartup()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				ulong num;
				if (commandLineArgs[i].ToLower() == "+connect_lobby" && TextSerialization.TryParseInvariant(commandLineArgs[i + 1], out num))
				{
					SteamworksLobbyManager.pendingSteamworksLobbyId = num;
				}
			}
			RoR2Application.onStart = (Action)Delegate.Combine(RoR2Application.onStart, new Action(SteamworksLobbyManager.CheckStartupLobby));
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x00013BC9 File Offset: 0x00011DC9
		private static void CheckStartupLobby()
		{
			if (SteamworksLobbyManager.pendingSteamworksLobbyId != 0UL)
			{
				Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", SteamworksLobbyManager.pendingSteamworksLobbyId), false);
				SteamworksLobbyManager.pendingSteamworksLobbyId = 0UL;
			}
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x00013BF9 File Offset: 0x00011DF9
		public static void OnServerIPDiscovered(string address, ushort port)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.client.Lobby.IsValid)
			{
				ulong owner = SteamworksLobbyManager.client.Lobby.Owner;
				ulong steamId = SteamworksLobbyManager.client.SteamId;
			}
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x00086D54 File Offset: 0x00084F54
		public static void OnServerSteamIDDiscovered(CSteamID serverId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.client.Lobby.IsValid && SteamworksLobbyManager.client.Lobby.Owner == SteamworksLobbyManager.client.SteamId)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("server_id", serverId.ToString());
			}
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x00013C2F File Offset: 0x00011E2F
		public static void SendMigrationMessage(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client != null)
			{
				SteamworksLobbyManager.client.Lobby.SendChatMessage("");
				SteamworksLobbyManager.JoinLobby(newLobbyId);
			}
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06001AA2 RID: 6818 RVA: 0x00086DBC File Offset: 0x00084FBC
		// (remove) Token: 0x06001AA3 RID: 6819 RVA: 0x00086DF0 File Offset: 0x00084FF0
		public static event Action onLobbiesUpdated;

		// Token: 0x06001AA4 RID: 6820 RVA: 0x00086E24 File Offset: 0x00085024
		public static void SetLobbyQuickPlayQueuedIfOwner(bool quickplayQueuedState)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			lobby.CurrentLobbyData.SetData("qp", quickplayQueuedState ? "1" : "0");
			lobby.CurrentLobbyData.SetData("v", TextSerialization.ToStringInvariant(SteamworksLobbyManager.v++));
			if (!quickplayQueuedState)
			{
				lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
			}
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x00086E9C File Offset: 0x0008509C
		public static void SetLobbyQuickPlayCutoffTimeIfOwner(uint? timestamp)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			if (timestamp == null)
			{
				lobby.CurrentLobbyData.RemoveData("qp_cutoff_time");
				return;
			}
			string text = TextSerialization.ToStringInvariant(timestamp.Value);
			lobby.CurrentLobbyData.SetData("qp_cutoff_time", text);
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x00086F00 File Offset: 0x00085100
		public static void SetStartingIfOwner(bool startingState)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			Lobby.LobbyData currentLobbyData = lobby.CurrentLobbyData;
			if (currentLobbyData == null)
			{
				return;
			}
			currentLobbyData.SetData("starting", startingState ? "1" : "0");
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x00013C53 File Offset: 0x00011E53
		private static void OnLobbyOwnershipGained()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipGained;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x00013C64 File Offset: 0x00011E64
		private static void OnLobbyOwnershipLost()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipLost;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06001AA9 RID: 6825 RVA: 0x00086F4C File Offset: 0x0008514C
		// (remove) Token: 0x06001AAA RID: 6826 RVA: 0x00086F80 File Offset: 0x00085180
		public static event Action onLobbyOwnershipGained;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06001AAB RID: 6827 RVA: 0x00086FB4 File Offset: 0x000851B4
		// (remove) Token: 0x06001AAC RID: 6828 RVA: 0x00086FE8 File Offset: 0x000851E8
		public static event Action onLobbyOwnershipLost;

		// Token: 0x04001D65 RID: 7525
		public static Lobby.Type preferredLobbyType = Lobby.Type.FriendsOnly;

		// Token: 0x04001D66 RID: 7526
		public static ulong pendingSteamworksLobbyId;

		// Token: 0x04001D68 RID: 7528
		private static bool _ownsLobby;

		// Token: 0x04001D69 RID: 7529
		private static int minimumPlayerCount = 2;

		// Token: 0x04001D6B RID: 7531
		public const string mdV = "v";

		// Token: 0x04001D6C RID: 7532
		public const string mdAppId = "appid";

		// Token: 0x04001D6D RID: 7533
		public const string mdTotalMaxPlayers = "total_max_players";

		// Token: 0x04001D6E RID: 7534
		public const string mdPlayerCount = "player_count";

		// Token: 0x04001D6F RID: 7535
		public const string mdQuickplayQueued = "qp";

		// Token: 0x04001D70 RID: 7536
		public const string mdQuickplayCutoffTime = "qp_cutoff_time";

		// Token: 0x04001D71 RID: 7537
		public const string mdServerId = "server_id";

		// Token: 0x04001D72 RID: 7538
		public const string mdServerAddress = "server_address";

		// Token: 0x04001D73 RID: 7539
		public const string mdMigrationId = "migration_id";

		// Token: 0x04001D74 RID: 7540
		public const string mdStarting = "starting";

		// Token: 0x04001D75 RID: 7541
		public const string mdBuildId = "build_id";

		// Token: 0x04001D77 RID: 7543
		private static readonly List<int> playerCountsList = new List<int>();

		// Token: 0x04001D7C RID: 7548
		private static bool startingFadeSet = false;

		// Token: 0x04001D7D RID: 7549
		private static readonly MethodInfo updateLobbyDataMethodInfo = typeof(Lobby).GetMethod("UpdateLobbyData", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04001D83 RID: 7555
		private static bool awaitingLobbyListUpdate = false;

		// Token: 0x04001D85 RID: 7557
		private static int v = 0;

		// Token: 0x020004A0 RID: 1184
		public class LobbyData
		{
			// Token: 0x06001AAF RID: 6831 RVA: 0x00013C75 File Offset: 0x00011E75
			public LobbyData()
			{
			}

			// Token: 0x06001AB0 RID: 6832 RVA: 0x0008706C File Offset: 0x0008526C
			public LobbyData(Lobby.LobbyData lobbyData)
			{
				SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 CS$<>8__locals1;
				CS$<>8__locals1.lobbyDataDictionary = lobbyData.GetAllData();
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadInt|11_2("total_max_players", ref this.totalMaxPlayers, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadInt|11_2("player_count", ref this.totalPlayerCount, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadBool|11_1("qp", ref this.quickplayQueued, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadCSteamID|11_3("server_id", ref this.serverId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadAddressPortPair|11_4("server_address", ref this.serverAddressPortPair, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadCSteamID|11_3("migration_id", ref this.migrationId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadBool|11_1("starting", ref this.starting, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadString|11_0("build_id", ref this.buildId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadNullableDate|11_5("qp_cutoff_time", out this.quickplayCutoffTime, ref CS$<>8__locals1);
				this.shouldConnect = (this.serverId.isValid || this.serverAddressPortPair.isValid);
			}

			// Token: 0x04001D88 RID: 7560
			public readonly int totalMaxPlayers = 4;

			// Token: 0x04001D89 RID: 7561
			public readonly int totalPlayerCount;

			// Token: 0x04001D8A RID: 7562
			public readonly bool quickplayQueued;

			// Token: 0x04001D8B RID: 7563
			public readonly CSteamID serverId;

			// Token: 0x04001D8C RID: 7564
			public readonly AddressPortPair serverAddressPortPair;

			// Token: 0x04001D8D RID: 7565
			public readonly CSteamID migrationId;

			// Token: 0x04001D8E RID: 7566
			public readonly bool starting;

			// Token: 0x04001D8F RID: 7567
			public readonly string buildId = "0";

			// Token: 0x04001D90 RID: 7568
			public readonly DateTime? quickplayCutoffTime;

			// Token: 0x04001D91 RID: 7569
			public readonly bool shouldConnect;
		}

		// Token: 0x020004A2 RID: 1186
		private enum LobbyMessageType : byte
		{
			// Token: 0x04001D94 RID: 7572
			Chat
		}
	}
}
