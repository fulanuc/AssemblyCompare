using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200028A RID: 650
	public class CharacterDirection : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x00009FFB File Offset: 0x000081FB
		// (set) Token: 0x06000CC0 RID: 3264 RVA: 0x00009FC5 File Offset: 0x000081C5
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

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x000520B0 File Offset: 0x000502B0
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

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0000A003 File Offset: 0x00008203
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x000520FC File Offset: 0x000502FC
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

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0000A02D File Offset: 0x0000822D
		// (set) Token: 0x06000CC5 RID: 3269 RVA: 0x0000A024 File Offset: 0x00008224
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0000A035 File Offset: 0x00008235
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0000A048 File Offset: 0x00008248
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0000A048 File Offset: 0x00008248
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x00052134 File Offset: 0x00050334
		private void Start()
		{
			this.UpdateAuthority();
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				this.modelAnimator = component.modelTransform.GetComponent<Animator>();
			}
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0000A050 File Offset: 0x00008250
		private void Update()
		{
			this.Simulate(Time.deltaTime);
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x0000A05D File Offset: 0x0000825D
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x00052168 File Offset: 0x00050368
		private static int PickIndex(float angle)
		{
			float num = Mathf.Sign(angle);
			int num2 = Mathf.CeilToInt((angle * num - 22.5f) * 0.0222222228f);
			return Mathf.Clamp(CharacterDirection.paramsMidIndex + num2 * (int)num, 0, CharacterDirection.turnAnimatorParamsSets.Length - 1);
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x000521AC File Offset: 0x000503AC
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

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040010DC RID: 4316
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x040010DD RID: 4317
		[Tooltip("The transform to rotate.")]
		public Transform targetTransform;

		// Token: 0x040010DE RID: 4318
		[Tooltip("The transform to take the rotation from for animator purposes. Commonly the root node.")]
		public Transform overrideAnimatorForwardTransform;

		// Token: 0x040010DF RID: 4319
		public RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x040010E0 RID: 4320
		public Animator modelAnimator;

		// Token: 0x040010E1 RID: 4321
		[Tooltip("The character direction is set by root rotation, rather than moveVector.")]
		public bool driveFromRootRotation;

		// Token: 0x040010E2 RID: 4322
		[Tooltip("The maximum turn rate in degrees/second.")]
		public float turnSpeed = 360f;

		// Token: 0x040010E3 RID: 4323
		private float yRotationVelocity;

		// Token: 0x040010E4 RID: 4324
		private float _yaw;

		// Token: 0x040010E5 RID: 4325
		private Vector3 targetVector = Vector3.zero;

		// Token: 0x040010E7 RID: 4327
		private const float offset = 22.5f;

		// Token: 0x040010E8 RID: 4328
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

		// Token: 0x040010E9 RID: 4329
		private static readonly int paramsMidIndex = CharacterDirection.turnAnimatorParamsSets.Length >> 1;

		// Token: 0x0200028B RID: 651
		private struct TurnAnimatorParamsSet
		{
			// Token: 0x06000CD4 RID: 3284 RVA: 0x000525CC File Offset: 0x000507CC
			public void Apply(Animator animator)
			{
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight45ParamHash, this.turnRight45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight90ParamHash, this.turnRight90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnRight135ParamHash, this.turnRight135);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft45ParamHash, this.turnLeft45);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft90ParamHash, this.turnLeft90);
				animator.SetBool(CharacterDirection.TurnAnimatorParamsSet.turnLeft135ParamHash, this.turnLeft135);
			}

			// Token: 0x040010EA RID: 4330
			public float angleMin;

			// Token: 0x040010EB RID: 4331
			public float angleMax;

			// Token: 0x040010EC RID: 4332
			public bool turnRight45;

			// Token: 0x040010ED RID: 4333
			public bool turnRight90;

			// Token: 0x040010EE RID: 4334
			public bool turnRight135;

			// Token: 0x040010EF RID: 4335
			public bool turnLeft45;

			// Token: 0x040010F0 RID: 4336
			public bool turnLeft90;

			// Token: 0x040010F1 RID: 4337
			public bool turnLeft135;

			// Token: 0x040010F2 RID: 4338
			private static readonly int turnRight45ParamHash = Animator.StringToHash("turnRight45");

			// Token: 0x040010F3 RID: 4339
			private static readonly int turnRight90ParamHash = Animator.StringToHash("turnRight90");

			// Token: 0x040010F4 RID: 4340
			private static readonly int turnRight135ParamHash = Animator.StringToHash("turnRight135");

			// Token: 0x040010F5 RID: 4341
			private static readonly int turnLeft45ParamHash = Animator.StringToHash("turnLeft45");

			// Token: 0x040010F6 RID: 4342
			private static readonly int turnLeft90ParamHash = Animator.StringToHash("turnLeft90");

			// Token: 0x040010F7 RID: 4343
			private static readonly int turnLeft135ParamHash = Animator.StringToHash("turnLeft135");
		}
	}
}
