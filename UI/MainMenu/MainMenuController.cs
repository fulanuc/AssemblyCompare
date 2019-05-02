using System;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000660 RID: 1632
	public sealed class MainMenuController : MonoBehaviour
	{
		// Token: 0x06002468 RID: 9320 RVA: 0x0001A83B File Offset: 0x00018A3B
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

		// Token: 0x06002469 RID: 9321 RVA: 0x000ACC64 File Offset: 0x000AAE64
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

		// Token: 0x0600246A RID: 9322 RVA: 0x0001A861 File Offset: 0x00018A61
		private static bool IsMainUserSignedIn()
		{
			return LocalUserManager.FindLocalUser(0) != null;
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x0001A86C File Offset: 0x00018A6C
		private bool IsInLobby()
		{
			return SteamworksLobbyManager.isInLobby;
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x000ACD70 File Offset: 0x000AAF70
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

		// Token: 0x0600246D RID: 9325 RVA: 0x0001A873 File Offset: 0x00018A73
		public void SetDesiredMenuScreen(BaseMainMenuScreen newDesiredMenuScreen)
		{
			this.desiredMenuScreen = newDesiredMenuScreen;
		}

		// Token: 0x0400275C RID: 10076
		[NonSerialized]
		public BaseMainMenuScreen desiredMenuScreen;

		// Token: 0x0400275D RID: 10077
		public BaseMainMenuScreen profileMenuScreen;

		// Token: 0x0400275E RID: 10078
		public BaseMainMenuScreen EAwarningProfileMenu;

		// Token: 0x0400275F RID: 10079
		public BaseMainMenuScreen multiplayerMenuScreen;

		// Token: 0x04002760 RID: 10080
		public BaseMainMenuScreen titleMenuScreen;

		// Token: 0x04002761 RID: 10081
		public BaseMainMenuScreen settingsMenuScreen;

		// Token: 0x04002762 RID: 10082
		public BaseMainMenuScreen moreMenuScreen;

		// Token: 0x04002763 RID: 10083
		private BaseMainMenuScreen currentMenuScreen;

		// Token: 0x04002764 RID: 10084
		public Transform cameraTransform;

		// Token: 0x04002765 RID: 10085
		public float camRotationSmoothDampTime;

		// Token: 0x04002766 RID: 10086
		public float camTranslationSmoothDampTime;

		// Token: 0x04002767 RID: 10087
		private Vector3 camSmoothDampPositionVelocity;

		// Token: 0x04002768 RID: 10088
		private Vector3 camSmoothDampRotationVelocity;

		// Token: 0x04002769 RID: 10089
		public float camTransitionDuration;

		// Token: 0x0400276A RID: 10090
		private float camTransitionTimer;

		// Token: 0x0400276B RID: 10091
		private static bool wasInMultiplayer = false;

		// Token: 0x0400276C RID: 10092
		private static bool eaWarningShown = false;

		// Token: 0x0400276D RID: 10093
		private static BoolConVar eaMessageSkipConVar = new BoolConVar("ea_message_skip", ConVarFlags.None, "0", "Whether or not to skip the early access splash screen.");
	}
}
