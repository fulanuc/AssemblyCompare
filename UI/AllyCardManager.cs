using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005BA RID: 1466
	public class AllyCardManager : MonoBehaviour
	{
		// Token: 0x06002113 RID: 8467 RVA: 0x000181F9 File Offset: 0x000163F9
		private void Awake()
		{
			this.cardAllocator = new UIElementAllocator<AllyCardController>((RectTransform)base.transform, Resources.Load<GameObject>("Prefabs/UI/AllyCard"));
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x0009F6E0 File Offset: 0x0009D8E0
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

		// Token: 0x04002355 RID: 9045
		public GameObject sourceGameObject;

		// Token: 0x04002356 RID: 9046
		private UIElementAllocator<AllyCardController> cardAllocator;
	}
}
