using System;
using UnityEngine;

namespace RoR2.CharacterAI
{
	// Token: 0x02000591 RID: 1425
	[RequireComponent(typeof(BaseAI))]
	public class AIOwnership : MonoBehaviour
	{
		// Token: 0x06002020 RID: 8224 RVA: 0x00017694 File Offset: 0x00015894
		private void Awake()
		{
			this.baseAI = base.GetComponent<BaseAI>();
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000176A2 File Offset: 0x000158A2
		private void FixedUpdate()
		{
			if (this.ownerMaster)
			{
				this.baseAI.leader.gameObject = this.ownerMaster.GetBodyObject();
			}
		}

		// Token: 0x0400224B RID: 8779
		public CharacterMaster ownerMaster;

		// Token: 0x0400224C RID: 8780
		private BaseAI baseAI;
	}
}
