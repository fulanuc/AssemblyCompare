using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200028A RID: 650
	public class CharacterDirection : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000CCE RID: 3278 RVA: 0x0000A060 File Offset: 0x00008260
		// (set) Token: 0x06000CCD RID: 3277 RVA: 0x0000A02A File Offset: 0x0000822A
		public float yaw
		{
			get
			{
				return this._yaw;
			}
			set
			{
				this._yaw = value;
				if (this.targetTransform)
				{
					this.targetTransform.rotation = Quaternion.Euler(0f, this._yaw, 0f);
				}
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x00052458 File Offset: 0x00050658
		public Vector3 animatorForward
		{
			get
			{
				if (!this.overrideAnimatorForwardTransform)
				{
					return this.forward;
				}
				float y = this.overrideAnimatorForwardTransform.eulerAngles.y;
				return Quaternion.Euler(0f, y, 0f) * Vector3.forward;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0000A068 File Offset: 0x00008268
		// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x000524A4 File Offset: 0x000506A4
		public Vector3 forward
		{
			get
			{
				return Quaternion.Euler(0f, this.yaw, 0f) * Vector3.forward;
			}
			set
			{
				value.y = 0f;
				this.yaw = Util.QuaternionSafeLookRotation(value, Vector3.up).eulerAngles.y;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0000A092 File Offset: 0x00008292
		// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x0000A089 File Offset: 0x00008289
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0000A09A File Offset: 0x0000829A
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0000A0AD File Offset: 0x000082AD
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0000A0AD File Offset: 0x000082AD
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000524DC File Offset: 0x000506DC
		private void Start()
		{
			this.UpdateAuthority();
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				this.modelAnimator = component.modelTransform.GetComponent<Animator>();
			}
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x0000A0B5 File Offset: 0x000082B5
		private void Update()
		{
			this.Simulate(Time.deltaTime);
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0000A0C2 File Offset: 0x000082C2
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x00052510 File Offset: 0x00050710
		private static int PickIndex(float angle)
		{
			float num = Mathf.Sign(angle);
			int num2 = Mathf.CeilToInt((angle * num - 22.5f) * 0.0222222228f);
			return Mathf.Clamp(CharacterDirection.paramsMidIndex + num2 * (int)num, 0, CharacterDirection.turnAnimatorParamsSets.Length - 1);
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x00052554 File Offset: 0x00050754
		private void Simulate(float deltaTime)
		{
			Quaternion quaternion = Quaternion.Euler(0f, this.yaw, 0f);
			if (this.hasEffectiveAuthority)
			{
				if (this.driveFromRootRotation)
				{
					Quaternion rhs = this.rootMotionAccumulator.ExtractRootRotation();
					if (this.targetTransform)
					{
						this.targetTransform.rotation = quaternion * rhs;
						float y = this.targetTransform.rotation.eulerAngles.y;
						this.yaw = y;
						float angle = 0f;
						if (this.moveVector.sqrMagnitude > 0f)
						{
							angle = Util.AngleSigned(Vector3.ProjectOnPlane(this.moveVector, Vector3.up), this.targetTransform.forward, -Vector3.up);
						}
						int num = CharacterDirection.PickIndex(angle);
						CharacterDirection.turnAnimatorParamsSets[num].Apply(this.modelAnimator);
					}
				}
				this.targetVector = this.moveVector;
				this.targetVector.y = 0f;
				if (this.targetVector != Vector3.zero && deltaTime != 0f)
				{
					this.targetVector.Normalize();
					Quaternion quaternion2 = Util.QuaternionSafeLookRotation(this.targetVector, Vector3.up);
					float num2 = Mathf.SmoothDampAngle(this.yaw, quaternion2.eulerAngles.y, ref this.yRotationVelocity, 360f / this.turnSpeed * 0.25f, float.PositiveInfinity, deltaTime);
					quaternion = Quaternion.Euler(0f, num2, 0f);
					this.yaw = num2;
				}
				if (this.targetTransform)
				{
					this.targetTransform.rotation = quaternion;
				}
			}
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040010E7 RID: 4327
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x040010E8 RID: 4328
		[Tooltip("The transform to rotate.")]
		public Transform targetTransform;

		// Token: 0x040010E9 RID: 4329
		[Tooltip("The transform to take the rotation from for animator purposes. Commonly the root node.")]
		public Transform overrideAnimatorForwardTransform;

		// Token: 0x040010EA RID: 4330
		public RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x040010EB RID: 4331
		public Animator modelAnimator;

		// Token: 0x040010EC RID: 4332
		[Tooltip("The character direction is set by root rotation, rather than moveVector.")]
		public bool driveFromRootRotation;

		// Token: 0x040010ED RID: 4333
		[Tooltip("The maximum turn rate in degrees/second.")]
		public float turnSpeed = 360f;

		// Token: 0x040010EE RID: 4334
		private float yRotationVelocity;

		// Token: 0x040010EF RID: 4335
		private float _yaw;

		// Token: 0x040010F0 RID: 4336
		private Vector3 targetVector = Vector3.zero;

		// Token: 0x040010F2 RID: 4338
		private const float offset = 22.5f;

		// Token: 0x040010F3 RID: 4339
		private static readonly CharacterDirection.TurnAnimatorParamsSet[] turnAnimatorParamsSets = new CharacterDirection.TurnAnimatorParamsSet[]
		{
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -180f,
				angleMax = -112.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = true
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -112.5f,
				angleMax = -67.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = true,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = -67.5f,
				angleMax = -22.5f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = true,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 22.5f,
				angleMax = 67.5f,
				turnRight45 = true,
				turnRight90 = false,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 67.5f,
				angleMax = 112.5f,
				turnRight45 = false,
				turnRight90 = true,
				turnRight135 = false,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			},
			new CharacterDirection.TurnAnimatorParamsSet
			{
				angleMin = 112.5f,
				angleMax = 180f,
				turnRight45 = false,
				turnRight90 = false,
				turnRight135 = true,
				turnLeft45 = false,
				turnLeft90 = false,
				turnLeft135 = false
			}
		};

		// Token: 0x040010F4 RID: 4340
		private static readonly int paramsMidIndex = CharacterDirection.turnAnimatorParamsSets.Length >> 1;

		// Token: 0x0200028B RID: 651
		private struct TurnAnimatorParamsSet
		{
			// Token: 0x06000CE1 RID: 3297 RVA: 0x00052974 File Offset: 0x00050B74
			public void Apply(Animator animator)
			{
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight45ParamHash, this.turnRight45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight90ParamHash, this.turnRight90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight135ParamHash, this.turnRight135);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft45ParamHash, this.turnLeft45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft90ParamHash, this.turnLeft90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft135ParamHash, this.turnLeft135);
			}

			// Token: 0x040010F5 RID: 4341
			public float angleMin;

			// Token: 0x040010F6 RID: 4342
			public float angleMax;

			// Token: 0x040010F7 RID: 4343
			public bool turnRight45;

			// Token: 0x040010F8 RID: 4344
			public bool turnRight90;

			// Token: 0x040010F9 RID: 4345
			public bool turnRight135;

			// Token: 0x040010FA RID: 4346
			public bool turnLeft45;

			// Token: 0x040010FB RID: 4347
			public bool turnLeft90;

			// Token: 0x040010FC RID: 4348
			public bool turnLeft135;

			// Token: 0x040010FD RID: 4349
			private static readonly int turnRight45ParamHash = Animator.StringToHash("turnRight45");

			// Token: 0x040010FE RID: 4350
			private static readonly int turnRight90ParamHash = Animator.StringToHash("turnRight90");

			// Token: 0x040010FF RID: 4351
			private static readonly int turnRight135ParamHash = Animator.StringToHash("turnRight135");

			// Token: 0x04001100 RID: 4352
			private static readonly int turnLeft45ParamHash = Animator.StringToHash("turnLeft45");

			// Token: 0x04001101 RID: 4353
			private static readonly int turnLeft90ParamHash = Animator.StringToHash("turnLeft90");

			// Token: 0x04001102 RID: 4354
			private static readonly int turnLeft135ParamHash = Animator.StringToHash("turnLeft135");
		}
	}
}
