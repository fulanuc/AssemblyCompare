using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002EF RID: 751
	public class ExperienceOrbBehavior : MonoBehaviour
	{
		// Token: 0x06000F39 RID: 3897 RVA: 0x0000BB95 File Offset: 0x00009D95
		private void Awake()
		{
			this.transform = base.transform;
			this.trail = base.GetComponent<TrailRenderer>();
			this.light = base.GetComponent<Light>();
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0005C218 File Offset: 0x0005A418
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

		// Token: 0x06000F3B RID: 3899 RVA: 0x0005C30C File Offset: 0x0005A50C
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

		// Token: 0x06000F3C RID: 3900 RVA: 0x0005C3A0 File Offset: 0x0005A5A0
		private static Vector3 CalculatePosition(Vector3 startPos, Vector3 initialVelocity, Vector3 targetPos, float t)
		{
			Vector3 a = startPos + initialVelocity * t;
			float t2 = t * t * t;
			return Vector3.LerpUnclamped(a, targetPos, t2);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0000BBBB File Offset: 0x00009DBB
		private void OnTriggerStay(Collider other)
		{
			if (other.transform == this.targetTransform)
			{
				this.OnHitTarget();
			}
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0005C3CC File Offset: 0x0005A5CC
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

		// Token: 0x04001339 RID: 4921
		public GameObject hitEffectPrefab;

		// Token: 0x0400133A RID: 4922
		private static string expSoundString = "Play_UI_xp_gain";

		// Token: 0x0400133B RID: 4923
		private new Transform transform;

		// Token: 0x0400133C RID: 4924
		private TrailRenderer trail;

		// Token: 0x0400133D RID: 4925
		private Light light;

		// Token: 0x0400133E RID: 4926
		[HideInInspector]
		public Transform targetTransform;

		// Token: 0x0400133F RID: 4927
		[HideInInspector]
		public float travelTime;

		// Token: 0x04001340 RID: 4928
		[HideInInspector]
		public ulong exp;

		// Token: 0x04001341 RID: 4929
		private float localTime;

		// Token: 0x04001342 RID: 4930
		private Vector3 startPos;

		// Token: 0x04001343 RID: 4931
		private Vector3 previousPos;

		// Token: 0x04001344 RID: 4932
		private Vector3 initialVelocity;

		// Token: 0x04001345 RID: 4933
		private float scale;

		// Token: 0x04001346 RID: 4934
		private bool consumed;
	}
}
