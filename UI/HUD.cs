using System;
using System.Collections.Generic;
using Assets.RoR2.Scripts.GameBehaviors.UI;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F2 RID: 1522
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class HUD : MonoBehaviour
	{
		// Token: 0x0600224B RID: 8779 RVA: 0x000A5114 File Offset: 0x000A3314
		private static void OnUICameraPreRender(UICamera uiCamera)
		{
			CameraRigController cameraRigController = uiCamera.cameraRigController;
			if (cameraRigController)
			{
				LocalUser localUser = cameraRigController.viewer ? cameraRigController.viewer.localUser : null;
				if (localUser != null)
				{
					HUD.lockInstancesList = true;
					for (int i = 0; i < HUD.instancesList.Count; i++)
					{
						HUD hud = HUD.instancesList[i];
						if (hud.localUserViewer == localUser)
						{
							hud.canvas.worldCamera = uiCamera.camera;
						}
						else
						{
							GameObject gameObject = hud.gameObject;
							HUD.instancesToReenableList.Add(gameObject);
							gameObject.SetActive(false);
						}
					}
					HUD.lockInstancesList = false;
				}
			}
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x000A51B8 File Offset: 0x000A33B8
		private static void OnUICameraPostRender(UICamera uiCamera)
		{
			HUD.lockInstancesList = true;
			for (int i = 0; i < HUD.instancesToReenableList.Count; i++)
			{
				HUD.instancesToReenableList[i].SetActive(true);
			}
			HUD.instancesToReenableList.Clear();
			HUD.lockInstancesList = false;
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x00018F93 File Offset: 0x00017193
		public void OnEnable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Add(this);
			}
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x00018FA7 File Offset: 0x000171A7
		public void OnDisable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Remove(this);
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600224F RID: 8783 RVA: 0x00018FBC File Offset: 0x000171BC
		public GameObject targetBodyObject
		{
			get
			{
				if (!this.targetMaster)
				{
					return null;
				}
				return this.targetMaster.GetBodyObject();
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x00018FD8 File Offset: 0x000171D8
		// (set) Token: 0x06002251 RID: 8785 RVA: 0x00018FE0 File Offset: 0x000171E0
		public CharacterMaster targetMaster { get; set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06002252 RID: 8786 RVA: 0x00018FE9 File Offset: 0x000171E9
		// (set) Token: 0x06002253 RID: 8787 RVA: 0x00018FF1 File Offset: 0x000171F1
		public LocalUser localUserViewer
		{
			get
			{
				return this._localUserViewer;
			}
			set
			{
				if (this._localUserViewer != value)
				{
					this._localUserViewer = value;
					this.eventSystemProvider.eventSystem = this._localUserViewer.eventSystem;
				}
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x00019019 File Offset: 0x00017219
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
			this.canvas = base.GetComponent<Canvas>();
			if (this.scoreboardPanel)
			{
				this.scoreboardPanel.SetActive(false);
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x0001904C File Offset: 0x0001724C
		private void Start()
		{
			this.mainContainer.SetActive(HUD.HUDEnableConVar.instance.boolValue);
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x000A5204 File Offset: 0x000A3404
		public void Update()
		{
			NetworkUser networkUser;
			if (!this.targetMaster)
			{
				networkUser = null;
			}
			else
			{
				PlayerCharacterMasterController component = this.targetMaster.GetComponent<PlayerCharacterMasterController>();
				networkUser = ((component != null) ? component.networkUser : null);
			}
			NetworkUser networkUser2 = networkUser;
			PlayerCharacterMasterController playerCharacterMasterController = this.targetMaster ? this.targetMaster.GetComponent<PlayerCharacterMasterController>() : null;
			Inventory inventory = this.targetMaster ? this.targetMaster.inventory : null;
			CharacterBody characterBody = this.targetBodyObject ? this.targetBodyObject.GetComponent<CharacterBody>() : null;
			if (this.healthBar && this.targetBodyObject)
			{
				this.healthBar.source = this.targetBodyObject.GetComponent<HealthComponent>();
			}
			if (this.expBar)
			{
				this.expBar.source = this.targetMaster;
			}
			if (this.levelText)
			{
				this.levelText.source = this.targetMaster;
			}
			if (this.moneyText)
			{
				this.moneyText.targetValue = (int)(this.targetMaster ? this.targetMaster.money : 0u);
			}
			if (this.lunarCoinContainer)
			{
				bool flag = this.localUserViewer != null && this.localUserViewer.userProfile.totalCollectedCoins > 0u;
				uint targetValue = networkUser2 ? networkUser2.lunarCoins : 0u;
				this.lunarCoinContainer.SetActive(flag);
				if (flag && this.lunarCoinText)
				{
					this.lunarCoinText.targetValue = (int)targetValue;
				}
			}
			if (this.itemInventoryDisplay)
			{
				this.itemInventoryDisplay.SetSubscribedInventory(inventory);
			}
			if (this.targetBodyObject)
			{
				SkillLocator component2 = this.targetBodyObject.GetComponent<SkillLocator>();
				if (component2)
				{
					if (this.skillIcons.Length != 0 && this.skillIcons[0])
					{
						this.skillIcons[0].targetSkillSlot = SkillSlot.Primary;
						this.skillIcons[0].targetSkill = component2.primary;
						this.skillIcons[0].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 1 && this.skillIcons[1])
					{
						this.skillIcons[1].targetSkillSlot = SkillSlot.Secondary;
						this.skillIcons[1].targetSkill = component2.secondary;
						this.skillIcons[1].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 2 && this.skillIcons[2])
					{
						this.skillIcons[2].targetSkillSlot = SkillSlot.Utility;
						this.skillIcons[2].targetSkill = component2.utility;
						this.skillIcons[2].playerCharacterMasterController = playerCharacterMasterController;
					}
					if (this.skillIcons.Length > 3 && this.skillIcons[3])
					{
						this.skillIcons[3].targetSkillSlot = SkillSlot.Special;
						this.skillIcons[3].targetSkill = component2.special;
						this.skillIcons[3].playerCharacterMasterController = playerCharacterMasterController;
					}
				}
			}
			foreach (EquipmentIcon equipmentIcon in this.equipmentIcons)
			{
				equipmentIcon.targetInventory = inventory;
				equipmentIcon.targetEquipmentSlot = (this.targetBodyObject ? this.targetBodyObject.GetComponent<EquipmentSlot>() : null);
				equipmentIcon.playerCharacterMasterController = (this.targetMaster ? this.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
			}
			if (this.buffDisplay)
			{
				this.buffDisplay.source = characterBody;
			}
			if (this.allyCardManager)
			{
				this.allyCardManager.sourceGameObject = this.targetBodyObject;
			}
			if (this.scoreboardPanel)
			{
				bool active = this.localUserViewer != null && this.localUserViewer.inputPlayer != null && this.localUserViewer.inputPlayer.GetButton("info");
				this.scoreboardPanel.SetActive(active);
			}
			if (this.speedometer)
			{
				this.speedometer.targetTransform = (this.targetBodyObject ? this.targetBodyObject.transform : null);
			}
			if (this.objectivePanelController)
			{
				this.objectivePanelController.SetCurrentMaster(this.targetMaster);
			}
			if (this.combatHealthBarViewer)
			{
				this.combatHealthBarViewer.crosshairTarget = (this.cameraRigController.lastCrosshairHurtBox ? this.cameraRigController.lastCrosshairHurtBox.healthComponent : null);
				this.combatHealthBarViewer.viewerBody = characterBody;
				this.combatHealthBarViewer.viewerBodyObject = this.targetBodyObject;
				this.combatHealthBarViewer.viewerTeamIndex = TeamComponent.GetObjectTeam(this.targetBodyObject);
			}
		}

		// Token: 0x040024F8 RID: 9464
		private static List<HUD> instancesList = new List<HUD>();

		// Token: 0x040024F9 RID: 9465
		private static bool lockInstancesList = false;

		// Token: 0x040024FA RID: 9466
		private static List<GameObject> instancesToReenableList = new List<GameObject>();

		// Token: 0x040024FC RID: 9468
		private LocalUser _localUserViewer;

		// Token: 0x040024FD RID: 9469
		[Tooltip("Immediate child of this object which contains all other UI.")]
		public GameObject mainContainer;

		// Token: 0x040024FE RID: 9470
		[NonSerialized]
		public CameraRigController cameraRigController;

		// Token: 0x040024FF RID: 9471
		public HealthBar healthBar;

		// Token: 0x04002500 RID: 9472
		public ExpBar expBar;

		// Token: 0x04002501 RID: 9473
		public LevelText levelText;

		// Token: 0x04002502 RID: 9474
		public MoneyText moneyText;

		// Token: 0x04002503 RID: 9475
		public GameObject lunarCoinContainer;

		// Token: 0x04002504 RID: 9476
		public MoneyText lunarCoinText;

		// Token: 0x04002505 RID: 9477
		public SkillIcon[] skillIcons;

		// Token: 0x04002506 RID: 9478
		public EquipmentIcon[] equipmentIcons;

		// Token: 0x04002507 RID: 9479
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x04002508 RID: 9480
		public BuffDisplay buffDisplay;

		// Token: 0x04002509 RID: 9481
		public AllyCardManager allyCardManager;

		// Token: 0x0400250A RID: 9482
		public ContextManager contextManager;

		// Token: 0x0400250B RID: 9483
		public GameObject scoreboardPanel;

		// Token: 0x0400250C RID: 9484
		public GameObject mainUIPanel;

		// Token: 0x0400250D RID: 9485
		public GameObject cinematicPanel;

		// Token: 0x0400250E RID: 9486
		public HUDSpeedometer speedometer;

		// Token: 0x0400250F RID: 9487
		public ObjectivePanelController objectivePanelController;

		// Token: 0x04002510 RID: 9488
		public CombatHealthBarViewer combatHealthBarViewer;

		// Token: 0x04002511 RID: 9489
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x04002512 RID: 9490
		private Canvas canvas;

		// Token: 0x020005F3 RID: 1523
		private class HUDEnableConVar : BaseConVar
		{
			// Token: 0x06002258 RID: 8792 RVA: 0x000090CD File Offset: 0x000072CD
			private HUDEnableConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002259 RID: 8793 RVA: 0x000A56A8 File Offset: 0x000A38A8
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					bool flag = num != 0;
					if (this.boolValue != flag)
					{
						this.boolValue = flag;
						foreach (HUD hud in HUD.instancesList)
						{
							hud.mainContainer.SetActive(this.boolValue);
						}
					}
				}
			}

			// Token: 0x0600225A RID: 8794 RVA: 0x00019063 File Offset: 0x00017263
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04002513 RID: 9491
			public static HUD.HUDEnableConVar instance = new HUD.HUDEnableConVar("hud_enable", ConVarFlags.None, "1", "Enable/disable the HUD.");

			// Token: 0x04002514 RID: 9492
			public bool boolValue;
		}
	}
}
