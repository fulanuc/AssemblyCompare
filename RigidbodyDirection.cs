using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003AB RID: 939
	[RequireComponent(typeof(VectorPID))]
	[RequireComponent(typeof(QuaternionPID))]
	public class RigidbodyDirection : MonoBehaviour
	{
		// Token: 0x060013F4 RID: 5108 RVA: 0x0006EC6C File Offset: 0x0006CE6C
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

		// Token: 0x060013F5 RID: 5109 RVA: 0x0006ECC0 File Offset: 0x0006CEC0
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

		// Token: 0x060013F6 RID: 5110 RVA: 0x0006EDE0 File Offset: 0x0006CFE0
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

		// Token: 0x04001793 RID: 6035
		public Vector3 aimDirection = Vector3.one;

		// Token: 0x04001794 RID: 6036
		public Rigidbody rigid;

		// Token: 0x04001795 RID: 6037
		public QuaternionPID angularVelocityPID;

		// Token: 0x04001796 RID: 6038
		public VectorPID torquePID;

		// Token: 0x04001797 RID: 6039
		public bool freezeXRotation;

		// Token: 0x04001798 RID: 6040
		public bool freezeYRotation;

		// Token: 0x04001799 RID: 6041
		public bool freezeZRotation;

		// Token: 0x0400179A RID: 6042
		private ModelLocator modelLocator;

		// Token: 0x0400179B RID: 6043
		private Animator animator;

		// Token: 0x0400179C RID: 6044
		public string animatorXCycle;

		// Token: 0x0400179D RID: 6045
		public string animatorYCycle;

		// Token: 0x0400179E RID: 6046
		public string animatorZCycle;

		// Token: 0x0400179F RID: 6047
		public float animatorTorqueScale;

		// Token: 0x040017A0 RID: 6048
		private InputBankTest inputBank;

		// Token: 0x040017A1 RID: 6049
		private Vector3 targetTorque;
	}
}
