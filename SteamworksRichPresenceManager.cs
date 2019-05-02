using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020004AB RID: 1195
	internal static class SteamworksRichPresenceManager
	{
		// Token: 0x06001B03 RID: 6915 RVA: 0x00013EDD File Offset: 0x000120DD
		private static void SetKeyValue([NotNull] string key, [CanBeNull] string value)
		{
			Client.Instance.User.SetRichPresence(key, value);
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x00087B94 File Offset: 0x00085D94
		private static void OnNetworkStart()
		{
			string text = null;
			CSteamID csteamID = CSteamID.nil;
			CSteamID csteamID2 = CSteamID.nil;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				string a = commandLineArgs[i].ToLower();
				CSteamID csteamID4;
				if (a == "+connect")
				{
					AddressPortPair addressPortPair;
					if (AddressPortPair.TryParse(commandLineArgs[i + 1], out addressPortPair))
					{
						text = addressPortPair.address + ":" + addressPortPair.port;
					}
				}
				else if (a == "+connect_steamworks_p2p")
				{
					CSteamID csteamID3;
					if (CSteamID.TryParse(commandLineArgs[i + 1], out csteamID3))
					{
						csteamID = csteamID3;
					}
				}
				else if (a == "+steam_lobby_join" && CSteamID.TryParse(commandLineArgs[i + 1], out csteamID4))
				{
					csteamID2 = csteamID4;
				}
			}
			if (csteamID2 != CSteamID.nil)
			{
				Console.instance.SubmitCmd(null, "steam_lobby_join " + csteamID2.value, false);
				return;
			}
			if (csteamID != CSteamID.nil)
			{
				Console.instance.SubmitCmd(null, "connect_steamworks_p2p " + csteamID.value, false);
				return;
			}
			if (text != null)
			{
				Console.instance.SubmitCmd(null, "connect " + text, false);
			}
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x00087CD4 File Offset: 0x00085ED4
		private static void OnLobbyChanged()
		{
			if (Client.Instance.Lobby.IsValid)
			{
				SteamworksRichPresenceManager.SetKeyValue("connect", "+steam_lobby_join " + Client.Instance.Lobby.CurrentLobby);
				return;
			}
			SteamworksRichPresenceManager.SetKeyValue("connect", null);
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x00087D28 File Offset: 0x00085F28
		private static void OnInvitedToGame(SteamFriend steamFriend, string connectString)
		{
			Debug.LogFormat("OnGameRichPresenceJoinRequested connectString=\"{0}\" steamFriend=\"{1}\"", new object[]
			{
				connectString,
				steamFriend.Name
			});
			string[] array = connectString.Split(new char[]
			{
				' '
			});
			if (array.Length >= 2)
			{
				CSteamID csteamID;
				if (array[0] == "+connect_steamworks_p2p" && CSteamID.TryParse(array[1], out csteamID))
				{
					if (!SteamworksLobbyManager.ownsLobby)
					{
						SteamworksLobbyManager.LeaveLobby();
					}
					Console.instance.SubmitCmd(null, "connect_steamworks_p2p " + csteamID.value, false);
				}
				CSteamID csteamID2;
				if (array[0] == "+steam_lobby_join" && CSteamID.TryParse(array[1], out csteamID2))
				{
					if (!SteamworksLobbyManager.ownsLobby)
					{
						SteamworksLobbyManager.LeaveLobby();
					}
					Console.instance.SubmitCmd(null, "steam_lobby_join " + csteamID2.value, false);
				}
			}
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x00013EF1 File Offset: 0x000120F1
		private static void OnGameServerChangeRequested(string address, string password)
		{
			Debug.LogFormat("OnGameServerChangeRequested address=\"{0}\"", new object[]
			{
				address
			});
			if (!SteamworksLobbyManager.ownsLobby)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			Console.instance.SubmitCmd(null, string.Format("connect \"{0}\"", address), false);
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00087E00 File Offset: 0x00086000
		private static void SetupCallbacks()
		{
			GameNetworkManager.onStartGlobal += SteamworksRichPresenceManager.OnNetworkStart;
			SteamworksLobbyManager.onLobbyChanged += SteamworksRichPresenceManager.OnLobbyChanged;
			if (Client.Instance != null)
			{
				Client.Instance.Friends.OnInvitedToGame += SteamworksRichPresenceManager.OnInvitedToGame;
				Client.Instance.Friends.OnGameServerChangeRequested = new Action<string, string>(SteamworksRichPresenceManager.OnGameServerChangeRequested);
			}
			new SteamworksRichPresenceManager.DifficultyField().Install();
			new SteamworksRichPresenceManager.GameModeField().Install();
			new SteamworksRichPresenceManager.ParticipationField().Install();
			new SteamworksRichPresenceManager.MinutesField().Install();
			new SteamworksRichPresenceManager.SteamPlayerGroupField().Install();
			new SteamworksRichPresenceManager.SteamDisplayField().Install();
			RoR2Application.onUpdate += SteamworksRichPresenceManager.BaseRichPresenceField.ProcessDirtyFields;
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00013F2A File Offset: 0x0001212A
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(SteamworksRichPresenceManager.SetupCallbacks));
		}

		// Token: 0x04001DB8 RID: 7608
		private const string rpConnect = "connect";

		// Token: 0x04001DB9 RID: 7609
		private const string rpStatus = "status";

		// Token: 0x04001DBA RID: 7610
		private const string rpSteamDisplay = "steam_display";

		// Token: 0x04001DBB RID: 7611
		private const string rpSteamPlayerGroup = "steam_player_group";

		// Token: 0x04001DBC RID: 7612
		private const string rpSteamPlayerGroupSize = "steam_player_group_size";

		// Token: 0x04001DBD RID: 7613
		private const string rpDifficulty = "difficulty";

		// Token: 0x04001DBE RID: 7614
		private const string rpGameMode = "gamemode";

		// Token: 0x04001DBF RID: 7615
		private const string rpParticipationType = "participation_type";

		// Token: 0x04001DC0 RID: 7616
		private const string rpMinutes = "minutes";

		// Token: 0x020004AC RID: 1196
		private abstract class BaseRichPresenceField
		{
			// Token: 0x06001B0A RID: 6922 RVA: 0x00013F4C File Offset: 0x0001214C
			public static void ProcessDirtyFields()
			{
				while (SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Count > 0)
				{
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Dequeue().UpdateIfNecessary();
				}
			}

			// Token: 0x1700027C RID: 636
			// (get) Token: 0x06001B0B RID: 6923
			protected abstract string key { get; }

			// Token: 0x06001B0C RID: 6924
			[CanBeNull]
			protected abstract string RebuildValue();

			// Token: 0x06001B0D RID: 6925 RVA: 0x000025F6 File Offset: 0x000007F6
			protected virtual void OnChanged()
			{
			}

			// Token: 0x06001B0E RID: 6926 RVA: 0x00013F6C File Offset: 0x0001216C
			public void SetDirty()
			{
				if (!this.isDirty)
				{
					this.isDirty = true;
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Enqueue(this);
				}
			}

			// Token: 0x06001B0F RID: 6927 RVA: 0x00087EBC File Offset: 0x000860BC
			private void UpdateIfNecessary()
			{
				if (!this.installed)
				{
					return;
				}
				this.isDirty = false;
				string a = this.RebuildValue();
				if (a != this.currentValue)
				{
					this.currentValue = a;
					SteamworksRichPresenceManager.SetKeyValue(this.key, this.currentValue);
					this.OnChanged();
				}
			}

			// Token: 0x06001B10 RID: 6928 RVA: 0x000025F6 File Offset: 0x000007F6
			protected virtual void OnInstall()
			{
			}

			// Token: 0x06001B11 RID: 6929 RVA: 0x000025F6 File Offset: 0x000007F6
			protected virtual void OnUninstall()
			{
			}

			// Token: 0x06001B12 RID: 6930 RVA: 0x00013F88 File Offset: 0x00012188
			public void Install()
			{
				if (!this.installed)
				{
					this.OnInstall();
					this.SetDirty();
					this.installed = true;
				}
			}

			// Token: 0x06001B13 RID: 6931 RVA: 0x00013FA5 File Offset: 0x000121A5
			public void Uninstall()
			{
				if (this.installed)
				{
					this.OnUninstall();
					this.installed = false;
					SteamworksRichPresenceManager.SetKeyValue(this.key, null);
				}
			}

			// Token: 0x06001B14 RID: 6932 RVA: 0x00013FC8 File Offset: 0x000121C8
			protected void SetDirtyableValue<T>(ref T field, T value) where T : struct, IEquatable<T>
			{
				if (!field.Equals(value))
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x06001B15 RID: 6933 RVA: 0x00013FE6 File Offset: 0x000121E6
			protected void SetDirtyableReference<T>(ref T field, T value) where T : class
			{
				if (field != value)
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x04001DC1 RID: 7617
			private static readonly Queue<SteamworksRichPresenceManager.BaseRichPresenceField> dirtyFields = new Queue<SteamworksRichPresenceManager.BaseRichPresenceField>();

			// Token: 0x04001DC2 RID: 7618
			private bool isDirty;

			// Token: 0x04001DC3 RID: 7619
			[CanBeNull]
			private string currentValue;

			// Token: 0x04001DC4 RID: 7620
			private bool installed;
		}

		// Token: 0x020004AD RID: 1197
		private sealed class DifficultyField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001B18 RID: 6936 RVA: 0x00014014 File Offset: 0x00012214
			protected override string key
			{
				get
				{
					return "difficulty";
				}
			}

			// Token: 0x06001B19 RID: 6937 RVA: 0x00087F0C File Offset: 0x0008610C
			protected override string RebuildValue()
			{
				if (!Run.instance)
				{
					return null;
				}
				switch (Run.instance.selectedDifficulty)
				{
				case DifficultyIndex.Easy:
					return "Easy";
				case DifficultyIndex.Normal:
					return "Normal";
				case DifficultyIndex.Hard:
					return "Hard";
				default:
					return null;
				}
			}

			// Token: 0x06001B1A RID: 6938 RVA: 0x0001401B File Offset: 0x0001221B
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B1B RID: 6939 RVA: 0x00014023 File Offset: 0x00012223
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B1C RID: 6940 RVA: 0x0001404D File Offset: 0x0001224D
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004AE RID: 1198
		private sealed class GameModeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700027E RID: 638
			// (get) Token: 0x06001B1E RID: 6942 RVA: 0x0001407F File Offset: 0x0001227F
			protected override string key
			{
				get
				{
					return "gamemode";
				}
			}

			// Token: 0x06001B1F RID: 6943 RVA: 0x00014086 File Offset: 0x00012286
			protected override string RebuildValue()
			{
				if (!Run.instance)
				{
					return null;
				}
				Run run = GameModeCatalog.FindGameModePrefabComponent(Run.instance.name);
				if (run == null)
				{
					return null;
				}
				return run.name;
			}

			// Token: 0x06001B20 RID: 6944 RVA: 0x0001401B File Offset: 0x0001221B
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B21 RID: 6945 RVA: 0x000140B0 File Offset: 0x000122B0
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B22 RID: 6946 RVA: 0x000140DA File Offset: 0x000122DA
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004AF RID: 1199
		private sealed class ParticipationField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700027F RID: 639
			// (get) Token: 0x06001B24 RID: 6948 RVA: 0x00014104 File Offset: 0x00012304
			protected override string key
			{
				get
				{
					return "participation_type";
				}
			}

			// Token: 0x06001B25 RID: 6949 RVA: 0x0001410B File Offset: 0x0001230B
			private void SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType newParticipationType)
			{
				if (this.participationType != newParticipationType)
				{
					this.participationType = newParticipationType;
					base.SetDirty();
				}
			}

			// Token: 0x06001B26 RID: 6950 RVA: 0x00087F5C File Offset: 0x0008615C
			protected override string RebuildValue()
			{
				switch (this.participationType)
				{
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive:
					return "Alive";
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead:
					return "Dead";
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator:
					return "Spectator";
				default:
					return null;
				}
			}

			// Token: 0x06001B27 RID: 6951 RVA: 0x00087F9C File Offset: 0x0008619C
			protected override void OnInstall()
			{
				base.OnInstall();
				LocalUserManager.onUserSignIn += this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut += this.OnLocalUserLost;
				Run.onRunStartGlobal += this.OnRunStart;
				Run.onRunDestroyGlobal += this.OnRunDestroy;
			}

			// Token: 0x06001B28 RID: 6952 RVA: 0x00087FF4 File Offset: 0x000861F4
			protected override void OnUninstall()
			{
				LocalUserManager.onUserSignIn -= this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut -= this.OnLocalUserLost;
				Run.onRunStartGlobal -= this.OnRunStart;
				Run.onRunDestroyGlobal -= this.OnRunDestroy;
				this.SetCurrentMaster(null);
			}

			// Token: 0x06001B29 RID: 6953 RVA: 0x0008804C File Offset: 0x0008624C
			private void SetTrackedUser(LocalUser newTrackedUser)
			{
				if (this.trackedUser != null)
				{
					this.trackedUser.onMasterChanged -= this.OnMasterChanged;
				}
				this.trackedUser = newTrackedUser;
				if (this.trackedUser != null)
				{
					this.trackedUser.onMasterChanged += this.OnMasterChanged;
				}
			}

			// Token: 0x06001B2A RID: 6954 RVA: 0x00014123 File Offset: 0x00012323
			private void OnLocalUserDiscovered(LocalUser localUser)
			{
				if (this.trackedUser == null)
				{
					this.SetTrackedUser(localUser);
				}
			}

			// Token: 0x06001B2B RID: 6955 RVA: 0x00014134 File Offset: 0x00012334
			private void OnLocalUserLost(LocalUser localUser)
			{
				if (this.trackedUser == localUser)
				{
					this.SetTrackedUser(null);
				}
			}

			// Token: 0x06001B2C RID: 6956 RVA: 0x00014146 File Offset: 0x00012346
			private void OnRunStart(Run run)
			{
				if (this.trackedUser != null && !this.trackedUser.cachedMasterObject)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator);
				}
			}

			// Token: 0x06001B2D RID: 6957 RVA: 0x00014169 File Offset: 0x00012369
			private void OnRunDestroy(Run run)
			{
				if (this.trackedUser != null)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.None);
				}
			}

			// Token: 0x06001B2E RID: 6958 RVA: 0x000880A0 File Offset: 0x000862A0
			private void OnMasterChanged()
			{
				PlayerCharacterMasterController cachedMasterController = this.trackedUser.cachedMasterController;
				this.SetCurrentMaster(cachedMasterController ? cachedMasterController.master : null);
			}

			// Token: 0x06001B2F RID: 6959 RVA: 0x000880D0 File Offset: 0x000862D0
			private void SetCurrentMaster(CharacterMaster newMaster)
			{
				if (this.currentMaster != null)
				{
					this.currentMaster.onBodyDeath.RemoveListener(new UnityAction(this.OnBodyDeath));
					this.currentMaster.onBodyStart -= this.OnBodyStart;
				}
				this.currentMaster = newMaster;
				if (this.currentMaster != null)
				{
					this.currentMaster.onBodyDeath.AddListener(new UnityAction(this.OnBodyDeath));
					this.currentMaster.onBodyStart += this.OnBodyStart;
				}
			}

			// Token: 0x06001B30 RID: 6960 RVA: 0x0001417A File Offset: 0x0001237A
			private void OnBodyDeath()
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead);
			}

			// Token: 0x06001B31 RID: 6961 RVA: 0x00014183 File Offset: 0x00012383
			private void OnBodyStart(CharacterBody body)
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive);
			}

			// Token: 0x04001DC5 RID: 7621
			private SteamworksRichPresenceManager.ParticipationField.ParticipationType participationType;

			// Token: 0x04001DC6 RID: 7622
			private LocalUser trackedUser;

			// Token: 0x04001DC7 RID: 7623
			private CharacterMaster currentMaster;

			// Token: 0x020004B0 RID: 1200
			private enum ParticipationType
			{
				// Token: 0x04001DC9 RID: 7625
				None,
				// Token: 0x04001DCA RID: 7626
				Alive,
				// Token: 0x04001DCB RID: 7627
				Dead,
				// Token: 0x04001DCC RID: 7628
				Spectator
			}
		}

		// Token: 0x020004B1 RID: 1201
		private sealed class MinutesField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000280 RID: 640
			// (get) Token: 0x06001B33 RID: 6963 RVA: 0x0001418C File Offset: 0x0001238C
			protected override string key
			{
				get
				{
					return "minutes";
				}
			}

			// Token: 0x06001B34 RID: 6964 RVA: 0x00014193 File Offset: 0x00012393
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.minutes);
			}

			// Token: 0x06001B35 RID: 6965 RVA: 0x0008815C File Offset: 0x0008635C
			private void FixedUpdate()
			{
				uint value = 0u;
				if (Run.instance)
				{
					value = (uint)Mathf.FloorToInt(Run.instance.fixedTime / 60f);
				}
				base.SetDirtyableValue<uint>(ref this.minutes, value);
			}

			// Token: 0x06001B36 RID: 6966 RVA: 0x000141A0 File Offset: 0x000123A0
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.FixedUpdate;
			}

			// Token: 0x06001B37 RID: 6967 RVA: 0x000141B9 File Offset: 0x000123B9
			protected override void OnUninstall()
			{
				RoR2Application.onFixedUpdate -= this.FixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x04001DCD RID: 7629
			private uint minutes;
		}

		// Token: 0x020004B2 RID: 1202
		private sealed class SteamPlayerGroupField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000281 RID: 641
			// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000141D2 File Offset: 0x000123D2
			protected override string key
			{
				get
				{
					return "steam_player_group";
				}
			}

			// Token: 0x06001B3A RID: 6970 RVA: 0x000141D9 File Offset: 0x000123D9
			private void SetLobbyId(CSteamID newLobbyId)
			{
				if (this.lobbyId != newLobbyId)
				{
					this.lobbyId = newLobbyId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B3B RID: 6971 RVA: 0x000141F6 File Offset: 0x000123F6
			private void SetHostId(CSteamID newHostId)
			{
				if (this.hostId != newHostId)
				{
					this.hostId = newHostId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B3C RID: 6972 RVA: 0x00014213 File Offset: 0x00012413
			private void SetGroupId(CSteamID newGroupId)
			{
				if (this.groupId != newGroupId)
				{
					this.groupId = newGroupId;
					base.SetDirty();
				}
			}

			// Token: 0x06001B3D RID: 6973 RVA: 0x0008819C File Offset: 0x0008639C
			private void UpdateGroupID()
			{
				if (this.hostId != CSteamID.nil)
				{
					this.SetGroupId(this.hostId);
					if (!(this.groupSizeField is SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldGame))
					{
						SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField = this.groupSizeField;
						if (steamPlayerGroupSizeField != null)
						{
							steamPlayerGroupSizeField.Uninstall();
						}
						this.groupSizeField = new SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldGame();
						this.groupSizeField.Install();
						return;
					}
				}
				else
				{
					this.SetGroupId(this.lobbyId);
					if (!(this.groupSizeField is SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldLobby))
					{
						SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField2 = this.groupSizeField;
						if (steamPlayerGroupSizeField2 != null)
						{
							steamPlayerGroupSizeField2.Uninstall();
						}
						this.groupSizeField = new SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldLobby();
						this.groupSizeField.Install();
					}
				}
			}

			// Token: 0x06001B3E RID: 6974 RVA: 0x0008823C File Offset: 0x0008643C
			protected override void OnInstall()
			{
				base.OnInstall();
				GameNetworkManager.onClientConnectGlobal += this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal += this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal += this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal += this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged += this.OnLobbyChanged;
			}

			// Token: 0x06001B3F RID: 6975 RVA: 0x000882A4 File Offset: 0x000864A4
			protected override void OnUninstall()
			{
				GameNetworkManager.onClientConnectGlobal -= this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal -= this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal -= this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal -= this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged -= this.OnLobbyChanged;
				SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField = this.groupSizeField;
				if (steamPlayerGroupSizeField != null)
				{
					steamPlayerGroupSizeField.Uninstall();
				}
				this.groupSizeField = null;
				base.OnUninstall();
			}

			// Token: 0x06001B40 RID: 6976 RVA: 0x00014230 File Offset: 0x00012430
			protected override string RebuildValue()
			{
				if (this.groupId == CSteamID.nil)
				{
					return null;
				}
				return TextSerialization.ToStringInvariant(this.groupId.value);
			}

			// Token: 0x06001B41 RID: 6977 RVA: 0x00088324 File Offset: 0x00086524
			private void OnClientConnectGlobal(NetworkConnection conn)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
				{
					this.hostId = steamNetworkConnection.steamId;
				}
			}

			// Token: 0x06001B42 RID: 6978 RVA: 0x00014256 File Offset: 0x00012456
			private void OnClientDisconnectGlobal(NetworkConnection conn)
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001B43 RID: 6979 RVA: 0x00014263 File Offset: 0x00012463
			private void OnStartServerGlobal()
			{
				this.hostId = new CSteamID(Client.Instance.SteamId);
			}

			// Token: 0x06001B44 RID: 6980 RVA: 0x00014256 File Offset: 0x00012456
			private void OnStopServerGlobal()
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001B45 RID: 6981 RVA: 0x0001427A File Offset: 0x0001247A
			private void OnLobbyChanged()
			{
				this.SetLobbyId(new CSteamID(Client.Instance.Lobby.CurrentLobby));
			}

			// Token: 0x04001DCE RID: 7630
			private CSteamID lobbyId = CSteamID.nil;

			// Token: 0x04001DCF RID: 7631
			private CSteamID hostId = CSteamID.nil;

			// Token: 0x04001DD0 RID: 7632
			private CSteamID groupId = CSteamID.nil;

			// Token: 0x04001DD1 RID: 7633
			private SteamworksRichPresenceManager.SteamPlayerGroupSizeField groupSizeField;
		}

		// Token: 0x020004B3 RID: 1203
		private abstract class SteamPlayerGroupSizeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000282 RID: 642
			// (get) Token: 0x06001B47 RID: 6983 RVA: 0x000142BF File Offset: 0x000124BF
			protected override string key
			{
				get
				{
					return "steam_player_group_size";
				}
			}

			// Token: 0x06001B48 RID: 6984 RVA: 0x000142C6 File Offset: 0x000124C6
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.groupSize);
			}

			// Token: 0x04001DD2 RID: 7634
			protected int groupSize;
		}

		// Token: 0x020004B4 RID: 1204
		private sealed class SteamPlayerGroupSizeFieldLobby : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001B4A RID: 6986 RVA: 0x000142D3 File Offset: 0x000124D3
			protected override void OnInstall()
			{
				base.OnInstall();
				SteamworksLobbyManager.onPlayerCountUpdated += this.UpdateGroupSize;
				this.UpdateGroupSize();
			}

			// Token: 0x06001B4B RID: 6987 RVA: 0x000142F2 File Offset: 0x000124F2
			protected override void OnUninstall()
			{
				SteamworksLobbyManager.onPlayerCountUpdated -= this.UpdateGroupSize;
				base.OnUninstall();
			}

			// Token: 0x06001B4C RID: 6988 RVA: 0x0001430B File Offset: 0x0001250B
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, SteamworksLobbyManager.calculatedTotalPlayerCount);
			}
		}

		// Token: 0x020004B5 RID: 1205
		private sealed class SteamPlayerGroupSizeFieldGame : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001B4E RID: 6990 RVA: 0x00014326 File Offset: 0x00012526
			protected override void OnInstall()
			{
				base.OnInstall();
				NetworkUser.onNetworkUserDiscovered += this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost += this.OnNetworkUserLost;
				this.UpdateGroupSize();
			}

			// Token: 0x06001B4F RID: 6991 RVA: 0x00014356 File Offset: 0x00012556
			protected override void OnUninstall()
			{
				NetworkUser.onNetworkUserDiscovered -= this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost -= this.OnNetworkUserLost;
				base.OnUninstall();
			}

			// Token: 0x06001B50 RID: 6992 RVA: 0x00014380 File Offset: 0x00012580
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, NetworkUser.readOnlyInstancesList.Count);
			}

			// Token: 0x06001B51 RID: 6993 RVA: 0x00014398 File Offset: 0x00012598
			private void OnNetworkUserLost(NetworkUser networkuser)
			{
				this.UpdateGroupSize();
			}

			// Token: 0x06001B52 RID: 6994 RVA: 0x00014398 File Offset: 0x00012598
			private void OnNetworkUserDiscovered(NetworkUser networkUser)
			{
				this.UpdateGroupSize();
			}
		}

		// Token: 0x020004B6 RID: 1206
		private sealed class SteamDisplayField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000283 RID: 643
			// (get) Token: 0x06001B54 RID: 6996 RVA: 0x000143A0 File Offset: 0x000125A0
			protected override string key
			{
				get
				{
					return "steam_display";
				}
			}

			// Token: 0x06001B55 RID: 6997 RVA: 0x00088348 File Offset: 0x00086548
			protected override string RebuildValue()
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (Run.instance)
				{
					if (GameOverController.instance)
					{
						return "#Display_GameOver";
					}
					return "#Display_InGame";
				}
				else
				{
					if (NetworkSession.instance)
					{
						return "#Display_PreGame";
					}
					if (SteamLobbyFinder.running)
					{
						return "#Display_Quickplay";
					}
					if (SteamworksLobbyManager.isInLobby)
					{
						return "#Display_InLobby";
					}
					if (activeScene.name == "logbook")
					{
						return "#Display_Logbook";
					}
					return "#Display_MainMenu";
				}
			}

			// Token: 0x06001B56 RID: 6998 RVA: 0x000143A7 File Offset: 0x000125A7
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onUpdate += base.SetDirty;
			}

			// Token: 0x06001B57 RID: 6999 RVA: 0x000143C0 File Offset: 0x000125C0
			protected override void OnUninstall()
			{
				RoR2Application.onUpdate -= base.SetDirty;
				base.OnUninstall();
			}
		}
	}
}
