using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using RoR2.Networking;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063F RID: 1599
	public class SteamworksLobbyUserList : MonoBehaviour
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060023D1 RID: 9169 RVA: 0x0001A144 File Offset: 0x00018344
		private bool ValidLobbyExists
		{
			get
			{
				return Client.Instance.Lobby.LobbyType != Lobby.Type.Error;
			}
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000AAC54 File Offset: 0x000A8E54
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

		// Token: 0x060023D3 RID: 9171 RVA: 0x000AAC94 File Offset: 0x000A8E94
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
			int num = validLobbyExists ? 4 : 0;
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

		// Token: 0x060023D4 RID: 9172 RVA: 0x000AAE04 File Offset: 0x000A9004
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

		// Token: 0x060023D5 RID: 9173 RVA: 0x000AAE98 File Offset: 0x000A9098
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

		// Token: 0x060023D6 RID: 9174 RVA: 0x000AAEE0 File Offset: 0x000A90E0
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged += this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated += this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated += this.Rebuild;
			this.Rebuild();
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x000AAF38 File Offset: 0x000A9138
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated -= this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged -= this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated -= this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated -= this.Rebuild;
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x0001A15B File Offset: 0x0001835B
		private void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			this.Rebuild();
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x0001A163 File Offset: 0x00018363
		private void OnLobbyMemberDataUpdated(ulong steamId)
		{
			this.UpdateUser(steamId);
		}

		// Token: 0x040026AF RID: 9903
		public TextMeshProUGUI lobbyStateText;

		// Token: 0x040026B0 RID: 9904
		public GameObject copyLobbyIDToClipboardButton;

		// Token: 0x040026B1 RID: 9905
		private List<SteamworksLobbyUserList.Element> elements = new List<SteamworksLobbyUserList.Element>();

		// Token: 0x02000640 RID: 1600
		private class Element
		{
			// Token: 0x060023DB RID: 9179 RVA: 0x0001A17F File Offset: 0x0001837F
			public void SetUser(ulong steamId, int subPlayerIndex)
			{
				this.steamId = steamId;
				this.userIcon.SetFromSteamId(steamId);
				this.usernameLabel.userSteamId = steamId;
				this.usernameLabel.subPlayerIndex = subPlayerIndex;
				this.Refresh();
			}

			// Token: 0x060023DC RID: 9180 RVA: 0x000AAF8C File Offset: 0x000A918C
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

			// Token: 0x060023DD RID: 9181 RVA: 0x000AB02C File Offset: 0x000A922C
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

			// Token: 0x040026B2 RID: 9906
			public ulong steamId;

			// Token: 0x040026B3 RID: 9907
			public GameObject gameObject;

			// Token: 0x040026B4 RID: 9908
			public SocialUserIcon userIcon;

			// Token: 0x040026B5 RID: 9909
			public SteamUsernameLabel usernameLabel;

			// Token: 0x040026B6 RID: 9910
			public GameObject lobbyLeaderCrown;

			// Token: 0x040026B7 RID: 9911
			public ChildLocator elementChildLocator;
		}
	}
}
