using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000429 RID: 1065
	public class WheelVehicleMotor : MonoBehaviour
	{
		// Token: 0x060017EE RID: 6126 RVA: 0x00011F74 File Offset: 0x00010174
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x0007B6C0 File Offset: 0x000798C0
		private void UpdateWheelParameter(WheelCollider wheel)
		{
			wheel.mass = this.wheelMass;
			wheel.radius = this.wheelRadius;
			wheel.suspensionDistance = this.wheelSuspensionDistance;
			wheel.forceAppPointDistance = this.wheelForceAppPointDistance;
			wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, -this.wheelWellDistance, wheel.transform.localPosition.z);
			wheel.suspensionSpring = new JointSpring
			{
				spring = this.wheelSuspensionSpringSpring,
				damper = this.wheelSuspensionSpringDamper,
				targetPosition = this.wheelSuspensionSpringTargetPosition
			};
			wheel.forwardFriction = new WheelFrictionCurve
			{
				extremumSlip = this.forwardFrictionExtremumSlip,
				extremumValue = this.forwardFrictionValue,
				asymptoteSlip = this.forwardFrictionAsymptoticSlip,
				asymptoteValue = this.forwardFrictionAsymptoticValue,
				stiffness = this.forwardFrictionStiffness
			};
			wheel.sidewaysFriction = new WheelFrictionCurve
			{
				extremumSlip = this.sidewaysFrictionExtremumSlip,
				extremumValue = this.sidewaysFrictionValue,
				asymptoteSlip = this.sidewaysFrictionAsymptoticSlip,
				asymptoteValue = this.sidewaysFrictionAsymptoticValue,
				stiffness = this.sidewaysFrictionStiffness
			};
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x0007B80C File Offset: 0x00079A0C
		private void UpdateAllWheelParameters()
		{
			foreach (WheelCollider wheel in this.driveWheels)
			{
				this.UpdateWheelParameter(wheel);
			}
			foreach (WheelCollider wheel2 in this.steerWheels)
			{
				this.UpdateWheelParameter(wheel2);
			}
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x0007B85C File Offset: 0x00079A5C
		private void FixedUpdate()
		{
			this.UpdateAllWheelParameters();
			if (this.inputBank)
			{
				this.moveVector = this.inputBank.moveVector;
				float f = 0f;
				if (this.moveVector.sqrMagnitude > 0f)
				{
					f = Util.AngleSigned(base.transform.forward, this.moveVector, Vector3.up);
				}
				WheelCollider[] array = this.steerWheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].steerAngle = Mathf.Min(this.maxSteerAngle, Mathf.Abs(f)) * Mathf.Sign(f);
				}
				array = this.driveWheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].motorTorque = this.moveVector.magnitude * this.motorTorque;
				}
			}
		}

		// Token: 0x04001AD9 RID: 6873
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x04001ADA RID: 6874
		public WheelCollider[] driveWheels;

		// Token: 0x04001ADB RID: 6875
		public WheelCollider[] steerWheels;

		// Token: 0x04001ADC RID: 6876
		public float motorTorque;

		// Token: 0x04001ADD RID: 6877
		public float maxSteerAngle;

		// Token: 0x04001ADE RID: 6878
		public float wheelMass = 20f;

		// Token: 0x04001ADF RID: 6879
		public float wheelRadius = 0.5f;

		// Token: 0x04001AE0 RID: 6880
		public float wheelWellDistance = 2.7f;

		// Token: 0x04001AE1 RID: 6881
		public float wheelSuspensionDistance = 0.3f;

		// Token: 0x04001AE2 RID: 6882
		public float wheelForceAppPointDistance;

		// Token: 0x04001AE3 RID: 6883
		public float wheelSuspensionSpringSpring = 35000f;

		// Token: 0x04001AE4 RID: 6884
		public float wheelSuspensionSpringDamper = 4500f;

		// Token: 0x04001AE5 RID: 6885
		public float wheelSuspensionSpringTargetPosition = 0.5f;

		// Token: 0x04001AE6 RID: 6886
		public float forwardFrictionExtremumSlip = 0.4f;

		// Token: 0x04001AE7 RID: 6887
		public float forwardFrictionValue = 1f;

		// Token: 0x04001AE8 RID: 6888
		public float forwardFrictionAsymptoticSlip = 0.8f;

		// Token: 0x04001AE9 RID: 6889
		public float forwardFrictionAsymptoticValue = 0.5f;

		// Token: 0x04001AEA RID: 6890
		public float forwardFrictionStiffness = 1f;

		// Token: 0x04001AEB RID: 6891
		public float sidewaysFrictionExtremumSlip = 0.2f;

		// Token: 0x04001AEC RID: 6892
		public float sidewaysFrictionValue = 1f;

		// Token: 0x04001AED RID: 6893
		public float sidewaysFrictionAsymptoticSlip = 0.5f;

		// Token: 0x04001AEE RID: 6894
		public float sidewaysFrictionAsymptoticValue = 0.75f;

		// Token: 0x04001AEF RID: 6895
		public float sidewaysFrictionStiffness = 1f;

		// Token: 0x04001AF0 RID: 6896
		private InputBankTest inputBank;
	}
}
