using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using RoR2.ConVar;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x0200045F RID: 1119
	public static class LocalUserManager
	{
		// Token: 0x0600192C RID: 6444 RVA: 0x000821B0 File Offset: 0x000803B0
		public static bool UserExists(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x000821E8 File Offset: 0x000803E8
		private static int FindUserIndex(int userId)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].id == userId)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x00082220 File Offset: 0x00080420
		public static LocalUser FindLocalUser(int userId)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].id == userId)
				{
					return LocalUserManager.localUsersList[i];
				}
			}
			return null;
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x00082264 File Offset: 0x00080464
		public static LocalUser FindLocalUser(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return LocalUserManager.localUsersList[i];
				}
			}
			return null;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x00012D8D File Offset: 0x00010F8D
		public static LocalUser GetFirstLocalUser()
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return null;
			}
			return LocalUserManager.localUsersList[0];
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x000822A8 File Offset: 0x000804A8
		private static int FindUserIndex(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x000822E0 File Offset: 0x000804E0
		private static int GetFirstAvailableId()
		{
			int i;
			for (i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.FindUserIndex(i) == -1)
				{
					return i;
				}
			}
			return i;
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x00082310 File Offset: 0x00080510
		private static void AddUser(Player inputPlayer, UserProfile userProfile)
		{
			if (LocalUserManager.UserExists(inputPlayer))
			{
				return;
			}
			int firstAvailableId = LocalUserManager.GetFirstAvailableId();
			LocalUser localUser = new LocalUser
			{
				inputPlayer = inputPlayer,
				id = firstAvailableId,
				userProfile = userProfile
			};
			LocalUserManager.localUsersList.Add(localUser);
			userProfile.OnLogin();
			MPEventSystem.FindByPlayer(inputPlayer).localUser = localUser;
			if (LocalUserManager.onUserSignIn != null)
			{
				LocalUserManager.onUserSignIn(localUser);
			}
			if (LocalUserManager.onLocalUsersUpdated != null)
			{
				LocalUserManager.onLocalUsersUpdated();
			}
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x00082388 File Offset: 0x00080588
		public static bool IsUserChangeSafe()
		{
			return SceneManager.GetActiveScene().name == "title";
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x000823B4 File Offset: 0x000805B4
		public static void SetLocalUsers(LocalUserManager.LocalUserInitializationInfo[] initializationInfo)
		{
			if (LocalUserManager.localUsersList.Count > 0)
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers while users are already signed in!");
				return;
			}
			if (!LocalUserManager.IsUserChangeSafe())
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers at this time, user login changes are not safe at this time.");
				return;
			}
			if (initializationInfo.Length == 1)
			{
				initializationInfo[0].player = LocalUserManager.GetRewiredMainPlayer();
			}
			for (int i = 0; i < initializationInfo.Length; i++)
			{
				LocalUserManager.AddUser(initializationInfo[i].player, initializationInfo[i].profile);
			}
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x00012DA9 File Offset: 0x00010FA9
		private static Player GetRewiredMainPlayer()
		{
			return ReInput.players.GetPlayer("PlayerMain");
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x00012DBA File Offset: 0x00010FBA
		private static void AddMainUser(UserProfile userProfile)
		{
			LocalUserManager.AddUser(LocalUserManager.GetRewiredMainPlayer(), userProfile);
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x00082430 File Offset: 0x00080630
		private static void RemoveUser(Player inputPlayer)
		{
			int num = LocalUserManager.FindUserIndex(inputPlayer);
			if (num != -1)
			{
				LocalUserManager.RemoveUser(num);
			}
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00082450 File Offset: 0x00080650
		private static void RemoveUser(int userIndex)
		{
			LocalUser localUser = LocalUserManager.localUsersList[userIndex];
			if (LocalUserManager.onUserSignOut != null)
			{
				LocalUserManager.onUserSignOut(localUser);
			}
			localUser.userProfile.OnLogout();
			MPEventSystem.FindByPlayer(localUser.inputPlayer).localUser = null;
			LocalUserManager.localUsersList.RemoveAt(userIndex);
			if (LocalUserManager.onLocalUsersUpdated != null)
			{
				LocalUserManager.onLocalUsersUpdated();
			}
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x000824B4 File Offset: 0x000806B4
		public static void ClearUsers()
		{
			if (!LocalUserManager.IsUserChangeSafe())
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers at this time, user login changes are not safe at this time.");
				return;
			}
			for (int i = LocalUserManager.localUsersList.Count - 1; i >= 0; i--)
			{
				LocalUserManager.RemoveUser(i);
			}
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x000824F0 File Offset: 0x000806F0
		private static Player ListenForStartSignIn()
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (!(player.name == "PlayerMain") && !LocalUserManager.UserExists(player) && player.GetButtonDown("Start"))
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00012DC7 File Offset: 0x00010FC7
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onUpdate += LocalUserManager.Update;
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0008254C File Offset: 0x0008074C
		private static void Update()
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				LocalUserManager.localUsersList[i].RebuildControlChain();
			}
		}

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x0600193E RID: 6462 RVA: 0x00082580 File Offset: 0x00080780
		// (remove) Token: 0x0600193F RID: 6463 RVA: 0x000825B4 File Offset: 0x000807B4
		public static event Action<LocalUser> onUserSignIn;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06001940 RID: 6464 RVA: 0x000825E8 File Offset: 0x000807E8
		// (remove) Token: 0x06001941 RID: 6465 RVA: 0x0008261C File Offset: 0x0008081C
		public static event Action<LocalUser> onUserSignOut;

		// Token: 0x06001942 RID: 6466 RVA: 0x00012DDA File Offset: 0x00010FDA
		[ConCommand(commandName = "remove_all_local_users", flags = ConVarFlags.None, helpText = "Removes all local users.")]
		private static void CCRemoveAllLocalUsers(ConCommandArgs args)
		{
			LocalUserManager.ClearUsers();
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x00082650 File Offset: 0x00080850
		[ConCommand(commandName = "print_local_users", flags = ConVarFlags.None, helpText = "Prints a list of all local users.")]
		private static void CCPrintLocalUsers(ConCommandArgs args)
		{
			string[] array = new string[LocalUserManager.localUsersList.Count];
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i] != null)
				{
					array[i] = string.Format("localUsersList[{0}] id={1} userProfile={2}", i, LocalUserManager.localUsersList[i].id, (LocalUserManager.localUsersList[i].userProfile != null) ? LocalUserManager.localUsersList[i].userProfile.fileName : "null");
				}
				else
				{
					array[i] = string.Format("localUsersList[{0}] null", i);
				}
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x00082710 File Offset: 0x00080910
		[ConCommand(commandName = "test_splitscreen", flags = ConVarFlags.None, helpText = "Logs in the specified number of guest users, or two by default.")]
		private static void CCTestSplitscreen(ConCommandArgs args)
		{
			int num = 2;
			int value;
			if (args.Count >= 1 && TextSerialization.TryParseInvariant(args[0], out value))
			{
				num = Mathf.Clamp(value, 1, 4);
			}
			if (!NetworkClient.active)
			{
				LocalUserManager.ClearUsers();
				LocalUserManager.LocalUserInitializationInfo[] array = new LocalUserManager.LocalUserInitializationInfo[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new LocalUserManager.LocalUserInitializationInfo
					{
						player = ReInput.players.GetPlayer(2 + i),
						profile = UserProfile.CreateGuestProfile()
					};
				}
				LocalUserManager.SetLocalUsers(array);
			}
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x0008279C File Offset: 0x0008099C
		[ConCommand(commandName = "export_controller_maps", flags = ConVarFlags.None, helpText = "Prints all Rewired ControllerMaps of the first player as xml.")]
		public static void CCExportControllerMaps(ConCommandArgs args)
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return;
			}
			foreach (string message in from v in LocalUserManager.localUsersList[0].inputPlayer.controllers.maps.GetAllMaps()
			select v.ToXmlString())
			{
				Debug.Log(message);
			}
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001946 RID: 6470 RVA: 0x00082834 File Offset: 0x00080A34
		// (remove) Token: 0x06001947 RID: 6471 RVA: 0x00082868 File Offset: 0x00080A68
		public static event Action onLocalUsersUpdated;

		// Token: 0x04001C7A RID: 7290
		private static readonly List<LocalUser> localUsersList = new List<LocalUser>();

		// Token: 0x04001C7B RID: 7291
		public static readonly ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.localUsersList.AsReadOnly();

		// Token: 0x04001C7C RID: 7292
		public static Player startPlayer;

		// Token: 0x02000460 RID: 1120
		public struct LocalUserInitializationInfo
		{
			// Token: 0x04001C80 RID: 7296
			public Player player;

			// Token: 0x04001C81 RID: 7297
			public UserProfile profile;
		}

		// Token: 0x02000461 RID: 1121
		private class UserProfileMainConVar : BaseConVar
		{
			// Token: 0x06001949 RID: 6473 RVA: 0x000090CD File Offset: 0x000072CD
			public UserProfileMainConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600194A RID: 6474 RVA: 0x0008289C File Offset: 0x00080A9C
			public override void SetString(string newValue)
			{
				if (LocalUserManager.readOnlyLocalUsersList.Count > 0)
				{
					Debug.Log("Can't change user_profile_main while there are users signed in.");
					return;
				}
				UserProfile profile = UserProfile.GetProfile(newValue);
				if (profile != null && !profile.isCorrupted)
				{
					LocalUserManager.AddMainUser(profile);
				}
			}

			// Token: 0x0600194B RID: 6475 RVA: 0x000828DC File Offset: 0x00080ADC
			public override string GetString()
			{
				int num = LocalUserManager.FindUserIndex(LocalUserManager.GetRewiredMainPlayer());
				if (num == -1)
				{
					return "";
				}
				return LocalUserManager.localUsersList[num].userProfile.fileName;
			}

			// Token: 0x04001C82 RID: 7298
			private static LocalUserManager.UserProfileMainConVar cvClCurrentUserProfile = new LocalUserManager.UserProfileMainConVar("user_profile_main", ConVarFlags.Archive, null, "The current user profile.");
		}
	}
}
