using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000631 RID: 1585
	[RequireComponent(typeof(TextMeshProUGUI))]
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileNameLabel : MonoBehaviour
	{
		// Token: 0x060023CC RID: 9164 RVA: 0x0001A1D8 File Offset: 0x000183D8
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060023CD RID: 9165 RVA: 0x000AA564 File Offset: 0x000A8764
		private void Update()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			string text;
			if (eventSystem == null)
			{
				text = null;
			}
			else
			{
				LocalUser localUser = eventSystem.localUser;
				text = ((localUser != null) ? localUser.userProfile.name : null);
			}
			string a = text ?? string.Empty;
			if (a != this.currentUserName)
			{
				this.currentUserName = a;
				this.label.text = Language.GetStringFormatted(this.token, new object[]
				{
					this.currentUserName
				});
			}
		}

		// Token: 0x04002682 RID: 9858
		public string token;

		// Token: 0x04002683 RID: 9859
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002684 RID: 9860
		private TextMeshProUGUI label;

		// Token: 0x04002685 RID: 9861
		private string currentUserName;
	}
}
