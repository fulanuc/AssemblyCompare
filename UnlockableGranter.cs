using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000415 RID: 1045
	public class UnlockableGranter : MonoBehaviour
	{
		// Token: 0x06001784 RID: 6020 RVA: 0x0007A274 File Offset: 0x00078474
		public void GrantUnlockable(Interactor interactor)
		{
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				Run.instance.GrantUnlockToSinglePlayer(this.unlockableString, component);
			}
		}

		// Token: 0x04001A7B RID: 6779
		public string unlockableString;
	}
}
