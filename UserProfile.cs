using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using Rewired;
using RoR2.Stats;
using UnityEngine;
using Zio;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020004CB RID: 1227
	public class UserProfile
	{
		// Token: 0x06001BA9 RID: 7081 RVA: 0x00089B10 File Offset: 0x00087D10
		public bool HasUnlockable(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			return unlockableDef == null || this.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001BAA RID: 7082 RVA: 0x000147EE File Offset: 0x000129EE
		public bool HasUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			return this.statSheet.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001BAB RID: 7083 RVA: 0x00089B30 File Offset: 0x00087D30
		public void AddUnlockToken(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef != null)
			{
				this.GrantUnlockable(unlockableDef);
			}
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x00089B50 File Offset: 0x00087D50
		public void GrantUnlockable(UnlockableDef unlockableDef)
		{
			if (!this.statSheet.HasUnlockable(unlockableDef))
			{
				this.statSheet.AddUnlockable(unlockableDef);
				Debug.LogFormat("{0} unlocked {1}", new object[]
				{
					this.name,
					unlockableDef.nameToken
				});
				this.RequestSave(false);
				Action<UserProfile, string> action = UserProfile.onUnlockableGranted;
				if (action == null)
				{
					return;
				}
				action(this, unlockableDef.name);
			}
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x000147FC File Offset: 0x000129FC
		public void RevokeUnlockable(UnlockableDef unlockableDef)
		{
			if (this.statSheet.HasUnlockable(unlockableDef))
			{
				this.statSheet.RemoveUnlockable(unlockableDef.index);
			}
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x00089BB8 File Offset: 0x00087DB8
		public bool HasSurvivorUnlocked(SurvivorIndex survivorIndex)
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
			return survivorDef != null && (survivorDef.unlockableName == "" || this.HasUnlockable(survivorDef.unlockableName));
		}

		// Token: 0x06001BAF RID: 7087 RVA: 0x0001481D File Offset: 0x00012A1D
		public bool HasDiscoveredPickup(PickupIndex pickupIndex)
		{
			return pickupIndex.isValid && this.discoveredPickups[pickupIndex.value];
		}

		// Token: 0x06001BB0 RID: 7088 RVA: 0x00014837 File Offset: 0x00012A37
		public void DiscoverPickup(PickupIndex pickupIndex)
		{
			if (!pickupIndex.isValid)
			{
				return;
			}
			this.discoveredPickups[pickupIndex.value] = true;
			Action<PickupIndex> action = this.onPickupDiscovered;
			if (action != null)
			{
				action(pickupIndex);
			}
			this.RequestSave(false);
		}

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06001BB1 RID: 7089 RVA: 0x00089BF4 File Offset: 0x00087DF4
		// (remove) Token: 0x06001BB2 RID: 7090 RVA: 0x00089C2C File Offset: 0x00087E2C
		public event Action<PickupIndex> onPickupDiscovered;

		// Token: 0x06001BB3 RID: 7091 RVA: 0x0001486A File Offset: 0x00012A6A
		public bool HasAchievement(string achievementName)
		{
			return this.achievementsList.Contains(achievementName);
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x00089C64 File Offset: 0x00087E64
		public bool CanSeeAchievement(string achievementName)
		{
			if (this.HasAchievement(achievementName))
			{
				return true;
			}
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementName);
			return achievementDef != null && (string.IsNullOrEmpty(achievementDef.prerequisiteAchievementIdentifier) || this.HasAchievement(achievementDef.prerequisiteAchievementIdentifier));
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x00014878 File Offset: 0x00012A78
		public void AddAchievement(string achievementName, bool isExternal)
		{
			this.achievementsList.Add(achievementName);
			this.unviewedAchievementsList.Add(achievementName);
			if (isExternal)
			{
				Client.Instance.Achievements.Trigger(achievementName, true);
			}
			this.RequestSave(false);
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x000148AE File Offset: 0x00012AAE
		public void RevokeAchievement(string achievementName)
		{
			this.achievementsList.Remove(achievementName);
			this.unviewedAchievementsList.Remove(achievementName);
			this.RequestSave(false);
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000148D1 File Offset: 0x00012AD1
		public bool hasUnviewedAchievement
		{
			get
			{
				return this.unviewedAchievementsList.Count > 0;
			}
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x000148E1 File Offset: 0x00012AE1
		public string PopNextUnviewedAchievementName()
		{
			if (this.unviewedAchievementsList.Count == 0)
			{
				return null;
			}
			string result = this.unviewedAchievementsList[0];
			this.unviewedAchievementsList.RemoveAt(0);
			return result;
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x00089CA4 File Offset: 0x00087EA4
		private static void GenerateSaveFieldFunctions()
		{
			UserProfile.nameToSaveFieldMap.Clear();
			foreach (FieldInfo fieldInfo in typeof(UserProfile).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				UserProfile.SaveFieldAttribute customAttribute = fieldInfo.GetCustomAttribute<UserProfile.SaveFieldAttribute>();
				if (customAttribute != null)
				{
					customAttribute.Setup(fieldInfo);
					UserProfile.nameToSaveFieldMap[fieldInfo.Name] = customAttribute;
				}
			}
			UserProfile.saveFieldNames = UserProfile.nameToSaveFieldMap.Keys.ToArray<string>();
			Array.Sort<string>(UserProfile.saveFieldNames);
			UserProfile.saveFields = (from name in UserProfile.saveFieldNames
			select UserProfile.nameToSaveFieldMap[name]).ToArray<UserProfile.SaveFieldAttribute>();
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x00089D54 File Offset: 0x00087F54
		public void SetSaveFieldString([NotNull] string fieldName, [NotNull] string value)
		{
			UserProfile.SaveFieldAttribute saveFieldAttribute;
			if (UserProfile.nameToSaveFieldMap.TryGetValue(fieldName, out saveFieldAttribute))
			{
				saveFieldAttribute.setter(this, value);
				return;
			}
			Debug.LogErrorFormat("Save field {0} is not defined.", new object[]
			{
				fieldName
			});
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x00089D94 File Offset: 0x00087F94
		public string GetSaveFieldString([NotNull] string fieldName)
		{
			UserProfile.SaveFieldAttribute saveFieldAttribute;
			if (UserProfile.nameToSaveFieldMap.TryGetValue(fieldName, out saveFieldAttribute))
			{
				return saveFieldAttribute.getter(this);
			}
			Debug.LogErrorFormat("Save field {0} is not defined.", new object[]
			{
				fieldName
			});
			return string.Empty;
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x00089DD8 File Offset: 0x00087FD8
		public void ApplyDeltaStatSheet(StatSheet deltaStatSheet)
		{
			int i = 0;
			int unlockableCount = deltaStatSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				this.GrantUnlockable(deltaStatSheet.GetUnlockable(i));
				i++;
			}
			this.statSheet.ApplyDelta(deltaStatSheet);
			Action action = this.onStatsReceived;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x06001BBD RID: 7101 RVA: 0x00089E24 File Offset: 0x00088024
		// (remove) Token: 0x06001BBE RID: 7102 RVA: 0x00089E5C File Offset: 0x0008805C
		public event Action onStatsReceived;

		// Token: 0x06001BBF RID: 7103 RVA: 0x0001490A File Offset: 0x00012B0A
		private void ResetShouldShowTutorial(ref UserProfile.TutorialProgression tutorialProgression)
		{
			tutorialProgression.shouldShow = (tutorialProgression.showCount < 3u);
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x0001491B File Offset: 0x00012B1B
		private void RebuildTutorialProgressions()
		{
			this.ResetShouldShowTutorial(ref this.tutorialDifficulty);
			this.ResetShouldShowTutorial(ref this.tutorialSprint);
			this.ResetShouldShowTutorial(ref this.tutorialEquipment);
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x00014941 File Offset: 0x00012B41
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			UserProfile.GenerateSaveFieldFunctions();
			RoR2Application.onUpdate += UserProfile.StaticUpdate;
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x00089E94 File Offset: 0x00088094
		private static void StaticUpdate()
		{
			UserProfile.secondAccumulator += Time.unscaledDeltaTime;
			if (UserProfile.secondAccumulator > 1f)
			{
				UserProfile.secondAccumulator -= 1f;
				foreach (UserProfile userProfile in UserProfile.loggedInProfiles)
				{
					userProfile.totalLoginSeconds += 1u;
				}
			}
			foreach (UserProfile userProfile2 in UserProfile.loggedInProfiles)
			{
				if (userProfile2.saveRequestPending && userProfile2.Save(false))
				{
					userProfile2.saveRequestPending = false;
				}
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06001BC3 RID: 7107 RVA: 0x00014959 File Offset: 0x00012B59
		// (set) Token: 0x06001BC4 RID: 7108 RVA: 0x00014961 File Offset: 0x00012B61
		public bool loggedIn { get; private set; }

		// Token: 0x06001BC5 RID: 7109 RVA: 0x00089F6C File Offset: 0x0008816C
		public void OnLogin()
		{
			if (this.loggedIn)
			{
				Debug.LogErrorFormat("Profile {0} is already logged in!", new object[]
				{
					this.fileName
				});
				return;
			}
			this.loggedIn = true;
			UserProfile.loggedInProfiles.Add(this);
			this.RebuildTutorialProgressions();
			foreach (string identifier in this.achievementsList)
			{
				Client.Instance.Achievements.Trigger(identifier, true);
			}
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x0001496A File Offset: 0x00012B6A
		public void OnLogout()
		{
			if (!this.loggedIn)
			{
				Debug.LogErrorFormat("Profile {0} is already logged out!", new object[]
				{
					this.fileName
				});
				return;
			}
			UserProfile.loggedInProfiles.Remove(this);
			this.loggedIn = false;
			this.RequestSave(true);
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x0008A004 File Offset: 0x00088204
		public static void HandleShutDown()
		{
			foreach (UserProfile userProfile in UserProfile.loggedInProfiles)
			{
				userProfile.RequestSave(true);
			}
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x0008A054 File Offset: 0x00088254
		public static void LoadUserProfiles()
		{
			UserProfile.loadedUserProfiles.Clear();
			FileSystem cloudStorage = RoR2Application.cloudStorage;
			if (!cloudStorage.DirectoryExists(UserProfile.userProfilesFolder))
			{
				cloudStorage.CreateDirectory(UserProfile.userProfilesFolder);
			}
			foreach (UPath path in cloudStorage.EnumeratePaths(UserProfile.userProfilesFolder))
			{
				if (cloudStorage.FileExists(path) && string.CompareOrdinal(path.GetExtensionWithDot(), ".xml") == 0)
				{
					UserProfile userProfile = UserProfile.LoadUserProfileFromDisk(cloudStorage, path);
					if (userProfile != null)
					{
						UserProfile.loadedUserProfiles[userProfile.fileName] = userProfile;
					}
				}
			}
			UserProfile.LoadDefaultProfile();
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x0008A104 File Offset: 0x00088304
		public static List<string> GetAvailableProfileNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, UserProfile> keyValuePair in UserProfile.loadedUserProfiles)
			{
				if (!keyValuePair.Value.isClaimed)
				{
					list.Add(keyValuePair.Key);
				}
			}
			list.Sort();
			return list;
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x0008A178 File Offset: 0x00088378
		public static UserProfile GetProfile(string profileName)
		{
			profileName = profileName.ToLower();
			UserProfile result;
			if (UserProfile.loadedUserProfiles.TryGetValue(profileName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x000149A8 File Offset: 0x00012BA8
		public void RequestSave(bool immediate = false)
		{
			if (!this.canSave)
			{
				return;
			}
			if (immediate)
			{
				this.Save(true);
				return;
			}
			this.saveRequestPending = true;
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x0008A1A0 File Offset: 0x000883A0
		private bool Save(bool blocking)
		{
			bool result;
			try
			{
				UserProfile.<>c__DisplayClass87_0 CS$<>8__locals1 = new UserProfile.<>c__DisplayClass87_0();
				Debug.LogFormat("Saving profile \"{0}\"...", new object[]
				{
					this.fileName
				});
				CS$<>8__locals1.stream = this.fileSystem.OpenFile(this.filePath, FileMode.Create, FileAccess.Write, FileShare.None);
				CS$<>8__locals1.tempCopy = new UserProfile();
				UserProfile.Copy(this, CS$<>8__locals1.tempCopy);
				Task task = new Task(new Action(CS$<>8__locals1.<Save>g__WriteAction|0));
				task.Start(TaskScheduler.Default);
				if (blocking)
				{
					task.Wait();
				}
				result = true;
			}
			catch (Exception message)
			{
				Debug.Log(message);
				result = false;
			}
			return result;
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x0008A244 File Offset: 0x00088444
		private static UserProfile LoadUserProfileFromDisk(IFileSystem fileSystem, UPath path)
		{
			Debug.LogFormat("Attempting to load user profile {0}", new object[]
			{
				path
			});
			UserProfile result;
			using (Stream stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Debug.LogFormat("stream.Length={0}", new object[]
				{
					stream.Length
				});
				UserProfile userProfile = UserProfile.XmlUtility.FromXml(XDocument.Load(stream));
				userProfile.fileName = Path.GetFileNameWithoutExtension(path.FullName);
				userProfile.canSave = true;
				userProfile.fileSystem = fileSystem;
				userProfile.filePath = path;
				result = userProfile;
			}
			return result;
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x0008A2E4 File Offset: 0x000884E4
		private static void Copy(UserProfile src, UserProfile dest)
		{
			dest.fileSystem = src.fileSystem;
			dest.filePath = src.filePath;
			StatSheet.Copy(src.statSheet, dest.statSheet);
			dest.tutorialSprint = src.tutorialSprint;
			dest.tutorialDifficulty = src.tutorialDifficulty;
			dest.tutorialEquipment = src.tutorialEquipment;
			UserProfile.SaveFieldAttribute[] array = UserProfile.saveFields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].copier(src, dest);
			}
			dest.isClaimed = false;
			dest.canSave = false;
			dest.fileName = src.fileName;
			dest.onPickupDiscovered = null;
			dest.onStatsReceived = null;
			dest.loggedIn = false;
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x0008A390 File Offset: 0x00088590
		private static void DeleteUserProfile(string fileName)
		{
			fileName = fileName.ToLower();
			string.Format(CultureInfo.InvariantCulture, "{0}{1}.xml", UserProfile.userProfilesFolder, fileName);
			UserProfile profile = UserProfile.GetProfile(fileName);
			if (UserProfile.loadedUserProfiles.ContainsKey(fileName))
			{
				UserProfile.loadedUserProfiles.Remove(fileName);
			}
			if (profile != null && profile.fileSystem != null)
			{
				profile.fileSystem.DeleteFile(profile.filePath);
			}
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000149C6 File Offset: 0x00012BC6
		public static XDocument ToXml(UserProfile userProfile)
		{
			return UserProfile.XmlUtility.ToXml(userProfile);
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x000149CE File Offset: 0x00012BCE
		private static UserProfile FromXml(XDocument doc)
		{
			return UserProfile.XmlUtility.FromXml(doc);
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x0008A40C File Offset: 0x0008860C
		public static UserProfile CreateProfile(IFileSystem fileSystem, string name)
		{
			UserProfile userProfile = UserProfile.FromXml(UserProfile.ToXml(UserProfile.defaultProfile));
			userProfile.fileName = Guid.NewGuid().ToString();
			userProfile.fileSystem = fileSystem;
			userProfile.filePath = UserProfile.userProfilesFolder / (userProfile.fileName + ".xml");
			userProfile.name = name;
			userProfile.canSave = true;
			UserProfile.loadedUserProfiles.Add(userProfile.fileName, userProfile);
			userProfile.Save(true);
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action != null)
			{
				action();
			}
			return userProfile;
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x0008A4A8 File Offset: 0x000886A8
		public static UserProfile CreateGuestProfile()
		{
			UserProfile userProfile = new UserProfile();
			UserProfile.Copy(UserProfile.defaultProfile, userProfile);
			userProfile.name = "Guest";
			return userProfile;
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x0008A4D4 File Offset: 0x000886D4
		[ConCommand(commandName = "user_profile_save", flags = ConVarFlags.None, helpText = "Saves the named profile to disk, if it exists.")]
		private static void CCUserProfileSave(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string text = args[0].ToLower();
			if (text == "default")
			{
				Debug.Log("Cannot save profile \"default\", it is a reserved profile.");
				return;
			}
			UserProfile profile = UserProfile.GetProfile(text);
			if (profile == null)
			{
				Debug.LogFormat("Could not find profile \"{0}\" to save.", new object[]
				{
					text
				});
				return;
			}
			profile.Save(true);
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x0008A538 File Offset: 0x00088738
		[ConCommand(commandName = "user_profile_copy", flags = ConVarFlags.None, helpText = "Copies the profile named by the first argument to a new profile named by the second argument. This does not save the profile.")]
		private static void CCUserProfileCopy(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			string text = args[0].ToLower();
			string text2 = args[1].ToLower();
			UserProfile profile = UserProfile.GetProfile(text);
			if (profile == null)
			{
				Debug.LogFormat("Profile {0} does not exist, so it cannot be copied.", new object[]
				{
					text
				});
				return;
			}
			if (UserProfile.GetProfile(text2) != null)
			{
				Debug.LogFormat("Profile {0} already exists, and cannot be copied to.", new object[]
				{
					text2
				});
				return;
			}
			UserProfile userProfile = new UserProfile();
			UserProfile.Copy(profile, userProfile);
			userProfile.fileSystem = (profile.fileSystem ?? RoR2Application.cloudStorage);
			userProfile.filePath = UserProfile.userProfilesFolder / (text2 + ".xml");
			userProfile.fileName = text2;
			userProfile.canSave = true;
			UserProfile.loadedUserProfiles[text2] = userProfile;
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x0008A610 File Offset: 0x00088810
		[ConCommand(commandName = "user_profile_delete", flags = ConVarFlags.None, helpText = "Unloads the named user profile and deletes it from the disk if it exists.")]
		private static void CCUserProfileDelete(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string a = args[0].ToLower();
			if (a == "default")
			{
				Debug.Log("Cannot delete profile \"default\", it is a reserved profile.");
				return;
			}
			UserProfile.DeleteUserProfile(a);
		}

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x06001BD7 RID: 7127 RVA: 0x0008A654 File Offset: 0x00088854
		// (remove) Token: 0x06001BD8 RID: 7128 RVA: 0x0008A688 File Offset: 0x00088888
		public static event Action onAvailableUserProfilesChanged;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x06001BD9 RID: 7129 RVA: 0x0008A6BC File Offset: 0x000888BC
		// (remove) Token: 0x06001BDA RID: 7130 RVA: 0x0008A6F0 File Offset: 0x000888F0
		public static event Action<UserProfile, string> onUnlockableGranted;

		// Token: 0x06001BDB RID: 7131 RVA: 0x000149D6 File Offset: 0x00012BD6
		private static void LoadDefaultProfile()
		{
			UserProfile.defaultProfile = UserProfile.XmlUtility.FromXml(XDocument.Parse("<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>"));
			UserProfile.defaultProfile.canSave = false;
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000149F7 File Offset: 0x00012BF7
		public bool HasViewedViewable(string viewableName)
		{
			return this.viewedViewables.Contains(viewableName);
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00014A05 File Offset: 0x00012C05
		public void MarkViewableAsViewed(string viewableName)
		{
			if (this.HasViewedViewable(viewableName))
			{
				return;
			}
			this.viewedViewables.Add(viewableName);
			Action<UserProfile> action = UserProfile.onUserProfileViewedViewablesChanged;
			if (action != null)
			{
				action(this);
			}
			this.RequestSave(false);
		}

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06001BDE RID: 7134 RVA: 0x0008A724 File Offset: 0x00088924
		// (remove) Token: 0x06001BDF RID: 7135 RVA: 0x0008A758 File Offset: 0x00088958
		public static event Action<UserProfile> onUserProfileViewedViewablesChanged;

		// Token: 0x04001E18 RID: 7704
		public bool isClaimed;

		// Token: 0x04001E19 RID: 7705
		public bool canSave;

		// Token: 0x04001E1A RID: 7706
		public string fileName;

		// Token: 0x04001E1B RID: 7707
		public IFileSystem fileSystem;

		// Token: 0x04001E1C RID: 7708
		public UPath filePath = UPath.Empty;

		// Token: 0x04001E1D RID: 7709
		[UserProfile.SaveFieldAttribute]
		public string name;

		// Token: 0x04001E1E RID: 7710
		[UserProfile.SaveFieldAttribute]
		public uint coins;

		// Token: 0x04001E1F RID: 7711
		[UserProfile.SaveFieldAttribute]
		public uint totalCollectedCoins;

		// Token: 0x04001E20 RID: 7712
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		public List<string> viewedUnlockablesList = new List<string>();

		// Token: 0x04001E21 RID: 7713
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupPickupsSet")]
		private readonly bool[] discoveredPickups = new bool[106];

		// Token: 0x04001E23 RID: 7715
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> achievementsList = new List<string>();

		// Token: 0x04001E24 RID: 7716
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> unviewedAchievementsList = new List<string>();

		// Token: 0x04001E25 RID: 7717
		[UserProfile.SaveFieldAttribute]
		public string version = "2";

		// Token: 0x04001E26 RID: 7718
		[UserProfile.SaveFieldAttribute]
		public float screenShakeScale = 1f;

		// Token: 0x04001E27 RID: 7719
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupKeyboardMap")]
		public KeyboardMap keyboardMap = new KeyboardMap(DefaultControllerMaps.defaultKeyboardMap);

		// Token: 0x04001E28 RID: 7720
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupMouseMap")]
		public MouseMap mouseMap = new MouseMap(DefaultControllerMaps.defaultMouseMap);

		// Token: 0x04001E29 RID: 7721
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupJoystickMap")]
		public JoystickMap joystickMap = new JoystickMap(DefaultControllerMaps.defaultJoystickMap);

		// Token: 0x04001E2A RID: 7722
		[UserProfile.SaveFieldAttribute]
		public float mouseLookSensitivity = 0.25f;

		// Token: 0x04001E2B RID: 7723
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleX = 1f;

		// Token: 0x04001E2C RID: 7724
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleY = 1f;

		// Token: 0x04001E2D RID: 7725
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertX;

		// Token: 0x04001E2E RID: 7726
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertY;

		// Token: 0x04001E2F RID: 7727
		[UserProfile.SaveFieldAttribute]
		public float stickLookSensitivity = 4f;

		// Token: 0x04001E30 RID: 7728
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleX = 1f;

		// Token: 0x04001E31 RID: 7729
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleY = 1f;

		// Token: 0x04001E32 RID: 7730
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertX;

		// Token: 0x04001E33 RID: 7731
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertY;

		// Token: 0x04001E34 RID: 7732
		[UserProfile.SaveFieldAttribute]
		public float gamepadVibrationScale = 1f;

		// Token: 0x04001E35 RID: 7733
		private static string[] saveFieldNames;

		// Token: 0x04001E36 RID: 7734
		private static UserProfile.SaveFieldAttribute[] saveFields;

		// Token: 0x04001E37 RID: 7735
		private static readonly Dictionary<string, UserProfile.SaveFieldAttribute> nameToSaveFieldMap = new Dictionary<string, UserProfile.SaveFieldAttribute>();

		// Token: 0x04001E38 RID: 7736
		public StatSheet statSheet = StatSheet.New();

		// Token: 0x04001E3A RID: 7738
		private const uint maxShowCount = 3u;

		// Token: 0x04001E3B RID: 7739
		public UserProfile.TutorialProgression tutorialDifficulty;

		// Token: 0x04001E3C RID: 7740
		public UserProfile.TutorialProgression tutorialSprint;

		// Token: 0x04001E3D RID: 7741
		public UserProfile.TutorialProgression tutorialEquipment;

		// Token: 0x04001E3E RID: 7742
		[UserProfile.SaveFieldAttribute]
		public uint totalLoginSeconds;

		// Token: 0x04001E3F RID: 7743
		[UserProfile.SaveFieldAttribute]
		public uint totalRunSeconds;

		// Token: 0x04001E40 RID: 7744
		[UserProfile.SaveFieldAttribute]
		public uint totalAliveSeconds;

		// Token: 0x04001E41 RID: 7745
		[UserProfile.SaveFieldAttribute]
		public uint totalRunCount;

		// Token: 0x04001E42 RID: 7746
		private static float secondAccumulator;

		// Token: 0x04001E43 RID: 7747
		private static readonly List<UserProfile> loggedInProfiles = new List<UserProfile>();

		// Token: 0x04001E45 RID: 7749
		private static UPath userProfilesFolder = "/UserProfiles";

		// Token: 0x04001E46 RID: 7750
		private static readonly Dictionary<string, UserProfile> loadedUserProfiles = new Dictionary<string, UserProfile>();

		// Token: 0x04001E47 RID: 7751
		private bool saveRequestPending;

		// Token: 0x04001E48 RID: 7752
		public static UserProfile defaultProfile;

		// Token: 0x04001E4B RID: 7755
		private const string defaultProfileContents = "<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>";

		// Token: 0x04001E4C RID: 7756
		[UserProfile.SaveFieldAttribute(defaultValue = "", explicitSetupMethod = "SetupTokenList", fieldName = "viewedViewables")]
		private readonly List<string> viewedViewables = new List<string>();

		// Token: 0x020004CC RID: 1228
		public class SaveFieldAttribute : Attribute
		{
			// Token: 0x06001BE2 RID: 7138 RVA: 0x0008A884 File Offset: 0x00088A84
			public void Setup(FieldInfo fieldInfo)
			{
				this.fieldInfo = fieldInfo;
				Type fieldType = fieldInfo.FieldType;
				this.fieldName = fieldInfo.Name;
				if (this.explicitSetupMethod != null)
				{
					MethodInfo method = typeof(UserProfile.SaveFieldAttribute).GetMethod(this.explicitSetupMethod, BindingFlags.Instance | BindingFlags.Public);
					if (method == null)
					{
						Debug.LogErrorFormat("Explicit setup {0} specified by field {1} could not be found. Use the nameof() operator to ensure the method exists.", Array.Empty<object>());
						return;
					}
					if (method.GetParameters().Length > 1)
					{
						Debug.LogErrorFormat("Explicit setup method {0} for field {1} must have one parameter.", new object[]
						{
							this.explicitSetupMethod,
							fieldInfo.Name
						});
						return;
					}
					method.Invoke(this, new object[]
					{
						fieldInfo
					});
					return;
				}
				else
				{
					if (fieldType == typeof(string))
					{
						this.SetupString(fieldInfo);
						return;
					}
					if (fieldType == typeof(int))
					{
						this.SetupInt(fieldInfo);
						return;
					}
					if (fieldType == typeof(uint))
					{
						this.SetupUint(fieldInfo);
						return;
					}
					if (fieldType == typeof(float))
					{
						this.SetupFloat(fieldInfo);
						return;
					}
					if (fieldType == typeof(bool))
					{
						this.SetupBool(fieldInfo);
						return;
					}
					Debug.LogErrorFormat("No explicit setup method or supported type for save field {0}", new object[]
					{
						fieldInfo.Name
					});
					return;
				}
			}

			// Token: 0x06001BE3 RID: 7139 RVA: 0x0008A9C0 File Offset: 0x00088BC0
			public void SetupString(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => (string)fieldInfo.GetValue(userProfile));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					fieldInfo.SetValue(userProfile, valueString);
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001BE4 RID: 7140 RVA: 0x0008AA10 File Offset: 0x00088C10
			public void SetupFloat(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((float)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					float num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001BE5 RID: 7141 RVA: 0x0008AA60 File Offset: 0x00088C60
			public void SetupInt(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((int)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					int num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001BE6 RID: 7142 RVA: 0x0008AAB0 File Offset: 0x00088CB0
			public void SetupUint(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((uint)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					uint num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001BE7 RID: 7143 RVA: 0x0008AB00 File Offset: 0x00088D00
			public void SetupBool(FieldInfo fieldInfo)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					if (!(bool)fieldInfo.GetValue(userProfile))
					{
						return "0";
					}
					return "1";
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					int num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num > 0);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001BE8 RID: 7144 RVA: 0x0008AB50 File Offset: 0x00088D50
			public void SetupTokenList(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => string.Join(" ", (List<string>)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					List<string> list = (List<string>)fieldInfo.GetValue(userProfile);
					list.Clear();
					foreach (string item in valueString.Split(new char[]
					{
						' '
					}))
					{
						list.Add(item);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					List<string> src = (List<string>)fieldInfo.GetValue(srcProfile);
					List<string> dest = (List<string>)fieldInfo.GetValue(destProfile);
					Util.CopyList<string>(src, dest);
				};
			}

			// Token: 0x06001BE9 RID: 7145 RVA: 0x0008ABA0 File Offset: 0x00088DA0
			public void SetupPickupsSet(FieldInfo fieldInfo)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					bool[] pickupsSet = (bool[])fieldInfo.GetValue(userProfile);
					return string.Join(" ", from pickupIndex in PickupIndex.allPickups
					where pickupsSet[pickupIndex.value]
					select pickupIndex.ToString());
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					bool[] array = (bool[])fieldInfo.GetValue(userProfile);
					Array.Clear(array, 0, 0);
					string[] array2 = valueString.Split(new char[]
					{
						' '
					});
					for (int i = 0; i < array2.Length; i++)
					{
						PickupIndex pickupIndex = PickupIndex.Find(array2[i]);
						if (pickupIndex.isValid)
						{
							array[pickupIndex.value] = true;
						}
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					Array sourceArray = (bool[])fieldInfo.GetValue(srcProfile);
					bool[] array = (bool[])fieldInfo.GetValue(destProfile);
					Array.Copy(sourceArray, array, array.Length);
				};
			}

			// Token: 0x06001BEA RID: 7146 RVA: 0x00014A64 File Offset: 0x00012C64
			public void SetupKeyboardMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Keyboard);
			}

			// Token: 0x06001BEB RID: 7147 RVA: 0x00014A6E File Offset: 0x00012C6E
			public void SetupMouseMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Mouse);
			}

			// Token: 0x06001BEC RID: 7148 RVA: 0x00014A78 File Offset: 0x00012C78
			public void SetupJoystickMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Joystick);
			}

			// Token: 0x06001BED RID: 7149 RVA: 0x0008ABF0 File Offset: 0x00088DF0
			private void SetupControllerMap(FieldInfo fieldInfo, ControllerType controllerType)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					ControllerMap controllerMap = (ControllerMap)fieldInfo.GetValue(userProfile);
					return ((controllerMap != null) ? controllerMap.ToXmlString() : null) ?? string.Empty;
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					fieldInfo.SetValue(userProfile, ControllerMap.CreateFromXml(controllerType, valueString));
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					switch (controllerType)
					{
					case ControllerType.Keyboard:
						fieldInfo.SetValue(destProfile, new KeyboardMap((KeyboardMap)fieldInfo.GetValue(srcProfile)));
						return;
					case ControllerType.Mouse:
						fieldInfo.SetValue(destProfile, new MouseMap((MouseMap)fieldInfo.GetValue(srcProfile)));
						return;
					case ControllerType.Joystick:
						fieldInfo.SetValue(destProfile, new JoystickMap((JoystickMap)fieldInfo.GetValue(srcProfile)));
						return;
					default:
						throw new NotImplementedException();
					}
				};
			}

			// Token: 0x04001E4E RID: 7758
			public Action<UserProfile, string> setter;

			// Token: 0x04001E4F RID: 7759
			public Func<UserProfile, string> getter;

			// Token: 0x04001E50 RID: 7760
			public Action<UserProfile, UserProfile> copier;

			// Token: 0x04001E51 RID: 7761
			public string defaultValue = string.Empty;

			// Token: 0x04001E52 RID: 7762
			public string fieldName;

			// Token: 0x04001E53 RID: 7763
			public string explicitSetupMethod;

			// Token: 0x04001E54 RID: 7764
			private FieldInfo fieldInfo;
		}

		// Token: 0x020004D7 RID: 1239
		public struct TutorialProgression
		{
			// Token: 0x04001E61 RID: 7777
			public uint showCount;

			// Token: 0x04001E62 RID: 7778
			public bool shouldShow;
		}

		// Token: 0x020004D8 RID: 1240
		private static class XmlUtility
		{
			// Token: 0x06001C14 RID: 7188 RVA: 0x00014C2A File Offset: 0x00012E2A
			private static XElement CreateStringField(string name, string value)
			{
				return new XElement(name, new XText(value));
			}

			// Token: 0x06001C15 RID: 7189 RVA: 0x00014C3D File Offset: 0x00012E3D
			private static XElement CreateUintField(string name, uint value)
			{
				return new XElement(name, new XText(TextSerialization.ToStringInvariant(value)));
			}

			// Token: 0x06001C16 RID: 7190 RVA: 0x0008AF2C File Offset: 0x0008912C
			private static XElement CreateStatsField(string name, StatSheet statSheet)
			{
				XElement xelement = new XElement(name);
				for (int i = 0; i < statSheet.fields.Length; i++)
				{
					XElement xelement2 = new XElement("stat", new XText(statSheet.fields[i].ToString()));
					xelement2.SetAttributeValue("name", statSheet.fields[i].name);
					xelement.Add(xelement2);
				}
				int unlockableCount = statSheet.GetUnlockableCount();
				for (int j = 0; j < unlockableCount; j++)
				{
					UnlockableDef unlockable = statSheet.GetUnlockable(j);
					XElement content = new XElement("unlock", new XText(unlockable.name));
					xelement.Add(content);
				}
				return xelement;
			}

			// Token: 0x06001C17 RID: 7191 RVA: 0x0008AFF4 File Offset: 0x000891F4
			private static XElement FindElement(XElement parent, string name)
			{
				foreach (XElement xelement in parent.Descendants())
				{
					if (xelement.Name == name)
					{
						return xelement;
					}
				}
				return null;
			}

			// Token: 0x06001C18 RID: 7192 RVA: 0x0008B054 File Offset: 0x00089254
			private static uint GetUintField(XElement container, string fieldName, uint defaultValue)
			{
				XElement xelement = UserProfile.XmlUtility.FindElement(container, fieldName);
				if (xelement != null)
				{
					XNode firstNode = xelement.FirstNode;
					if (firstNode != null && firstNode.NodeType == XmlNodeType.Text)
					{
						uint result;
						if (!TextSerialization.TryParseInvariant(((XText)firstNode).Value, out result))
						{
							return defaultValue;
						}
						return result;
					}
				}
				return defaultValue;
			}

			// Token: 0x06001C19 RID: 7193 RVA: 0x0008B098 File Offset: 0x00089298
			private static string GetStringField(XElement container, string fieldName, string defaultValue)
			{
				XElement xelement = UserProfile.XmlUtility.FindElement(container, fieldName);
				if (xelement != null)
				{
					XNode firstNode = xelement.FirstNode;
					if (firstNode != null && firstNode.NodeType == XmlNodeType.Text)
					{
						return ((XText)firstNode).Value;
					}
				}
				return defaultValue;
			}

			// Token: 0x06001C1A RID: 7194 RVA: 0x0008B0D0 File Offset: 0x000892D0
			private static void GetStatsField(XElement container, string fieldName, StatSheet dest)
			{
				XElement xelement = container.Elements().FirstOrDefault((XElement element) => element.Name == fieldName);
				if (xelement == null)
				{
					return;
				}
				foreach (XElement xelement2 in from element in xelement.Elements()
				where element.Name == "stat"
				select element)
				{
					XAttribute xattribute = xelement2.Attributes().FirstOrDefault((XAttribute attribute) => attribute.Name == "name");
					string statName = (xattribute != null) ? xattribute.Value : null;
					XText xtext = xelement2.Nodes().FirstOrDefault((XNode node) => node.NodeType == XmlNodeType.Text) as XText;
					string value = (xtext != null) ? xtext.Value : null;
					dest.SetStatValueFromString(StatDef.Find(statName), value);
				}
				foreach (XElement xelement3 in from element in xelement.Elements()
				where element.Name == "unlock"
				select element)
				{
					XText xtext2 = xelement3.Nodes().FirstOrDefault((XNode node) => node.NodeType == XmlNodeType.Text) as XText;
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef((xtext2 != null) ? xtext2.Value : null);
					if (unlockableDef != null)
					{
						dest.AddUnlockable(unlockableDef);
					}
				}
			}

			// Token: 0x06001C1B RID: 7195 RVA: 0x0008B290 File Offset: 0x00089490
			public static XDocument ToXml(UserProfile userProfile)
			{
				object[] array = new object[UserProfile.saveFields.Length];
				for (int i = 0; i < UserProfile.saveFields.Length; i++)
				{
					UserProfile.SaveFieldAttribute saveFieldAttribute = UserProfile.saveFields[i];
					array[i] = UserProfile.XmlUtility.CreateStringField(saveFieldAttribute.fieldName, saveFieldAttribute.getter(userProfile));
				}
				object[] element = new object[]
				{
					UserProfile.XmlUtility.CreateStatsField("stats", userProfile.statSheet),
					UserProfile.XmlUtility.CreateUintField("tutorialDifficulty", userProfile.tutorialDifficulty.showCount),
					UserProfile.XmlUtility.CreateUintField("tutorialEquipment", userProfile.tutorialEquipment.showCount),
					UserProfile.XmlUtility.CreateUintField("tutorialSprint", userProfile.tutorialSprint.showCount)
				};
				return new XDocument(new object[]
				{
					new XElement("UserProfile", array.Append(element).ToArray<object>())
				});
			}

			// Token: 0x06001C1C RID: 7196 RVA: 0x0008B36C File Offset: 0x0008956C
			public static UserProfile FromXml(XDocument doc)
			{
				UserProfile userProfile = new UserProfile();
				XElement root = doc.Root;
				foreach (UserProfile.SaveFieldAttribute saveFieldAttribute in UserProfile.saveFields)
				{
					string stringField = UserProfile.XmlUtility.GetStringField(root, saveFieldAttribute.fieldName, null);
					if (stringField != null)
					{
						saveFieldAttribute.setter(userProfile, stringField);
					}
				}
				UserProfile.XmlUtility.GetStatsField(root, "stats", userProfile.statSheet);
				userProfile.tutorialDifficulty.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialDifficulty", userProfile.tutorialDifficulty.showCount);
				userProfile.tutorialEquipment.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialEquipment", userProfile.tutorialEquipment.showCount);
				userProfile.tutorialSprint.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialSprint", userProfile.tutorialSprint.showCount);
				return userProfile;
			}
		}
	}
}
