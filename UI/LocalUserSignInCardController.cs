using System;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F6 RID: 1526
	public class LocalUserSignInCardController : MonoBehaviour
	{
		// Token: 0x0600223A RID: 8762 RVA: 0x000A56E4 File Offset: 0x000A38E4
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

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x0600223B RID: 8763 RVA: 0x00018F2F File Offset: 0x0001712F
		// (set) Token: 0x0600223C RID: 8764 RVA: 0x00018F37 File Offset: 0x00017137
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

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600223D RID: 8765 RVA: 0x00018F59 File Offset: 0x00017159
		// (set) Token: 0x0600223E RID: 8766 RVA: 0x00018F61 File Offset: 0x00017161
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

		// Token: 0x0600223F RID: 8767 RVA: 0x00018F9C File Offset: 0x0001719C
		private void OnUserSelectedUserProfile(UserProfile userProfile)
		{
			this.requestedUserProfile = userProfile;
		}

		// Token: 0x04002542 RID: 9538
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002543 RID: 9539
		public TextMeshProUGUI promptLabel;

		// Token: 0x04002544 RID: 9540
		public Image cardImage;

		// Token: 0x04002545 RID: 9541
		public Sprite playerCardNone;

		// Token: 0x04002546 RID: 9542
		public Sprite playerCardKBM;

		// Token: 0x04002547 RID: 9543
		public Sprite playerCardController;

		// Token: 0x04002548 RID: 9544
		public Color unselectedColor;

		// Token: 0x04002549 RID: 9545
		public Color selectedColor;

		// Token: 0x0400254A RID: 9546
		private UserProfileListController userProfileSelectionList;

		// Token: 0x0400254B RID: 9547
		public GameObject userProfileSelectionListPrefab;

		// Token: 0x0400254C RID: 9548
		private Player _rewiredPlayer;

		// Token: 0x0400254D RID: 9549
		private UserProfile _requestedUserProfile;
	}
}
