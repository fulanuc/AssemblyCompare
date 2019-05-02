using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.Orbs
{
	// Token: 0x0200051A RID: 1306
	[RequireComponent(typeof(EffectComponent))]
	public class OrbEffect : MonoBehaviour
	{
		// Token: 0x06001D72 RID: 7538 RVA: 0x000905A8 File Offset: 0x0008E7A8
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

		// Token: 0x06001D73 RID: 7539 RVA: 0x000158DA File Offset: 0x00013ADA
		private void Update()
		{
			this.UpdateOrb(Time.deltaTime);
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x000907D4 File Offset: 0x0008E9D4
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

		// Token: 0x06001D75 RID: 7541 RVA: 0x00090928 File Offset: 0x0008EB28
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

		// Token: 0x06001D76 RID: 7542 RVA: 0x000158E7 File Offset: 0x00013AE7
		public void InstantiatePrefab(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x00015906 File Offset: 0x00013B06
		public void InstantiateEffect(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position
			}, false);
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x0001592A File Offset: 0x00013B2A
		public void InstantiateEffectCopyRotation(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = base.transform.rotation
			}, false);
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0001595F File Offset: 0x00013B5F
		public void InstantiateEffectOppositeFacing(GameObject prefab)
		{
			EffectManager.instance.SpawnEffect(prefab, new EffectData
			{
				origin = base.transform.position,
				rotation = Util.QuaternionSafeLookRotation(-base.transform.forward)
			}, false);
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0001599E File Offset: 0x00013B9E
		public void InstantiatePrefabOppositeFacing(GameObject prefab)
		{
			UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform.position, Util.QuaternionSafeLookRotation(-base.transform.forward));
		}

		// Token: 0x04001FA2 RID: 8098
		private Transform targetTransform;

		// Token: 0x04001FA3 RID: 8099
		private float duration;

		// Token: 0x04001FA4 RID: 8100
		private Vector3 startPosition;

		// Token: 0x04001FA5 RID: 8101
		private Vector3 previousPosition;

		// Token: 0x04001FA6 RID: 8102
		private Vector3 lastKnownTargetPosition;

		// Token: 0x04001FA7 RID: 8103
		private float age;

		// Token: 0x04001FA8 RID: 8104
		[Header("Curve Parameters")]
		public Vector3 startVelocity1;

		// Token: 0x04001FA9 RID: 8105
		public Vector3 startVelocity2;

		// Token: 0x04001FAA RID: 8106
		public Vector3 endVelocity1;

		// Token: 0x04001FAB RID: 8107
		public Vector3 endVelocity2;

		// Token: 0x04001FAC RID: 8108
		private Vector3 startVelocity;

		// Token: 0x04001FAD RID: 8109
		private Vector3 endVelocity;

		// Token: 0x04001FAE RID: 8110
		public AnimationCurve movementCurve;

		// Token: 0x04001FAF RID: 8111
		public BezierCurveLine bezierCurveLine;

		// Token: 0x04001FB0 RID: 8112
		public bool faceMovement = true;

		// Token: 0x04001FB1 RID: 8113
		[Tooltip("An effect prefab to spawn on Start")]
		[Header("Start Effect")]
		public GameObject startEffect;

		// Token: 0x04001FB2 RID: 8114
		public float startEffectScale = 1f;

		// Token: 0x04001FB3 RID: 8115
		public bool startEffectCopiesRotation;

		// Token: 0x04001FB4 RID: 8116
		[Header("End Effect")]
		[Tooltip("An effect prefab to spawn on end")]
		public GameObject endEffect;

		// Token: 0x04001FB5 RID: 8117
		public float endEffectScale = 1f;

		// Token: 0x04001FB6 RID: 8118
		public bool endEffectCopiesRotation;

		// Token: 0x04001FB7 RID: 8119
		[Header("Arrival Behavior")]
		public UnityEvent onArrival;
	}
}
