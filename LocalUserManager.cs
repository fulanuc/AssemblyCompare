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
	// Token: 0x02000454 RID: 1108
	public static class LocalUserManager
	{
		// Token: 0x060018D0 RID: 6352 RVA: 0x00081810 File Offset: 0x0007FA10
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

		// Token: 0x060018D1 RID: 6353 RVA: 0x00081848 File Offset: 0x0007FA48
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

		// Token: 0x060018D2 RID: 6354 RVA: 0x00081880 File Offset: 0x0007FA80
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

		// Token: 0x060018D3 RID: 6355 RVA: 0x000818C4 File Offset: 0x0007FAC4
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

		// Token: 0x060018D4 RID: 6356 RVA: 0x00012880 File Offset: 0x00010A80
		public static LocalUser GetFirstLocalUser()
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return null;
			}
			return LocalUserManager.localUsersList[0];
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x00081908 File Offset: 0x0007FB08
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

		// Token: 0x060018D6 RID: 6358 RVA: 0x00081940 File Offset: 0x0007FB40
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

		// Token: 0x060018D7 RID: 6359 RVA: 0x00081970 File Offset: 0x0007FB70
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

		// Token: 0x060018D8 RID: 6360 RVA: 0x000819E8 File Offset: 0x0007FBE8
		public static bool IsUserChangeSafe()
		{
			return SceneManager.GetActiveScene().name == "title";
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x00081A14 File Offset: 0x0007FC14
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

		// Token: 0x060018DA RID: 6362 RVA: 0x0001289C File Offset: 0x00010A9C
		private static Player GetRewiredMainPlayer()
		{
			return ReInput.players.GetPlayer("PlayerMain");
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x000128AD File Offset: 0x00010AAD
		private static void AddMainUser(UserProfile userProfile)
		{
			LocalUserManager.AddUser(LocalUserManager.GetRewiredMainPlayer(), userProfile);
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x00081A90 File Offset: 0x0007FC90
		private static void RemoveUser(Player inputPlayer)
		{
			int num = LocalUserManager.FindUserIndex(inputPlayer);
			if (num != -1)
			{
				LocalUserManager.RemoveUser(num);
			}
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x00081AB0 File Offset: 0x0007FCB0
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

		// Token: 0x060018DE RID: 6366 RVA: 0x00081B14 File Offset: 0x0007FD14
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

		// Token: 0x060018DF RID: 6367 RVA: 0x00081B50 File Offset: 0x0007FD50
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

		// Token: 0x060018E0 RID: 6368 RVA: 0x000128BA File Offset: 0x00010ABA
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onUpdate += LocalUserManager.Update;
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x00081BAC File Offset: 0x0007FDAC
		private static void Update()
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				LocalUserManager.localUsersList[i].RebuildControlChain();
			}
		}

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060018E2 RID: 6370 RVA: 0x00081BE0 File Offset: 0x0007FDE0
		// (remove) Token: 0x060018E3 RID: 6371 RVA: 0x00081C14 File Offset: 0x0007FE14
		public static event Action<LocalUser> onUserSignIn;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060018E4 RID: 6372 RVA: 0x00081C48 File Offset: 0x0007FE48
		// (remove) Token: 0x060018E5 RID: 6373 RVA: 0x00081C7C File Offset: 0x0007FE7C
		public static event Action<LocalUser> onUserSignOut;

		// Token: 0x060018E6 RID: 6374 RVA: 0x000128CD File Offset: 0x00010ACD
		[ConCommand(commandName = "remove_all_local_users", flags = ConVarFlags.None, helpText = "Removes all local users.")]
		private static void CCRemoveAllLocalUsers(ConCommandArgs args)
		{
			LocalUserManager.ClearUsers();
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x00081CB0 File Offset: 0x0007FEB0
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

		// Token: 0x060018E8 RID: 6376 RVA: 0x00081D70 File Offset: 0x0007FF70
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

		// Token: 0x060018E9 RID: 6377 RVA: 0x00081DFC File Offset: 0x0007FFFC
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

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060018EA RID: 6378 RVA: 0x00081E94 File Offset: 0x00080094
		// (remove) Token: 0x060018EB RID: 6379 RVA: 0x00081EC8 File Offset: 0x000800C8
		public static event Action onLocalUsersUpdated;

		// Token: 0x04001C46 RID: 7238
		private static readonly List<LocalUser> localUsersList = new List<LocalUser>();

		// Token: 0x04001C47 RID: 7239
		public static readonly ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.localUsersList.AsReadOnly();

		// Token: 0x04001C48 RID: 7240
		public static Player startPlayer;

		// Token: 0x02000455 RID: 1109
		public struct LocalUserInitializationInfo
		{
			// Token: 0x04001C4C RID: 7244
			public Player player;

			// Token: 0x04001C4D RID: 7245
			public UserProfile profile;
		}

		// Token: 0x02000456 RID: 1110
		private class UserProfileMainConVar : BaseConVar
		{
			// Token: 0x060018ED RID: 6381 RVA: 0x000090A8 File Offset: 0x000072A8
			public UserProfileMainConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060018EE RID: 6382 RVA: 0x00081EFC File Offset: 0x000800FC
			public override void SetString(string newValue)
			{
				if (LocalUserManager.readOnlyLocalUsersList.Count > 0)
				{
					Debug.Log("Can't change user_profile_main while there are users signed in.");
					return;
				}
				UserProfile profile = UserProfile.GetProfile(newValue);
				if (profile != null)
				{
					LocalUserManager.AddMainUser(profile);
				}
			}

			// Token: 0x060018EF RID: 6383 RVA: 0x00081F34 File Offset: 0x00080134
			public override string GetString()
			{
				int num = LocalUserManager.FindUserIndex(LocalUserManager.GetRewiredMainPlayer());
				if (num == -1)
				{
					return "";
				}
				return LocalUserManager.localUsersList[num].userProfile.fileName;
			}

			// Token: 0x04001C4E RID: 7246
			private static LocalUserManager.UserProfileMainConVar cvClCurrentUserProfile = new LocalUserManager.UserProfileMainConVar("user_profile_main", ConVarFlags.Archive, null, "The current user profile.");
		}
	}
}
