using System;
using System.Collections.Generic;
using Assets.RoR2.Scripts.GameBehaviors.UI;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E0 RID: 1504
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class HUD : MonoBehaviour
	{
		// Token: 0x060021BA RID: 8634 RVA: 0x000A3B40 File Offset: 0x000A1D40
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

		// Token: 0x060021BB RID: 8635 RVA: 0x000A3BE4 File Offset: 0x000A1DE4
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

		// Token: 0x060021BC RID: 8636 RVA: 0x00018899 File Offset: 0x00016A99
		public void OnEnable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Add(this);
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x000188AD File Offset: 0x00016AAD
		public void OnDisable()
		{
			if (!HUD.lockInstancesList)
			{
				HUD.instancesList.Remove(this);
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x060021BE RID: 8638 RVA: 0x000188C2 File Offset: 0x00016AC2
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

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x060021BF RID: 8639 RVA: 0x000188DE File Offset: 0x00016ADE
		// (set) Token: 0x060021C0 RID: 8640 RVA: 0x000188E6 File Offset: 0x00016AE6
		public CharacterMaster targetMaster { get; set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060021C1 RID: 8641 RVA: 0x000188EF File Offset: 0x00016AEF
		// (set) Token: 0x060021C2 RID: 8642 RVA: 0x000188F7 File Offset: 0x00016AF7
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

		// Token: 0x060021C3 RID: 8643 RVA: 0x0001891F File Offset: 0x00016B1F
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
			this.canvas = base.GetComponent<Canvas>();
			if (this.scoreboardPanel)
			{
				this.scoreboardPanel.SetActive(false);
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x00018952 File Offset: 0x00016B52
		private void Start()
		{
			this.mainContainer.SetActive(HUD.HUDEnableConVar.instance.boolValue);
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x000A3C30 File Offset: 0x000A1E30
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

		// Token: 0x040024A4 RID: 9380
		private static List<HUD> instancesList = new List<HUD>();

		// Token: 0x040024A5 RID: 9381
		private static bool lockInstancesList = false;

		// Token: 0x040024A6 RID: 9382
		private static List<GameObject> instancesToReenableList = new List<GameObject>();

		// Token: 0x040024A8 RID: 9384
		private LocalUser _localUserViewer;

		// Token: 0x040024A9 RID: 9385
		[Tooltip("Immediate child of this object which contains all other UI.")]
		public GameObject mainContainer;

		// Token: 0x040024AA RID: 9386
		[NonSerialized]
		public CameraRigController cameraRigController;

		// Token: 0x040024AB RID: 9387
		public HealthBar healthBar;

		// Token: 0x040024AC RID: 9388
		public ExpBar expBar;

		// Token: 0x040024AD RID: 9389
		public LevelText levelText;

		// Token: 0x040024AE RID: 9390
		public MoneyText moneyText;

		// Token: 0x040024AF RID: 9391
		public GameObject lunarCoinContainer;

		// Token: 0x040024B0 RID: 9392
		public MoneyText lunarCoinText;

		// Token: 0x040024B1 RID: 9393
		public SkillIcon[] skillIcons;

		// Token: 0x040024B2 RID: 9394
		public EquipmentIcon[] equipmentIcons;

		// Token: 0x040024B3 RID: 9395
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x040024B4 RID: 9396
		public BuffDisplay buffDisplay;

		// Token: 0x040024B5 RID: 9397
		public AllyCardManager allyCardManager;

		// Token: 0x040024B6 RID: 9398
		public ContextManager contextManager;

		// Token: 0x040024B7 RID: 9399
		public GameObject scoreboardPanel;

		// Token: 0x040024B8 RID: 9400
		public GameObject mainUIPanel;

		// Token: 0x040024B9 RID: 9401
		public GameObject cinematicPanel;

		// Token: 0x040024BA RID: 9402
		public HUDSpeedometer speedometer;

		// Token: 0x040024BB RID: 9403
		public ObjectivePanelController objectivePanelController;

		// Token: 0x040024BC RID: 9404
		public CombatHealthBarViewer combatHealthBarViewer;

		// Token: 0x040024BD RID: 9405
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x040024BE RID: 9406
		private Canvas canvas;

		// Token: 0x020005E1 RID: 1505
		private class HUDEnableConVar : BaseConVar
		{
			// Token: 0x060021C7 RID: 8647 RVA: 0x000090A8 File Offset: 0x000072A8
			private HUDEnableConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060021C8 RID: 8648 RVA: 0x000A40D4 File Offset: 0x000A22D4
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

			// Token: 0x060021C9 RID: 8649 RVA: 0x00018969 File Offset: 0x00016B69
			public override string GetString()
			{
				if (!this.boolValue)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x040024BF RID: 9407
			public static HUD.HUDEnableConVar instance = new HUD.HUDEnableConVar("hud_enable", ConVarFlags.None, "1", "Enable/disable the HUD.");

			// Token: 0x040024C0 RID: 9408
			public bool boolValue;
		}
	}
}
