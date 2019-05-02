using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000254 RID: 596
	public class AimAssistTarget : MonoBehaviour
	{
		// Token: 0x06000B20 RID: 2848 RVA: 0x00008F3D File Offset: 0x0000713D
		private void OnEnable()
		{
			AimAssistTarget.instancesList.Add(this);
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00008F4A File Offset: 0x0000714A
		private void OnDisable()
		{
			AimAssistTarget.instancesList.Remove(this);
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x00008F58 File Offset: 0x00007158
		private void FixedUpdate()
		{
			if (this.healthComponent && !this.healthComponent.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0004B040 File Offset: 0x00049240
		private void OnDrawGizmos()
		{
			if (this.point0)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this.point0.position, 1f * this.assistScale * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(this.point0.position, 1f * this.assistScale * CameraRigController.aimStickAssistMaxSize.value * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
			}
			if (this.point1)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this.point1.position, 1f * this.assistScale * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(this.point1.position, 1f * this.assistScale * CameraRigController.aimStickAssistMaxSize.value * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
			}
			if (this.point0 && this.point1)
			{
				Gizmos.DrawLine(this.point0.position, this.point1.position);
			}
		}

		// Token: 0x04000F22 RID: 3874
		public Transform point0;

		// Token: 0x04000F23 RID: 3875
		public Transform point1;

		// Token: 0x04000F24 RID: 3876
		public float assistScale = 1f;

		// Token: 0x04000F25 RID: 3877
		public HealthComponent healthComponent;

		// Token: 0x04000F26 RID: 3878
		public TeamComponent teamComponent;

		// Token: 0x04000F27 RID: 3879
		public static List<AimAssistTarget> instancesList = new List<AimAssistTarget>();

		// Token: 0x04000F28 RID: 3880
		public static FloatConVar debugAimAssistVisualCoefficient = new FloatConVar("debug_aim_assist_visual_coefficient", ConVarFlags.None, "2", "Magic for debug visuals. Don't touch.");
	}
}
