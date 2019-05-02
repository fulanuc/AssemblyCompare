using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D8 RID: 1496
	[RequireComponent(typeof(Image))]
	internal class CurrentDifficultyIconController : MonoBehaviour
	{
		// Token: 0x060021CE RID: 8654 RVA: 0x000A1FD8 File Offset: 0x000A01D8
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
