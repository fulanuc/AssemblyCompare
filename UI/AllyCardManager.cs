using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A8 RID: 1448
	public class AllyCardManager : MonoBehaviour
	{
		// Token: 0x06002082 RID: 8322 RVA: 0x00017AFF File Offset: 0x00015CFF
		private void Awake()
		{
			this.cardAllocator = new UIElementAllocator<AllyCardController>((RectTransform)base.transform, Resources.Load<GameObject>("Prefabs/UI/AllyCard"));
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x0009E10C File Offset: 0x0009C30C
		private void Update()
		{
			TeamIndex teamIndex = TeamIndex.None;
			TeamComponent teamComponent = null;
			if (this.sourceGameObject)
			{
				teamComponent = this.sourceGameObject.GetComponent<TeamComponent>();
				if (teamComponent)
				{
					teamIndex = teamComponent.teamIndex;
				}
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			int num = teamMembers.Count;
			if (teamComponent && teamMembers.Contains(teamComponent))
			{
				num--;
			}
			this.cardAllocator.AllocateElements(num);
			int i = 0;
			int num2 = 0;
			while (i < teamMembers.Count)
			{
				GameObject gameObject = teamMembers[i].gameObject;
				if (gameObject != this.sourceGameObject)
				{
					this.cardAllocator.elements[num2++].sourceGameObject = gameObject;
				}
				i++;
			}
		}

		// Token: 0x04002301 RID: 8961
		public GameObject sourceGameObject;

		// Token: 0x04002302 RID: 8962
		private UIElementAllocator<AllyCardController> cardAllocator;
	}
}
