using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000423 RID: 1059
	public class WheelVehicleMotor : MonoBehaviour
	{
		// Token: 0x060017AA RID: 6058 RVA: 0x00011B42 File Offset: 0x0000FD42
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x0007B100 File Offset: 0x00079300
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

		// Token: 0x060017AC RID: 6060 RVA: 0x0007B24C File Offset: 0x0007944C
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

		// Token: 0x060017AD RID: 6061 RVA: 0x0007B29C File Offset: 0x0007949C
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

		// Token: 0x04001AB0 RID: 6832
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x04001AB1 RID: 6833
		public WheelCollider[] driveWheels;

		// Token: 0x04001AB2 RID: 6834
		public WheelCollider[] steerWheels;

		// Token: 0x04001AB3 RID: 6835
		public float motorTorque;

		// Token: 0x04001AB4 RID: 6836
		public float maxSteerAngle;

		// Token: 0x04001AB5 RID: 6837
		public float wheelMass = 20f;

		// Token: 0x04001AB6 RID: 6838
		public float wheelRadius = 0.5f;

		// Token: 0x04001AB7 RID: 6839
		public float wheelWellDistance = 2.7f;

		// Token: 0x04001AB8 RID: 6840
		public float wheelSuspensionDistance = 0.3f;

		// Token: 0x04001AB9 RID: 6841
		public float wheelForceAppPointDistance;

		// Token: 0x04001ABA RID: 6842
		public float wheelSuspensionSpringSpring = 35000f;

		// Token: 0x04001ABB RID: 6843
		public float wheelSuspensionSpringDamper = 4500f;

		// Token: 0x04001ABC RID: 6844
		public float wheelSuspensionSpringTargetPosition = 0.5f;

		// Token: 0x04001ABD RID: 6845
		public float forwardFrictionExtremumSlip = 0.4f;

		// Token: 0x04001ABE RID: 6846
		public float forwardFrictionValue = 1f;

		// Token: 0x04001ABF RID: 6847
		public float forwardFrictionAsymptoticSlip = 0.8f;

		// Token: 0x04001AC0 RID: 6848
		public float forwardFrictionAsymptoticValue = 0.5f;

		// Token: 0x04001AC1 RID: 6849
		public float forwardFrictionStiffness = 1f;

		// Token: 0x04001AC2 RID: 6850
		public float sidewaysFrictionExtremumSlip = 0.2f;

		// Token: 0x04001AC3 RID: 6851
		public float sidewaysFrictionValue = 1f;

		// Token: 0x04001AC4 RID: 6852
		public float sidewaysFrictionAsymptoticSlip = 0.5f;

		// Token: 0x04001AC5 RID: 6853
		public float sidewaysFrictionAsymptoticValue = 0.75f;

		// Token: 0x04001AC6 RID: 6854
		public float sidewaysFrictionStiffness = 1f;

		// Token: 0x04001AC7 RID: 6855
		private InputBankTest inputBank;
	}
}
