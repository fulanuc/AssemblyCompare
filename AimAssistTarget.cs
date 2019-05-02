using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000254 RID: 596
	public class AimAssistTarget : MonoBehaviour
	{
		// Token: 0x06000B1D RID: 2845 RVA: 0x00008F18 File Offset: 0x00007118
		private void OnEnable()
		{
			AimAssistTarget.instancesList.Add(this);
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x00008F25 File Offset: 0x00007125
		private void OnDisable()
		{
			AimAssistTarget.instancesList.Remove(this);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00008F33 File Offset: 0x00007133
		private void FixedUpdate()
		{
			if (this.healthComponent && !this.healthComponent.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0004AE34 File Offset: 0x00049034
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

		// Token: 0x04000F1C RID: 3868
		public Transform point0;

		// Token: 0x04000F1D RID: 3869
		public Transform point1;

		// Token: 0x04000F1E RID: 3870
		public float assistScale = 1f;

		// Token: 0x04000F1F RID: 3871
		public HealthComponent healthComponent;

		// Token: 0x04000F20 RID: 3872
		public TeamComponent teamComponent;

		// Token: 0x04000F21 RID: 3873
		public static List<AimAssistTarget> instancesList = new List<AimAssistTarget>();

		// Token: 0x04000F22 RID: 3874
		public static FloatConVar debugAimAssistVisualCoefficient = new FloatConVar("debug_aim_assist_visual_coefficient", ConVarFlags.None, "2", "Magic for debug visuals. Don't touch.");
	}
}
