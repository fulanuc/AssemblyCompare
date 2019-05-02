using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020004B8 RID: 1208
	internal static class SteamworksRichPresenceManager
	{
		// Token: 0x06001B66 RID: 7014 RVA: 0x000143EF File Offset: 0x000125EF
		private static void SetKeyValue([NotNull] string key, [CanBeNull] string value)
		{
			Client.Instance.User.SetRichPresence(key, value);
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x00088704 File Offset: 0x00086904
		private static void OnNetworkStart()
		{
			string text = null;
			CSteamID csteamID = CSteamID.nil;
			CSteamID csteamID2 = CSteamID.nil;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				string a = commandLineArgs[i].ToLower(CultureInfo.InvariantCulture);
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

		// Token: 0x06001B68 RID: 7016 RVA: 0x00088848 File Offset: 0x00086A48
		private static void OnLobbyChanged()
		{
			if (Client.Instance.Lobby.IsValid)
			{
				SteamworksRichPresenceManager.SetKeyValue("connect", "+steam_lobby_join " + Client.Instance.Lobby.CurrentLobby);
				return;
			}
			SteamworksRichPresenceManager.SetKeyValue("connect", null);
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x0008889C File Offset: 0x00086A9C
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

		// Token: 0x06001B6A RID: 7018 RVA: 0x00014403 File Offset: 0x00012603
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

		// Token: 0x06001B6B RID: 7019 RVA: 0x00088974 File Offset: 0x00086B74
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

		// Token: 0x06001B6C RID: 7020 RVA: 0x0001443C File Offset: 0x0001263C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(SteamworksRichPresenceManager.SetupCallbacks));
		}

		// Token: 0x04001DF1 RID: 7665
		private const string rpConnect = "connect";

		// Token: 0x04001DF2 RID: 7666
		private const string rpStatus = "status";

		// Token: 0x04001DF3 RID: 7667
		private const string rpSteamDisplay = "steam_display";

		// Token: 0x04001DF4 RID: 7668
		private const string rpSteamPlayerGroup = "steam_player_group";

		// Token: 0x04001DF5 RID: 7669
		private const string rpSteamPlayerGroupSize = "steam_player_group_size";

		// Token: 0x04001DF6 RID: 7670
		private const string rpDifficulty = "difficulty";

		// Token: 0x04001DF7 RID: 7671
		private const string rpGameMode = "gamemode";

		// Token: 0x04001DF8 RID: 7672
		private const string rpParticipationType = "participation_type";

		// Token: 0x04001DF9 RID: 7673
		private const string rpMinutes = "minutes";

		// Token: 0x020004B9 RID: 1209
		private abstract class BaseRichPresenceField
		{
			// Token: 0x06001B6D RID: 7021 RVA: 0x0001445E File Offset: 0x0001265E
			public static void ProcessDirtyFields()
			{
				while (SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Count > 0)
				{
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Dequeue().UpdateIfNecessary();
				}
			}

			// Token: 0x17000288 RID: 648
			// (get) Token: 0x06001B6E RID: 7022
			protected abstract string key { get; }

			// Token: 0x06001B6F RID: 7023
			[CanBeNull]
			protected abstract string RebuildValue();

			// Token: 0x06001B70 RID: 7024 RVA: 0x000025DA File Offset: 0x000007DA
			protected virtual void OnChanged()
			{
			}

			// Token: 0x06001B71 RID: 7025 RVA: 0x0001447E File Offset: 0x0001267E
			public void SetDirty()
			{
				if (!this.isDirty)
				{
					this.isDirty = true;
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Enqueue(this);
				}
			}

			// Token: 0x06001B72 RID: 7026 RVA: 0x00088A30 File Offset: 0x00086C30
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

			// Token: 0x06001B73 RID: 7027 RVA: 0x000025DA File Offset: 0x000007DA
			protected virtual void OnInstall()
			{
			}

			// Token: 0x06001B74 RID: 7028 RVA: 0x000025DA File Offset: 0x000007DA
			protected virtual void OnUninstall()
			{
			}

			// Token: 0x06001B75 RID: 7029 RVA: 0x0001449A File Offset: 0x0001269A
			public void Install()
			{
				if (!this.installed)
				{
					this.OnInstall();
					this.SetDirty();
					this.installed = true;
				}
			}

			// Token: 0x06001B76 RID: 7030 RVA: 0x000144B7 File Offset: 0x000126B7
			public void Uninstall()
			{
				if (this.installed)
				{
					this.OnUninstall();
					this.installed = false;
					SteamworksRichPresenceManager.SetKeyValue(this.key, null);
				}
			}

			// Token: 0x06001B77 RID: 7031 RVA: 0x000144DA File Offset: 0x000126DA
			protected void SetDirtyableValue<T>(ref T field, T value) where T : struct, IEquatable<T>
			{
				if (!field.Equals(value))
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x06001B78 RID: 7032 RVA: 0x000144F8 File Offset: 0x000126F8
			protected void SetDirtyableReference<T>(ref T field, T value) where T : class
			{
				if (field != value)
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x04001DFA RID: 7674
			private static readonly Queue<SteamworksRichPresenceManager.BaseRichPresenceField> dirtyFields = new Queue<SteamworksRichPresenceManager.BaseRichPresenceField>();

			// Token: 0x04001DFB RID: 7675
			private bool isDirty;

			// Token: 0x04001DFC RID: 7676
			[CanBeNull]
			private string currentValue;

			// Token: 0x04001DFD RID: 7677
			private bool installed;
		}

		// Token: 0x020004BA RID: 1210
		private sealed class DifficultyField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000289 RID: 649
			// (get) Token: 0x06001B7B RID: 7035 RVA: 0x00014526 File Offset: 0x00012726
			protected override string key
			{
				get
				{
					return "difficulty";
				}
			}

			// Token: 0x06001B7C RID: 7036 RVA: 0x00088A80 File Offset: 0x00086C80
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

			// Token: 0x06001B7D RID: 7037 RVA: 0x0001452D File Offset: 0x0001272D
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B7E RID: 7038 RVA: 0x00014535 File Offset: 0x00012735
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B7F RID: 7039 RVA: 0x0001455F File Offset: 0x0001275F
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004BB RID: 1211
		private sealed class GameModeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028A RID: 650
			// (get) Token: 0x06001B81 RID: 7041 RVA: 0x00014591 File Offset: 0x00012791
			protected override string key
			{
				get
				{
					return "gamemode";
				}
			}

			// Token: 0x06001B82 RID: 7042 RVA: 0x00014598 File Offset: 0x00012798
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

			// Token: 0x06001B83 RID: 7043 RVA: 0x0001452D File Offset: 0x0001272D
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B84 RID: 7044 RVA: 0x000145C2 File Offset: 0x000127C2
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B85 RID: 7045 RVA: 0x000145EC File Offset: 0x000127EC
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004BC RID: 1212
		private sealed class ParticipationField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028B RID: 651
			// (get) Token: 0x06001B87 RID: 7047 RVA: 0x00014616 File Offset: 0x00012816
			protected override string key
			{
				get
				{
					return "participation_type";
				}
			}

			// Token: 0x06001B88 RID: 7048 RVA: 0x0001461D File Offset: 0x0001281D
			private void SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType newParticipationType)
			{
				if (this.participationType != newParticipationType)
				{
					this.participationType = newParticipationType;
					base.SetDirty();
				}
			}

			// Token: 0x06001B89 RID: 7049 RVA: 0x00088AD0 File Offset: 0x00086CD0
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

			// Token: 0x06001B8A RID: 7050 RVA: 0x00088B10 File Offset: 0x00086D10
			protected override void OnInstall()
			{
				base.OnInstall();
				LocalUserManager.onUserSignIn += this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut += this.OnLocalUserLost;
				Run.onRunStartGlobal += this.OnRunStart;
				Run.onRunDestroyGlobal += this.OnRunDestroy;
			}

			// Token: 0x06001B8B RID: 7051 RVA: 0x00088B68 File Offset: 0x00086D68
			protected override void OnUninstall()
			{
				LocalUserManager.onUserSignIn -= this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut -= this.OnLocalUserLost;
				Run.onRunStartGlobal -= this.OnRunStart;
				Run.onRunDestroyGlobal -= this.OnRunDestroy;
				this.SetCurrentMaster(null);
			}

			// Token: 0x06001B8C RID: 7052 RVA: 0x00088BC0 File Offset: 0x00086DC0
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

			// Token: 0x06001B8D RID: 7053 RVA: 0x00014635 File Offset: 0x00012835
			private void OnLocalUserDiscovered(LocalUser localUser)
			{
				if (this.trackedUser == null)
				{
					this.SetTrackedUser(localUser);
				}
			}

			// Token: 0x06001B8E RID: 7054 RVA: 0x00014646 File Offset: 0x00012846
			private void OnLocalUserLost(LocalUser localUser)
			{
				if (this.trackedUser == localUser)
				{
					this.SetTrackedUser(null);
				}
			}

			// Token: 0x06001B8F RID: 7055 RVA: 0x00014658 File Offset: 0x00012858
			private void OnRunStart(Run run)
			{
				if (this.trackedUser != null && !this.trackedUser.cachedMasterObject)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator);
				}
			}

			// Token: 0x06001B90 RID: 7056 RVA: 0x0001467B File Offset: 0x0001287B
			private void OnRunDestroy(Run run)
			{
				if (this.trackedUser != null)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.None);
				}
			}

			// Token: 0x06001B91 RID: 7057 RVA: 0x00088C14 File Offset: 0x00086E14
			private void OnMasterChanged()
			{
				PlayerCharacterMasterController cachedMasterController = this.trackedUser.cachedMasterController;
				this.SetCurrentMaster(cachedMasterController ? cachedMasterController.master : null);
			}

			// Token: 0x06001B92 RID: 7058 RVA: 0x00088C44 File Offset: 0x00086E44
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

			// Token: 0x06001B93 RID: 7059 RVA: 0x0001468C File Offset: 0x0001288C
			private void OnBodyDeath()
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead);
			}

			// Token: 0x06001B94 RID: 7060 RVA: 0x00014695 File Offset: 0x00012895
			private void OnBodyStart(CharacterBody body)
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive);
			}

			// Token: 0x04001DFE RID: 7678
			private SteamworksRichPresenceManager.ParticipationField.ParticipationType participationType;

			// Token: 0x04001DFF RID: 7679
			private LocalUser trackedUser;

			// Token: 0x04001E00 RID: 7680
			private CharacterMaster currentMaster;

			// Token: 0x020004BD RID: 1213
			private enum ParticipationType
			{
				// Token: 0x04001E02 RID: 7682
				None,
				// Token: 0x04001E03 RID: 7683
				Alive,
				// Token: 0x04001E04 RID: 7684
				Dead,
				// Token: 0x04001E05 RID: 7685
				Spectator
			}
		}

		// Token: 0x020004BE RID: 1214
		private sealed class MinutesField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028C RID: 652
			// (get) Token: 0x06001B96 RID: 7062 RVA: 0x0001469E File Offset: 0x0001289E
			protected override string key
			{
				get
				{
					return "minutes";
				}
			}

			// Token: 0x06001B97 RID: 7063 RVA: 0x000146A5 File Offset: 0x000128A5
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.minutes);
			}

			// Token: 0x06001B98 RID: 7064 RVA: 0x00088CD0 File Offset: 0x00086ED0
			private void FixedUpdate()
			{
				uint value = 0u;
				if (Run.instance)
				{
					value = (uint)Mathf.FloorToInt(Run.instance.GetRunStopwatch() / 60f);
				}
				base.SetDirtyableValue<uint>(ref this.minutes, value);
			}

			// Token: 0x06001B99 RID: 7065 RVA: 0x000146B2 File Offset: 0x000128B2
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.FixedUpdate;
			}

			// Token: 0x06001B9A RID: 7066 RVA: 0x000146CB File Offset: 0x000128CB
			protected override void OnUninstall()
			{
				RoR2Application.onFixedUpdate -= this.FixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x04001E06 RID: 7686
			private uint minutes;
		}

		// Token: 0x020004BF RID: 1215
		private sealed class SteamPlayerGroupField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028D RID: 653
			// (get) Token: 0x06001B9C RID: 7068 RVA: 0x000146E4 File Offset: 0x000128E4
			protected override string key
			{
				get
				{
					return "steam_player_group";
				}
			}

			// Token: 0x06001B9D RID: 7069 RVA: 0x000146EB File Offset: 0x000128EB
			private void SetLobbyId(CSteamID newLobbyId)
			{
				if (this.lobbyId != newLobbyId)
				{
					this.lobbyId = newLobbyId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B9E RID: 7070 RVA: 0x00014708 File Offset: 0x00012908
			private void SetHostId(CSteamID newHostId)
			{
				if (this.hostId != newHostId)
				{
					this.hostId = newHostId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B9F RID: 7071 RVA: 0x00014725 File Offset: 0x00012925
			private void SetGroupId(CSteamID newGroupId)
			{
				if (this.groupId != newGroupId)
				{
					this.groupId = newGroupId;
					base.SetDirty();
				}
			}

			// Token: 0x06001BA0 RID: 7072 RVA: 0x00088D10 File Offset: 0x00086F10
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

			// Token: 0x06001BA1 RID: 7073 RVA: 0x00088DB0 File Offset: 0x00086FB0
			protected override void OnInstall()
			{
				base.OnInstall();
				GameNetworkManager.onClientConnectGlobal += this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal += this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal += this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal += this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged += this.OnLobbyChanged;
			}

			// Token: 0x06001BA2 RID: 7074 RVA: 0x00088E18 File Offset: 0x00087018
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

			// Token: 0x06001BA3 RID: 7075 RVA: 0x00014742 File Offset: 0x00012942
			protected override string RebuildValue()
			{
				if (this.groupId == CSteamID.nil)
				{
					return null;
				}
				return TextSerialization.ToStringInvariant(this.groupId.value);
			}

			// Token: 0x06001BA4 RID: 7076 RVA: 0x00088E98 File Offset: 0x00087098
			private void OnClientConnectGlobal(NetworkConnection conn)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
				{
					this.hostId = steamNetworkConnection.steamId;
				}
			}

			// Token: 0x06001BA5 RID: 7077 RVA: 0x00014768 File Offset: 0x00012968
			private void OnClientDisconnectGlobal(NetworkConnection conn)
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001BA6 RID: 7078 RVA: 0x00014775 File Offset: 0x00012975
			private void OnStartServerGlobal()
			{
				this.hostId = new CSteamID(Client.Instance.SteamId);
			}

			// Token: 0x06001BA7 RID: 7079 RVA: 0x00014768 File Offset: 0x00012968
			private void OnStopServerGlobal()
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001BA8 RID: 7080 RVA: 0x0001478C File Offset: 0x0001298C
			private void OnLobbyChanged()
			{
				this.SetLobbyId(new CSteamID(Client.Instance.Lobby.CurrentLobby));
			}

			// Token: 0x04001E07 RID: 7687
			private CSteamID lobbyId = CSteamID.nil;

			// Token: 0x04001E08 RID: 7688
			private CSteamID hostId = CSteamID.nil;

			// Token: 0x04001E09 RID: 7689
			private CSteamID groupId = CSteamID.nil;

			// Token: 0x04001E0A RID: 7690
			private SteamworksRichPresenceManager.SteamPlayerGroupSizeField groupSizeField;
		}

		// Token: 0x020004C0 RID: 1216
		private abstract class SteamPlayerGroupSizeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028E RID: 654
			// (get) Token: 0x06001BAA RID: 7082 RVA: 0x000147D1 File Offset: 0x000129D1
			protected override string key
			{
				get
				{
					return "steam_player_group_size";
				}
			}

			// Token: 0x06001BAB RID: 7083 RVA: 0x000147D8 File Offset: 0x000129D8
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.groupSize);
			}

			// Token: 0x04001E0B RID: 7691
			protected int groupSize;
		}

		// Token: 0x020004C1 RID: 1217
		private sealed class SteamPlayerGroupSizeFieldLobby : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001BAD RID: 7085 RVA: 0x000147E5 File Offset: 0x000129E5
			protected override void OnInstall()
			{
				base.OnInstall();
				SteamworksLobbyManager.onPlayerCountUpdated += this.UpdateGroupSize;
				this.UpdateGroupSize();
			}

			// Token: 0x06001BAE RID: 7086 RVA: 0x00014804 File Offset: 0x00012A04
			protected override void OnUninstall()
			{
				SteamworksLobbyManager.onPlayerCountUpdated -= this.UpdateGroupSize;
				base.OnUninstall();
			}

			// Token: 0x06001BAF RID: 7087 RVA: 0x0001481D File Offset: 0x00012A1D
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, SteamworksLobbyManager.calculatedTotalPlayerCount);
			}
		}

		// Token: 0x020004C2 RID: 1218
		private sealed class SteamPlayerGroupSizeFieldGame : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001BB1 RID: 7089 RVA: 0x00014838 File Offset: 0x00012A38
			protected override void OnInstall()
			{
				base.OnInstall();
				NetworkUser.onNetworkUserDiscovered += this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost += this.OnNetworkUserLost;
				this.UpdateGroupSize();
			}

			// Token: 0x06001BB2 RID: 7090 RVA: 0x00014868 File Offset: 0x00012A68
			protected override void OnUninstall()
			{
				NetworkUser.onNetworkUserDiscovered -= this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost -= this.OnNetworkUserLost;
				base.OnUninstall();
			}

			// Token: 0x06001BB3 RID: 7091 RVA: 0x00014892 File Offset: 0x00012A92
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, NetworkUser.readOnlyInstancesList.Count);
			}

			// Token: 0x06001BB4 RID: 7092 RVA: 0x000148AA File Offset: 0x00012AAA
			private void OnNetworkUserLost(NetworkUser networkuser)
			{
				this.UpdateGroupSize();
			}

			// Token: 0x06001BB5 RID: 7093 RVA: 0x000148AA File Offset: 0x00012AAA
			private void OnNetworkUserDiscovered(NetworkUser networkUser)
			{
				this.UpdateGroupSize();
			}
		}

		// Token: 0x020004C3 RID: 1219
		private sealed class SteamDisplayField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700028F RID: 655
			// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000148B2 File Offset: 0x00012AB2
			protected override string key
			{
				get
				{
					return "steam_display";
				}
			}

			// Token: 0x06001BB8 RID: 7096 RVA: 0x00088EBC File Offset: 0x000870BC
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

			// Token: 0x06001BB9 RID: 7097 RVA: 0x000148B9 File Offset: 0x00012AB9
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onUpdate += base.SetDirty;
			}

			// Token: 0x06001BBA RID: 7098 RVA: 0x000148D2 File Offset: 0x00012AD2
			protected override void OnUninstall()
			{
				RoR2Application.onUpdate -= base.SetDirty;
				base.OnUninstall();
			}
		}
	}
}
