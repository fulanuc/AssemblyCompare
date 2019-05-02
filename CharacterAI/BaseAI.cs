using System;
using System.Linq;
using EntityStates;
using JetBrains.Annotations;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace RoR2.CharacterAI
{
	// Token: 0x020005A9 RID: 1449
	[RequireComponent(typeof(CharacterMaster))]
	public class BaseAI : MonoBehaviour
	{
		// Token: 0x060020B7 RID: 8375 RVA: 0x0009D8A0 File Offset: 0x0009BAA0
		private void Awake()
		{
			this.targetRefreshTimer = 0.5f;
			this.master = base.GetComponent<CharacterMaster>();
			this.stateMachine = base.GetComponent<EntityStateMachine>();
			if (this.stateMachine)
			{
				this.stateMachine.SetNextState(EntityState.Instantiate(this.scanState));
			}
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.skillDrivers = base.GetComponents<AISkillDriver>();
			this.currentEnemy = new BaseAI.Target(this);
			this.leader = new BaseAI.Target(this);
			this.buddy = new BaseAI.Target(this);
		}

		// Token: 0x060020B8 RID: 8376 RVA: 0x0009D930 File Offset: 0x0009BB30
		public void Start()
		{
			if (!Util.HasEffectiveAuthority(this.networkIdentity))
			{
				base.enabled = false;
				if (this.stateMachine)
				{
					this.stateMachine.enabled = false;
				}
			}
			if (NetworkServer.active)
			{
				this.skillDriverUpdateTimer = UnityEngine.Random.value;
			}
		}

		// Token: 0x060020B9 RID: 8377 RVA: 0x0009D97C File Offset: 0x0009BB7C
		public void FixedUpdate()
		{
			if (this.drawAIPath)
			{
				this.DebugDrawPath(Color.red, Time.fixedDeltaTime);
			}
			this.enemyAttention -= Time.fixedDeltaTime;
			if (this.currentEnemy.characterBody && this.body && this.currentEnemy.characterBody.GetVisibilityLevel(this.body) < VisibilityLevel.Revealed)
			{
				this.currentEnemy.Reset();
			}
			if (this.pendingPath != null && this.pendingPath.status == PathTask.TaskStatus.Complete)
			{
				this.pathFollower.SetPath(this.pendingPath.path);
				this.pendingPath.path.Dispose();
				this.pendingPath = null;
			}
			if (this.body)
			{
				this.targetRefreshTimer -= Time.fixedDeltaTime;
				this.skillDriverUpdateTimer -= Time.fixedDeltaTime;
				if (this.skillDriverUpdateTimer <= 0f)
				{
					if (this.skillDriverEvaluation.dominantSkillDriver)
					{
						this.selectedSkilldriverName = this.skillDriverEvaluation.dominantSkillDriver.customName;
						if (this.skillDriverEvaluation.dominantSkillDriver.resetCurrentEnemyOnNextDriverSelection)
						{
							this.currentEnemy.Reset();
							this.targetRefreshTimer = 0f;
						}
					}
					if (!this.currentEnemy.gameObject && this.targetRefreshTimer <= 0f)
					{
						this.targetRefreshTimer = 0.5f;
						this.enemySearch.viewer = this.body;
						this.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
						this.enemySearch.teamMaskFilter.RemoveTeam(this.master.teamIndex);
						this.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
						this.enemySearch.minDistanceFilter = 0f;
						this.enemySearch.maxDistanceFilter = float.PositiveInfinity;
						this.enemySearch.searchOrigin = this.bodyInputBank.aimOrigin;
						this.enemySearch.searchDirection = this.bodyInputBank.aimDirection;
						this.enemySearch.maxAngleFilter = (this.fullVision ? 180f : 90f);
						this.enemySearch.filterByLoS = true;
						this.enemySearch.RefreshCandidates();
						HurtBox hurtBox = this.enemySearch.GetResults().FirstOrDefault<HurtBox>();
						if (hurtBox && hurtBox.healthComponent)
						{
							this.currentEnemy.gameObject = hurtBox.healthComponent.gameObject;
							this.currentEnemy.bestHurtBox = hurtBox;
						}
						if (this.currentEnemy.gameObject)
						{
							this.enemyAttention = this.enemyAttentionDuration;
						}
					}
					this.skillDriverEvaluation = this.EvaluateSkillDrivers();
					if (this.skillDriverEvaluation.dominantSkillDriver && this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride >= 0f)
					{
						this.skillDriverUpdateTimer = this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride;
					}
					else
					{
						this.skillDriverUpdateTimer = UnityEngine.Random.Range(0.166666672f, 0.2f);
					}
				}
			}
			if (this.bodyInputBank)
			{
				if (this.skillDriverEvaluation.dominantSkillDriver)
				{
					AISkillDriver.AimType aimType = this.skillDriverEvaluation.dominantSkillDriver.aimType;
					if (aimType != AISkillDriver.AimType.None)
					{
						BaseAI.Target target = null;
						switch (aimType)
						{
						case AISkillDriver.AimType.AtMoveTarget:
							target = this.skillDriverEvaluation.target;
							break;
						case AISkillDriver.AimType.AtCurrentEnemy:
							target = this.currentEnemy;
							break;
						case AISkillDriver.AimType.AtCurrentLeader:
							target = this.leader;
							break;
						}
						if (target != null)
						{
							Vector3 a;
							if (target.GetBullseyePosition(out a))
							{
								this.desiredAimDirection = (a - this.bodyInputBank.aimOrigin).normalized;
							}
						}
						else
						{
							if (this.bodyInputBank.moveVector != Vector3.zero)
							{
								this.desiredAimDirection = this.bodyInputBank.moveVector;
							}
							this.bodyInputBank.sprint.PushState(this.skillDriverEvaluation.dominantSkillDriver.shouldSprint);
						}
					}
				}
				Vector3 aimDirection = this.bodyInputBank.aimDirection;
				Vector3 eulerAngles = Util.QuaternionSafeLookRotation(this.desiredAimDirection).eulerAngles;
				Vector3 eulerAngles2 = Util.QuaternionSafeLookRotation(aimDirection).eulerAngles;
				float fixedDeltaTime = Time.fixedDeltaTime;
				float x = Mathf.SmoothDampAngle(eulerAngles2.x, eulerAngles.x, ref this.aimVelocity.x, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				float y = Mathf.SmoothDampAngle(eulerAngles2.y, eulerAngles.y, ref this.aimVelocity.y, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				float z = Mathf.SmoothDampAngle(eulerAngles2.z, eulerAngles.z, ref this.aimVelocity.z, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				this.bodyInputBank.aimDirection = Quaternion.Euler(x, y, z) * Vector3.forward;
			}
			this.debugEnemyHurtBox = this.currentEnemy.bestHurtBox;
		}

		// Token: 0x060020BA RID: 8378 RVA: 0x0009DE68 File Offset: 0x0009C068
		public virtual void OnBodyStart(CharacterBody newBody)
		{
			this.body = newBody;
			this.bodyTransform = newBody.transform;
			this.bodyCharacterDirection = newBody.GetComponent<CharacterDirection>();
			this.bodyCharacterMotor = newBody.GetComponent<CharacterMotor>();
			this.bodyInputBank = newBody.GetComponent<InputBankTest>();
			this.bodyHealthComponent = newBody.GetComponent<HealthComponent>();
			this.bodySkillLocator = newBody.GetComponent<SkillLocator>();
			this.localNavigator.SetBody(newBody);
			base.enabled = true;
			if (this.stateMachine && Util.HasEffectiveAuthority(this.networkIdentity))
			{
				this.stateMachine.enabled = true;
				this.stateMachine.SetNextState(EntityState.Instantiate(this.scanState));
			}
			if (this.bodyInputBank)
			{
				this.desiredAimDirection = this.bodyInputBank.aimDirection;
			}
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x00017E19 File Offset: 0x00016019
		public virtual void OnBodyDeath()
		{
			this.OnBodyLost();
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x00017E19 File Offset: 0x00016019
		public virtual void OnBodyDestroyed()
		{
			this.OnBodyLost();
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x0009DF34 File Offset: 0x0009C134
		public virtual void OnBodyLost()
		{
			if (this.body)
			{
				base.enabled = false;
				this.body = null;
				this.bodyTransform = null;
				this.bodyCharacterDirection = null;
				this.bodyCharacterMotor = null;
				this.bodyInputBank = null;
				this.bodyHealthComponent = null;
				this.bodySkillLocator = null;
				this.localNavigator.SetBody(null);
				if (this.stateMachine)
				{
					this.stateMachine.enabled = false;
					this.stateMachine.SetState(new Idle());
				}
			}
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x0009DFBC File Offset: 0x0009C1BC
		public virtual void OnBodyDamaged(DamageInfo damageInfo)
		{
			if (!damageInfo.attacker)
			{
				return;
			}
			if (!this.body)
			{
				return;
			}
			if ((!this.currentEnemy.gameObject || this.enemyAttention <= 0f) && damageInfo.attacker != this.body.gameObject)
			{
				this.currentEnemy.gameObject = damageInfo.attacker;
				this.enemyAttention = this.enemyAttentionDuration;
			}
		}

		// Token: 0x060020BF RID: 8383 RVA: 0x00017E21 File Offset: 0x00016021
		private void UpdateTargets()
		{
			this.currentEnemy.Update();
			this.leader.Update();
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x0009E03C File Offset: 0x0009C23C
		public virtual bool HasLOS(Vector3 start, Vector3 end)
		{
			RaycastHit raycastHit;
			return !Physics.Raycast(new Ray
			{
				origin = start,
				direction = end - start
			}, out raycastHit, Vector3.Magnitude(end - start), LayerIndex.world.mask);
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x0009E090 File Offset: 0x0009C290
		public virtual bool HasLOS(Vector3 end)
		{
			if (!this.bodyInputBank)
			{
				return false;
			}
			Vector3 aimOrigin = this.bodyInputBank.aimOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(new Ray
			{
				origin = aimOrigin,
				direction = end - aimOrigin
			}, out raycastHit, Vector3.Magnitude(end - aimOrigin), LayerIndex.world.mask);
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x0009E100 File Offset: 0x0009C300
		private NodeGraph.PathRequest GeneratePathRequest(Vector3 endPos)
		{
			Vector3 position = this.bodyTransform.position;
			if (this.bodyCharacterMotor)
			{
				position.y -= this.bodyCharacterMotor.capsuleHeight * 0.5f;
			}
			return new NodeGraph.PathRequest
			{
				startPos = position,
				endPos = endPos,
				maxJumpHeight = this.body.maxJumpHeight,
				maxSpeed = this.body.moveSpeed,
				hullClassification = this.body.hullClassification,
				path = new Path(this.GetNodeGraph())
			};
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x00017E39 File Offset: 0x00016039
		public NodeGraph GetNodeGraph()
		{
			return SceneInfo.instance.GetNodeGraph(this.nodegraphType);
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x0009E19C File Offset: 0x0009C39C
		public void RefreshPath(Vector3 startVec, Vector3 endVec)
		{
			NodeGraph.PathRequest pathRequest = this.GeneratePathRequest(endVec);
			NodeGraph nodeGraph = this.GetNodeGraph();
			if (nodeGraph)
			{
				this.pendingPath = nodeGraph.ComputePath(pathRequest);
			}
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x00017E4B File Offset: 0x0001604B
		public void DebugDrawPath(Color color, float duration)
		{
			this.pathFollower.DebugDrawPath(color, duration);
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x0009E1D0 File Offset: 0x0009C3D0
		private static bool CheckLoS(Vector3 start, Vector3 end)
		{
			Vector3 direction = end - start;
			RaycastHit raycastHit;
			return !Physics.Raycast(start, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x0009E20C File Offset: 0x0009C40C
		public HurtBox GetBestHurtBox(GameObject target)
		{
			CharacterBody component = target.GetComponent<CharacterBody>();
			HurtBoxGroup hurtBoxGroup = (component != null) ? component.hurtBoxGroup : null;
			if (hurtBoxGroup && hurtBoxGroup.bullseyeCount > 1 && this.bodyInputBank)
			{
				Vector3 aimOrigin = this.bodyInputBank.aimOrigin;
				HurtBox hurtBox = null;
				float num = float.PositiveInfinity;
				foreach (HurtBox hurtBox2 in hurtBoxGroup.hurtBoxes)
				{
					if (hurtBox2.isBullseye)
					{
						Vector3 position = hurtBox2.transform.position;
						if (BaseAI.CheckLoS(aimOrigin, hurtBox2.transform.position))
						{
							float sqrMagnitude = (position - aimOrigin).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
								hurtBox = hurtBox2;
							}
						}
					}
				}
				if (hurtBox)
				{
					return hurtBox;
				}
			}
			return Util.FindBodyMainHurtBox(target);
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x0009E2E4 File Offset: 0x0009C4E4
		public bool GameObjectPassesSkillDriverFilters(BaseAI.Target target, AISkillDriver skillDriver, out float separationSqrMagnitude)
		{
			separationSqrMagnitude = 0f;
			if (!target.gameObject)
			{
				return false;
			}
			float num = 1f;
			if (target.healthComponent)
			{
				num = target.healthComponent.combinedHealthFraction;
			}
			if (num < skillDriver.minTargetHealthFraction || num > skillDriver.maxTargetHealthFraction)
			{
				return false;
			}
			float num2 = 0f;
			if (this.body)
			{
				num2 = this.body.radius;
			}
			float num3 = 0f;
			if (target.characterBody)
			{
				num3 = target.characterBody.radius;
			}
			Vector3 b = this.bodyInputBank ? this.bodyInputBank.aimOrigin : this.bodyTransform.position;
			Vector3 a;
			target.GetBullseyePosition(out a);
			float sqrMagnitude = (a - b).sqrMagnitude;
			separationSqrMagnitude = sqrMagnitude - num3 * num3 - num2 * num2;
			return separationSqrMagnitude >= skillDriver.minDistanceSqr && separationSqrMagnitude <= skillDriver.maxDistanceSqr && (!skillDriver.selectionRequiresTargetLoS || target.hasLoS);
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x0009E3F0 File Offset: 0x0009C5F0
		public BaseAI.SkillDriverEvaluation EvaluateSkillDrivers()
		{
			this.UpdateTargets();
			BaseAI.SkillDriverEvaluation result = default(BaseAI.SkillDriverEvaluation);
			float num = 1f;
			if (this.bodyHealthComponent)
			{
				num = this.bodyHealthComponent.combinedHealthFraction;
			}
			float positiveInfinity = float.PositiveInfinity;
			if (this.bodySkillLocator)
			{
				for (int i = 0; i < this.skillDrivers.Length; i++)
				{
					AISkillDriver aiskillDriver = this.skillDrivers[i];
					if (!aiskillDriver.noRepeat || !(this.skillDriverEvaluation.dominantSkillDriver == aiskillDriver))
					{
						BaseAI.Target target = null;
						if (aiskillDriver.skillSlot != SkillSlot.None && aiskillDriver.requireSkillReady)
						{
							GenericSkill skill = this.bodySkillLocator.GetSkill(aiskillDriver.skillSlot);
							if (!skill || !skill.CanExecute())
							{
								goto IL_2B6;
							}
						}
						if (aiskillDriver.minUserHealthFraction <= num && aiskillDriver.maxUserHealthFraction >= num)
						{
							switch (aiskillDriver.moveTargetType)
							{
							case AISkillDriver.TargetType.CurrentEnemy:
								if (this.GameObjectPassesSkillDriverFilters(this.currentEnemy, aiskillDriver, out positiveInfinity))
								{
									target = this.currentEnemy;
								}
								break;
							case AISkillDriver.TargetType.NearestFriendlyInSkillRange:
								if (this.bodyInputBank)
								{
									this.buddySearch.teamMaskFilter = TeamMask.none;
									this.buddySearch.teamMaskFilter.AddTeam(this.master.teamIndex);
									this.buddySearch.sortMode = BullseyeSearch.SortMode.Distance;
									this.buddySearch.minDistanceFilter = aiskillDriver.minDistanceSqr;
									this.buddySearch.maxDistanceFilter = aiskillDriver.maxDistance;
									this.buddySearch.searchOrigin = this.bodyInputBank.aimOrigin;
									this.buddySearch.searchDirection = this.bodyInputBank.aimDirection;
									this.buddySearch.maxAngleFilter = 180f;
									this.buddySearch.filterByLoS = aiskillDriver.activationRequiresTargetLoS;
									this.buddySearch.RefreshCandidates();
									if (this.body)
									{
										this.buddySearch.FilterOutGameObject(this.body.gameObject);
									}
									this.buddySearch.FilterCandidatesByHealthFraction(aiskillDriver.minTargetHealthFraction, aiskillDriver.maxTargetHealthFraction);
									HurtBox hurtBox = this.buddySearch.GetResults().FirstOrDefault<HurtBox>();
									if (hurtBox && hurtBox.healthComponent)
									{
										this.buddy.gameObject = hurtBox.healthComponent.gameObject;
										this.buddy.bestHurtBox = hurtBox;
									}
									if (this.GameObjectPassesSkillDriverFilters(this.buddy, aiskillDriver, out positiveInfinity))
									{
										target = this.buddy;
									}
								}
								break;
							case AISkillDriver.TargetType.CurrentLeader:
								if (this.GameObjectPassesSkillDriverFilters(this.leader, aiskillDriver, out positiveInfinity))
								{
									target = this.leader;
								}
								break;
							}
							if (target != null)
							{
								result.dominantSkillDriver = aiskillDriver;
								result.target = target;
								result.separationSqrMagnitude = positiveInfinity;
								break;
							}
						}
					}
					IL_2B6:;
				}
			}
			return result;
		}

		// Token: 0x040022C9 RID: 8905
		protected CharacterMaster master;

		// Token: 0x040022CA RID: 8906
		protected CharacterBody body;

		// Token: 0x040022CB RID: 8907
		protected Transform bodyTransform;

		// Token: 0x040022CC RID: 8908
		protected CharacterDirection bodyCharacterDirection;

		// Token: 0x040022CD RID: 8909
		protected CharacterMotor bodyCharacterMotor;

		// Token: 0x040022CE RID: 8910
		protected InputBankTest bodyInputBank;

		// Token: 0x040022CF RID: 8911
		protected HealthComponent bodyHealthComponent;

		// Token: 0x040022D0 RID: 8912
		protected SkillLocator bodySkillLocator;

		// Token: 0x040022D1 RID: 8913
		protected NetworkIdentity networkIdentity;

		// Token: 0x040022D2 RID: 8914
		protected AISkillDriver[] skillDrivers;

		// Token: 0x040022D3 RID: 8915
		[Tooltip("If true, this character can spot enemies behind itself.")]
		public bool fullVision;

		// Token: 0x040022D4 RID: 8916
		[Tooltip("The minimum distance this character will try to maintain from its enemy, in meters, backing up if closer than this range.")]
		public float minDistanceFromEnemy;

		// Token: 0x040022D5 RID: 8917
		public float enemyAttentionDuration = 5f;

		// Token: 0x040022D6 RID: 8918
		public BaseAI.NavigationType navigationType;

		// Token: 0x040022D7 RID: 8919
		public MapNodeGroup.GraphType nodegraphType;

		// Token: 0x040022D8 RID: 8920
		[Tooltip("The state machine to run while the body exists.")]
		public EntityStateMachine stateMachine;

		// Token: 0x040022D9 RID: 8921
		public SerializableEntityStateType scanState;

		// Token: 0x040022DA RID: 8922
		public bool isHealer;

		// Token: 0x040022DB RID: 8923
		public float enemyAttention;

		// Token: 0x040022DC RID: 8924
		public float aimVectorDampTime = 0.2f;

		// Token: 0x040022DD RID: 8925
		public float aimVectorMaxSpeed = 6f;

		// Token: 0x040022DE RID: 8926
		[HideInInspector]
		public Vector3 desiredAimDirection = Vector3.forward;

		// Token: 0x040022DF RID: 8927
		private Vector3 aimVelocity = Vector3.zero;

		// Token: 0x040022E0 RID: 8928
		private float targetRefreshTimer;

		// Token: 0x040022E1 RID: 8929
		private const float targetRefreshDuration = 0.5f;

		// Token: 0x040022E2 RID: 8930
		public PathFollower pathFollower = new PathFollower();

		// Token: 0x040022E3 RID: 8931
		public LocalNavigator localNavigator = new LocalNavigator();

		// Token: 0x040022E4 RID: 8932
		protected PathTask pendingPath;

		// Token: 0x040022E5 RID: 8933
		public NavMeshPath navMeshPath;

		// Token: 0x040022E6 RID: 8934
		public bool drawAIPath;

		// Token: 0x040022E7 RID: 8935
		public string selectedSkilldriverName;

		// Token: 0x040022E8 RID: 8936
		private const float maxVisionDistance = float.PositiveInfinity;

		// Token: 0x040022E9 RID: 8937
		public HurtBox debugEnemyHurtBox;

		// Token: 0x040022EA RID: 8938
		private BaseAI.Target currentEnemy;

		// Token: 0x040022EB RID: 8939
		public BaseAI.Target leader;

		// Token: 0x040022EC RID: 8940
		private BaseAI.Target buddy;

		// Token: 0x040022ED RID: 8941
		private BullseyeSearch enemySearch = new BullseyeSearch();

		// Token: 0x040022EE RID: 8942
		private BullseyeSearch buddySearch = new BullseyeSearch();

		// Token: 0x040022EF RID: 8943
		private float skillDriverUpdateTimer;

		// Token: 0x040022F0 RID: 8944
		private const float skillDriverMinUpdateInterval = 0.166666672f;

		// Token: 0x040022F1 RID: 8945
		private const float skillDriverMaxUpdateInterval = 0.2f;

		// Token: 0x040022F2 RID: 8946
		public BaseAI.SkillDriverEvaluation skillDriverEvaluation;

		// Token: 0x020005AA RID: 1450
		public enum NavigationType
		{
			// Token: 0x040022F4 RID: 8948
			Nodegraph,
			// Token: 0x040022F5 RID: 8949
			NavMesh
		}

		// Token: 0x020005AB RID: 1451
		public class Target
		{
			// Token: 0x060020CB RID: 8395 RVA: 0x00017E5A File Offset: 0x0001605A
			public Target([NotNull] BaseAI owner)
			{
				this.owner = owner;
			}

			// Token: 0x170002E0 RID: 736
			// (get) Token: 0x060020CC RID: 8396 RVA: 0x00017E70 File Offset: 0x00016070
			// (set) Token: 0x060020CD RID: 8397 RVA: 0x0009E740 File Offset: 0x0009C940
			public GameObject gameObject
			{
				get
				{
					return this._gameObject;
				}
				set
				{
					if (value == this._gameObject)
					{
						return;
					}
					this._gameObject = value;
					GameObject gameObject = this.gameObject;
					this.characterBody = ((gameObject != null) ? gameObject.GetComponent<CharacterBody>() : null);
					CharacterBody characterBody = this.characterBody;
					this.healthComponent = ((characterBody != null) ? characterBody.healthComponent : null);
					CharacterBody characterBody2 = this.characterBody;
					this.hurtBoxGroup = ((characterBody2 != null) ? characterBody2.hurtBoxGroup : null);
					this.bullseyeCount = (this.hurtBoxGroup ? this.hurtBoxGroup.bullseyeCount : 0);
					this.mainHurtBox = (this.hurtBoxGroup ? this.hurtBoxGroup.mainHurtBox : null);
					this.bestHurtBox = this.mainHurtBox;
					this.hasLoS = false;
					this.unset = !this._gameObject;
				}
			}

			// Token: 0x060020CE RID: 8398 RVA: 0x0009E814 File Offset: 0x0009CA14
			public void Update()
			{
				if (!this.gameObject)
				{
					return;
				}
				this.hasLoS = (this.bestHurtBox && this.owner.HasLOS(this.bestHurtBox.transform.position));
				if (this.bullseyeCount > 1 && !this.hasLoS)
				{
					this.bestHurtBox = this.GetBestHurtBox(out this.hasLoS);
				}
			}

			// Token: 0x060020CF RID: 8399 RVA: 0x0009E884 File Offset: 0x0009CA84
			public bool GetBullseyePosition(out Vector3 position)
			{
				if (!this.bestHurtBox)
				{
					position = (this.gameObject ? this.gameObject.transform.position : Vector3.zero);
					return false;
				}
				position = this.bestHurtBox.transform.position;
				return true;
			}

			// Token: 0x060020D0 RID: 8400 RVA: 0x0009E8E4 File Offset: 0x0009CAE4
			private HurtBox GetBestHurtBox(out bool hadLoS)
			{
				if (this.owner.bodyInputBank)
				{
					Vector3 aimOrigin = this.owner.bodyInputBank.aimOrigin;
					HurtBox hurtBox = null;
					float num = float.PositiveInfinity;
					foreach (HurtBox hurtBox2 in this.hurtBoxGroup.hurtBoxes)
					{
						if (hurtBox2.isBullseye)
						{
							Vector3 position = hurtBox2.transform.position;
							if (BaseAI.CheckLoS(aimOrigin, hurtBox2.transform.position))
							{
								float sqrMagnitude = (position - aimOrigin).sqrMagnitude;
								if (sqrMagnitude < num)
								{
									num = sqrMagnitude;
									hurtBox = hurtBox2;
								}
							}
						}
					}
					if (hurtBox)
					{
						hadLoS = true;
						return hurtBox;
					}
				}
				hadLoS = false;
				return this.mainHurtBox;
			}

			// Token: 0x170002E1 RID: 737
			// (get) Token: 0x060020D1 RID: 8401 RVA: 0x00017E78 File Offset: 0x00016078
			// (set) Token: 0x060020D2 RID: 8402 RVA: 0x00017E80 File Offset: 0x00016080
			public CharacterBody characterBody { get; private set; }

			// Token: 0x170002E2 RID: 738
			// (get) Token: 0x060020D3 RID: 8403 RVA: 0x00017E89 File Offset: 0x00016089
			// (set) Token: 0x060020D4 RID: 8404 RVA: 0x00017E91 File Offset: 0x00016091
			public HealthComponent healthComponent { get; private set; }

			// Token: 0x060020D5 RID: 8405 RVA: 0x0009E9A4 File Offset: 0x0009CBA4
			public void Reset()
			{
				if (this.unset)
				{
					return;
				}
				this._gameObject = null;
				this.characterBody = null;
				this.healthComponent = null;
				this.hurtBoxGroup = null;
				this.bullseyeCount = 0;
				this.mainHurtBox = null;
				this.bestHurtBox = this.mainHurtBox;
				this.hasLoS = false;
				this.unset = true;
			}

			// Token: 0x040022F6 RID: 8950
			private readonly BaseAI owner;

			// Token: 0x040022F7 RID: 8951
			private bool unset = true;

			// Token: 0x040022F8 RID: 8952
			private GameObject _gameObject;

			// Token: 0x040022FB RID: 8955
			public HurtBox bestHurtBox;

			// Token: 0x040022FC RID: 8956
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x040022FD RID: 8957
			private HurtBox mainHurtBox;

			// Token: 0x040022FE RID: 8958
			private int bullseyeCount;

			// Token: 0x040022FF RID: 8959
			public bool hasLoS;
		}

		// Token: 0x020005AC RID: 1452
		public struct SkillDriverEvaluation
		{
			// Token: 0x04002300 RID: 8960
			public AISkillDriver dominantSkillDriver;

			// Token: 0x04002301 RID: 8961
			public BaseAI.Target target;

			// Token: 0x04002302 RID: 8962
			public float separationSqrMagnitude;
		}
	}
}
