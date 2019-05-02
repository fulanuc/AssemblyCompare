using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E0 RID: 992
	public class ShakeEmitter : MonoBehaviour
	{
		// Token: 0x060015A3 RID: 5539 RVA: 0x000105E6 File Offset: 0x0000E7E6
		public void StartShake()
		{
			this.stopwatch = 0f;
			this.halfPeriodVector = UnityEngine.Random.onUnitSphere;
			this.halfPeriodTimer = this.wave.period * 0.5f;
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00010615 File Offset: 0x0000E815
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

		// Token: 0x060015A5 RID: 5541 RVA: 0x0001064A File Offset: 0x0000E84A
		private void OnEnable()
		{
			ShakeEmitter.instances.Add(this);
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x00010657 File Offset: 0x0000E857
		private void OnDisable()
		{
			ShakeEmitter.instances.Remove(this);
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x00010665 File Offset: 0x0000E865
		private void OnValidate()
		{
			if (this.wave.frequency == 0f)
			{
				this.wave.frequency = 1f;
				Debug.Log("ShakeEmitter with wave frequency 0.0 is not allowed!");
			}
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00010693 File Offset: 0x0000E893
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += ShakeEmitter.UpdateAll;
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x000739AC File Offset: 0x00071BAC
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

		// Token: 0x060015AA RID: 5546 RVA: 0x000106A6 File Offset: 0x0000E8A6
		public float CurrentShakeFade()
		{
			if (!this.amplitudeTimeDecay)
			{
				return 1f;
			}
			return 1f - this.stopwatch / this.duration;
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x000739F0 File Offset: 0x00071BF0
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

		// Token: 0x060015AC RID: 5548 RVA: 0x00073AB0 File Offset: 0x00071CB0
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

		// Token: 0x060015AD RID: 5549 RVA: 0x00073BB8 File Offset: 0x00071DB8
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

		// Token: 0x060015AE RID: 5550 RVA: 0x00073C44 File Offset: 0x00071E44
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

		// Token: 0x040018CD RID: 6349
		private static readonly List<ShakeEmitter> instances = new List<ShakeEmitter>();

		// Token: 0x040018CE RID: 6350
		[Tooltip("Whether or not to begin shaking as soon as this instance becomes active.")]
		public bool shakeOnStart = true;

		// Token: 0x040018CF RID: 6351
		[Tooltip("The wave description of this motion.")]
		public Wave wave = new Wave
		{
			amplitude = 1f,
			frequency = 1f,
			cycleOffset = 0f
		};

		// Token: 0x040018D0 RID: 6352
		[Tooltip("How long the shake lasts, in seconds.")]
		public float duration = 1f;

		// Token: 0x040018D1 RID: 6353
		[Tooltip("How far the wave reaches.")]
		public float radius = 10f;

		// Token: 0x040018D2 RID: 6354
		[Tooltip("Whether or not the radius should be multiplied with local scale.")]
		public bool scaleShakeRadiusWithLocalScale;

		// Token: 0x040018D3 RID: 6355
		[Tooltip("Whether or not the ampitude should decay with time.")]
		public bool amplitudeTimeDecay = true;

		// Token: 0x040018D4 RID: 6356
		private float stopwatch = float.PositiveInfinity;

		// Token: 0x040018D5 RID: 6357
		private float halfPeriodTimer;

		// Token: 0x040018D6 RID: 6358
		private Vector3 halfPeriodVector;

		// Token: 0x040018D7 RID: 6359
		private Vector3 currentOffset;

		// Token: 0x040018D8 RID: 6360
		private const float deepRumbleFactor = 5f;

		// Token: 0x020003E1 RID: 993
		public struct MotorBias
		{
			// Token: 0x040018D9 RID: 6361
			public float deepLeftBias;

			// Token: 0x040018DA RID: 6362
			public float quickRightBias;
		}
	}
}
