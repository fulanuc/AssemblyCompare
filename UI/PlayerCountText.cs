using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061B RID: 1563
	public class PlayerCountText : MonoBehaviour
	{
		// Token: 0x06002331 RID: 9009 RVA: 0x000A8E00 File Offset: 0x000A7000
		private void Update()
		{
			if (this.targetText)
			{
				this.targetText.text = string.Format("{0}/{1}", NetworkUser.readOnlyInstancesList.Count, NetworkManager.singleton.maxConnections);
			}
		}

		// Token: 0x0400261D RID: 9757
		public Text targetText;
	}
}
