using System;
using System.Globalization;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063D RID: 1597
	public class SteamJoinClipboardLobby : MonoBehaviour
	{
		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060023C7 RID: 9159 RVA: 0x0001A0D0 File Offset: 0x000182D0
		// (set) Token: 0x060023C8 RID: 9160 RVA: 0x0001A0D8 File Offset: 0x000182D8
		public bool validClipboardLobbyID { get; private set; }

		// Token: 0x060023C9 RID: 9161 RVA: 0x0001A0E1 File Offset: 0x000182E1
		private void OnEnable()
		{
			SingletonHelper.Assign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x0001A0EE File Offset: 0x000182EE
		private void OnDisable()
		{
			SingletonHelper.Unassign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x0001A0FB File Offset: 0x000182FB
		static SteamJoinClipboardLobby()
		{
			SteamworksLobbyManager.onLobbyJoined += SteamJoinClipboardLobby.OnLobbyJoined;
		}

		// Token: 0x060023CC RID: 9164 RVA: 0x000AAB3C File Offset: 0x000A8D3C
		private static void OnLobbyJoined(bool success)
		{
			if (SteamJoinClipboardLobby.instance && SteamJoinClipboardLobby.instance.resultTextComponent)
			{
				SteamJoinClipboardLobby.instance.resultTextTimer = 4f;
				SteamJoinClipboardLobby.instance.resultTextComponent.text = Language.GetString(success ? "STEAM_JOIN_LOBBY_CLIPBOARD_SUCCESS" : "STEAM_JOIN_LOBBY_CLIPBOARD_FAIL");
			}
		}

		// Token: 0x060023CD RID: 9165 RVA: 0x000AAB98 File Offset: 0x000A8D98
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

		// Token: 0x060023CE RID: 9166 RVA: 0x0001A10E File Offset: 0x0001830E
		public void TryToJoinClipboardLobby()
		{
			Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_join {0}", this.clipboardLobbyID.ToString()), true);
		}

		// Token: 0x040026A7 RID: 9895
		public TextMeshProUGUI buttonText;

		// Token: 0x040026A8 RID: 9896
		public TextMeshProUGUI resultTextComponent;

		// Token: 0x040026A9 RID: 9897
		public MPButton mpButton;

		// Token: 0x040026AA RID: 9898
		private CSteamID clipboardLobbyID;

		// Token: 0x040026AC RID: 9900
		private const float resultTextDuration = 4f;

		// Token: 0x040026AD RID: 9901
		protected float resultTextTimer;

		// Token: 0x040026AE RID: 9902
		private static SteamJoinClipboardLobby instance;
	}
}
