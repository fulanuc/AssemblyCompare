using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000296 RID: 662
	[RequireComponent(typeof(CharacterBody))]
	public class CharacterMotor : BaseCharacterController, ILifeBehavior
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x0000A67E File Offset: 0x0000887E
		public float walkSpeed
		{
			get
			{
				return this.body.moveSpeed * this.walkSpeedPenaltyCoefficient;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0000A692 File Offset: 0x00008892
		public float acceleration
		{
			get
			{
				return this.body.acceleration;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x0000A69F File Offset: 0x0000889F
		public bool atRest
		{
			get
			{
				return this.restStopwatch > 1f;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x0000A6B7 File Offset: 0x000088B7
		// (set) Token: 0x06000D55 RID: 3413 RVA: 0x0000A6AE File Offset: 0x000088AE
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000D57 RID: 3415 RVA: 0x0000A6BF File Offset: 0x000088BF
		public Vector3 estimatedFloorNormal
		{
			get
			{
				return base.Motor.GroundingStatus.GroundNormal;
			}
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0000A6D1 File Offset: 0x000088D1
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x00054938 File Offset: 0x00052B38
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.capsuleCollider = base.GetComponent<CapsuleCollider>();
			this.previousPosition = base.transform.position;
			base.Motor.Rigidbody.mass = this.mass;
			base.Motor.CollidableLayers = LayerIndex.defaultLayer.collisionMask;
			base.Motor.MaxStableSlopeAngle = 70f;
			base.Motor.MaxStableDenivelationAngle = 55f;
			if (this.generateParametersOnAwake)
			{
				this.GenerateParameters();
			}
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0000A6E4 File Offset: 0x000088E4
		private void Start()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0000A6E4 File Offset: 0x000088E4
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0000A6E4 File Offset: 0x000088E4
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0000A6EC File Offset: 0x000088EC
		private void OnEnable()
		{
			CharacterMotor.instancesList.Add(this);
			base.Motor.enabled = true;
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0000A705 File Offset: 0x00008905
		private void OnDisable()
		{
			base.Motor.enabled = false;
			CharacterMotor.instancesList.Remove(this);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x000549CC File Offset: 0x00052BCC
		private void PreMove(float deltaTime)
		{
			if (this.hasEffectiveAuthority)
			{
				float num = this.acceleration;
				float num2 = this.velocity.y;
				num2 += Physics.gravity.y * deltaTime;
				if (this.isGrounded)
				{
					num2 = Mathf.Max(num2, 0f);
				}
				else
				{
					num *= (this.disableAirControlUntilCollision ? 0f : this.airControl);
				}
				Vector2 vector = new Vector2(this.velocity.x, this.velocity.z);
				Vector2 a = Vector2.zero;
				Vector3 v = Vector2.zero;
				if (this.canWalk)
				{
					a = new Vector2(this.moveDirection.x, this.moveDirection.z);
					if (this.body.isSprinting)
					{
						float magnitude = a.magnitude;
						if (magnitude < 1f && magnitude > 0f)
						{
							a /= magnitude;
						}
					}
					v = a * this.walkSpeed;
				}
				vector = Vector2.MoveTowards(vector, v, num * deltaTime);
				this.velocity = new Vector3(vector.x, num2, vector.y);
			}
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x0000A71F File Offset: 0x0000891F
		public void OnDeathStart()
		{
			this.alive = false;
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00054AF8 File Offset: 0x00052CF8
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (fixedDeltaTime == 0f)
			{
				return;
			}
			Vector3 position = base.transform.position;
			if ((this.previousPosition - position).sqrMagnitude < 0.000625000044f * fixedDeltaTime)
			{
				this.restStopwatch += fixedDeltaTime;
			}
			else
			{
				this.restStopwatch = 0f;
			}
			this.previousPosition = position;
			if (this.netIsGrounded)
			{
				this.lastGroundedTime = Run.FixedTimeStamp.now;
			}
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0000A728 File Offset: 0x00008928
		private void GenerateParameters()
		{
			this.slopeLimit = 70f;
			this.stepOffset = Mathf.Min(this.capsuleHeight * 0.1f, 0.2f);
			this.stepHandlingMethod = StepHandlingMethod.None;
			this.ledgeHandling = false;
			this.interactiveRigidbodyHandling = true;
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x0000A766 File Offset: 0x00008966
		private bool canWalk
		{
			get
			{
				return !this.muteWalkMotion && this.alive;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x0000A778 File Offset: 0x00008978
		public bool isGrounded
		{
			get
			{
				if (!this.hasEffectiveAuthority)
				{
					return this.netIsGrounded;
				}
				return base.Motor.GroundingStatus.IsStableOnGround;
			}
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x00054B74 File Offset: 0x00052D74
		public void ApplyForce(Vector3 force, bool alwaysApply = false)
		{
			if (NetworkServer.active && !this.hasEffectiveAuthority)
			{
				this.CallRpcApplyForce(force, alwaysApply);
				return;
			}
			if (this.mass != 0f)
			{
				Vector3 vector = force * (1f / this.mass);
				if (vector.y < 6f && this.isGrounded && !alwaysApply)
				{
					vector.y = 0f;
				}
				this.velocity += vector;
			}
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x0000A799 File Offset: 0x00008999
		[ClientRpc]
		private void RpcApplyForce(Vector3 force, bool alwaysApply)
		{
			if (!NetworkServer.active)
			{
				this.ApplyForce(force, alwaysApply);
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000D67 RID: 3431 RVA: 0x0000A7AA File Offset: 0x000089AA
		// (set) Token: 0x06000D68 RID: 3432 RVA: 0x0000A7B2 File Offset: 0x000089B2
		public Vector3 moveDirection
		{
			get
			{
				return this._moveDirection;
			}
			set
			{
				this._moveDirection = value;
				this._moveDirection.y = 0f;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x0000A7CB File Offset: 0x000089CB
		// (set) Token: 0x06000D6A RID: 3434 RVA: 0x0000A7D8 File Offset: 0x000089D8
		private float slopeLimit
		{
			get
			{
				return base.Motor.MaxStableSlopeAngle;
			}
			set
			{
				base.Motor.MaxStableSlopeAngle = value;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000D6B RID: 3435 RVA: 0x0000A7E6 File Offset: 0x000089E6
		// (set) Token: 0x06000D6C RID: 3436 RVA: 0x0000A7F3 File Offset: 0x000089F3
		public float stepOffset
		{
			get
			{
				return base.Motor.MaxStepHeight;
			}
			set
			{
				base.Motor.MaxStepHeight = value;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x0000A801 File Offset: 0x00008A01
		public float capsuleHeight
		{
			get
			{
				return this.capsuleCollider.height;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x0000A80E File Offset: 0x00008A0E
		public float capsuleRadius
		{
			get
			{
				return this.capsuleCollider.radius;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000D6F RID: 3439 RVA: 0x00054BF0 File Offset: 0x00052DF0
		// (set) Token: 0x06000D70 RID: 3440 RVA: 0x0000A81B File Offset: 0x00008A1B
		public StepHandlingMethod stepHandlingMethod
		{
			get
			{
				return base.Motor.StepHandling = StepHandlingMethod.None;
			}
			set
			{
				base.Motor.StepHandling = value;
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000D71 RID: 3441 RVA: 0x0000A829 File Offset: 0x00008A29
		// (set) Token: 0x06000D72 RID: 3442 RVA: 0x0000A836 File Offset: 0x00008A36
		public bool ledgeHandling
		{
			get
			{
				return base.Motor.LedgeHandling;
			}
			set
			{
				base.Motor.LedgeHandling = value;
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000D73 RID: 3443 RVA: 0x0000A844 File Offset: 0x00008A44
		// (set) Token: 0x06000D74 RID: 3444 RVA: 0x0000A851 File Offset: 0x00008A51
		public bool interactiveRigidbodyHandling
		{
			get
			{
				return base.Motor.InteractiveRigidbodyHandling;
			}
			set
			{
				base.Motor.InteractiveRigidbodyHandling = value;
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000D75 RID: 3445 RVA: 0x0000A85F File Offset: 0x00008A5F
		// (set) Token: 0x06000D76 RID: 3446 RVA: 0x0000A867 File Offset: 0x00008A67
		public Run.FixedTimeStamp lastGroundedTime { get; private set; } = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x06000D77 RID: 3447 RVA: 0x0000A870 File Offset: 0x00008A70
		private void OnHitGround(Vector3 fallVelocity)
		{
			if (NetworkServer.active)
			{
				GlobalEventManager.instance.OnCharacterHitGround(this.body, fallVelocity);
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.CallCmdHitGround(fallVelocity);
			}
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0000A89A File Offset: 0x00008A9A
		[Command]
		private void CmdHitGround(Vector3 fallVelocity)
		{
			this.OnHitGround(fallVelocity);
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0000A8A3 File Offset: 0x00008AA3
		public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			currentRotation = Quaternion.identity;
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x0000A8B0 File Offset: 0x00008AB0
		public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			currentVelocity = this.velocity;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00054C0C File Offset: 0x00052E0C
		public override void BeforeCharacterUpdate(float deltaTime)
		{
			if (this.rootMotion != Vector3.zero)
			{
				base.Motor.MoveCharacter(base.transform.position + this.rootMotion);
				this.rootMotion = Vector3.zero;
			}
			this.PreMove(deltaTime);
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00054C60 File Offset: 0x00052E60
		public override void PostGroundingUpdate(float deltaTime)
		{
			if (base.Motor.GroundingStatus.IsStableOnGround != base.Motor.LastGroundingStatus.IsStableOnGround)
			{
				this.netIsGrounded = base.Motor.GroundingStatus.IsStableOnGround;
				if (base.Motor.GroundingStatus.IsStableOnGround)
				{
					this.OnLanded();
					return;
				}
				this.OnLeaveStableGround();
			}
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00054CC4 File Offset: 0x00052EC4
		private void OnLanded()
		{
			this.jumpCount = 0;
			CharacterMotor.HitGroundInfo hitGroundInfo = new CharacterMotor.HitGroundInfo
			{
				velocity = this.lastVelocity,
				position = base.Motor.GroundingStatus.GroundPoint
			};
			if (NetworkServer.active)
			{
				CharacterMotor.HitGroundDelegate hitGroundDelegate = this.onHitGround;
				if (hitGroundDelegate != null)
				{
					hitGroundDelegate(ref hitGroundInfo);
				}
				GlobalEventManager.instance.OnCharacterHitGround(this.body, hitGroundInfo.velocity);
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				CharacterMotor.HitGroundDelegate hitGroundDelegate2 = this.onHitGround;
				if (hitGroundDelegate2 != null)
				{
					hitGroundDelegate2(ref hitGroundInfo);
				}
				this.CallCmdHitGround(hitGroundInfo.velocity);
			}
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0000A8BE File Offset: 0x00008ABE
		private void OnLeaveStableGround()
		{
			if (this.jumpCount < 1)
			{
				this.jumpCount = 1;
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0000A8D0 File Offset: 0x00008AD0
		public override void AfterCharacterUpdate(float deltaTime)
		{
			this.lastVelocity = this.velocity;
			this.velocity = base.Motor.BaseVelocity;
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0000A8EF File Offset: 0x00008AEF
		public override bool IsColliderValidForCollisions(Collider coll)
		{
			return !coll.isTrigger && coll != base.Motor.Capsule;
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000D82 RID: 3458 RVA: 0x00054D60 File Offset: 0x00052F60
		// (remove) Token: 0x06000D83 RID: 3459 RVA: 0x00054D98 File Offset: 0x00052F98
		public event CharacterMotor.HitGroundDelegate onHitGround;

		// Token: 0x06000D84 RID: 3460 RVA: 0x0000A90C File Offset: 0x00008B0C
		public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			this.disableAirControlUntilCollision = false;
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x000025DA File Offset: 0x000007DA
		public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x00054E20 File Offset: 0x00053020
		static CharacterMotor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMotor), CharacterMotor.kCmdCmdHitGround, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeCmdCmdHitGround));
			CharacterMotor.kRpcRpcApplyForce = -1753076289;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterMotor), CharacterMotor.kRpcRpcApplyForce, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeRpcRpcApplyForce));
			NetworkCRC.RegisterBehaviour("CharacterMotor", 0);
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0000A915 File Offset: 0x00008B15
		protected static void InvokeCmdCmdHitGround(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdHitGround called on client.");
				return;
			}
			((CharacterMotor)obj).CmdHitGround(reader.ReadVector3());
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x00054E9C File Offset: 0x0005309C
		public void CallCmdHitGround(Vector3 fallVelocity)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdHitGround called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdHitGround(fallVelocity);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterMotor.kCmdCmdHitGround);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(fallVelocity);
			base.SendCommandInternal(networkWriter, 0, "CmdHitGround");
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0000A93E File Offset: 0x00008B3E
		protected static void InvokeRpcRpcApplyForce(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcApplyForce called on server.");
				return;
			}
			((CharacterMotor)obj).RpcApplyForce(reader.ReadVector3(), reader.ReadBoolean());
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00054F28 File Offset: 0x00053128
		public void CallRpcApplyForce(Vector3 force, bool alwaysApply)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcApplyForce called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterMotor.kRpcRpcApplyForce);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(force);
			networkWriter.Write(alwaysApply);
			this.SendRPCInternal(networkWriter, 0, "RpcApplyForce");
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00054FA8 File Offset: 0x000531A8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			bool flag2;
			return flag2 || flag;
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x0000A96D File Offset: 0x00008B6D
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
		}

		// Token: 0x04001166 RID: 4454
		public static readonly List<CharacterMotor> instancesList = new List<CharacterMotor>();

		// Token: 0x04001167 RID: 4455
		[HideInInspector]
		public float walkSpeedPenaltyCoefficient = 1f;

		// Token: 0x04001168 RID: 4456
		[Tooltip("The character direction component to supply a move vector to.")]
		public CharacterDirection characterDirection;

		// Token: 0x04001169 RID: 4457
		[Tooltip("Whether or not a move vector supplied to this component can cause movement. Use this when the object is driven by root motion.")]
		public bool muteWalkMotion;

		// Token: 0x0400116A RID: 4458
		[Tooltip("The mass of this character.")]
		public float mass = 1f;

		// Token: 0x0400116B RID: 4459
		[Tooltip("The air control value of this character as a fraction of ground control.")]
		public float airControl = 0.25f;

		// Token: 0x0400116C RID: 4460
		[Tooltip("Disables Air Control for things like jumppads")]
		public bool disableAirControlUntilCollision;

		// Token: 0x0400116D RID: 4461
		[Tooltip("Auto-assigns parameters skin width, slope angle, and step offset as a function of the Character Motor's radius and height")]
		public bool generateParametersOnAwake = true;

		// Token: 0x0400116E RID: 4462
		private CharacterBody body;

		// Token: 0x0400116F RID: 4463
		private CapsuleCollider capsuleCollider;

		// Token: 0x04001170 RID: 4464
		private bool alive = true;

		// Token: 0x04001171 RID: 4465
		private const float restDuration = 1f;

		// Token: 0x04001172 RID: 4466
		private const float restVelocityThreshold = 0.025f;

		// Token: 0x04001173 RID: 4467
		private const float restVelocityThresholdSqr = 0.000625000044f;

		// Token: 0x04001174 RID: 4468
		public const float slipStartAngle = 70f;

		// Token: 0x04001175 RID: 4469
		public const float slipEndAngle = 55f;

		// Token: 0x04001176 RID: 4470
		private float restStopwatch;

		// Token: 0x04001177 RID: 4471
		private Vector3 previousPosition;

		// Token: 0x04001179 RID: 4473
		[NonSerialized]
		public int jumpCount;

		// Token: 0x0400117A RID: 4474
		[NonSerialized]
		public bool netIsGrounded;

		// Token: 0x0400117B RID: 4475
		[NonSerialized]
		public Vector3 velocity;

		// Token: 0x0400117C RID: 4476
		private Vector3 lastVelocity;

		// Token: 0x0400117D RID: 4477
		[NonSerialized]
		public Vector3 rootMotion;

		// Token: 0x0400117E RID: 4478
		private Vector3 _moveDirection;

		// Token: 0x04001181 RID: 4481
		private static int kRpcRpcApplyForce;

		// Token: 0x04001182 RID: 4482
		private static int kCmdCmdHitGround = 2030335022;

		// Token: 0x02000297 RID: 663
		public struct HitGroundInfo
		{
			// Token: 0x06000D8F RID: 3471 RVA: 0x0000A977 File Offset: 0x00008B77
			public override string ToString()
			{
				return string.Format("velocity={0} position={1}", this.velocity, this.position);
			}

			// Token: 0x04001183 RID: 4483
			public Vector3 velocity;

			// Token: 0x04001184 RID: 4484
			public Vector3 position;
		}

		// Token: 0x02000298 RID: 664
		// (Invoke) Token: 0x06000D91 RID: 3473
		public delegate void HitGroundDelegate(ref CharacterMotor.HitGroundInfo hitGroundInfo);
	}
}
