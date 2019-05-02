using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000353 RID: 851
	public class MageLastElementTracker : MonoBehaviour
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060011AD RID: 4525 RVA: 0x0000D77E File Offset: 0x0000B97E
		// (set) Token: 0x060011AE RID: 4526 RVA: 0x0000D786 File Offset: 0x0000B986
		[HideInInspector]
		public MageElement mageElement { get; private set; }

		// Token: 0x060011AF RID: 4527 RVA: 0x0000D78F File Offset: 0x0000B98F
		private void Start()
		{
			this.ApplyElement(MageElement.Fire);
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00066BAC File Offset: 0x00064DAC
		public void ApplyElement(MageElement element)
		{
			this.mageElement = element;
			int num = (int)(element - MageElement.Fire);
			this.targetSkill.skillNameToken = this.elementSkillInfos[num].nameToken;
			this.targetSkill.skillDescriptionToken = this.elementSkillInfos[num].descriptionToken;
			this.targetSkill.icon = this.elementSkillInfos[num].sprite;
		}

		// Token: 0x040015A5 RID: 5541
		public GenericSkill targetSkill;

		// Token: 0x040015A6 RID: 5542
		public MageLastElementTracker.ElementSkillInfo[] elementSkillInfos;

		// Token: 0x02000354 RID: 852
		[Serializable]
		public struct ElementSkillInfo
		{
			// Token: 0x040015A7 RID: 5543
			public string nameToken;

			// Token: 0x040015A8 RID: 5544
			public string descriptionToken;

			// Token: 0x040015A9 RID: 5545
			public Sprite sprite;
		}
	}
}
