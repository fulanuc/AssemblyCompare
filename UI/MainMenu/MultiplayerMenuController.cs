using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000676 RID: 1654
	public class MultiplayerMenuController : BaseMainMenuScreen
	{
		// Token: 0x17000330 RID: 816
		// (get) Token: 0x0600251A RID: 9498 RVA: 0x0001B0DF File Offset: 0x000192DF
		// (set) Token: 0x0600251B RID: 9499 RVA: 0x0001B0E6 File Offset: 0x000192E6
		public static MultiplayerMenuController instance { get; private set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x0600251C RID: 9500 RVA: 0x0001B0EE File Offset: 0x000192EE
		public bool isInHostingState
		{
			get
			{
				return this.state == MultiplayerMenuController.State.Hosting;
			}
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x0001B0F9 File Offset: 0x000192F9
		public void OnEnable()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
			this.state = MultiplayerMenuController.State.Idle;
			MultiplayerMenuController.instance = SingletonHelper.Assign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
			if (!SteamworksLobbyManager.isInLobby)
			{
				SteamworksLobbyManager.CreateLobby();
			}
			SteamworksLobbyManager.onLobbyLeave += this.OnLobbyLeave;
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x0001B136 File Offset: 0x00019336
		public void OnDisable()
		{
			SteamworksLobbyManager.onLobbyLeave -= this.OnLobbyLeave;
			if (!GameNetworkManager.singleton.isNetworkActive)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			MultiplayerMenuController.instance = SingletonHelper.Unassign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x0001B16A File Offset: 0x0001936A
		private void OnLobbyLeave(ulong lobbyId)
		{
			if (!SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.awaitingJoin)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x0001B17F File Offset: 0x0001937F
		public void Awake()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000AE928 File Offset: 0x000ACB28
		public void LerpAllUI(LerpUIRect.LerpState lerpState)
		{
			LerpUIRect[] array = this.uiToLerp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].lerpState = lerpState;
			}
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x0001B188 File Offset: 0x00019388
		public void Host()
		{
			if (this.state == MultiplayerMenuController.State.Idle)
			{
				Console.instance.SubmitCmd(null, "transition_command \"gamemode ClassicRun; host 1;\"", false);
				SteamworksLobbyManager.SetStartingIfOwner(true);
				this.state = MultiplayerMenuController.State.Hosting;
				this.titleStopwatch = 0f;
				this.LerpAllUI(LerpUIRect.LerpState.Leaving);
			}
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x000AE954 File Offset: 0x000ACB54
		private void Update()
		{
			this.titleStopwatch += Time.deltaTime;
			if (this.titleStopwatch >= 0.6f)
			{
				switch (this.state)
				{
				case MultiplayerMenuController.State.Hosting:
					this.state = MultiplayerMenuController.State.Waiting;
					this.titleStopwatch = -2f;
					break;
				case MultiplayerMenuController.State.Waiting:
					this.state = MultiplayerMenuController.State.Idle;
					this.titleStopwatch = 0.1f;
					this.LerpAllUI(LerpUIRect.LerpState.Entering);
					break;
				}
			}
			this.quickplayButton.interactable = this.ShouldEnableQuickplayButton();
			this.startPrivateGameButton.interactable = this.ShouldEnableStartPrivateGameButton();
			this.joinClipboardLobbyButtonController.mpButton.interactable = this.ShouldEnableJoinClipboardLobbyButton();
			this.inviteButton.interactable = this.ShouldEnableInviteButton();
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x0001B1C2 File Offset: 0x000193C2
		private bool ShouldEnableQuickplayButton()
		{
			return SteamworksLobbyManager.ownsLobby || SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0001B1D7 File Offset: 0x000193D7
		private bool ShouldEnableStartPrivateGameButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && SteamworksLobbyManager.ownsLobby;
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0001B1EC File Offset: 0x000193EC
		private bool ShouldEnableJoinClipboardLobbyButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && this.joinClipboardLobbyButtonController.validClipboardLobbyID;
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0001B207 File Offset: 0x00019407
		private bool ShouldEnableInviteButton()
		{
			return !SteamworksLobbyManager.isFull && !SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x040027D9 RID: 10201
		private const float titleTransitionDuration = 0.5f;

		// Token: 0x040027DA RID: 10202
		private const float titleTransitionBuffer = 0.1f;

		// Token: 0x040027DB RID: 10203
		public Image fadeImage;

		// Token: 0x040027DC RID: 10204
		public LerpUIRect[] uiToLerp;

		// Token: 0x040027DD RID: 10205
		private float titleStopwatch;

		// Token: 0x040027DE RID: 10206
		private MultiplayerMenuController.State state;

		// Token: 0x040027DF RID: 10207
		public MPButton quickplayButton;

		// Token: 0x040027E0 RID: 10208
		public MPButton startPrivateGameButton;

		// Token: 0x040027E1 RID: 10209
		public SteamJoinClipboardLobby joinClipboardLobbyButtonController;

		// Token: 0x040027E2 RID: 10210
		public MPButton inviteButton;

		// Token: 0x02000677 RID: 1655
		private enum State
		{
			// Token: 0x040027E4 RID: 10212
			Idle,
			// Token: 0x040027E5 RID: 10213
			Hosting,
			// Token: 0x040027E6 RID: 10214
			Waiting
		}
	}
}
