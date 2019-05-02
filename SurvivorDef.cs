using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004C5 RID: 1221
	public class SurvivorDef
	{
		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x00088F40 File Offset: 0x00087140
		public string displayNameToken
		{
			get
			{
				if (this.bodyPrefab)
				{
					IDisplayNameProvider component = this.bodyPrefab.GetComponent<IDisplayNameProvider>();
					if (component != null)
					{
						return component.GetDisplayName();
					}
				}
				return "";
			}
		}

		// Token: 0x04001E16 RID: 7702
		public GameObject bodyPrefab;

		// Token: 0x04001E17 RID: 7703
		public GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/NullModel");

		// Token: 0x04001E18 RID: 7704
		public SurvivorIndex survivorIndex;

		// Token: 0x04001E19 RID: 7705
		public string unlockableName = "";

		// Token: 0x04001E1A RID: 7706
		public string descriptionToken;

		// Token: 0x04001E1B RID: 7707
		public Color primaryColor;
	}
}
