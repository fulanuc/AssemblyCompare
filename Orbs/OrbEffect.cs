using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.Orbs
{
	// Token: 0x02000529 RID: 1321
	[RequireComponent(typeof(EffectComponent))]
	public class OrbEffect : MonoBehaviour
	{
		// Token: 0x06001DDA RID: 7642 RVA: 0x0009131C File Offset: 0x0008F51C
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			this.startPosition = component.effectData.origin;
			this.previousPosition = this.startPosition;
			GameObject gameObject = component.effectData.ResolveHurtBoxReference();
			this.targetTransform = (gameObject ? gameObject.transform : null);
			this.duration = component.effectData.genericFloat;
			if (this.duration == 0f)
			{
				Debug.LogFormat("Zero duration for effect \"{0}\"", new object[]
				{
					base.gameObject.name
				});
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.lastKnownTargetPosition = (this.targetTransform ? this.targetTransform.position : this.startPosition);
			if (this.startEffect)
			{
				EffectData effectData = new EffectData
				{
					origin = base.transform.position,
					scale = this.startEffectScale
				};
				if (this.startEffectCopiesRotation)
				{
					effectData.rotation = base.transform.rotation;
				}
				EffectManager.instance.SpawnEffect(this.startEffect, effectData, false);
			}
			this.startVelocity.x = Mathf.Lerp(this.startVelocity1.x, this.startVelocity2.x, UnityEngine.Random.value);
			this.startVelocity.y = Mathf.Lerp(this.startVelocity1.y, this.startVelocity2.y, UnityEngine.Random.value);
			this.startVelocity.z = Mathf.Lerp(this.startVelocity1.z, this.startVelocity2.z, UnityEngine.Random.value);
			this.endVelocity.x = Mathf.Lerp(this.endVelocity1.x, this.endVelocity2.x, UnityEngine.Random.value);
			this.endVelocity.y = Mathf.Lerp(this.endVelocity1.y, this.endVelocity2.y, UnityEngine.Random.value);
			this.endVelocity.z = Mathf.Lerp(this.endVelocity1.z, this.endVelocity2.z, UnityEngine.Random.value);
			this.UpdateOrb(0f);
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x00015DA3 File Offset: 0x00013FA3
		private void Update()
		{
			this.UpdateOrb(Time.deltaTime);
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x00091548 File Offset: 0x0008F748
		private void UpdateOrb(float deltaTime)
		{
			if (this.targetTransform)
			{
				this.lastKnownTargetPosition = this.targetTransform.position;
			}
			float num = Mathf.Clamp01(this.age / this.duration);
			float num2 = this.movementCurve.Evaluate(num);
			Vector3 vector = Vector3.Lerp(this.startPosition + this.startVelocity * num2, this.lastKnownTargetPosition + this.endVelocity * (1f - num2), num2);
			base.transform.position = vector;
			if (this.faceMovement && vector != this.previousPosition)
			{
				base.transform.forward = vector - this.previousPosition;
			}
			this.UpdateBezier();
			if (num == 1f)
			{
				this.onArrival.Invoke();
				if (this.endEffect)
				{
					EffectData effectData = new EffectData
					{
						origin = base.transform.position,
						scale = this.endEffectScale
					};
					if (this.endEffectCopiesRotation)
					{
						effectData.rotation = base.transform.rotation;
					}
					EffectManager.instance.SpawnEffect(this.endEffect, effectData, false);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.previousPosition = vector;
			this.age += deltaTime;
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x0009169C File Offset: 0x0008F89C
		private void UpdateBezier()
		{
			if (this.bezierCurveLine)
			{
				this.bezierCurveLine.p1 = this.startPosition;
				this.bezierCurveLine.v0 = this.endVelocity;
				this.bezierCurveLine.v1 = this.startVelocity;
				this.bezierCurveLine.UpdateBezier(0f);
			}
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x00015DB0 File Offset: 0x00013FB0
		public void InstantiatePrefab(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x00015DCF File Offset: 0x00013FCF
		public void InstantiateEffect(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position
			}, false);
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x00015DF3 File Offset: 0x00013FF3
		public void InstantiateEffectCopyRotation(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = base.transform.rotation
			}, false);
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x00015E28 File Offset: 0x00014028
		public void InstantiateEffectOppositeFacing(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = Util.QuaternionSafeLookRotation(-base.transform.forward)
			}, false);
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x00015E67 File Offset: 0x00014067
		public void InstantiatePrefabOppositeFacing(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, Util.QuaternionSafeLookRotation(-base.transform.forward));
		}

		// Token: 0x04001FE0 RID: 8160
		private Transform targetTransform;

		// Token: 0x04001FE1 RID: 8161
		private float duration;

		// Token: 0x04001FE2 RID: 8162
		private Vector3 startPosition;

		// Token: 0x04001FE3 RID: 8163
		private Vector3 previousPosition;

		// Token: 0x04001FE4 RID: 8164
		private Vector3 lastKnownTargetPosition;

		// Token: 0x04001FE5 RID: 8165
		private float age;

		// Token: 0x04001FE6 RID: 8166
		[Header("Curve Parameters")]
		public Vector3 startVelocity1;

		// Token: 0x04001FE7 RID: 8167
		public Vector3 startVelocity2;

		// Token: 0x04001FE8 RID: 8168
		public Vector3 endVelocity1;

		// Token: 0x04001FE9 RID: 8169
		public Vector3 endVelocity2;

		// Token: 0x04001FEA RID: 8170
		private Vector3 startVelocity;

		// Token: 0x04001FEB RID: 8171
		private Vector3 endVelocity;

		// Token: 0x04001FEC RID: 8172
		public AnimationCurve movementCurve;

		// Token: 0x04001FED RID: 8173
		public BezierCurveLine bezierCurveLine;

		// Token: 0x04001FEE RID: 8174
		public bool faceMovement = true;

		// Token: 0x04001FEF RID: 8175
		[Header("Start Effect")]
		[Tooltip("An effect prefab to spawn on Start")]
		public GameObject startEffect;

		// Token: 0x04001FF0 RID: 8176
		public float startEffectScale = 1f;

		// Token: 0x04001FF1 RID: 8177
		public bool startEffectCopiesRotation;

		// Token: 0x04001FF2 RID: 8178
		[Tooltip("An effect prefab to spawn on end")]
		[Header("End Effect")]
		public GameObject endEffect;

		// Token: 0x04001FF3 RID: 8179
		public float endEffectScale = 1f;

		// Token: 0x04001FF4 RID: 8180
		public bool endEffectCopiesRotation;

		// Token: 0x04001FF5 RID: 8181
		[Header("Arrival Behavior")]
		public UnityEvent onArrival;
	}
}
