using System;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000672 RID: 1650
	public sealed class MainMenuController : MonoBehaviour
	{
		// Token: 0x060024FF RID: 9471 RVA: 0x0001AF6E File Offset: 0x0001916E
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient client)
			{
				if (!NetworkServer.active || !NetworkServer.dontListen)
				{
					MainMenuController.wasInMultiplayer = true;
				}
			};
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x000AE354 File Offset: 0x000AC554
		private void Start()
		{
			MainMenuController.wasInMultiplayer = false;
			this.titleMenuScreen.gameObject.SetActive(false);
			this.multiplayerMenuScreen.gameObject.SetActive(false);
			this.settingsMenuScreen.gameObject.SetActive(false);
			this.moreMenuScreen.gameObject.SetActive(false);
			this.desiredMenuScreen = (MainMenuController.wasInMultiplayer ? this.multiplayerMenuScreen : this.titleMenuScreen);
			if (!MainMenuController.eaWarningShown || !MainMenuController.IsMainUserSignedIn())
			{
				MainMenuController.eaWarningShown = true;
				if (!MainMenuController.eaMessageSkipConVar.value)
				{
					this.desiredMenuScreen = this.EAwarningProfileMenu;
				}
			}
			this.currentMenuScreen = this.desiredMenuScreen;
			this.currentMenuScreen.gameObject.SetActive(true);
			this.cameraTransform.position = this.currentMenuScreen.desiredCameraTransform.position;
			this.cameraTransform.rotation = this.currentMenuScreen.desiredCameraTransform.rotation;
			if (this.currentMenuScreen)
			{
				this.currentMenuScreen.OnEnter(this);
			}
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x0001AF94 File Offset: 0x00019194
		private static bool IsMainUserSignedIn()
		{
			return LocalUserManager.FindLocalUser(0) != null;
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0001AF9F File Offset: 0x0001919F
		private bool IsInLobby()
		{
			return SteamworksLobbyManager.isInLobby;
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x000AE460 File Offset: 0x000AC660
		private void Update()
		{
			if (this.IsInLobby() && this.currentMenuScreen != this.multiplayerMenuScreen)
			{
				this.desiredMenuScreen = this.multiplayerMenuScreen;
			}
			if (!MainMenuController.IsMainUserSignedIn() && this.currentMenuScreen != this.EAwarningProfileMenu)
			{
				this.desiredMenuScreen = this.profileMenuScreen;
			}
			if (this.desiredMenuScreen != this.currentMenuScreen)
			{
				this.currentMenuScreen.shouldDisplay = false;
				if (this.currentMenuScreen.IsReadyToLeave())
				{
					this.currentMenuScreen.OnExit(this);
					this.currentMenuScreen.gameObject.SetActive(false);
					this.currentMenuScreen = this.desiredMenuScreen;
					this.camTransitionTimer = this.camTransitionDuration;
					this.currentMenuScreen.OnEnter(this);
					return;
				}
			}
			else
			{
				this.camTransitionTimer -= Time.deltaTime;
				this.cameraTransform.position = Vector3.SmoothDamp(this.cameraTransform.position, this.currentMenuScreen.desiredCameraTransform.position, ref this.camSmoothDampPositionVelocity, this.camTranslationSmoothDampTime);
				Vector3 eulerAngles = this.cameraTransform.eulerAngles;
				Vector3 eulerAngles2 = this.currentMenuScreen.desiredCameraTransform.eulerAngles;
				eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, eulerAngles2.x, ref this.camSmoothDampRotationVelocity.x, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, eulerAngles2.y, ref this.camSmoothDampRotationVelocity.y, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, eulerAngles2.z, ref this.camSmoothDampRotationVelocity.z, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				this.cameraTransform.eulerAngles = eulerAngles;
				if (this.camTransitionTimer <= 0f)
				{
					this.currentMenuScreen.gameObject.SetActive(true);
					this.currentMenuScreen.shouldDisplay = true;
				}
			}
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0001AFA6 File Offset: 0x000191A6
		public void SetDesiredMenuScreen(BaseMainMenuScreen newDesiredMenuScreen)
		{
			this.desiredMenuScreen = newDesiredMenuScreen;
		}

		// Token: 0x040027B8 RID: 10168
		[NonSerialized]
		public BaseMainMenuScreen desiredMenuScreen;

		// Token: 0x040027B9 RID: 10169
		public BaseMainMenuScreen profileMenuScreen;

		// Token: 0x040027BA RID: 10170
		public BaseMainMenuScreen EAwarningProfileMenu;

		// Token: 0x040027BB RID: 10171
		public BaseMainMenuScreen multiplayerMenuScreen;

		// Token: 0x040027BC RID: 10172
		public BaseMainMenuScreen titleMenuScreen;

		// Token: 0x040027BD RID: 10173
		public BaseMainMenuScreen settingsMenuScreen;

		// Token: 0x040027BE RID: 10174
		public BaseMainMenuScreen moreMenuScreen;

		// Token: 0x040027BF RID: 10175
		private BaseMainMenuScreen currentMenuScreen;

		// Token: 0x040027C0 RID: 10176
		public Transform cameraTransform;

		// Token: 0x040027C1 RID: 10177
		public float camRotationSmoothDampTime;

		// Token: 0x040027C2 RID: 10178
		public float camTranslationSmoothDampTime;

		// Token: 0x040027C3 RID: 10179
		private Vector3 camSmoothDampPositionVelocity;

		// Token: 0x040027C4 RID: 10180
		private Vector3 camSmoothDampRotationVelocity;

		// Token: 0x040027C5 RID: 10181
		public float camTransitionDuration;

		// Token: 0x040027C6 RID: 10182
		private float camTransitionTimer;

		// Token: 0x040027C7 RID: 10183
		private static bool wasInMultiplayer = false;

		// Token: 0x040027C8 RID: 10184
		private static bool eaWarningShown = false;

		// Token: 0x040027C9 RID: 10185
		private static BoolConVar eaMessageSkipConVar = new BoolConVar("ea_message_skip", ConVarFlags.None, "0", "Whether or not to skip the early access splash screen.");
	}
}
