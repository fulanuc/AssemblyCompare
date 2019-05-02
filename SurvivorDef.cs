using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004B8 RID: 1208
	public class SurvivorDef
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06001B59 RID: 7001 RVA: 0x000883CC File Offset: 0x000865CC
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

		// Token: 0x04001DDD RID: 7645
		public GameObject bodyPrefab;

		// Token: 0x04001DDE RID: 7646
		public GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/NullModel");

		// Token: 0x04001DDF RID: 7647
		public SurvivorIndex survivorIndex;

		// Token: 0x04001DE0 RID: 7648
		public string unlockableName = "";

		// Token: 0x04001DE1 RID: 7649
		public string descriptionToken;

		// Token: 0x04001DE2 RID: 7650
		public Color primaryColor;
	}
}
