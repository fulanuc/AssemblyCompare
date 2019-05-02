using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000492 RID: 1170
	public static class SceneCatalog
	{
		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001A5C RID: 6748 RVA: 0x00013890 File Offset: 0x00011A90
		public static int sceneDefCount
		{
			get
			{
				return SceneCatalog.indexToSceneDef.Length;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001A5D RID: 6749 RVA: 0x00013899 File Offset: 0x00011A99
		public static IEnumerable<SceneDef> allSceneDefs
		{
			get
			{
				return SceneCatalog.indexToSceneDef;
			}
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x000138A0 File Offset: 0x00011AA0
		[NotNull]
		public static SceneDef GetSceneDef(int sceneIndex)
		{
			return SceneCatalog.indexToSceneDef[sceneIndex];
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x000138A9 File Offset: 0x00011AA9
		// (set) Token: 0x06001A60 RID: 6752 RVA: 0x000138B0 File Offset: 0x00011AB0
		[NotNull]
		public static SceneDef mostRecentSceneDef { get; private set; }

		// Token: 0x06001A61 RID: 6753 RVA: 0x000863A0 File Offset: 0x000845A0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			SceneCatalog.indexToSceneDef = Resources.LoadAll<SceneDef>("SceneDefs/");
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
			{
				SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(newScene.name);
				if (SceneCatalog.currentSceneDef != null)
				{
					SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
					Action<SceneDef> action = SceneCatalog.onMostRecentSceneDefChanged;
					if (action == null)
					{
						return;
					}
					action(SceneCatalog.mostRecentSceneDef);
				}
			};
			SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(SceneManager.GetActiveScene().name);
			SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
			SceneCatalog.availability.MakeAvailable();
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x000138B8 File Offset: 0x00011AB8
		[NotNull]
		public static string GetUnlockableLogFromSceneName([NotNull] string name)
		{
			return string.Format(CultureInfo.InvariantCulture, "Logs.Stages.{0}", name);
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x000138CA File Offset: 0x00011ACA
		[CanBeNull]
		public static SceneDef GetSceneDefForCurrentScene()
		{
			return SceneCatalog.GetSceneDefFromScene(SceneManager.GetActiveScene());
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0008640C File Offset: 0x0008460C
		[CanBeNull]
		public static SceneDef GetSceneDefFromSceneName([NotNull] string name)
		{
			for (int i = 0; i < SceneCatalog.indexToSceneDef.Length; i++)
			{
				if (SceneCatalog.indexToSceneDef[i].sceneName == name)
				{
					return SceneCatalog.indexToSceneDef[i];
				}
			}
			return null;
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x000138D6 File Offset: 0x00011AD6
		[CanBeNull]
		public static SceneDef GetSceneDefFromScene(Scene scene)
		{
			return SceneCatalog.GetSceneDefFromSceneName(scene.name);
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001A66 RID: 6758 RVA: 0x00086448 File Offset: 0x00084648
		// (remove) Token: 0x06001A67 RID: 6759 RVA: 0x0008647C File Offset: 0x0008467C
		public static event Action<SceneDef> onMostRecentSceneDefChanged;

		// Token: 0x04001D70 RID: 7536
		private static SceneDef[] indexToSceneDef;

		// Token: 0x04001D71 RID: 7537
		private static string currentSceneName = string.Empty;

		// Token: 0x04001D72 RID: 7538
		private static SceneDef currentSceneDef;

		// Token: 0x04001D74 RID: 7540
		public static ResourceAvailability availability;
	}
}
