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
		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x0000A619 File Offset: 0x00008819
		public float walkSpeed
		{
			get
			{
				return this.body.moveSpeed * this.walkSpeedPenaltyCoefficient;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x0000A62D File Offset: 0x0000882D
		public float acceleration
		{
			get
			{
				return this.body.acceleration;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0000A63A File Offset: 0x0000883A
		public bool atRest
		{
			get
			{
				return this.restStopwatch > 1f;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000D55 RID: 3413 RVA: 0x0000A652 File Offset: 0x00008852
		// (set) Token: 0x06000D54 RID: 3412 RVA: 0x0000A649 File Offset: 0x00008849
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x0000A65A File Offset: 0x0000885A
		public Vector3 estimatedFloorNormal
		{
			get
			{
				return base.Motor.GroundingStatus.GroundNormal;
			}
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0000A66C File Offset: 0x0000886C
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x00054B10 File Offset: 0x00052D10
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

		// Token: 0x06000D59 RID: 3417 RVA: 0x0000A67F File Offset: 0x0000887F
		private void Start()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0000A67F File Offset: 0x0000887F
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0000A67F File Offset: 0x0000887F
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0000A687 File Offset: 0x00008887
		private void OnEnable()
		{
			CharacterMotor.instancesList.Add(this);
			base.Motor.enabled = true;
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0000A6A0 File Offset: 0x000088A0
		private void OnDisable()
		{
			base.Motor.enabled = false;
			CharacterMotor.instancesList.Remove(this);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00054BA4 File Offset: 0x00052DA4
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

		// Token: 0x06000D5F RID: 3423 RVA: 0x0000A6BA File Offset: 0x000088BA
		public void OnDeathStart()
		{
			this.alive = false;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00054CD0 File Offset: 0x00052ED0
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
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0000A6C3 File Offset: 0x000088C3
		private void GenerateParameters()
		{
			this.slopeLimit = 70f;
			this.stepOffset = Mathf.Min(this.capsuleHeight * 0.1f, 0.2f);
			this.stepHandlingMethod = StepHandlingMethod.None;
			this.ledgeHandling = false;
			this.interactiveRigidbodyHandling = true;
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x0000A701 File Offset: 0x00008901
		private bool canWalk
		{
			get
			{
				return !this.muteWalkMotion && this.alive;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x0000A713 File Offset: 0x00008913
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

		// Token: 0x06000D64 RID: 3428 RVA: 0x00054D38 File Offset: 0x00052F38
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

		// Token: 0x06000D65 RID: 3429 RVA: 0x0000A734 File Offset: 0x00008934
		[ClientRpc]
		private void RpcApplyForce(Vector3 force, bool alwaysApply)
		{
			if (!NetworkServer.active)
			{
				this.ApplyForce(force, alwaysApply);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000D66 RID: 3430 RVA: 0x0000A745 File Offset: 0x00008945
		// (set) Token: 0x06000D67 RID: 3431 RVA: 0x0000A74D File Offset: 0x0000894D
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

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000D68 RID: 3432 RVA: 0x0000A766 File Offset: 0x00008966
		// (set) Token: 0x06000D69 RID: 3433 RVA: 0x0000A773 File Offset: 0x00008973
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

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000D6A RID: 3434 RVA: 0x0000A781 File Offset: 0x00008981
		// (set) Token: 0x06000D6B RID: 3435 RVA: 0x0000A78E File Offset: 0x0000898E
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

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x0000A79C File Offset: 0x0000899C
		public float capsuleHeight
		{
			get
			{
				return this.capsuleCollider.height;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x0000A7A9 File Offset: 0x000089A9
		public float capsuleRadius
		{
			get
			{
				return this.capsuleCollider.radius;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x00054DB4 File Offset: 0x00052FB4
		// (set) Token: 0x06000D6F RID: 3439 RVA: 0x0000A7B6 File Offset: 0x000089B6
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

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000D70 RID: 3440 RVA: 0x0000A7C4 File Offset: 0x000089C4
		// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0000A7D1 File Offset: 0x000089D1
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

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x0000A7DF File Offset: 0x000089DF
		// (set) Token: 0x06000D73 RID: 3443 RVA: 0x0000A7EC File Offset: 0x000089EC
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

		// Token: 0x06000D74 RID: 3444 RVA: 0x0000A7FA File Offset: 0x000089FA
		private void OnHitGround(Vector3 fallVelocity)
		{
			if (NetworkServer.active)
			{
				GlobalEventManager.instance.OnCharacterHitGround(this.body, fallVelocity);
				return;
			}
			this.CallCmdHitGround(fallVelocity);
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0000A81C File Offset: 0x00008A1C
		[Command]
		private void CmdHitGround(Vector3 fallVelocity)
		{
			this.OnHitGround(fallVelocity);
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x0000A825 File Offset: 0x00008A25
		public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			currentRotation = Quaternion.identity;
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x0000A832 File Offset: 0x00008A32
		public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			currentVelocity = this.velocity;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00054DD0 File Offset: 0x00052FD0
		public override void BeforeCharacterUpdate(float deltaTime)
		{
			if (this.rootMotion != Vector3.zero)
			{
				base.Motor.MoveCharacter(base.transform.position + this.rootMotion);
				this.rootMotion = Vector3.zero;
			}
			this.PreMove(deltaTime);
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x00054E24 File Offset: 0x00053024
		public override void PostGroundingUpdate(float deltaTime)
		{
			if (base.Motor.GroundingStatus.IsStableOnGround != base.Motor.LastGroundingStatus.IsStableOnGround)
			{
				if (base.Motor.GroundingStatus.IsStableOnGround)
				{
					this.OnLanded();
					return;
				}
				this.OnLeaveStableGround();
			}
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00054E74 File Offset: 0x00053074
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

		// Token: 0x06000D7B RID: 3451 RVA: 0x0000A840 File Offset: 0x00008A40
		private void OnLeaveStableGround()
		{
			if (this.jumpCount < 1)
			{
				this.jumpCount = 1;
			}
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0000A852 File Offset: 0x00008A52
		public override void AfterCharacterUpdate(float deltaTime)
		{
			this.lastVelocity = this.velocity;
			this.velocity = base.Motor.BaseVelocity;
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x0000A871 File Offset: 0x00008A71
		public override bool IsColliderValidForCollisions(Collider coll)
		{
			return !coll.isTrigger && coll != base.Motor.Capsule;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000D7F RID: 3455 RVA: 0x00054F10 File Offset: 0x00053110
		// (remove) Token: 0x06000D80 RID: 3456 RVA: 0x00054F48 File Offset: 0x00053148
		public event CharacterMotor.HitGroundDelegate onHitGround;

		// Token: 0x06000D81 RID: 3457 RVA: 0x0000A88E File Offset: 0x00008A8E
		public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			this.disableAirControlUntilCollision = false;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x00054F80 File Offset: 0x00053180
		static CharacterMotor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMotor), CharacterMotor.kCmdCmdHitGround, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeCmdCmdHitGround));
			CharacterMotor.kRpcRpcApplyForce = -1753076289;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterMotor), CharacterMotor.kRpcRpcApplyForce, new NetworkBehaviour.CmdDelegate(CharacterMotor.InvokeRpcRpcApplyForce));
			NetworkCRC.RegisterBehaviour("CharacterMotor", 0);
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0000A8CE File Offset: 0x00008ACE
		protected static void InvokeCmdCmdHitGround(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdHitGround called on client.");
				return;
			}
			((CharacterMotor)obj).CmdHitGround(reader.ReadVector3());
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x00054FFC File Offset: 0x000531FC
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

		// Token: 0x06000D88 RID: 3464 RVA: 0x0000A8F7 File Offset: 0x00008AF7
		protected static void InvokeRpcRpcApplyForce(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcApplyForce called on server.");
				return;
			}
			((CharacterMotor)obj).RpcApplyForce(reader.ReadVector3(), reader.ReadBoolean());
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x00055088 File Offset: 0x00053288
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

		// Token: 0x06000D8A RID: 3466 RVA: 0x00055108 File Offset: 0x00053308
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool flag = base.OnSerialize(writer, forceAll);
			bool flag2;
			return flag2 || flag;
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0000A926 File Offset: 0x00008B26
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
		}

		// Token: 0x0400115C RID: 4444
		public static readonly List<CharacterMotor> instancesList = new List<CharacterMotor>();

		// Token: 0x0400115D RID: 4445
		[HideInInspector]
		public float walkSpeedPenaltyCoefficient = 1f;

		// Token: 0x0400115E RID: 4446
		[Tooltip("The character direction component to supply a move vector to.")]
		public CharacterDirection characterDirection;

		// Token: 0x0400115F RID: 4447
		[Tooltip("Whether or not a move vector supplied to this component can cause movement. Use this when the object is driven by root motion.")]
		public bool muteWalkMotion;

		// Token: 0x04001160 RID: 4448
		[Tooltip("The mass of this character.")]
		public float mass = 1f;

		// Token: 0x04001161 RID: 4449
		[Tooltip("The air control value of this character as a fraction of ground control.")]
		public float airControl = 0.25f;

		// Token: 0x04001162 RID: 4450
		[Tooltip("Disables Air Control for things like jumppads")]
		public bool disableAirControlUntilCollision;

		// Token: 0x04001163 RID: 4451
		[Tooltip("Auto-assigns parameters skin width, slope angle, and step offset as a function of the Character Motor's radius and height")]
		public bool generateParametersOnAwake = true;

		// Token: 0x04001164 RID: 4452
		private CharacterBody body;

		// Token: 0x04001165 RID: 4453
		private CapsuleCollider capsuleCollider;

		// Token: 0x04001166 RID: 4454
		private bool alive = true;

		// Token: 0x04001167 RID: 4455
		private const float restDuration = 1f;

		// Token: 0x04001168 RID: 4456
		private const float restVelocityThreshold = 0.025f;

		// Token: 0x04001169 RID: 4457
		private const float restVelocityThresholdSqr = 0.000625000044f;

		// Token: 0x0400116A RID: 4458
		public const float slipStartAngle = 70f;

		// Token: 0x0400116B RID: 4459
		public const float slipEndAngle = 55f;

		// Token: 0x0400116C RID: 4460
		private float restStopwatch;

		// Token: 0x0400116D RID: 4461
		private Vector3 previousPosition;

		// Token: 0x0400116F RID: 4463
		[NonSerialized]
		public int jumpCount;

		// Token: 0x04001170 RID: 4464
		[NonSerialized]
		public bool netIsGrounded;

		// Token: 0x04001171 RID: 4465
		[NonSerialized]
		public Vector3 velocity;

		// Token: 0x04001172 RID: 4466
		private Vector3 lastVelocity;

		// Token: 0x04001173 RID: 4467
		[NonSerialized]
		public Vector3 rootMotion;

		// Token: 0x04001174 RID: 4468
		private Vector3 _moveDirection;

		// Token: 0x04001176 RID: 4470
		private static int kRpcRpcApplyForce;

		// Token: 0x04001177 RID: 4471
		private static int kCmdCmdHitGround = 2030335022;

		// Token: 0x02000297 RID: 663
		public struct HitGroundInfo
		{
			// Token: 0x06000D8C RID: 3468 RVA: 0x0000A930 File Offset: 0x00008B30
			public override string ToString()
			{
				return string.Format("velocity={0} position={1}", this.velocity, this.position);
			}

			// Token: 0x04001178 RID: 4472
			public Vector3 velocity;

			// Token: 0x04001179 RID: 4473
			public Vector3 position;
		}

		// Token: 0x02000298 RID: 664
		// (Invoke) Token: 0x06000D8E RID: 3470
		public delegate void HitGroundDelegate(ref CharacterMotor.HitGroundInfo hitGroundInfo);
	}
}
