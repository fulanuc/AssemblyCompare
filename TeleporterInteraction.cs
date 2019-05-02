using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FE RID: 1022
	[RequireComponent(typeof(SceneExitController))]
	public sealed class TeleporterInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider
	{
		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x00011046 File Offset: 0x0000F246
		// (set) Token: 0x060016AF RID: 5807 RVA: 0x0001104E File Offset: 0x0000F24E
		private TeleporterInteraction.ActivationState activationState
		{
			get
			{
				return (TeleporterInteraction.ActivationState)this.activationStateInternal;
			}
			set
			{
				this.NetworkactivationStateInternal = (uint)value;
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x00011057 File Offset: 0x0000F257
		public bool isIdle
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Idle;
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060016B1 RID: 5809 RVA: 0x00011062 File Offset: 0x0000F262
		public bool isIdleToCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.IdleToCharging;
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060016B2 RID: 5810 RVA: 0x0001106D File Offset: 0x0000F26D
		public bool isInFinalSequence
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Finished;
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060016B3 RID: 5811 RVA: 0x00011078 File Offset: 0x0000F278
		public bool isCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Charging;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060016B4 RID: 5812 RVA: 0x00011083 File Offset: 0x0000F283
		public bool isCharged
		{
			get
			{
				return this.activationState >= TeleporterInteraction.ActivationState.Charged;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060016B5 RID: 5813 RVA: 0x00011091 File Offset: 0x0000F291
		public float chargeFraction
		{
			get
			{
				return this.chargePercent * 0.01f;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x000110A1 File Offset: 0x0000F2A1
		// (set) Token: 0x060016B7 RID: 5815 RVA: 0x000110A8 File Offset: 0x0000F2A8
		public static TeleporterInteraction instance { get; private set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060016B8 RID: 5816 RVA: 0x000110B0 File Offset: 0x0000F2B0
		// (set) Token: 0x060016B9 RID: 5817 RVA: 0x000110B8 File Offset: 0x0000F2B8
		public bool shouldAttemptToSpawnShopPortal
		{
			get
			{
				return this._shouldAttemptToSpawnShopPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnShopPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnShopPortal = value;
				if (this._shouldAttemptToSpawnShopPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_SHOP_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060016BA RID: 5818 RVA: 0x000110EF File Offset: 0x0000F2EF
		// (set) Token: 0x060016BB RID: 5819 RVA: 0x000110F7 File Offset: 0x0000F2F7
		public bool shouldAttemptToSpawnGoldshoresPortal
		{
			get
			{
				return this._shouldAttemptToSpawnGoldshoresPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnGoldshoresPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnGoldshoresPortal = value;
				if (this._shouldAttemptToSpawnGoldshoresPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_GOLDSHORES_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060016BC RID: 5820 RVA: 0x0001112E File Offset: 0x0000F32E
		// (set) Token: 0x060016BD RID: 5821 RVA: 0x00011136 File Offset: 0x0000F336
		public bool shouldAttemptToSpawnMSPortal
		{
			get
			{
				return this._shouldAttemptToSpawnMSPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnMSPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnMSPortal = value;
				if (this._shouldAttemptToSpawnMSPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_MS_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x0001116D File Offset: 0x0000F36D
		private void OnSyncShouldAttemptToSpawnShopPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnShopPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("ShopPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x0001119E File Offset: 0x0000F39E
		private void OnSyncShouldAttemptToSpawnGoldshoresPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnGoldshoresPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("GoldshoresPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x000111CF File Offset: 0x0000F3CF
		private void OnSyncShouldAttemptToSpawnMSPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnMSPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("MSPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x000775A8 File Offset: 0x000757A8
		private void Awake()
		{
			this.remainingChargeTimer = this.chargeDuration;
			this.monsterCheckTimer = 0f;
			this.childLocator = base.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
			this.bossShrineIndicator = this.childLocator.FindChild("BossShrineSymbol").gameObject;
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00011200 File Offset: 0x0000F400
		private void OnEnable()
		{
			TeleporterInteraction.instance = SingletonHelper.Assign<TeleporterInteraction>(TeleporterInteraction.instance, this);
			InstanceTracker.Add<TeleporterInteraction>(this);
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x00011218 File Offset: 0x0000F418
		private void OnDisable()
		{
			InstanceTracker.Remove<TeleporterInteraction>(this);
			TeleporterInteraction.instance = SingletonHelper.Unassign<TeleporterInteraction>(TeleporterInteraction.instance, this);
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00077600 File Offset: 0x00075800
		private void Start()
		{
			if (this.clearRadiusIndicator)
			{
				float num = this.clearRadius * 2f;
				this.clearRadiusIndicator.transform.localScale = new Vector3(num, num, num);
			}
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
				SceneInfo instance = SceneInfo.instance;
				if (instance)
				{
					ClassicStageInfo component = instance.GetComponent<ClassicStageInfo>();
					if (component && component.destinations.Length != 0)
					{
						Run.instance.PickNextStageScene(component.destinations);
					}
				}
				float nextNormalizedFloat = this.rng.nextNormalizedFloat;
				float num2 = this.baseShopSpawnChance / (float)(Run.instance.shopPortalCount + 1);
				this.shouldAttemptToSpawnShopPortal = (nextNormalizedFloat < num2);
				int num3 = 4;
				int stageClearCount = Run.instance.stageClearCount;
				if ((stageClearCount + 1) % num3 == 3 && stageClearCount > num3)
				{
					this.shouldAttemptToSpawnMSPortal = true;
				}
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00011230 File Offset: 0x0000F430
		public string GetContextString(Interactor activator)
		{
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				return Language.GetString(this.beginContextString);
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				return Language.GetString(this.exitContextString);
			}
			return null;
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0001125C File Offset: 0x0000F45C
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.locked)
			{
				return Interactability.Disabled;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				return Interactability.Available;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				if (!this.monstersCleared)
				{
					return Interactability.ConditionsNotMet;
				}
				return Interactability.Available;
			}
			else
			{
				if (this.activationState == TeleporterInteraction.ActivationState.Charging)
				{
					return Interactability.ConditionsNotMet;
				}
				return Interactability.Disabled;
			}
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000776F0 File Offset: 0x000758F0
		public void OnInteractionBegin(Interactor activator)
		{
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				this.CallRpcClientOnActivated(activator.gameObject);
				Chat.SendBroadcastChat(new Chat.SubjectChatMessage
				{
					subjectCharacterBodyGameObject = activator.gameObject,
					baseToken = "PLAYER_ACTIVATED_TELEPORTER"
				});
				if (this.showBossIndicator)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "SHRINE_BOSS_BEGIN_TRIAL"
					});
				}
				this.activationState = TeleporterInteraction.ActivationState.IdleToCharging;
				return;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				this.activationState = TeleporterInteraction.ActivationState.Finished;
				base.GetComponent<SceneExitController>().Begin();
			}
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x000038B4 File Offset: 0x00001AB4
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00011293 File Offset: 0x0000F493
		[ClientRpc]
		private void RpcClientOnActivated(GameObject activatorObject)
		{
			if (this.musicSource)
			{
				this.musicSource.Play();
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x000112AD File Offset: 0x0000F4AD
		private void UpdateMonstersClear()
		{
			this.monstersCleared = (BossGroup.GetTotalBossCount() == 0);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x00077774 File Offset: 0x00075974
		private int GetPlayerCountInRadius()
		{
			int num = 0;
			Vector3 position = base.transform.position;
			float num2 = this.clearRadius * this.clearRadius;
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject) && (teamMembers[i].transform.position - position).sqrMagnitude <= num2)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0000DA33 File Offset: 0x0000BC33
		private int GetMonsterCount()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000112BD File Offset: 0x0000F4BD
		private float DiminishingReturns(int i)
		{
			return (1f - Mathf.Pow(0.5f, (float)i)) * 2f;
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x000112D7 File Offset: 0x0000F4D7
		[Server]
		public void AddShrineStack()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AddShrineStack()' called on client");
				return;
			}
			if (this.activationState <= TeleporterInteraction.ActivationState.IdleToCharging)
			{
				this.shrineBonusStacks++;
				this.NetworkshowBossIndicator = true;
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x000777FC File Offset: 0x000759FC
		public bool IsInChargingRange(GameObject gameObject)
		{
			return (gameObject.transform.position - base.transform.position).sqrMagnitude <= this.clearRadius * this.clearRadius;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0001130C File Offset: 0x0000F50C
		public bool IsInChargingRange(CharacterBody characterBody)
		{
			return this.IsInChargingRange(characterBody.gameObject);
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x00077840 File Offset: 0x00075A40
		public void FixedUpdate()
		{
			this.bossShrineIndicator.SetActive(this.showBossIndicator);
			if (this.previousActivationState != this.activationState)
			{
				this.OnStateChanged(this.previousActivationState, this.activationState);
			}
			this.previousActivationState = this.activationState;
			this.StateFixedUpdate();
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x00077898 File Offset: 0x00075A98
		private void StateFixedUpdate()
		{
			switch (this.activationState)
			{
			case TeleporterInteraction.ActivationState.IdleToCharging:
				this.idleToChargingStopwatch += Time.fixedDeltaTime;
				if (this.idleToChargingStopwatch > 3f)
				{
					this.activationState = TeleporterInteraction.ActivationState.Charging;
				}
				break;
			case TeleporterInteraction.ActivationState.Charging:
			{
				int num = Run.instance ? Run.instance.livingPlayerCount : 0;
				float num2 = (num != 0) ? ((float)this.GetPlayerCountInRadius() / (float)num * Time.fixedDeltaTime) : 0f;
				bool isCharging = num2 > 0f;
				this.remainingChargeTimer = Mathf.Max(this.remainingChargeTimer - num2, 0f);
				if (NetworkServer.active)
				{
					this.NetworkchargePercent = (uint)((byte)Mathf.RoundToInt(99f * (1f - this.remainingChargeTimer / this.chargeDuration)));
				}
				if (SceneWeatherController.instance)
				{
					SceneWeatherController.instance.weatherLerp = SceneWeatherController.instance.weatherLerpOverChargeTime.Evaluate(1f - this.remainingChargeTimer / this.chargeDuration);
				}
				if (!this.teleporterPositionIndicator)
				{
					this.teleporterPositionIndicator = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/TeleporterChargingPositionIndicator"), base.transform.position, Quaternion.identity);
					this.teleporterPositionIndicator.GetComponent<PositionIndicator>().targetTransform = base.transform;
				}
				else
				{
					ChargeIndicatorController component = this.teleporterPositionIndicator.GetComponent<ChargeIndicatorController>();
					component.isCharging = isCharging;
					component.chargingText.text = this.chargePercent.ToString() + "%";
				}
				this.UpdateMonstersClear();
				if (this.remainingChargeTimer <= 0f && NetworkServer.active)
				{
					if (this.bonusDirector)
					{
						this.bonusDirector.enabled = false;
					}
					if (this.monstersCleared)
					{
						if (this.bossDirector)
						{
							this.bossDirector.enabled = false;
						}
						this.activationState = TeleporterInteraction.ActivationState.Charged;
						this.OnChargingFinished();
					}
				}
				break;
			}
			case TeleporterInteraction.ActivationState.Charged:
				this.monsterCheckTimer -= Time.fixedDeltaTime;
				if (this.monsterCheckTimer <= 0f)
				{
					this.monsterCheckTimer = 1f;
					this.UpdateMonstersClear();
				}
				this.NetworkshowBossIndicator = false;
				break;
			}
			if (this.clearRadiusIndicator)
			{
				this.clearRadiusIndicator.SetActive(this.activationState >= TeleporterInteraction.ActivationState.Charging);
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x00077AF4 File Offset: 0x00075CF4
		private void OnStateChanged(TeleporterInteraction.ActivationState oldActivationState, TeleporterInteraction.ActivationState newActivationState)
		{
			switch (newActivationState)
			{
			case TeleporterInteraction.ActivationState.Idle:
				return;
			case TeleporterInteraction.ActivationState.IdleToCharging:
				this.childLocator.FindChild("IdleToChargingEffect").gameObject.SetActive(true);
				this.childLocator.FindChild("PPVolume").gameObject.SetActive(true);
				return;
			case TeleporterInteraction.ActivationState.Charging:
			{
				Action<TeleporterInteraction> action = TeleporterInteraction.onTeleporterBeginChargingGlobal;
				if (action != null)
				{
					action(this);
				}
				if (NetworkServer.active)
				{
					if (this.bonusDirector)
					{
						this.bonusDirector.enabled = true;
					}
					if (this.bossDirector)
					{
						this.bossDirector.enabled = true;
						this.bossDirector.monsterCredit += (float)((int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f) * (float)(1 + this.shrineBonusStacks)));
						this.bossDirector.currentSpawnTarget = base.gameObject;
						this.bossDirector.SetNextSpawnAsBoss();
					}
					if (DirectorCore.instance)
					{
						CombatDirector[] components = DirectorCore.instance.GetComponents<CombatDirector>();
						if (components.Length != 0)
						{
							CombatDirector[] array = components;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].enabled = false;
							}
						}
					}
					if (this.chestLockCoroutine == null)
					{
						this.chestLockCoroutine = base.StartCoroutine(this.ChestLockCoroutine());
					}
				}
				this.childLocator.FindChild("IdleToChargingEffect").gameObject.SetActive(false);
				this.childLocator.FindChild("ChargingEffect").gameObject.SetActive(true);
				return;
			}
			case TeleporterInteraction.ActivationState.Charged:
			{
				this.teleporterPositionIndicator.GetComponent<ChargeIndicatorController>().isCharged = true;
				this.childLocator.FindChild("ChargingEffect").gameObject.SetActive(false);
				this.childLocator.FindChild("ChargedEffect").gameObject.SetActive(true);
				this.childLocator.FindChild("BossShrineSymbol").gameObject.SetActive(false);
				Action<TeleporterInteraction> action2 = TeleporterInteraction.onTeleporterChargedGlobal;
				if (action2 == null)
				{
					return;
				}
				action2(this);
				return;
			}
			case TeleporterInteraction.ActivationState.Finished:
			{
				this.childLocator.FindChild("ChargedEffect").gameObject.SetActive(false);
				Action<TeleporterInteraction> action3 = TeleporterInteraction.onTeleporterFinishGlobal;
				if (action3 == null)
				{
					return;
				}
				action3(this);
				return;
			}
			default:
				throw new ArgumentOutOfRangeException("newActivationState", newActivationState, null);
			}
		}

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x060016D4 RID: 5844 RVA: 0x00077D30 File Offset: 0x00075F30
		// (remove) Token: 0x060016D5 RID: 5845 RVA: 0x00077D64 File Offset: 0x00075F64
		public static event Action<TeleporterInteraction> onTeleporterBeginChargingGlobal;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x060016D6 RID: 5846 RVA: 0x00077D98 File Offset: 0x00075F98
		// (remove) Token: 0x060016D7 RID: 5847 RVA: 0x00077DCC File Offset: 0x00075FCC
		public static event Action<TeleporterInteraction> onTeleporterChargedGlobal;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x060016D8 RID: 5848 RVA: 0x00077E00 File Offset: 0x00076000
		// (remove) Token: 0x060016D9 RID: 5849 RVA: 0x00077E34 File Offset: 0x00076034
		public static event Action<TeleporterInteraction> onTeleporterFinishGlobal;

		// Token: 0x060016DA RID: 5850 RVA: 0x00077E68 File Offset: 0x00076068
		[Server]
		private void OnChargingFinished()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::OnChargingFinished()' called on client");
				return;
			}
			if (this.shouldAttemptToSpawnShopPortal)
			{
				this.AttemptToSpawnShopPortal();
			}
			if (this.shouldAttemptToSpawnGoldshoresPortal)
			{
				this.AttemptToSpawnGoldshoresPortal();
			}
			if (this.shouldAttemptToSpawnMSPortal)
			{
				this.AttemptToSpawnMSPortal();
			}
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x00077EB4 File Offset: 0x000760B4
		[Server]
		private void AttemptToSpawnShopPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnShopPortal()' called on client");
				return;
			}
			Debug.Log("Submitting request for shop portal");
			if (DirectorCore.instance.TrySpawnObject(this.shopPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 30f,
				minDistance = 0f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng))
			{
				Debug.Log("Succeeded in creating shop portal!");
				Run.instance.shopPortalCount++;
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_SHOP_OPEN"
				});
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00077F70 File Offset: 0x00076170
		[Server]
		private void AttemptToSpawnGoldshoresPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnGoldshoresPortal()' called on client");
				return;
			}
			if (DirectorCore.instance.TrySpawnObject(this.goldshoresPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 40f,
				minDistance = 10f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng))
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_GOLDSHORES_OPEN"
				});
			}
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00078004 File Offset: 0x00076204
		[Server]
		private void AttemptToSpawnMSPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnMSPortal()' called on client");
				return;
			}
			if (DirectorCore.instance.TrySpawnObject(this.msPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 40f,
				minDistance = 10f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng))
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_MS_OPEN"
				});
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00011078 File Offset: 0x0000F278
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.activationState == TeleporterInteraction.ActivationState.Charging;
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x0001131A File Offset: 0x0000F51A
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/TimerHologramContent");
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00078098 File Offset: 0x00076298
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			TimerHologramContent component = hologramContentObject.GetComponent<TimerHologramContent>();
			if (component)
			{
				component.displayValue = this.remainingChargeTimer;
			}
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00011326 File Offset: 0x0000F526
		private IEnumerator ChestLockCoroutine()
		{
			List<GameObject> lockInstances = new List<GameObject>();
			Vector3 myPosition = base.transform.position;
			float maxDistanceSqr = this.clearRadius * this.clearRadius;
			PurchaseInteraction[] purchasables = UnityEngine.Object.FindObjectsOfType<PurchaseInteraction>();
			int num;
			for (int i = 0; i < purchasables.Length; i = num)
			{
				if (purchasables[i] && purchasables[i].available)
				{
					Vector3 position = purchasables[i].transform.position;
					if ((position - myPosition).sqrMagnitude > maxDistanceSqr && !purchasables[i].lockGameObject)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.lockPrefab, position, Quaternion.identity);
						NetworkServer.Spawn(gameObject);
						purchasables[i].NetworklockGameObject = gameObject;
						lockInstances.Add(gameObject);
						yield return new WaitForSeconds(0.1f);
					}
				}
				num = i + 1;
			}
			while (this.activationState == TeleporterInteraction.ActivationState.Charging)
			{
				yield return new WaitForSeconds(1f);
			}
			for (int i = 0; i < lockInstances.Count; i = num)
			{
				UnityEngine.Object.Destroy(lockInstances[i]);
				yield return new WaitForSeconds(0.1f);
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060016E5 RID: 5861 RVA: 0x000780C0 File Offset: 0x000762C0
		// (set) Token: 0x060016E6 RID: 5862 RVA: 0x0001135E File Offset: 0x0000F55E
		public uint NetworkactivationStateInternal
		{
			get
			{
				return this.activationStateInternal;
			}
			set
			{
				base.SetSyncVar<uint>(value, ref this.activationStateInternal, 1u);
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060016E7 RID: 5863 RVA: 0x000780D4 File Offset: 0x000762D4
		// (set) Token: 0x060016E8 RID: 5864 RVA: 0x00011372 File Offset: 0x0000F572
		public bool Networklocked
		{
			get
			{
				return this.locked;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.locked, 2u);
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060016E9 RID: 5865 RVA: 0x000780E8 File Offset: 0x000762E8
		// (set) Token: 0x060016EA RID: 5866 RVA: 0x00011386 File Offset: 0x0000F586
		public uint NetworkchargePercent
		{
			get
			{
				return this.chargePercent;
			}
			set
			{
				base.SetSyncVar<uint>(value, ref this.chargePercent, 4u);
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060016EB RID: 5867 RVA: 0x000780FC File Offset: 0x000762FC
		// (set) Token: 0x060016EC RID: 5868 RVA: 0x0001139A File Offset: 0x0000F59A
		public bool Network_shouldAttemptToSpawnShopPortal
		{
			get
			{
				return this._shouldAttemptToSpawnShopPortal;
			}
			set
			{
				uint dirtyBit = 8u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnShopPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnShopPortal, dirtyBit);
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x00078110 File Offset: 0x00076310
		// (set) Token: 0x060016EE RID: 5870 RVA: 0x000113D9 File Offset: 0x0000F5D9
		public bool Network_shouldAttemptToSpawnGoldshoresPortal
		{
			get
			{
				return this._shouldAttemptToSpawnGoldshoresPortal;
			}
			set
			{
				uint dirtyBit = 16u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnGoldshoresPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnGoldshoresPortal, dirtyBit);
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060016EF RID: 5871 RVA: 0x00078124 File Offset: 0x00076324
		// (set) Token: 0x060016F0 RID: 5872 RVA: 0x00011418 File Offset: 0x0000F618
		public bool Network_shouldAttemptToSpawnMSPortal
		{
			get
			{
				return this._shouldAttemptToSpawnMSPortal;
			}
			set
			{
				uint dirtyBit = 32u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnMSPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnMSPortal, dirtyBit);
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x00078138 File Offset: 0x00076338
		// (set) Token: 0x060016F2 RID: 5874 RVA: 0x00011457 File Offset: 0x0000F657
		public bool NetworkshowBossIndicator
		{
			get
			{
				return this.showBossIndicator;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.showBossIndicator, 64u);
			}
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x0001146B File Offset: 0x0000F66B
		protected static void InvokeRpcRpcClientOnActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientOnActivated called on server.");
				return;
			}
			((TeleporterInteraction)obj).RpcClientOnActivated(reader.ReadGameObject());
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x0007814C File Offset: 0x0007634C
		public void CallRpcClientOnActivated(GameObject activatorObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcClientOnActivated called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)TeleporterInteraction.kRpcRpcClientOnActivated);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(activatorObject);
			this.SendRPCInternal(networkWriter, 0, "RpcClientOnActivated");
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00011494 File Offset: 0x0000F694
		static TeleporterInteraction()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(TeleporterInteraction), TeleporterInteraction.kRpcRpcClientOnActivated, new NetworkBehaviour.CmdDelegate(TeleporterInteraction.InvokeRpcRpcClientOnActivated));
			NetworkCRC.RegisterBehaviour("TeleporterInteraction", 0);
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x000781C0 File Offset: 0x000763C0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32(this.activationStateInternal);
				writer.Write(this.locked);
				writer.WritePackedUInt32(this.chargePercent);
				writer.Write(this._shouldAttemptToSpawnShopPortal);
				writer.Write(this._shouldAttemptToSpawnGoldshoresPortal);
				writer.Write(this._shouldAttemptToSpawnMSPortal);
				writer.Write(this.showBossIndicator);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this.activationStateInternal);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.locked);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this.chargePercent);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnShopPortal);
			}
			if ((base.syncVarDirtyBits & 16u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnGoldshoresPortal);
			}
			if ((base.syncVarDirtyBits & 32u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnMSPortal);
			}
			if ((base.syncVarDirtyBits & 64u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.showBossIndicator);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x000783A8 File Offset: 0x000765A8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.activationStateInternal = reader.ReadPackedUInt32();
				this.locked = reader.ReadBoolean();
				this.chargePercent = reader.ReadPackedUInt32();
				this._shouldAttemptToSpawnShopPortal = reader.ReadBoolean();
				this._shouldAttemptToSpawnGoldshoresPortal = reader.ReadBoolean();
				this._shouldAttemptToSpawnMSPortal = reader.ReadBoolean();
				this.showBossIndicator = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.activationStateInternal = reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.locked = reader.ReadBoolean();
			}
			if ((num & 4) != 0)
			{
				this.chargePercent = reader.ReadPackedUInt32();
			}
			if ((num & 8) != 0)
			{
				this.OnSyncShouldAttemptToSpawnShopPortal(reader.ReadBoolean());
			}
			if ((num & 16) != 0)
			{
				this.OnSyncShouldAttemptToSpawnGoldshoresPortal(reader.ReadBoolean());
			}
			if ((num & 32) != 0)
			{
				this.OnSyncShouldAttemptToSpawnMSPortal(reader.ReadBoolean());
			}
			if ((num & 64) != 0)
			{
				this.showBossIndicator = reader.ReadBoolean();
			}
		}

		// Token: 0x040019C4 RID: 6596
		[SyncVar]
		private uint activationStateInternal;

		// Token: 0x040019C5 RID: 6597
		private TeleporterInteraction.ActivationState previousActivationState;

		// Token: 0x040019C6 RID: 6598
		[SyncVar]
		public bool locked;

		// Token: 0x040019C7 RID: 6599
		public AudioSource musicSource;

		// Token: 0x040019C8 RID: 6600
		[Tooltip("How long it takes for this teleporter to finish activating.")]
		public float chargeDuration = 90f;

		// Token: 0x040019C9 RID: 6601
		[Tooltip("The radius within which no monsters must exist for the teleporter event to conclude. Changing at runtime will not currently update the indicator scale.")]
		public float clearRadius = 40f;

		// Token: 0x040019CA RID: 6602
		[Tooltip("An object which will be used to represent the clear radius.")]
		public GameObject clearRadiusIndicator;

		// Token: 0x040019CB RID: 6603
		[HideInInspector]
		public float remainingChargeTimer;

		// Token: 0x040019CC RID: 6604
		public int shrineBonusStacks;

		// Token: 0x040019CD RID: 6605
		[SyncVar]
		private uint chargePercent;

		// Token: 0x040019CE RID: 6606
		private float idleToChargingStopwatch;

		// Token: 0x040019CF RID: 6607
		private float monsterCheckTimer;

		// Token: 0x040019D0 RID: 6608
		private GameObject teleporterPositionIndicator;

		// Token: 0x040019D1 RID: 6609
		public string beginContextString;

		// Token: 0x040019D2 RID: 6610
		public string exitContextString;

		// Token: 0x040019D3 RID: 6611
		private ChildLocator childLocator;

		// Token: 0x040019D4 RID: 6612
		public CombatDirector bonusDirector;

		// Token: 0x040019D5 RID: 6613
		public CombatDirector bossDirector;

		// Token: 0x040019D7 RID: 6615
		private GameObject bossShrineIndicator;

		// Token: 0x040019D8 RID: 6616
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnShopPortal")]
		private bool _shouldAttemptToSpawnShopPortal;

		// Token: 0x040019D9 RID: 6617
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnGoldshoresPortal")]
		private bool _shouldAttemptToSpawnGoldshoresPortal;

		// Token: 0x040019DA RID: 6618
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnMSPortal")]
		private bool _shouldAttemptToSpawnMSPortal;

		// Token: 0x040019DB RID: 6619
		private Xoroshiro128Plus rng;

		// Token: 0x040019DC RID: 6620
		private bool monstersCleared;

		// Token: 0x040019DD RID: 6621
		[SyncVar]
		private bool showBossIndicator;

		// Token: 0x040019E1 RID: 6625
		public SpawnCard shopPortalSpawnCard;

		// Token: 0x040019E2 RID: 6626
		public SpawnCard goldshoresPortalSpawnCard;

		// Token: 0x040019E3 RID: 6627
		public SpawnCard msPortalSpawnCard;

		// Token: 0x040019E4 RID: 6628
		public float baseShopSpawnChance = 0.25f;

		// Token: 0x040019E5 RID: 6629
		[Tooltip("The networked object which will be instantiated to lock purchasables.")]
		public GameObject lockPrefab;

		// Token: 0x040019E6 RID: 6630
		private Coroutine chestLockCoroutine;

		// Token: 0x040019E7 RID: 6631
		private static int kRpcRpcClientOnActivated = 1157394167;

		// Token: 0x020003FF RID: 1023
		private enum ActivationState
		{
			// Token: 0x040019E9 RID: 6633
			Idle,
			// Token: 0x040019EA RID: 6634
			IdleToCharging,
			// Token: 0x040019EB RID: 6635
			Charging,
			// Token: 0x040019EC RID: 6636
			Charged,
			// Token: 0x040019ED RID: 6637
			Finished
		}
	}
}
