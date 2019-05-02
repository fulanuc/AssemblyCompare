using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F2 RID: 498
	public static class AchievementManager
	{
		// Token: 0x060009C9 RID: 2505 RVA: 0x00045D50 File Offset: 0x00043F50
		public static UserAchievementManager GetUserAchievementManager([NotNull] LocalUser user)
		{
			UserAchievementManager result;
			AchievementManager.userToManagerMap.TryGetValue(user, out result);
			return result;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00045D6C File Offset: 0x00043F6C
		[SystemInitializer(new Type[]
		{
			typeof(UnlockableCatalog)
		})]
		private static void DoInit()
		{
			AchievementManager.CollectAchievementDefs(AchievementManager.achievementNamesToDefs);
			LocalUserManager.onUserSignIn += delegate(LocalUser localUser)
			{
				if (!localUser.userProfile.canSave)
				{
					return;
				}
				UserAchievementManager userAchievementManager = new UserAchievementManager();
				userAchievementManager.OnInstall(localUser);
				AchievementManager.userToManagerMap[localUser] = userAchievementManager;
			};
			LocalUserManager.onUserSignOut += delegate(LocalUser localUser)
			{
				UserAchievementManager userAchievementManager;
				if (AchievementManager.userToManagerMap.TryGetValue(localUser, out userAchievementManager))
				{
					userAchievementManager.OnUninstall();
					AchievementManager.userToManagerMap.Remove(localUser);
				}
			};
			RoR2Application.onUpdate += delegate()
			{
				foreach (KeyValuePair<LocalUser, UserAchievementManager> keyValuePair in AchievementManager.userToManagerMap)
				{
					keyValuePair.Value.Update();
				}
			};
			AchievementManager.availability.MakeAvailable();
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00007E3C File Offset: 0x0000603C
		public static void AddTask(Action action)
		{
			AchievementManager.taskQueue.Enqueue(action);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00007E49 File Offset: 0x00006049
		public static void ProcessTasks()
		{
			while (AchievementManager.taskQueue.Count > 0)
			{
				AchievementManager.taskQueue.Dequeue()();
			}
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00045DFC File Offset: 0x00043FFC
		public static AchievementDef GetAchievementDef(string achievementIdentifier)
		{
			AchievementDef result;
			if (AchievementManager.achievementNamesToDefs.TryGetValue(achievementIdentifier, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00007E69 File Offset: 0x00006069
		public static AchievementDef GetAchievementDef(AchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.achievementDefs.Length)
			{
				return AchievementManager.achievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00007E91 File Offset: 0x00006091
		public static AchievementDef GetAchievementDef(ServerAchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.serverAchievementDefs.Length)
			{
				return AchievementManager.serverAchievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00045E1C File Offset: 0x0004401C
		public static AchievementDef GetAchievementDefFromUnlockable(string unlockableRewardIdentifier)
		{
			for (int i = 0; i < AchievementManager.achievementDefs.Length; i++)
			{
				if (AchievementManager.achievementDefs[i].unlockableRewardIdentifier == unlockableRewardIdentifier)
				{
					return AchievementManager.achievementDefs[i];
				}
			}
			return null;
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x00007EB9 File Offset: 0x000060B9
		public static int achievementCount
		{
			get
			{
				return AchievementManager.achievementDefs.Length;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x00007EC2 File Offset: 0x000060C2
		public static int serverAchievementCount
		{
			get
			{
				return AchievementManager.serverAchievementDefs.Length;
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x00045E58 File Offset: 0x00044058
		public static void CollectAchievementDefs(Dictionary<string, AchievementDef> map)
		{
			List<AchievementDef> list = new List<AchievementDef>();
			map.Clear();
			foreach (Type type2 in from type in typeof(BaseAchievement).Assembly.GetTypes()
			where type.IsSubclassOf(typeof(BaseAchievement))
			orderby type.Name
			select type)
			{
				RegisterAchievementAttribute registerAchievementAttribute = (RegisterAchievementAttribute)type2.GetCustomAttributes(false).FirstOrDefault((object v) => v is RegisterAchievementAttribute);
				if (registerAchievementAttribute != null)
				{
					if (map.ContainsKey(registerAchievementAttribute.identifier))
					{
						Debug.LogErrorFormat("Class {0} attempted to register as achievement {1}, but class {2} has already registered as that achievement.", new object[]
						{
							type2.FullName,
							registerAchievementAttribute.identifier,
							AchievementManager.achievementNamesToDefs[registerAchievementAttribute.identifier].type.FullName
						});
					}
					else
					{
						AchievementDef achievementDef = new AchievementDef
						{
							identifier = registerAchievementAttribute.identifier,
							unlockableRewardIdentifier = registerAchievementAttribute.unlockableRewardIdentifier,
							prerequisiteAchievementIdentifier = registerAchievementAttribute.prerequisiteAchievementIdentifier,
							nameToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper(CultureInfo.InvariantCulture) + "_NAME",
							descriptionToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper(CultureInfo.InvariantCulture) + "_DESCRIPTION",
							iconPath = "Textures/AchievementIcons/tex" + registerAchievementAttribute.identifier + "Icon",
							type = type2,
							serverTrackerType = registerAchievementAttribute.serverTrackerType
						};
						AchievementManager.achievementIdentifiers.Add(registerAchievementAttribute.identifier);
						map.Add(registerAchievementAttribute.identifier, achievementDef);
						list.Add(achievementDef);
						UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
						if (unlockableDef != null)
						{
							unlockableDef.getHowToUnlockString = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
							{
								Language.GetString(achievementDef.nameToken),
								Language.GetString(achievementDef.descriptionToken)
							}));
							unlockableDef.getUnlockedString = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
							{
								unlockableDef.getHowToUnlockString()
							}));
						}
					}
				}
			}
			AchievementManager.achievementDefs = list.ToArray();
			AchievementManager.SortAchievements(AchievementManager.achievementDefs);
			AchievementManager.serverAchievementDefs = (from achievementDef in AchievementManager.achievementDefs
			where achievementDef.serverTrackerType != null
			select achievementDef).ToArray<AchievementDef>();
			for (int i = 0; i < AchievementManager.achievementDefs.Length; i++)
			{
				AchievementManager.achievementDefs[i].index = new AchievementIndex
				{
					intValue = i
				};
			}
			for (int j = 0; j < AchievementManager.serverAchievementDefs.Length; j++)
			{
				AchievementManager.serverAchievementDefs[j].serverIndex = new ServerAchievementIndex
				{
					intValue = j
				};
			}
			for (int k = 0; k < AchievementManager.achievementIdentifiers.Count; k++)
			{
				string currentAchievementIdentifier = AchievementManager.achievementIdentifiers[k];
				map[currentAchievementIdentifier].childAchievementIdentifiers = (from v in AchievementManager.achievementIdentifiers
				where map[v].prerequisiteAchievementIdentifier == currentAchievementIdentifier
				select v).ToArray<string>();
			}
			Action action = AchievementManager.onAchievementsRegistered;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x00046230 File Offset: 0x00044430
		private static void SortAchievements(AchievementDef[] achievementDefsArray)
		{
			AchievementManager.AchievementSortPair[] array = new AchievementManager.AchievementSortPair[achievementDefsArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new AchievementManager.AchievementSortPair
				{
					score = UnlockableCatalog.GetUnlockableSortScore(achievementDefsArray[i].unlockableRewardIdentifier),
					achievementDef = achievementDefsArray[i]
				};
			}
			Array.Sort<AchievementManager.AchievementSortPair>(array, (AchievementManager.AchievementSortPair a, AchievementManager.AchievementSortPair b) => a.score - b.score);
			for (int j = 0; j < array.Length; j++)
			{
				achievementDefsArray[j] = array[j].achievementDef;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060009D5 RID: 2517 RVA: 0x000462C4 File Offset: 0x000444C4
		// (remove) Token: 0x060009D6 RID: 2518 RVA: 0x000462F8 File Offset: 0x000444F8
		public static event Action onAchievementsRegistered;

		// Token: 0x060009D7 RID: 2519 RVA: 0x0004632C File Offset: 0x0004452C
		public static AchievementManager.Enumerator GetEnumerator()
		{
			return default(AchievementManager.Enumerator);
		}

		// Token: 0x04000CFE RID: 3326
		private static readonly Dictionary<LocalUser, UserAchievementManager> userToManagerMap = new Dictionary<LocalUser, UserAchievementManager>();

		// Token: 0x04000CFF RID: 3327
		public static ResourceAvailability availability;

		// Token: 0x04000D00 RID: 3328
		private static readonly Queue<Action> taskQueue = new Queue<Action>();

		// Token: 0x04000D01 RID: 3329
		private static readonly Dictionary<string, AchievementDef> achievementNamesToDefs = new Dictionary<string, AchievementDef>();

		// Token: 0x04000D02 RID: 3330
		private static readonly List<string> achievementIdentifiers = new List<string>();

		// Token: 0x04000D03 RID: 3331
		public static readonly ReadOnlyCollection<string> readOnlyAchievementIdentifiers = AchievementManager.achievementIdentifiers.AsReadOnly();

		// Token: 0x04000D04 RID: 3332
		private static AchievementDef[] achievementDefs;

		// Token: 0x04000D05 RID: 3333
		private static AchievementDef[] serverAchievementDefs;

		// Token: 0x04000D07 RID: 3335
		public static readonly GenericStaticEnumerable<AchievementDef, AchievementManager.Enumerator> allAchievementDefs;

		// Token: 0x020001F3 RID: 499
		private struct AchievementSortPair
		{
			// Token: 0x04000D08 RID: 3336
			public int score;

			// Token: 0x04000D09 RID: 3337
			public AchievementDef achievementDef;
		}

		// Token: 0x020001F4 RID: 500
		public struct Enumerator : IEnumerator<AchievementDef>, IEnumerator, IDisposable
		{
			// Token: 0x060009D9 RID: 2521 RVA: 0x00007F04 File Offset: 0x00006104
			public bool MoveNext()
			{
				this.position++;
				return this.position < AchievementManager.achievementDefs.Length;
			}

			// Token: 0x060009DA RID: 2522 RVA: 0x00007F23 File Offset: 0x00006123
			public void Reset()
			{
				this.position = -1;
			}

			// Token: 0x170000A7 RID: 167
			// (get) Token: 0x060009DB RID: 2523 RVA: 0x00007F2C File Offset: 0x0000612C
			public AchievementDef Current
			{
				get
				{
					return AchievementManager.achievementDefs[this.position];
				}
			}

			// Token: 0x170000A8 RID: 168
			// (get) Token: 0x060009DC RID: 2524 RVA: 0x00007F3A File Offset: 0x0000613A
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x060009DD RID: 2525 RVA: 0x000025DA File Offset: 0x000007DA
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04000D0A RID: 3338
			private int position;
		}
	}
}
