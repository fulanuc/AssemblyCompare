using System;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045E RID: 1118
	public class LocalUser
	{
		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001906 RID: 6406 RVA: 0x00012C7F File Offset: 0x00010E7F
		// (set) Token: 0x06001907 RID: 6407 RVA: 0x00081B78 File Offset: 0x0007FD78
		public Player inputPlayer
		{
			get
			{
				return this._inputPlayer;
			}
			set
			{
				if (this._inputPlayer == value)
				{
					return;
				}
				if (this._inputPlayer != null)
				{
					this.OnRewiredPlayerLost(this._inputPlayer);
				}
				this._inputPlayer = value;
				this.eventSystem = MPEventSystemManager.FindEventSystem(this._inputPlayer);
				if (this._inputPlayer != null)
				{
					this.OnRewiredPlayerDiscovered(this._inputPlayer);
				}
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06001908 RID: 6408 RVA: 0x00012C87 File Offset: 0x00010E87
		// (set) Token: 0x06001909 RID: 6409 RVA: 0x00012C8F File Offset: 0x00010E8F
		public MPEventSystem eventSystem { get; private set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x00012C98 File Offset: 0x00010E98
		// (set) Token: 0x0600190B RID: 6411 RVA: 0x00012CA0 File Offset: 0x00010EA0
		public UserProfile userProfile
		{
			get
			{
				return this._userProfile;
			}
			set
			{
				this._userProfile = value;
				this.ApplyUserProfileBindingsToRewiredPlayer();
			}
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x00012CAF File Offset: 0x00010EAF
		static LocalUser()
		{
			ReInput.ControllerConnectedEvent += LocalUser.OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += LocalUser.OnControllerDisconnected;
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x00081BD0 File Offset: 0x0007FDD0
		private static void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser.inputPlayer.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					localUser.OnControllerDiscovered(ReInput.controllers.GetController(args.controllerType, args.controllerId));
				}
			}
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x00081C50 File Offset: 0x0007FE50
		private static void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser.inputPlayer.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					localUser.OnControllerLost(ReInput.controllers.GetController(args.controllerType, args.controllerId));
				}
			}
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x00081CD0 File Offset: 0x0007FED0
		private void OnRewiredPlayerDiscovered(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerDiscovered(controller);
			}
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00081D24 File Offset: 0x0007FF24
		private void OnRewiredPlayerLost(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerLost(controller);
			}
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x000025DA File Offset: 0x000007DA
		private void OnControllerDiscovered(Controller controller)
		{
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00012CD3 File Offset: 0x00010ED3
		private void OnControllerLost(Controller controller)
		{
			this.inputPlayer.controllers.maps.ClearMapsForController(controller.type, controller.id, true);
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x00081D78 File Offset: 0x0007FF78
		private void ApplyUserProfileBindingsToRewiredPlayer()
		{
			if (this.inputPlayer == null)
			{
				return;
			}
			if (this.userProfile != null)
			{
				this.inputPlayer.controllers.maps.ClearAllMaps(false);
				foreach (Controller controller in this.inputPlayer.controllers.Controllers)
				{
					this.inputPlayer.controllers.maps.LoadMap(controller.type, controller.id, 2, 0);
					this.<ApplyUserProfileBindingsToRewiredPlayer>g__ApplyUserProfileBindingstoRewiredController|20_0(controller);
				}
				this.inputPlayer.controllers.maps.SetAllMapsEnabled(true);
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06001914 RID: 6420 RVA: 0x00012CF7 File Offset: 0x00010EF7
		public bool isUIFocused
		{
			get
			{
				return this.eventSystem.currentSelectedGameObject;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06001915 RID: 6421 RVA: 0x00012D09 File Offset: 0x00010F09
		// (set) Token: 0x06001916 RID: 6422 RVA: 0x00012D11 File Offset: 0x00010F11
		public NetworkUser currentNetworkUser { get; private set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06001917 RID: 6423 RVA: 0x00012D1A File Offset: 0x00010F1A
		// (set) Token: 0x06001918 RID: 6424 RVA: 0x00012D22 File Offset: 0x00010F22
		public PlayerCharacterMasterController cachedMasterController { get; private set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06001919 RID: 6425 RVA: 0x00012D2B File Offset: 0x00010F2B
		// (set) Token: 0x0600191A RID: 6426 RVA: 0x00012D33 File Offset: 0x00010F33
		public GameObject cachedMasterObject { get; private set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600191B RID: 6427 RVA: 0x00012D3C File Offset: 0x00010F3C
		// (set) Token: 0x0600191C RID: 6428 RVA: 0x00012D44 File Offset: 0x00010F44
		public CharacterBody cachedBody { get; private set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600191D RID: 6429 RVA: 0x00012D4D File Offset: 0x00010F4D
		// (set) Token: 0x0600191E RID: 6430 RVA: 0x00012D55 File Offset: 0x00010F55
		public GameObject cachedBodyObject { get; private set; }

		// Token: 0x0600191F RID: 6431 RVA: 0x00081E34 File Offset: 0x00080034
		public void RebuildControlChain()
		{
			UnityEngine.Object cachedMasterController = this.cachedMasterController;
			this.cachedMasterController = null;
			this.cachedMasterObject = null;
			UnityEngine.Object cachedBody = this.cachedBody;
			this.cachedBody = null;
			this.cachedBodyObject = null;
			if (this.currentNetworkUser)
			{
				this.cachedMasterObject = this.currentNetworkUser.masterObject;
				if (this.cachedMasterObject)
				{
					this.cachedMasterController = this.cachedMasterObject.GetComponent<PlayerCharacterMasterController>();
				}
				if (this.cachedMasterController)
				{
					this.cachedBody = this.cachedMasterController.master.GetBody();
					if (this.cachedBody)
					{
						this.cachedBodyObject = this.cachedBody.gameObject;
					}
				}
			}
			if (cachedBody != this.cachedBody)
			{
				Action action = this.onBodyChanged;
				if (action != null)
				{
					action();
				}
			}
			if (cachedMasterController != this.cachedMasterController)
			{
				Action action2 = this.onMasterChanged;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x06001920 RID: 6432 RVA: 0x00081F24 File Offset: 0x00080124
		// (remove) Token: 0x06001921 RID: 6433 RVA: 0x00081F5C File Offset: 0x0008015C
		public event Action onBodyChanged;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x06001922 RID: 6434 RVA: 0x00081F94 File Offset: 0x00080194
		// (remove) Token: 0x06001923 RID: 6435 RVA: 0x00081FCC File Offset: 0x000801CC
		public event Action onMasterChanged;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06001924 RID: 6436 RVA: 0x00082004 File Offset: 0x00080204
		// (remove) Token: 0x06001925 RID: 6437 RVA: 0x0008203C File Offset: 0x0008023C
		public event Action<NetworkUser> onNetworkUserFound;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x06001926 RID: 6438 RVA: 0x00082074 File Offset: 0x00080274
		// (remove) Token: 0x06001927 RID: 6439 RVA: 0x000820AC File Offset: 0x000802AC
		public event Action<NetworkUser> onNetworkUserLost;

		// Token: 0x06001928 RID: 6440 RVA: 0x00012D5E File Offset: 0x00010F5E
		public void LinkNetworkUser(NetworkUser newNetworkUser)
		{
			if (this.currentNetworkUser)
			{
				return;
			}
			this.currentNetworkUser = newNetworkUser;
			newNetworkUser.localUser = this;
			Action<NetworkUser> action = this.onNetworkUserFound;
			if (action == null)
			{
				return;
			}
			action(newNetworkUser);
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x000820E4 File Offset: 0x000802E4
		public void UnlinkNetworkUser()
		{
			Action<NetworkUser> action = this.onNetworkUserLost;
			if (action != null)
			{
				action(this.currentNetworkUser);
			}
			this.currentNetworkUser.localUser = null;
			this.currentNetworkUser = null;
			this.cachedMasterController = null;
			this.cachedMasterObject = null;
			this.cachedBody = null;
			this.cachedBodyObject = null;
		}

		// Token: 0x04001C6C RID: 7276
		private Player _inputPlayer;

		// Token: 0x04001C6E RID: 7278
		private UserProfile _userProfile;

		// Token: 0x04001C6F RID: 7279
		public int id;

		// Token: 0x04001C75 RID: 7285
		public CameraRigController cameraRigController;
	}
}
