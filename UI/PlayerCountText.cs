using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062D RID: 1581
	public class PlayerCountText : MonoBehaviour
	{
		// Token: 0x060023C1 RID: 9153 RVA: 0x000AA47C File Offset: 0x000A867C
		private void Update()
		{
			if (this.targetText)
			{
				this.targetText.text = string.Format("{0}/{1}", NetworkUser.readOnlyInstancesList.Count, NetworkManager.singleton.maxConnections);
			}
		}

		// Token: 0x04002678 RID: 9848
		public Text targetText;
	}
}
