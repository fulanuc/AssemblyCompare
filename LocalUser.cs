using System;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000453 RID: 1107
	public class LocalUser
	{
		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060018AA RID: 6314 RVA: 0x00012772 File Offset: 0x00010972
		// (set) Token: 0x060018AB RID: 6315 RVA: 0x000811D8 File Offset: 0x0007F3D8
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

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060018AC RID: 6316 RVA: 0x0001277A File Offset: 0x0001097A
		// (set) Token: 0x060018AD RID: 6317 RVA: 0x00012782 File Offset: 0x00010982
		public MPEventSystem eventSystem { get; private set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060018AE RID: 6318 RVA: 0x0001278B File Offset: 0x0001098B
		// (set) Token: 0x060018AF RID: 6319 RVA: 0x00012793 File Offset: 0x00010993
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

		// Token: 0x060018B0 RID: 6320 RVA: 0x000127A2 File Offset: 0x000109A2
		static LocalUser()
		{
			ReInput.ControllerConnectedEvent += LocalUser.OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += LocalUser.OnControllerDisconnected;
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x00081230 File Offset: 0x0007F430
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

		// Token: 0x060018B2 RID: 6322 RVA: 0x000812B0 File Offset: 0x0007F4B0
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

		// Token: 0x060018B3 RID: 6323 RVA: 0x00081330 File Offset: 0x0007F530
		private void OnRewiredPlayerDiscovered(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerDiscovered(controller);
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x00081384 File Offset: 0x0007F584
		private void OnRewiredPlayerLost(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerLost(controller);
			}
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x000025F6 File Offset: 0x000007F6
		private void OnControllerDiscovered(Controller controller)
		{
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x000127C6 File Offset: 0x000109C6
		private void OnControllerLost(Controller controller)
		{
			this.inputPlayer.controllers.maps.ClearMapsForController(controller.type, controller.id, true);
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x000813D8 File Offset: 0x0007F5D8
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

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060018B8 RID: 6328 RVA: 0x000127EA File Offset: 0x000109EA
		public bool isUIFocused
		{
			get
			{
				return this.eventSystem.currentSelectedGameObject;
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060018B9 RID: 6329 RVA: 0x000127FC File Offset: 0x000109FC
		// (set) Token: 0x060018BA RID: 6330 RVA: 0x00012804 File Offset: 0x00010A04
		public NetworkUser currentNetworkUser { get; private set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060018BB RID: 6331 RVA: 0x0001280D File Offset: 0x00010A0D
		// (set) Token: 0x060018BC RID: 6332 RVA: 0x00012815 File Offset: 0x00010A15
		public PlayerCharacterMasterController cachedMasterController { get; private set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060018BD RID: 6333 RVA: 0x0001281E File Offset: 0x00010A1E
		// (set) Token: 0x060018BE RID: 6334 RVA: 0x00012826 File Offset: 0x00010A26
		public GameObject cachedMasterObject { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060018BF RID: 6335 RVA: 0x0001282F File Offset: 0x00010A2F
		// (set) Token: 0x060018C0 RID: 6336 RVA: 0x00012837 File Offset: 0x00010A37
		public CharacterBody cachedBody { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060018C1 RID: 6337 RVA: 0x00012840 File Offset: 0x00010A40
		// (set) Token: 0x060018C2 RID: 6338 RVA: 0x00012848 File Offset: 0x00010A48
		public GameObject cachedBodyObject { get; private set; }

		// Token: 0x060018C3 RID: 6339 RVA: 0x00081494 File Offset: 0x0007F694
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

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060018C4 RID: 6340 RVA: 0x00081584 File Offset: 0x0007F784
		// (remove) Token: 0x060018C5 RID: 6341 RVA: 0x000815BC File Offset: 0x0007F7BC
		public event Action onBodyChanged;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060018C6 RID: 6342 RVA: 0x000815F4 File Offset: 0x0007F7F4
		// (remove) Token: 0x060018C7 RID: 6343 RVA: 0x0008162C File Offset: 0x0007F82C
		public event Action onMasterChanged;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060018C8 RID: 6344 RVA: 0x00081664 File Offset: 0x0007F864
		// (remove) Token: 0x060018C9 RID: 6345 RVA: 0x0008169C File Offset: 0x0007F89C
		public event Action<NetworkUser> onNetworkUserFound;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060018CA RID: 6346 RVA: 0x000816D4 File Offset: 0x0007F8D4
		// (remove) Token: 0x060018CB RID: 6347 RVA: 0x0008170C File Offset: 0x0007F90C
		public event Action<NetworkUser> onNetworkUserLost;

		// Token: 0x060018CC RID: 6348 RVA: 0x00012851 File Offset: 0x00010A51
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

		// Token: 0x060018CD RID: 6349 RVA: 0x00081744 File Offset: 0x0007F944
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

		// Token: 0x04001C38 RID: 7224
		private Player _inputPlayer;

		// Token: 0x04001C3A RID: 7226
		private UserProfile _userProfile;

		// Token: 0x04001C3B RID: 7227
		public int id;

		// Token: 0x04001C41 RID: 7233
		public CameraRigController cameraRigController;
	}
}
