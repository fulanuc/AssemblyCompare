using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Facepunch.Steamworks;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020004AC RID: 1196
	public class SteamworksLobbyManager
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06001ABA RID: 6842 RVA: 0x00013D37 File Offset: 0x00011F37
		private static Client client
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06001ABB RID: 6843 RVA: 0x00013D3E File Offset: 0x00011F3E
		// (set) Token: 0x06001ABC RID: 6844 RVA: 0x00013D45 File Offset: 0x00011F45
		public static bool isInLobby { get; private set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06001ABD RID: 6845 RVA: 0x00013D4D File Offset: 0x00011F4D
		// (set) Token: 0x06001ABE RID: 6846 RVA: 0x00013D54 File Offset: 0x00011F54
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

		// Token: 0x06001ABF RID: 6847 RVA: 0x00013D7B File Offset: 0x00011F7B
		private static void UpdateOwnsLobby()
		{
			SteamworksLobbyManager.ownsLobby = SteamworksLobbyManager.client.Lobby.IsOwner;
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x00013D91 File Offset: 0x00011F91
		public static bool hasMinimumPlayerCount
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.totalPlayerCount >= SteamworksLobbyManager.minimumPlayerCount;
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x00013DA7 File Offset: 0x00011FA7
		// (set) Token: 0x06001AC2 RID: 6850 RVA: 0x00013DAE File Offset: 0x00011FAE
		public static bool isFull { get; private set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06001AC3 RID: 6851 RVA: 0x00013DB6 File Offset: 0x00011FB6
		public static CSteamID serverId
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.serverId;
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x00013DC2 File Offset: 0x00011FC2
		// (set) Token: 0x06001AC5 RID: 6853 RVA: 0x00013DC9 File Offset: 0x00011FC9
		public static SteamworksLobbyManager.LobbyData newestLobbyData { get; private set; }

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00086A14 File Offset: 0x00084C14
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

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00086ACC File Offset: 0x00084CCC
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

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00013DD1 File Offset: 0x00011FD1
		public static int GetLobbyMemberPlayerCountByIndex(int memberIndex)
		{
			if (memberIndex >= SteamworksLobbyManager.playerCountsList.Count)
			{
				return 0;
			}
			return SteamworksLobbyManager.playerCountsList[memberIndex];
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06001AC9 RID: 6857 RVA: 0x00013DED File Offset: 0x00011FED
		// (set) Token: 0x06001ACA RID: 6858 RVA: 0x00013DF4 File Offset: 0x00011FF4
		public static int calculatedTotalPlayerCount { get; private set; }

		// Token: 0x06001ACB RID: 6859 RVA: 0x00086BF0 File Offset: 0x00084DF0
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
					int num3 = TextSerialization.TryParseInvariant(SteamworksLobbyManager.client.Lobby.GetMemberData(num2, "player_count"), out num3) ? Math.Min(Math.Max(1, num3), RoR2Application.maxLocalPlayers) : 1;
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

		// Token: 0x06001ACC RID: 6860 RVA: 0x00013DFC File Offset: 0x00011FFC
		[ConCommand(commandName = "steam_lobby_update_player_count", flags = ConVarFlags.None, helpText = "Forces a refresh of the steam lobby player count.")]
		private static void CCSteamLobbyUpdatePlayerCount(ConCommandArgs args)
		{
			SteamworksLobbyManager.UpdatePlayerCount();
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06001ACD RID: 6861 RVA: 0x00086DB4 File Offset: 0x00084FB4
		// (remove) Token: 0x06001ACE RID: 6862 RVA: 0x00086DE8 File Offset: 0x00084FE8
		public static event Action onPlayerCountUpdated;

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06001ACF RID: 6863 RVA: 0x00086E1C File Offset: 0x0008501C
		// (remove) Token: 0x06001AD0 RID: 6864 RVA: 0x00086E50 File Offset: 0x00085050
		public static event Action onLobbyChanged;

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00086E84 File Offset: 0x00085084
		private static void OnLobbyChanged()
		{
			SteamworksLobbyManager.isInLobby = SteamworksLobbyManager.client.Lobby.IsValid;
			SteamworksLobbyManager.UpdateOwnsLobby();
			if (SteamworksLobbyManager.client.Lobby.CurrentLobbyData != null)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("total_max_players", TextSerialization.ToStringInvariant(RoR2Application.maxPlayers));
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

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00013E03 File Offset: 0x00012003
		public static void CreateLobby()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.client.Lobby.Create(SteamworksLobbyManager.preferredLobbyType, RoR2Application.maxPlayers);
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x00013E35 File Offset: 0x00012035
		// (set) Token: 0x06001AD4 RID: 6868 RVA: 0x00013E3C File Offset: 0x0001203C
		public static bool awaitingJoin { get; private set; }

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00013E44 File Offset: 0x00012044
		public static void JoinLobby(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.awaitingJoin = true;
			SteamworksLobbyManager.client.Lobby.Join(newLobbyId.value);
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00013E69 File Offset: 0x00012069
		public static void LeaveLobby()
		{
			Client client = SteamworksLobbyManager.client;
			if (client == null)
			{
				return;
			}
			client.Lobby.Leave();
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00086F18 File Offset: 0x00085118
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

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00013E7F File Offset: 0x0001207F
		private static void StaticUpdate()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.UpdateOwnsLobby();
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00013E8E File Offset: 0x0001208E
		[ConCommand(commandName = "steam_lobby_create")]
		private static void CCSteamLobbyCreate(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x00013E9A File Offset: 0x0001209A
		[ConCommand(commandName = "steam_lobby_create_if_none")]
		private static void CCSteamLobbyCreateIfNone(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!SteamworksLobbyManager.client.Lobby.IsValid)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00013EB7 File Offset: 0x000120B7
		[ConCommand(commandName = "steam_lobby_find")]
		private static void CCSteamLobbyFind(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.awaitingLobbyListUpdate = true;
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x00086F7C File Offset: 0x0008517C
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

		// Token: 0x06001ADD RID: 6877 RVA: 0x00013EC4 File Offset: 0x000120C4
		[ConCommand(commandName = "steam_lobby_leave")]
		private static void CCSteamLobbyLeave(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.LeaveLobby();
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x00086FD4 File Offset: 0x000851D4
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

		// Token: 0x06001ADF RID: 6879 RVA: 0x0008703C File Offset: 0x0008523C
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

		// Token: 0x06001AE0 RID: 6880 RVA: 0x00013ED0 File Offset: 0x000120D0
		[ConCommand(commandName = "steam_lobby_open_invite_overlay", flags = ConVarFlags.None, helpText = "Opens the steam overlay to the friend invite dialog.")]
		private static void CCSteamLobbyOpenInviteOverlay(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.client.Overlay.OpenInviteDialog(SteamworksLobbyManager.client.Lobby.CurrentLobby);
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x00013EF5 File Offset: 0x000120F5
		[ConCommand(commandName = "steam_lobby_copy_to_clipboard", flags = ConVarFlags.None, helpText = "Copies the currently active lobby to the clipboard if applicable.")]
		private static void CCSteamLobbyCopyToClipboard(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			GUIUtility.systemCopyBuffer = TextSerialization.ToStringInvariant(SteamworksLobbyManager.client.Lobby.CurrentLobby);
			Chat.AddMessage(Language.GetString("STEAM_COPY_LOBBY_TO_CLIPBOARD_MESSAGE"));
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x00087088 File Offset: 0x00085288
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

		// Token: 0x06001AE3 RID: 6883 RVA: 0x00013F24 File Offset: 0x00012124
		[ConCommand(commandName = "steam_id", flags = ConVarFlags.None, helpText = "Displays your steam id.")]
		private static void CCSteamId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Steam id = {0}", new object[]
			{
				SteamworksLobbyManager.client.SteamId
			});
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x00013F4D File Offset: 0x0001214D
		[ConCommand(commandName = "steam_lobby_id", flags = ConVarFlags.None, helpText = "Displays the steam id of the current lobby.")]
		private static void CCSteamLobbyId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Lobby id = {0}", new object[]
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobby
			});
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x00087134 File Offset: 0x00085334
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

		// Token: 0x06001AE6 RID: 6886 RVA: 0x000871DC File Offset: 0x000853DC
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

		// Token: 0x06001AE7 RID: 6887 RVA: 0x000872A4 File Offset: 0x000854A4
		public static void ForceLobbyDataUpdate()
		{
			Client client = SteamworksLobbyManager.client;
			Lobby lobby = (client != null) ? client.Lobby : null;
			if (lobby != null)
			{
				SteamworksLobbyManager.updateLobbyDataMethodInfo.Invoke(lobby, Array.Empty<object>());
			}
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x000872D8 File Offset: 0x000854D8
		private static void OnChatMessageReceived(ulong senderId, byte[] buffer, int byteCount)
		{
			NetworkReader networkReader = new NetworkReader(buffer);
			if (byteCount >= 1 && networkReader.ReadByte() == 0)
			{
				Chat.AddMessage(string.Format("{0}: {1}", Client.Instance.Friends.Get(senderId), networkReader.ReadString()));
			}
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x00013F7B File Offset: 0x0001217B
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

		// Token: 0x06001AEA RID: 6890 RVA: 0x00013FA0 File Offset: 0x000121A0
		public static void StartMigrateLobby(CSteamID newLobbyId)
		{
			SteamworksLobbyManager.client.Lobby.Joinable = false;
			SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("migration_id", TextSerialization.ToStringInvariant(newLobbyId.value));
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x00013FD7 File Offset: 0x000121D7
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

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001AEC RID: 6892 RVA: 0x00087320 File Offset: 0x00085520
		// (remove) Token: 0x06001AED RID: 6893 RVA: 0x00087354 File Offset: 0x00085554
		public static event Action onLobbyDataUpdated;

		// Token: 0x06001AEE RID: 6894 RVA: 0x00087388 File Offset: 0x00085588
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
						GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(SteamworksLobbyManager.newestLobbyData.serverId);
					}
				}
				else
				{
					GameNetworkManager.singleton.desiredHost = GameNetworkManager.HostDescription.none;
				}
			}
			Action action = SteamworksLobbyManager.onLobbyDataUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06001AEF RID: 6895 RVA: 0x00087454 File Offset: 0x00085654
		// (remove) Token: 0x06001AF0 RID: 6896 RVA: 0x00087488 File Offset: 0x00085688
		public static event Action<bool> onLobbyJoined;

		// Token: 0x06001AF1 RID: 6897 RVA: 0x000874BC File Offset: 0x000856BC
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

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06001AF2 RID: 6898 RVA: 0x00087608 File Offset: 0x00085808
		// (remove) Token: 0x06001AF3 RID: 6899 RVA: 0x0008763C File Offset: 0x0008583C
		public static event Action<ulong> onLobbyLeave;

		// Token: 0x06001AF4 RID: 6900 RVA: 0x00014013 File Offset: 0x00012213
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

		// Token: 0x06001AF5 RID: 6901 RVA: 0x00014044 File Offset: 0x00012244
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

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06001AF6 RID: 6902 RVA: 0x00087670 File Offset: 0x00085870
		// (remove) Token: 0x06001AF7 RID: 6903 RVA: 0x000876A4 File Offset: 0x000858A4
		public static event Action<ulong> onLobbyMemberDataUpdated;

		// Token: 0x06001AF8 RID: 6904 RVA: 0x00014076 File Offset: 0x00012276
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

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06001AF9 RID: 6905 RVA: 0x000876D8 File Offset: 0x000858D8
		// (remove) Token: 0x06001AFA RID: 6906 RVA: 0x0008770C File Offset: 0x0008590C
		public static event Action<Lobby.MemberStateChange, ulong, ulong> onLobbyStateChanged;

		// Token: 0x06001AFB RID: 6907 RVA: 0x00087740 File Offset: 0x00085940
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

		// Token: 0x06001AFC RID: 6908 RVA: 0x0001408D File Offset: 0x0001228D
		private static void OnLobbyJoinRequested(ulong lobbyId)
		{
			Debug.LogFormat("Request to join lobby {0} received. Attempting to join lobby.", new object[]
			{
				lobbyId
			});
			SteamworksLobbyManager.JoinLobby(new CSteamID(lobbyId));
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x000140B3 File Offset: 0x000122B3
		private static void OnUserInvitedToLobby(ulong lobbyId, ulong senderId)
		{
			Debug.LogFormat("Received invitation to lobby {0} from sender {1}.", new object[]
			{
				lobbyId,
				senderId
			});
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x00087790 File Offset: 0x00085990
		[ConCommand(commandName = "dump_lobbies", flags = ConVarFlags.None, helpText = "")]
		private static void DumpLobbies(ConCommandArgs args)
		{
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.MaxResults = new int?(50);
			filter.DistanceFilter = LobbyList.Filter.Distance.Worldwide;
			SteamworksLobbyManager.client.LobbyList.Refresh(filter);
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x000877C8 File Offset: 0x000859C8
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

		// Token: 0x06001B00 RID: 6912 RVA: 0x00087850 File Offset: 0x00085A50
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnStartup()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				ulong num;
				if (commandLineArgs[i].ToLower(CultureInfo.InvariantCulture) == "+connect_lobby" && TextSerialization.TryParseInvariant(commandLineArgs[i + 1], out num))
				{
					SteamworksLobbyManager.pendingSteamworksLobbyId = num;
				}
			}
			RoR2Application.onStart = (Action)Delegate.Combine(RoR2Application.onStart, new Action(SteamworksLobbyManager.CheckStartupLobby));
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x000140D7 File Offset: 0x000122D7
		private static void CheckStartupLobby()
		{
			if (SteamworksLobbyManager.pendingSteamworksLobbyId != 0UL)
			{
				Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", SteamworksLobbyManager.pendingSteamworksLobbyId), false);
				SteamworksLobbyManager.pendingSteamworksLobbyId = 0UL;
			}
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x00014107 File Offset: 0x00012307
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

		// Token: 0x06001B03 RID: 6915 RVA: 0x000878C0 File Offset: 0x00085AC0
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

		// Token: 0x06001B04 RID: 6916 RVA: 0x0001413D File Offset: 0x0001233D
		public static void SendMigrationMessage(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client != null)
			{
				SteamworksLobbyManager.client.Lobby.SendChatMessage("");
				SteamworksLobbyManager.JoinLobby(newLobbyId);
			}
		}

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06001B05 RID: 6917 RVA: 0x00087928 File Offset: 0x00085B28
		// (remove) Token: 0x06001B06 RID: 6918 RVA: 0x0008795C File Offset: 0x00085B5C
		public static event Action onLobbiesUpdated;

		// Token: 0x06001B07 RID: 6919 RVA: 0x00087990 File Offset: 0x00085B90
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

		// Token: 0x06001B08 RID: 6920 RVA: 0x00087A08 File Offset: 0x00085C08
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

		// Token: 0x06001B09 RID: 6921 RVA: 0x00087A6C File Offset: 0x00085C6C
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

		// Token: 0x06001B0A RID: 6922 RVA: 0x00014161 File Offset: 0x00012361
		private static void OnLobbyOwnershipGained()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipGained;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x00014172 File Offset: 0x00012372
		private static void OnLobbyOwnershipLost()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipLost;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x06001B0C RID: 6924 RVA: 0x00087AB8 File Offset: 0x00085CB8
		// (remove) Token: 0x06001B0D RID: 6925 RVA: 0x00087AEC File Offset: 0x00085CEC
		public static event Action onLobbyOwnershipGained;

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x06001B0E RID: 6926 RVA: 0x00087B20 File Offset: 0x00085D20
		// (remove) Token: 0x06001B0F RID: 6927 RVA: 0x00087B54 File Offset: 0x00085D54
		public static event Action onLobbyOwnershipLost;

		// Token: 0x04001D9E RID: 7582
		public static Lobby.Type preferredLobbyType = Lobby.Type.FriendsOnly;

		// Token: 0x04001D9F RID: 7583
		public static ulong pendingSteamworksLobbyId;

		// Token: 0x04001DA1 RID: 7585
		private static bool _ownsLobby;

		// Token: 0x04001DA2 RID: 7586
		private static int minimumPlayerCount = 2;

		// Token: 0x04001DA4 RID: 7588
		public const string mdV = "v";

		// Token: 0x04001DA5 RID: 7589
		public const string mdAppId = "appid";

		// Token: 0x04001DA6 RID: 7590
		public const string mdTotalMaxPlayers = "total_max_players";

		// Token: 0x04001DA7 RID: 7591
		public const string mdPlayerCount = "player_count";

		// Token: 0x04001DA8 RID: 7592
		public const string mdQuickplayQueued = "qp";

		// Token: 0x04001DA9 RID: 7593
		public const string mdQuickplayCutoffTime = "qp_cutoff_time";

		// Token: 0x04001DAA RID: 7594
		public const string mdServerId = "server_id";

		// Token: 0x04001DAB RID: 7595
		public const string mdServerAddress = "server_address";

		// Token: 0x04001DAC RID: 7596
		public const string mdMigrationId = "migration_id";

		// Token: 0x04001DAD RID: 7597
		public const string mdStarting = "starting";

		// Token: 0x04001DAE RID: 7598
		public const string mdBuildId = "build_id";

		// Token: 0x04001DB0 RID: 7600
		private static readonly List<int> playerCountsList = new List<int>();

		// Token: 0x04001DB5 RID: 7605
		private static bool startingFadeSet = false;

		// Token: 0x04001DB6 RID: 7606
		private static readonly MethodInfo updateLobbyDataMethodInfo = typeof(Lobby).GetMethod("UpdateLobbyData", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04001DBC RID: 7612
		private static bool awaitingLobbyListUpdate = false;

		// Token: 0x04001DBE RID: 7614
		private static int v = 0;

		// Token: 0x020004AD RID: 1197
		public class LobbyData
		{
			// Token: 0x06001B12 RID: 6930 RVA: 0x00014183 File Offset: 0x00012383
			public LobbyData()
			{
			}

			// Token: 0x06001B13 RID: 6931 RVA: 0x00087BD8 File Offset: 0x00085DD8
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

			// Token: 0x04001DC1 RID: 7617
			public readonly int totalMaxPlayers = RoR2Application.maxPlayers;

			// Token: 0x04001DC2 RID: 7618
			public readonly int totalPlayerCount;

			// Token: 0x04001DC3 RID: 7619
			public readonly bool quickplayQueued;

			// Token: 0x04001DC4 RID: 7620
			public readonly CSteamID serverId;

			// Token: 0x04001DC5 RID: 7621
			public readonly AddressPortPair serverAddressPortPair;

			// Token: 0x04001DC6 RID: 7622
			public readonly CSteamID migrationId;

			// Token: 0x04001DC7 RID: 7623
			public readonly bool starting;

			// Token: 0x04001DC8 RID: 7624
			public readonly string buildId = "0";

			// Token: 0x04001DC9 RID: 7625
			public readonly DateTime? quickplayCutoffTime;

			// Token: 0x04001DCA RID: 7626
			public readonly bool shouldConnect;
		}

		// Token: 0x020004AF RID: 1199
		private enum LobbyMessageType : byte
		{
			// Token: 0x04001DCD RID: 7629
			Chat
		}
	}
}
