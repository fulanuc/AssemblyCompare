using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using Facepunch.Steamworks;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2.Networking
{
	// Token: 0x02000576 RID: 1398
	public class GameNetworkManager : NetworkManager
	{
		// Token: 0x06001F36 RID: 7990 RVA: 0x00099020 File Offset: 0x00097220
		static GameNetworkManager()
		{
			GameNetworkManager.loadingSceneAsyncFieldInfo = typeof(NetworkManager).GetField("s_LoadingSceneAsync", BindingFlags.Static | BindingFlags.NonPublic);
			if (GameNetworkManager.loadingSceneAsyncFieldInfo == null)
			{
				Debug.LogError("NetworkManager.s_LoadingSceneAsync field could not be found! Make sure to provide a proper implementation for this version of Unity.");
			}
			GameNetworkManager.StaticInit();
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x000990CC File Offset: 0x000972CC
		private static void StaticInit()
		{
			GameNetworkManager.onStartServerGlobal += delegate()
			{
				if (!NetworkServer.dontListen)
				{
					GameNetworkManager.singleton.StartSteamworksServer();
				}
			};
			GameNetworkManager.onStopServerGlobal += delegate()
			{
				GameNetworkManager.singleton.StopSteamworksServer();
			};
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001F38 RID: 7992 RVA: 0x00099124 File Offset: 0x00097324
		private static bool isLoadingScene
		{
			get
			{
				AsyncOperation asyncOperation = (AsyncOperation)GameNetworkManager.loadingSceneAsyncFieldInfo.GetValue(null);
				return asyncOperation != null && !asyncOperation.isDone;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001F39 RID: 7993 RVA: 0x00016D77 File Offset: 0x00014F77
		public new static GameNetworkManager singleton
		{
			get
			{
				return (GameNetworkManager)NetworkManager.singleton;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001F3A RID: 7994 RVA: 0x00016D83 File Offset: 0x00014F83
		// (set) Token: 0x06001F3B RID: 7995 RVA: 0x00016D8B File Offset: 0x00014F8B
		public float unpredictedServerFixedTime { get; private set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001F3C RID: 7996 RVA: 0x00016D94 File Offset: 0x00014F94
		public float serverFixedTime
		{
			get
			{
				return this.unpredictedServerFixedTime + this.filteredClientRTT;
			}
		}

		// Token: 0x06001F3D RID: 7997 RVA: 0x00099150 File Offset: 0x00097350
		private static NetworkUser[] GetConnectionNetworkUsers(NetworkConnection conn)
		{
			List<PlayerController> playerControllers = conn.playerControllers;
			NetworkUser[] array = new NetworkUser[playerControllers.Count];
			for (int i = 0; i < playerControllers.Count; i++)
			{
				array[i] = playerControllers[i].gameObject.GetComponent<NetworkUser>();
			}
			return array;
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x00016DA3 File Offset: 0x00014FA3
		private void Ping(NetworkConnection conn, int channelId)
		{
			conn.SendByChannel(65, new GameNetworkManager.PingMessage
			{
				timeStampMs = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds
			}, channelId);
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x00099198 File Offset: 0x00097398
		protected void Start()
		{
			foreach (QosType value in base.channels)
			{
				base.connectionConfig.AddChannel(value);
			}
			base.connectionConfig.PacketSize = 1200;
			Client instance = Client.Instance;
			if (instance != null)
			{
				instance.Networking.OnP2PData = new Networking.OnRecievedP2PData(this.OnP2PData);
				for (int i = 0; i < base.connectionConfig.ChannelCount; i++)
				{
					instance.Networking.SetListenChannel(i, true);
				}
				instance.Networking.OnIncomingConnection = new Func<ulong, bool>(this.OnIncomingP2PConnection);
				instance.Networking.OnConnectionFailed = new Action<ulong, Networking.SessionError>(this.OnClientP2PConnectionFailed);
			}
			Action action = GameNetworkManager.onStartGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06001F40 RID: 8000 RVA: 0x00099284 File Offset: 0x00097484
		// (remove) Token: 0x06001F41 RID: 8001 RVA: 0x000992B8 File Offset: 0x000974B8
		public static event Action onStartGlobal;

		// Token: 0x06001F42 RID: 8002 RVA: 0x00016DCA File Offset: 0x00014FCA
		private void OnDestroy()
		{
			typeof(NetworkManager).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x000992EC File Offset: 0x000974EC
		protected void FixedUpdate()
		{
			if (NetworkServer.active || NetworkClient.active)
			{
				this.unpredictedServerFixedTime += Time.fixedDeltaTime;
			}
			this.FixedUpdateServer();
			this.FixedUpdateClient();
			this.debugServerTime = this.unpredictedServerFixedTime;
			this.debugRTT = this.clientRTT;
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x00016DEA File Offset: 0x00014FEA
		protected void Update()
		{
			this.UpdateServer();
			this.UpdateClient();
		}

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06001F45 RID: 8005 RVA: 0x00099340 File Offset: 0x00097540
		// (remove) Token: 0x06001F46 RID: 8006 RVA: 0x00099374 File Offset: 0x00097574
		public static event Action<NetworkClient> onStartClientGlobal;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06001F47 RID: 8007 RVA: 0x000993A8 File Offset: 0x000975A8
		// (remove) Token: 0x06001F48 RID: 8008 RVA: 0x000993DC File Offset: 0x000975DC
		public static event Action onStopClientGlobal;

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06001F49 RID: 8009 RVA: 0x00099410 File Offset: 0x00097610
		// (remove) Token: 0x06001F4A RID: 8010 RVA: 0x00099444 File Offset: 0x00097644
		public static event Action<NetworkConnection> onClientConnectGlobal;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06001F4B RID: 8011 RVA: 0x00099478 File Offset: 0x00097678
		// (remove) Token: 0x06001F4C RID: 8012 RVA: 0x000994AC File Offset: 0x000976AC
		public static event Action<NetworkConnection> onClientDisconnectGlobal;

		// Token: 0x06001F4D RID: 8013 RVA: 0x000994E0 File Offset: 0x000976E0
		public override void OnStartClient(NetworkClient newClient)
		{
			base.OnStartClient(newClient);
			foreach (string str in GameNetworkManager.spawnableFolders)
			{
				GameObject[] array2 = Resources.LoadAll<GameObject>("Prefabs/" + str + "/");
				for (int j = 0; j < array2.Length; j++)
				{
					ClientScene.RegisterPrefab(array2[j]);
				}
			}
			ClientScene.RegisterPrefab(Resources.Load<GameObject>("Prefabs/NetworkSession"));
			ClientScene.RegisterPrefab(Resources.Load<GameObject>("Prefabs/Stage"));
			NetworkMessageHandlerAttribute.RegisterClientMessages(newClient);
			Action<NetworkClient> action = GameNetworkManager.onStartClientGlobal;
			if (action == null)
			{
				return;
			}
			action(newClient);
		}

		// Token: 0x06001F4E RID: 8014 RVA: 0x00016DF8 File Offset: 0x00014FF8
		public override void OnStopClient()
		{
			Action action = GameNetworkManager.onStopClientGlobal;
			if (action != null)
			{
				action();
			}
			base.OnStopClient();
		}

		// Token: 0x06001F4F RID: 8015 RVA: 0x00016E10 File Offset: 0x00015010
		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			this.clientRTT = 0f;
			this.filteredClientRTT = 0f;
			this.ClientSetPlayers(conn);
			Action<NetworkConnection> action = GameNetworkManager.onClientConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x00099574 File Offset: 0x00097774
		public override void OnClientDisconnect(NetworkConnection conn)
		{
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				Debug.LogFormat("Closing connection with steamId {0}", new object[]
				{
					steamNetworkConnection.steamId
				});
			}
			base.OnClientDisconnect(conn);
			Action<NetworkConnection> action = GameNetworkManager.onClientDisconnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F51 RID: 8017 RVA: 0x000995C0 File Offset: 0x000977C0
		public void ClientAddPlayer(short playerControllerId, NetworkConnection connection = null)
		{
			foreach (PlayerController playerController in ClientScene.localPlayers)
			{
				if (playerController.playerControllerId == playerControllerId && playerController.IsValid && playerController.gameObject)
				{
					Debug.LogFormat("Player {0} already added, aborting.", new object[]
					{
						playerControllerId
					});
					return;
				}
			}
			Debug.LogFormat("Adding local player controller {0} on connection {1}", new object[]
			{
				playerControllerId,
				connection
			});
			GameNetworkManager.AddPlayerMessage extraMessage;
			if (RoR2Application.instance.steamworksClient != null)
			{
				extraMessage = new GameNetworkManager.AddPlayerMessage
				{
					steamId = RoR2Application.instance.steamworksClient.SteamId,
					steamAuthTicketData = RoR2Application.instance.steamworksAuthTicket.Data
				};
			}
			else
			{
				extraMessage = new GameNetworkManager.AddPlayerMessage
				{
					steamId = 0UL,
					steamAuthTicketData = Array.Empty<byte>()
				};
			}
			ClientScene.AddPlayer(connection, playerControllerId, extraMessage);
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x000996C4 File Offset: 0x000978C4
		private void UpdateClient()
		{
			NetworkClient client = this.client;
			if (((client != null) ? client.connection : null) is SteamNetworkConnection)
			{
				Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
				if (Client.Instance.Networking.GetP2PSessionState(((SteamNetworkConnection)this.client.connection).steamId.value, ref p2PSessionState) && p2PSessionState.Connecting == 0 && p2PSessionState.ConnectionActive == 0)
				{
					base.StopClient();
				}
			}
			bool flag = (this.client != null && !ClientScene.ready) || GameNetworkManager.isLoadingScene;
			if (GameNetworkManager.wasFading != flag)
			{
				if (flag)
				{
					FadeToBlackManager.fadeCount++;
				}
				else
				{
					FadeToBlackManager.fadeCount--;
				}
				GameNetworkManager.wasFading = flag;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001F53 RID: 8019 RVA: 0x00016E46 File Offset: 0x00015046
		// (set) Token: 0x06001F54 RID: 8020 RVA: 0x00016E4E File Offset: 0x0001504E
		public float clientRTT { get; private set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001F55 RID: 8021 RVA: 0x00016E57 File Offset: 0x00015057
		// (set) Token: 0x06001F56 RID: 8022 RVA: 0x00016E5F File Offset: 0x0001505F
		public float filteredClientRTT { get; private set; }

		// Token: 0x06001F57 RID: 8023 RVA: 0x00099778 File Offset: 0x00097978
		private void FixedUpdateClient()
		{
			if (!NetworkClient.active || this.client == null)
			{
				return;
			}
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null)
			{
				this.clientRTT = steamNetworkConnection.rtt * 0.001f;
			}
			else
			{
				this.clientRTT = (float)this.client.GetRTT() * 0.001f;
			}
			if (this.filteredClientRTT == 0f)
			{
				this.filteredClientRTT = this.clientRTT;
			}
			else
			{
				this.filteredClientRTT = Mathf.SmoothDamp(this.filteredClientRTT, this.clientRTT, ref this.rttVelocity, this.filteredRTTSmoothDuration, 100f, Time.fixedDeltaTime);
			}
			int i = 0;
			int count = NetworkClient.allClients.Count;
			while (i < count)
			{
				NetworkConnection connection = NetworkClient.allClients[i].connection;
				if (!Util.ConnectionIsLocal(connection))
				{
					this.Ping(connection, QosChannelIndex.ping.intVal);
				}
				i++;
			}
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x00016E68 File Offset: 0x00015068
		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			base.autoCreatePlayer = false;
			base.OnClientSceneChanged(conn);
			this.ClientSetPlayers(conn);
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x00099860 File Offset: 0x00097A60
		private void ClientSetPlayers(NetworkConnection conn)
		{
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			for (int i = 0; i < readOnlyLocalUsersList.Count; i++)
			{
				this.ClientAddPlayer((short)readOnlyLocalUsersList[i].id, conn);
			}
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x00016E7F File Offset: 0x0001507F
		public void RequestJoinSteamServer(CSteamID serverId)
		{
			if (!base.GetComponent<GameNetworkManager.SteamClientJoinAttempt>())
			{
				GameNetworkManager.SteamClientJoinAttempt steamClientJoinAttempt = base.gameObject.AddComponent<GameNetworkManager.SteamClientJoinAttempt>();
				steamClientJoinAttempt.networkManager = this;
				steamClientJoinAttempt.serverId = serverId;
			}
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x00099898 File Offset: 0x00097A98
		private void StartClientSteam(CSteamID serverId)
		{
			if (!NetworkServer.active)
			{
				NetworkManager.networkSceneName = "";
			}
			string text = "";
			if (this.isNetworkActive)
			{
				text += "isNetworkActive ";
			}
			if (NetworkClient.active)
			{
				text += "NetworkClient.active ";
			}
			if (NetworkServer.active)
			{
				text += "NetworkClient.active ";
			}
			if (GameNetworkManager.isLoadingScene)
			{
				text += "isLoadingScene ";
			}
			if (text != "")
			{
				Debug.Log(text);
				RoR2Application.onNextUpdate += delegate()
				{
				};
			}
			SteamNetworkClient steamNetworkClient = new SteamNetworkClient(new SteamNetworkConnection(serverId));
			steamNetworkClient.Configure(base.connectionConfig, 1);
			base.UseExternalClient(steamNetworkClient);
			steamNetworkClient.Connect();
			Debug.LogFormat("Initiating connection to server {0}...", new object[]
			{
				serverId.value
			});
			if (!Client.Instance.Networking.SendP2PPacket(serverId.value, null, 0, Networking.SendType.Reliable, 0))
			{
				Debug.LogFormat("Failed to send connection request to server {0}.", new object[]
				{
					serverId.value
				});
			}
		}

		// Token: 0x06001F5C RID: 8028 RVA: 0x000999C0 File Offset: 0x00097BC0
		public bool IsConnectedToServer(CSteamID serverSteamId)
		{
			if (this.client == null || !this.client.connection.isConnected || Client.Instance == null)
			{
				return false;
			}
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null)
			{
				return steamNetworkConnection.steamId == serverSteamId;
			}
			return this.client.connection.address == "localServer" && serverSteamId.value == Client.Instance.SteamId;
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x00099A44 File Offset: 0x00097C44
		[ConCommand(commandName = "server_send_test_to_clients", flags = ConVarFlags.Cheat, helpText = "Sends a test packet from this server to all clients.")]
		private static void CCServerSendTestToClients(ConCommandArgs args)
		{
			if (NetworkServer.active)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(64);
				networkWriter.Write(-123456);
				networkWriter.FinishMessage();
				NetworkServer.SendWriterToReady(null, networkWriter, QosChannelIndex.defaultReliable.intVal);
			}
		}

		// Token: 0x06001F5E RID: 8030 RVA: 0x00099A88 File Offset: 0x00097C88
		[ConCommand(commandName = "client_send_test_to_server", flags = ConVarFlags.Cheat, helpText = "Sends a test packet from this client to the connected server.")]
		private static void CCClientSendTestToServer(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(64);
				networkWriter.Write(-123456);
				networkWriter.FinishMessage();
				GameNetworkManager.singleton.client.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			}
		}

		// Token: 0x06001F5F RID: 8031 RVA: 0x00099AE8 File Offset: 0x00097CE8
		[ConCommand(commandName = "client_set_players", flags = ConVarFlags.None, helpText = "Adds network players for all local players. Debug only.")]
		private static void CCClientSetPlayers(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				GameNetworkManager.singleton.ClientSetPlayers(GameNetworkManager.singleton.client.connection);
			}
		}

		// Token: 0x06001F60 RID: 8032 RVA: 0x00099B38 File Offset: 0x00097D38
		[ConCommand(commandName = "ping", flags = ConVarFlags.None, helpText = "Prints the current round trip time from this client to the server and back.")]
		private static void CCPing(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				uint rtt;
				if (GameNetworkManager.singleton.client.connection is SteamNetworkConnection)
				{
					rtt = ((SteamNetworkConnection)GameNetworkManager.singleton.client.connection).rtt;
				}
				else
				{
					rtt = (uint)GameNetworkManager.singleton.client.GetRTT();
				}
				Debug.LogFormat("rtt={0}ms", new object[]
				{
					rtt
				});
			}
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x00099BCC File Offset: 0x00097DCC
		[ConCommand(commandName = "set_scene", flags = ConVarFlags.None, helpText = "Changes to the named scene.")]
		private static void CCSetScene(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			int boolValue = RoR2Application.cvCheats.boolValue ? 1 : 0;
			bool flag = !GameNetworkManager.singleton || GameNetworkManager.singleton.isNetworkActive;
			if (boolValue == 0 && flag && Array.IndexOf<string>(GameNetworkManager.sceneWhiteList, args[0]) == -1)
			{
				return;
			}
			if (!GameNetworkManager.singleton)
			{
				throw new ConCommandException("set_scene failed: GameNetworkManager is not available.");
			}
			if (NetworkServer.active)
			{
				Debug.LogFormat("Setting server scene to {0}", new object[]
				{
					args[0]
				});
				GameNetworkManager.singleton.ServerChangeScene(args[0]);
				return;
			}
			if (!NetworkClient.active)
			{
				Debug.LogFormat("Setting offline scene to {0}", new object[]
				{
					args[0]
				});
				SceneManager.LoadScene(args[0], LoadSceneMode.Single);
				return;
			}
			throw new ConCommandException("Cannot change scene while connected to a remote server.");
		}

		// Token: 0x06001F62 RID: 8034 RVA: 0x00099CAC File Offset: 0x00097EAC
		[ConCommand(commandName = "scene_list", flags = ConVarFlags.None, helpText = "Prints a list of all available scene names.")]
		private static void CCSceneList(ConCommandArgs args)
		{
			string[] array = new string[SceneManager.sceneCountInBuildSettings];
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				array[i] = string.Format("[{0}]={1}", i, SceneUtility.GetScenePathByBuildIndex(i));
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x00099D00 File Offset: 0x00097F00
		[ConCommand(commandName = "dump_network_ids", flags = ConVarFlags.None, helpText = "Lists the network ids of all currently networked game objects.")]
		private static void CCDumpNetworkIDs(ConCommandArgs args)
		{
			NetworkIdentity[] array = UnityEngine.Object.FindObjectsOfType<NetworkIdentity>();
			for (int i = 0; i < array.Length; i++)
			{
				Debug.LogFormat("{0}={1}", new object[]
				{
					array[i].netId.Value,
					array[i].gameObject.name
				});
			}
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x00099D5C File Offset: 0x00097F5C
		[ConCommand(commandName = "disconnect", flags = ConVarFlags.None, helpText = "Disconnect from a server or shut down the current server.")]
		private static void CCDisconnect(ConCommandArgs args)
		{
			if (!GameNetworkManager.singleton || !GameNetworkManager.singleton.isNetworkActive)
			{
				return;
			}
			Debug.Log("Network shutting down...");
			if (NetworkServer.active && GameNetworkManager.singleton.client != null)
			{
				GameNetworkManager.singleton.StopHost();
			}
			else if (NetworkServer.active)
			{
				GameNetworkManager.singleton.StopServer();
			}
			else if (GameNetworkManager.singleton.client != null)
			{
				GameNetworkManager.singleton.StopClient();
			}
			Debug.Log("Network shutdown complete.");
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x00099DE0 File Offset: 0x00097FE0
		[ConCommand(commandName = "connect", flags = ConVarFlags.None, helpText = "Connect to a server.")]
		private static void CCConnect(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			string[] array = args[0].Split(new char[]
			{
				':'
			});
			string text = array[0];
			ushort num = 7777;
			if (array.Length > 1)
			{
				TextSerialization.TryParseInvariant(array[1], out num);
			}
			Console.instance.SubmitCmd(null, "disconnect", false);
			Debug.LogFormat("Attempting connection. ip={0} port={1}", new object[]
			{
				text,
				num
			});
			GameNetworkManager.singleton.networkAddress = text;
			GameNetworkManager.singleton.networkPort = (int)num;
			GameNetworkManager.singleton.StartClient();
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x00099E88 File Offset: 0x00098088
		[ConCommand(commandName = "connect_steamworks_p2p", flags = ConVarFlags.None, helpText = "Connect to a server using Steamworks P2P. Argument is the 64-bit Steam ID of the server to connect to.")]
		private static void CCConnectSteamworksP2P(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			if (GameNetworkManager.singleton.isNetworkActive)
			{
				Console.instance.SubmitCmd(null, "disconnect", false);
			}
			CSteamID csteamID;
			if (!CSteamID.TryParse(args[0], out csteamID))
			{
				throw new ConCommandException("Could not parse server id.");
			}
			if (Client.Instance.Lobby.IsValid && !SteamworksLobbyManager.ownsLobby && csteamID != SteamworksLobbyManager.serverId)
			{
				Debug.LogFormat("Cannot connect to server {0}: Server is not the one specified by the current steam lobby.", new object[]
				{
					csteamID
				});
				return;
			}
			if (Client.Instance.SteamId == csteamID.value)
			{
				return;
			}
			GameNetworkManager.singleton.RequestJoinSteamServer(csteamID);
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x00099F48 File Offset: 0x00098148
		[ConCommand(commandName = "host", flags = ConVarFlags.None, helpText = "Host a server. First argument is whether or not to listen for incoming connections.")]
		private static void CCHost(ConCommandArgs args)
		{
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			args.CheckArgumentCount(1);
			if (SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.ownsLobby)
			{
				return;
			}
			bool flag = false;
			if (NetworkServer.active)
			{
				Debug.Log("Server already running.");
				flag = true;
			}
			else if (GameNetworkManager.singleton.client != null)
			{
				GameNetworkManager.singleton.StopClient();
			}
			if (!flag)
			{
				int num;
				if (TextSerialization.TryParseInvariant(args[0], out num))
				{
					NetworkServer.dontListen = (num == 0);
				}
				GameNetworkManager.singleton.StartHost();
			}
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x00099FD0 File Offset: 0x000981D0
		[ConCommand(commandName = "steam_get_p2p_session_state")]
		private static void CCSteamGetP2PSessionState(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			CSteamID csteamID;
			if (!CSteamID.TryParse(args[0], out csteamID))
			{
				throw new ConCommandException("Could not parse steam id.");
			}
			Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
			if (Client.Instance.Networking.GetP2PSessionState(csteamID.value, ref p2PSessionState))
			{
				Debug.LogFormat("ConnectionActive={0}\nConnecting={1}\nP2PSessionError={2}\nUsingRelay={3}\nBytesQueuedForSend={4}\nPacketsQueuedForSend={5}\nRemoteIP={6}\nRemotePort={7}", new object[]
				{
					p2PSessionState.ConnectionActive,
					p2PSessionState.Connecting,
					p2PSessionState.P2PSessionError,
					p2PSessionState.UsingRelay,
					p2PSessionState.BytesQueuedForSend,
					p2PSessionState.PacketsQueuedForSend,
					p2PSessionState.RemoteIP,
					p2PSessionState.RemotePort
				});
				return;
			}
			Debug.LogFormat("Could not get p2p session info for steamId={0}", new object[]
			{
				csteamID
			});
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x0009A0D4 File Offset: 0x000982D4
		[ConCommand(commandName = "steam_server_print_info")]
		private static void CCSteamServerPrintInfo(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			if (GameNetworkManager.singleton.steamworksServer == null)
			{
				throw new ConCommandException("No steamworks server.");
			}
			Debug.Log("" + string.Format("IsValid={0}\n", GameNetworkManager.singleton.steamworksServer.IsValid) + string.Format("Product={0}\n", GameNetworkManager.singleton.steamworksServer.Product) + string.Format("ModDir={0}\n", GameNetworkManager.singleton.steamworksServer.ModDir) + string.Format("SteamId={0}\n", GameNetworkManager.singleton.steamworksServer.SteamId) + string.Format("DedicatedServer={0}\n", GameNetworkManager.singleton.steamworksServer.DedicatedServer) + string.Format("LoggedOn={0}\n", GameNetworkManager.singleton.steamworksServer.LoggedOn) + string.Format("ServerName={0}\n", GameNetworkManager.singleton.steamworksServer.ServerName) + string.Format("PublicIp={0}\n", GameNetworkManager.singleton.steamworksServer.PublicIp) + string.Format("Passworded={0}\n", GameNetworkManager.singleton.steamworksServer.Passworded) + string.Format("MaxPlayers={0}\n", GameNetworkManager.singleton.steamworksServer.MaxPlayers) + string.Format("BotCount={0}\n", GameNetworkManager.singleton.steamworksServer.BotCount) + string.Format("MapName={0}\n", GameNetworkManager.singleton.steamworksServer.MapName) + string.Format("GameDescription={0}\n", GameNetworkManager.singleton.steamworksServer.GameDescription) + string.Format("GameTags={0}\n", GameNetworkManager.singleton.steamworksServer.GameTags));
		}

		// Token: 0x06001F6A RID: 8042 RVA: 0x0009A2DC File Offset: 0x000984DC
		[ConCommand(commandName = "kick_steam", flags = ConVarFlags.SenderMustBeServer, helpText = "Kicks the user with the specified steam id from the server.")]
		private static void CCKickSteam(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID steamId;
			if (CSteamID.TryParse(args[0], out steamId))
			{
				NetworkConnection client = GameNetworkManager.singleton.GetClient(steamId);
				if (client != null)
				{
					GameNetworkManager.singleton.ServerDisconnectClient(client, true);
				}
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001F6B RID: 8043 RVA: 0x00016EA6 File Offset: 0x000150A6
		// (set) Token: 0x06001F6C RID: 8044 RVA: 0x00016EAE File Offset: 0x000150AE
		public bool isHost { get; private set; }

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06001F6D RID: 8045 RVA: 0x0009A324 File Offset: 0x00098524
		// (remove) Token: 0x06001F6E RID: 8046 RVA: 0x0009A358 File Offset: 0x00098558
		public static event Action onStartHostGlobal;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06001F6F RID: 8047 RVA: 0x0009A38C File Offset: 0x0009858C
		// (remove) Token: 0x06001F70 RID: 8048 RVA: 0x0009A3C0 File Offset: 0x000985C0
		public static event Action onStopHostGlobal;

		// Token: 0x06001F71 RID: 8049 RVA: 0x00016EB7 File Offset: 0x000150B7
		public override void OnStartHost()
		{
			base.OnStartHost();
			this.isHost = true;
			Action action = GameNetworkManager.onStartHostGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x00016ED5 File Offset: 0x000150D5
		public override void OnStopHost()
		{
			Action action = GameNetworkManager.onStopHostGlobal;
			if (action != null)
			{
				action();
			}
			this.isHost = false;
			base.OnStopHost();
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x00016EF4 File Offset: 0x000150F4
		[NetworkMessageHandler(client = true, server = false, msgType = 67)]
		private static void HandleKick(NetworkMessage netMsg)
		{
			GameNetworkManager.singleton.StopClient();
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x0009A3F4 File Offset: 0x000985F4
		[NetworkMessageHandler(msgType = 54, client = true)]
		private static void HandleUpdateTime(NetworkMessage netMsg)
		{
			float unpredictedServerFixedTime = netMsg.reader.ReadSingle();
			GameNetworkManager.singleton.unpredictedServerFixedTime = unpredictedServerFixedTime;
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x0009A418 File Offset: 0x00098618
		[NetworkMessageHandler(msgType = 64, client = true, server = true)]
		private static void HandleTest(NetworkMessage netMsg)
		{
			int num = netMsg.reader.ReadInt32();
			Debug.LogFormat("Received test packet. value={0}", new object[]
			{
				num
			});
		}

		// Token: 0x06001F76 RID: 8054 RVA: 0x0009A44C File Offset: 0x0009864C
		[NetworkMessageHandler(msgType = 65, client = true, server = true)]
		private static void HandlePing(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			netMsg.conn.SendByChannel(66, reader.ReadMessage<GameNetworkManager.PingMessage>(), netMsg.channelId);
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x0009A47C File Offset: 0x0009867C
		[NetworkMessageHandler(msgType = 66, client = true, server = true)]
		private static void HandlePingResponse(NetworkMessage netMsg)
		{
			uint timeStampMs = netMsg.reader.ReadMessage<GameNetworkManager.PingMessage>().timeStampMs;
			uint rtt = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds - timeStampMs;
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (netMsg.conn as SteamNetworkConnection)) != null)
			{
				steamNetworkConnection.rtt = rtt;
			}
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x00016F00 File Offset: 0x00015100
		public static bool IsMemberInSteamLobby(CSteamID steamId)
		{
			return Client.Instance.Lobby.UserIsInCurrentLobby(steamId.value);
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x0009A4C4 File Offset: 0x000986C4
		private void OnP2PData(ulong steamId, byte[] data, int dataLength, int channel)
		{
			CSteamID steamId2 = new CSteamID(steamId);
			if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
			{
				Debug.LogFormat("Received packet from {0} dataLength={1} channel={2}", new object[]
				{
					steamId,
					dataLength,
					channel
				});
			}
			NetworkConnection networkConnection;
			if (NetworkServer.active)
			{
				networkConnection = this.GetClient(steamId2);
			}
			else
			{
				NetworkClient client = this.client;
				networkConnection = ((client != null) ? client.connection : null);
			}
			if (networkConnection != null)
			{
				networkConnection.TransportReceive(data, dataLength, 0);
			}
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x0009A544 File Offset: 0x00098744
		public CSteamID GetSteamIDForConnection(NetworkConnection conn)
		{
			if (this.client != null && conn == this.client.connection)
			{
				return new CSteamID(Client.Instance.SteamId);
			}
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = 0; i < connections.Count; i++)
			{
				if (connections[i] == conn)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null)
					{
						return steamNetworkConnection.steamId;
					}
					Debug.LogError("Client is not a SteamNetworkConnection");
				}
			}
			Debug.LogError("Could not find Steam ID");
			return CSteamID.nil;
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x0009A5C8 File Offset: 0x000987C8
		private bool OnIncomingP2PConnection(ulong steamId)
		{
			bool flag = false;
			if (NetworkServer.active)
			{
				flag = (!NetworkServer.dontListen && !this.steamIdBanList.Contains(steamId) && !this.IsServerAtMaxConnections());
			}
			else if (this.client is SteamNetworkClient && ((SteamNetworkClient)this.client).steamConnection.steamId.value == steamId)
			{
				flag = true;
			}
			Debug.LogFormat("Incoming Steamworks connection from Steam ID {0}: {1}", new object[]
			{
				steamId,
				flag ? "accepted" : "rejected"
			});
			if (flag)
			{
				this.CreateP2PConnectionWithPeer(new CSteamID(steamId));
			}
			return flag;
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x0009A66C File Offset: 0x0009886C
		private void OnServerP2PConnectionFailed(ulong steamId, Networking.SessionError sessionError)
		{
			Debug.LogFormat("GameNetworkManager.OnServerP2PConnectionFailed steamId={0} sessionError={1}", new object[]
			{
				steamId,
				sessionError
			});
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId)
				{
					this.ServerHandleClientDisconnect(steamNetworkConnection);
				}
			}
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x0009A6DC File Offset: 0x000988DC
		private void OnClientP2PConnectionFailed(ulong steamId, Networking.SessionError sessionError)
		{
			Debug.LogFormat("GameNetworkManager.OnClientP2PConnectionFailed steamId={0} sessionError={1}", new object[]
			{
				steamId,
				sessionError
			});
			SteamNetworkConnection steamNetworkConnection;
			if (this.client != null && (steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId)
			{
				steamNetworkConnection.InvokeHandlerNoData(33);
				steamNetworkConnection.Disconnect();
				steamNetworkConnection.Dispose();
			}
			if (NetworkServer.active)
			{
				ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
				for (int i = connections.Count - 1; i >= 0; i--)
				{
					SteamNetworkConnection steamNetworkConnection2;
					if ((steamNetworkConnection2 = (connections[i] as SteamNetworkConnection)) != null && steamNetworkConnection2.steamId.value == steamId)
					{
						steamNetworkConnection2.InvokeHandlerNoData(33);
						steamNetworkConnection2.Disconnect();
						steamNetworkConnection2.Dispose();
					}
				}
			}
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x0009A7A0 File Offset: 0x000989A0
		public void CreateP2PConnectionWithPeer(CSteamID peer)
		{
			SteamNetworkConnection steamNetworkConnection = new SteamNetworkConnection(peer);
			steamNetworkConnection.ForceInitialize(NetworkServer.hostTopology);
			int num = -1;
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = 1; i < connections.Count; i++)
			{
				if (connections[i] == null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = connections.Count;
			}
			steamNetworkConnection.connectionId = num;
			NetworkServer.AddExternalConnection(steamNetworkConnection);
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(32);
			networkWriter.FinishMessage();
			steamNetworkConnection.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06001F7F RID: 8063 RVA: 0x0009A82C File Offset: 0x00098A2C
		// (remove) Token: 0x06001F80 RID: 8064 RVA: 0x0009A860 File Offset: 0x00098A60
		public static event Action onStartServerGlobal;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06001F81 RID: 8065 RVA: 0x0009A894 File Offset: 0x00098A94
		// (remove) Token: 0x06001F82 RID: 8066 RVA: 0x0009A8C8 File Offset: 0x00098AC8
		public static event Action onStopServerGlobal;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06001F83 RID: 8067 RVA: 0x0009A8FC File Offset: 0x00098AFC
		// (remove) Token: 0x06001F84 RID: 8068 RVA: 0x0009A930 File Offset: 0x00098B30
		public static event Action<NetworkConnection> onServerConnectGlobal;

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06001F85 RID: 8069 RVA: 0x0009A964 File Offset: 0x00098B64
		// (remove) Token: 0x06001F86 RID: 8070 RVA: 0x0009A998 File Offset: 0x00098B98
		public static event Action<NetworkConnection> onServerDisconnectGlobal;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06001F87 RID: 8071 RVA: 0x0009A9CC File Offset: 0x00098BCC
		// (remove) Token: 0x06001F88 RID: 8072 RVA: 0x0009AA00 File Offset: 0x00098C00
		public static event Action<string> onServerSceneChangedGlobal;

		// Token: 0x06001F89 RID: 8073 RVA: 0x0009AA34 File Offset: 0x00098C34
		private void StartSteamworksServer()
		{
			string modDir = "Risk of Rain 2";
			string gameDesc = "Risk of Rain 2";
			this.steamworksServer = new Server(632360u, new ServerInit(modDir, gameDesc)
			{
				VersionString = "0.0.0.0",
				Secure = true,
				IpAddress = IPAddress.Any,
				GamePort = 7777
			});
			Debug.LogFormat("steamworksServer.IsValid={0}", new object[]
			{
				this.steamworksServer.IsValid
			});
			if (!this.steamworksServer.IsValid)
			{
				this.steamworksServer.Dispose();
				this.steamworksServer = null;
			}
			if (this.steamworksServer != null)
			{
				this.steamworksServer.MaxPlayers = base.maxConnections;
				this.steamworksServer.ServerName = "NAME";
				this.steamworksServer.DedicatedServer = false;
				this.steamworksServer.AutomaticHeartbeats = true;
				this.steamworksServer.MapName = SceneManager.GetActiveScene().name;
				this.steamworksServer.LogOnAnonymous();
				Debug.LogFormat("steamworksServer.LoggedOn={0}", new object[]
				{
					this.steamworksServer.LoggedOn
				});
				base.StartCoroutine("CheckIPUntilAvailable");
				base.StartCoroutine("UpdateSteamServerPlayers");
				SteamworksLobbyManager.OnServerSteamIDDiscovered(new CSteamID(Client.Instance.SteamId));
				GameNetworkManager.onServerSceneChangedGlobal += this.UpdateSteamMapName;
			}
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x00016F17 File Offset: 0x00015117
		private void UpdateSteamMapName(string sceneName)
		{
			if (this.steamworksServer != null)
			{
				this.steamworksServer.MapName = sceneName;
			}
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x00016F2D File Offset: 0x0001512D
		private void StopSteamworksServer()
		{
			GameNetworkManager.onServerSceneChangedGlobal -= this.UpdateSteamMapName;
			if (this.steamworksServer != null)
			{
				this.steamworksServer.Dispose();
				this.steamworksServer = null;
			}
			base.StopCoroutine("CheckIPUntilAvailable");
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00016F65 File Offset: 0x00015165
		private IEnumerator CheckIPUntilAvailable()
		{
			IPAddress address = null;
			while (this.steamworksServer != null && (address = this.steamworksServer.PublicIp) == null)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
			if (address != null)
			{
				SteamworksLobbyManager.OnServerIPDiscovered(address.ToString(), (ushort)NetworkServer.listenPort);
			}
			else
			{
				Debug.Log("Failed to find Steamworks server IP.");
			}
			yield break;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x00016F74 File Offset: 0x00015174
		private IEnumerator UpdateSteamServerPlayers()
		{
			while (this.steamworksServer != null)
			{
				foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (networkUser.connectionToClient as SteamNetworkConnection)) != null)
					{
						this.steamworksServer.UpdatePlayer(steamNetworkConnection.steamId.value, networkUser.userName, 0);
					}
				}
				yield return new WaitForSecondsRealtime(1f);
			}
			yield break;
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x0009AB98 File Offset: 0x00098D98
		public NetworkConnection GetClient(CSteamID steamId)
		{
			if (!NetworkServer.active)
			{
				return null;
			}
			if (steamId.value == Client.Instance.SteamId && NetworkServer.connections.Count > 0)
			{
				return NetworkServer.connections[0];
			}
			using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (enumerator.Current as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId.value)
					{
						return steamNetworkConnection;
					}
				}
			}
			Debug.LogError("Client not found");
			return null;
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x00016F83 File Offset: 0x00015183
		public override void OnStartServer()
		{
			base.OnStartServer();
			NetworkMessageHandlerAttribute.RegisterServerMessages();
			this.unpredictedServerFixedTime = 0f;
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkSession"));
			Action action = GameNetworkManager.onStartServerGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x0009AC40 File Offset: 0x00098E40
		public override void OnStopServer()
		{
			Action action = GameNetworkManager.onStopServerGlobal;
			if (action != null)
			{
				action();
			}
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			List<NetworkConnection> localConnections = NetworkServer.localConnections;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null && !localConnections.Contains(networkConnection))
				{
					this.ServerDisconnectClient(networkConnection, false);
				}
			}
			base.OnStopServer();
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x00016FBA File Offset: 0x000151BA
		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			if (NetworkUser.readOnlyInstancesList.Count >= base.maxConnections)
			{
				this.ServerDisconnectClient(conn, true);
				return;
			}
			Action<NetworkConnection> action = GameNetworkManager.onServerConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x0009ACA0 File Offset: 0x00098EA0
		public override void OnServerDisconnect(NetworkConnection conn)
		{
			Action<NetworkConnection> action = GameNetworkManager.onServerDisconnectGlobal;
			if (action != null)
			{
				action(conn);
			}
			if (conn.clientOwnedObjects != null)
			{
				foreach (NetworkInstanceId netId in new HashSet<NetworkInstanceId>(conn.clientOwnedObjects))
				{
					GameObject gameObject = NetworkServer.FindLocalObject(netId);
					if (gameObject != null && gameObject.GetComponent<CharacterMaster>())
					{
						NetworkIdentity component = gameObject.GetComponent<NetworkIdentity>();
						if (component && component.clientAuthorityOwner == conn)
						{
							component.RemoveClientAuthority(conn);
						}
					}
				}
			}
			List<PlayerController> playerControllers = conn.playerControllers;
			for (int i = 0; i < playerControllers.Count; i++)
			{
				NetworkUser component2 = playerControllers[i].gameObject.GetComponent<NetworkUser>();
				if (component2)
				{
					Chat.SendPlayerDisconnectedMessage(component2);
				}
			}
			if (conn is SteamNetworkConnection)
			{
				Debug.LogFormat("Closing connection with steamId {0}", new object[]
				{
					((SteamNetworkConnection)conn).steamId.value
				});
			}
			base.OnServerDisconnect(conn);
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x00016FEE File Offset: 0x000151EE
		private void ServerHandleClientDisconnect(NetworkConnection conn)
		{
			conn.InvokeHandlerNoData(33);
			conn.Disconnect();
			conn.Dispose();
			if (conn is SteamNetworkConnection)
			{
				NetworkServer.RemoveExternalConnection(conn.connectionId);
			}
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x0009ADBC File Offset: 0x00098FBC
		private void ServerDisconnectClient(NetworkConnection conn, bool kick)
		{
			Debug.LogFormat("Disconnecting client on connection {0}", new object[]
			{
				conn.connectionId
			});
			SteamNetworkConnection steamNetworkConnection = conn as SteamNetworkConnection;
			bool flag = steamNetworkConnection != null && kick;
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(33);
			networkWriter.FinishMessage();
			conn.SendByChannel(67, new GameNetworkManager.KickMessage
			{
				reason = 0
			}, QosChannelIndex.defaultReliable.intVal);
			conn.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			conn.InvokeHandlerNoData(33);
			conn.Disconnect();
			conn.Dispose();
			if (steamNetworkConnection != null)
			{
				NetworkServer.RemoveExternalConnection(conn.connectionId);
			}
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x00017018 File Offset: 0x00015218
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			this.OnServerAddPlayer(conn, playerControllerId, null);
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x00017023 File Offset: 0x00015223
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId, extraMessageReader);
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x0009AE5C File Offset: 0x0009905C
		private void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			if (base.playerPrefab == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
				}
				return;
			}
			if (base.playerPrefab.GetComponent<NetworkIdentity>() == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
				}
				return;
			}
			if ((int)playerControllerId < conn.playerControllers.Count && conn.playerControllers[(int)playerControllerId].IsValid && conn.playerControllers[(int)playerControllerId].gameObject != null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("There is already a player at that playerControllerId for this connections.");
				}
				return;
			}
			if (NetworkUser.readOnlyInstancesList.Count >= base.maxConnections)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("Cannot add any more players.)");
				}
				return;
			}
			GameNetworkManager.AddPlayerMessage addPlayerMessage = extraMessageReader.ReadMessage<GameNetworkManager.AddPlayerMessage>();
			Transform startPosition = base.GetStartPosition();
			GameObject gameObject;
			if (startPosition != null)
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(base.playerPrefab, startPosition.position, startPosition.rotation);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(base.playerPrefab, Vector3.zero, Quaternion.identity);
			}
			Debug.LogFormat("GameNetworkManager.AddPlayerInternal(conn={0}, playerControllerId={1}, extraMessageReader={2}", new object[]
			{
				conn,
				playerControllerId,
				extraMessageReader
			});
			NetworkUser component = gameObject.GetComponent<NetworkUser>();
			bool flag = Util.ConnectionIsLocal(conn);
			NetworkUserId id = NetworkUserId.FromIp(conn.address, (byte)playerControllerId);
			CSteamID csteamID = CSteamID.nil;
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				csteamID = steamNetworkConnection.steamId;
			}
			else if (flag && Client.Instance != null)
			{
				csteamID = new CSteamID(Client.Instance.SteamId);
			}
			if (csteamID != CSteamID.nil)
			{
				id = NetworkUserId.FromSteamId(csteamID.value, (byte)playerControllerId);
			}
			if (this.steamworksServer != null && csteamID != CSteamID.nil && this.steamworksServer.Auth.StartSession(addPlayerMessage.steamAuthTicketData, csteamID.value))
			{
				this.steamworksServer.UpdatePlayer(csteamID.value, RoR2Application.instance.steamworksClient.Friends.GetName(csteamID.value), 0);
			}
			component.id = id;
			Chat.SendPlayerConnectedMessage(component);
			NetworkServer.AddPlayerForConnection(conn, gameObject, playerControllerId);
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x0009B074 File Offset: 0x00099274
		private void UpdateServer()
		{
			Server server = this.steamworksServer;
			if (server != null)
			{
				server.Update();
			}
			if (NetworkServer.active)
			{
				ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
				for (int i = connections.Count - 1; i >= 0; i--)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null)
					{
						Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
						if (Client.Instance.Networking.GetP2PSessionState(steamNetworkConnection.steamId.value, ref p2PSessionState) && p2PSessionState.Connecting == 0 && p2PSessionState.ConnectionActive == 0)
						{
							this.ServerHandleClientDisconnect(steamNetworkConnection);
						}
					}
				}
			}
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x0009B100 File Offset: 0x00099300
		private void FixedUpdateServer()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.timeTransmitTimer -= Time.fixedDeltaTime;
			if (this.timeTransmitTimer <= 0f)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(54);
				networkWriter.Write(this.unpredictedServerFixedTime);
				networkWriter.FinishMessage();
				NetworkServer.SendWriterToReady(null, networkWriter, QosChannelIndex.time.intVal);
				this.timeTransmitTimer += this.timeTransmitInterval;
			}
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					this.Ping(networkConnection, QosChannelIndex.ping.intVal);
				}
			}
		}

		// Token: 0x06001F9A RID: 8090 RVA: 0x0001702E File Offset: 0x0001522E
		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);
			if (Run.instance)
			{
				Run.instance.OnServerSceneChanged(sceneName);
			}
			Action<string> action = GameNetworkManager.onServerSceneChangedGlobal;
			if (action == null)
			{
				return;
			}
			action(sceneName);
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x0009B1D0 File Offset: 0x000993D0
		private bool IsServerAtMaxConnections()
		{
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			if (connections.Count >= base.maxConnections)
			{
				int num = 0;
				using (IEnumerator<NetworkConnection> enumerator = connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null)
						{
							num++;
						}
					}
				}
				return num >= base.maxConnections;
			}
			return false;
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x0009B23C File Offset: 0x0009943C
		public int GetConnectingClientCount()
		{
			int num = 0;
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			int count = connections.Count;
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			int count2 = readOnlyInstancesList.Count;
			for (int i = 0; i < count; i++)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null)
				{
					bool flag = false;
					for (int j = 0; j < count2; j++)
					{
						if (readOnlyInstancesList[j].connectionToClient == networkConnection)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x040021CF RID: 8655
		private static readonly FieldInfo loadingSceneAsyncFieldInfo;

		// Token: 0x040021D1 RID: 8657
		private static readonly string[] spawnableFolders = new string[]
		{
			"CharacterBodies",
			"CharacterMasters",
			"Projectiles",
			"NetworkedObjects",
			"GameModes"
		};

		// Token: 0x040021D3 RID: 8659
		public float debugServerTime;

		// Token: 0x040021D4 RID: 8660
		public float debugRTT;

		// Token: 0x040021D9 RID: 8665
		private static bool wasFading = false;

		// Token: 0x040021DB RID: 8667
		private float rttVelocity;

		// Token: 0x040021DC RID: 8668
		public float filteredRTTSmoothDuration = 0.1f;

		// Token: 0x040021DE RID: 8670
		private static CSteamID pendingHostId = CSteamID.nil;

		// Token: 0x040021DF RID: 8671
		private static readonly string[] sceneWhiteList = new string[]
		{
			"title",
			"crystalworld",
			"logbook"
		};

		// Token: 0x040021E3 RID: 8675
		private List<ulong> steamIdBanList = new List<ulong>();

		// Token: 0x040021E9 RID: 8681
		public Server steamworksServer;

		// Token: 0x040021EA RID: 8682
		public float timeTransmitInterval = 0.0166666675f;

		// Token: 0x040021EB RID: 8683
		private float timeTransmitTimer;

		// Token: 0x02000577 RID: 1399
		private class SteamClientJoinAttempt : MonoBehaviour
		{
			// Token: 0x06001F9E RID: 8094 RVA: 0x0009B2B8 File Offset: 0x000994B8
			private void Update()
			{
				this.age += Time.unscaledDeltaTime;
				this.isLoadingScene = GameNetworkManager.isLoadingScene;
				this.sceneCount = SceneManager.sceneCount;
				this.clientActive = NetworkClient.active;
				if (this.age > 1f && !this.isLoadingScene && this.sceneCount == 1 && !this.clientActive)
				{
					UnityEngine.Object.Destroy(this);
					this.networkManager.StartClientSteam(this.serverId);
				}
			}

			// Token: 0x040021EC RID: 8684
			public GameNetworkManager networkManager;

			// Token: 0x040021ED RID: 8685
			public CSteamID serverId;

			// Token: 0x040021EE RID: 8686
			private float age;

			// Token: 0x040021EF RID: 8687
			private bool isLoadingScene;

			// Token: 0x040021F0 RID: 8688
			private int sceneCount;

			// Token: 0x040021F1 RID: 8689
			private bool clientActive;
		}

		// Token: 0x02000578 RID: 1400
		private class NetLogLevelConVar : BaseConVar
		{
			// Token: 0x06001FA0 RID: 8096 RVA: 0x000090A8 File Offset: 0x000072A8
			public NetLogLevelConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FA1 RID: 8097 RVA: 0x0009B338 File Offset: 0x00099538
			public override void SetString(string newValue)
			{
				int currentLogLevel;
				if (TextSerialization.TryParseInvariant(newValue, out currentLogLevel))
				{
					LogFilter.currentLogLevel = currentLogLevel;
				}
			}

			// Token: 0x06001FA2 RID: 8098 RVA: 0x00017087 File Offset: 0x00015287
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(LogFilter.currentLogLevel);
			}

			// Token: 0x040021F2 RID: 8690
			private static GameNetworkManager.NetLogLevelConVar cvNetLogLevel = new GameNetworkManager.NetLogLevelConVar("net_loglevel", ConVarFlags.Engine, null, "Network log verbosity.");
		}

		// Token: 0x02000579 RID: 1401
		private class SvListenConVar : BaseConVar
		{
			// Token: 0x06001FA4 RID: 8100 RVA: 0x000090A8 File Offset: 0x000072A8
			public SvListenConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FA5 RID: 8101 RVA: 0x0009B358 File Offset: 0x00099558
			public override void SetString(string newValue)
			{
				if (NetworkServer.active)
				{
					Debug.Log("Can't change value of sv_listen while server is running.");
					return;
				}
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					NetworkServer.dontListen = (num == 0);
				}
			}

			// Token: 0x06001FA6 RID: 8102 RVA: 0x000170AC File Offset: 0x000152AC
			public override string GetString()
			{
				if (!NetworkServer.dontListen)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x040021F3 RID: 8691
			private static GameNetworkManager.SvListenConVar cvSvListen = new GameNetworkManager.SvListenConVar("sv_listen", ConVarFlags.Engine, null, "Whether or not the server will accept connections from other players.");
		}

		// Token: 0x0200057A RID: 1402
		private class SvMaxPlayersConVar : BaseConVar
		{
			// Token: 0x06001FA8 RID: 8104 RVA: 0x000090A8 File Offset: 0x000072A8
			public SvMaxPlayersConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FA9 RID: 8105 RVA: 0x0009B38C File Offset: 0x0009958C
			public override void SetString(string newValue)
			{
				if (NetworkServer.active)
				{
					Debug.Log("Can't change value of sv_maxplayers while server is running.");
					return;
				}
				int val;
				if (NetworkManager.singleton && TextSerialization.TryParseInvariant(newValue, out val))
				{
					NetworkManager.singleton.maxConnections = Math.Min(Math.Max(val, 1), 16);
				}
			}

			// Token: 0x06001FAA RID: 8106 RVA: 0x000170D9 File Offset: 0x000152D9
			public override string GetString()
			{
				if (!NetworkManager.singleton)
				{
					return "1";
				}
				return TextSerialization.ToStringInvariant(NetworkManager.singleton.maxConnections);
			}

			// Token: 0x040021F4 RID: 8692
			private static GameNetworkManager.SvMaxPlayersConVar cvSvListen = new GameNetworkManager.SvMaxPlayersConVar("sv_maxplayers", ConVarFlags.Engine, null, "Maximum number of players allowed.");
		}

		// Token: 0x0200057B RID: 1403
		private class KickMessage : MessageBase
		{
			// Token: 0x06001FAD RID: 8109 RVA: 0x00017115 File Offset: 0x00015315
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.reason);
			}

			// Token: 0x06001FAE RID: 8110 RVA: 0x00017123 File Offset: 0x00015323
			public override void Deserialize(NetworkReader reader)
			{
				this.reason = (int)reader.ReadPackedUInt32();
			}

			// Token: 0x040021F5 RID: 8693
			public int reason;
		}

		// Token: 0x0200057C RID: 1404
		protected class AddPlayerMessage : MessageBase
		{
			// Token: 0x06001FB0 RID: 8112 RVA: 0x00017131 File Offset: 0x00015331
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt64(this.steamId);
				writer.WriteBytesFull(this.steamAuthTicketData);
			}

			// Token: 0x06001FB1 RID: 8113 RVA: 0x0001714B File Offset: 0x0001534B
			public override void Deserialize(NetworkReader reader)
			{
				this.steamId = reader.ReadPackedUInt64();
				this.steamAuthTicketData = reader.ReadBytesAndSize();
			}

			// Token: 0x040021F6 RID: 8694
			public ulong steamId;

			// Token: 0x040021F7 RID: 8695
			public byte[] steamAuthTicketData;
		}

		// Token: 0x0200057D RID: 1405
		private class PingMessage : MessageBase
		{
			// Token: 0x06001FB3 RID: 8115 RVA: 0x00017165 File Offset: 0x00015365
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.timeStampMs);
			}

			// Token: 0x06001FB4 RID: 8116 RVA: 0x00017173 File Offset: 0x00015373
			public override void Deserialize(NetworkReader reader)
			{
				this.timeStampMs = reader.ReadPackedUInt32();
			}

			// Token: 0x040021F8 RID: 8696
			public uint timeStampMs;
		}
	}
}
