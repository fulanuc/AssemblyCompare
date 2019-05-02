using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000641 RID: 1601
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SetLabelTextToMainUserProfileName : MonoBehaviour
	{
		// Token: 0x06002421 RID: 9249 RVA: 0x0001A5F1 File Offset: 0x000187F1
		private void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x000AB704 File Offset: 0x000A9904
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

		// Token: 0x040026CE RID: 9934
		private TextMeshProUGUI label;
	}
}
