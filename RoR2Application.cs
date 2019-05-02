using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Facepunch.Steamworks;
using Rewired;
using RoR2.ConVar;
using RoR2.Networking;
using SteamAPIValidator;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020003B5 RID: 949
	public class RoR2Application : MonoBehaviour
	{
		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x0000F4F9 File Offset: 0x0000D6F9
		// (set) Token: 0x06001428 RID: 5160 RVA: 0x0000F501 File Offset: 0x0000D701
		public Client steamworksClient { get; private set; }

		// Token: 0x06001429 RID: 5161 RVA: 0x0000F50A File Offset: 0x0000D70A
		public static string GetBuildId()
		{
			if (RoR2Application.isModded)
			{
				return "MOD";
			}
			return RoR2Application.steamBuildId;
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x0000F51E File Offset: 0x0000D71E
		// (set) Token: 0x0600142B RID: 5163 RVA: 0x0000F525 File Offset: 0x0000D725
		public static RoR2Application instance { get; private set; }

		// Token: 0x0600142C RID: 5164 RVA: 0x0006FDA4 File Offset: 0x0006DFA4
		private void Awake()
		{
			if (RoR2Application.maxPlayers != 4)
			{
				RoR2Application.isModded = true;
			}
			this.stopwatch.Start();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (RoR2Application.instance)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			RoR2Application.instance = this;
			if (!this.loaded)
			{
				this.OnLoad();
				this.loaded = true;
			}
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0000F52D File Offset: 0x0000D72D
		private void Start()
		{
			if (RoR2Application.instance == this && RoR2Application.onStart != null)
			{
				RoR2Application.onStart();
				RoR2Application.onStart = null;
			}
		}

		// Token: 0x0600142E RID: 5166
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtSetTimerResolution(int desiredResolution, bool setResolution, out int currentResolution);

		// Token: 0x0600142F RID: 5167
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtQueryTimerResolution(out int minimumResolution, out int maximumResolution, out int currentResolution);

		// Token: 0x06001430 RID: 5168 RVA: 0x0006FE08 File Offset: 0x0006E008
		private void Update()
		{
			if (RoR2Application.waitMsConVar.value >= 0)
			{
				Thread.Sleep(RoR2Application.waitMsConVar.value);
			}
			if (this.steamworksClient != null)
			{
				this.steamworksClient.Update();
			}
			Cursor.lockState = ((MPEventSystemManager.kbmEventSystem.isCursorVisible || MPEventSystemManager.combinedEventSystem.isCursorVisible) ? CursorLockMode.None : CursorLockMode.Locked);
			Cursor.visible = false;
			if (RoR2Application.onUpdate != null)
			{
				RoR2Application.onUpdate();
			}
			Action action = Interlocked.Exchange<Action>(ref RoR2Application.onNextUpdate, null);
			if (action != null)
			{
				action();
			}
			RoR2Application.timeTimers.Update(Time.deltaTime);
			RoR2Application.unscaledTimeTimers.Update(Time.unscaledDeltaTime);
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001431 RID: 5169 RVA: 0x0006FEB0 File Offset: 0x0006E0B0
		// (remove) Token: 0x06001432 RID: 5170 RVA: 0x0006FEE4 File Offset: 0x0006E0E4
		public static event Action onUpdate;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001433 RID: 5171 RVA: 0x0006FF18 File Offset: 0x0006E118
		// (remove) Token: 0x06001434 RID: 5172 RVA: 0x0006FF4C File Offset: 0x0006E14C
		public static event Action onFixedUpdate;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06001435 RID: 5173 RVA: 0x0006FF80 File Offset: 0x0006E180
		// (remove) Token: 0x06001436 RID: 5174 RVA: 0x0006FFB4 File Offset: 0x0006E1B4
		public static event Action onLateUpdate;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06001437 RID: 5175 RVA: 0x0006FFE8 File Offset: 0x0006E1E8
		// (remove) Token: 0x06001438 RID: 5176 RVA: 0x0007001C File Offset: 0x0006E21C
		public static event Action onNextUpdate;

		// Token: 0x06001439 RID: 5177 RVA: 0x0000F553 File Offset: 0x0000D753
		private void FixedUpdate()
		{
			Action action = RoR2Application.onFixedUpdate;
			if (action != null)
			{
				action();
			}
			RoR2Application.fixedTimeTimers.Update(Time.fixedDeltaTime);
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0000F574 File Offset: 0x0000D774
		private void LateUpdate()
		{
			Action action = RoR2Application.onLateUpdate;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600143B RID: 5179 RVA: 0x0000F585 File Offset: 0x0000D785
		// (set) Token: 0x0600143C RID: 5180 RVA: 0x0000F58C File Offset: 0x0000D78C
		public static FileSystem fileSystem { get; private set; }

		// Token: 0x0600143D RID: 5181 RVA: 0x00070050 File Offset: 0x0006E250
		private void OnLoad()
		{
			RoR2Application.UnitySystemConsoleRedirector.Redirect();
			if (File.Exists("steam_appid.txt"))
			{
				try
				{
					File.Delete("steam_appid.txt");
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
				}
				if (File.Exists("steam_appid.txt"))
				{
					Debug.Log("Cannot delete steam_appid.txt. Quitting...");
					Application.Quit();
					return;
				}
			}
			Config.ForUnity(Application.platform.ToString());
			this.steamworksClient = new Client(632360u);
			if (Client.RestartIfNecessary(632360u) || !this.steamworksClient.IsValid || !SteamApiValidator.IsValidSteamApiDll())
			{
				Debug.Log("Unable to initialize Facepunch.Steamworks.");
				Application.Quit();
				return;
			}
			if (!this.steamworksClient.App.IsSubscribed(632360u))
			{
				Debug.Log("Steam user not subscribed to app. Quitting...");
				Application.Quit();
				return;
			}
			RoR2Application.steamBuildId = TextSerialization.ToStringInvariant(this.steamworksClient.BuildId);
			this.steamworksAuthTicket = this.steamworksClient.Auth.GetAuthSessionTicket();
			SteamworksEventManager.Init(this.steamworksClient);
			this.steamworksAvailability.MakeAvailable();
			PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
			RoR2Application.fileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Application.dataPath), true);
			RoR2Application.cloudStorage = RoR2Application.fileSystem;
			RoR2Application.cloudStorage = new SteamworksRemoteStorageFileSystem();
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Rewired Input Manager"));
			ReInput.ControllerConnectedEvent += RoR2Application.AssignNewController;
			foreach (ControllerType controllerType in new ControllerType[]
			{
				ControllerType.Keyboard,
				ControllerType.Mouse,
				ControllerType.Joystick
			})
			{
				Controller[] controllers = ReInput.controllers.GetControllers(controllerType);
				if (controllers != null)
				{
					for (int j = 0; j < controllers.Length; j++)
					{
						RoR2Application.AssignNewController(controllers[j]);
					}
				}
			}
			this.stateManager.Initialize();
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/MPEventSystemManager"));
			UnityEngine.Object.Instantiate<GameObject>(this.networkManagerPrefab);
			if (UnityEngine.Object.FindObjectOfType<AkInitializer>())
			{
				Debug.LogError("Attempting to initialize wwise when AkInitializer already exists! This will cause a crash!");
				return;
			}
			this.wwiseGlobalInstance = UnityEngine.Object.Instantiate<GameObject>(this.wwiseGlobalPrefab);
			UnityEngine.Object.Instantiate<GameObject>(this.audioManagerPrefab);
			GameObject gameObject = new GameObject("Console");
			gameObject.AddComponent<SetDontDestroyOnLoad>();
			gameObject.AddComponent<Console>();
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode loadSceneMode)
			{
				Debug.LogFormat("Loaded scene {0} loadSceneMode={1}", new object[]
				{
					scene.name,
					loadSceneMode
				});
			};
			SceneManager.sceneUnloaded += delegate(Scene scene)
			{
				Debug.LogFormat("Unloaded scene {0}", new object[]
				{
					scene.name
				});
			};
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
			{
				Debug.LogFormat("Active scene changed from {0} to {1}", new object[]
				{
					oldScene.name,
					newScene.name
				});
			};
			SystemInitializerAttribute.Execute();
			UserProfile.LoadUserProfiles();
			if (RoR2Application.onLoad != null)
			{
				RoR2Application.onLoad();
				RoR2Application.onLoad = null;
			}
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00070314 File Offset: 0x0006E514
		private void ShutdownSteamworks()
		{
			if (this.steamworksClient != null)
			{
				if (GameNetworkManager.singleton)
				{
					GameNetworkManager.singleton.ForceCloseAllConnections();
				}
				Debug.Log("Shutting down Steamworks...");
				this.steamworksClient.Lobby.Leave();
				if (Server.Instance != null)
				{
					Server.Instance.Dispose();
				}
				this.steamworksClient.Update();
				this.steamworksClient.Dispose();
				this.steamworksClient = null;
				Debug.Log("Shut down Steamworks.");
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0000F594 File Offset: 0x0000D794
		private void OnDestroy()
		{
			this.ShutdownSteamworks();
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0000F59C File Offset: 0x0000D79C
		private void OnApplicationQuit()
		{
			UserProfile.HandleShutDown();
			if (Console.instance)
			{
				Console.instance.SaveArchiveConVars();
			}
			this.ShutdownSteamworks();
			bool isEditor = Application.isEditor;
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x0000F5C5 File Offset: 0x0000D7C5
		public static bool isInSinglePlayer
		{
			get
			{
				return NetworkServer.dontListen && LocalUserManager.readOnlyLocalUsersList.Count == 1;
			}
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x00070394 File Offset: 0x0006E594
		private static void AssignJoystickToAvailablePlayer(Controller controller)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (player.name != "PlayerMain" && player.controllers.joystickCount == 0 && !player.controllers.hasKeyboard && !player.controllers.hasMouse)
				{
					player.controllers.AddController(controller, false);
					return;
				}
			}
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0000F5DD File Offset: 0x0000D7DD
		private static void AssignNewController(ControllerStatusChangedEventArgs args)
		{
			RoR2Application.AssignNewController(ReInput.controllers.GetController(args.controllerType, args.controllerId));
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0000F5FA File Offset: 0x0000D7FA
		private static void AssignNewController(Controller controller)
		{
			ReInput.players.GetPlayer("PlayerMain").controllers.AddController(controller, false);
			if (controller.type == ControllerType.Joystick)
			{
				RoR2Application.AssignJoystickToAvailablePlayer(controller);
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06001445 RID: 5189 RVA: 0x0000F626 File Offset: 0x0000D826
		// (set) Token: 0x06001446 RID: 5190 RVA: 0x0000F62D File Offset: 0x0000D82D
		public static bool sessionCheatsEnabled { get; private set; }

		// Token: 0x06001447 RID: 5191 RVA: 0x0007040C File Offset: 0x0006E60C
		[ConCommand(commandName = "pause", flags = ConVarFlags.None, helpText = "Toggles game pause state.")]
		private static void CCTogglePause(ConCommandArgs args)
		{
			if (RoR2Application.instance.pauseScreenInstance)
			{
				UnityEngine.Object.Destroy(RoR2Application.instance.pauseScreenInstance);
				RoR2Application.instance.pauseScreenInstance = null;
				return;
			}
			if (NetworkManager.singleton.isNetworkActive)
			{
				RoR2Application.instance.pauseScreenInstance = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/PauseScreen"), RoR2Application.instance.transform);
			}
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x0000F635 File Offset: 0x0000D835
		[ConCommand(commandName = "quit", flags = ConVarFlags.None, helpText = "Close the application.")]
		private static void CCQuit(ConCommandArgs args)
		{
			Application.Quit();
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0000F63C File Offset: 0x0000D83C
		public static void IncrementActiveWriteCount()
		{
			Interlocked.Increment(ref RoR2Application.activeWriteCount);
			RoR2Application.saveIconAlpha = 2f;
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0000F655 File Offset: 0x0000D855
		public static void DecrementActiveWriteCount()
		{
			Interlocked.Decrement(ref RoR2Application.activeWriteCount);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0000F662 File Offset: 0x0000D862
		[RuntimeInitializeOnLoadMethod]
		private static void InitSaveIcon()
		{
			UnityEngine.UI.Image saveImage = RoR2Application.instance.mainCanvas.transform.Find("SafeArea/SaveIcon").GetComponent<UnityEngine.UI.Image>();
			RoR2Application.onUpdate += delegate()
			{
				UnityEngine.Color color = saveImage.color;
				if (RoR2Application.activeWriteCount <= 0)
				{
					color.a = (RoR2Application.saveIconAlpha = Mathf.Max(RoR2Application.saveIconAlpha - 4f * Time.unscaledDeltaTime, 0f));
				}
				saveImage.color = color;
			};
		}

		// Token: 0x040017DA RID: 6106
		[HideInInspector]
		[SerializeField]
		private bool loaded;

		// Token: 0x040017DB RID: 6107
		public static readonly string messageForModders = "We don't officially support modding at this time but if you're going to mod the game please change this value to true if you're modding the game. This will disable some things like Prismatic Trials and put players into a separate matchmaking queue from vanilla users to protect their game experience.";

		// Token: 0x040017DC RID: 6108
		public static bool isModded = false;

		// Token: 0x040017DD RID: 6109
		public GameObject networkManagerPrefab;

		// Token: 0x040017DE RID: 6110
		public GameObject wwiseGlobalPrefab;

		// Token: 0x040017DF RID: 6111
		public GameObject audioManagerPrefab;

		// Token: 0x040017E0 RID: 6112
		public EntityStateManager stateManager;

		// Token: 0x040017E1 RID: 6113
		public PostProcessVolume postProcessSettingsController;

		// Token: 0x040017E2 RID: 6114
		public Canvas mainCanvas;

		// Token: 0x040017E3 RID: 6115
		public Stopwatch stopwatch = new Stopwatch();

		// Token: 0x040017E4 RID: 6116
		public const string gameName = "Risk of Rain 2";

		// Token: 0x040017E5 RID: 6117
		private const uint ror2AppId = 632360u;

		// Token: 0x040017E6 RID: 6118
		public const uint appId = 632360u;

		// Token: 0x040017E8 RID: 6120
		public Auth.Ticket steamworksAuthTicket;

		// Token: 0x040017E9 RID: 6121
		public ResourceAvailability steamworksAvailability;

		// Token: 0x040017EA RID: 6122
		private static string steamBuildId = "STEAM_UNINITIALIZED";

		// Token: 0x040017EB RID: 6123
		public static readonly int hardMaxPlayers = 16;

		// Token: 0x040017EC RID: 6124
		public static readonly int maxPlayers = 4;

		// Token: 0x040017ED RID: 6125
		public static readonly int maxLocalPlayers = 4;

		// Token: 0x040017EE RID: 6126
		private GameObject wwiseGlobalInstance;

		// Token: 0x040017F0 RID: 6128
		private static IntConVar waitMsConVar = new IntConVar("wait_ms", ConVarFlags.None, "-1", "How many milliseconds to sleep between each frame. -1 for no sleeping between frames.");

		// Token: 0x040017F1 RID: 6129
		private GameObject pauseScreenInstance;

		// Token: 0x040017F2 RID: 6130
		public static readonly TimerQueue timeTimers = new TimerQueue();

		// Token: 0x040017F3 RID: 6131
		public static readonly TimerQueue fixedTimeTimers = new TimerQueue();

		// Token: 0x040017F4 RID: 6132
		public static readonly TimerQueue unscaledTimeTimers = new TimerQueue();

		// Token: 0x040017FA RID: 6138
		public static FileSystem cloudStorage;

		// Token: 0x040017FB RID: 6139
		public static Action onLoad;

		// Token: 0x040017FC RID: 6140
		public static Action onStart;

		// Token: 0x040017FD RID: 6141
		private const bool isRewiredFixedYet = true;

		// Token: 0x040017FF RID: 6143
		public static RoR2Application.CheatsConVar cvCheats = new RoR2Application.CheatsConVar("cheats", ConVarFlags.ExecuteOnServer, "0", "Enable cheats. Achievements, unlock progression, and stat tracking will be disabled until the application is restarted.");

		// Token: 0x04001800 RID: 6144
		private static float oldTimeScale = 1f;

		// Token: 0x04001801 RID: 6145
		public static Action onPauseStartGlobal;

		// Token: 0x04001802 RID: 6146
		public static Action onPauseEndGlobal;

		// Token: 0x04001803 RID: 6147
		private static RoR2Application.TimeScaleConVar cvTimeScale = new RoR2Application.TimeScaleConVar("timescale", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timescale of the game.");

		// Token: 0x04001804 RID: 6148
		private static RoR2Application.TimeStepConVar cvTimeStep = new RoR2Application.TimeStepConVar("timestep", ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat | ConVarFlags.Engine, null, "The timestep of the game.");

		// Token: 0x04001805 RID: 6149
		public static readonly Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);

		// Token: 0x04001806 RID: 6150
		public static BoolConVar enableDamageNumbers = new BoolConVar("enable_damage_numbers", ConVarFlags.None, "1", "Whether or not damage and healing numbers spawn.");

		// Token: 0x04001807 RID: 6151
		private static int activeWriteCount;

		// Token: 0x04001808 RID: 6152
		private static volatile float saveIconAlpha = 0f;

		// Token: 0x020003B6 RID: 950
		private class TimerResolutionConVar : BaseConVar
		{
			// Token: 0x0600144E RID: 5198 RVA: 0x000090CD File Offset: 0x000072CD
			private TimerResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600144F RID: 5199 RVA: 0x00070578 File Offset: 0x0006E778
			public override void SetString(string newValue)
			{
				int desiredResolution;
				if (TextSerialization.TryParseInvariant(newValue, out desiredResolution))
				{
					int num;
					RoR2Application.NtSetTimerResolution(desiredResolution, true, out num);
					Debug.LogFormat("{0} set to {1}", new object[]
					{
						this.name,
						num
					});
				}
			}

			// Token: 0x06001450 RID: 5200 RVA: 0x000705BC File Offset: 0x0006E7BC
			public override string GetString()
			{
				int num;
				int num2;
				int value;
				RoR2Application.NtQueryTimerResolution(out num, out num2, out value);
				return TextSerialization.ToStringInvariant(value);
			}

			// Token: 0x04001809 RID: 6153
			private static RoR2Application.TimerResolutionConVar instance = new RoR2Application.TimerResolutionConVar("timer_resolution", ConVarFlags.Engine, null, "The Windows timer resolution.");
		}

		// Token: 0x020003B7 RID: 951
		public class CheatsConVar : BaseConVar
		{
			// Token: 0x06001452 RID: 5202 RVA: 0x000090CD File Offset: 0x000072CD
			public CheatsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06001453 RID: 5203 RVA: 0x0000F6C9 File Offset: 0x0000D8C9
			// (set) Token: 0x06001454 RID: 5204 RVA: 0x0000F6D1 File Offset: 0x0000D8D1
			public bool boolValue
			{
				get
				{
					return this._boolValue;
				}
				private set
				{
					if (this._boolValue)
					{
						RoR2Application.sessionCheatsEnabled = true;
					}
				}
			}

			// Token: 0x06001455 RID: 5205 RVA: 0x000705DC File Offset: 0x0006E7DC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					this.boolValue = (num != 0);
				}
			}

			// Token: 0x06001456 RID: 5206 RVA: 0x0000F6E1 File Offset: 0x0000D8E1
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x0400180A RID: 6154
			private bool _boolValue;
		}

		// Token: 0x020003B8 RID: 952
		private class TimeScaleConVar : BaseConVar
		{
			// Token: 0x06001457 RID: 5207 RVA: 0x000090CD File Offset: 0x000072CD
			public TimeScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001458 RID: 5208 RVA: 0x00070600 File Offset: 0x0006E800
			public override void SetString(string newValue)
			{
				float timeScale;
				if (TextSerialization.TryParseInvariant(newValue, out timeScale))
				{
					Time.timeScale = timeScale;
				}
			}

			// Token: 0x06001459 RID: 5209 RVA: 0x0000F6F6 File Offset: 0x0000D8F6
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.timeScale);
			}
		}

		// Token: 0x020003B9 RID: 953
		private class TimeStepConVar : BaseConVar
		{
			// Token: 0x0600145A RID: 5210 RVA: 0x000090CD File Offset: 0x000072CD
			public TimeStepConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600145B RID: 5211 RVA: 0x00070620 File Offset: 0x0006E820
			public override void SetString(string newValue)
			{
				float fixedDeltaTime;
				if (TextSerialization.TryParseInvariant(newValue, out fixedDeltaTime))
				{
					Time.fixedDeltaTime = fixedDeltaTime;
				}
			}

			// Token: 0x0600145C RID: 5212 RVA: 0x0000F702 File Offset: 0x0000D902
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Time.fixedDeltaTime);
			}
		}

		// Token: 0x020003BA RID: 954
		private class SyncPhysicsConVar : BaseConVar
		{
			// Token: 0x0600145D RID: 5213 RVA: 0x000090CD File Offset: 0x000072CD
			private SyncPhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600145E RID: 5214 RVA: 0x00070640 File Offset: 0x0006E840
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (Physics.autoSyncTransforms != flag)
					{
						Physics.autoSyncTransforms = flag;
					}
				}
			}

			// Token: 0x0600145F RID: 5215 RVA: 0x0000F70E File Offset: 0x0000D90E
			public override string GetString()
			{
				if (!Physics.autoSyncTransforms)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x0400180B RID: 6155
			public static RoR2Application.SyncPhysicsConVar instance = new RoR2Application.SyncPhysicsConVar("sync_physics", ConVarFlags.None, "0", "Enable/disables Physics 'autosyncing' between moves.");
		}

		// Token: 0x020003BB RID: 955
		private class AutoSimulatePhysicsConVar : BaseConVar
		{
			// Token: 0x06001461 RID: 5217 RVA: 0x000090CD File Offset: 0x000072CD
			private AutoSimulatePhysicsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001462 RID: 5218 RVA: 0x0007066C File Offset: 0x0006E86C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (flag != Physics.autoSimulation)
					{
						Physics.autoSimulation = flag;
					}
				}
			}

			// Token: 0x06001463 RID: 5219 RVA: 0x0000F73E File Offset: 0x0000D93E
			public override string GetString()
			{
				if (!Physics.autoSimulation)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x0400180C RID: 6156
			public static RoR2Application.AutoSimulatePhysicsConVar instance = new RoR2Application.AutoSimulatePhysicsConVar("auto_simulate_physics", ConVarFlags.None, "1", "Enable/disables Physics autosimulate.");
		}

		// Token: 0x020003BC RID: 956
		private static class UnitySystemConsoleRedirector
		{
			// Token: 0x06001465 RID: 5221 RVA: 0x0000F76E File Offset: 0x0000D96E
			public static void Redirect()
			{
				Console.SetOut(new RoR2Application.UnitySystemConsoleRedirector.OutWriter());
				Console.SetError(new RoR2Application.UnitySystemConsoleRedirector.ErrorWriter());
			}

			// Token: 0x020003BD RID: 957
			private class OutWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001466 RID: 5222 RVA: 0x0000F784 File Offset: 0x0000D984
				public override void WriteBufferToUnity(string str)
				{
					Debug.Log(str);
				}
			}

			// Token: 0x020003BE RID: 958
			private class ErrorWriter : RoR2Application.UnitySystemConsoleRedirector.UnityTextWriter
			{
				// Token: 0x06001468 RID: 5224 RVA: 0x0000F794 File Offset: 0x0000D994
				public override void WriteBufferToUnity(string str)
				{
					Debug.LogError(str);
				}
			}

			// Token: 0x020003BF RID: 959
			private abstract class UnityTextWriter : TextWriter
			{
				// Token: 0x0600146A RID: 5226 RVA: 0x0000F79C File Offset: 0x0000D99C
				public override void Flush()
				{
					this.WriteBufferToUnity(this.buffer.ToString());
					this.buffer.Length = 0;
				}

				// Token: 0x0600146B RID: 5227
				public abstract void WriteBufferToUnity(string str);

				// Token: 0x0600146C RID: 5228 RVA: 0x00070698 File Offset: 0x0006E898
				public override void Write(string value)
				{
					this.buffer.Append(value);
					if (value != null)
					{
						int length = value.Length;
						if (length > 0 && value[length - 1] == '\n')
						{
							this.Flush();
						}
					}
				}

				// Token: 0x0600146D RID: 5229 RVA: 0x0000F7BB File Offset: 0x0000D9BB
				public override void Write(char value)
				{
					this.buffer.Append(value);
					if (value == '\n')
					{
						this.Flush();
					}
				}

				// Token: 0x0600146E RID: 5230 RVA: 0x0000F7D5 File Offset: 0x0000D9D5
				public override void Write(char[] value, int index, int count)
				{
					this.Write(new string(value, index, count));
				}

				// Token: 0x170001CA RID: 458
				// (get) Token: 0x0600146F RID: 5231 RVA: 0x0000F7E5 File Offset: 0x0000D9E5
				public override Encoding Encoding
				{
					get
					{
						return Encoding.Default;
					}
				}

				// Token: 0x0400180D RID: 6157
				private StringBuilder buffer = new StringBuilder();
			}
		}
	}
}
