using System;
using UnityEngine;

namespace RoR2.CharacterAI
{
	// Token: 0x020005A4 RID: 1444
	[RequireComponent(typeof(BaseAI))]
	public class AIOwnership : MonoBehaviour
	{
		// Token: 0x060020B1 RID: 8369 RVA: 0x00017DC3 File Offset: 0x00015FC3
		private void Awake()
		{
			this.baseAI = base.GetComponent<BaseAI>();
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00017DD1 File Offset: 0x00015FD1
		private void FixedUpdate()
		{
			if (this.ownerMaster)
			{
				this.baseAI.leader.gameObject = this.ownerMaster.GetBodyObject();
			}
		}

		// Token: 0x040022A3 RID: 8867
		public CharacterMaster ownerMaster;

		// Token: 0x040022A4 RID: 8868
		private BaseAI baseAI;
	}
}
