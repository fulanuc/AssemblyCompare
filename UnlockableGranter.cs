using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040F RID: 1039
	public class UnlockableGranter : MonoBehaviour
	{
		// Token: 0x06001741 RID: 5953 RVA: 0x00079CB4 File Offset: 0x00077EB4
		public void GrantUnlockable(Interactor interactor)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Run.instance.GrantUnlockToSinglePlayer(this.unlockableString, component);
			}
		}

		// Token: 0x04001A52 RID: 6738
		public string unlockableString;
	}
}
