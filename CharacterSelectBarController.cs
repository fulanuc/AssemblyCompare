using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040B RID: 1035
	public class CharacterSelectBarController : MonoBehaviour
	{
		// Token: 0x0600173D RID: 5949 RVA: 0x00011782 File Offset: 0x0000F982
		private bool ShouldDisplaySurvivor(SurvivorDef survivorDef)
		{
			return survivorDef != null;
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x000795D8 File Offset: 0x000777D8
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

		// Token: 0x0600173F RID: 5951 RVA: 0x00011788 File Offset: 0x0000F988
		private void Awake()
		{
			this.survivorIconControllers = new UIElementAllocator<SurvivorIconController>(this.iconContainer, this.choiceButtonPrefab);
			this.Build();
		}

		// Token: 0x04001A42 RID: 6722
		public GameObject choiceButtonPrefab;

		// Token: 0x04001A43 RID: 6723
		public GameObject WIPButtonPrefab;

		// Token: 0x04001A44 RID: 6724
		public RectTransform iconContainer;

		// Token: 0x04001A45 RID: 6725
		private UIElementAllocator<SurvivorIconController> survivorIconControllers;
	}
}
