﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F8 RID: 1016
	[RequireComponent(typeof(SceneExitController))]
	public class TeleporterInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider
	{
		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600166F RID: 5743 RVA: 0x00010C2D File Offset: 0x0000EE2D
		// (set) Token: 0x06001670 RID: 5744 RVA: 0x00010C35 File Offset: 0x0000EE35
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

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001671 RID: 5745 RVA: 0x00010C3E File Offset: 0x0000EE3E
		public bool isIdle
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Idle;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x00010C49 File Offset: 0x0000EE49
		public bool isIdleToCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.IdleToCharging;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001673 RID: 5747 RVA: 0x00010C54 File Offset: 0x0000EE54
		public bool isInFinalSequence
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Finished;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x00010C5F File Offset: 0x0000EE5F
		public bool isCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Charging;
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06001675 RID: 5749 RVA: 0x00010C6A File Offset: 0x0000EE6A
		public bool isCharged
		{
			get
			{
				return this.activationState >= TeleporterInteraction.ActivationState.Charged;
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001676 RID: 5750 RVA: 0x00010C78 File Offset: 0x0000EE78
		public float chargeFraction
		{
			get
			{
				return this.chargePercent * 0.01f;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06001677 RID: 5751 RVA: 0x00010C88 File Offset: 0x0000EE88
		// (set) Token: 0x06001678 RID: 5752 RVA: 0x00010C8F File Offset: 0x0000EE8F
		public static TeleporterInteraction instance { get; private set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06001679 RID: 5753 RVA: 0x00010C97 File Offset: 0x0000EE97
		// (set) Token: 0x0600167A RID: 5754 RVA: 0x00010C9F File Offset: 0x0000EE9F
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

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x0600167B RID: 5755 RVA: 0x00010CD6 File Offset: 0x0000EED6
		// (set) Token: 0x0600167C RID: 5756 RVA: 0x00010CDE File Offset: 0x0000EEDE
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

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x0600167D RID: 5757 RVA: 0x00010D15 File Offset: 0x0000EF15
		// (set) Token: 0x0600167E RID: 5758 RVA: 0x00010D1D File Offset: 0x0000EF1D
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

		// Token: 0x0600167F RID: 5759 RVA: 0x00010D54 File Offset: 0x0000EF54
		private void OnSyncShouldAttemptToSpawnShopPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnShopPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("ShopPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00010D85 File Offset: 0x0000EF85
		private void OnSyncShouldAttemptToSpawnGoldshoresPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnGoldshoresPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("GoldshoresPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00010DB6 File Offset: 0x0000EFB6
		private void OnSyncShouldAttemptToSpawnMSPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnMSPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("MSPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00077018 File Offset: 0x00075218
		private void Awake()
		{
			this.remainingChargeTimer = this.chargeDuration;
			this.monsterCheckTimer = 0f;
			this.childLocator = base.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
			this.bossShrineIndicator = this.childLocator.FindChild("BossShrineSymbol").gameObject;
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00010DE7 File Offset: 0x0000EFE7
		private void OnEnable()
		{
			TeleporterInteraction.instance = SingletonHelper.Assign<TeleporterInteraction>(TeleporterInteraction.instance, this);
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00010DF9 File Offset: 0x0000EFF9
		private void OnDisable()
		{
			TeleporterInteraction.instance = SingletonHelper.Unassign<TeleporterInteraction>(TeleporterInteraction.instance, this);
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00077070 File Offset: 0x00075270
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

		// Token: 0x06001686 RID: 5766 RVA: 0x00010E0B File Offset: 0x0000F00B
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

		// Token: 0x06001687 RID: 5767 RVA: 0x00010E37 File Offset: 0x0000F037
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

		// Token: 0x06001688 RID: 5768 RVA: 0x00077160 File Offset: 0x00075360
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

		// Token: 0x06001689 RID: 5769 RVA: 0x00010E6E File Offset: 0x0000F06E
		[ClientRpc]
		private void RpcClientOnActivated(GameObject activatorObject)
		{
			if (this.musicSource)
			{
				this.musicSource.Play();
			}
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x00010E88 File Offset: 0x0000F088
		private void UpdateMonstersClear()
		{
			this.monstersCleared = (BossGroup.GetTotalBossCount() == 0);
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x000771E4 File Offset: 0x000753E4
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

		// Token: 0x0600168C RID: 5772 RVA: 0x0000D94A File Offset: 0x0000BB4A
		private int GetMonsterCount()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00010E98 File Offset: 0x0000F098
		private float DiminishingReturns(int i)
		{
			return (1f - Mathf.Pow(0.5f, (float)i)) * 2f;
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00010EB2 File Offset: 0x0000F0B2
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

		// Token: 0x0600168F RID: 5775 RVA: 0x0007726C File Offset: 0x0007546C
		public bool IsInChargingRange(GameObject gameObject)
		{
			return (gameObject.transform.position - base.transform.position).sqrMagnitude <= this.clearRadius * this.clearRadius;
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00010EE7 File Offset: 0x0000F0E7
		public bool IsInChargingRange(CharacterBody characterBody)
		{
			return this.IsInChargingRange(characterBody.gameObject);
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x000772B0 File Offset: 0x000754B0
		public void FixedUpdate()
		{
			this.bossShrineIndicator.SetActive(this.activationState == TeleporterInteraction.ActivationState.Idle || this.showBossIndicator);
			if (this.previousActivationState != this.activationState)
			{
				this.OnStateChanged(this.previousActivationState, this.activationState);
			}
			this.previousActivationState = this.activationState;
			this.StateFixedUpdate();
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x0007730C File Offset: 0x0007550C
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

		// Token: 0x06001693 RID: 5779 RVA: 0x00077568 File Offset: 0x00075768
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

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06001694 RID: 5780 RVA: 0x000777A4 File Offset: 0x000759A4
		// (remove) Token: 0x06001695 RID: 5781 RVA: 0x000777D8 File Offset: 0x000759D8
		public static event Action<TeleporterInteraction> onTeleporterBeginChargingGlobal;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06001696 RID: 5782 RVA: 0x0007780C File Offset: 0x00075A0C
		// (remove) Token: 0x06001697 RID: 5783 RVA: 0x00077840 File Offset: 0x00075A40
		public static event Action<TeleporterInteraction> onTeleporterChargedGlobal;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06001698 RID: 5784 RVA: 0x00077874 File Offset: 0x00075A74
		// (remove) Token: 0x06001699 RID: 5785 RVA: 0x000778A8 File Offset: 0x00075AA8
		public static event Action<TeleporterInteraction> onTeleporterFinishGlobal;

		// Token: 0x0600169A RID: 5786 RVA: 0x000778DC File Offset: 0x00075ADC
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

		// Token: 0x0600169B RID: 5787 RVA: 0x00077928 File Offset: 0x00075B28
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

		// Token: 0x0600169C RID: 5788 RVA: 0x000779E4 File Offset: 0x00075BE4
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

		// Token: 0x0600169D RID: 5789 RVA: 0x00077A78 File Offset: 0x00075C78
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

		// Token: 0x0600169E RID: 5790 RVA: 0x00010C5F File Offset: 0x0000EE5F
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.activationState == TeleporterInteraction.ActivationState.Charging;
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00010EF5 File Offset: 0x0000F0F5
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/TimerHologramContent");
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00077B0C File Offset: 0x00075D0C
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			TimerHologramContent component = hologramContentObject.GetComponent<TimerHologramContent>();
			if (component)
			{
				component.displayValue = this.remainingChargeTimer;
			}
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x00010F01 File Offset: 0x0000F101
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

		// Token: 0x060016A2 RID: 5794 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x00077B34 File Offset: 0x00075D34
		// (set) Token: 0x060016A6 RID: 5798 RVA: 0x00010F39 File Offset: 0x0000F139
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

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060016A7 RID: 5799 RVA: 0x00077B48 File Offset: 0x00075D48
		// (set) Token: 0x060016A8 RID: 5800 RVA: 0x00010F4D File Offset: 0x0000F14D
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

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060016A9 RID: 5801 RVA: 0x00077B5C File Offset: 0x00075D5C
		// (set) Token: 0x060016AA RID: 5802 RVA: 0x00010F61 File Offset: 0x0000F161
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

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060016AB RID: 5803 RVA: 0x00077B70 File Offset: 0x00075D70
		// (set) Token: 0x060016AC RID: 5804 RVA: 0x00010F75 File Offset: 0x0000F175
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

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060016AD RID: 5805 RVA: 0x00077B84 File Offset: 0x00075D84
		// (set) Token: 0x060016AE RID: 5806 RVA: 0x00010FB4 File Offset: 0x0000F1B4
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

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060016AF RID: 5807 RVA: 0x00077B98 File Offset: 0x00075D98
		// (set) Token: 0x060016B0 RID: 5808 RVA: 0x00010FF3 File Offset: 0x0000F1F3
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

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060016B1 RID: 5809 RVA: 0x00077BAC File Offset: 0x00075DAC
		// (set) Token: 0x060016B2 RID: 5810 RVA: 0x00011032 File Offset: 0x0000F232
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

		// Token: 0x060016B3 RID: 5811 RVA: 0x00011046 File Offset: 0x0000F246
		protected static void InvokeRpcRpcClientOnActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientOnActivated called on server.");
				return;
			}
			((TeleporterInteraction)obj).RpcClientOnActivated(reader.ReadGameObject());
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x00077BC0 File Offset: 0x00075DC0
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

		// Token: 0x060016B5 RID: 5813 RVA: 0x0001106F File Offset: 0x0000F26F
		static TeleporterInteraction()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(TeleporterInteraction), TeleporterInteraction.kRpcRpcClientOnActivated, new NetworkBehaviour.CmdDelegate(TeleporterInteraction.InvokeRpcRpcClientOnActivated));
			NetworkCRC.RegisterBehaviour("TeleporterInteraction", 0);
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x00077C34 File Offset: 0x00075E34
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

		// Token: 0x060016B7 RID: 5815 RVA: 0x00077E1C File Offset: 0x0007601C
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

		// Token: 0x0400199B RID: 6555
		[SyncVar]
		private uint activationStateInternal;

		// Token: 0x0400199C RID: 6556
		private TeleporterInteraction.ActivationState previousActivationState;

		// Token: 0x0400199D RID: 6557
		[SyncVar]
		public bool locked;

		// Token: 0x0400199E RID: 6558
		public AudioSource musicSource;

		// Token: 0x0400199F RID: 6559
		[Tooltip("How long it takes for this teleporter to finish activating.")]
		public float chargeDuration = 90f;

		// Token: 0x040019A0 RID: 6560
		[Tooltip("The radius within which no monsters must exist for the teleporter event to conclude. Changing at runtime will not currently update the indicator scale.")]
		public float clearRadius = 40f;

		// Token: 0x040019A1 RID: 6561
		[Tooltip("An object which will be used to represent the clear radius.")]
		public GameObject clearRadiusIndicator;

		// Token: 0x040019A2 RID: 6562
		[HideInInspector]
		public float remainingChargeTimer;

		// Token: 0x040019A3 RID: 6563
		public int shrineBonusStacks;

		// Token: 0x040019A4 RID: 6564
		[SyncVar]
		private uint chargePercent;

		// Token: 0x040019A5 RID: 6565
		private float idleToChargingStopwatch;

		// Token: 0x040019A6 RID: 6566
		private float monsterCheckTimer;

		// Token: 0x040019A7 RID: 6567
		private GameObject teleporterPositionIndicator;

		// Token: 0x040019A8 RID: 6568
		public string beginContextString;

		// Token: 0x040019A9 RID: 6569
		public string exitContextString;

		// Token: 0x040019AA RID: 6570
		private ChildLocator childLocator;

		// Token: 0x040019AB RID: 6571
		public CombatDirector bonusDirector;

		// Token: 0x040019AC RID: 6572
		public CombatDirector bossDirector;

		// Token: 0x040019AE RID: 6574
		private GameObject bossShrineIndicator;

		// Token: 0x040019AF RID: 6575
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnShopPortal")]
		private bool _shouldAttemptToSpawnShopPortal;

		// Token: 0x040019B0 RID: 6576
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnGoldshoresPortal")]
		private bool _shouldAttemptToSpawnGoldshoresPortal;

		// Token: 0x040019B1 RID: 6577
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnMSPortal")]
		private bool _shouldAttemptToSpawnMSPortal;

		// Token: 0x040019B2 RID: 6578
		private Xoroshiro128Plus rng;

		// Token: 0x040019B3 RID: 6579
		private bool monstersCleared;

		// Token: 0x040019B4 RID: 6580
		[SyncVar]
		private bool showBossIndicator;

		// Token: 0x040019B8 RID: 6584
		public SpawnCard shopPortalSpawnCard;

		// Token: 0x040019B9 RID: 6585
		public SpawnCard goldshoresPortalSpawnCard;

		// Token: 0x040019BA RID: 6586
		public SpawnCard msPortalSpawnCard;

		// Token: 0x040019BB RID: 6587
		public float baseShopSpawnChance = 0.25f;

		// Token: 0x040019BC RID: 6588
		[Tooltip("The networked object which will be instantiated to lock purchasables.")]
		public GameObject lockPrefab;

		// Token: 0x040019BD RID: 6589
		private Coroutine chestLockCoroutine;

		// Token: 0x040019BE RID: 6590
		private static int kRpcRpcClientOnActivated = 1157394167;

		// Token: 0x020003F9 RID: 1017
		private enum ActivationState
		{
			// Token: 0x040019C0 RID: 6592
			Idle,
			// Token: 0x040019C1 RID: 6593
			IdleToCharging,
			// Token: 0x040019C2 RID: 6594
			Charging,
			// Token: 0x040019C3 RID: 6595
			Charged,
			// Token: 0x040019C4 RID: 6596
			Finished
		}
	}
}
