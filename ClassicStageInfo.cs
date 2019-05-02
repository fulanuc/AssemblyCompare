using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029F RID: 671
	[RequireComponent(typeof(SceneInfo))]
	public class ClassicStageInfo : MonoBehaviour
	{
		// Token: 0x06000DB5 RID: 3509 RVA: 0x00055584 File Offset: 0x00053784
		private WeightedSelection<DirectorCard> GenerateDirectorCardWeightedSelection(DirectorCardCategorySelection categorySelection)
		{
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			foreach (DirectorCardCategorySelection.Category category in categorySelection.categories)
			{
				float num = categorySelection.SumAllWeightsInCategory(category);
				foreach (DirectorCard directorCard in category.cards)
				{
					weightedSelection.AddChoice(directorCard, (float)directorCard.selectionWeight / num * category.selectionWeight);
				}
			}
			return weightedSelection;
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000DB6 RID: 3510 RVA: 0x0000AB04 File Offset: 0x00008D04
		// (set) Token: 0x06000DB7 RID: 3511 RVA: 0x0000AB0B File Offset: 0x00008D0B
		public static ClassicStageInfo instance { get; private set; }

		// Token: 0x06000DB8 RID: 3512 RVA: 0x000555FC File Offset: 0x000537FC
		private void Awake()
		{
			this.interactableSelection = this.GenerateDirectorCardWeightedSelection(this.interactableCategories);
			this.monsterSelection = this.GenerateDirectorCardWeightedSelection(this.monsterCategories);
			if (NetworkServer.active && Util.CheckRoll(2f, 0f, null))
			{
				Debug.Log("Trying to find family selection...");
				WeightedSelection<ClassicStageInfo.MonsterFamily> weightedSelection = new WeightedSelection<ClassicStageInfo.MonsterFamily>(8);
				for (int i = 0; i < this.possibleMonsterFamilies.Length; i++)
				{
					if (this.possibleMonsterFamilies[i].minimumStageCompletion <= Run.instance.stageClearCount && this.possibleMonsterFamilies[i].maximumStageCompletion > Run.instance.stageClearCount)
					{
						weightedSelection.AddChoice(this.possibleMonsterFamilies[i], this.possibleMonsterFamilies[i].selectionWeight);
					}
				}
				if (weightedSelection.Count > 0)
				{
					ClassicStageInfo.MonsterFamily monsterFamily = weightedSelection.Evaluate(UnityEngine.Random.value);
					this.monsterSelection = this.GenerateDirectorCardWeightedSelection(monsterFamily.monsterFamilyCategories);
					base.StartCoroutine("BroadcastFamilySelection", monsterFamily);
				}
			}
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x0000AB13 File Offset: 0x00008D13
		private IEnumerator BroadcastFamilySelection(ClassicStageInfo.MonsterFamily selectedFamily)
		{
			yield return new WaitForSeconds(1f);
			Chat.SendBroadcastChat(new Chat.SimpleChatMessage
			{
				baseToken = selectedFamily.familySelectionChatString
			});
			yield break;
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x0000AB22 File Offset: 0x00008D22
		private void OnEnable()
		{
			ClassicStageInfo.instance = this;
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x0000AB2A File Offset: 0x00008D2A
		private void OnDisable()
		{
			ClassicStageInfo.instance = null;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00055708 File Offset: 0x00053908
		private static float CalculateTotalWeight(DirectorCard[] cards)
		{
			float num = 0f;
			foreach (DirectorCard directorCard in cards)
			{
				num += (float)directorCard.selectionWeight;
			}
			return num;
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x0005573C File Offset: 0x0005393C
		private static bool CardIsMiniBoss(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "GolemMaster" || name == "BisonMaster" || name == "GreaterWispMaster" || name == "BeetleGuardMaster";
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x00055790 File Offset: 0x00053990
		private static bool CardIsChest(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "EquipmentBarrel" || name.Contains("Chest") || card.spawnCard.prefab.name.Contains("TripleShop");
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x000557E8 File Offset: 0x000539E8
		private static bool CardIsBarrel(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name != "EquipmentBarrel" && name.Contains("Barrel");
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x0000AB32 File Offset: 0x00008D32
		private static bool CardIsChampion(DirectorCard card)
		{
			return card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion;
		}

		// Token: 0x040011A1 RID: 4513
		[Tooltip("Stages that can be destinations of the teleporter.")]
		public SceneField[] destinations;

		// Token: 0x040011A2 RID: 4514
		[SerializeField]
		private DirectorCardCategorySelection interactableCategories;

		// Token: 0x040011A3 RID: 4515
		[SerializeField]
		private DirectorCardCategorySelection monsterCategories;

		// Token: 0x040011A4 RID: 4516
		public ClassicStageInfo.MonsterFamily[] possibleMonsterFamilies;

		// Token: 0x040011A5 RID: 4517
		public WeightedSelection<DirectorCard> interactableSelection;

		// Token: 0x040011A6 RID: 4518
		public WeightedSelection<DirectorCard> monsterSelection;

		// Token: 0x040011A7 RID: 4519
		[SerializeField]
		[HideInInspector]
		private DirectorCard[] monsterCards;

		// Token: 0x040011A8 RID: 4520
		[SerializeField]
		[HideInInspector]
		public DirectorCard[] interactableCards;

		// Token: 0x040011A9 RID: 4521
		public int sceneDirectorInteractibleCredits = 200;

		// Token: 0x040011AA RID: 4522
		public int sceneDirectorMonsterCredits = 20;

		// Token: 0x040011AC RID: 4524
		private const float monsterFamilyChance = 2f;

		// Token: 0x020002A0 RID: 672
		[Serializable]
		public struct MonsterFamily
		{
			// Token: 0x040011AD RID: 4525
			[SerializeField]
			public DirectorCardCategorySelection monsterFamilyCategories;

			// Token: 0x040011AE RID: 4526
			public string familySelectionChatString;

			// Token: 0x040011AF RID: 4527
			public float selectionWeight;

			// Token: 0x040011B0 RID: 4528
			public int minimumStageCompletion;

			// Token: 0x040011B1 RID: 4529
			public int maximumStageCompletion;
		}
	}
}
