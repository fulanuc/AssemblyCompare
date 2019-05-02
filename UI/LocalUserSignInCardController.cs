using System;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000608 RID: 1544
	public class LocalUserSignInCardController : MonoBehaviour
	{
		// Token: 0x060022CA RID: 8906 RVA: 0x000A6D60 File Offset: 0x000A4F60
		private void Update()
		{
			if (this.requestedUserProfile != null != this.userProfileSelectionList)
			{
				if (!this.userProfileSelectionList)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.userProfileSelectionListPrefab, base.transform);
					this.userProfileSelectionList = gameObject.GetComponent<UserProfileListController>();
					this.userProfileSelectionList.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.FindEventSystem(this.rewiredPlayer);
					this.userProfileSelectionList.onProfileSelected += this.OnUserSelectedUserProfile;
				}
				else
				{
					UnityEngine.Object.Destroy(this.userProfileSelectionList.gameObject);
					this.userProfileSelectionList = null;
				}
			}
			if (this.rewiredPlayer == null)
			{
				this.nameLabel.gameObject.SetActive(false);
				this.promptLabel.text = "Press 'Start'";
				this.cardImage.color = this.unselectedColor;
				this.cardImage.sprite = this.playerCardNone;
				return;
			}
			this.cardImage.color = this.selectedColor;
			this.nameLabel.gameObject.SetActive(true);
			if (this.requestedUserProfile == null)
			{
				this.cardImage.sprite = this.playerCardNone;
				this.nameLabel.text = "";
				this.promptLabel.text = "...";
				return;
			}
			this.cardImage.sprite = this.playerCardKBM;
			this.nameLabel.text = this.requestedUserProfile.name;
			this.promptLabel.text = "";
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060022CB RID: 8907 RVA: 0x000195DC File Offset: 0x000177DC
		// (set) Token: 0x060022CC RID: 8908 RVA: 0x000195E4 File Offset: 0x000177E4
		public Player rewiredPlayer
		{
			get
			{
				return this._rewiredPlayer;
			}
			set
			{
				if (this._rewiredPlayer == value)
				{
					return;
				}
				this._rewiredPlayer = value;
				if (this._rewiredPlayer == null)
				{
					this.requestedUserProfile = null;
				}
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060022CD RID: 8909 RVA: 0x00019606 File Offset: 0x00017806
		// (set) Token: 0x060022CE RID: 8910 RVA: 0x0001960E File Offset: 0x0001780E
		public UserProfile requestedUserProfile
		{
			get
			{
				return this._requestedUserProfile;
			}
			private set
			{
				if (this._requestedUserProfile == value)
				{
					return;
				}
				if (this._requestedUserProfile != null)
				{
					this._requestedUserProfile.isClaimed = false;
				}
				this._requestedUserProfile = value;
				if (this._requestedUserProfile != null)
				{
					this._requestedUserProfile.isClaimed = true;
				}
			}
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x00019649 File Offset: 0x00017849
		private void OnUserSelectedUserProfile(UserProfile userProfile)
		{
			this.requestedUserProfile = userProfile;
		}

		// Token: 0x0400259D RID: 9629
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400259E RID: 9630
		public TextMeshProUGUI promptLabel;

		// Token: 0x0400259F RID: 9631
		public Image cardImage;

		// Token: 0x040025A0 RID: 9632
		public Sprite playerCardNone;

		// Token: 0x040025A1 RID: 9633
		public Sprite playerCardKBM;

		// Token: 0x040025A2 RID: 9634
		public Sprite playerCardController;

		// Token: 0x040025A3 RID: 9635
		public Color unselectedColor;

		// Token: 0x040025A4 RID: 9636
		public Color selectedColor;

		// Token: 0x040025A5 RID: 9637
		private UserProfileListController userProfileSelectionList;

		// Token: 0x040025A6 RID: 9638
		public GameObject userProfileSelectionListPrefab;

		// Token: 0x040025A7 RID: 9639
		private Player _rewiredPlayer;

		// Token: 0x040025A8 RID: 9640
		private UserProfile _requestedUserProfile;
	}
}
