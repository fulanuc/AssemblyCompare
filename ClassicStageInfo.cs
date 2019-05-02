using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029D RID: 669
	[RequireComponent(typeof(SceneInfo))]
	public class ClassicStageInfo : MonoBehaviour
	{
		// Token: 0x06000DAE RID: 3502 RVA: 0x00055640 File Offset: 0x00053840
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

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000DAF RID: 3503 RVA: 0x0000AAB2 File Offset: 0x00008CB2
		// (set) Token: 0x06000DB0 RID: 3504 RVA: 0x0000AAB9 File Offset: 0x00008CB9
		public static ClassicStageInfo instance { get; private set; }

		// Token: 0x06000DB1 RID: 3505 RVA: 0x000556B8 File Offset: 0x000538B8
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

		// Token: 0x06000DB2 RID: 3506 RVA: 0x0000AAC1 File Offset: 0x00008CC1
		private IEnumerator BroadcastFamilySelection(ClassicStageInfo.MonsterFamily selectedFamily)
		{
			yield return new WaitForSeconds(1f);
			Chat.SendBroadcastChat(new Chat.SimpleChatMessage
			{
				baseToken = selectedFamily.familySelectionChatString
			});
			yield break;
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x0000AAD0 File Offset: 0x00008CD0
		private void OnEnable()
		{
			ClassicStageInfo.instance = this;
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x0000AAD8 File Offset: 0x00008CD8
		private void OnDisable()
		{
			ClassicStageInfo.instance = null;
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x000557C4 File Offset: 0x000539C4
		private static float CalculateTotalWeight(DirectorCard[] cards)
		{
			float num = 0f;
			foreach (DirectorCard directorCard in cards)
			{
				num += (float)directorCard.selectionWeight;
			}
			return num;
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x000557F8 File Offset: 0x000539F8
		private static bool CardIsMiniBoss(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "GolemMaster" || name == "BisonMaster" || name == "GreaterWispMaster" || name == "BeetleGuardMaster";
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x0005584C File Offset: 0x00053A4C
		private static bool CardIsChest(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "EquipmentBarrel" || name.Contains("Chest") || card.spawnCard.prefab.name.Contains("TripleShop");
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x000558A4 File Offset: 0x00053AA4
		private static bool CardIsBarrel(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name != "EquipmentBarrel" && name.Contains("Barrel");
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x0000AAE0 File Offset: 0x00008CE0
		private static bool CardIsChampion(DirectorCard card)
		{
			return card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion;
		}

		// Token: 0x0400118F RID: 4495
		[Tooltip("Stages that can be destinations of the teleporter.")]
		public SceneField[] destinations;

		// Token: 0x04001190 RID: 4496
		[SerializeField]
		private DirectorCardCategorySelection interactableCategories;

		// Token: 0x04001191 RID: 4497
		[SerializeField]
		private DirectorCardCategorySelection monsterCategories;

		// Token: 0x04001192 RID: 4498
		public ClassicStageInfo.MonsterFamily[] possibleMonsterFamilies;

		// Token: 0x04001193 RID: 4499
		public WeightedSelection<DirectorCard> interactableSelection;

		// Token: 0x04001194 RID: 4500
		public WeightedSelection<DirectorCard> monsterSelection;

		// Token: 0x04001195 RID: 4501
		[SerializeField]
		[HideInInspector]
		private DirectorCard[] monsterCards;

		// Token: 0x04001196 RID: 4502
		[SerializeField]
		[HideInInspector]
		public DirectorCard[] interactableCards;

		// Token: 0x04001197 RID: 4503
		public int sceneDirectorInteractibleCredits = 200;

		// Token: 0x04001198 RID: 4504
		public int sceneDirectorMonsterCredits = 20;

		// Token: 0x0400119A RID: 4506
		private const float monsterFamilyChance = 2f;

		// Token: 0x0200029E RID: 670
		[Serializable]
		public struct MonsterFamily
		{
			// Token: 0x0400119B RID: 4507
			[SerializeField]
			public DirectorCardCategorySelection monsterFamilyCategories;

			// Token: 0x0400119C RID: 4508
			public string familySelectionChatString;

			// Token: 0x0400119D RID: 4509
			public float selectionWeight;

			// Token: 0x0400119E RID: 4510
			public int minimumStageCompletion;

			// Token: 0x0400119F RID: 4511
			public int maximumStageCompletion;
		}
	}
}
