using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061F RID: 1567
	[RequireComponent(typeof(TextMeshProUGUI))]
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileNameLabel : MonoBehaviour
	{
		// Token: 0x0600233C RID: 9020 RVA: 0x00019B0A File Offset: 0x00017D0A
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x000A8EE8 File Offset: 0x000A70E8
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

		// Token: 0x04002627 RID: 9767
		public string token;

		// Token: 0x04002628 RID: 9768
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002629 RID: 9769
		private TextMeshProUGUI label;

		// Token: 0x0400262A RID: 9770
		private string currentUserName;
	}
}
