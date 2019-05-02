using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000405 RID: 1029
	public class CharacterSelectBarController : MonoBehaviour
	{
		// Token: 0x060016FA RID: 5882 RVA: 0x00011356 File Offset: 0x0000F556
		private bool ShouldDisplaySurvivor(SurvivorDef survivorDef)
		{
			return survivorDef != null;
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x00079018 File Offset: 0x00077218
		private void Build()
		{
			List<SurvivorIndex> list = new List<SurvivorIndex>();
			for (int i = 0; i < SurvivorCatalog.idealSurvivorOrder.Length; i++)
			{
				SurvivorIndex survivorIndex = SurvivorCatalog.idealSurvivorOrder[i];
				SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
				if (this.ShouldDisplaySurvivor(survivorDef))
				{
					list.Add(survivorIndex);
				}
			}
			this.survivorIconControllers.AllocateElements(list.Count);
			ReadOnlyCollection<SurvivorIconController> elements = this.survivorIconControllers.elements;
			for (int j = 0; j < list.Count; j++)
			{
				elements[j].survivorIndex = list[j];
			}
			for (int k = list.Count; k < SurvivorCatalog.survivorMaxCount; k++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.WIPButtonPrefab, this.iconContainer).gameObject.SetActive(true);
			}
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0001135C File Offset: 0x0000F55C
		private void Awake()
		{
			this.survivorIconControllers = new UIElementAllocator<SurvivorIconController>(this.iconContainer, this.choiceButtonPrefab);
			this.Build();
		}

		// Token: 0x04001A19 RID: 6681
		public GameObject choiceButtonPrefab;

		// Token: 0x04001A1A RID: 6682
		public GameObject WIPButtonPrefab;

		// Token: 0x04001A1B RID: 6683
		public RectTransform iconContainer;

		// Token: 0x04001A1C RID: 6684
		private UIElementAllocator<SurvivorIconController> survivorIconControllers;
	}
}
