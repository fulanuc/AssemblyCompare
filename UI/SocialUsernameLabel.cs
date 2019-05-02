using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064B RID: 1611
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SocialUsernameLabel : MonoBehaviour
	{
		// Token: 0x0600244B RID: 9291 RVA: 0x0001A767 File Offset: 0x00018967
		private void Awake()
		{
			this.textMeshComponent = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x0001A775 File Offset: 0x00018975
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUsernameLabel.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x0600244D RID: 9293 RVA: 0x0001A786 File Offset: 0x00018986
		// (set) Token: 0x0600244E RID: 9294 RVA: 0x0001A78E File Offset: 0x0001898E
		public ulong userSteamId
		{
			get
			{
				return this._userSteamId;
			}
			set
			{
				this.sourceType = SocialUsernameLabel.SourceType.Steam;
				this._userSteamId = value;
			}
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x000ABF84 File Offset: 0x000AA184
		public void RefreshForSteam()
		{
			Client instance = Client.Instance;
			if (instance != null)
			{
				this.textMeshComponent.text = instance.Friends.GetName(this.userSteamId);
				if (this.subPlayerIndex != 0)
				{
					TextMeshProUGUI textMeshProUGUI = this.textMeshComponent;
					textMeshProUGUI.text = string.Concat(new object[]
					{
						textMeshProUGUI.text,
						"(",
						this.subPlayerIndex + 1,
						")"
					});
				}
			}
		}

		// Token: 0x040026F8 RID: 9976
		protected TextMeshProUGUI textMeshComponent;

		// Token: 0x040026F9 RID: 9977
		private SocialUsernameLabel.SourceType sourceType;

		// Token: 0x040026FA RID: 9978
		private ulong _userSteamId;

		// Token: 0x040026FB RID: 9979
		public int subPlayerIndex;

		// Token: 0x0200064C RID: 1612
		private enum SourceType
		{
			// Token: 0x040026FD RID: 9981
			None,
			// Token: 0x040026FE RID: 9982
			Steam
		}
	}
}
