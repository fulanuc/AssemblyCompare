using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000639 RID: 1593
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SocialUsernameLabel : MonoBehaviour
	{
		// Token: 0x060023BB RID: 9147 RVA: 0x0001A099 File Offset: 0x00018299
		private void Awake()
		{
			this.textMeshComponent = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x0001A0A7 File Offset: 0x000182A7
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUsernameLabel.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060023BD RID: 9149 RVA: 0x0001A0B8 File Offset: 0x000182B8
		// (set) Token: 0x060023BE RID: 9150 RVA: 0x0001A0C0 File Offset: 0x000182C0
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

		// Token: 0x060023BF RID: 9151 RVA: 0x000AA908 File Offset: 0x000A8B08
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

		// Token: 0x0400269D RID: 9885
		protected TextMeshProUGUI textMeshComponent;

		// Token: 0x0400269E RID: 9886
		private SocialUsernameLabel.SourceType sourceType;

		// Token: 0x0400269F RID: 9887
		private ulong _userSteamId;

		// Token: 0x040026A0 RID: 9888
		public int subPlayerIndex;

		// Token: 0x0200063A RID: 1594
		private enum SourceType
		{
			// Token: 0x040026A2 RID: 9890
			None,
			// Token: 0x040026A3 RID: 9891
			Steam
		}
	}
}
