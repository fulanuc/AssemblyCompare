using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AC RID: 940
	[RequireComponent(typeof(VectorPID))]
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(InputBankTest))]
	public class RigidbodyMotor : MonoBehaviour
	{
		// Token: 0x060013F8 RID: 5112 RVA: 0x0006EEE4 File Offset: 0x0006D0E4
		private void Start()
		{
			Vector3 vector = this.rigid.centerOfMass;
			vector += this.centerOfMassOffset;
			this.rigid.centerOfMass = vector;
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.bodyAnimatorSmoothingParameters = base.GetComponent<BodyAnimatorSmoothingParameters>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					this.animator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x0000F341 File Offset: 0x0000D541
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + this.rigid.centerOfMass, 0.5f);
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x0006EF74 File Offset: 0x0006D174
		public static float GetPitch(Vector3 v)
		{
			float x = Mathf.Sqrt(v.x * v.x + v.z * v.z);
			return -Mathf.Atan2(v.y, x);
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x0006EFB0 File Offset: 0x0006D1B0
		private void Update()
		{
			if (this.animator)
			{
				Vector3 vector = base.transform.InverseTransformVector(this.moveVector) / Mathf.Max(1f, this.moveVector.magnitude);
				BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters = this.bodyAnimatorSmoothingParameters ? this.bodyAnimatorSmoothingParameters.smoothingParameters : BodyAnimatorSmoothingParameters.defaultParameters;
				if (this.animatorForward.Length > 0)
				{
					this.animator.SetFloat(this.animatorForward, vector.z, smoothingParameters.forwardSpeedSmoothDamp, Time.deltaTime);
				}
				if (this.animatorRight.Length > 0)
				{
					this.animator.SetFloat(this.animatorRight, vector.x, smoothingParameters.rightSpeedSmoothDamp, Time.deltaTime);
				}
				if (this.animatorUp.Length > 0)
				{
					this.animator.SetFloat(this.animatorUp, vector.y, smoothingParameters.forwardSpeedSmoothDamp, Time.deltaTime);
				}
			}
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x0006F0AC File Offset: 0x0006D2AC
		private void FixedUpdate()
		{
			if (this.inputBank && this.rigid && this.forcePID)
			{
				Vector3 aimDirection = this.inputBank.aimDirection;
				Vector3 targetVector = this.moveVector;
				this.forcePID.inputVector = this.rigid.velocity;
				this.forcePID.targetVector = targetVector;
				Debug.DrawLine(base.transform.position, base.transform.position + this.forcePID.targetVector, Color.red, 0.1f);
				Vector3 vector = this.forcePID.UpdatePID();
				this.rigid.AddForceAtPosition(Vector3.ClampMagnitude(vector, this.characterBody.acceleration), base.transform.position, ForceMode.Acceleration);
			}
		}

		// Token: 0x040017A2 RID: 6050
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x040017A3 RID: 6051
		public Rigidbody rigid;

		// Token: 0x040017A4 RID: 6052
		public VectorPID forcePID;

		// Token: 0x040017A5 RID: 6053
		public Vector3 centerOfMassOffset;

		// Token: 0x040017A6 RID: 6054
		public string animatorForward;

		// Token: 0x040017A7 RID: 6055
		public string animatorRight;

		// Token: 0x040017A8 RID: 6056
		public string animatorUp;

		// Token: 0x040017A9 RID: 6057
		private CharacterBody characterBody;

		// Token: 0x040017AA RID: 6058
		private InputBankTest inputBank;

		// Token: 0x040017AB RID: 6059
		private ModelLocator modelLocator;

		// Token: 0x040017AC RID: 6060
		private Animator animator;

		// Token: 0x040017AD RID: 6061
		private BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters;
	}
}
