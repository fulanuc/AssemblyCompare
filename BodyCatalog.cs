using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000206 RID: 518
	public static class BodyCatalog
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000A12 RID: 2578 RVA: 0x0000822D File Offset: 0x0000642D
		public static IEnumerable<GameObject> allBodyPrefabs
		{
			get
			{
				return BodyCatalog.bodyPrefabs;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000A13 RID: 2579 RVA: 0x00008234 File Offset: 0x00006434
		public static IEnumerable<CharacterBody> allBodyPrefabBodyBodyComponents
		{
			get
			{
				return BodyCatalog.bodyPrefabBodyComponents;
			}
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x0000823B File Offset: 0x0000643B
		public static GameObject GetBodyPrefab(int index)
		{
			if ((ulong)index < (ulong)((long)BodyCatalog.bodyPrefabs.Length))
			{
				return BodyCatalog.bodyPrefabs[index];
			}
			return null;
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00008252 File Offset: 0x00006452
		public static CharacterBody GetBodyPrefabBodyComponent(int index)
		{
			if ((ulong)index < (ulong)((long)BodyCatalog.bodyPrefabBodyComponents.Length))
			{
				return BodyCatalog.bodyPrefabBodyComponents[index];
			}
			return null;
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00046D68 File Offset: 0x00044F68
		public static int FindBodyIndex([NotNull] string bodyName)
		{
			int result;
			if (BodyCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00046D88 File Offset: 0x00044F88
		public static int FindBodyIndexCaseInsensitive([NotNull] string bodyName)
		{
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				if (string.Compare(BodyCatalog.bodyPrefabs[i].name, bodyName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00008269 File Offset: 0x00006469
		public static int FindBodyIndex(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return -1;
			}
			return BodyCatalog.FindBodyIndex(bodyObject.name);
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00046DC0 File Offset: 0x00044FC0
		public static GameObject FindBodyPrefab([NotNull] string bodyName)
		{
			int num = BodyCatalog.FindBodyIndex(bodyName);
			if (num != -1)
			{
				return BodyCatalog.GetBodyPrefab(num);
			}
			return null;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00046DE0 File Offset: 0x00044FE0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			BodyCatalog.bodyPrefabs = Resources.LoadAll<GameObject>("Prefabs/CharacterBodies/");
			BodyCatalog.bodyPrefabBodyComponents = new CharacterBody[BodyCatalog.bodyPrefabs.Length];
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				BodyCatalog.nameToIndexMap.Add(BodyCatalog.bodyPrefabs[i].name, i);
				BodyCatalog.nameToIndexMap.Add(BodyCatalog.bodyPrefabs[i].name + "(Clone)", i);
				BodyCatalog.bodyPrefabBodyComponents[i] = BodyCatalog.bodyPrefabs[i].GetComponent<CharacterBody>();
				Texture2D texture2D = Resources.Load<Texture2D>("Textures/BodyIcons/" + BodyCatalog.bodyPrefabs[i].name);
				if (texture2D)
				{
					BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon = texture2D;
				}
			}
			BodyCatalog.availability.MakeAvailable();
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00008280 File Offset: 0x00006480
		private static IEnumerator GeneratePortraits()
		{
			ModelPanel modelPanel = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/IconGenerator")).GetComponentInChildren<ModelPanel>();
			yield return new WaitForEndOfFrame();
			int num;
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i = num)
			{
				if (BodyCatalog.bodyPrefabBodyComponents[i] && (!BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon || BodyCatalog.bodyPrefabBodyComponents[i].portraitIcon.name == "texDifficultyNormalIcon"))
				{
					try
					{
						Debug.LogFormat("Generating portrait for {0}", new object[]
						{
							BodyCatalog.bodyPrefabs[i].name
						});
						ModelPanel modelPanel2 = modelPanel;
						ModelLocator component = BodyCatalog.bodyPrefabs[i].GetComponent<ModelLocator>();
						modelPanel2.modelPrefab = ((component != null) ? component.modelTransform.gameObject : null);
						modelPanel.SetAnglesForCharacterThumbnail(true);
					}
					catch (Exception message)
					{
						Debug.Log(message);
					}
					yield return new WaitForSeconds(1f);
					modelPanel.SetAnglesForCharacterThumbnail(true);
					yield return new WaitForEndOfFrame();
					yield return new WaitForEndOfFrame();
					try
					{
						Texture2D texture2D = new Texture2D(modelPanel.renderTexture.width, modelPanel.renderTexture.height, TextureFormat.ARGB32, false, false);
						RenderTexture active = RenderTexture.active;
						RenderTexture.active = modelPanel.renderTexture;
						texture2D.ReadPixels(new Rect(0f, 0f, (float)modelPanel.renderTexture.width, (float)modelPanel.renderTexture.height), 0, 0, false);
						RenderTexture.active = active;
						byte[] array = texture2D.EncodeToPNG();
						using (FileStream fileStream = new FileStream("Assets/RoR2/GeneratedPortraits/" + BodyCatalog.bodyPrefabs[i].name + ".png", FileMode.Create, FileAccess.Write))
						{
							fileStream.Write(array, 0, array.Length);
						}
					}
					catch (Exception message2)
					{
						Debug.Log(message2);
					}
					yield return new WaitForEndOfFrame();
				}
				num = i + 1;
			}
			UnityEngine.Object.Destroy(modelPanel.transform.root.gameObject);
			yield break;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00008288 File Offset: 0x00006488
		[ConCommand(commandName = "body_generate_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all bodies that are currently using the default.")]
		private static void CCBodyGeneratePortraits(ConCommandArgs args)
		{
			RoR2Application.instance.StartCoroutine(BodyCatalog.GeneratePortraits());
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00046EAC File Offset: 0x000450AC
		[ConCommand(commandName = "body_list", flags = ConVarFlags.None, helpText = "Prints a list of all character bodies in the game.")]
		private static void CCBodyList(ConCommandArgs args)
		{
			string[] array = new string[BodyCatalog.bodyPrefabs.Length];
			for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
			{
				array[i] = BodyCatalog.bodyPrefabs[i].name;
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x04000D65 RID: 3429
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x04000D66 RID: 3430
		private static GameObject[] bodyPrefabs;

		// Token: 0x04000D67 RID: 3431
		private static CharacterBody[] bodyPrefabBodyComponents;

		// Token: 0x04000D68 RID: 3432
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
