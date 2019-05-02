using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using RoR2.Networking;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000651 RID: 1617
	public class SteamworksLobbyUserList : MonoBehaviour
	{
		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06002461 RID: 9313 RVA: 0x0001A812 File Offset: 0x00018A12
		private bool ValidLobbyExists
		{
			get
			{
				return Client.Instance.Lobby.LobbyType != Lobby.Type.Error;
			}
		}

		// Token: 0x06002462 RID: 9314 RVA: 0x000AC2D0 File Offset: 0x000AA4D0
		private void Update()
		{
			Client instance = Client.Instance;
			if (instance == null)
			{
				return;
			}
			if (!instance.Lobby.IsValid && this.elements.Count > 0)
			{
				this.Rebuild();
			}
			this.UpdateLobbyString();
		}

		// Token: 0x06002463 RID: 9315 RVA: 0x000AC310 File Offset: 0x000AA510
		private void Rebuild()
		{
			Client instance = Client.Instance;
			if (instance == null)
			{
				return;
			}
			bool validLobbyExists = this.ValidLobbyExists;
			ulong currentLobby = instance.Lobby.CurrentLobby;
			ulong[] memberIDs = instance.Lobby.GetMemberIDs();
			int num = validLobbyExists ? RoR2Application.maxPlayers : 0;
			this.copyLobbyIDToClipboardButton.SetActive(validLobbyExists);
			while (this.elements.Count > num)
			{
				int index = this.elements.Count - 1;
				UnityEngine.Object.Destroy(this.elements[index].gameObject);
				this.elements.RemoveAt(index);
			}
			while (this.elements.Count < num)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/SteamLobbyUserListElement"), base.transform);
				SocialUserIcon componentInChildren = gameObject.GetComponentInChildren<SocialUserIcon>();
				SteamUsernameLabel componentInChildren2 = gameObject.GetComponentInChildren<SteamUsernameLabel>();
				ChildLocator component = gameObject.GetComponent<ChildLocator>();
				this.elements.Add(new SteamworksLobbyUserList.Element
				{
					gameObject = gameObject,
					userIcon = componentInChildren,
					usernameLabel = componentInChildren2,
					elementChildLocator = component
				});
			}
			int i = 0;
			for (int j = 0; j < memberIDs.Length; j++)
			{
				int lobbyMemberPlayerCountByIndex = SteamworksLobbyManager.GetLobbyMemberPlayerCountByIndex(j);
				for (int k = 0; k < lobbyMemberPlayerCountByIndex; k++)
				{
					this.elements[i++].SetUser(memberIDs[j], k);
				}
			}
			while (i < num)
			{
				this.elements[i].SetUser(0UL, 0);
				i++;
			}
		}

		// Token: 0x06002464 RID: 9316 RVA: 0x000AC484 File Offset: 0x000AA684
		private void UpdateLobbyString()
		{
			if (this.lobbyStateText)
			{
				string text = "";
				switch (Client.Instance.Lobby.LobbyType)
				{
				case Lobby.Type.Private:
					text = Language.GetString("STEAM_LOBBY_PRIVATE");
					break;
				case Lobby.Type.FriendsOnly:
					text = Language.GetString("STEAM_LOBBY_FRIENDSONLY");
					break;
				case Lobby.Type.Public:
					text = Language.GetString("STEAM_LOBBY_PUBLIC");
					break;
				case Lobby.Type.Invisible:
					text = Language.GetString("STEAM_LOBBY_INVISIBLE");
					break;
				case Lobby.Type.Error:
					text = "";
					break;
				}
				this.lobbyStateText.text = text;
			}
		}

		// Token: 0x06002465 RID: 9317 RVA: 0x000AC518 File Offset: 0x000AA718
		private void UpdateUser(ulong userId)
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				if (this.elements[i].steamId == userId)
				{
					this.elements[i].Refresh();
				}
			}
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x000AC560 File Offset: 0x000AA760
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged += this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated += this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated += this.Rebuild;
			this.Rebuild();
		}

		// Token: 0x06002467 RID: 9319 RVA: 0x000AC5B8 File Offset: 0x000AA7B8
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated -= this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged -= this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated -= this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated -= this.Rebuild;
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x0001A829 File Offset: 0x00018A29
		private void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			this.Rebuild();
		}

		// Token: 0x06002469 RID: 9321 RVA: 0x0001A831 File Offset: 0x00018A31
		private void OnLobbyMemberDataUpdated(ulong steamId)
		{
			this.UpdateUser(steamId);
		}

		// Token: 0x0400270A RID: 9994
		public TextMeshProUGUI lobbyStateText;

		// Token: 0x0400270B RID: 9995
		public GameObject copyLobbyIDToClipboardButton;

		// Token: 0x0400270C RID: 9996
		private List<SteamworksLobbyUserList.Element> elements = new List<SteamworksLobbyUserList.Element>();

		// Token: 0x02000652 RID: 1618
		private class Element
		{
			// Token: 0x0600246B RID: 9323 RVA: 0x0001A84D File Offset: 0x00018A4D
			public void SetUser(ulong steamId, int subPlayerIndex)
			{
				this.steamId = steamId;
				this.userIcon.SetFromSteamId(steamId);
				this.usernameLabel.userSteamId = steamId;
				this.usernameLabel.subPlayerIndex = subPlayerIndex;
				this.Refresh();
			}

			// Token: 0x0600246C RID: 9324 RVA: 0x000AC60C File Offset: 0x000AA80C
			public void Refresh()
			{
				if (this.steamId == 0UL)
				{
					this.elementChildLocator.FindChild("UserIcon").gameObject.SetActive(false);
					this.elementChildLocator.FindChild("InviteButton").gameObject.SetActive(true);
				}
				else
				{
					this.elementChildLocator.FindChild("UserIcon").gameObject.SetActive(true);
					this.elementChildLocator.FindChild("InviteButton").gameObject.SetActive(false);
				}
				this.userIcon.Refresh();
				this.usernameLabel.Refresh();
				this.RefreshCrownAndPromoteButton();
			}

			// Token: 0x0600246D RID: 9325 RVA: 0x000AC6AC File Offset: 0x000AA8AC
			private void RefreshCrownAndPromoteButton()
			{
				if (Client.Instance == null)
				{
					return;
				}
				bool flag = Client.Instance.Lobby.Owner == this.steamId && this.steamId > 0UL;
				if (this.lobbyLeaderCrown != flag)
				{
					if (flag)
					{
						this.lobbyLeaderCrown = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/LobbyLeaderCrown"), this.gameObject.transform);
					}
					else
					{
						UnityEngine.Object.Destroy(this.lobbyLeaderCrown);
						this.lobbyLeaderCrown = null;
					}
				}
				if (this.elementChildLocator)
				{
					bool flag2 = !flag && SteamworksLobbyManager.ownsLobby && this.steamId != 0UL && !SteamLobbyFinder.running && !NetworkSession.instance;
					GameObject gameObject = this.elementChildLocator.FindChild("PromoteButton").gameObject;
					if (gameObject)
					{
						gameObject.SetActive(flag2);
						if (flag2)
						{
							MPButton component = gameObject.GetComponent<MPButton>();
							if (component)
							{
								component.onClick.RemoveAllListeners();
								component.onClick.AddListener(delegate()
								{
									Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_assign_owner {0}", TextSerialization.ToStringInvariant(this.steamId)), false);
								});
							}
						}
					}
				}
			}

			// Token: 0x0400270D RID: 9997
			public ulong steamId;

			// Token: 0x0400270E RID: 9998
			public GameObject gameObject;

			// Token: 0x0400270F RID: 9999
			public SocialUserIcon userIcon;

			// Token: 0x04002710 RID: 10000
			public SteamUsernameLabel usernameLabel;

			// Token: 0x04002711 RID: 10001
			public GameObject lobbyLeaderCrown;

			// Token: 0x04002712 RID: 10002
			public ChildLocator elementChildLocator;
		}
	}
}
