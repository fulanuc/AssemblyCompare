using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000641 RID: 1601
	[RequireComponent(typeof(TMP_InputField))]
	public class SubmitInputFieldOnEnterKey : MonoBehaviour
	{
		// Token: 0x060023E0 RID: 9184 RVA: 0x0001A1DA File Offset: 0x000183DA
		private void Awake()
		{
			this.inputField = base.GetComponent<TMP_InputField>();
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x0001A1E8 File Offset: 0x000183E8
		private void Update()
		{
			if (this.inputField.isFocused && this.inputField.text != "")
			{
				Input.GetKeyDown(KeyCode.Return);
			}
		}

		// Token: 0x040026B8 RID: 9912
		private TMP_InputField inputField;
	}
}
