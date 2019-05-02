using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062F RID: 1583
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SetLabelTextToMainUserProfileName : MonoBehaviour
	{
		// Token: 0x06002391 RID: 9105 RVA: 0x00019F23 File Offset: 0x00018123
		private void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x000AA088 File Offset: 0x000A8288
		private void OnEnable()
		{
			LocalUser localUser = LocalUserManager.FindLocalUser(0);
			if (localUser != null)
			{
				string name = localUser.userProfile.name;
				this.label.text = string.Format(Language.GetString("TITLE_PROFILE"), name);
				return;
			}
			this.label.text = "NO USER";
		}

		// Token: 0x04002673 RID: 9843
		private TextMeshProUGUI label;
	}
}
