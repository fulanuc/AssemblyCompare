using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005AB RID: 1451
	public class AssignStageToken : MonoBehaviour
	{
		// Token: 0x0600208F RID: 8335 RVA: 0x00017B7E File Offset: 0x00015D7E
		private void Start()
		{
			this.titleText.text = Language.GetString(SceneInfo.instance.sceneDef.nameToken);
			this.subtitleText.text = Language.GetString(SceneInfo.instance.sceneDef.subtitleToken);
		}

		// Token: 0x0400230F RID: 8975
		public TextMeshProUGUI titleText;

		// Token: 0x04002310 RID: 8976
		public TextMeshProUGUI subtitleText;
	}
}
