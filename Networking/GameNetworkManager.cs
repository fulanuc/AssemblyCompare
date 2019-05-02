using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Facepunch.Steamworks;
using RoR2.ConVar;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2.Networking
{
	// Token: 0x02000585 RID: 1413
	public class GameNetworkManager : NetworkManager
	{
		// Token: 0x06001FA4 RID: 8100 RVA: 0x00099D68 File Offset: 0x00097F68
		static GameNetworkManager()
		{
			GameNetworkManager.loadingSceneAsyncFieldInfo = typeof(NetworkManager).GetField("s_LoadingSceneAsync", BindingFlags.Static | BindingFlags.NonPublic);
			if (GameNetworkManager.loadingSceneAsyncFieldInfo == null)
			{
				Debug.LogError("NetworkManager.s_LoadingSceneAsync field could not be found! Make sure to provide a proper implementation for this version of Unity.");
			}
			GameNetworkManager.StaticInit();
		}

		// Token: 0x06001FA5 RID: 8101 RVA: 0x00099E14 File Offset: 0x00098014
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

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06001FA6 RID: 8102 RVA: 0x00099E6C File Offset: 0x0009806C
		private static bool isLoadingScene
		{
			get
			{
				AsyncOperation asyncOperation = (AsyncOperation)GameNetworkManager.loadingSceneAsyncFieldInfo.GetValue(null);
				return asyncOperation != null && !asyncOperation.isDone;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06001FA7 RID: 8103 RVA: 0x000172B5 File Offset: 0x000154B5
		public new static GameNetworkManager singleton
		{
			get
			{
				return (GameNetworkManager)NetworkManager.singleton;
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06001FA8 RID: 8104 RVA: 0x000172C1 File Offset: 0x000154C1
		// (set) Token: 0x06001FA9 RID: 8105 RVA: 0x000172C9 File Offset: 0x000154C9
		public float unpredictedServerFixedTime { get; private set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06001FAA RID: 8106 RVA: 0x000172D2 File Offset: 0x000154D2
		public float serverFixedTime
		{
			get
			{
				return this.unpredictedServerFixedTime + this.filteredClientRTT;
			}
		}

		// Token: 0x06001FAB RID: 8107 RVA: 0x00099E98 File Offset: 0x00098098
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

		// Token: 0x06001FAC RID: 8108 RVA: 0x000172E1 File Offset: 0x000154E1
		private void Ping(NetworkConnection conn, int channelId)
		{
			conn.SendByChannel(65, new GameNetworkManager.PingMessage
			{
				timeStampMs = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds
			}, channelId);
		}

		// Token: 0x06001FAD RID: 8109 RVA: 0x00099EE0 File Offset: 0x000980E0
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

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06001FAE RID: 8110 RVA: 0x00099FCC File Offset: 0x000981CC
		// (remove) Token: 0x06001FAF RID: 8111 RVA: 0x0009A000 File Offset: 0x00098200
		public static event Action onStartGlobal;

		// Token: 0x06001FB0 RID: 8112 RVA: 0x00017308 File Offset: 0x00015508
		private void OnDestroy()
		{
			typeof(NetworkManager).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
		}

		// Token: 0x06001FB1 RID: 8113 RVA: 0x0009A034 File Offset: 0x00098234
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

		// Token: 0x06001FB2 RID: 8114 RVA: 0x00017328 File Offset: 0x00015528
		protected void Update()
		{
			this.EnsureDesiredHost();
			this.UpdateServer();
			this.UpdateClient();
		}

		// Token: 0x06001FB3 RID: 8115 RVA: 0x0009A088 File Offset: 0x00098288
		private void EnsureDesiredHost()
		{
			if (false | this.serverShuttingDown | this.clientIsConnecting | (NetworkServer.active && GameNetworkManager.isLoadingScene) | (!NetworkClient.active && GameNetworkManager.isLoadingScene))
			{
				return;
			}
			if (this.isNetworkActive && !this.actedUponDesiredHost && !this.desiredHost.DescribesCurrentHost())
			{
				this.Disconnect();
				return;
			}
			if (!this.actedUponDesiredHost)
			{
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.Self)
				{
					if (NetworkServer.active)
					{
						return;
					}
					this.actedUponDesiredHost = true;
					base.maxConnections = this.desiredHost.hostingParameters.maxPlayers;
					NetworkServer.dontListen = !this.desiredHost.hostingParameters.listen;
					this.StartHost();
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.Steam && Time.unscaledTime - this.lastDesiredHostSetTime >= 0f)
				{
					this.actedUponDesiredHost = true;
					this.StartClientSteam(this.desiredHost.steamId);
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.IPv4 && Time.unscaledTime - this.lastDesiredHostSetTime >= 0f)
				{
					this.actedUponDesiredHost = true;
					Debug.LogFormat("Attempting connection. ip={0} port={1}", new object[]
					{
						this.desiredHost.addressPortPair.address,
						this.desiredHost.addressPortPair.port
					});
					GameNetworkManager.singleton.networkAddress = this.desiredHost.addressPortPair.address;
					GameNetworkManager.singleton.networkPort = (int)this.desiredHost.addressPortPair.port;
					GameNetworkManager.singleton.StartClient();
				}
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06001FB4 RID: 8116 RVA: 0x0001733C File Offset: 0x0001553C
		// (set) Token: 0x06001FB5 RID: 8117 RVA: 0x0009A22C File Offset: 0x0009842C
		public GameNetworkManager.HostDescription desiredHost
		{
			get
			{
				return this._desiredHost;
			}
			set
			{
				if (this._desiredHost.Equals(value))
				{
					return;
				}
				this._desiredHost = value;
				this.actedUponDesiredHost = false;
				this.lastDesiredHostSetTime = Time.unscaledTime;
				Debug.LogFormat("GameNetworkManager.desiredHost={0}", new object[]
				{
					this._desiredHost.ToString()
				});
			}
		}

		// Token: 0x06001FB6 RID: 8118 RVA: 0x0009A288 File Offset: 0x00098488
		public void ForceCloseAllConnections()
		{
			Client instance = Client.Instance;
			Networking networking = (instance != null) ? instance.Networking : null;
			if (networking == null)
			{
				return;
			}
			using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (enumerator.Current as SteamNetworkConnection)) != null)
					{
						networking.CloseSession(steamNetworkConnection.steamId.value);
					}
				}
			}
			NetworkClient client = this.client;
			SteamNetworkConnection steamNetworkConnection2;
			if ((steamNetworkConnection2 = (((client != null) ? client.connection : null) as SteamNetworkConnection)) != null)
			{
				networking.CloseSession(steamNetworkConnection2.steamId.value);
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06001FB7 RID: 8119 RVA: 0x00017344 File Offset: 0x00015544
		private bool clientIsConnecting
		{
			get
			{
				NetworkClient client = this.client;
				return ((client != null) ? client.connection : null) != null && !this.client.isConnected;
			}
		}

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06001FB8 RID: 8120 RVA: 0x0009A32C File Offset: 0x0009852C
		// (remove) Token: 0x06001FB9 RID: 8121 RVA: 0x0009A360 File Offset: 0x00098560
		public static event Action<NetworkClient> onStartClientGlobal;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06001FBA RID: 8122 RVA: 0x0009A394 File Offset: 0x00098594
		// (remove) Token: 0x06001FBB RID: 8123 RVA: 0x0009A3C8 File Offset: 0x000985C8
		public static event Action onStopClientGlobal;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06001FBC RID: 8124 RVA: 0x0009A3FC File Offset: 0x000985FC
		// (remove) Token: 0x06001FBD RID: 8125 RVA: 0x0009A430 File Offset: 0x00098630
		public static event Action<NetworkConnection> onClientConnectGlobal;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06001FBE RID: 8126 RVA: 0x0009A464 File Offset: 0x00098664
		// (remove) Token: 0x06001FBF RID: 8127 RVA: 0x0009A498 File Offset: 0x00098698
		public static event Action<NetworkConnection> onClientDisconnectGlobal;

		// Token: 0x06001FC0 RID: 8128 RVA: 0x0009A4CC File Offset: 0x000986CC
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

		// Token: 0x06001FC1 RID: 8129 RVA: 0x0009A560 File Offset: 0x00098760
		public override void OnStopClient()
		{
			this.ForceCloseAllConnections();
			if (this.actedUponDesiredHost)
			{
				GameNetworkManager.singleton.desiredHost = default(GameNetworkManager.HostDescription);
			}
			Action action = GameNetworkManager.onStopClientGlobal;
			if (action != null)
			{
				action();
			}
			base.OnStopClient();
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x0001736A File Offset: 0x0001556A
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

		// Token: 0x06001FC3 RID: 8131 RVA: 0x0009A5A4 File Offset: 0x000987A4
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

		// Token: 0x06001FC4 RID: 8132 RVA: 0x0009A5F0 File Offset: 0x000987F0
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

		// Token: 0x06001FC5 RID: 8133 RVA: 0x0009A6F4 File Offset: 0x000988F4
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

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06001FC6 RID: 8134 RVA: 0x000173A0 File Offset: 0x000155A0
		// (set) Token: 0x06001FC7 RID: 8135 RVA: 0x000173A8 File Offset: 0x000155A8
		public float clientRTT { get; private set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06001FC8 RID: 8136 RVA: 0x000173B1 File Offset: 0x000155B1
		// (set) Token: 0x06001FC9 RID: 8137 RVA: 0x000173B9 File Offset: 0x000155B9
		public float filteredClientRTT { get; private set; }

		// Token: 0x06001FCA RID: 8138 RVA: 0x0009A7A8 File Offset: 0x000989A8
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

		// Token: 0x06001FCB RID: 8139 RVA: 0x0009A890 File Offset: 0x00098A90
		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(NetworkManager.networkSceneName);
			string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name);
			Debug.LogFormat("OnClientSceneChanged networkSceneName={0} currentSceneName={1}", new object[]
			{
				fileNameWithoutExtension,
				fileNameWithoutExtension2
			});
			if (fileNameWithoutExtension != fileNameWithoutExtension2)
			{
				Debug.Log("OnClientSceneChanged skipped due to scene mismatch.");
				return;
			}
			base.autoCreatePlayer = false;
			base.OnClientSceneChanged(conn);
			this.ClientSetPlayers(conn);
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x0009A8FC File Offset: 0x00098AFC
		private void ClientSetPlayers(NetworkConnection conn)
		{
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			for (int i = 0; i < readOnlyLocalUsersList.Count; i++)
			{
				this.ClientAddPlayer((short)readOnlyLocalUsersList[i].id, conn);
			}
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x0009A934 File Offset: 0x00098B34
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

		// Token: 0x06001FCE RID: 8142 RVA: 0x0009AA5C File Offset: 0x00098C5C
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

		// Token: 0x06001FCF RID: 8143 RVA: 0x000173C2 File Offset: 0x000155C2
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClientInit()
		{
			SceneCatalog.onMostRecentSceneDefChanged += GameNetworkManager.ClientUpdateOfflineScene;
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x000173D5 File Offset: 0x000155D5
		private static void ClientUpdateOfflineScene(SceneDef sceneDef)
		{
			if (GameNetworkManager.singleton && sceneDef.isOfflineScene)
			{
				GameNetworkManager.singleton.offlineScene = sceneDef.sceneName;
			}
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x000173FB File Offset: 0x000155FB
		private static void EnsureNetworkManagerNotBusy()
		{
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			if (GameNetworkManager.singleton.serverShuttingDown || GameNetworkManager.isLoadingScene)
			{
				throw new ConCommandException("NetworkManager is busy and cannot receive commands.");
			}
		}

		// Token: 0x06001FD2 RID: 8146 RVA: 0x0009AAE0 File Offset: 0x00098CE0
		[ConCommand(commandName = "client_set_players", flags = ConVarFlags.None, helpText = "Adds network players for all local players. Debug only.")]
		private static void CCClientSetPlayers(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				GameNetworkManager.singleton.ClientSetPlayers(GameNetworkManager.singleton.client.connection);
			}
		}

		// Token: 0x06001FD3 RID: 8147 RVA: 0x0009AB30 File Offset: 0x00098D30
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

		// Token: 0x06001FD4 RID: 8148 RVA: 0x0009ABC4 File Offset: 0x00098DC4
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

		// Token: 0x06001FD5 RID: 8149 RVA: 0x0009ACA4 File Offset: 0x00098EA4
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

		// Token: 0x06001FD6 RID: 8150 RVA: 0x0009ACF8 File Offset: 0x00098EF8
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

		// Token: 0x06001FD7 RID: 8151 RVA: 0x0009AD54 File Offset: 0x00098F54
		[ConCommand(commandName = "disconnect", flags = ConVarFlags.None, helpText = "Disconnect from a server or shut down the current server.")]
		private static void CCDisconnect(ConCommandArgs args)
		{
			GameNetworkManager.singleton.desiredHost = default(GameNetworkManager.HostDescription);
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x00017428 File Offset: 0x00015628
		private void Disconnect()
		{
			if (this.serverShuttingDown)
			{
				return;
			}
			if (GameNetworkManager.singleton.isNetworkActive)
			{
				Debug.Log("Network shutting down...");
				if (NetworkServer.active)
				{
					GameNetworkManager.singleton.RequestServerShutdown();
					return;
				}
				GameNetworkManager.singleton.StopClient();
			}
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x0009AD74 File Offset: 0x00098F74
		[ConCommand(commandName = "connect", flags = ConVarFlags.None, helpText = "Connect to a server.")]
		private static void CCConnect(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			GameNetworkManager.EnsureNetworkManagerNotBusy();
			string[] array = args[0].Split(new char[]
			{
				':'
			});
			string address = array[0];
			ushort port = 7777;
			if (array.Length > 1)
			{
				TextSerialization.TryParseInvariant(array[1], out port);
			}
			GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new AddressPortPair(address, port));
		}

		// Token: 0x06001FDA RID: 8154 RVA: 0x0009ADE8 File Offset: 0x00098FE8
		[ConCommand(commandName = "connect_steamworks_p2p", flags = ConVarFlags.None, helpText = "Connect to a server using Steamworks P2P. Argument is the 64-bit Steam ID of the server to connect to.")]
		private static void CCConnectSteamworksP2P(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
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
			GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(csteamID);
		}

		// Token: 0x06001FDB RID: 8155 RVA: 0x0009AE80 File Offset: 0x00099080
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
			if (!flag)
			{
				int maxPlayers = GameNetworkManager.SvMaxPlayersConVar.instance.intValue;
				if (SteamworksLobbyManager.isInLobby)
				{
					maxPlayers = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers;
				}
				GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters
				{
					listen = args.GetArgBool(0),
					maxPlayers = maxPlayers
				});
			}
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x0009AF18 File Offset: 0x00099118
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

		// Token: 0x06001FDD RID: 8157 RVA: 0x0009B01C File Offset: 0x0009921C
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

		// Token: 0x06001FDE RID: 8158 RVA: 0x0009B224 File Offset: 0x00099424
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
					GameNetworkManager.singleton.ServerKickClient(client, GameNetworkManager.KickReason.Kick);
				}
			}
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x0009B26C File Offset: 0x0009946C
		[ConCommand(commandName = "ban_steam", flags = ConVarFlags.SenderMustBeServer, helpText = "Bans the user with the specified steam id from the server.")]
		private static void CCBanSteam(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID steamId;
			if (CSteamID.TryParse(args[0], out steamId))
			{
				NetworkConnection client = GameNetworkManager.singleton.GetClient(steamId);
				if (client != null)
				{
					GameNetworkManager.singleton.ServerKickClient(client, GameNetworkManager.KickReason.Ban);
				}
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06001FE0 RID: 8160 RVA: 0x00017465 File Offset: 0x00015665
		// (set) Token: 0x06001FE1 RID: 8161 RVA: 0x0001746D File Offset: 0x0001566D
		public bool isHost { get; private set; }

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06001FE2 RID: 8162 RVA: 0x0009B2B4 File Offset: 0x000994B4
		// (remove) Token: 0x06001FE3 RID: 8163 RVA: 0x0009B2E8 File Offset: 0x000994E8
		public static event Action onStartHostGlobal;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06001FE4 RID: 8164 RVA: 0x0009B31C File Offset: 0x0009951C
		// (remove) Token: 0x06001FE5 RID: 8165 RVA: 0x0009B350 File Offset: 0x00099550
		public static event Action onStopHostGlobal;

		// Token: 0x06001FE6 RID: 8166 RVA: 0x00017476 File Offset: 0x00015676
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

		// Token: 0x06001FE7 RID: 8167 RVA: 0x00017494 File Offset: 0x00015694
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

		// Token: 0x06001FE8 RID: 8168 RVA: 0x0009B384 File Offset: 0x00099584
		[NetworkMessageHandler(client = true, server = false, msgType = 67)]
		private static void HandleKick(NetworkMessage netMsg)
		{
			GameNetworkManager.KickMessage kickMessage = netMsg.ReadMessage<GameNetworkManager.KickMessage>();
			Debug.LogFormat("Received kick message. Reason={0}", new object[]
			{
				kickMessage.reason
			});
			GameNetworkManager.singleton.StopClient();
			string displayToken = kickMessage.GetDisplayToken();
			SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
			simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair("DISCONNECTED", Array.Empty<object>());
			simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair(displayToken, Array.Empty<object>());
			simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
			simpleDialogBox.rootObject.transform.SetParent(RoR2Application.instance.mainCanvas.transform);
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x0009B424 File Offset: 0x00099624
		[NetworkMessageHandler(msgType = 54, client = true)]
		private static void HandleUpdateTime(NetworkMessage netMsg)
		{
			float unpredictedServerFixedTime = netMsg.reader.ReadSingle();
			GameNetworkManager.singleton.unpredictedServerFixedTime = unpredictedServerFixedTime;
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x0009B448 File Offset: 0x00099648
		[NetworkMessageHandler(msgType = 64, client = true, server = true)]
		private static void HandleTest(NetworkMessage netMsg)
		{
			int num = netMsg.reader.ReadInt32();
			Debug.LogFormat("Received test packet. value={0}", new object[]
			{
				num
			});
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x0009B47C File Offset: 0x0009967C
		[NetworkMessageHandler(msgType = 65, client = true, server = true)]
		private static void HandlePing(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			netMsg.conn.SendByChannel(66, reader.ReadMessage<GameNetworkManager.PingMessage>(), netMsg.channelId);
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x0009B4AC File Offset: 0x000996AC
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

		// Token: 0x06001FED RID: 8173 RVA: 0x000174B3 File Offset: 0x000156B3
		public static bool IsMemberInSteamLobby(CSteamID steamId)
		{
			return Client.Instance.Lobby.UserIsInCurrentLobby(steamId.value);
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x0009B4F4 File Offset: 0x000996F4
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

		// Token: 0x06001FEF RID: 8175 RVA: 0x0009B574 File Offset: 0x00099774
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

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0009B5F8 File Offset: 0x000997F8
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

		// Token: 0x06001FF1 RID: 8177 RVA: 0x0009B69C File Offset: 0x0009989C
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

		// Token: 0x06001FF2 RID: 8178 RVA: 0x0009B70C File Offset: 0x0009990C
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

		// Token: 0x06001FF3 RID: 8179 RVA: 0x0009B7D0 File Offset: 0x000999D0
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

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06001FF4 RID: 8180 RVA: 0x0009B85C File Offset: 0x00099A5C
		// (remove) Token: 0x06001FF5 RID: 8181 RVA: 0x0009B890 File Offset: 0x00099A90
		public static event Action onStartServerGlobal;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06001FF6 RID: 8182 RVA: 0x0009B8C4 File Offset: 0x00099AC4
		// (remove) Token: 0x06001FF7 RID: 8183 RVA: 0x0009B8F8 File Offset: 0x00099AF8
		public static event Action onStopServerGlobal;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x06001FF8 RID: 8184 RVA: 0x0009B92C File Offset: 0x00099B2C
		// (remove) Token: 0x06001FF9 RID: 8185 RVA: 0x0009B960 File Offset: 0x00099B60
		public static event Action<NetworkConnection> onServerConnectGlobal;

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x06001FFA RID: 8186 RVA: 0x0009B994 File Offset: 0x00099B94
		// (remove) Token: 0x06001FFB RID: 8187 RVA: 0x0009B9C8 File Offset: 0x00099BC8
		public static event Action<NetworkConnection> onServerDisconnectGlobal;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x06001FFC RID: 8188 RVA: 0x0009B9FC File Offset: 0x00099BFC
		// (remove) Token: 0x06001FFD RID: 8189 RVA: 0x0009BA30 File Offset: 0x00099C30
		public static event Action<string> onServerSceneChangedGlobal;

		// Token: 0x06001FFE RID: 8190 RVA: 0x0009BA64 File Offset: 0x00099C64
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

		// Token: 0x06001FFF RID: 8191 RVA: 0x000174CA File Offset: 0x000156CA
		private void UpdateSteamMapName(string sceneName)
		{
			if (this.steamworksServer != null)
			{
				this.steamworksServer.MapName = sceneName;
			}
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000174E0 File Offset: 0x000156E0
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

		// Token: 0x06002001 RID: 8193 RVA: 0x00017518 File Offset: 0x00015718
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

		// Token: 0x06002002 RID: 8194 RVA: 0x00017527 File Offset: 0x00015727
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

		// Token: 0x06002003 RID: 8195 RVA: 0x0009BBC8 File Offset: 0x00099DC8
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

		// Token: 0x06002004 RID: 8196 RVA: 0x00017536 File Offset: 0x00015736
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

		// Token: 0x06002005 RID: 8197 RVA: 0x0001756D File Offset: 0x0001576D
		public override void OnStopServer()
		{
			Action action = GameNetworkManager.onStopServerGlobal;
			if (action != null)
			{
				action();
			}
			base.OnStopServer();
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x00017585 File Offset: 0x00015785
		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			if (NetworkUser.readOnlyInstancesList.Count >= base.maxConnections)
			{
				this.ServerKickClient(conn, GameNetworkManager.KickReason.ServerFull);
				return;
			}
			Action<NetworkConnection> action = GameNetworkManager.onServerConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x0009BC70 File Offset: 0x00099E70
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

		// Token: 0x06002008 RID: 8200 RVA: 0x000175B9 File Offset: 0x000157B9
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

		// Token: 0x06002009 RID: 8201 RVA: 0x0009BD8C File Offset: 0x00099F8C
		private void ServerKickClient(NetworkConnection conn, GameNetworkManager.KickReason reason)
		{
			Debug.LogFormat("Kicking client on connection {0}: Reason {1}", new object[]
			{
				conn.connectionId,
				reason
			});
			conn.SendByChannel(67, new GameNetworkManager.KickMessage
			{
				reason = reason
			}, QosChannelIndex.defaultReliable.intVal);
			conn.FlushChannels();
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				if (reason == GameNetworkManager.KickReason.Ban)
				{
					this.steamIdBanList.Add(steamNetworkConnection.steamId.value);
				}
				steamNetworkConnection.ignore = true;
			}
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x0009BE10 File Offset: 0x0009A010
		private void ServerDisconnectClient(NetworkConnection conn)
		{
			Debug.LogFormat("Disconnecting client on connection {0}", new object[]
			{
				conn.connectionId
			});
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(33);
			networkWriter.FinishMessage();
			conn.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			this.ServerHandleClientDisconnect(conn);
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x000175E3 File Offset: 0x000157E3
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			this.OnServerAddPlayer(conn, playerControllerId, null);
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000175EE File Offset: 0x000157EE
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId, extraMessageReader);
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x0009BE68 File Offset: 0x0009A068
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

		// Token: 0x0600200E RID: 8206 RVA: 0x0009C080 File Offset: 0x0009A280
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

		// Token: 0x0600200F RID: 8207 RVA: 0x0009C10C File Offset: 0x0009A30C
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

		// Token: 0x06002010 RID: 8208 RVA: 0x0009C1DC File Offset: 0x0009A3DC
		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);
			if (Run.instance)
			{
				Run.instance.OnServerSceneChanged(sceneName);
			}
			Action<string> action = GameNetworkManager.onServerSceneChangedGlobal;
			if (action != null)
			{
				action(sceneName);
			}
			while (GameNetworkManager.clientsReadyDuringLevelTransition.Count > 0)
			{
				NetworkConnection networkConnection = GameNetworkManager.clientsReadyDuringLevelTransition.Dequeue();
				try
				{
					if (networkConnection.isConnected)
					{
						this.OnServerReady(networkConnection);
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("OnServerReady could not be called for client: {0}", new object[]
					{
						ex.Message
					});
				}
			}
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x0009C270 File Offset: 0x0009A470
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

		// Token: 0x06002012 RID: 8210 RVA: 0x0009C2DC File Offset: 0x0009A4DC
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

		// Token: 0x06002013 RID: 8211 RVA: 0x000175F9 File Offset: 0x000157F9
		public void RequestServerShutdown()
		{
			if (this.serverShuttingDown)
			{
				return;
			}
			this.serverShuttingDown = true;
			base.StartCoroutine(this.ServerShutdownCoroutine());
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x00017618 File Offset: 0x00015818
		private IEnumerator ServerShutdownCoroutine()
		{
			Debug.Log("Server shutting down...");
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					this.ServerKickClient(networkConnection, GameNetworkManager.KickReason.ServerShutdown);
				}
			}
			Debug.Log("Issued kick message to all remote clients.");
			float maxWait = 0.2f;
			float t = 0f;
			while (t < maxWait && !GameNetworkManager.<ServerShutdownCoroutine>g__CheckConnectionsEmpty|161_0())
			{
				yield return new WaitForEndOfFrame();
				t += Time.unscaledDeltaTime;
			}
			Debug.Log("Finished waiting for clients to disconnect.");
			if (this.client != null)
			{
				Debug.Log("StopHost()");
				base.StopHost();
			}
			else
			{
				Debug.Log("StopServer()");
				base.StopServer();
			}
			this.serverShuttingDown = false;
			Debug.Log("Server shutdown complete.");
			yield break;
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x00017627 File Offset: 0x00015827
		private static void ServerHandleReady(NetworkMessage netMsg)
		{
			if (GameNetworkManager.isLoadingScene)
			{
				GameNetworkManager.clientsReadyDuringLevelTransition.Enqueue(netMsg.conn);
				Debug.Log("Client readied during a level transition! Queuing their request.");
				return;
			}
			GameNetworkManager.singleton.OnServerReady(netMsg.conn);
			Debug.Log("Client ready.");
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x00017665 File Offset: 0x00015865
		private void RegisterServerOverrideMessages()
		{
			NetworkServer.RegisterHandler(35, new NetworkMessageDelegate(GameNetworkManager.ServerHandleReady));
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x0001767A File Offset: 0x0001587A
		public override void ServerChangeScene(string newSceneName)
		{
			this.RegisterServerOverrideMessages();
			base.ServerChangeScene(newSceneName);
		}

		// Token: 0x0400220D RID: 8717
		private static readonly FieldInfo loadingSceneAsyncFieldInfo;

		// Token: 0x0400220F RID: 8719
		private static readonly string[] spawnableFolders = new string[]
		{
			"CharacterBodies",
			"CharacterMasters",
			"Projectiles",
			"NetworkedObjects",
			"GameModes"
		};

		// Token: 0x04002211 RID: 8721
		public float debugServerTime;

		// Token: 0x04002212 RID: 8722
		public float debugRTT;

		// Token: 0x04002213 RID: 8723
		private bool actedUponDesiredHost;

		// Token: 0x04002214 RID: 8724
		private float lastDesiredHostSetTime = float.NegativeInfinity;

		// Token: 0x04002215 RID: 8725
		private GameNetworkManager.HostDescription _desiredHost;

		// Token: 0x0400221A RID: 8730
		private static bool wasFading = false;

		// Token: 0x0400221C RID: 8732
		private float rttVelocity;

		// Token: 0x0400221D RID: 8733
		public float filteredRTTSmoothDuration = 0.1f;

		// Token: 0x0400221F RID: 8735
		private static readonly string[] sceneWhiteList = new string[]
		{
			"title",
			"crystalworld",
			"logbook"
		};

		// Token: 0x04002223 RID: 8739
		private List<ulong> steamIdBanList = new List<ulong>();

		// Token: 0x04002229 RID: 8745
		public Server steamworksServer;

		// Token: 0x0400222A RID: 8746
		public float timeTransmitInterval = 0.0166666675f;

		// Token: 0x0400222B RID: 8747
		private float timeTransmitTimer;

		// Token: 0x0400222C RID: 8748
		private bool serverShuttingDown;

		// Token: 0x0400222D RID: 8749
		private static readonly Queue<NetworkConnection> clientsReadyDuringLevelTransition = new Queue<NetworkConnection>();

		// Token: 0x02000586 RID: 1414
		public struct HostDescription : IEquatable<GameNetworkManager.HostDescription>
		{
			// Token: 0x0600201A RID: 8218 RVA: 0x000176BD File Offset: 0x000158BD
			public HostDescription(CSteamID steamId)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Steam;
				this.steamId = steamId;
			}

			// Token: 0x0600201B RID: 8219 RVA: 0x000176D4 File Offset: 0x000158D4
			public HostDescription(AddressPortPair addressPortPair)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.IPv4;
				this.addressPortPair = addressPortPair;
			}

			// Token: 0x0600201C RID: 8220 RVA: 0x000176EB File Offset: 0x000158EB
			public HostDescription(GameNetworkManager.HostDescription.HostingParameters hostingParameters)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Self;
				this.hostingParameters = hostingParameters;
			}

			// Token: 0x0600201D RID: 8221 RVA: 0x0009C3B0 File Offset: 0x0009A5B0
			public bool DescribesCurrentHost()
			{
				switch (this.hostType)
				{
				case GameNetworkManager.HostDescription.HostType.None:
					return !GameNetworkManager.singleton.isNetworkActive;
				case GameNetworkManager.HostDescription.HostType.Self:
					return NetworkServer.active && this.hostingParameters.listen != NetworkServer.dontListen;
				case GameNetworkManager.HostDescription.HostType.Steam:
				{
					NetworkClient client = GameNetworkManager.singleton.client;
					SteamNetworkConnection steamNetworkConnection;
					return (steamNetworkConnection = (((client != null) ? client.connection : null) as SteamNetworkConnection)) != null && steamNetworkConnection.steamId == this.steamId;
				}
				case GameNetworkManager.HostDescription.HostType.IPv4:
				{
					NetworkClient client2 = GameNetworkManager.singleton.client;
					NetworkConnection networkConnection;
					return (networkConnection = ((client2 != null) ? client2.connection : null)) != null && networkConnection.address == this.addressPortPair.address;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			// Token: 0x0600201E RID: 8222 RVA: 0x00017702 File Offset: 0x00015902
			private HostDescription(object o)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.None;
			}

			// Token: 0x0600201F RID: 8223 RVA: 0x0009C47C File Offset: 0x0009A67C
			public bool Equals(GameNetworkManager.HostDescription other)
			{
				return this.hostType == other.hostType && this.steamId.Equals(other.steamId) && this.addressPortPair.Equals(other.addressPortPair) && this.hostingParameters.Equals(other.hostingParameters);
			}

			// Token: 0x06002020 RID: 8224 RVA: 0x0009C4DC File Offset: 0x0009A6DC
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is GameNetworkManager.HostDescription)
				{
					GameNetworkManager.HostDescription other = (GameNetworkManager.HostDescription)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06002021 RID: 8225 RVA: 0x0009C508 File Offset: 0x0009A708
			public override int GetHashCode()
			{
				return (int)(((this.hostType * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.steamId.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.addressPortPair.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.hostingParameters.GetHashCode());
			}

			// Token: 0x06002022 RID: 8226 RVA: 0x0009C56C File Offset: 0x0009A76C
			public override string ToString()
			{
				GameNetworkManager.HostDescription.sharedStringBuilder.Clear();
				GameNetworkManager.HostDescription.sharedStringBuilder.Append("{ hostType=").Append(this.hostType);
				switch (this.hostType)
				{
				case GameNetworkManager.HostDescription.HostType.None:
					break;
				case GameNetworkManager.HostDescription.HostType.Self:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" listen=").Append(this.hostingParameters.listen);
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" maxPlayers=").Append(this.hostingParameters.maxPlayers);
					break;
				case GameNetworkManager.HostDescription.HostType.Steam:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" steamId=").Append(this.steamId);
					break;
				case GameNetworkManager.HostDescription.HostType.IPv4:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" address=").Append(this.addressPortPair.address);
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" port=").Append(this.addressPortPair.port);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				GameNetworkManager.HostDescription.sharedStringBuilder.Append(" }");
				return GameNetworkManager.HostDescription.sharedStringBuilder.ToString();
			}

			// Token: 0x0400222E RID: 8750
			public readonly GameNetworkManager.HostDescription.HostType hostType;

			// Token: 0x0400222F RID: 8751
			public readonly CSteamID steamId;

			// Token: 0x04002230 RID: 8752
			public readonly AddressPortPair addressPortPair;

			// Token: 0x04002231 RID: 8753
			public readonly GameNetworkManager.HostDescription.HostingParameters hostingParameters;

			// Token: 0x04002232 RID: 8754
			public static readonly GameNetworkManager.HostDescription none = new GameNetworkManager.HostDescription(null);

			// Token: 0x04002233 RID: 8755
			private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

			// Token: 0x02000587 RID: 1415
			public enum HostType
			{
				// Token: 0x04002235 RID: 8757
				None,
				// Token: 0x04002236 RID: 8758
				Self,
				// Token: 0x04002237 RID: 8759
				Steam,
				// Token: 0x04002238 RID: 8760
				IPv4
			}

			// Token: 0x02000588 RID: 1416
			public struct HostingParameters : IEquatable<GameNetworkManager.HostDescription.HostingParameters>
			{
				// Token: 0x06002024 RID: 8228 RVA: 0x00017729 File Offset: 0x00015929
				public bool Equals(GameNetworkManager.HostDescription.HostingParameters other)
				{
					return this.listen == other.listen && this.maxPlayers == other.maxPlayers;
				}

				// Token: 0x06002025 RID: 8229 RVA: 0x0009C68C File Offset: 0x0009A88C
				public override bool Equals(object obj)
				{
					if (obj == null)
					{
						return false;
					}
					if (obj is GameNetworkManager.HostDescription.HostingParameters)
					{
						GameNetworkManager.HostDescription.HostingParameters other = (GameNetworkManager.HostDescription.HostingParameters)obj;
						return this.Equals(other);
					}
					return false;
				}

				// Token: 0x06002026 RID: 8230 RVA: 0x00017749 File Offset: 0x00015949
				public override int GetHashCode()
				{
					return this.listen.GetHashCode() * 397 ^ this.maxPlayers;
				}

				// Token: 0x04002239 RID: 8761
				public bool listen;

				// Token: 0x0400223A RID: 8762
				public int maxPlayers;
			}
		}

		// Token: 0x02000589 RID: 1417
		private class NetLogLevelConVar : BaseConVar
		{
			// Token: 0x06002027 RID: 8231 RVA: 0x000090CD File Offset: 0x000072CD
			public NetLogLevelConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002028 RID: 8232 RVA: 0x0009C6B8 File Offset: 0x0009A8B8
			public override void SetString(string newValue)
			{
				int currentLogLevel;
				if (TextSerialization.TryParseInvariant(newValue, out currentLogLevel))
				{
					LogFilter.currentLogLevel = currentLogLevel;
				}
			}

			// Token: 0x06002029 RID: 8233 RVA: 0x00017763 File Offset: 0x00015963
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(LogFilter.currentLogLevel);
			}

			// Token: 0x0400223B RID: 8763
			private static GameNetworkManager.NetLogLevelConVar cvNetLogLevel = new GameNetworkManager.NetLogLevelConVar("net_loglevel", ConVarFlags.Engine, null, "Network log verbosity.");
		}

		// Token: 0x0200058A RID: 1418
		private class SvListenConVar : BaseConVar
		{
			// Token: 0x0600202B RID: 8235 RVA: 0x000090CD File Offset: 0x000072CD
			public SvListenConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600202C RID: 8236 RVA: 0x0009C6D8 File Offset: 0x0009A8D8
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

			// Token: 0x0600202D RID: 8237 RVA: 0x00017788 File Offset: 0x00015988
			public override string GetString()
			{
				if (!NetworkServer.dontListen)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x0400223C RID: 8764
			private static GameNetworkManager.SvListenConVar cvSvListen = new GameNetworkManager.SvListenConVar("sv_listen", ConVarFlags.Engine, null, "Whether or not the server will accept connections from other players.");
		}

		// Token: 0x0200058B RID: 1419
		private class SvMaxPlayersConVar : BaseConVar
		{
			// Token: 0x0600202F RID: 8239 RVA: 0x000090CD File Offset: 0x000072CD
			public SvMaxPlayersConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002030 RID: 8240 RVA: 0x0009C70C File Offset: 0x0009A90C
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
					NetworkManager.singleton.maxConnections = Math.Min(Math.Max(val, 1), RoR2Application.hardMaxPlayers);
				}
			}

			// Token: 0x06002031 RID: 8241 RVA: 0x000177B5 File Offset: 0x000159B5
			public override string GetString()
			{
				if (!NetworkManager.singleton)
				{
					return "1";
				}
				return TextSerialization.ToStringInvariant(NetworkManager.singleton.maxConnections);
			}

			// Token: 0x170002CF RID: 719
			// (get) Token: 0x06002032 RID: 8242 RVA: 0x000177D8 File Offset: 0x000159D8
			public int intValue
			{
				get
				{
					return NetworkManager.singleton.maxConnections;
				}
			}

			// Token: 0x0400223D RID: 8765
			public static readonly GameNetworkManager.SvMaxPlayersConVar instance = new GameNetworkManager.SvMaxPlayersConVar("sv_maxplayers", ConVarFlags.Engine, null, "Maximum number of players allowed.");
		}

		// Token: 0x0200058C RID: 1420
		private class KickMessage : MessageBase
		{
			// Token: 0x170002D0 RID: 720
			// (get) Token: 0x06002034 RID: 8244 RVA: 0x000177FD File Offset: 0x000159FD
			// (set) Token: 0x06002035 RID: 8245 RVA: 0x00017805 File Offset: 0x00015A05
			public GameNetworkManager.KickReason reason
			{
				get
				{
					return (GameNetworkManager.KickReason)this.netReason;
				}
				set
				{
					this.netReason = (int)value;
				}
			}

			// Token: 0x06002036 RID: 8246 RVA: 0x0009C75C File Offset: 0x0009A95C
			public string GetDisplayToken()
			{
				switch (this.reason)
				{
				case GameNetworkManager.KickReason.ServerShutdown:
					return "KICK_REASON_SERVERSHUTDOWN";
				case GameNetworkManager.KickReason.Timeout:
					return "KICK_REASON_TIMEOUT";
				case GameNetworkManager.KickReason.Kick:
					return "KICK_REASON_KICK";
				case GameNetworkManager.KickReason.Ban:
					return "KICK_REASON_BAN";
				case GameNetworkManager.KickReason.BadPassword:
					return "KICK_REASON_BADPASSWORD";
				case GameNetworkManager.KickReason.BadVersion:
					return "KICK_REASON_BADVERSION";
				case GameNetworkManager.KickReason.ServerFull:
					return "KICK_REASON_SERVERFULL";
				default:
					return "KICK_REASON_UNSPECIFIED";
				}
			}

			// Token: 0x06002038 RID: 8248 RVA: 0x0001780E File Offset: 0x00015A0E
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.netReason);
			}

			// Token: 0x06002039 RID: 8249 RVA: 0x0001781C File Offset: 0x00015A1C
			public override void Deserialize(NetworkReader reader)
			{
				this.netReason = (int)reader.ReadPackedUInt32();
			}

			// Token: 0x0400223E RID: 8766
			public int netReason;
		}

		// Token: 0x0200058D RID: 1421
		protected class AddPlayerMessage : MessageBase
		{
			// Token: 0x0600203B RID: 8251 RVA: 0x0001782A File Offset: 0x00015A2A
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt64(this.steamId);
				writer.WriteBytesFull(this.steamAuthTicketData);
			}

			// Token: 0x0600203C RID: 8252 RVA: 0x00017844 File Offset: 0x00015A44
			public override void Deserialize(NetworkReader reader)
			{
				this.steamId = reader.ReadPackedUInt64();
				this.steamAuthTicketData = reader.ReadBytesAndSize();
			}

			// Token: 0x0400223F RID: 8767
			public ulong steamId;

			// Token: 0x04002240 RID: 8768
			public byte[] steamAuthTicketData;
		}

		// Token: 0x0200058E RID: 1422
		public enum KickReason
		{
			// Token: 0x04002242 RID: 8770
			Unspecified,
			// Token: 0x04002243 RID: 8771
			ServerShutdown,
			// Token: 0x04002244 RID: 8772
			Timeout,
			// Token: 0x04002245 RID: 8773
			Kick,
			// Token: 0x04002246 RID: 8774
			Ban,
			// Token: 0x04002247 RID: 8775
			BadPassword,
			// Token: 0x04002248 RID: 8776
			BadVersion,
			// Token: 0x04002249 RID: 8777
			ServerFull
		}

		// Token: 0x0200058F RID: 1423
		private class PingMessage : MessageBase
		{
			// Token: 0x0600203E RID: 8254 RVA: 0x0001785E File Offset: 0x00015A5E
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.timeStampMs);
			}

			// Token: 0x0600203F RID: 8255 RVA: 0x0001786C File Offset: 0x00015A6C
			public override void Deserialize(NetworkReader reader)
			{
				this.timeStampMs = reader.ReadPackedUInt32();
			}

			// Token: 0x0400224A RID: 8778
			public uint timeStampMs;
		}
	}
}
