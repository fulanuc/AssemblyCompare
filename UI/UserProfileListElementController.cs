using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000660 RID: 1632
	[RequireComponent(typeof(MPButton))]
	public class UserProfileListElementController : MonoBehaviour
	{
		// Token: 0x060024C0 RID: 9408 RVA: 0x0001AC0C File Offset: 0x00018E0C
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
			this.button.onClick.AddListener(new UnityAction(this.InformListControllerOfSelection));
		}

		// Token: 0x060024C1 RID: 9409 RVA: 0x0001AC36 File Offset: 0x00018E36
		private void InformListControllerOfSelection()
		{
			if (!this.userProfile.isCorrupted)
			{
				this.listController.SendProfileSelection(this.userProfile);
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x060024C2 RID: 9410 RVA: 0x0001AC56 File Offset: 0x00018E56
		// (set) Token: 0x060024C3 RID: 9411 RVA: 0x000ADA14 File Offset: 0x000ABC14
		public UserProfile userProfile
		{
			get
			{
				return this._userProfile;
			}
			set
			{
				this._userProfile = value;
				this.nameLabel.text = ((this._userProfile == null) ? "???" : this._userProfile.name);
				if (this.playTimeLabel)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(this._userProfile.totalLoginSeconds);
					this.playTimeLabel.text = string.Format("{0}:{1:D2}", (uint)timeSpan.TotalHours, (uint)timeSpan.Minutes);
				}
			}
		}

		// Token: 0x04002772 RID: 10098
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002773 RID: 10099
		private MPButton button;

		// Token: 0x04002774 RID: 10100
		public TextMeshProUGUI playTimeLabel;

		// Token: 0x04002775 RID: 10101
		[NonSerialized]
		public UserProfileListController listController;

		// Token: 0x04002776 RID: 10102
		private UserProfile _userProfile;
	}
}
