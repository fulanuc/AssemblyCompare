using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000350 RID: 848
	public class MageLastElementTracker : MonoBehaviour
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001196 RID: 4502 RVA: 0x0000D695 File Offset: 0x0000B895
		// (set) Token: 0x06001197 RID: 4503 RVA: 0x0000D69D File Offset: 0x0000B89D
		[HideInInspector]
		public MageElement mageElement { get; private set; }

		// Token: 0x06001198 RID: 4504 RVA: 0x0000D6A6 File Offset: 0x0000B8A6
		private void Start()
		{
			this.ApplyElement(MageElement.Fire);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00066874 File Offset: 0x00064A74
		public void ApplyElement(MageElement element)
		{
			this.mageElement = element;
			int num = (int)(element - MageElement.Fire);
			this.targetSkill.skillNameToken = this.elementSkillInfos[num].nameToken;
			this.targetSkill.skillDescriptionToken = this.elementSkillInfos[num].descriptionToken;
			this.targetSkill.icon = this.elementSkillInfos[num].sprite;
		}

		// Token: 0x0400158C RID: 5516
		public GenericSkill targetSkill;

		// Token: 0x0400158D RID: 5517
		public MageLastElementTracker.ElementSkillInfo[] elementSkillInfos;

		// Token: 0x02000351 RID: 849
		[Serializable]
		public struct ElementSkillInfo
		{
			// Token: 0x0400158E RID: 5518
			public string nameToken;

			// Token: 0x0400158F RID: 5519
			public string descriptionToken;

			// Token: 0x04001590 RID: 5520
			public Sprite sprite;
		}
	}
}
