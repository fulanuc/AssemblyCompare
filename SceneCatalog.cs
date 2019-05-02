using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000485 RID: 1157
	public static class SceneCatalog
	{
		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060019FA RID: 6650 RVA: 0x00013362 File Offset: 0x00011562
		public static int sceneDefCount
		{
			get
			{
				return SceneCatalog.indexToSceneDef.Length;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060019FB RID: 6651 RVA: 0x0001336B File Offset: 0x0001156B
		public static IEnumerable<SceneDef> allSceneDefs
		{
			get
			{
				return SceneCatalog.indexToSceneDef;
			}
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x00013372 File Offset: 0x00011572
		[NotNull]
		public static SceneDef GetSceneDef(int sceneIndex)
		{
			return SceneCatalog.indexToSceneDef[sceneIndex];
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060019FD RID: 6653 RVA: 0x0001337B File Offset: 0x0001157B
		// (set) Token: 0x060019FE RID: 6654 RVA: 0x00013382 File Offset: 0x00011582
		[NotNull]
		public static SceneDef mostRecentSceneDef { get; private set; }

		// Token: 0x060019FF RID: 6655 RVA: 0x000858D4 File Offset: 0x00083AD4
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			SceneCatalog.indexToSceneDef = Resources.LoadAll<SceneDef>("SceneDefs/");
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
			{
				SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(newScene.name);
				if (SceneCatalog.currentSceneDef)
				{
					SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
				}
			};
			SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(SceneManager.GetActiveScene().name);
			SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
			SceneCatalog.availability.MakeAvailable();
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0001338A File Offset: 0x0001158A
		[NotNull]
		public static string GetUnlockableLogFromSceneName([NotNull] string name)
		{
			return string.Format(CultureInfo.InvariantCulture, "Logs.Stages.{0}", name);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0001339C File Offset: 0x0001159C
		[CanBeNull]
		public static SceneDef GetSceneDefForCurrentScene()
		{
			return SceneCatalog.GetSceneDefFromScene(SceneManager.GetActiveScene());
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x00085940 File Offset: 0x00083B40
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

		// Token: 0x06001A03 RID: 6659 RVA: 0x000133A8 File Offset: 0x000115A8
		[CanBeNull]
		public static SceneDef GetSceneDefFromScene(Scene scene)
		{
			return SceneCatalog.GetSceneDefFromSceneName(scene.name);
		}

		// Token: 0x04001D38 RID: 7480
		private static SceneDef[] indexToSceneDef;

		// Token: 0x04001D39 RID: 7481
		private static string currentSceneName = string.Empty;

		// Token: 0x04001D3A RID: 7482
		private static SceneDef currentSceneDef;

		// Token: 0x04001D3C RID: 7484
		public static ResourceAvailability availability;
	}
}
