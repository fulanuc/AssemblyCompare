using System;
using System.Globalization;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064F RID: 1615
	public class SteamJoinClipboardLobby : MonoBehaviour
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06002457 RID: 9303 RVA: 0x0001A79E File Offset: 0x0001899E
		// (set) Token: 0x06002458 RID: 9304 RVA: 0x0001A7A6 File Offset: 0x000189A6
		public bool validClipboardLobbyID { get; private set; }

		// Token: 0x06002459 RID: 9305 RVA: 0x0001A7AF File Offset: 0x000189AF
		private void OnEnable()
		{
			SingletonHelper.Assign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x0600245A RID: 9306 RVA: 0x0001A7BC File Offset: 0x000189BC
		private void OnDisable()
		{
			SingletonHelper.Unassign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x0001A7C9 File Offset: 0x000189C9
		static SteamJoinClipboardLobby()
		{
			SteamworksLobbyManager.onLobbyJoined += SteamJoinClipboardLobby.OnLobbyJoined;
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x000AC1B8 File Offset: 0x000AA3B8
		private static void OnLobbyJoined(bool success)
		{
			if (SteamJoinClipboardLobby.instance && SteamJoinClipboardLobby.instance.resultTextComponent)
			{
				SteamJoinClipboardLobby.instance.resultTextTimer = 4f;
				SteamJoinClipboardLobby.instance.resultTextComponent.text = Language.GetString(success ? "STEAM_JOIN_LOBBY_CLIPBOARD_SUCCESS" : "STEAM_JOIN_LOBBY_CLIPBOARD_FAIL");
			}
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x000AC214 File Offset: 0x000AA414
		private void FixedUpdate()
		{
			Client client = Client.Instance;
			this.validClipboardLobbyID = false;
			if (client != null)
			{
				string systemCopyBuffer = GUIUtility.systemCopyBuffer;
				this.validClipboardLobbyID = (CSteamID.TryParse(systemCopyBuffer, out this.clipboardLobbyID) && this.clipboardLobbyID.isLobby && this.clipboardLobbyID.value != client.Lobby.CurrentLobby);
			}
			this.buttonText.text = string.Format(Language.GetString("STEAM_JOIN_LOBBY_ON_CLIPBOARD"), Array.Empty<object>());
			if (this.resultTextTimer > 0f)
			{
				this.resultTextTimer -= Time.fixedDeltaTime;
				this.resultTextComponent.enabled = true;
				return;
			}
			this.resultTextComponent.enabled = false;
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x0001A7DC File Offset: 0x000189DC
		public void TryToJoinClipboardLobby()
		{
			Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_join {0}", this.clipboardLobbyID.ToString()), true);
		}

		// Token: 0x04002702 RID: 9986
		public TextMeshProUGUI buttonText;

		// Token: 0x04002703 RID: 9987
		public TextMeshProUGUI resultTextComponent;

		// Token: 0x04002704 RID: 9988
		public MPButton mpButton;

		// Token: 0x04002705 RID: 9989
		private CSteamID clipboardLobbyID;

		// Token: 0x04002707 RID: 9991
		private const float resultTextDuration = 4f;

		// Token: 0x04002708 RID: 9992
		protected float resultTextTimer;

		// Token: 0x04002709 RID: 9993
		private static SteamJoinClipboardLobby instance;
	}
}
