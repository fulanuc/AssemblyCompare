using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000653 RID: 1619
	[RequireComponent(typeof(TMP_InputField))]
	public class SubmitInputFieldOnEnterKey : MonoBehaviour
	{
		// Token: 0x06002470 RID: 9328 RVA: 0x0001A8A8 File Offset: 0x00018AA8
		private void Awake()
		{
			this.inputField = base.GetComponent<TMP_InputField>();
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x0001A8B6 File Offset: 0x00018AB6
		private void Update()
		{
			if (this.inputField.isFocused && this.inputField.text != "")
			{
				Input.GetKeyDown(KeyCode.Return);
			}
		}

		// Token: 0x04002713 RID: 10003
		private TMP_InputField inputField;
	}
}
