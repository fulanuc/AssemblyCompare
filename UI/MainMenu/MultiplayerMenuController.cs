using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000664 RID: 1636
	public class MultiplayerMenuController : BaseMainMenuScreen
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06002483 RID: 9347 RVA: 0x0001A9AC File Offset: 0x00018BAC
		// (set) Token: 0x06002484 RID: 9348 RVA: 0x0001A9B3 File Offset: 0x00018BB3
		public static MultiplayerMenuController instance { get; private set; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06002485 RID: 9349 RVA: 0x0001A9BB File Offset: 0x00018BBB
		public bool isInHostingState
		{
			get
			{
				return this.state == MultiplayerMenuController.State.Hosting;
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x0001A9C6 File Offset: 0x00018BC6
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

		// Token: 0x06002487 RID: 9351 RVA: 0x0001AA03 File Offset: 0x00018C03
		public void OnDisable()
		{
			SteamworksLobbyManager.onLobbyLeave -= this.OnLobbyLeave;
			if (!GameNetworkManager.singleton.isNetworkActive)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			MultiplayerMenuController.instance = SingletonHelper.Unassign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x0001AA37 File Offset: 0x00018C37
		private void OnLobbyLeave(ulong lobbyId)
		{
			if (!SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.awaitingJoin)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x0001AA4C File Offset: 0x00018C4C
		public void Awake()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x000AD238 File Offset: 0x000AB438
		public void LerpAllUI(LerpUIRect.LerpState lerpState)
		{
			LerpUIRect[] array = this.uiToLerp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].lerpState = lerpState;
			}
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x0001AA55 File Offset: 0x00018C55
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

		// Token: 0x0600248C RID: 9356 RVA: 0x000AD264 File Offset: 0x000AB464
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

		// Token: 0x0600248D RID: 9357 RVA: 0x0001AA8F File Offset: 0x00018C8F
		private bool ShouldEnableQuickplayButton()
		{
			return SteamworksLobbyManager.ownsLobby || SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x0001AAA4 File Offset: 0x00018CA4
		private bool ShouldEnableStartPrivateGameButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && SteamworksLobbyManager.ownsLobby;
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x0001AAB9 File Offset: 0x00018CB9
		private bool ShouldEnableJoinClipboardLobbyButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && this.joinClipboardLobbyButtonController.validClipboardLobbyID;
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x0001AAD4 File Offset: 0x00018CD4
		private bool ShouldEnableInviteButton()
		{
			return !SteamworksLobbyManager.isFull && !SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x000038B4 File Offset: 0x00001AB4
		public override bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x0400277D RID: 10109
		private const float titleTransitionDuration = 0.5f;

		// Token: 0x0400277E RID: 10110
		private const float titleTransitionBuffer = 0.1f;

		// Token: 0x0400277F RID: 10111
		public Image fadeImage;

		// Token: 0x04002780 RID: 10112
		public LerpUIRect[] uiToLerp;

		// Token: 0x04002781 RID: 10113
		private float titleStopwatch;

		// Token: 0x04002782 RID: 10114
		private MultiplayerMenuController.State state;

		// Token: 0x04002783 RID: 10115
		public MPButton quickplayButton;

		// Token: 0x04002784 RID: 10116
		public MPButton startPrivateGameButton;

		// Token: 0x04002785 RID: 10117
		public SteamJoinClipboardLobby joinClipboardLobbyButtonController;

		// Token: 0x04002786 RID: 10118
		public MPButton inviteButton;

		// Token: 0x02000665 RID: 1637
		private enum State
		{
			// Token: 0x04002788 RID: 10120
			Idle,
			// Token: 0x04002789 RID: 10121
			Hosting,
			// Token: 0x0400278A RID: 10122
			Waiting
		}
	}
}
