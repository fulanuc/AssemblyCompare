using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C6 RID: 1478
	[RequireComponent(typeof(Image))]
	internal class CurrentDifficultyIconController : MonoBehaviour
	{
		// Token: 0x0600213D RID: 8509 RVA: 0x000A0A04 File Offset: 0x0009EC04
		private void Start()
		{
			if (Run.instance)
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty);
				base.GetComponent<Image>().sprite = difficultyDef.GetIconSprite();
			}
		}
	}
}
