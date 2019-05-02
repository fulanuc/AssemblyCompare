using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x0200064E RID: 1614
	[RequireComponent(typeof(MPButton))]
	public class UserProfileListElementController : MonoBehaviour
	{
		// Token: 0x06002430 RID: 9264 RVA: 0x0001A541 File Offset: 0x00018741
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
			this.button.onClick.AddListener(new UnityAction(this.InformListControllerOfSelection));
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x0001A56B File Offset: 0x0001876B
		private void InformListControllerOfSelection()
		{
			this.listController.SendProfileSelection(this.userProfile);
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06002432 RID: 9266 RVA: 0x0001A57E File Offset: 0x0001877E
		// (set) Token: 0x06002433 RID: 9267 RVA: 0x000AC394 File Offset: 0x000AA594
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

		// Token: 0x04002717 RID: 10007
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002718 RID: 10008
		private MPButton button;

		// Token: 0x04002719 RID: 10009
		public TextMeshProUGUI playTimeLabel;

		// Token: 0x0400271A RID: 10010
		[NonSerialized]
		public UserProfileListController listController;

		// Token: 0x0400271B RID: 10011
		private UserProfile _userProfile;
	}
}
