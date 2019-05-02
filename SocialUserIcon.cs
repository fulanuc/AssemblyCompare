using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000411 RID: 1041
	[RequireComponent(typeof(RawImage))]
	public class SocialUserIcon : UIBehaviour
	{
		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x000118D9 File Offset: 0x0000FAD9
		private Texture defaultTexture
		{
			get
			{
				return Resources.Load<Texture>("Textures/UI/texDefaultSocialUserIcon");
			}
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x00079F08 File Offset: 0x00078108
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

		// Token: 0x06001764 RID: 5988 RVA: 0x000118E5 File Offset: 0x0000FAE5
		protected override void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.generatedTexture);
			this.generatedTexture = null;
			base.OnDestroy();
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x000118FF File Offset: 0x0000FAFF
		protected override void Awake()
		{
			base.Awake();
			this.rawImageComponent = base.GetComponent<RawImage>();
			this.rawImageComponent.texture = this.defaultTexture;
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00011924 File Offset: 0x0000FB24
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

		// Token: 0x06001767 RID: 5991 RVA: 0x00079F78 File Offset: 0x00078178
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

		// Token: 0x06001768 RID: 5992 RVA: 0x00011953 File Offset: 0x0000FB53
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

		// Token: 0x06001769 RID: 5993 RVA: 0x00079FCC File Offset: 0x000781CC
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

		// Token: 0x0600176A RID: 5994 RVA: 0x0007A028 File Offset: 0x00078228
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

		// Token: 0x04001A6C RID: 6764
		private RawImage rawImageComponent;

		// Token: 0x04001A6D RID: 6765
		protected Texture2D generatedTexture;

		// Token: 0x04001A6E RID: 6766
		private SocialUserIcon.SourceType sourceType;

		// Token: 0x04001A6F RID: 6767
		private ulong userSteamId;

		// Token: 0x04001A70 RID: 6768
		public Friends.AvatarSize avatarSize;

		// Token: 0x02000412 RID: 1042
		private enum SourceType
		{
			// Token: 0x04001A72 RID: 6770
			None,
			// Token: 0x04001A73 RID: 6771
			Steam
		}
	}
}
