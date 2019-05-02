using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005BD RID: 1469
	public class AssignStageToken : MonoBehaviour
	{
		// Token: 0x06002120 RID: 8480 RVA: 0x00018278 File Offset: 0x00016478
		private void Start()
		{
			this.titleText.text = Language.GetString(SceneInfo.instance.sceneDef.nameToken);
			this.subtitleText.text = Language.GetString(SceneInfo.instance.sceneDef.subtitleToken);
		}

		// Token: 0x04002363 RID: 9059
		public TextMeshProUGUI titleText;

		// Token: 0x04002364 RID: 9060
		public TextMeshProUGUI subtitleText;
	}
}
