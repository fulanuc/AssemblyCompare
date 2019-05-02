using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x0200040B RID: 1035
	[RequireComponent(typeof(RawImage))]
	public class SocialUserIcon : UIBehaviour
	{
		// Token: 0x1700021E RID: 542
		// (get) Token: 0x0600171F RID: 5919 RVA: 0x000114AD File Offset: 0x0000F6AD
		private Texture defaultTexture
		{
			get
			{
				return Resources.Load<Texture>("Textures/UI/texDefaultSocialUserIcon");
			}
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00079948 File Offset: 0x00077B48
		protected void BuildTexture(int width, int height)
		{
			if (this.generatedTexture && (this.generatedTexture.width != width || this.generatedTexture.height != height))
			{
				this.generatedTexture.Resize(width, height);
			}
			if (!this.generatedTexture)
			{
				this.generatedTexture = new Texture2D(width, height);
				this.rawImageComponent.texture = this.generatedTexture;
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x000114B9 File Offset: 0x0000F6B9
		protected override void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.generatedTexture);
			this.generatedTexture = null;
			base.OnDestroy();
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x000114D3 File Offset: 0x0000F6D3
		protected override void Awake()
		{
			base.Awake();
			this.rawImageComponent = base.GetComponent<RawImage>();
			this.rawImageComponent.texture = this.defaultTexture;
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x000114F8 File Offset: 0x0000F6F8
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUserIcon.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
			if (!this.generatedTexture)
			{
				this.rawImageComponent.texture = this.defaultTexture;
			}
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000799B8 File Offset: 0x00077BB8
		public virtual void SetFromMaster(CharacterMaster master)
		{
			if (master)
			{
				PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					NetworkUser networkUser = component.networkUser;
					this.SetFromSteamId(networkUser.id.value);
					return;
				}
			}
			this.userSteamId = 0UL;
			this.sourceType = SocialUserIcon.SourceType.None;
			this.Refresh();
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x00011527 File Offset: 0x0000F727
		public void SetFromSteamId(ulong steamId)
		{
			if (this.sourceType == SocialUserIcon.SourceType.Steam && steamId == this.userSteamId)
			{
				return;
			}
			this.sourceType = SocialUserIcon.SourceType.Steam;
			this.userSteamId = steamId;
			this.Refresh();
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x00079A0C File Offset: 0x00077C0C
		private void RefreshForSteam()
		{
			Client instance = Client.Instance;
			if (instance != null)
			{
				Facepunch.Steamworks.Image cachedAvatar = instance.Friends.GetCachedAvatar(this.avatarSize, this.userSteamId);
				if (cachedAvatar != null)
				{
					this.OnSteamAvatarReceived(cachedAvatar);
					return;
				}
				instance.Friends.GetAvatar(this.avatarSize, this.userSteamId, new Action<Facepunch.Steamworks.Image>(this.OnSteamAvatarReceived));
			}
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x00079A68 File Offset: 0x00077C68
		private void OnSteamAvatarReceived(Facepunch.Steamworks.Image image)
		{
			if (!this)
			{
				return;
			}
			if (image == null)
			{
				return;
			}
			int width = image.Width;
			int height = image.Height;
			this.BuildTexture(width, height);
			byte[] data = image.Data;
			Color32[] array = new Color32[data.Length / 4];
			for (int i = 0; i < height; i++)
			{
				int num = height - 1 - i;
				for (int j = 0; j < width; j++)
				{
					int num2 = (i * width + j) * 4;
					array[num * width + j] = new Color32(data[num2], data[num2 + 1], data[num2 + 2], data[num2 + 3]);
				}
			}
			if (this.generatedTexture)
			{
				this.generatedTexture.SetPixels32(array);
				this.generatedTexture.Apply();
			}
		}

		// Token: 0x04001A43 RID: 6723
		private RawImage rawImageComponent;

		// Token: 0x04001A44 RID: 6724
		protected Texture2D generatedTexture;

		// Token: 0x04001A45 RID: 6725
		private SocialUserIcon.SourceType sourceType;

		// Token: 0x04001A46 RID: 6726
		private ulong userSteamId;

		// Token: 0x04001A47 RID: 6727
		public Friends.AvatarSize avatarSize;

		// Token: 0x0200040C RID: 1036
		private enum SourceType
		{
			// Token: 0x04001A49 RID: 6729
			None,
			// Token: 0x04001A4A RID: 6730
			Steam
		}
	}
}
