using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DA RID: 986
	public class ShakeEmitter : MonoBehaviour
	{
		// Token: 0x06001565 RID: 5477 RVA: 0x000101DD File Offset: 0x0000E3DD
		public void StartShake()
		{
			this.stopwatch = 0f;
			this.halfPeriodVector = UnityEngine.Random.onUnitSphere;
			this.halfPeriodTimer = this.wave.period * 0.5f;
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x0001020C File Offset: 0x0000E40C
		private void Start()
		{
			if (this.scaleShakeRadiusWithLocalScale)
			{
				this.radius *= base.transform.localScale.x;
			}
			if (this.shakeOnStart)
			{
				this.StartShake();
			}
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00010241 File Offset: 0x0000E441
		private void OnEnable()
		{
			ShakeEmitter.instances.Add(this);
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0001024E File Offset: 0x0000E44E
		private void OnDisable()
		{
			ShakeEmitter.instances.Remove(this);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0001025C File Offset: 0x0000E45C
		private void OnValidate()
		{
			if (this.wave.frequency == 0f)
			{
				this.wave.frequency = 1f;
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0001028A File Offset: 0x0000E48A
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ShakeEmitter.UpdateAll;
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0007332C File Offset: 0x0007152C
		public static void UpdateAll()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime == 0f)
			{
				return;
			}
			for (int i = 0; i < ShakeEmitter.instances.Count; i++)
			{
				ShakeEmitter.instances[i].ManualUpdate(deltaTime);
			}
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x0001029D File Offset: 0x0000E49D
		public float CurrentShakeFade()
		{
			if (!this.amplitudeTimeDecay)
			{
				return 1f;
			}
			return 1f - this.stopwatch / this.duration;
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00073370 File Offset: 0x00071570
		public void ManualUpdate(float deltaTime)
		{
			this.stopwatch += deltaTime;
			if (this.stopwatch < this.duration)
			{
				float d = this.CurrentShakeFade();
				this.halfPeriodTimer -= deltaTime;
				if (this.halfPeriodTimer < 0f)
				{
					this.halfPeriodVector = Vector3.Slerp(UnityEngine.Random.onUnitSphere, -this.halfPeriodVector, 0.5f);
					this.halfPeriodTimer += this.wave.period * 0.5f;
				}
				this.currentOffset = this.halfPeriodVector * this.wave.Evaluate(this.halfPeriodTimer) * d;
				return;
			}
			this.currentOffset = Vector3.zero;
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00073430 File Offset: 0x00071630
		public static void ApplySpacialRumble(LocalUser localUser, Transform cameraTransform)
		{
			Vector3 right = cameraTransform.right;
			Vector3 position = cameraTransform.position;
			float num = 0f;
			float num2 = 0f;
			int i = 0;
			int count = ShakeEmitter.instances.Count;
			while (i < count)
			{
				ShakeEmitter shakeEmitter = ShakeEmitter.instances[i];
				Vector3 position2 = shakeEmitter.transform.position;
				float value = Vector3.Dot(position2 - position, right);
				float sqrMagnitude = (position - position2).sqrMagnitude;
				float num3 = shakeEmitter.radius;
				float num4 = 0f;
				if (sqrMagnitude < num3 * num3)
				{
					float num5 = 1f - Mathf.Sqrt(sqrMagnitude) / num3;
					num4 = shakeEmitter.CurrentShakeFade() * shakeEmitter.wave.amplitude * num5;
				}
				float num6 = Mathf.Clamp01(Util.Remap(value, -1f, 1f, 0f, 1f));
				float num7 = num4;
				num += num7 * (1f - num6);
				num2 += num7 * num6;
				i++;
			}
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00073538 File Offset: 0x00071738
		public static void ApplyDeepQuickRumble(LocalUser localUser, Vector3 bonusPosition)
		{
			float magnitude = bonusPosition.magnitude;
			float gamepadVibrationScale = localUser.userProfile.gamepadVibrationScale;
			localUser.inputPlayer.SetVibration(0, magnitude * gamepadVibrationScale / 5f);
			localUser.inputPlayer.SetVibration(1, magnitude * gamepadVibrationScale);
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x00073580 File Offset: 0x00071780
		public static Vector3 ComputeTotalShakeAtPoint(Vector3 position)
		{
			Vector3 vector = Vector3.zero;
			int i = 0;
			int count = ShakeEmitter.instances.Count;
			while (i < count)
			{
				ShakeEmitter shakeEmitter = ShakeEmitter.instances[i];
				float sqrMagnitude = (position - shakeEmitter.transform.position).sqrMagnitude;
				float num = shakeEmitter.radius;
				if (sqrMagnitude < num * num)
				{
					float d = 1f - Mathf.Sqrt(sqrMagnitude) / num;
					vector += shakeEmitter.currentOffset * d;
				}
				i++;
			}
			return vector;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x0007360C File Offset: 0x0007180C
		public static ShakeEmitter CreateSimpleShakeEmitter(Vector3 position, Wave wave, float duration, float radius, bool amplitudeTimeDecay)
		{
			if (wave.frequency == 0f)
			{
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
				wave.frequency = 1f;
			}
			GameObject gameObject = new GameObject("ShakeEmitter", new Type[]
			{
				typeof(ShakeEmitter),
				typeof(DestroyOnTimer)
			});
			ShakeEmitter component = gameObject.GetComponent<ShakeEmitter>();
			DestroyOnTimer component2 = gameObject.GetComponent<DestroyOnTimer>();
			gameObject.transform.position = position;
			component.wave = wave;
			component.duration = duration;
			component.radius = radius;
			component.amplitudeTimeDecay = amplitudeTimeDecay;
			component2.duration = duration;
			return component;
		}

		// Token: 0x040018A4 RID: 6308
		private static readonly List<ShakeEmitter> instances = new List<ShakeEmitter>();

		// Token: 0x040018A5 RID: 6309
		[Tooltip("Whether or not to begin shaking as soon as this instance becomes active.")]
		public bool shakeOnStart = true;

		// Token: 0x040018A6 RID: 6310
		[Tooltip("The wave description of this motion.")]
		public Wave wave = new Wave
		{
			amplitude = 1f,
			frequency = 1f,
			cycleOffset = 0f
		};

		// Token: 0x040018A7 RID: 6311
		[Tooltip("How long the shake lasts, in seconds.")]
		public float duration = 1f;

		// Token: 0x040018A8 RID: 6312
		[Tooltip("How far the wave reaches.")]
		public float radius = 10f;

		// Token: 0x040018A9 RID: 6313
		[Tooltip("Whether or not the radius should be multiplied with local scale.")]
		public bool scaleShakeRadiusWithLocalScale;

		// Token: 0x040018AA RID: 6314
		[Tooltip("Whether or not the ampitude should decay with time.")]
		public bool amplitudeTimeDecay = true;

		// Token: 0x040018AB RID: 6315
		private float stopwatch = float.PositiveInfinity;

		// Token: 0x040018AC RID: 6316
		private float halfPeriodTimer;

		// Token: 0x040018AD RID: 6317
		private Vector3 halfPeriodVector;

		// Token: 0x040018AE RID: 6318
		private Vector3 currentOffset;

		// Token: 0x040018AF RID: 6319
		private const float deepRumbleFactor = 5f;

		// Token: 0x020003DB RID: 987
		public struct MotorBias
		{
			// Token: 0x040018B0 RID: 6320
			public float deepLeftBias;

			// Token: 0x040018B1 RID: 6321
			public float quickRightBias;
		}
	}
}
