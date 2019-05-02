using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A6 RID: 934
	[RequireComponent(typeof(QuaternionPID))]
	[RequireComponent(typeof(VectorPID))]
	public class RigidbodyDirection : MonoBehaviour
	{
		// Token: 0x060013D7 RID: 5079 RVA: 0x0006EA64 File Offset: 0x0006CC64
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					this.animator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0006EAB8 File Offset: 0x0006CCB8
		private void Update()
		{
			if (this.animator)
			{
				if (this.animatorXCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorXCycle, Mathf.Clamp(0.5f + this.targetTorque.x * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
				if (this.animatorYCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorYCycle, Mathf.Clamp(0.5f + this.targetTorque.y * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
				if (this.animatorZCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorZCycle, Mathf.Clamp(0.5f + this.targetTorque.z * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
			}
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x0006EBD8 File Offset: 0x0006CDD8
		private void FixedUpdate()
		{
			if (this.inputBank && this.rigid && this.angularVelocityPID && this.torquePID)
			{
				this.angularVelocityPID.inputQuat = this.rigid.transform.rotation;
				Quaternion targetQuat = Util.QuaternionSafeLookRotation(this.aimDirection);
				if (this.freezeXRotation)
				{
					targetQuat.x = 0f;
				}
				if (this.freezeYRotation)
				{
					targetQuat.y = 0f;
				}
				if (this.freezeZRotation)
				{
					targetQuat.z = 0f;
				}
				this.angularVelocityPID.targetQuat = targetQuat;
				Vector3 targetVector = this.angularVelocityPID.UpdatePID();
				this.torquePID.inputVector = this.rigid.angularVelocity;
				this.torquePID.targetVector = targetVector;
				Vector3 torque = this.torquePID.UpdatePID();
				this.rigid.AddTorque(torque, ForceMode.Acceleration);
			}
		}

		// Token: 0x04001779 RID: 6009
		public Vector3 aimDirection = Vector3.one;

		// Token: 0x0400177A RID: 6010
		public Rigidbody rigid;

		// Token: 0x0400177B RID: 6011
		public QuaternionPID angularVelocityPID;

		// Token: 0x0400177C RID: 6012
		public VectorPID torquePID;

		// Token: 0x0400177D RID: 6013
		public bool freezeXRotation;

		// Token: 0x0400177E RID: 6014
		public bool freezeYRotation;

		// Token: 0x0400177F RID: 6015
		public bool freezeZRotation;

		// Token: 0x04001780 RID: 6016
		private ModelLocator modelLocator;

		// Token: 0x04001781 RID: 6017
		private Animator animator;

		// Token: 0x04001782 RID: 6018
		public string animatorXCycle;

		// Token: 0x04001783 RID: 6019
		public string animatorYCycle;

		// Token: 0x04001784 RID: 6020
		public string animatorZCycle;

		// Token: 0x04001785 RID: 6021
		public float animatorTorqueScale;

		// Token: 0x04001786 RID: 6022
		private InputBankTest inputBank;

		// Token: 0x04001787 RID: 6023
		private Vector3 targetTorque;
	}
}
