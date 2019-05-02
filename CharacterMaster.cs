using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200028E RID: 654
	[RequireComponent(typeof(Inventory))]
	[DisallowMultipleComponent]
	public class CharacterMaster : NetworkBehaviour
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0000A0E9 File Offset: 0x000082E9
		// (set) Token: 0x06000CE6 RID: 3302 RVA: 0x0000A0F1 File Offset: 0x000082F1
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x0000A0FA File Offset: 0x000082FA
		// (set) Token: 0x06000CE8 RID: 3304 RVA: 0x0000A102 File Offset: 0x00008302
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0000A10B File Offset: 0x0000830B
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0000A11E File Offset: 0x0000831E
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0000A12C File Offset: 0x0000832C
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
			base.OnStopAuthority();
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000CEC RID: 3308 RVA: 0x00052A8C File Offset: 0x00050C8C
		// (remove) Token: 0x06000CED RID: 3309 RVA: 0x00052AC4 File Offset: 0x00050CC4
		public event Action<CharacterBody> onBodyStart;

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000CEE RID: 3310 RVA: 0x0000A13A File Offset: 0x0000833A
		// (set) Token: 0x06000CEF RID: 3311 RVA: 0x0000A142 File Offset: 0x00008342
		public TeamIndex teamIndex
		{
			get
			{
				return this._teamIndex;
			}
			set
			{
				if (this._teamIndex == value)
				{
					return;
				}
				this._teamIndex = value;
				if (NetworkServer.active)
				{
					base.SetDirtyBit(8u);
				}
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000CF0 RID: 3312 RVA: 0x0000A163 File Offset: 0x00008363
		public static ReadOnlyCollection<CharacterMaster> readOnlyInstancesList
		{
			get
			{
				return CharacterMaster._readOnlyInstancesList;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000CF2 RID: 3314 RVA: 0x0000A173 File Offset: 0x00008373
		// (set) Token: 0x06000CF1 RID: 3313 RVA: 0x0000A16A File Offset: 0x0000836A
		public Inventory inventory { get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x0000A17B File Offset: 0x0000837B
		// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x0000A183 File Offset: 0x00008383
		private NetworkInstanceId bodyInstanceId
		{
			get
			{
				return this._bodyInstanceId;
			}
			set
			{
				if (value == this._bodyInstanceId)
				{
					return;
				}
				base.SetDirtyBit(1u);
				this._bodyInstanceId = value;
			}
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0000A1A2 File Offset: 0x000083A2
		private void OnSyncBodyInstanceId(NetworkInstanceId value)
		{
			this.resolvedBodyInstance = null;
			this.bodyResolved = (value == NetworkInstanceId.Invalid);
			this._bodyInstanceId = value;
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0000A1C3 File Offset: 0x000083C3
		// (set) Token: 0x06000CF7 RID: 3319 RVA: 0x00052AFC File Offset: 0x00050CFC
		private GameObject bodyInstanceObject
		{
			get
			{
				if (!this.bodyResolved)
				{
					this.resolvedBodyInstance = Util.FindNetworkObject(this.bodyInstanceId);
					if (this.resolvedBodyInstance)
					{
						this.bodyResolved = true;
					}
				}
				return this.resolvedBodyInstance;
			}
			set
			{
				NetworkInstanceId bodyInstanceId = NetworkInstanceId.Invalid;
				this.resolvedBodyInstance = null;
				this.bodyResolved = true;
				if (value)
				{
					NetworkIdentity component = value.GetComponent<NetworkIdentity>();
					if (component)
					{
						bodyInstanceId = component.netId;
						this.resolvedBodyInstance = value;
					}
				}
				this.bodyInstanceId = bodyInstanceId;
			}
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x0000A1F8 File Offset: 0x000083F8
		[Server]
		public void GiveExperience(ulong amount)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::GiveExperience(System.UInt64)' called on client");
				return;
			}
			TeamManager.instance.GiveTeamExperience(this.teamIndex, amount);
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000CF9 RID: 3321 RVA: 0x0000A220 File Offset: 0x00008420
		// (set) Token: 0x06000CFA RID: 3322 RVA: 0x0000A228 File Offset: 0x00008428
		public uint money
		{
			get
			{
				return this._money;
			}
			set
			{
				if (value == this._money)
				{
					return;
				}
				base.SetDirtyBit(2u);
				this._money = value;
			}
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0000A242 File Offset: 0x00008442
		public void GiveMoney(uint amount)
		{
			this.money += amount;
			StatManager.OnGoldCollected(this, (ulong)amount);
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000CFC RID: 3324 RVA: 0x0000A25A File Offset: 0x0000845A
		public float luck
		{
			get
			{
				if (this.inventory)
				{
					return (float)this.inventory.GetItemCount(ItemIndex.Clover);
				}
				return 0f;
			}
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00052B4C File Offset: 0x00050D4C
		[Server]
		public void AddDeployable(Deployable deployable, DeployableSlot slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::AddDeployable(RoR2.Deployable,RoR2.DeployableSlot)' called on client");
				return;
			}
			if (deployable.ownerMaster)
			{
				Debug.LogErrorFormat("Attempted to add deployable {0} which already belongs to master {1} to master {2}.", new object[]
				{
					deployable.gameObject,
					deployable.ownerMaster.gameObject,
					base.gameObject
				});
			}
			if (this.deployablesList == null)
			{
				this.deployablesList = new List<DeployableInfo>();
			}
			int num = 0;
			int num2 = 0;
			switch (slot)
			{
			case DeployableSlot.EngiMine:
				num2 = 10;
				break;
			case DeployableSlot.EngiTurret:
				num2 = 2;
				break;
			case DeployableSlot.BeetleGuardAlly:
				num2 = this.inventory.GetItemCount(ItemIndex.BeetleGland);
				break;
			case DeployableSlot.EngiBubbleShield:
				num2 = 1;
				break;
			}
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].slot == slot)
				{
					num++;
					if (num >= num2)
					{
						Deployable deployable2 = this.deployablesList[i].deployable;
						this.deployablesList.RemoveAt(i);
						deployable2.ownerMaster = null;
						deployable2.onUndeploy.Invoke();
					}
				}
			}
			this.deployablesList.Add(new DeployableInfo
			{
				deployable = deployable,
				slot = slot
			});
			deployable.ownerMaster = this;
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00052C84 File Offset: 0x00050E84
		[Server]
		public int GetDeployableCount(DeployableSlot slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Int32 RoR2.CharacterMaster::GetDeployableCount(RoR2.DeployableSlot)' called on client");
				return 0;
			}
			if (this.deployablesList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].slot == slot)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x00052CE4 File Offset: 0x00050EE4
		[Server]
		public void RemoveDeployable(Deployable deployable)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::RemoveDeployable(RoR2.Deployable)' called on client");
				return;
			}
			if (this.deployablesList == null || deployable.ownerMaster != this)
			{
				return;
			}
			deployable.ownerMaster = null;
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].deployable == deployable)
				{
					this.deployablesList.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x00052D64 File Offset: 0x00050F64
		[Server]
		public CharacterBody SpawnBody(GameObject bodyPrefab, Vector3 position, Quaternion rotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::SpawnBody(UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return null;
			}
			if (this.bodyInstanceObject)
			{
				Debug.LogError("Character cannot have more than one body at this time.");
				return null;
			}
			if (!bodyPrefab)
			{
				Debug.LogErrorFormat("Attempted to spawn body of character master {0} with no body prefab.", new object[]
				{
					base.gameObject
				});
			}
			if (!bodyPrefab.GetComponent<CharacterBody>())
			{
				Debug.LogErrorFormat("Attempted to spawn body of character master {0} with a body prefab that has no {1} component attached.", new object[]
				{
					base.gameObject,
					typeof(CharacterBody).Name
				});
			}
			bool flag = bodyPrefab.GetComponent<CharacterDirection>();
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(bodyPrefab, position, flag ? Quaternion.identity : rotation);
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			component.masterObject = base.gameObject;
			if (flag)
			{
				CharacterDirection component2 = gameObject.GetComponent<CharacterDirection>();
				float y = rotation.eulerAngles.y;
				component2.yaw = y;
			}
			NetworkConnection clientAuthorityOwner = base.GetComponent<NetworkIdentity>().clientAuthorityOwner;
			if (clientAuthorityOwner != null)
			{
				NetworkServer.SpawnWithClientAuthority(gameObject, clientAuthorityOwner);
			}
			else
			{
				NetworkServer.Spawn(gameObject);
			}
			this.bodyInstanceObject = gameObject;
			Run.instance.OnServerCharacterBodySpawned(component);
			return component;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0000A27D File Offset: 0x0000847D
		[Server]
		public void DestroyBody()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::DestroyBody()' called on client");
				return;
			}
			if (this.bodyInstanceObject != null)
			{
				UnityEngine.Object.Destroy(this.bodyInstanceObject);
				this.bodyInstanceObject = null;
			}
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0000A2B4 File Offset: 0x000084B4
		public GameObject GetBodyObject()
		{
			return this.bodyInstanceObject;
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x0000A2BC File Offset: 0x000084BC
		public bool alive
		{
			get
			{
				return this.bodyInstanceObject;
			}
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x00052E88 File Offset: 0x00051088
		public CharacterBody GetBody()
		{
			GameObject bodyObject = this.GetBodyObject();
			if (!bodyObject)
			{
				return null;
			}
			return bodyObject.GetComponent<CharacterBody>();
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0000A2C9 File Offset: 0x000084C9
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.inventory = base.GetComponent<Inventory>();
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0000A2E3 File Offset: 0x000084E3
		private void Start()
		{
			this.UpdateAuthority();
			if (this.spawnOnStart && NetworkServer.active)
			{
				this.SpawnBodyHere();
			}
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0000A300 File Offset: 0x00008500
		[Server]
		public void SpawnBodyHere()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::SpawnBodyHere()' called on client");
				return;
			}
			this.SpawnBody(this.bodyPrefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0000A33A File Offset: 0x0000853A
		private void OnEnable()
		{
			CharacterMaster.instancesList.Add(this);
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0000A347 File Offset: 0x00008547
		private void OnDisable()
		{
			CharacterMaster.instancesList.Remove(this);
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0000A355 File Offset: 0x00008555
		private void OnDestroy()
		{
			if (this.isBoss)
			{
				this.isBoss = false;
			}
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x00052EAC File Offset: 0x000510AC
		public void OnBodyStart(CharacterBody body)
		{
			this.preventGameOver = true;
			this.killerBodyIndex = -1;
			TeamComponent component = body.GetComponent<TeamComponent>();
			if (component)
			{
				component.teamIndex = this.teamIndex;
			}
			body.RecalculateStats();
			if (NetworkServer.active)
			{
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyStart(body);
				}
			}
			bool flag = false;
			PlayerCharacterMasterController component2 = base.GetComponent<PlayerCharacterMasterController>();
			if (component2)
			{
				if (component2.networkUserObject)
				{
					flag = component2.networkUserObject.GetComponent<NetworkIdentity>().isLocalPlayer;
				}
				component2.OnBodyStart();
			}
			if (flag)
			{
				GlobalEventManager.instance.OnLocalPlayerBodySpawn(body);
			}
			if (this.inventory.GetItemCount(ItemIndex.Ghost) > 0)
			{
				Util.PlaySound("Play_item_proc_ghostOnKill", body.gameObject);
			}
			if (NetworkServer.active)
			{
				HealthComponent component3 = body.GetComponent<HealthComponent>();
				if (component3)
				{
					component3.Networkhealth = component3.fullHealth;
				}
				this.UpdateBodyGodMode();
				this.StartLifeStopwatch();
				GlobalEventManager.instance.OnCharacterBodySpawn(body);
			}
			Action<CharacterBody> action = this.onBodyStart;
			if (action == null)
			{
				return;
			}
			action(body);
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x00052FC4 File Offset: 0x000511C4
		public void OnBodyDeath()
		{
			if (NetworkServer.active)
			{
				this.deathFootPosition = this.GetBody().footPosition;
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyDeath();
				}
				PlayerCharacterMasterController component = base.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					component.OnBodyDeath();
				}
				if (this.inventory.GetItemCount(ItemIndex.ExtraLife) > 0)
				{
					this.inventory.RemoveItem(ItemIndex.ExtraLife, 1);
					base.Invoke("RespawnExtraLife", 2f);
					base.Invoke("PlayExtraLifeSFX", 1f);
				}
				else
				{
					if (this.destroyOnBodyDeath)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					this.preventGameOver = false;
					this.preventRespawnUntilNextStageServer = true;
				}
				this.ResetLifeStopwatch();
			}
			UnityEvent unityEvent = this.onBodyDeath;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x00053098 File Offset: 0x00051298
		public void TrueKill()
		{
			int itemCount = this.inventory.GetItemCount(ItemIndex.ExtraLife);
			this.inventory.ResetItem(ItemIndex.ExtraLife);
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, itemCount);
			base.CancelInvoke("RespawnExtraLife");
			base.CancelInvoke("PlayExtraLifeSFX");
			CharacterBody body = this.GetBody();
			if (body)
			{
				body.healthComponent.Suicide(null);
			}
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x00053100 File Offset: 0x00051300
		private void PlayExtraLifeSFX()
		{
			GameObject bodyInstanceObject = this.bodyInstanceObject;
			if (bodyInstanceObject)
			{
				Util.PlaySound("Play_item_proc_extraLife", bodyInstanceObject);
			}
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x00053128 File Offset: 0x00051328
		public void RespawnExtraLife()
		{
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);
			this.Respawn(this.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
			this.GetBody().AddTimedBuff(BuffIndex.Immune, 3f);
			GameObject gameObject = Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
			if (this.bodyInstanceObject)
			{
				foreach (EntityStateMachine entityStateMachine in this.bodyInstanceObject.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
				if (gameObject)
				{
					EffectManager.instance.SpawnEffect(gameObject, new EffectData
					{
						origin = this.deathFootPosition,
						rotation = this.bodyInstanceObject.transform.rotation
					}, true);
				}
			}
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x000531FC File Offset: 0x000513FC
		public void OnBodyDamaged(DamageInfo damageInfo)
		{
			BaseAI[] components = base.GetComponents<BaseAI>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnBodyDamaged(damageInfo);
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x00053228 File Offset: 0x00051428
		public void OnBodyDestroyed()
		{
			if (NetworkServer.active)
			{
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyDestroyed();
				}
				this.PauseLifeStopwatch();
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0000A366 File Offset: 0x00008566
		// (set) Token: 0x06000D13 RID: 3347 RVA: 0x0000A36E File Offset: 0x0000856E
		private float internalSurvivalTime
		{
			get
			{
				return this._internalSurvivalTime;
			}
			set
			{
				if (value == this._internalSurvivalTime)
				{
					return;
				}
				base.SetDirtyBit(4u);
				this._internalSurvivalTime = value;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x0000A388 File Offset: 0x00008588
		public float currentLifeStopwatch
		{
			get
			{
				if (this.internalSurvivalTime <= 0f)
				{
					return -this.internalSurvivalTime;
				}
				if (Run.instance)
				{
					return Run.instance.GetRunStopwatch() - this.internalSurvivalTime;
				}
				return 0f;
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0000A3C2 File Offset: 0x000085C2
		[Server]
		private void StartLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::StartLifeStopwatch()' called on client");
				return;
			}
			if (this.internalSurvivalTime > 0f)
			{
				return;
			}
			this.internalSurvivalTime = Run.instance.GetRunStopwatch() - this.currentLifeStopwatch;
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0000A3FE File Offset: 0x000085FE
		[Server]
		private void PauseLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::PauseLifeStopwatch()' called on client");
				return;
			}
			if (this.internalSurvivalTime <= 0f)
			{
				return;
			}
			this.internalSurvivalTime = -this.currentLifeStopwatch;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0000A430 File Offset: 0x00008630
		[Server]
		private void ResetLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::ResetLifeStopwatch()' called on client");
				return;
			}
			this.internalSurvivalTime = 0f;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0000A452 File Offset: 0x00008652
		[Server]
		public int GetKillerBodyIndex()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Int32 RoR2.CharacterMaster::GetKillerBodyIndex()' called on client");
				return 0;
			}
			return this.killerBodyIndex;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x00053260 File Offset: 0x00051460
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				CharacterMaster victimMaster = damageReport.victimMaster;
				if (victimMaster)
				{
					victimMaster.killerBodyIndex = BodyCatalog.FindBodyIndex(damageReport.damageInfo.attacker);
				}
			};
			Stage.onServerStageBegin += delegate(Stage stage)
			{
				foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
				{
					characterMaster.preventRespawnUntilNextStageServer = false;
				}
			};
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0000A470 File Offset: 0x00008670
		[Command]
		public void CmdRespawn(string bodyName)
		{
			if (this.preventRespawnUntilNextStageServer)
			{
				return;
			}
			if (!string.IsNullOrEmpty(bodyName))
			{
				this.bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
			}
			if (Stage.instance)
			{
				Stage.instance.RespawnCharacter(this);
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x000532B8 File Offset: 0x000514B8
		[Server]
		public CharacterBody Respawn(Vector3 footPosition, Quaternion rotation, bool tryToGroundSafely = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::Respawn(UnityEngine.Vector3,UnityEngine.Quaternion,System.Boolean)' called on client");
				return null;
			}
			this.DestroyBody();
			if (this.bodyPrefab)
			{
				CharacterBody component = this.bodyPrefab.GetComponent<CharacterBody>();
				if (component)
				{
					Vector3 vector = footPosition;
					if (tryToGroundSafely)
					{
						Vector3 vector2 = vector;
						RaycastHit raycastHit = default(RaycastHit);
						Ray ray = new Ray(footPosition + Vector3.up * 1f, Vector3.down);
						float maxDistance = 3f;
						if (Physics.SphereCast(ray, component.radius, out raycastHit, maxDistance, LayerIndex.world.mask))
						{
							vector2.y += 1f - raycastHit.distance;
						}
						vector = vector2;
					}
					Vector3 position = new Vector3(vector.x, vector.y + Util.GetBodyPrefabFootOffset(this.bodyPrefab), vector.z);
					return this.SpawnBody(this.bodyPrefab, position, rotation);
				}
				Debug.LogErrorFormat("Trying to respawn as object {0} who has no Character Body!", new object[]
				{
					this.bodyPrefab
				});
			}
			return null;
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0000A4A6 File Offset: 0x000086A6
		private void ToggleGod()
		{
			this.godMode = !this.godMode;
			this.UpdateBodyGodMode();
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x000533D4 File Offset: 0x000515D4
		private void UpdateBodyGodMode()
		{
			if (this.bodyInstanceObject)
			{
				HealthComponent component = this.bodyInstanceObject.GetComponent<HealthComponent>();
				if (component)
				{
					component.godMode = this.godMode;
				}
			}
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x00053410 File Offset: 0x00051610
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 15u;
			}
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			bool flag3 = (num & 4u) > 0u;
			bool flag4 = (num & 8u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this._bodyInstanceId);
			}
			if (flag2)
			{
				writer.WritePackedUInt32(this._money);
			}
			if (flag3)
			{
				writer.Write(this._internalSurvivalTime);
			}
			if (flag4)
			{
				writer.Write(this.teamIndex);
			}
			return num > 0u;
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x00053490 File Offset: 0x00051690
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			bool flag3 = (b & 4) > 0;
			bool flag4 = (b & 8) > 0;
			if (flag)
			{
				NetworkInstanceId value = reader.ReadNetworkId();
				this.OnSyncBodyInstanceId(value);
			}
			if (flag2)
			{
				this._money = reader.ReadPackedUInt32();
			}
			if (flag3)
			{
				this._internalSurvivalTime = reader.ReadSingle();
			}
			if (flag4)
			{
				this.teamIndex = reader.ReadTeamIndex();
			}
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000534FC File Offset: 0x000516FC
		static CharacterMaster()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMaster), CharacterMaster.kCmdCmdRespawn, new NetworkBehaviour.CmdDelegate(CharacterMaster.InvokeCmdCmdRespawn));
			NetworkCRC.RegisterBehaviour("CharacterMaster", 0);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0000A4FB File Offset: 0x000086FB
		protected static void InvokeCmdCmdRespawn(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdRespawn called on client.");
				return;
			}
			((CharacterMaster)obj).CmdRespawn(reader.ReadString());
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x0005355C File Offset: 0x0005175C
		public void CallCmdRespawn(string bodyName)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdRespawn called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdRespawn(bodyName);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterMaster.kCmdCmdRespawn);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(bodyName);
			base.SendCommandInternal(networkWriter, 0, "CmdRespawn");
		}

		// Token: 0x04001108 RID: 4360
		[Tooltip("The prefab of this character's body.")]
		public GameObject bodyPrefab;

		// Token: 0x04001109 RID: 4361
		[Tooltip("Whether or not to spawn the body at the position of this manager object as soon as Start runs.")]
		public bool spawnOnStart;

		// Token: 0x0400110A RID: 4362
		[Tooltip("The team of the body.")]
		[SerializeField]
		[FormerlySerializedAs("teamIndex")]
		private TeamIndex _teamIndex;

		// Token: 0x0400110E RID: 4366
		public UnityEvent onBodyDeath;

		// Token: 0x0400110F RID: 4367
		[Tooltip("Whether or not to destroy this master when the body dies.")]
		public bool destroyOnBodyDeath = true;

		// Token: 0x04001110 RID: 4368
		private static List<CharacterMaster> instancesList = new List<CharacterMaster>();

		// Token: 0x04001111 RID: 4369
		private static ReadOnlyCollection<CharacterMaster> _readOnlyInstancesList = new ReadOnlyCollection<CharacterMaster>(CharacterMaster.instancesList);

		// Token: 0x04001113 RID: 4371
		private const uint bodyDirtyBit = 1u;

		// Token: 0x04001114 RID: 4372
		private const uint moneyDirtyBit = 2u;

		// Token: 0x04001115 RID: 4373
		private const uint survivalTimeDirtyBit = 4u;

		// Token: 0x04001116 RID: 4374
		private const uint teamDirtyBit = 8u;

		// Token: 0x04001117 RID: 4375
		private const uint allDirtyBits = 15u;

		// Token: 0x04001118 RID: 4376
		private NetworkInstanceId _bodyInstanceId = NetworkInstanceId.Invalid;

		// Token: 0x04001119 RID: 4377
		private GameObject resolvedBodyInstance;

		// Token: 0x0400111A RID: 4378
		private bool bodyResolved;

		// Token: 0x0400111B RID: 4379
		private uint _money;

		// Token: 0x0400111C RID: 4380
		public bool isBoss;

		// Token: 0x0400111D RID: 4381
		[NonSerialized]
		private List<DeployableInfo> deployablesList;

		// Token: 0x0400111E RID: 4382
		public bool preventGameOver = true;

		// Token: 0x0400111F RID: 4383
		private Vector3 deathFootPosition = Vector3.zero;

		// Token: 0x04001120 RID: 4384
		private Vector3 deathAimVector = Vector3.zero;

		// Token: 0x04001121 RID: 4385
		private const float respawnDelayDuration = 2f;

		// Token: 0x04001122 RID: 4386
		private float _internalSurvivalTime;

		// Token: 0x04001123 RID: 4387
		private int killerBodyIndex = -1;

		// Token: 0x04001124 RID: 4388
		private bool preventRespawnUntilNextStageServer;

		// Token: 0x04001125 RID: 4389
		private bool godMode;

		// Token: 0x04001126 RID: 4390
		private static int kCmdCmdRespawn = 1097984413;
	}
}
