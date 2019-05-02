using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.Networking;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000281 RID: 641
	[RequireComponent(typeof(SkillLocator))]
	[RequireComponent(typeof(TeamComponent))]
	[DisallowMultipleComponent]
	public class CharacterBody : NetworkBehaviour, ILifeBehavior, IDisplayNameProvider
	{
		// Token: 0x06000C27 RID: 3111 RVA: 0x000098A7 File Offset: 0x00007AA7
		public string GetDisplayName()
		{
			return Language.GetString(this.baseNameToken);
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x000098B4 File Offset: 0x00007AB4
		public string GetSubtitle()
		{
			return Language.GetString(this.subtitleNameToken);
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0004F898 File Offset: 0x0004DA98
		public string GetUserName()
		{
			string text = "";
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					text = component.GetDisplayName();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = this.GetDisplayName();
			}
			return text;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0004F8E4 File Offset: 0x0004DAE4
		public string GetColoredUserName()
		{
			Color32 userColor = new Color32(127, 127, 127, byte.MaxValue);
			string text = null;
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					GameObject networkUserObject = component.networkUserObject;
					if (networkUserObject)
					{
						NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
						if (component2)
						{
							userColor = component2.userColor;
							text = component2.userName;
						}
					}
				}
			}
			if (text == null)
			{
				text = this.GetDisplayName();
			}
			return Util.GenerateColoredString(text, userColor);
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x000098C1 File Offset: 0x00007AC1
		[Server]
		public void AddBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] + 1);
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0004F968 File Offset: 0x0004DB68
		[Server]
		public void RemoveBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::RemoveBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] - 1);
			if (buffType == BuffIndex.MedkitHeal && this.GetBuffCount(BuffIndex.MedkitHeal) == 0)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.Medkit);
				this.healthComponent.Heal((float)itemCount * 10f, default(ProcChainMask), true);
				Util.PlaySound("Play_item_proc_crit_heal", base.gameObject);
				EffectData effectData = new EffectData();
				effectData.origin = this.transform.position;
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MedkitHealEffect"), effectData, true);
			}
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x0004FA24 File Offset: 0x0004DC24
		[Server]
		private void SetBuffCount(BuffIndex buffType, int newCount)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::SetBuffCount(RoR2.BuffIndex,System.Int32)' called on client");
				return;
			}
			if (newCount == this.buffs[(int)buffType])
			{
				return;
			}
			this.buffs[(int)buffType] = newCount;
			BuffMask a = (newCount == 0) ? this.buffMask.GetBuffRemoved(buffType) : this.buffMask.GetBuffAdded(buffType);
			if (a != this.buffMask)
			{
				BuffMask oldBuffMask = this.buffMask;
				this.buffMask = a;
				if (NetworkClient.active)
				{
					this.OnClientBuffsChanged(oldBuffMask);
				}
				base.SetDirtyBit(2u);
			}
			if (buffType != BuffIndex.AttackSpeedOnCrit)
			{
				if (buffType != BuffIndex.OnFire)
				{
					if (buffType == BuffIndex.BeetleJuice)
					{
						base.SetDirtyBit(128u);
					}
				}
				else
				{
					base.SetDirtyBit(64u);
				}
			}
			else
			{
				base.SetDirtyBit(4u);
			}
			this.statsDirty = true;
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x000098E9 File Offset: 0x00007AE9
		public int GetBuffCount(BuffIndex buffType)
		{
			if (NetworkServer.active)
			{
				return this.buffs[(int)buffType];
			}
			if (BuffCatalog.GetBuffDef(buffType).canStack)
			{
				return this.buffs[(int)buffType];
			}
			if (!this.HasBuff(buffType))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x0000991D File Offset: 0x00007B1D
		public bool HasBuff(BuffIndex buffType)
		{
			return this.buffMask.HasBuff(buffType);
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0004FAE0 File Offset: 0x0004DCE0
		[Server]
		public void AddTimedBuff(BuffIndex buffType, float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddTimedBuff(RoR2.BuffIndex,System.Single)' called on client");
				return;
			}
			BuffDef buffDef = BuffCatalog.GetBuffDef(buffType);
			if (buffType != BuffIndex.AttackSpeedOnCrit)
			{
				if (buffType != BuffIndex.BeetleJuice)
				{
					bool flag = false;
					if (!buffDef.canStack)
					{
						for (int i = 0; i < this.timedBuffs.Count; i++)
						{
							if (this.timedBuffs[i].buffIndex == buffType)
							{
								flag = true;
								this.timedBuffs[i].timer = Mathf.Max(this.timedBuffs[i].timer, duration);
								break;
							}
						}
					}
					if (!flag)
					{
						this.timedBuffs.Add(new CharacterBody.TimedBuff
						{
							buffIndex = buffType,
							timer = duration
						});
						this.AddBuff(buffType);
					}
				}
				else
				{
					int num = 0;
					for (int j = 0; j < this.timedBuffs.Count; j++)
					{
						if (this.timedBuffs[j].buffIndex == buffType)
						{
							num++;
							if (this.timedBuffs[j].timer < duration)
							{
								this.timedBuffs[j].timer = duration;
							}
						}
					}
					if (num < 10)
					{
						this.timedBuffs.Add(new CharacterBody.TimedBuff
						{
							buffIndex = buffType,
							timer = duration
						});
						this.AddBuff(buffType);
						return;
					}
				}
				return;
			}
			int num2 = this.inventory ? this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit) : 0;
			int num3 = 0;
			int num4 = -1;
			float num5 = 999f;
			for (int k = 0; k < this.timedBuffs.Count; k++)
			{
				if (this.timedBuffs[k].buffIndex == buffType)
				{
					num3++;
					if (this.timedBuffs[k].timer < num5)
					{
						num4 = k;
						num5 = this.timedBuffs[k].timer;
					}
				}
			}
			if (num3 < 1 + num2 * 2)
			{
				this.timedBuffs.Add(new CharacterBody.TimedBuff
				{
					buffIndex = buffType,
					timer = duration
				});
				this.AddBuff(buffType);
				ChildLocator component = this.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("HandL");
					Transform transform2 = component.FindChild("HandR");
					if (transform)
					{
						UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/WolfProcEffect"), transform).transform.localScale = Vector3.one * (float)num3;
					}
					if (transform2)
					{
						UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Effects/WolfProcEffect"), transform2).transform.localScale = Vector3.one * (float)num3;
					}
				}
			}
			else if (num4 > -1)
			{
				this.timedBuffs[num4].timer = duration;
			}
			Util.PlaySound("Play_item_proc_crit_attack_speed" + Mathf.Min(3, num3 + 1), base.gameObject);
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0004FDCC File Offset: 0x0004DFCC
		[Server]
		public void ClearTimedBuffs(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ClearTimedBuffs(RoR2.BuffIndex)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				if (this.timedBuffs[i].buffIndex == buffType)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0004FE40 File Offset: 0x0004E040
		[Server]
		private void UpdateBuffs(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateBuffs(System.Single)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				this.timedBuffs[i].timer -= deltaTime;
				if (this.timedBuffs[i].timer <= 0f)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0004FED0 File Offset: 0x0004E0D0
		[Client]
		private void OnClientBuffsChanged(BuffMask oldBuffMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::OnClientBuffsChanged(RoR2.BuffMask)' called on server");
				return;
			}
			bool flag = this.buffMask.HasBuff(BuffIndex.WarCryBuff);
			if (!flag && this.warCryEffectInstance)
			{
				UnityEngine.Object.Destroy(this.warCryEffectInstance);
			}
			if (flag && !this.warCryEffectInstance)
			{
				Debug.Log("should spawn warcry");
				Transform transform = this.mainHurtBox ? this.mainHurtBox.transform : this.transform;
				if (transform)
				{
					Debug.Log("main hurtbox found");
					this.warCryEffectInstance = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Effects/WarCryEffect"), transform.position, Quaternion.identity, transform);
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000C34 RID: 3124 RVA: 0x0000992B File Offset: 0x00007B2B
		public CharacterMaster master
		{
			get
			{
				if (!this.masterObject)
				{
					return null;
				}
				return this._master;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x00009942 File Offset: 0x00007B42
		// (set) Token: 0x06000C36 RID: 3126 RVA: 0x0000994A File Offset: 0x00007B4A
		public Inventory inventory { get; private set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000C37 RID: 3127 RVA: 0x00009953 File Offset: 0x00007B53
		// (set) Token: 0x06000C38 RID: 3128 RVA: 0x0000995B File Offset: 0x00007B5B
		public bool isPlayerControlled { get; private set; }

		// Token: 0x06000C39 RID: 3129 RVA: 0x0004FF8C File Offset: 0x0004E18C
		private void OnInventoryChanged()
		{
			EquipmentIndex currentEquipmentIndex = this.inventory.currentEquipmentIndex;
			if (currentEquipmentIndex != this.previousEquipmentIndex)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.previousEquipmentIndex);
				EquipmentDef equipmentDef2 = EquipmentCatalog.GetEquipmentDef(currentEquipmentIndex);
				if (equipmentDef != null)
				{
					this.OnEquipmentLost(equipmentDef);
				}
				if (equipmentDef2 != null)
				{
					this.OnEquipmentGained(equipmentDef2);
				}
				this.previousEquipmentIndex = currentEquipmentIndex;
			}
			this.statsDirty = true;
			bool flag = this.inventory.GetItemCount(ItemIndex.Ghost) > 0;
			if (flag != this.disablingHurtBoxes)
			{
				if (this.hurtBoxGroup)
				{
					if (flag)
					{
						HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
						int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
						hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
					}
					else
					{
						HurtBoxGroup hurtBoxGroup2 = this.hurtBoxGroup;
						int hurtBoxesDeactivatorCounter = hurtBoxGroup2.hurtBoxesDeactivatorCounter - 1;
						hurtBoxGroup2.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
					}
				}
				this.disablingHurtBoxes = flag;
			}
			this.AddItemBehavior<CharacterBody.MushroomItemBehavior>(this.inventory.GetItemCount(ItemIndex.Mushroom));
			this.AddItemBehavior<CharacterBody.IcicleItemBehavior>(this.inventory.GetItemCount(ItemIndex.Icicle));
			this.AddItemBehavior<CharacterBody.HeadstomperItemBehavior>(this.inventory.GetItemCount(ItemIndex.FallBoots));
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x00009964 File Offset: 0x00007B64
		private void OnEquipmentLost(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.RemoveBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x00009982 File Offset: 0x00007B82
		private void OnEquipmentGained(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.AddBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000C3C RID: 3132 RVA: 0x00050090 File Offset: 0x0004E290
		// (remove) Token: 0x06000C3D RID: 3133 RVA: 0x000500C8 File Offset: 0x0004E2C8
		public event Action onInventoryChanged;

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x00050100 File Offset: 0x0004E300
		// (set) Token: 0x06000C3F RID: 3135 RVA: 0x000099A0 File Offset: 0x00007BA0
		public GameObject masterObject
		{
			get
			{
				if (!this._masterObject)
				{
					if (NetworkServer.active)
					{
						this._masterObject = NetworkServer.FindLocalObject(this.masterObjectId);
					}
					else if (NetworkClient.active)
					{
						this._masterObject = ClientScene.FindLocalObject(this.masterObjectId);
					}
					this._master = (this._masterObject ? this._masterObject.GetComponent<CharacterMaster>() : null);
					if (this._master)
					{
						this.isPlayerControlled = (this._masterObject && this._masterObject.GetComponent<PlayerCharacterMasterController>());
						if (this.inventory)
						{
							this.inventory.onInventoryChanged -= this.OnInventoryChanged;
						}
						this.inventory = this._master.inventory;
						if (this.inventory)
						{
							this.inventory.onInventoryChanged += this.OnInventoryChanged;
							this.OnInventoryChanged();
						}
						this.statsDirty = true;
					}
				}
				return this._masterObject;
			}
			set
			{
				this.masterObjectId = value.GetComponent<NetworkIdentity>().netId;
				this.statsDirty = true;
			}
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x000099BA File Offset: 0x00007BBA
		private void UpdateMasterLink()
		{
			if (!this.linkedToMaster && this.master && this.master)
			{
				this.master.OnBodyStart(this);
				this.linkedToMaster = true;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000C41 RID: 3137 RVA: 0x000099F1 File Offset: 0x00007BF1
		// (set) Token: 0x06000C42 RID: 3138 RVA: 0x000099F9 File Offset: 0x00007BF9
		public CharacterMotor characterMotor { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000C43 RID: 3139 RVA: 0x00009A02 File Offset: 0x00007C02
		// (set) Token: 0x06000C44 RID: 3140 RVA: 0x00009A0A File Offset: 0x00007C0A
		public TeamComponent teamComponent { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000C45 RID: 3141 RVA: 0x00009A13 File Offset: 0x00007C13
		// (set) Token: 0x06000C46 RID: 3142 RVA: 0x00009A1B File Offset: 0x00007C1B
		public HealthComponent healthComponent { get; private set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x00009A24 File Offset: 0x00007C24
		// (set) Token: 0x06000C48 RID: 3144 RVA: 0x00009A2C File Offset: 0x00007C2C
		public EquipmentSlot equipmentSlot { get; private set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x00009A35 File Offset: 0x00007C35
		// (set) Token: 0x06000C4A RID: 3146 RVA: 0x00009A3D File Offset: 0x00007C3D
		public ModelLocator modelLocator { get; private set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000C4B RID: 3147 RVA: 0x00009A46 File Offset: 0x00007C46
		// (set) Token: 0x06000C4C RID: 3148 RVA: 0x00009A4E File Offset: 0x00007C4E
		public HurtBoxGroup hurtBoxGroup { get; private set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000C4D RID: 3149 RVA: 0x00009A57 File Offset: 0x00007C57
		// (set) Token: 0x06000C4E RID: 3150 RVA: 0x00009A5F File Offset: 0x00007C5F
		public HurtBox mainHurtBox { get; private set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x00009A68 File Offset: 0x00007C68
		// (set) Token: 0x06000C50 RID: 3152 RVA: 0x00009A70 File Offset: 0x00007C70
		public Transform coreTransform { get; private set; }

		// Token: 0x06000C51 RID: 3153 RVA: 0x00050214 File Offset: 0x0004E414
		private void Awake()
		{
			this.transform = base.transform;
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.equipmentSlot = base.GetComponent<EquipmentSlot>();
			this.skillLocator = base.GetComponent<SkillLocator>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.characterMotor = base.GetComponent<CharacterMotor>();
			this.sfxLocator = base.GetComponent<SfxLocator>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				this.hurtBoxGroup = ((modelTransform != null) ? modelTransform.GetComponent<HurtBoxGroup>() : null);
				if (this.hurtBoxGroup)
				{
					this.mainHurtBox = this.hurtBoxGroup.mainHurtBox;
				}
			}
			HurtBox mainHurtBox = this.mainHurtBox;
			this.coreTransform = (((mainHurtBox != null) ? mainHurtBox.transform : null) ?? this.transform);
			this.radius = 1f;
			CapsuleCollider component = base.GetComponent<CapsuleCollider>();
			if (component)
			{
				this.radius = component.radius;
				return;
			}
			SphereCollider component2 = base.GetComponent<SphereCollider>();
			if (component2)
			{
				this.radius = component2.radius;
			}
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x00050330 File Offset: 0x0004E530
		private void Start()
		{
			bool flag = (this.bodyFlags & CharacterBody.BodyFlags.Masterless) > CharacterBody.BodyFlags.None;
			this.outOfCombatStopwatch = float.PositiveInfinity;
			this.outOfDangerStopwatch = float.PositiveInfinity;
			this.notMovingStopwatch = 0f;
			this.warCryTimer = 30f;
			if (NetworkServer.active)
			{
				this.outOfCombat = true;
				this.outOfDanger = true;
			}
			GlobalEventManager.instance.OnCharacterBodyStart(this);
			this.RecalculateStats();
			this.UpdateMasterLink();
			if (flag)
			{
				this.healthComponent.Networkhealth = this.maxHealth;
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x00009A79 File Offset: 0x00007C79
		public void Update()
		{
			this.UpdateSpreadBloom(Time.deltaTime);
			this.UpdateAllTemporaryVisualEffects();
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x000503B4 File Offset: 0x0004E5B4
		public void FixedUpdate()
		{
			this.outOfCombatStopwatch += Time.fixedDeltaTime;
			this.outOfDangerStopwatch += Time.fixedDeltaTime;
			this.aimTimer = Mathf.Max(this.aimTimer - Time.fixedDeltaTime, 0f);
			if (NetworkServer.active)
			{
				this.UpdateMultiKill(Time.fixedDeltaTime);
			}
			this.UpdateMasterLink();
			bool outOfCombat = this.outOfCombat;
			bool flag = outOfCombat;
			if (NetworkServer.active || base.hasAuthority)
			{
				flag = (this.outOfCombatStopwatch >= 5f);
				if (this.outOfCombat != flag)
				{
					if (NetworkServer.active)
					{
						base.SetDirtyBit(8u);
					}
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if (NetworkServer.active)
			{
				this.UpdateBuffs(Time.fixedDeltaTime);
				bool flag2 = this.outOfDangerStopwatch >= 7f;
				bool outOfDanger = this.outOfDanger;
				bool flag3 = outOfCombat && outOfDanger;
				bool flag4 = flag && flag2;
				if (this.outOfDanger != flag2)
				{
					base.SetDirtyBit(16u);
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
				if (flag4 && !flag3)
				{
					this.OnOutOfCombatAndDangerServer();
				}
				Vector3 position = this.transform.position;
				float num = 0.1f * Time.fixedDeltaTime;
				if ((position - this.previousPosition).sqrMagnitude <= num * num)
				{
					this.notMovingStopwatch += Time.fixedDeltaTime;
				}
				else
				{
					this.notMovingStopwatch = 0f;
				}
				this.previousPosition = position;
				this.UpdateTeslaCoil();
				this.UpdateBeetleGuardAllies();
				this.UpdateHelfire();
				int num2 = 0;
				if (this.inventory)
				{
					num2 = this.inventory.GetItemCount(ItemIndex.WarCryOnCombat);
				}
				if (num2 > 0)
				{
					this.warCryTimer -= Time.fixedDeltaTime;
					this.warCryReady = (this.warCryTimer <= 0f);
					if (this.warCryReady && (!this.outOfCombat && outOfCombat))
					{
						this.warCryTimer = 30f;
						this.ActivateWarCryAura(num2);
					}
				}
			}
			if (this.statsDirty)
			{
				this.RecalculateStats();
			}
			this.UpdateFireTrail();
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00009A8C File Offset: 0x00007C8C
		public void OnDeathStart()
		{
			base.enabled = false;
			if (this.master)
			{
				this.master.OnBodyDeath();
			}
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x00009AAD File Offset: 0x00007CAD
		public void OnTakeDamage(DamageInfo damageInfo)
		{
			this.outOfDangerStopwatch = 0f;
			if (this.master)
			{
				this.master.OnBodyDamaged(damageInfo);
			}
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x00009AD3 File Offset: 0x00007CD3
		public void OnSkillActivated(GenericSkill skill)
		{
			if (skill.isCombatSkill)
			{
				this.outOfCombatStopwatch = 0f;
			}
			if (!NetworkServer.active)
			{
				this.CallCmdOnSkillActivated((sbyte)this.skillLocator.FindSkillSlot(skill));
				return;
			}
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnDamageDealt(DamageReport damageReport)
		{
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00009B02 File Offset: 0x00007D02
		public void OnDestroy()
		{
			if (this.inventory)
			{
				this.inventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			if (this.master)
			{
				this.master.OnBodyDestroyed();
			}
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x000505C0 File Offset: 0x0004E7C0
		public float GetNormalizedThreatValue()
		{
			if (Run.instance)
			{
				return (this.master ? this.master.money : 0f) / Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 2f);
			}
			return 0f;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x00009B40 File Offset: 0x00007D40
		private void OnEnable()
		{
			CharacterBody.instancesList.Add(this);
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x00009B4D File Offset: 0x00007D4D
		private void OnDisable()
		{
			CharacterBody.instancesList.Remove(this);
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000C5D RID: 3165 RVA: 0x00009B5B File Offset: 0x00007D5B
		// (set) Token: 0x06000C5E RID: 3166 RVA: 0x00050618 File Offset: 0x0004E818
		public bool isSprinting
		{
			get
			{
				return this._isSprinting;
			}
			set
			{
				if (this._isSprinting != value)
				{
					this._isSprinting = value;
					this.RecalculateStats();
					if (value)
					{
						this.OnSprintStart();
					}
					else
					{
						this.OnSprintStop();
					}
					if (NetworkServer.active)
					{
						base.SetDirtyBit(32u);
						return;
					}
					if (base.hasAuthority)
					{
						this.CallCmdUpdateSprint(value);
					}
				}
			}
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x000025DA File Offset: 0x000007DA
		private void OnSprintStart()
		{
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x000025DA File Offset: 0x000007DA
		private void OnSprintStop()
		{
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00009B63 File Offset: 0x00007D63
		[Command]
		private void CmdUpdateSprint(bool newIsSprinting)
		{
			this.isSprinting = newIsSprinting;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x00009B6C File Offset: 0x00007D6C
		[Command]
		private void CmdOnSkillActivated(sbyte skillIndex)
		{
			this.OnSkillActivated(this.skillLocator.GetSkill((SkillSlot)skillIndex));
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x00009B80 File Offset: 0x00007D80
		// (set) Token: 0x06000C64 RID: 3172 RVA: 0x00009B88 File Offset: 0x00007D88
		public bool outOfCombat { get; private set; } = true;

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000C65 RID: 3173 RVA: 0x00009B91 File Offset: 0x00007D91
		// (set) Token: 0x06000C66 RID: 3174 RVA: 0x00009B99 File Offset: 0x00007D99
		public bool outOfDanger
		{
			get
			{
				return this._outOfDanger;
			}
			private set
			{
				if (this._outOfDanger == value)
				{
					return;
				}
				this._outOfDanger = value;
				this.OnOutOfDangerChanged();
			}
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00009BB2 File Offset: 0x00007DB2
		private void OnOutOfDangerChanged()
		{
			if (this.outOfDanger && this.healthComponent.shield != this.healthComponent.fullShield)
			{
				Util.PlaySound("Play_item_proc_personal_shield_recharge", base.gameObject);
			}
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x0005066C File Offset: 0x0004E86C
		[Server]
		private void OnOutOfCombatAndDangerServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::OnOutOfCombatAndDangerServer()' called on client");
				return;
			}
			if (this.inventory && this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat) > 0)
			{
				EffectData effectData = new EffectData();
				effectData.origin = this.corePosition;
				CharacterDirection component = base.GetComponent<CharacterDirection>();
				bool flag = false;
				if (component && component.moveVector != Vector3.zero)
				{
					effectData.rotation = Util.QuaternionSafeLookRotation(component.moveVector);
					flag = true;
				}
				if (!flag)
				{
					effectData.rotation = this.transform.rotation;
				}
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/SprintActivate"), effectData, true);
			}
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00009BE5 File Offset: 0x00007DE5
		[Server]
		public bool GetNotMoving()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.CharacterBody::GetNotMoving()' called on client");
				return false;
			}
			return this.notMovingStopwatch >= 2f;
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x00009C0D File Offset: 0x00007E0D
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x00009C15 File Offset: 0x00007E15
		public float experience { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x00009C1E File Offset: 0x00007E1E
		// (set) Token: 0x06000C6D RID: 3181 RVA: 0x00009C26 File Offset: 0x00007E26
		public float level { get; private set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x00009C2F File Offset: 0x00007E2F
		// (set) Token: 0x06000C6F RID: 3183 RVA: 0x00009C37 File Offset: 0x00007E37
		public float maxHealth { get; private set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00009C40 File Offset: 0x00007E40
		// (set) Token: 0x06000C71 RID: 3185 RVA: 0x00009C48 File Offset: 0x00007E48
		public float regen { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x00009C51 File Offset: 0x00007E51
		// (set) Token: 0x06000C73 RID: 3187 RVA: 0x00009C59 File Offset: 0x00007E59
		public float maxShield { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x00009C62 File Offset: 0x00007E62
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x00009C6A File Offset: 0x00007E6A
		public float moveSpeed { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000C76 RID: 3190 RVA: 0x00009C73 File Offset: 0x00007E73
		// (set) Token: 0x06000C77 RID: 3191 RVA: 0x00009C7B File Offset: 0x00007E7B
		public float acceleration { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x00009C84 File Offset: 0x00007E84
		// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00009C8C File Offset: 0x00007E8C
		public float jumpPower { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000C7A RID: 3194 RVA: 0x00009C95 File Offset: 0x00007E95
		// (set) Token: 0x06000C7B RID: 3195 RVA: 0x00009C9D File Offset: 0x00007E9D
		public int maxJumpCount { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000C7C RID: 3196 RVA: 0x00009CA6 File Offset: 0x00007EA6
		// (set) Token: 0x06000C7D RID: 3197 RVA: 0x00009CAE File Offset: 0x00007EAE
		public float maxJumpHeight { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000C7E RID: 3198 RVA: 0x00009CB7 File Offset: 0x00007EB7
		// (set) Token: 0x06000C7F RID: 3199 RVA: 0x00009CBF File Offset: 0x00007EBF
		public float damage { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x00009CC8 File Offset: 0x00007EC8
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x00009CD0 File Offset: 0x00007ED0
		public float attackSpeed { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x00009CD9 File Offset: 0x00007ED9
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x00009CE1 File Offset: 0x00007EE1
		public float crit { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x00009CEA File Offset: 0x00007EEA
		// (set) Token: 0x06000C85 RID: 3205 RVA: 0x00009CF2 File Offset: 0x00007EF2
		public float armor { get; private set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00009CFB File Offset: 0x00007EFB
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x00009D03 File Offset: 0x00007F03
		public float critHeal { get; private set; }

		// Token: 0x06000C88 RID: 3208 RVA: 0x00009D0C File Offset: 0x00007F0C
		public float CalcLunarDaggerPower()
		{
			if (this.inventory)
			{
				return Mathf.Pow(2f, (float)this.inventory.GetItemCount(ItemIndex.LunarDagger));
			}
			return 1f;
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x00050720 File Offset: 0x0004E920
		public void RecalculateStats()
		{
			this.experience = TeamManager.instance.GetTeamExperience(this.teamComponent.teamIndex);
			this.level = TeamManager.instance.GetTeamLevel(this.teamComponent.teamIndex);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int bonusStockFromBody = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			int num22 = 0;
			float num23 = 1f;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			uint num24 = 0u;
			if (this.inventory)
			{
				this.level += (float)this.inventory.GetItemCount(ItemIndex.LevelBonus);
				num = this.inventory.GetItemCount(ItemIndex.Infusion);
				num2 = this.inventory.GetItemCount(ItemIndex.HealWhileSafe);
				num3 = this.inventory.GetItemCount(ItemIndex.PersonalShield);
				num4 = this.inventory.GetItemCount(ItemIndex.Hoof);
				num5 = this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat);
				num6 = this.inventory.GetItemCount(ItemIndex.Feather);
				num7 = this.inventory.GetItemCount(ItemIndex.Syringe);
				num8 = this.inventory.GetItemCount(ItemIndex.CritGlasses);
				num9 = this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit);
				num10 = this.inventory.GetItemCount(ItemIndex.CooldownOnCrit);
				num11 = this.inventory.GetItemCount(ItemIndex.HealOnCrit);
				num12 = this.GetBuffCount(BuffIndex.BeetleJuice);
				num13 = this.inventory.GetItemCount(ItemIndex.ShieldOnly);
				num14 = this.inventory.GetItemCount(ItemIndex.AlienHead);
				num15 = this.inventory.GetItemCount(ItemIndex.Knurl);
				num16 = this.inventory.GetItemCount(ItemIndex.BoostHp);
				num17 = this.inventory.GetItemCount(ItemIndex.CritHeal);
				num18 = this.inventory.GetItemCount(ItemIndex.SprintBonus);
				bonusStockFromBody = this.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine);
				num19 = this.inventory.GetItemCount(ItemIndex.SprintArmor);
				num20 = this.inventory.GetItemCount(ItemIndex.UtilitySkillMagazine);
				num21 = this.inventory.GetItemCount(ItemIndex.HealthDecay);
				num23 = this.CalcLunarDaggerPower();
				equipmentIndex = this.inventory.currentEquipmentIndex;
				num24 = this.inventory.infusionBonus;
				num22 = this.inventory.GetItemCount(ItemIndex.DrizzlePlayerHelper);
			}
			float num25 = this.level - 1f;
			this.isElite = this.buffMask.containsEliteBuff;
			float maxHealth = this.maxHealth;
			float maxShield = this.maxShield;
			float num26 = this.baseMaxHealth + this.levelMaxHealth * num25;
			float num27 = 1f;
			num27 += (float)num16 * 0.1f;
			if (num > 0)
			{
				num26 += num24;
			}
			num26 += (float)num15 * 40f;
			num26 *= num27;
			num26 /= num23;
			this.maxHealth = num26;
			float num28 = this.baseRegen + this.levelRegen * num25;
			num28 *= 2.5f;
			if (this.outOfDanger && num2 > 0)
			{
				num28 *= 2.5f + (float)(num2 - 1) * 1.5f;
			}
			num28 += (float)num15 * 1.6f;
			if (num21 > 0)
			{
				num28 -= this.maxHealth / (float)num21;
			}
			this.regen = num28;
			float num29 = this.baseMaxShield + this.levelMaxShield * num25;
			num29 += (float)num3 * 25f;
			if (this.HasBuff(BuffIndex.EngiShield))
			{
				num29 += this.maxHealth * 1f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num29 += this.maxHealth * 0.5f;
			}
			if (num13 > 0)
			{
				num29 += this.maxHealth * (1.5f + (float)(num13 - 1) * 0.25f);
				this.maxHealth = 1f;
			}
			if (this.buffMask.HasBuff(BuffIndex.AffixBlue))
			{
				float num30 = this.maxHealth * 0.5f;
				this.maxHealth -= num30;
				num29 += this.maxHealth;
			}
			this.maxShield = num29;
			float num31 = this.baseMoveSpeed + this.levelMoveSpeed * num25;
			float num32 = 1f;
			if (Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Spirit))
			{
				float num33 = 1f;
				if (this.healthComponent)
				{
					num33 = this.healthComponent.combinedHealthFraction;
				}
				num32 += 1f - num33;
			}
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num31 += 2f;
			}
			if (this.isSprinting)
			{
				num31 *= this.sprintingSpeedMultiplier;
			}
			if (this.outOfCombat && this.outOfDanger && num5 > 0)
			{
				num32 += (float)num5 * 0.3f;
			}
			num32 += (float)num4 * 0.14f;
			if (this.isSprinting && num18 > 0)
			{
				num32 += (0.1f + 0.2f * (float)num18) / this.sprintingSpeedMultiplier;
			}
			if (this.HasBuff(BuffIndex.BugWings))
			{
				num32 += 0.2f;
			}
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num32 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.EnrageAncientWisp))
			{
				num32 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.CloakSpeed))
			{
				num32 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.TempestSpeed))
			{
				num32 += 1f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num32 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num32 += 0.3f;
			}
			float num34 = 1f;
			if (this.HasBuff(BuffIndex.Slow50))
			{
				num34 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow60))
			{
				num34 += 0.6f;
			}
			if (this.HasBuff(BuffIndex.Slow80))
			{
				num34 += 0.8f;
			}
			if (this.HasBuff(BuffIndex.ClayGoo))
			{
				num34 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow30))
			{
				num34 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.Cripple))
			{
				num34 += 1f;
			}
			num31 *= num32 / num34;
			if (num12 > 0)
			{
				num31 *= 1f - 0.05f * (float)num12;
			}
			this.moveSpeed = num31;
			this.acceleration = this.moveSpeed / this.baseMoveSpeed * this.baseAcceleration;
			float jumpPower = this.baseJumpPower + this.levelJumpPower * num25;
			this.jumpPower = jumpPower;
			this.maxJumpHeight = Trajectory.CalculateApex(this.jumpPower);
			this.maxJumpCount = this.baseJumpCount + num6;
			float num35 = this.baseDamage + this.levelDamage * num25;
			float num36 = 1f;
			int num37 = this.inventory ? this.inventory.GetItemCount(ItemIndex.BoostDamage) : 0;
			if (num37 > 0)
			{
				num36 += (float)num37 * 0.1f;
			}
			if (num12 > 0)
			{
				num36 -= 0.05f * (float)num12;
			}
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num36 += 1f;
			}
			num36 += num23 - 1f;
			num35 *= num36;
			this.damage = num35;
			float num38 = this.baseAttackSpeed + this.levelAttackSpeed * num25;
			float num39 = 1f;
			num39 += (float)num7 * 0.15f;
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num39 += 0.5f;
			}
			num39 += (float)this.buffs[2] * 0.12f;
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num39 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.EnrageAncientWisp))
			{
				num39 += 2f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num39 += 1f;
			}
			num38 *= num39;
			if (num12 > 0)
			{
				num38 *= 1f - 0.05f * (float)num12;
			}
			this.attackSpeed = num38;
			float num40 = this.baseCrit + this.levelCrit * num25;
			num40 += (float)num8 * 10f;
			if (num9 > 0)
			{
				num40 += 5f;
			}
			if (num10 > 0)
			{
				num40 += 5f;
			}
			if (num11 > 0)
			{
				num40 += 5f;
			}
			if (num17 > 0)
			{
				num40 += 5f;
			}
			if (this.HasBuff(BuffIndex.FullCrit))
			{
				num40 += 100f;
			}
			this.crit = num40;
			this.armor = this.baseArmor + this.levelArmor * num25 + (this.HasBuff(BuffIndex.ArmorBoost) ? 200f : 0f);
			this.armor += (float)num22 * 70f;
			if (this.HasBuff(BuffIndex.Cripple))
			{
				this.armor -= 20f;
			}
			if (this.isSprinting && num19 > 0)
			{
				this.armor += (float)(num19 * 30);
			}
			float num41 = 1f;
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num41 *= 0.25f;
			}
			for (int i = 0; i < num14; i++)
			{
				num41 *= 0.75f;
			}
			if (this.HasBuff(BuffIndex.NoCooldowns))
			{
				num41 = 0f;
			}
			if (this.skillLocator.primary)
			{
				this.skillLocator.primary.cooldownScale = num41;
			}
			if (this.skillLocator.secondary)
			{
				this.skillLocator.secondary.cooldownScale = num41;
				this.skillLocator.secondary.SetBonusStockFromBody(bonusStockFromBody);
			}
			if (this.skillLocator.utility)
			{
				float num42 = num41;
				if (num20 > 0)
				{
					num42 *= 0.6666667f;
				}
				this.skillLocator.utility.cooldownScale = num42;
				this.skillLocator.utility.SetBonusStockFromBody(num20 * 2);
			}
			if (this.skillLocator.special)
			{
				this.skillLocator.special.cooldownScale = num41;
			}
			this.critHeal = 0f;
			if (num17 > 0)
			{
				float crit = this.crit;
				this.crit /= (float)(num17 + 1);
				this.critHeal = crit - this.crit;
			}
			if (NetworkServer.active)
			{
				float num43 = this.maxHealth - maxHealth;
				float num44 = this.maxShield - maxShield;
				if (num43 > 0f)
				{
					this.healthComponent.Heal(num43, default(ProcChainMask), false);
				}
				else if (this.healthComponent.health > this.maxHealth)
				{
					this.healthComponent.Networkhealth = this.maxHealth;
				}
				if (num44 > 0f)
				{
					this.healthComponent.RechargeShield(num44);
				}
			}
			this.statsDirty = false;
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x00009D39 File Offset: 0x00007F39
		public void OnLevelChanged()
		{
			this.statsDirty = true;
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x00009D42 File Offset: 0x00007F42
		public void SetAimTimer(float duration)
		{
			this.aimTimer = duration;
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00009D4B File Offset: 0x00007F4B
		public bool shouldAim
		{
			get
			{
				return this.aimTimer > 0f && !this.isSprinting;
			}
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x00051154 File Offset: 0x0004F354
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			if ((b & 1) != 0)
			{
				NetworkInstanceId c = reader.ReadNetworkId();
				if (c != this.masterObjectId)
				{
					this.masterObjectId = c;
					this.statsDirty = true;
				}
			}
			if ((b & 2) != 0)
			{
				BuffMask a = reader.ReadBuffMask();
				if (a != this.buffMask)
				{
					BuffMask oldBuffMask = this.buffMask;
					this.buffMask = a;
					this.statsDirty = true;
					this.OnClientBuffsChanged(oldBuffMask);
				}
			}
			if ((b & 4) != 0)
			{
				byte b2 = reader.ReadByte();
				if (this.buffs[2] != (int)b2)
				{
					this.buffs[2] = (int)b2;
					this.statsDirty = true;
				}
			}
			if ((b & 8) != 0)
			{
				bool flag = reader.ReadBoolean();
				if (!base.hasAuthority && flag != this.outOfCombat)
				{
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if ((b & 16) != 0)
			{
				bool flag2 = reader.ReadBoolean();
				if (flag2 != this.outOfDanger)
				{
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
			}
			if ((b & 32) != 0)
			{
				bool flag3 = reader.ReadBoolean();
				if (flag3 != this.isSprinting && !base.hasAuthority)
				{
					this.statsDirty = true;
					this.isSprinting = flag3;
				}
			}
			if ((b & 64) != 0)
			{
				byte b3 = reader.ReadByte();
				if (this.buffs[4] != (int)b3)
				{
					this.buffs[4] = (int)b3;
					this.statsDirty = true;
				}
			}
			if ((b & 128) != 0)
			{
				byte b4 = reader.ReadByte();
				if (this.buffs[18] != (int)b4)
				{
					this.buffs[18] = (int)b4;
					this.statsDirty = true;
				}
			}
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x000512CC File Offset: 0x0004F4CC
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 255u;
			}
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			bool flag3 = (num & 4u) > 0u;
			bool flag4 = (num & 8u) > 0u;
			bool flag5 = (num & 16u) > 0u;
			bool flag6 = (num & 32u) > 0u;
			bool flag7 = (num & 64u) > 0u;
			bool flag8 = (num & 128u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.masterObjectId);
			}
			if (flag2)
			{
				writer.WriteBuffMask(this.buffMask);
			}
			if (flag3)
			{
				writer.Write((byte)this.buffs[2]);
			}
			if (flag4)
			{
				writer.Write(this.outOfCombat);
			}
			if (flag5)
			{
				writer.Write(this.outOfDanger);
			}
			if (flag6)
			{
				writer.Write(this.isSprinting);
			}
			if (flag7)
			{
				writer.Write((byte)this.buffs[4]);
			}
			if (flag8)
			{
				writer.Write((byte)this.buffs[18]);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x000513C4 File Offset: 0x0004F5C4
		public T AddItemBehavior<T>(int stack) where T : CharacterBody.ItemBehavior
		{
			T t = base.GetComponent<T>();
			if (stack > 0)
			{
				if (!t)
				{
					t = base.gameObject.AddComponent<T>();
					t.body = this;
				}
				t.stack = stack;
				return t;
			}
			if (t)
			{
				UnityEngine.Object.Destroy(t);
			}
			return default(T);
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00009D65 File Offset: 0x00007F65
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x00009D6D File Offset: 0x00007F6D
		public bool warCryReady
		{
			get
			{
				return this._warCryReady;
			}
			private set
			{
				if (this._warCryReady != value)
				{
					this._warCryReady = value;
					if (NetworkServer.active)
					{
						this.CallRpcSyncWarCryReady(value);
					}
				}
			}
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x00051430 File Offset: 0x0004F630
		[Server]
		private void ActivateWarCryAura(int stacks)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ActivateWarCryAura(System.Int32)' called on client");
				return;
			}
			if (this.warCryAuraController)
			{
				UnityEngine.Object.Destroy(this.warCryAuraController);
			}
			this.warCryAuraController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarCryAura"), this.transform.position, this.transform.rotation, this.transform);
			this.warCryAuraController.GetComponent<TeamFilter>().teamIndex = this.teamComponent.teamIndex;
			BuffWard component = this.warCryAuraController.GetComponent<BuffWard>();
			component.expireDuration = 2f + 4f * (float)stacks;
			component.Networkradius = 8f + 4f * (float)stacks;
			this.warCryAuraController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x00009D8D File Offset: 0x00007F8D
		[ClientRpc]
		private void RpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				this.warCryReady = value;
			}
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x00051500 File Offset: 0x0004F700
		private void OnKilledOther(DamageReport damageReport)
		{
			this.killCount++;
			this.AddMultiKill(1);
			CharacterBody.IcicleItemBehavior component = base.GetComponent<CharacterBody.IcicleItemBehavior>();
			if (component)
			{
				component.OnOwnerKillOther();
			}
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x00051538 File Offset: 0x0004F738
		[Server]
		private void UpdateTeslaCoil()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateTeslaCoil()' called on client");
				return;
			}
			if (this.inventory)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.ShockNearby);
				if (itemCount > 0)
				{
					this.teslaBuffRollTimer += Time.fixedDeltaTime;
					if (this.teslaBuffRollTimer >= 10f)
					{
						this.teslaBuffRollTimer = 0f;
						this.teslaCrit = Util.CheckRoll(this.crit, this.master);
						if (this.HasBuff(BuffIndex.TeslaField))
						{
							this.AddBuff(BuffIndex.TeslaField);
						}
						else
						{
							this.RemoveBuff(BuffIndex.TeslaField);
						}
					}
					if (this.HasBuff(BuffIndex.TeslaField))
					{
						this.teslaFireTimer += Time.fixedDeltaTime;
						this.teslaResetListTimer += Time.fixedDeltaTime;
						if (this.teslaFireTimer >= 0.0833333358f)
						{
							this.teslaFireTimer = 0f;
							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.origin = this.corePosition;
							lightningOrb.damageValue = this.damage * 2f;
							lightningOrb.isCrit = this.teslaCrit;
							lightningOrb.bouncesRemaining = 2 * itemCount;
							lightningOrb.teamIndex = this.teamComponent.teamIndex;
							lightningOrb.attacker = base.gameObject;
							lightningOrb.procCoefficient = 0.3f;
							lightningOrb.bouncedObjects = this.previousTeslaTargetList;
							lightningOrb.lightningType = LightningOrb.LightningType.Tesla;
							lightningOrb.damageColorIndex = DamageColorIndex.Item;
							lightningOrb.range = 35f;
							HurtBox hurtBox = lightningOrb.PickNextTarget(this.transform.position);
							if (hurtBox)
							{
								this.previousTeslaTargetList.Add(hurtBox.healthComponent);
								lightningOrb.target = hurtBox;
								OrbManager.instance.AddOrb(lightningOrb);
							}
						}
						if (this.teslaResetListTimer >= this.teslaResetListInterval)
						{
							this.teslaResetListTimer -= this.teslaResetListInterval;
							this.previousTeslaTargetList.Clear();
						}
					}
				}
			}
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x00009D9D File Offset: 0x00007F9D
		public void AddHelfireDuration(float duration)
		{
			this.helfireLifetime = duration;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x00051718 File Offset: 0x0004F918
		[Server]
		private void UpdateHelfire()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateHelfire()' called on client");
				return;
			}
			this.helfireLifetime -= Time.fixedDeltaTime;
			bool flag = false;
			if (this.inventory)
			{
				flag = (this.inventory.GetItemCount(ItemIndex.BurnNearby) > 0 || this.helfireLifetime > 0f);
			}
			if (this.helfireController != flag)
			{
				if (flag)
				{
					this.helfireController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HelfireController")).GetComponent<HelfireController>();
					this.helfireController.networkedBodyAttachment.AttachToGameObjectAndSpawn(base.gameObject);
					return;
				}
				UnityEngine.Object.Destroy(this.helfireController.gameObject);
				this.helfireController = null;
			}
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x000517D8 File Offset: 0x0004F9D8
		private void UpdateFireTrail()
		{
			bool flag = this.HasBuff(BuffIndex.AffixRed);
			if (flag != this.fireTrail)
			{
				if (flag)
				{
					this.fireTrail = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
					this.fireTrail.transform.position = this.footPosition;
					this.fireTrail.owner = base.gameObject;
					this.fireTrail.radius *= this.radius;
				}
				else
				{
					UnityEngine.Object.Destroy(this.fireTrail.gameObject);
					this.fireTrail = null;
				}
			}
			if (this.fireTrail)
			{
				this.fireTrail.damagePerSecond = this.damage * 1.5f;
			}
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0005189C File Offset: 0x0004FA9C
		private void UpdateBeetleGuardAllies()
		{
			if (NetworkServer.active)
			{
				int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.BeetleGland) : 0;
				if (num > 0 && this.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly) < num)
				{
					this.guardResummonCooldown -= Time.fixedDeltaTime;
					if (this.guardResummonCooldown <= 0f)
					{
						this.guardResummonCooldown = 30f;
						GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Approximate,
							minDistance = 3f,
							maxDistance = 40f,
							spawnOnTarget = this.transform
						}, RoR2Application.rng);
						if (gameObject)
						{
							CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
							AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
							BaseAI component3 = gameObject.GetComponent<BaseAI>();
							if (component)
							{
								component.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
								component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
								component.inventory.GiveItem(ItemIndex.BoostHp, 10);
								GameObject bodyObject = component.GetBodyObject();
								if (bodyObject)
								{
									Deployable component4 = bodyObject.GetComponent<Deployable>();
									this.master.AddDeployable(component4, DeployableSlot.BeetleGuardAlly);
								}
							}
							if (component2)
							{
								component2.ownerMaster = this.master;
							}
							if (component3)
							{
								component3.leader.gameObject = base.gameObject;
							}
						}
					}
				}
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x00009DA6 File Offset: 0x00007FA6
		private float bestFitRadius
		{
			get
			{
				return Mathf.Max(this.radius, this.characterMotor ? this.characterMotor.capsuleHeight : 1f);
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x00051A14 File Offset: 0x0004FC14
		private void UpdateAllTemporaryVisualEffects()
		{
			this.UpdateSingleTemporaryVisualEffect(ref this.engiShieldTempEffect, "Prefabs/TemporaryVisualEffects/EngiShield", this.bestFitRadius, this.healthComponent.shield > 0f && this.HasBuff(BuffIndex.EngiShield));
			this.UpdateSingleTemporaryVisualEffect(ref this.bucklerShieldTempEffect, "Prefabs/TemporaryVisualEffects/BucklerDefense", this.radius, this.isSprinting && this.inventory.GetItemCount(ItemIndex.SprintArmor) > 0);
			this.UpdateSingleTemporaryVisualEffect(ref this.slowDownTimeTempEffect, "Prefabs/TemporaryVisualEffects/SlowDownTime", this.radius, this.HasBuff(BuffIndex.Slow60));
			this.UpdateSingleTemporaryVisualEffect(ref this.crippleEffect, "Prefabs/TemporaryVisualEffects/CrippleEffect", this.radius, this.HasBuff(BuffIndex.Cripple));
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x00051AC8 File Offset: 0x0004FCC8
		private void UpdateSingleTemporaryVisualEffect(ref TemporaryVisualEffect tempEffect, string resourceString, float effectRadius, bool active)
		{
			bool flag = tempEffect != null;
			if (flag != active)
			{
				if (active)
				{
					if (!flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(resourceString), this.corePosition, Quaternion.identity);
						tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
						tempEffect.parentTransform = this.coreTransform;
						tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
						tempEffect.healthComponent = this.healthComponent;
						tempEffect.radius = effectRadius;
						return;
					}
				}
				else if (tempEffect)
				{
					tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
				}
			}
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x00051B48 File Offset: 0x0004FD48
		public VisibilityLevel GetVisibilityLevel(CharacterBody observer)
		{
			if (!this.HasBuff(BuffIndex.Cloak))
			{
				return VisibilityLevel.Visible;
			}
			if (!observer)
			{
				return VisibilityLevel.Revealed;
			}
			TeamIndex teamIndex = this.teamComponent ? this.teamComponent.teamIndex : TeamIndex.Neutral;
			TeamIndex teamIndex2 = observer.teamComponent ? observer.teamComponent.teamIndex : TeamIndex.Neutral;
			if (teamIndex != teamIndex2)
			{
				return VisibilityLevel.Cloaked;
			}
			return VisibilityLevel.Revealed;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x00009DD2 File Offset: 0x00007FD2
		public void AddSpreadBloom(float value)
		{
			this.spreadBloomInternal = Mathf.Min(this.spreadBloomInternal + value, 1f);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00009DEC File Offset: 0x00007FEC
		public void SetSpreadBloom(float value, bool canOnlyIncreaseBloom = true)
		{
			if (canOnlyIncreaseBloom)
			{
				this.spreadBloomInternal = Mathf.Clamp(value, this.spreadBloomInternal, 1f);
				return;
			}
			this.spreadBloomInternal = Mathf.Min(value, 1f);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00051BA8 File Offset: 0x0004FDA8
		private void UpdateSpreadBloom(float dt)
		{
			float num = 1f / this.spreadBloomDecayTime;
			this.spreadBloomInternal = Mathf.Max(this.spreadBloomInternal - num * dt, 0f);
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000CA1 RID: 3233 RVA: 0x00009E1A File Offset: 0x0000801A
		public float spreadBloomAngle
		{
			get
			{
				return this.spreadBloomCurve.Evaluate(this.spreadBloomInternal);
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00051BDC File Offset: 0x0004FDDC
		[Client]
		public void SendConstructTurret(CharacterBody builder, Vector3 position, Quaternion rotation)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::SendConstructTurret(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on server");
				return;
			}
			CharacterBody.ConstructTurretMessage constructTurretMessage = new CharacterBody.ConstructTurretMessage();
			constructTurretMessage.builder = builder.gameObject;
			constructTurretMessage.position = position;
			constructTurretMessage.rotation = rotation;
			ClientScene.readyConnection.Send(62, constructTurretMessage);
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x00051C2C File Offset: 0x0004FE2C
		[NetworkMessageHandler(msgType = 62, server = true)]
		private static void HandleConstructTurret(NetworkMessage netMsg)
		{
			CharacterBody.ConstructTurretMessage constructTurretMessage = netMsg.ReadMessage<CharacterBody.ConstructTurretMessage>();
			if (constructTurretMessage.builder)
			{
				CharacterBody component = constructTurretMessage.builder.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						GameObject original = MasterCatalog.FindMasterPrefab("EngiTurretMaster");
						GameObject bodyPrefab = BodyCatalog.FindBodyPrefab("EngiTurretBody");
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, constructTurretMessage.position, constructTurretMessage.rotation);
						CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
						component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
						Inventory component3 = gameObject.GetComponent<Inventory>();
						component3.CopyItemsFrom(master.inventory);
						component3.ResetItem(ItemIndex.WardOnLevel);
						component3.ResetItem(ItemIndex.BeetleGland);
						component3.ResetItem(ItemIndex.CrippleWardOnLevel);
						NetworkServer.Spawn(gameObject);
						Deployable deployable = gameObject.AddComponent<Deployable>();
						deployable.onUndeploy = new UnityEvent();
						deployable.onUndeploy.AddListener(new UnityAction(component2.TrueKill));
						master.AddDeployable(deployable, DeployableSlot.EngiTurret);
						component2.SpawnBody(bodyPrefab, constructTurretMessage.position, constructTurretMessage.rotation);
					}
				}
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x00009E2D File Offset: 0x0000802D
		// (set) Token: 0x06000CA5 RID: 3237 RVA: 0x00009E35 File Offset: 0x00008035
		public int multiKillCount { get; private set; }

		// Token: 0x06000CA6 RID: 3238 RVA: 0x00051D34 File Offset: 0x0004FF34
		[Server]
		public void AddMultiKill(int kills)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddMultiKill(System.Int32)' called on client");
				return;
			}
			this.multiKillTimer = 1f;
			this.multiKillCount += kills;
			int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.WarCryOnMultiKill) : 0;
			if (num > 0 && this.multiKillCount >= 4)
			{
				this.AddTimedBuff(BuffIndex.WarCryBuff, 2f + 4f * (float)num);
			}
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00051DB0 File Offset: 0x0004FFB0
		[Server]
		private void UpdateMultiKill(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateMultiKill(System.Single)' called on client");
				return;
			}
			this.multiKillTimer -= deltaTime;
			if (this.multiKillTimer <= 0f)
			{
				this.multiKillTimer = 0f;
				this.multiKillCount = 0;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x00009E3E File Offset: 0x0000803E
		public Vector3 corePosition
		{
			get
			{
				return this.coreTransform.position;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000CA9 RID: 3241 RVA: 0x00051E00 File Offset: 0x00050000
		public Vector3 footPosition
		{
			get
			{
				Vector3 position = this.transform.position;
				if (this.characterMotor)
				{
					position.y -= this.characterMotor.capsuleHeight * 0.5f;
				}
				return position;
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000CAA RID: 3242 RVA: 0x00009E4B File Offset: 0x0000804B
		// (set) Token: 0x06000CAB RID: 3243 RVA: 0x00009E53 File Offset: 0x00008053
		public float radius { get; private set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x00009E5C File Offset: 0x0000805C
		public Vector3 aimOrigin
		{
			get
			{
				if (!this.aimOriginTransform)
				{
					return this.corePosition;
				}
				return this.aimOriginTransform.position;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000CAD RID: 3245 RVA: 0x00009E7D File Offset: 0x0000807D
		// (set) Token: 0x06000CAE RID: 3246 RVA: 0x00009E85 File Offset: 0x00008085
		public bool isElite { get; private set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000CAF RID: 3247 RVA: 0x00009E8E File Offset: 0x0000808E
		public bool isBoss
		{
			get
			{
				return this.master && this.master.isBoss;
			}
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x00009EAA File Offset: 0x000080AA
		[ClientRpc]
		public void RpcBark()
		{
			if (this.sfxLocator)
			{
				Util.PlaySound(this.sfxLocator.barkSound, base.gameObject);
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x00051EB0 File Offset: 0x000500B0
		static CharacterBody()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdUpdateSprint, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdUpdateSprint));
			CharacterBody.kCmdCmdOnSkillActivated = 384138986;
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdOnSkillActivated, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdOnSkillActivated));
			CharacterBody.kRpcRpcSyncWarCryReady = 1893254821;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcSyncWarCryReady, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcSyncWarCryReady));
			CharacterBody.kRpcRpcBark = -76716871;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcBark, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcBark));
			NetworkCRC.RegisterBehaviour("CharacterBody", 0);
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x00009ED0 File Offset: 0x000080D0
		protected static void InvokeCmdCmdUpdateSprint(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateSprint called on client.");
				return;
			}
			((CharacterBody)obj).CmdUpdateSprint(reader.ReadBoolean());
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x00009EF9 File Offset: 0x000080F9
		protected static void InvokeCmdCmdOnSkillActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdOnSkillActivated called on client.");
				return;
			}
			((CharacterBody)obj).CmdOnSkillActivated((sbyte)reader.ReadPackedUInt32());
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x00051F90 File Offset: 0x00050190
		public void CallCmdUpdateSprint(bool newIsSprinting)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdUpdateSprint called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdUpdateSprint(newIsSprinting);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdUpdateSprint);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(newIsSprinting);
			base.SendCommandInternal(networkWriter, 0, "CmdUpdateSprint");
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0005201C File Offset: 0x0005021C
		public void CallCmdOnSkillActivated(sbyte skillIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdOnSkillActivated called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdOnSkillActivated(skillIndex);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdOnSkillActivated);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32((uint)skillIndex);
			base.SendCommandInternal(networkWriter, 0, "CmdOnSkillActivated");
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x00009F22 File Offset: 0x00008122
		protected static void InvokeRpcRpcSyncWarCryReady(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSyncWarCryReady called on server.");
				return;
			}
			((CharacterBody)obj).RpcSyncWarCryReady(reader.ReadBoolean());
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00009F4B File Offset: 0x0000814B
		protected static void InvokeRpcRpcBark(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcBark called on server.");
				return;
			}
			((CharacterBody)obj).RpcBark();
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x000520A8 File Offset: 0x000502A8
		public void CallRpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSyncWarCryReady called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcSyncWarCryReady);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(value);
			this.SendRPCInternal(networkWriter, 0, "RpcSyncWarCryReady");
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0005211C File Offset: 0x0005031C
		public void CallRpcBark()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcBark called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcBark);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcBark");
		}

		// Token: 0x04001050 RID: 4176
		[Tooltip("The language token to use as the base name of this character.")]
		public string baseNameToken;

		// Token: 0x04001051 RID: 4177
		public string subtitleNameToken;

		// Token: 0x04001052 RID: 4178
		private int[] buffs = new int[31];

		// Token: 0x04001053 RID: 4179
		private List<CharacterBody.TimedBuff> timedBuffs = new List<CharacterBody.TimedBuff>();

		// Token: 0x04001054 RID: 4180
		private BuffMask buffMask;

		// Token: 0x04001055 RID: 4181
		private GameObject warCryEffectInstance;

		// Token: 0x04001056 RID: 4182
		[EnumMask(typeof(CharacterBody.BodyFlags))]
		public CharacterBody.BodyFlags bodyFlags;

		// Token: 0x04001057 RID: 4183
		private NetworkInstanceId masterObjectId;

		// Token: 0x04001058 RID: 4184
		private GameObject _masterObject;

		// Token: 0x04001059 RID: 4185
		private CharacterMaster _master;

		// Token: 0x0400105B RID: 4187
		private bool linkedToMaster;

		// Token: 0x0400105D RID: 4189
		private bool disablingHurtBoxes;

		// Token: 0x0400105E RID: 4190
		private EquipmentIndex previousEquipmentIndex;

		// Token: 0x04001060 RID: 4192
		private new Transform transform;

		// Token: 0x04001065 RID: 4197
		private SkillLocator skillLocator;

		// Token: 0x04001066 RID: 4198
		private SfxLocator sfxLocator;

		// Token: 0x0400106B RID: 4203
		private static List<CharacterBody> instancesList = new List<CharacterBody>();

		// Token: 0x0400106C RID: 4204
		public static readonly ReadOnlyCollection<CharacterBody> readOnlyInstancesList = new ReadOnlyCollection<CharacterBody>(CharacterBody.instancesList);

		// Token: 0x0400106D RID: 4205
		private bool _isSprinting;

		// Token: 0x0400106E RID: 4206
		private float sprintingSpeedMultiplier = 1.45f;

		// Token: 0x0400106F RID: 4207
		private const float outOfCombatDelay = 5f;

		// Token: 0x04001070 RID: 4208
		private const float outOfDangerDelay = 7f;

		// Token: 0x04001071 RID: 4209
		private float outOfCombatStopwatch;

		// Token: 0x04001072 RID: 4210
		private float outOfDangerStopwatch;

		// Token: 0x04001074 RID: 4212
		private bool _outOfDanger = true;

		// Token: 0x04001075 RID: 4213
		private Vector3 previousPosition;

		// Token: 0x04001076 RID: 4214
		private const float notMovingWait = 2f;

		// Token: 0x04001077 RID: 4215
		private float notMovingStopwatch;

		// Token: 0x04001078 RID: 4216
		public bool rootMotionInMainState;

		// Token: 0x04001079 RID: 4217
		public float mainRootSpeed;

		// Token: 0x0400107A RID: 4218
		public float baseMaxHealth;

		// Token: 0x0400107B RID: 4219
		public float baseRegen;

		// Token: 0x0400107C RID: 4220
		public float baseMaxShield;

		// Token: 0x0400107D RID: 4221
		public float baseMoveSpeed;

		// Token: 0x0400107E RID: 4222
		public float baseAcceleration;

		// Token: 0x0400107F RID: 4223
		public float baseJumpPower;

		// Token: 0x04001080 RID: 4224
		public float baseDamage;

		// Token: 0x04001081 RID: 4225
		public float baseAttackSpeed;

		// Token: 0x04001082 RID: 4226
		public float baseCrit;

		// Token: 0x04001083 RID: 4227
		public float baseArmor;

		// Token: 0x04001084 RID: 4228
		public int baseJumpCount = 1;

		// Token: 0x04001085 RID: 4229
		public bool autoCalculateLevelStats;

		// Token: 0x04001086 RID: 4230
		public float levelMaxHealth;

		// Token: 0x04001087 RID: 4231
		public float levelRegen;

		// Token: 0x04001088 RID: 4232
		public float levelMaxShield;

		// Token: 0x04001089 RID: 4233
		public float levelMoveSpeed;

		// Token: 0x0400108A RID: 4234
		public float levelJumpPower;

		// Token: 0x0400108B RID: 4235
		public float levelDamage;

		// Token: 0x0400108C RID: 4236
		public float levelAttackSpeed;

		// Token: 0x0400108D RID: 4237
		public float levelCrit;

		// Token: 0x0400108E RID: 4238
		public float levelArmor;

		// Token: 0x0400109E RID: 4254
		private bool statsDirty;

		// Token: 0x0400109F RID: 4255
		private float aimTimer;

		// Token: 0x040010A0 RID: 4256
		private const uint masterDirtyBit = 1u;

		// Token: 0x040010A1 RID: 4257
		private const uint buffMaskBit = 2u;

		// Token: 0x040010A2 RID: 4258
		private const uint attackSpeedOnCritBuffBit = 4u;

		// Token: 0x040010A3 RID: 4259
		private const uint outOfCombatBit = 8u;

		// Token: 0x040010A4 RID: 4260
		private const uint outOfDangerBit = 16u;

		// Token: 0x040010A5 RID: 4261
		private const uint sprintingBit = 32u;

		// Token: 0x040010A6 RID: 4262
		private const uint onFireBuffBit = 64u;

		// Token: 0x040010A7 RID: 4263
		private const uint beetleJuiceBuffBit = 128u;

		// Token: 0x040010A8 RID: 4264
		private GameObject warCryAuraController;

		// Token: 0x040010A9 RID: 4265
		private float warCryTimer;

		// Token: 0x040010AA RID: 4266
		private const float warCryChargeDuration = 30f;

		// Token: 0x040010AB RID: 4267
		private bool _warCryReady;

		// Token: 0x040010AC RID: 4268
		[HideInInspector]
		public int killCount;

		// Token: 0x040010AD RID: 4269
		private float teslaBuffRollTimer;

		// Token: 0x040010AE RID: 4270
		private const float teslaRollInterval = 10f;

		// Token: 0x040010AF RID: 4271
		private float teslaFireTimer;

		// Token: 0x040010B0 RID: 4272
		private float teslaResetListTimer;

		// Token: 0x040010B1 RID: 4273
		private float teslaResetListInterval = 0.5f;

		// Token: 0x040010B2 RID: 4274
		private const float teslaFireInterval = 0.0833333358f;

		// Token: 0x040010B3 RID: 4275
		private bool teslaCrit;

		// Token: 0x040010B4 RID: 4276
		private List<HealthComponent> previousTeslaTargetList = new List<HealthComponent>();

		// Token: 0x040010B5 RID: 4277
		private HelfireController helfireController;

		// Token: 0x040010B6 RID: 4278
		private float helfireLifetime;

		// Token: 0x040010B7 RID: 4279
		private DamageTrail fireTrail;

		// Token: 0x040010B8 RID: 4280
		public bool wasLucky;

		// Token: 0x040010B9 RID: 4281
		private const float timeBetweenGuardResummons = 30f;

		// Token: 0x040010BA RID: 4282
		private float guardResummonCooldown;

		// Token: 0x040010BB RID: 4283
		private TemporaryVisualEffect engiShieldTempEffect;

		// Token: 0x040010BC RID: 4284
		private TemporaryVisualEffect bucklerShieldTempEffect;

		// Token: 0x040010BD RID: 4285
		private TemporaryVisualEffect slowDownTimeTempEffect;

		// Token: 0x040010BE RID: 4286
		private TemporaryVisualEffect crippleEffect;

		// Token: 0x040010BF RID: 4287
		[Tooltip("How long it takes for spread bloom to reset from full.")]
		public float spreadBloomDecayTime = 0.45f;

		// Token: 0x040010C0 RID: 4288
		[Tooltip("The spread bloom interpretation curve.")]
		public AnimationCurve spreadBloomCurve;

		// Token: 0x040010C1 RID: 4289
		private float spreadBloomInternal;

		// Token: 0x040010C2 RID: 4290
		[Tooltip("The crosshair prefab used for this body.")]
		public GameObject crosshairPrefab;

		// Token: 0x040010C3 RID: 4291
		[HideInInspector]
		public bool hideCrosshair;

		// Token: 0x040010C4 RID: 4292
		private const float multiKillMaxInterval = 1f;

		// Token: 0x040010C5 RID: 4293
		private float multiKillTimer;

		// Token: 0x040010C7 RID: 4295
		private const int multiKillThresholdForWarcry = 4;

		// Token: 0x040010C9 RID: 4297
		[Tooltip("The child transform to be used as the aiming origin.")]
		public Transform aimOriginTransform;

		// Token: 0x040010CA RID: 4298
		[Tooltip("The hull size to use when pathfinding for this object.")]
		public HullClassification hullClassification;

		// Token: 0x040010CB RID: 4299
		[Tooltip("The icon displayed for ally healthbars")]
		public Texture portraitIcon;

		// Token: 0x040010CC RID: 4300
		[FormerlySerializedAs("isBoss")]
		[Tooltip("Whether or not this is a boss for dropping items on death.")]
		public bool isChampion;

		// Token: 0x040010CE RID: 4302
		private static int kCmdCmdUpdateSprint = -1006016914;

		// Token: 0x040010CF RID: 4303
		private static int kCmdCmdOnSkillActivated;

		// Token: 0x040010D0 RID: 4304
		private static int kRpcRpcSyncWarCryReady;

		// Token: 0x040010D1 RID: 4305
		private static int kRpcRpcBark;

		// Token: 0x02000282 RID: 642
		private class TimedBuff
		{
			// Token: 0x040010D2 RID: 4306
			public BuffIndex buffIndex;

			// Token: 0x040010D3 RID: 4307
			public float timer;
		}

		// Token: 0x02000283 RID: 643
		[Flags]
		public enum BodyFlags : byte
		{
			// Token: 0x040010D5 RID: 4309
			None = 0,
			// Token: 0x040010D6 RID: 4310
			IgnoreFallDamage = 1,
			// Token: 0x040010D7 RID: 4311
			Mechanical = 2,
			// Token: 0x040010D8 RID: 4312
			Masterless = 4,
			// Token: 0x040010D9 RID: 4313
			ImmuneToGoo = 8,
			// Token: 0x040010DA RID: 4314
			ImmuneToExecutes = 16,
			// Token: 0x040010DB RID: 4315
			SprintAnyDirection = 32
		}

		// Token: 0x02000284 RID: 644
		public class ItemBehavior : MonoBehaviour
		{
			// Token: 0x040010DC RID: 4316
			public CharacterBody body;

			// Token: 0x040010DD RID: 4317
			public int stack;
		}

		// Token: 0x02000285 RID: 645
		public class MushroomItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CBE RID: 3262 RVA: 0x00052188 File Offset: 0x00050388
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				int stack = this.stack;
				bool flag = stack > 0 && this.body.GetNotMoving();
				float networkradius = 1.5f + 1.5f * (float)stack;
				if (this.mushroomWard != flag)
				{
					if (flag)
					{
						this.mushroomWard = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MushroomWard"), this.body.footPosition, Quaternion.identity);
						this.mushroomWard.GetComponent<TeamFilter>().teamIndex = this.body.teamComponent.teamIndex;
						NetworkServer.Spawn(this.mushroomWard);
					}
					else
					{
						UnityEngine.Object.Destroy(this.mushroomWard);
						this.mushroomWard = null;
					}
				}
				if (this.mushroomWard)
				{
					HealingWard component = this.mushroomWard.GetComponent<HealingWard>();
					component.healFraction = 0.0225f + 0.0225f * (float)stack;
					component.healPoints = 0f;
					component.Networkradius = networkradius;
				}
			}

			// Token: 0x06000CBF RID: 3263 RVA: 0x00009F6E File Offset: 0x0000816E
			private void OnDisable()
			{
				if (this.mushroomWard)
				{
					UnityEngine.Object.Destroy(this.mushroomWard);
				}
			}

			// Token: 0x040010DE RID: 4318
			private GameObject mushroomWard;
		}

		// Token: 0x02000286 RID: 646
		public class IcicleItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CC1 RID: 3265 RVA: 0x00052278 File Offset: 0x00050478
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				bool flag = this.stack > 0;
				if (this.icicleAura != flag)
				{
					if (flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/IcicleAura"), base.transform.position, Quaternion.identity);
						this.icicleAura = gameObject.GetComponent<IcicleAuraController>();
						this.icicleAura.Networkowner = base.gameObject;
						NetworkServer.Spawn(gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.icicleAura.gameObject);
					this.icicleAura = null;
				}
			}

			// Token: 0x06000CC2 RID: 3266 RVA: 0x00009F90 File Offset: 0x00008190
			public void OnOwnerKillOther()
			{
				if (this.icicleAura)
				{
					this.icicleAura.OnOwnerKillOther();
				}
			}

			// Token: 0x06000CC3 RID: 3267 RVA: 0x00009FAA File Offset: 0x000081AA
			private void OnDisable()
			{
				if (this.icicleAura)
				{
					UnityEngine.Object.Destroy(this.icicleAura);
				}
			}

			// Token: 0x040010DF RID: 4319
			private IcicleAuraController icicleAura;
		}

		// Token: 0x02000287 RID: 647
		public class HeadstomperItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x06000CC5 RID: 3269 RVA: 0x00052304 File Offset: 0x00050504
			private void FixedUpdate()
			{
				bool flag = this.stack > 0;
				if (flag != this.headstompersControllerObject)
				{
					if (flag)
					{
						this.headstompersControllerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HeadstompersController"));
						this.headstompersControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.body.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x06000CC6 RID: 3270 RVA: 0x00009FC4 File Offset: 0x000081C4
			private void OnDisable()
			{
				if (this.headstompersControllerObject)
				{
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x040010E0 RID: 4320
			private GameObject headstompersControllerObject;
		}

		// Token: 0x02000288 RID: 648
		private class ConstructTurretMessage : MessageBase
		{
			// Token: 0x06000CC9 RID: 3273 RVA: 0x00009FDE File Offset: 0x000081DE
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.builder);
				writer.Write(this.position);
				writer.Write(this.rotation);
			}

			// Token: 0x06000CCA RID: 3274 RVA: 0x0000A004 File Offset: 0x00008204
			public override void Deserialize(NetworkReader reader)
			{
				this.builder = reader.ReadGameObject();
				this.position = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
			}

			// Token: 0x040010E1 RID: 4321
			public GameObject builder;

			// Token: 0x040010E2 RID: 4322
			public Vector3 position;

			// Token: 0x040010E3 RID: 4323
			public Quaternion rotation;
		}
	}
}
