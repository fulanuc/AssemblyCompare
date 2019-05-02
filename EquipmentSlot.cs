using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002E9 RID: 745
	[RequireComponent(typeof(CharacterBody))]
	public class EquipmentSlot : NetworkBehaviour
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x0000B8E2 File Offset: 0x00009AE2
		// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x0000B8EA File Offset: 0x00009AEA
		public EquipmentIndex equipmentIndex { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0000B8F3 File Offset: 0x00009AF3
		// (set) Token: 0x06000EFB RID: 3835 RVA: 0x0000B8FB File Offset: 0x00009AFB
		public int stock { get; private set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000EFC RID: 3836 RVA: 0x0000B904 File Offset: 0x00009B04
		// (set) Token: 0x06000EFD RID: 3837 RVA: 0x0000B90C File Offset: 0x00009B0C
		public int maxStock { get; private set; }

		// Token: 0x06000EFE RID: 3838 RVA: 0x0005AC3C File Offset: 0x00058E3C
		private void UpdateInventory()
		{
			this.inventory = this.characterBody.inventory;
			if (this.inventory)
			{
				this.equipmentIndex = this.inventory.GetEquipmentIndex();
				this.stock = (int)this.inventory.GetEquipment((uint)this.inventory.activeEquipmentSlot).charges;
				this.maxStock = this.inventory.GetActiveEquipmentMaxCharges();
				this._rechargeTime = this.inventory.GetEquipment((uint)this.inventory.activeEquipmentSlot).chargeFinishTime;
				return;
			}
			this.equipmentIndex = EquipmentIndex.None;
			this.stock = 0;
			this.maxStock = 0;
			this._rechargeTime = Run.FixedTimeStamp.positiveInfinity;
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0000B915 File Offset: 0x00009B15
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x0000B91D File Offset: 0x00009B1D
		public CharacterBody characterBody { get; private set; }

		// Token: 0x06000F01 RID: 3841 RVA: 0x0005ACEC File Offset: 0x00058EEC
		[Server]
		private void UpdateGoldGat()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.EquipmentSlot::UpdateGoldGat()' called on client");
				return;
			}
			bool flag = this.equipmentIndex == EquipmentIndex.GoldGat;
			if (flag != this.goldgatControllerObject)
			{
				if (flag)
				{
					this.goldgatControllerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GoldGatController"));
					this.goldgatControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
					return;
				}
				UnityEngine.Object.Destroy(this.goldgatControllerObject);
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000F02 RID: 3842 RVA: 0x0000B926 File Offset: 0x00009B26
		public float cooldownTimer
		{
			get
			{
				return this._rechargeTime.timeUntil;
			}
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0005AD64 File Offset: 0x00058F64
		public Transform FindActiveEquipmentDisplay()
		{
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				Transform modelTransform = component.modelTransform;
				if (modelTransform)
				{
					CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
					if (component2)
					{
						List<GameObject> equipmentDisplayObjects = component2.GetEquipmentDisplayObjects(this.equipmentIndex);
						if (equipmentDisplayObjects.Count > 0)
						{
							return equipmentDisplayObjects[0].transform;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0005ADC4 File Offset: 0x00058FC4
		[ClientRpc]
		private void RpcOnClientEquipmentActivationRecieved()
		{
			Util.PlaySound(EquipmentSlot.equipmentActivateString, base.gameObject);
			EquipmentIndex equipmentIndex = this.equipmentIndex;
			if (equipmentIndex != EquipmentIndex.Blackhole)
			{
				if (equipmentIndex != EquipmentIndex.CritOnUse)
				{
					if (equipmentIndex == EquipmentIndex.BFG)
					{
						Transform transform = this.FindActiveEquipmentDisplay();
						if (transform)
						{
							Animator componentInChildren = transform.GetComponentInChildren<Animator>();
							if (componentInChildren)
							{
								componentInChildren.SetTrigger("Fire");
								return;
							}
						}
					}
				}
				else
				{
					Transform transform2 = this.FindActiveEquipmentDisplay();
					if (transform2)
					{
						Animator componentInChildren2 = transform2.GetComponentInChildren<Animator>();
						if (componentInChildren2)
						{
							componentInChildren2.SetBool("active", true);
							componentInChildren2.SetFloat("activeDuration", 8f);
							componentInChildren2.SetFloat("activeStopwatch", 0f);
						}
					}
				}
			}
			else
			{
				Transform transform3 = this.FindActiveEquipmentDisplay();
				if (transform3)
				{
					GravCubeController component = transform3.GetComponent<GravCubeController>();
					if (component)
					{
						component.ActivateCube(9f);
						return;
					}
				}
			}
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0005AEAC File Offset: 0x000590AC
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.targetIndicator = new Indicator(base.gameObject, null);
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0000B933 File Offset: 0x00009B33
		private void OnDestroy()
		{
			this.targetIndicator.active = false;
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0005AEFC File Offset: 0x000590FC
		private void FixedUpdate()
		{
			this.UpdateInventory();
			if (NetworkServer.active)
			{
				this.subcooldownTimer -= Time.fixedDeltaTime;
				if (this.missileTimer > 0f)
				{
					this.missileTimer = Mathf.Max(this.missileTimer - Time.fixedDeltaTime, 0f);
				}
				if (this.missileTimer == 0f && this.remainingMissiles > 0)
				{
					this.remainingMissiles--;
					this.missileTimer = 0.125f;
					this.FireMissile();
				}
				this.UpdateGoldGat();
				if (this.bfgChargeTimer > 0f)
				{
					this.bfgChargeTimer -= Time.fixedDeltaTime;
					if (this.bfgChargeTimer < 0f)
					{
						Vector3 position = base.transform.position;
						Ray ray = new Ray
						{
							direction = this.inputBank.aimDirection,
							origin = this.inputBank.aimOrigin
						};
						Transform transform = this.FindActiveEquipmentDisplay();
						if (transform)
						{
							ChildLocator componentInChildren = transform.GetComponentInChildren<ChildLocator>();
							if (componentInChildren)
							{
								Transform transform2 = componentInChildren.FindChild("Muzzle");
								if (transform2)
								{
									ray.origin = transform2.position;
								}
							}
						}
						this.healthComponent.TakeDamageForce(ray.direction * -1500f, true);
						ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/BeamSphere"), ray.origin, Util.QuaternionSafeLookRotation(ray.direction), base.gameObject, this.characterBody.damage * 2f, 0f, Util.CheckRoll(this.characterBody.crit, this.characterBody.master), DamageColorIndex.Item, null, -1f);
						this.bfgChargeTimer = 0f;
					}
				}
				if (this.equipmentIndex == EquipmentIndex.PassiveHealing != this.passiveHealingFollower)
				{
					if (this.equipmentIndex == EquipmentIndex.PassiveHealing)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealingFollower"), base.transform.position, Quaternion.identity);
						this.passiveHealingFollower = gameObject.GetComponent<HealingFollowerController>();
						this.passiveHealingFollower.NetworkownerBodyObject = base.gameObject;
						NetworkServer.Spawn(gameObject);
					}
					else
					{
						UnityEngine.Object.Destroy(this.passiveHealingFollower.gameObject);
						this.passiveHealingFollower = null;
					}
				}
			}
			if (!this.inputBank.activateEquipment.justPressed)
			{
				Inventory inventory = this.inventory;
				if (inventory == null || inventory.GetItemCount(ItemIndex.AutoCastEquipment) <= 0)
				{
					return;
				}
			}
			if (NetworkServer.active)
			{
				this.ExecuteIfReady();
				return;
			}
			this.CallCmdExecuteIfReady();
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0000B941 File Offset: 0x00009B41
		[Command]
		private void CmdExecuteIfReady()
		{
			this.ExecuteIfReady();
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0000B94A File Offset: 0x00009B4A
		[Server]
		public bool ExecuteIfReady()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.EquipmentSlot::ExecuteIfReady()' called on client");
				return false;
			}
			if (this.equipmentIndex != EquipmentIndex.None && this.stock > 0)
			{
				this.Execute();
				return true;
			}
			return false;
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0005B18C File Offset: 0x0005938C
		private void Execute()
		{
			EquipmentIndex equipmentIndex = this.equipmentIndex;
			if (EquipmentCatalog.GetEquipmentDef(equipmentIndex) != null && this.subcooldownTimer <= 0f && this.PerformEquipmentAction(equipmentIndex))
			{
				this.inventory.DeductActiveEquipmentCharges(1);
				this.CallRpcOnClientEquipmentActivationRecieved();
				Action<EquipmentSlot, EquipmentIndex> action = EquipmentSlot.onServerEquipmentActivated;
				if (action == null)
				{
					return;
				}
				action(this, equipmentIndex);
			}
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000F0B RID: 3851 RVA: 0x0005B1E4 File Offset: 0x000593E4
		// (remove) Token: 0x06000F0C RID: 3852 RVA: 0x0005B218 File Offset: 0x00059418
		public static event Action<EquipmentSlot, EquipmentIndex> onServerEquipmentActivated;

		// Token: 0x06000F0D RID: 3853 RVA: 0x0005B24C File Offset: 0x0005944C
		private void FireMissile()
		{
			EquipmentIndex equipmentIndex = this.equipmentIndex;
			GameObject prefab;
			float num;
			if (equipmentIndex == EquipmentIndex.AffixRed)
			{
				prefab = Resources.Load<GameObject>("Prefabs/Projectiles/RedAffixMissileProjectile");
				num = 1f;
			}
			else
			{
				prefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
				num = 3f;
			}
			bool crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
			Vector3 position = this.inputBank ? this.inputBank.aimOrigin : base.transform.position;
			Vector3 vector = this.inputBank ? this.inputBank.aimDirection : base.transform.forward;
			Vector3 a = Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f;
			ProjectileManager.instance.FireProjectile(prefab, position, Util.QuaternionSafeLookRotation(a + UnityEngine.Random.insideUnitSphere * 0f), base.gameObject, this.characterBody.damage * num, 200f, crit, DamageColorIndex.Item, null, -1f);
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x0005B358 File Offset: 0x00059558
		private bool PerformEquipmentAction(EquipmentIndex equipmentIndex)
		{
			switch (equipmentIndex)
			{
			case EquipmentIndex.CommandMissile:
				this.remainingMissiles += 12;
				return true;
			case EquipmentIndex.Saw:
			{
				Vector3 position = base.transform.position;
				Ray ray = new Ray
				{
					direction = this.inputBank.aimDirection,
					origin = this.inputBank.aimOrigin
				};
				bool crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), ray.origin, Util.QuaternionSafeLookRotation(ray.direction), base.gameObject, this.characterBody.damage, 0f, crit, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.Fruit:
				if (this.healthComponent)
				{
					Util.PlaySound("Play_item_use_fruit", base.gameObject);
					EffectData effectData = new EffectData();
					effectData.origin = base.transform.position;
					effectData.SetNetworkedObjectReference(base.gameObject);
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
					this.healthComponent.HealFraction(0.5f, default(ProcChainMask));
					return true;
				}
				return true;
			case EquipmentIndex.Meteor:
			{
				MeteorStormController component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), this.characterBody.corePosition, Quaternion.identity).GetComponent<MeteorStormController>();
				component.owner = base.gameObject;
				component.ownerDamage = this.characterBody.damage;
				component.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				NetworkServer.Spawn(component.gameObject);
				return true;
			}
			case EquipmentIndex.SoulJar:
				return true;
			case EquipmentIndex.Blackhole:
			{
				Vector3 position2 = base.transform.position;
				Ray ray2 = new Ray
				{
					direction = this.inputBank.aimDirection,
					origin = this.inputBank.aimOrigin
				};
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/GravSphere"), position2, Util.QuaternionSafeLookRotation(ray2.direction), base.gameObject, 0f, 0f, false, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.GhostGun:
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GhostGun"), base.transform.position, Quaternion.identity);
				gameObject.GetComponent<GhostGunController>().owner = base.gameObject;
				NetworkServer.Spawn(gameObject);
				return true;
			}
			case EquipmentIndex.CritOnUse:
				this.characterBody.AddTimedBuff(BuffIndex.FullCrit, 8f);
				return true;
			case EquipmentIndex.DroneBackup:
			{
				Util.PlaySound("Play_item_use_radio", base.gameObject);
				int num = 4;
				float num2 = 25f;
				if (NetworkServer.active)
				{
					for (int i = 0; i < num; i++)
					{
						Vector2 vector = UnityEngine.Random.insideUnitCircle.normalized * 3f;
						Vector3 position3 = base.transform.position + new Vector3(vector.x, 0f, vector.y);
						this.SummonMaster(Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster"), position3).gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = num2 + UnityEngine.Random.Range(0f, 3f);
					}
					return true;
				}
				return true;
			}
			case EquipmentIndex.OrbitalLaser:
			{
				Vector3 position4 = base.transform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray
				{
					direction = this.inputBank.aimDirection,
					origin = this.inputBank.aimOrigin
				}, out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					position4 = raycastHit.point;
				}
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/NetworkedObjects/OrbitalLaser"), position4, Quaternion.identity);
				gameObject2.GetComponent<OrbitalLaserController>().ownerBody = this.characterBody;
				NetworkServer.Spawn(gameObject2);
				return true;
			}
			case EquipmentIndex.BFG:
				this.bfgChargeTimer = 2f;
				this.subcooldownTimer = 2.2f;
				return true;
			case EquipmentIndex.Enigma:
			{
				EquipmentIndex equipmentIndex2 = EquipmentCatalog.enigmaEquipmentList[UnityEngine.Random.Range(0, EquipmentCatalog.enigmaEquipmentList.Count - 1)];
				this.PerformEquipmentAction(equipmentIndex2);
				return true;
			}
			case EquipmentIndex.Jetpack:
			{
				JetpackController jetpackController = JetpackController.FindJetpackController(base.gameObject);
				if (!jetpackController)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/JetpackController"));
					jetpackController = gameObject3.GetComponent<JetpackController>();
					jetpackController.NetworktargetObject = base.gameObject;
					NetworkServer.Spawn(gameObject3);
					return true;
				}
				jetpackController.ResetTimer();
				return true;
			}
			case EquipmentIndex.Lightning:
			{
				HurtBox hurtBox = this.currentTargetHurtBox;
				if (hurtBox)
				{
					this.subcooldownTimer = 0.2f;
					OrbManager.instance.AddOrb(new LightningStrikeOrb
					{
						attacker = base.gameObject,
						damageColorIndex = DamageColorIndex.Item,
						damageValue = this.characterBody.damage * 30f,
						isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master),
						procChainMask = default(ProcChainMask),
						procCoefficient = 1f,
						target = hurtBox
					});
					return true;
				}
				return false;
			}
			case EquipmentIndex.PassiveHealing:
				if (this.passiveHealingFollower)
				{
					this.passiveHealingFollower.AssignNewTarget(this.currentTargetBodyObject);
					return true;
				}
				return true;
			case EquipmentIndex.BurnNearby:
				if (this.characterBody)
				{
					this.characterBody.AddHelfireDuration(8f);
					return true;
				}
				return true;
			case EquipmentIndex.SoulCorruptor:
			{
				HurtBox hurtBox2 = this.currentTargetHurtBox;
				if (!hurtBox2)
				{
					return false;
				}
				if (!hurtBox2.healthComponent || hurtBox2.healthComponent.combinedHealthFraction > 0.25f)
				{
					return false;
				}
				Util.TryToCreateGhost(hurtBox2.healthComponent.body, this.characterBody, 30);
				hurtBox2.healthComponent.Suicide(base.gameObject);
				return true;
			}
			case EquipmentIndex.Scanner:
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), this.characterBody.corePosition, Quaternion.identity));
				return true;
			case EquipmentIndex.CrippleWard:
				NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/CrippleWard"), this.characterBody.corePosition, Quaternion.identity));
				this.inventory.SetEquipmentIndex(EquipmentIndex.None);
				return true;
			}
			return false;
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0005B9EC File Offset: 0x00059BEC
		private Ray GetAimRay()
		{
			return new Ray
			{
				direction = this.inputBank.aimDirection,
				origin = this.inputBank.aimOrigin
			};
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0005BA28 File Offset: 0x00059C28
		[Server]
		private CharacterMaster SummonMaster(GameObject masterObjectPrefab, Vector3 position)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.EquipmentSlot::SummonMaster(UnityEngine.GameObject,UnityEngine.Vector3)' called on client");
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(masterObjectPrefab, position, base.transform.rotation);
			NetworkServer.Spawn(gameObject);
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			component.SpawnBody(component.bodyPrefab, position, base.transform.rotation);
			AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
			if (component2)
			{
				CharacterBody characterBody = this.characterBody;
				if (characterBody)
				{
					CharacterMaster master = characterBody.master;
					if (master)
					{
						component2.ownerMaster = master;
					}
				}
			}
			BaseAI component3 = gameObject.GetComponent<BaseAI>();
			if (component3)
			{
				component3.leader.gameObject = base.gameObject;
			}
			return component;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0005BAE8 File Offset: 0x00059CE8
		private void ConfigureTargetFinderBase()
		{
			this.targetFinder.teamMaskFilter = TeamMask.allButNeutral;
			this.targetFinder.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.targetFinder.sortMode = BullseyeSearch.SortMode.Angle;
			this.targetFinder.filterByLoS = true;
			float num;
			Ray ray = CameraRigController.ModifyAimRayIfApplicable(this.GetAimRay(), base.gameObject, out num);
			this.targetFinder.searchOrigin = ray.origin;
			this.targetFinder.searchDirection = ray.direction;
			this.targetFinder.maxAngleFilter = 10f;
			this.targetFinder.viewer = this.characterBody;
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0000B97D File Offset: 0x00009B7D
		private void ConfigureTargetFinderForEnemies()
		{
			this.ConfigureTargetFinderBase();
			this.targetFinder.teamMaskFilter = TeamMask.all;
			this.targetFinder.teamMaskFilter.RemoveTeam(this.teamComponent.teamIndex);
			this.targetFinder.RefreshCandidates();
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0005BB94 File Offset: 0x00059D94
		private void ConfigureTargetFinderForFriendlies()
		{
			this.ConfigureTargetFinderBase();
			this.targetFinder.teamMaskFilter = TeamMask.none;
			this.targetFinder.teamMaskFilter.AddTeam(this.teamComponent.teamIndex);
			this.targetFinder.RefreshCandidates();
			this.targetFinder.FilterOutGameObject(base.gameObject);
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0005BBF0 File Offset: 0x00059DF0
		private void Update()
		{
			bool flag = (this.equipmentIndex == EquipmentIndex.Lightning || this.equipmentIndex == EquipmentIndex.SoulCorruptor) && this.stock > 0;
			bool flag2 = this.equipmentIndex == EquipmentIndex.PassiveHealing && this.stock > 0;
			if (flag || flag2)
			{
				if (flag)
				{
					this.ConfigureTargetFinderForEnemies();
				}
				if (flag2)
				{
					this.ConfigureTargetFinderForFriendlies();
				}
				this.currentTargetHurtBox = this.targetFinder.GetResults().FirstOrDefault<HurtBox>();
				this.currentTargetBodyObject = (this.currentTargetHurtBox ? this.currentTargetHurtBox.healthComponent.gameObject : null);
			}
			else
			{
				this.currentTargetHurtBox = null;
			}
			bool flag3 = this.currentTargetHurtBox;
			if (flag3)
			{
				EquipmentIndex equipmentIndex = this.equipmentIndex;
				if (equipmentIndex != EquipmentIndex.Lightning)
				{
					if (equipmentIndex != EquipmentIndex.PassiveHealing)
					{
						this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/LightningIndicator");
					}
					else
					{
						this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/WoodSpriteIndicator");
					}
				}
				else
				{
					this.targetIndicator.visualizerPrefab = Resources.Load<GameObject>("Prefabs/LightningIndicator");
				}
			}
			this.targetIndicator.active = flag3;
			this.targetIndicator.targetTransform = (flag3 ? this.currentTargetHurtBox.transform : null);
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0005BD20 File Offset: 0x00059F20
		static EquipmentSlot()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(EquipmentSlot), EquipmentSlot.kCmdCmdExecuteIfReady, new NetworkBehaviour.CmdDelegate(EquipmentSlot.InvokeCmdCmdExecuteIfReady));
			EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved = 1342577121;
			NetworkBehaviour.RegisterRpcDelegate(typeof(EquipmentSlot), EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved, new NetworkBehaviour.CmdDelegate(EquipmentSlot.InvokeRpcRpcOnClientEquipmentActivationRecieved));
			NetworkCRC.RegisterBehaviour("EquipmentSlot", 0);
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0000B9CE File Offset: 0x00009BCE
		protected static void InvokeCmdCmdExecuteIfReady(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdExecuteIfReady called on client.");
				return;
			}
			((EquipmentSlot)obj).CmdExecuteIfReady();
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0005BD9C File Offset: 0x00059F9C
		public void CallCmdExecuteIfReady()
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdExecuteIfReady called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdExecuteIfReady();
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)EquipmentSlot.kCmdCmdExecuteIfReady);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			base.SendCommandInternal(networkWriter, 0, "CmdExecuteIfReady");
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0000B9F1 File Offset: 0x00009BF1
		protected static void InvokeRpcRpcOnClientEquipmentActivationRecieved(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcOnClientEquipmentActivationRecieved called on server.");
				return;
			}
			((EquipmentSlot)obj).RpcOnClientEquipmentActivationRecieved();
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0005BE18 File Offset: 0x0005A018
		public void CallRpcOnClientEquipmentActivationRecieved()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcOnClientEquipmentActivationRecieved called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)EquipmentSlot.kRpcRpcOnClientEquipmentActivationRecieved);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcOnClientEquipmentActivationRecieved");
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400130D RID: 4877
		private Inventory inventory;

		// Token: 0x04001311 RID: 4881
		private Run.FixedTimeStamp _rechargeTime;

		// Token: 0x04001313 RID: 4883
		private HealthComponent healthComponent;

		// Token: 0x04001314 RID: 4884
		private InputBankTest inputBank;

		// Token: 0x04001315 RID: 4885
		private TeamComponent teamComponent;

		// Token: 0x04001316 RID: 4886
		private const float fullCritDuration = 8f;

		// Token: 0x04001317 RID: 4887
		public static string equipmentActivateString = "Play_UI_equipment_activate";

		// Token: 0x04001318 RID: 4888
		private float missileTimer;

		// Token: 0x04001319 RID: 4889
		private float bfgChargeTimer;

		// Token: 0x0400131A RID: 4890
		private float subcooldownTimer;

		// Token: 0x0400131B RID: 4891
		private const float missileInterval = 0.125f;

		// Token: 0x0400131C RID: 4892
		private int remainingMissiles;

		// Token: 0x0400131D RID: 4893
		private HealingFollowerController passiveHealingFollower;

		// Token: 0x0400131E RID: 4894
		private GameObject goldgatControllerObject;

		// Token: 0x04001320 RID: 4896
		private Indicator targetIndicator;

		// Token: 0x04001321 RID: 4897
		private BullseyeSearch targetFinder = new BullseyeSearch();

		// Token: 0x04001322 RID: 4898
		private HurtBox currentTargetHurtBox;

		// Token: 0x04001323 RID: 4899
		private GameObject currentTargetBodyObject;

		// Token: 0x04001324 RID: 4900
		private static int kRpcRpcOnClientEquipmentActivationRecieved;

		// Token: 0x04001325 RID: 4901
		private static int kCmdCmdExecuteIfReady = -303452611;
	}
}
