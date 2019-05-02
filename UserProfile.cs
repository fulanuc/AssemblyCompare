using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
	// Token: 0x020004D9 RID: 1241
	public class UserProfile
	{
		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06001C0B RID: 7179 RVA: 0x00014CBB File Offset: 0x00012EBB
		// (set) Token: 0x06001C0C RID: 7180 RVA: 0x00014CC3 File Offset: 0x00012EC3
		public bool isCorrupted { get; private set; }

		// Token: 0x06001C0D RID: 7181 RVA: 0x0008A568 File Offset: 0x00088768
		public bool HasUnlockable(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			return unlockableDef == null || this.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x00014CCC File Offset: 0x00012ECC
		public bool HasUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			return this.statSheet.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x0008A588 File Offset: 0x00088788
		public void AddUnlockToken(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef != null)
			{
				this.GrantUnlockable(unlockableDef);
			}
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x0008A5A8 File Offset: 0x000887A8
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

		// Token: 0x06001C11 RID: 7185 RVA: 0x00014CDA File Offset: 0x00012EDA
		public void RevokeUnlockable(UnlockableDef unlockableDef)
		{
			if (this.statSheet.HasUnlockable(unlockableDef))
			{
				this.statSheet.RemoveUnlockable(unlockableDef.index);
			}
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x0008A610 File Offset: 0x00088810
		public bool HasSurvivorUnlocked(SurvivorIndex survivorIndex)
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
			return survivorDef != null && (survivorDef.unlockableName == "" || this.HasUnlockable(survivorDef.unlockableName));
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00014CFB File Offset: 0x00012EFB
		public bool HasDiscoveredPickup(PickupIndex pickupIndex)
		{
			return pickupIndex.isValid && this.discoveredPickups[pickupIndex.value];
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x00014D15 File Offset: 0x00012F15
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

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x06001C15 RID: 7189 RVA: 0x0008A64C File Offset: 0x0008884C
		// (remove) Token: 0x06001C16 RID: 7190 RVA: 0x0008A684 File Offset: 0x00088884
		public event Action<PickupIndex> onPickupDiscovered;

		// Token: 0x06001C17 RID: 7191 RVA: 0x00014D48 File Offset: 0x00012F48
		public bool HasAchievement(string achievementName)
		{
			return this.achievementsList.Contains(achievementName);
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x0008A6BC File Offset: 0x000888BC
		public bool CanSeeAchievement(string achievementName)
		{
			if (this.HasAchievement(achievementName))
			{
				return true;
			}
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementName);
			return achievementDef != null && (string.IsNullOrEmpty(achievementDef.prerequisiteAchievementIdentifier) || this.HasAchievement(achievementDef.prerequisiteAchievementIdentifier));
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x00014D56 File Offset: 0x00012F56
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

		// Token: 0x06001C1A RID: 7194 RVA: 0x00014D8C File Offset: 0x00012F8C
		public void RevokeAchievement(string achievementName)
		{
			this.achievementsList.Remove(achievementName);
			this.unviewedAchievementsList.Remove(achievementName);
			this.RequestSave(false);
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06001C1B RID: 7195 RVA: 0x00014DAF File Offset: 0x00012FAF
		public bool hasUnviewedAchievement
		{
			get
			{
				return this.unviewedAchievementsList.Count > 0;
			}
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x00014DBF File Offset: 0x00012FBF
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

		// Token: 0x06001C1D RID: 7197 RVA: 0x0008A6FC File Offset: 0x000888FC
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

		// Token: 0x06001C1E RID: 7198 RVA: 0x0008A7AC File Offset: 0x000889AC
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

		// Token: 0x06001C1F RID: 7199 RVA: 0x0008A7EC File Offset: 0x000889EC
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

		// Token: 0x06001C20 RID: 7200 RVA: 0x0008A830 File Offset: 0x00088A30
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

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06001C21 RID: 7201 RVA: 0x0008A87C File Offset: 0x00088A7C
		// (remove) Token: 0x06001C22 RID: 7202 RVA: 0x0008A8B4 File Offset: 0x00088AB4
		public event Action onStatsReceived;

		// Token: 0x06001C23 RID: 7203 RVA: 0x00014DE8 File Offset: 0x00012FE8
		private void ResetShouldShowTutorial(ref UserProfile.TutorialProgression tutorialProgression)
		{
			tutorialProgression.shouldShow = (tutorialProgression.showCount < 3u);
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x00014DF9 File Offset: 0x00012FF9
		private void RebuildTutorialProgressions()
		{
			this.ResetShouldShowTutorial(ref this.tutorialDifficulty);
			this.ResetShouldShowTutorial(ref this.tutorialSprint);
			this.ResetShouldShowTutorial(ref this.tutorialEquipment);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x00014E1F File Offset: 0x0001301F
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			UserProfile.GenerateSaveFieldFunctions();
			RoR2Application.onUpdate += UserProfile.StaticUpdate;
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x0008A8EC File Offset: 0x00088AEC
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

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x00014E37 File Offset: 0x00013037
		// (set) Token: 0x06001C28 RID: 7208 RVA: 0x00014E3F File Offset: 0x0001303F
		public bool loggedIn { get; private set; }

		// Token: 0x06001C29 RID: 7209 RVA: 0x0008A9C4 File Offset: 0x00088BC4
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

		// Token: 0x06001C2A RID: 7210 RVA: 0x00014E48 File Offset: 0x00013048
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

		// Token: 0x06001C2B RID: 7211 RVA: 0x0008AA5C File Offset: 0x00088C5C
		public static void HandleShutDown()
		{
			foreach (UserProfile userProfile in UserProfile.loggedInProfiles)
			{
				userProfile.RequestSave(true);
			}
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x0008AAAC File Offset: 0x00088CAC
		public static void LoadUserProfiles()
		{
			UserProfile.loadedUserProfiles.Clear();
			UserProfile.LoadDefaultProfile();
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
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x0008AB5C File Offset: 0x00088D5C
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

		// Token: 0x06001C2E RID: 7214 RVA: 0x0008ABD0 File Offset: 0x00088DD0
		public static UserProfile GetProfile(string profileName)
		{
			profileName = profileName.ToLower(CultureInfo.InvariantCulture);
			UserProfile result;
			if (UserProfile.loadedUserProfiles.TryGetValue(profileName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x00014E86 File Offset: 0x00013086
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

		// Token: 0x06001C30 RID: 7216 RVA: 0x0008ABFC File Offset: 0x00088DFC
		private bool Save(bool blocking)
		{
			bool result;
			try
			{
				UserProfile.<>c__DisplayClass92_0 CS$<>8__locals1 = new UserProfile.<>c__DisplayClass92_0();
				Task task = this.currentWriteTask;
				if (task != null)
				{
					task.Wait();
				}
				Debug.LogFormat("Saving profile \"{0}\"...", new object[]
				{
					this.fileName
				});
				if (!this.fileSystem.FileExists(this.filePath))
				{
					blocking = true;
				}
				CS$<>8__locals1.stream = this.fileSystem.OpenFile(this.filePath, FileMode.Create, FileAccess.Write, FileShare.None);
				CS$<>8__locals1.tempCopy = new UserProfile();
				UserProfile.Copy(this, CS$<>8__locals1.tempCopy);
				this.currentWriteTask = new Task(new Action(CS$<>8__locals1.<Save>g__WriteAction|0));
				this.currentWriteTask.Start(TaskScheduler.Default);
				if (blocking)
				{
					Task task2 = this.currentWriteTask;
					if (task2 != null)
					{
						task2.Wait();
					}
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

		// Token: 0x06001C31 RID: 7217 RVA: 0x0008ACDC File Offset: 0x00088EDC
		private static void SkipBOM(Stream stream)
		{
			long position = stream.Position;
			if (stream.Length - position < 3L)
			{
				return;
			}
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			if (num == 255 && num2 == 254)
			{
				Debug.Log("Skipping UTF-8 BOM");
				return;
			}
			int num3 = stream.ReadByte();
			if (num == 239 && num2 == 187 && num3 == 191)
			{
				Debug.Log("Skipping UTF-16 BOM");
				return;
			}
			stream.Position = position;
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x0008AD58 File Offset: 0x00088F58
		private static UserProfile LoadUserProfileFromDisk(IFileSystem fileSystem, UPath path)
		{
			Debug.LogFormat("Attempting to load user profile {0}", new object[]
			{
				path
			});
			UserProfile result;
			try
			{
				using (Stream stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					UserProfile.SkipBOM(stream);
					using (TextReader textReader = new StreamReader(stream, Encoding.UTF8))
					{
						Debug.LogFormat("stream.Length={0}", new object[]
						{
							stream.Length
						});
						UserProfile userProfile = UserProfile.XmlUtility.FromXml(XDocument.Load(textReader));
						userProfile.fileName = path.GetNameWithoutExtension();
						userProfile.canSave = true;
						userProfile.fileSystem = fileSystem;
						userProfile.filePath = path;
						result = userProfile;
					}
				}
			}
			catch (XmlException ex)
			{
				Debug.LogFormat("Failed to load user profile {0}: {1}", new object[]
				{
					path,
					ex.Message
				});
				UserProfile userProfile2 = UserProfile.CreateGuestProfile();
				userProfile2.fileSystem = fileSystem;
				userProfile2.filePath = path;
				userProfile2.fileName = path.GetNameWithoutExtension();
				userProfile2.name = string.Format("<color=#FF7F7FFF>Corrupted Profile: {0}</color>", userProfile2.fileName);
				userProfile2.canSave = false;
				userProfile2.isCorrupted = true;
				result = userProfile2;
			}
			catch (Exception ex2)
			{
				Debug.LogFormat("Failed to load user profile {0}: {1}", new object[]
				{
					path,
					ex2.Message
				});
				result = null;
			}
			return result;
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x0008AED4 File Offset: 0x000890D4
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

		// Token: 0x06001C34 RID: 7220 RVA: 0x0008AF80 File Offset: 0x00089180
		private static void DeleteUserProfile(string fileName)
		{
			fileName = fileName.ToLower(CultureInfo.InvariantCulture);
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

		// Token: 0x06001C35 RID: 7221 RVA: 0x00014EA4 File Offset: 0x000130A4
		public static XDocument ToXml(UserProfile userProfile)
		{
			return UserProfile.XmlUtility.ToXml(userProfile);
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00014EAC File Offset: 0x000130AC
		private static UserProfile FromXml(XDocument doc)
		{
			return UserProfile.XmlUtility.FromXml(doc);
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x0008AFE8 File Offset: 0x000891E8
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

		// Token: 0x06001C38 RID: 7224 RVA: 0x0008B084 File Offset: 0x00089284
		public static UserProfile CreateGuestProfile()
		{
			UserProfile userProfile = new UserProfile();
			UserProfile.Copy(UserProfile.defaultProfile, userProfile);
			userProfile.name = "Guest";
			return userProfile;
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x0008B0B0 File Offset: 0x000892B0
		[ConCommand(commandName = "user_profile_save", flags = ConVarFlags.None, helpText = "Saves the named profile to disk, if it exists.")]
		private static void CCUserProfileSave(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string text = args[0].ToLower(CultureInfo.InvariantCulture);
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

		// Token: 0x06001C3A RID: 7226 RVA: 0x0008B118 File Offset: 0x00089318
		[ConCommand(commandName = "user_profile_copy", flags = ConVarFlags.None, helpText = "Copies the profile named by the first argument to a new profile named by the second argument. This does not save the profile.")]
		private static void CCUserProfileCopy(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			string text = args[0].ToLower(CultureInfo.InvariantCulture);
			string text2 = args[1].ToLower(CultureInfo.InvariantCulture);
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

		// Token: 0x06001C3B RID: 7227 RVA: 0x0008B1FC File Offset: 0x000893FC
		[ConCommand(commandName = "user_profile_delete", flags = ConVarFlags.None, helpText = "Unloads the named user profile and deletes it from the disk if it exists.")]
		private static void CCUserProfileDelete(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string a = args[0].ToLower(CultureInfo.InvariantCulture);
			if (a == "default")
			{
				Debug.Log("Cannot delete profile \"default\", it is a reserved profile.");
				return;
			}
			UserProfile.DeleteUserProfile(a);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x0008B244 File Offset: 0x00089444
		[ConCommand(commandName = "create_corrupted_profiles", flags = ConVarFlags.None, helpText = "Creates corrupted user profiles.")]
		private static void CCCreateCorruptedProfiles(ConCommandArgs args)
		{
			UserProfile.<>c__DisplayClass107_0 CS$<>8__locals1;
			CS$<>8__locals1.fileSystem = RoR2Application.cloudStorage;
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|107_0("empty", "", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|107_0("truncated", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<UserProfile>\r\n", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|107_0("multiroot", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<UserProfile>\r\n</UserProfile>\r\n<UserProfile>\r\n</UserProfile>", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|107_0("outoforder", "<?xml version=\"1.0\" encodi=\"utf-8\"ng?>\r\n<Userrofile>\r\n<UserProfile>\r\n</UserProfileProfile>\r\n</UserP>", ref CS$<>8__locals1);
		}

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06001C3D RID: 7229 RVA: 0x0008B2A4 File Offset: 0x000894A4
		// (remove) Token: 0x06001C3E RID: 7230 RVA: 0x0008B2D8 File Offset: 0x000894D8
		public static event Action onAvailableUserProfilesChanged;

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06001C3F RID: 7231 RVA: 0x0008B30C File Offset: 0x0008950C
		// (remove) Token: 0x06001C40 RID: 7232 RVA: 0x0008B340 File Offset: 0x00089540
		public static event Action<UserProfile, string> onUnlockableGranted;

		// Token: 0x06001C41 RID: 7233 RVA: 0x00014EB4 File Offset: 0x000130B4
		private static void LoadDefaultProfile()
		{
			UserProfile.defaultProfile = UserProfile.XmlUtility.FromXml(XDocument.Parse("<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>"));
			UserProfile.defaultProfile.canSave = false;
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x00014ED5 File Offset: 0x000130D5
		public bool HasViewedViewable(string viewableName)
		{
			return this.viewedViewables.Contains(viewableName);
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00014EE3 File Offset: 0x000130E3
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

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06001C44 RID: 7236 RVA: 0x0008B374 File Offset: 0x00089574
		// (remove) Token: 0x06001C45 RID: 7237 RVA: 0x0008B3A8 File Offset: 0x000895A8
		public static event Action<UserProfile> onUserProfileViewedViewablesChanged;

		// Token: 0x04001E52 RID: 7762
		public bool isClaimed;

		// Token: 0x04001E53 RID: 7763
		public bool canSave;

		// Token: 0x04001E54 RID: 7764
		public string fileName;

		// Token: 0x04001E55 RID: 7765
		public IFileSystem fileSystem;

		// Token: 0x04001E56 RID: 7766
		public UPath filePath = UPath.Empty;

		// Token: 0x04001E57 RID: 7767
		[UserProfile.SaveFieldAttribute]
		public string name;

		// Token: 0x04001E58 RID: 7768
		[UserProfile.SaveFieldAttribute]
		public uint coins;

		// Token: 0x04001E59 RID: 7769
		[UserProfile.SaveFieldAttribute]
		public uint totalCollectedCoins;

		// Token: 0x04001E5B RID: 7771
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		public List<string> viewedUnlockablesList = new List<string>();

		// Token: 0x04001E5C RID: 7772
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupPickupsSet")]
		private readonly bool[] discoveredPickups = new bool[106];

		// Token: 0x04001E5E RID: 7774
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> achievementsList = new List<string>();

		// Token: 0x04001E5F RID: 7775
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> unviewedAchievementsList = new List<string>();

		// Token: 0x04001E60 RID: 7776
		[UserProfile.SaveFieldAttribute]
		public string version = "2";

		// Token: 0x04001E61 RID: 7777
		[UserProfile.SaveFieldAttribute]
		public float screenShakeScale = 1f;

		// Token: 0x04001E62 RID: 7778
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupKeyboardMap")]
		public KeyboardMap keyboardMap = new KeyboardMap(DefaultControllerMaps.defaultKeyboardMap);

		// Token: 0x04001E63 RID: 7779
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupMouseMap")]
		public MouseMap mouseMap = new MouseMap(DefaultControllerMaps.defaultMouseMap);

		// Token: 0x04001E64 RID: 7780
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupJoystickMap")]
		public JoystickMap joystickMap = new JoystickMap(DefaultControllerMaps.defaultJoystickMap);

		// Token: 0x04001E65 RID: 7781
		[UserProfile.SaveFieldAttribute]
		public float mouseLookSensitivity = 0.25f;

		// Token: 0x04001E66 RID: 7782
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleX = 1f;

		// Token: 0x04001E67 RID: 7783
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleY = 1f;

		// Token: 0x04001E68 RID: 7784
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertX;

		// Token: 0x04001E69 RID: 7785
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertY;

		// Token: 0x04001E6A RID: 7786
		[UserProfile.SaveFieldAttribute]
		public float stickLookSensitivity = 4f;

		// Token: 0x04001E6B RID: 7787
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleX = 1f;

		// Token: 0x04001E6C RID: 7788
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleY = 1f;

		// Token: 0x04001E6D RID: 7789
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertX;

		// Token: 0x04001E6E RID: 7790
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertY;

		// Token: 0x04001E6F RID: 7791
		[UserProfile.SaveFieldAttribute]
		public float gamepadVibrationScale = 1f;

		// Token: 0x04001E70 RID: 7792
		private static string[] saveFieldNames;

		// Token: 0x04001E71 RID: 7793
		private static UserProfile.SaveFieldAttribute[] saveFields;

		// Token: 0x04001E72 RID: 7794
		private static readonly Dictionary<string, UserProfile.SaveFieldAttribute> nameToSaveFieldMap = new Dictionary<string, UserProfile.SaveFieldAttribute>();

		// Token: 0x04001E73 RID: 7795
		public StatSheet statSheet = StatSheet.New();

		// Token: 0x04001E75 RID: 7797
		private const uint maxShowCount = 3u;

		// Token: 0x04001E76 RID: 7798
		public UserProfile.TutorialProgression tutorialDifficulty;

		// Token: 0x04001E77 RID: 7799
		public UserProfile.TutorialProgression tutorialSprint;

		// Token: 0x04001E78 RID: 7800
		public UserProfile.TutorialProgression tutorialEquipment;

		// Token: 0x04001E79 RID: 7801
		[UserProfile.SaveFieldAttribute]
		public uint totalLoginSeconds;

		// Token: 0x04001E7A RID: 7802
		[UserProfile.SaveFieldAttribute]
		public uint totalRunSeconds;

		// Token: 0x04001E7B RID: 7803
		[UserProfile.SaveFieldAttribute]
		public uint totalAliveSeconds;

		// Token: 0x04001E7C RID: 7804
		[UserProfile.SaveFieldAttribute]
		public uint totalRunCount;

		// Token: 0x04001E7D RID: 7805
		private static float secondAccumulator;

		// Token: 0x04001E7E RID: 7806
		private static readonly List<UserProfile> loggedInProfiles = new List<UserProfile>();

		// Token: 0x04001E80 RID: 7808
		private static UPath userProfilesFolder = "/UserProfiles";

		// Token: 0x04001E81 RID: 7809
		private static readonly Dictionary<string, UserProfile> loadedUserProfiles = new Dictionary<string, UserProfile>();

		// Token: 0x04001E82 RID: 7810
		private bool saveRequestPending;

		// Token: 0x04001E83 RID: 7811
		private Task currentWriteTask;

		// Token: 0x04001E84 RID: 7812
		private static readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

		// Token: 0x04001E85 RID: 7813
		public static UserProfile defaultProfile;

		// Token: 0x04001E88 RID: 7816
		private const string defaultProfileContents = "<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>";

		// Token: 0x04001E89 RID: 7817
		[UserProfile.SaveFieldAttribute(defaultValue = "", explicitSetupMethod = "SetupTokenList", fieldName = "viewedViewables")]
		private readonly List<string> viewedViewables = new List<string>();

		// Token: 0x020004DA RID: 1242
		public class SaveFieldAttribute : Attribute
		{
			// Token: 0x06001C49 RID: 7241 RVA: 0x0008B5B4 File Offset: 0x000897B4
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

			// Token: 0x06001C4A RID: 7242 RVA: 0x0008B6F0 File Offset: 0x000898F0
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

			// Token: 0x06001C4B RID: 7243 RVA: 0x0008B740 File Offset: 0x00089940
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

			// Token: 0x06001C4C RID: 7244 RVA: 0x0008B790 File Offset: 0x00089990
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

			// Token: 0x06001C4D RID: 7245 RVA: 0x0008B7E0 File Offset: 0x000899E0
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

			// Token: 0x06001C4E RID: 7246 RVA: 0x0008B830 File Offset: 0x00089A30
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

			// Token: 0x06001C4F RID: 7247 RVA: 0x0008B880 File Offset: 0x00089A80
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

			// Token: 0x06001C50 RID: 7248 RVA: 0x0008B8D0 File Offset: 0x00089AD0
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

			// Token: 0x06001C51 RID: 7249 RVA: 0x00014F13 File Offset: 0x00013113
			public void SetupKeyboardMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Keyboard);
			}

			// Token: 0x06001C52 RID: 7250 RVA: 0x00014F1D File Offset: 0x0001311D
			public void SetupMouseMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Mouse);
			}

			// Token: 0x06001C53 RID: 7251 RVA: 0x00014F27 File Offset: 0x00013127
			public void SetupJoystickMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Joystick);
			}

			// Token: 0x06001C54 RID: 7252 RVA: 0x0008B920 File Offset: 0x00089B20
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

			// Token: 0x04001E8B RID: 7819
			public Action<UserProfile, string> setter;

			// Token: 0x04001E8C RID: 7820
			public Func<UserProfile, string> getter;

			// Token: 0x04001E8D RID: 7821
			public Action<UserProfile, UserProfile> copier;

			// Token: 0x04001E8E RID: 7822
			public string defaultValue = string.Empty;

			// Token: 0x04001E8F RID: 7823
			public string fieldName;

			// Token: 0x04001E90 RID: 7824
			public string explicitSetupMethod;

			// Token: 0x04001E91 RID: 7825
			private FieldInfo fieldInfo;
		}

		// Token: 0x020004E5 RID: 1253
		public struct TutorialProgression
		{
			// Token: 0x04001E9E RID: 7838
			public uint showCount;

			// Token: 0x04001E9F RID: 7839
			public bool shouldShow;
		}

		// Token: 0x020004E6 RID: 1254
		private static class XmlUtility
		{
			// Token: 0x06001C7B RID: 7291 RVA: 0x000150D9 File Offset: 0x000132D9
			private static XElement CreateStringField(string name, string value)
			{
				return new XElement(name, new XText(value));
			}

			// Token: 0x06001C7C RID: 7292 RVA: 0x000150EC File Offset: 0x000132EC
			private static XElement CreateUintField(string name, uint value)
			{
				return new XElement(name, new XText(TextSerialization.ToStringInvariant(value)));
			}

			// Token: 0x06001C7D RID: 7293 RVA: 0x0008BC5C File Offset: 0x00089E5C
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

			// Token: 0x06001C7E RID: 7294 RVA: 0x0008BD24 File Offset: 0x00089F24
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

			// Token: 0x06001C7F RID: 7295 RVA: 0x0008BD84 File Offset: 0x00089F84
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

			// Token: 0x06001C80 RID: 7296 RVA: 0x0008BDC8 File Offset: 0x00089FC8
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

			// Token: 0x06001C81 RID: 7297 RVA: 0x0008BE00 File Offset: 0x0008A000
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

			// Token: 0x06001C82 RID: 7298 RVA: 0x0008BFC0 File Offset: 0x0008A1C0
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

			// Token: 0x06001C83 RID: 7299 RVA: 0x0008C09C File Offset: 0x0008A29C
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
