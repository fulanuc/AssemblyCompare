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
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000CD8 RID: 3288 RVA: 0x0000A084 File Offset: 0x00008284
		// (set) Token: 0x06000CD9 RID: 3289 RVA: 0x0000A08C File Offset: 0x0000828C
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0000A095 File Offset: 0x00008295
		// (set) Token: 0x06000CDB RID: 3291 RVA: 0x0000A09D File Offset: 0x0000829D
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000CDC RID: 3292 RVA: 0x0000A0A6 File Offset: 0x000082A6
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0000A0B9 File Offset: 0x000082B9
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x0000A0C7 File Offset: 0x000082C7
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
			base.OnStopAuthority();
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000CDF RID: 3295 RVA: 0x000526E4 File Offset: 0x000508E4
		// (remove) Token: 0x06000CE0 RID: 3296 RVA: 0x0005271C File Offset: 0x0005091C
		public event Action<CharacterBody> onBodyStart;

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000CE1 RID: 3297 RVA: 0x0000A0D5 File Offset: 0x000082D5
		// (set) Token: 0x06000CE2 RID: 3298 RVA: 0x0000A0DD File Offset: 0x000082DD
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

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x0000A0FE File Offset: 0x000082FE
		public static ReadOnlyCollection<CharacterMaster> readOnlyInstancesList
		{
			get
			{
				return CharacterMaster._readOnlyInstancesList;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0000A10E File Offset: 0x0000830E
		// (set) Token: 0x06000CE4 RID: 3300 RVA: 0x0000A105 File Offset: 0x00008305
		public Inventory inventory { get; private set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x0000A116 File Offset: 0x00008316
		// (set) Token: 0x06000CE7 RID: 3303 RVA: 0x0000A11E File Offset: 0x0000831E
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

		// Token: 0x06000CE8 RID: 3304 RVA: 0x0000A13D File Offset: 0x0000833D
		private void OnSyncBodyInstanceId(NetworkInstanceId value)
		{
			this.resolvedBodyInstance = null;
			this.bodyResolved = (value == NetworkInstanceId.Invalid);
			this._bodyInstanceId = value;
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x0000A15E File Offset: 0x0000835E
		// (set) Token: 0x06000CEA RID: 3306 RVA: 0x00052754 File Offset: 0x00050954
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

		// Token: 0x06000CEB RID: 3307 RVA: 0x0000A193 File Offset: 0x00008393
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

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0000A1BB File Offset: 0x000083BB
		// (set) Token: 0x06000CED RID: 3309 RVA: 0x0000A1C3 File Offset: 0x000083C3
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

		// Token: 0x06000CEE RID: 3310 RVA: 0x0000A1DD File Offset: 0x000083DD
		public void GiveMoney(uint amount)
		{
			this.money += amount;
			StatManager.OnGoldCollected(this, (ulong)amount);
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0000A1F5 File Offset: 0x000083F5
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

		// Token: 0x06000CF0 RID: 3312 RVA: 0x000527A4 File Offset: 0x000509A4
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

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000528DC File Offset: 0x00050ADC
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

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0005293C File Offset: 0x00050B3C
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

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000529BC File Offset: 0x00050BBC
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

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0000A218 File Offset: 0x00008418
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

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0000A24F File Offset: 0x0000844F
		public GameObject GetBodyObject()
		{
			return this.bodyInstanceObject;
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0000A257 File Offset: 0x00008457
		public bool alive
		{
			get
			{
				return this.bodyInstanceObject;
			}
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00052AE0 File Offset: 0x00050CE0
		public CharacterBody GetBody()
		{
			GameObject bodyObject = this.GetBodyObject();
			if (!bodyObject)
			{
				return null;
			}
			return bodyObject.GetComponent<CharacterBody>();
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x0000A264 File Offset: 0x00008464
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.inventory = base.GetComponent<Inventory>();
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0000A27E File Offset: 0x0000847E
		private void Start()
		{
			this.UpdateAuthority();
			if (this.spawnOnStart && NetworkServer.active)
			{
				this.SpawnBodyHere();
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x0000A29B File Offset: 0x0000849B
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

		// Token: 0x06000CFB RID: 3323 RVA: 0x0000A2D5 File Offset: 0x000084D5
		private void OnEnable()
		{
			CharacterMaster.instancesList.Add(this);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0000A2E2 File Offset: 0x000084E2
		private void OnDisable()
		{
			CharacterMaster.instancesList.Remove(this);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0000A2F0 File Offset: 0x000084F0
		private void OnDestroy()
		{
			if (this.isBoss)
			{
				this.isBoss = false;
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00052B04 File Offset: 0x00050D04
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

		// Token: 0x06000CFF RID: 3327 RVA: 0x00052C1C File Offset: 0x00050E1C
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

		// Token: 0x06000D00 RID: 3328 RVA: 0x00052CF0 File Offset: 0x00050EF0
		public void TrueKill()
		{
			int itemCount = this.inventory.GetItemCount(ItemIndex.ExtraLife);
			this.inventory.ResetItem(ItemIndex.ExtraLife);
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, itemCount);
			CharacterBody body = this.GetBody();
			if (body)
			{
				body.healthComponent.Suicide(null);
			}
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x00052D44 File Offset: 0x00050F44
		private void PlayExtraLifeSFX()
		{
			GameObject bodyInstanceObject = this.bodyInstanceObject;
			if (bodyInstanceObject)
			{
				Util.PlaySound("Play_item_proc_extraLife", bodyInstanceObject);
			}
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x00052D6C File Offset: 0x00050F6C
		public void RespawnExtraLife()
		{
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);
			this.Respawn(this.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
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

		// Token: 0x06000D03 RID: 3331 RVA: 0x00052E40 File Offset: 0x00051040
		public void OnBodyDamaged(DamageInfo damageInfo)
		{
			BaseAI[] components = base.GetComponents<BaseAI>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnBodyDamaged(damageInfo);
			}
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x00052E6C File Offset: 0x0005106C
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

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x0000A301 File Offset: 0x00008501
		// (set) Token: 0x06000D06 RID: 3334 RVA: 0x0000A309 File Offset: 0x00008509
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

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000D07 RID: 3335 RVA: 0x0000A323 File Offset: 0x00008523
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
					return Run.instance.fixedTime - this.internalSurvivalTime;
				}
				return 0f;
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0000A35D File Offset: 0x0000855D
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
			this.internalSurvivalTime = Run.instance.fixedTime - this.currentLifeStopwatch;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0000A399 File Offset: 0x00008599
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

		// Token: 0x06000D0A RID: 3338 RVA: 0x0000A3CB File Offset: 0x000085CB
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

		// Token: 0x06000D0B RID: 3339 RVA: 0x0000A3ED File Offset: 0x000085ED
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

		// Token: 0x06000D0C RID: 3340 RVA: 0x00052EA4 File Offset: 0x000510A4
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

		// Token: 0x06000D0D RID: 3341 RVA: 0x0000A40B File Offset: 0x0000860B
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

		// Token: 0x06000D0E RID: 3342 RVA: 0x00052EFC File Offset: 0x000510FC
		[Server]
		public CharacterBody Respawn(Vector3 footPosition, Quaternion rotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::Respawn(UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return null;
			}
			this.DestroyBody();
			if (this.bodyPrefab)
			{
				Vector3 position = footPosition;
				position.y += Util.GetBodyPrefabFootOffset(this.bodyPrefab);
				return this.SpawnBody(this.bodyPrefab, position, rotation);
			}
			return null;
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0000A441 File Offset: 0x00008641
		private void ToggleGod()
		{
			this.godMode = !this.godMode;
			this.UpdateBodyGodMode();
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x00052F68 File Offset: 0x00051168
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

		// Token: 0x06000D11 RID: 3345 RVA: 0x00052FA4 File Offset: 0x000511A4
		[ConCommand(commandName = "god", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Toggles god mode on the sender.")]
		private static void CCGod(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					component.ToggleGod();
					Debug.LogFormat("godmode {0}", new object[]
					{
						component.godMode ? "ON" : "OFF"
					});
				}
			}
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x00053004 File Offset: 0x00051204
		[ConCommand(commandName = "zoom", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Gives a bunch of items to help travel the map.")]
		private static void CCZoom(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					component.GiveItem(ItemIndex.Hoof, 30);
					component.GiveItem(ItemIndex.Feather, 100);
				}
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00053048 File Offset: 0x00051248
		[ConCommand(commandName = "unzoom", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Removes the effects of zoom")]
		private static void CCUnzoom(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					component.GiveItem(ItemIndex.Hoof, -30);
					component.GiveItem(ItemIndex.Feather, -100);
				}
			}
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0005308C File Offset: 0x0005128C
		[ConCommand(commandName = "give_random_items", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Gives a random set of items. It will give approximately 80% white, 19% green, 1% red.")]
		private static void CCGiveRandomItems(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					try
					{
						int num;
						TextSerialization.TryParseInvariant(args[0], out num);
						if (num > 0)
						{
							WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
							weightedSelection.AddChoice(Run.instance.availableTier1DropList, 80f);
							weightedSelection.AddChoice(Run.instance.availableTier2DropList, 19f);
							weightedSelection.AddChoice(Run.instance.availableTier3DropList, 1f);
							for (int i = 0; i < num; i++)
							{
								List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
								component.GiveItem(list[UnityEngine.Random.Range(0, list.Count)].itemIndex, 1);
							}
						}
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0005317C File Offset: 0x0005137C
		[ConCommand(commandName = "give_item", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Gives the named item to the sender. Second argument can specify stack.")]
		private static void CCGiveItem(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					try
					{
						int count = 1;
						if (args.Count > 1)
						{
							TextSerialization.TryParseInvariant(args[1], out count);
						}
						component.GiveItem((ItemIndex)Enum.Parse(typeof(ItemIndex), args[0]), count);
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0005320C File Offset: 0x0005140C
		[ConCommand(commandName = "inventory_clear", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Clears the sender's inventory.")]
		private static void CCClearItems(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
					{
						component.ResetItem(itemIndex);
					}
					component.SetEquipmentIndex(EquipmentIndex.None);
				}
			}
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x00053264 File Offset: 0x00051464
		[ConCommand(commandName = "give_equipment", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Gives the named equipment to the sender.")]
		private static void CCGiveEquipment(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			if (args.senderMasterObject)
			{
				Inventory component = args.senderMasterObject.GetComponent<Inventory>();
				if (component)
				{
					try
					{
						component.SetEquipmentIndex((EquipmentIndex)Enum.Parse(typeof(EquipmentIndex), args[0]));
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x000532D8 File Offset: 0x000514D8
		[ConCommand(commandName = "give_money", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Gives the specified amount of money to the sender.")]
		private static void CCGiveMoney(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			if (args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					try
					{
						int num = 1;
						if (args.Count > 0)
						{
							TextSerialization.TryParseInvariant(args[0], out num);
						}
						component.money += (uint)num;
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x00053354 File Offset: 0x00051554
		[ConCommand(commandName = "set_team", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Sets the team of the sender to the one specified.")]
		private static void CCSetTeam(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			TeamIndex teamIndex;
			if (Enum.TryParse<TeamIndex>(args[0], true, out teamIndex) && args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					component.teamIndex = teamIndex;
					if (component.bodyInstanceObject)
					{
						TeamComponent component2 = component.bodyInstanceObject.GetComponent<TeamComponent>();
						if (component2)
						{
							component2.teamIndex = teamIndex;
						}
					}
				}
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000533D0 File Offset: 0x000515D0
		[ConCommand(commandName = "kill", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Kills the character controlled by the command sender (you).")]
		private static void CCKill(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					GameObject bodyObject = component.GetBodyObject();
					if (bodyObject)
					{
						HealthComponent component2 = bodyObject.GetComponent<HealthComponent>();
						if (component2)
						{
							component2.Suicide(null);
						}
					}
				}
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x00053428 File Offset: 0x00051628
		[ConCommand(commandName = "respawn", flags = ConVarFlags.Cheat, helpText = "Respawns a character with the specified body controlled by the command sender (you).")]
		private static void CCRespawn(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					string text = (args.Count > 0) ? args[0] : component.bodyPrefab.name;
					int num = BodyCatalog.FindBodyIndexCaseInsensitive(text);
					if (num != -1)
					{
						text = BodyCatalog.GetBodyPrefab(num).name;
					}
					Debug.LogFormat("Spawning as {0}...", new object[]
					{
						text
					});
					component.CallCmdRespawn(text);
				}
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x000534AC File Offset: 0x000516AC
		[ConCommand(commandName = "create_master", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Spawns the named master where the sender is looking.")]
		private static void CCCreatePickup(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			GameObject gameObject = Resources.Load<GameObject>(string.Format("Prefabs/CharacterMasters/{0}", args[0]));
			if (!gameObject)
			{
				return;
			}
			GameObject senderMasterObject = args.senderMasterObject;
			if (!senderMasterObject)
			{
				return;
			}
			CharacterMaster component = senderMasterObject.GetComponent<CharacterMaster>();
			if (!component)
			{
				return;
			}
			CharacterBody body = component.GetBody();
			if (!body)
			{
				return;
			}
			InputBankTest component2 = body.GetComponent<InputBankTest>();
			if (!component2)
			{
				return;
			}
			Ray ray = new Ray(component2.aimOrigin, component2.aimDirection);
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(body.gameObject, ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, raycastHit.point, Quaternion.identity);
				NetworkServer.Spawn(gameObject2);
				gameObject2.GetComponent<CharacterMaster>().SpawnBodyHere();
			}
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x00053584 File Offset: 0x00051784
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

		// Token: 0x06000D1E RID: 3358 RVA: 0x00053604 File Offset: 0x00051804
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

		// Token: 0x06000D20 RID: 3360 RVA: 0x00053670 File Offset: 0x00051870
		static CharacterMaster()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMaster), CharacterMaster.kCmdCmdRespawn, new NetworkBehaviour.CmdDelegate(CharacterMaster.InvokeCmdCmdRespawn));
			NetworkCRC.RegisterBehaviour("CharacterMaster", 0);
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0000A496 File Offset: 0x00008696
		protected static void InvokeCmdCmdRespawn(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdRespawn called on client.");
				return;
			}
			((CharacterMaster)obj).CmdRespawn(reader.ReadString());
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x000536D0 File Offset: 0x000518D0
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

		// Token: 0x040010FD RID: 4349
		[Tooltip("The prefab of this character's body.")]
		public GameObject bodyPrefab;

		// Token: 0x040010FE RID: 4350
		[Tooltip("Whether or not to spawn the body at the position of this manager object as soon as Start runs.")]
		public bool spawnOnStart;

		// Token: 0x040010FF RID: 4351
		[SerializeField]
		[Tooltip("The team of the body.")]
		[FormerlySerializedAs("teamIndex")]
		private TeamIndex _teamIndex;

		// Token: 0x04001103 RID: 4355
		public UnityEvent onBodyDeath;

		// Token: 0x04001104 RID: 4356
		[Tooltip("Whether or not to destroy this master when the body dies.")]
		public bool destroyOnBodyDeath = true;

		// Token: 0x04001105 RID: 4357
		private static List<CharacterMaster> instancesList = new List<CharacterMaster>();

		// Token: 0x04001106 RID: 4358
		private static ReadOnlyCollection<CharacterMaster> _readOnlyInstancesList = new ReadOnlyCollection<CharacterMaster>(CharacterMaster.instancesList);

		// Token: 0x04001108 RID: 4360
		private const uint bodyDirtyBit = 1u;

		// Token: 0x04001109 RID: 4361
		private const uint moneyDirtyBit = 2u;

		// Token: 0x0400110A RID: 4362
		private const uint survivalTimeDirtyBit = 4u;

		// Token: 0x0400110B RID: 4363
		private const uint teamDirtyBit = 8u;

		// Token: 0x0400110C RID: 4364
		private const uint allDirtyBits = 15u;

		// Token: 0x0400110D RID: 4365
		private NetworkInstanceId _bodyInstanceId = NetworkInstanceId.Invalid;

		// Token: 0x0400110E RID: 4366
		private GameObject resolvedBodyInstance;

		// Token: 0x0400110F RID: 4367
		private bool bodyResolved;

		// Token: 0x04001110 RID: 4368
		private uint _money;

		// Token: 0x04001111 RID: 4369
		public bool isBoss;

		// Token: 0x04001112 RID: 4370
		[NonSerialized]
		private List<DeployableInfo> deployablesList;

		// Token: 0x04001113 RID: 4371
		public bool preventGameOver = true;

		// Token: 0x04001114 RID: 4372
		private Vector3 deathFootPosition = Vector3.zero;

		// Token: 0x04001115 RID: 4373
		private Vector3 deathAimVector = Vector3.zero;

		// Token: 0x04001116 RID: 4374
		private const float respawnDelayDuration = 2f;

		// Token: 0x04001117 RID: 4375
		private float _internalSurvivalTime;

		// Token: 0x04001118 RID: 4376
		private int killerBodyIndex = -1;

		// Token: 0x04001119 RID: 4377
		private bool preventRespawnUntilNextStageServer;

		// Token: 0x0400111A RID: 4378
		private bool godMode;

		// Token: 0x0400111B RID: 4379
		private static int kCmdCmdRespawn = 1097984413;
	}
}
