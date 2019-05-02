using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F2 RID: 754
	public class ExperienceOrbBehavior : MonoBehaviour
	{
		// Token: 0x06000F49 RID: 3913 RVA: 0x0000BC43 File Offset: 0x00009E43
		private void Awake()
		{
			this.transform = base.transform;
			this.trail = base.GetComponent<TrailRenderer>();
			this.light = base.GetComponent<Light>();
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0005C438 File Offset: 0x0005A638
		private void Start()
		{
			this.localTime = 0f;
			this.consumed = false;
			this.startPos = this.transform.position;
			this.previousPos = this.startPos;
			this.scale = 2f * Mathf.Log(this.exp + 1f, 6f);
			this.initialVelocity = (Vector3.up * 4f + UnityEngine.Random.insideUnitSphere * 1f) * this.scale;
			this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
			this.trail.startWidth = 0.05f * this.scale;
			if (this.light)
			{
				this.light.range = 1f * this.scale;
			}
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0005C52C File Offset: 0x0005A72C
		private void Update()
		{
			this.localTime += Time.deltaTime;
			if (!this.targetTransform)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			float num = Mathf.Clamp01(this.localTime / this.travelTime);
			this.previousPos = this.transform.position;
			this.transform.position = ExperienceOrbBehavior.CalculatePosition(this.startPos, this.initialVelocity, this.targetTransform.position, num);
			if (num >= 1f)
			{
				this.OnHitTarget();
				return;
			}
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0005C5C0 File Offset: 0x0005A7C0
		private static Vector3 CalculatePosition(Vector3 startPos, Vector3 initialVelocity, Vector3 targetPos, float t)
		{
			Vector3 a = startPos + initialVelocity * t;
			float t2 = t * t * t;
			return Vector3.LerpUnclamped(a, targetPos, t2);
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0000BC69 File Offset: 0x00009E69
		private void OnTriggerStay(Collider other)
		{
			if (other.transform == this.targetTransform)
			{
				this.OnHitTarget();
			}
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0005C5EC File Offset: 0x0005A7EC
		private void OnHitTarget()
		{
			if (!this.consumed)
			{
				this.consumed = true;
				Util.PlaySound(ExperienceOrbBehavior.expSoundString, this.targetTransform.gameObject);
				UnityEngine.Object.Instantiate<GameObject>(this.hitEffectPrefab, this.transform.position, Util.QuaternionSafeLookRotation(this.previousPos - this.startPos)).transform.localScale *= this.scale;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04001350 RID: 4944
		public GameObject hitEffectPrefab;

		// Token: 0x04001351 RID: 4945
		private static string expSoundString = "Play_UI_xp_gain";

		// Token: 0x04001352 RID: 4946
		private new Transform transform;

		// Token: 0x04001353 RID: 4947
		private TrailRenderer trail;

		// Token: 0x04001354 RID: 4948
		private Light light;

		// Token: 0x04001355 RID: 4949
		[HideInInspector]
		public Transform targetTransform;

		// Token: 0x04001356 RID: 4950
		[HideInInspector]
		public float travelTime;

		// Token: 0x04001357 RID: 4951
		[HideInInspector]
		public ulong exp;

		// Token: 0x04001358 RID: 4952
		private float localTime;

		// Token: 0x04001359 RID: 4953
		private Vector3 startPos;

		// Token: 0x0400135A RID: 4954
		private Vector3 previousPos;

		// Token: 0x0400135B RID: 4955
		private Vector3 initialVelocity;

		// Token: 0x0400135C RID: 4956
		private float scale;

		// Token: 0x0400135D RID: 4957
		private bool consumed;
	}
}
