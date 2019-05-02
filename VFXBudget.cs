using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200041E RID: 1054
	public static class VFXBudget
	{
		// Token: 0x0600179B RID: 6043 RVA: 0x0007A768 File Offset: 0x00078968
		public static bool CanAffordSpawn(GameObject prefab)
		{
			VFXAttributes component = prefab.GetComponent<VFXAttributes>();
			if (component)
			{
				int intensityScore = component.GetIntensityScore();
				int num = VFXBudget.totalCost + intensityScore + VFXBudget.particleCostBias.value;
				switch (component.vfxPriority)
				{
				case VFXAttributes.VFXPriority.Low:
					return Mathf.Pow((float)VFXBudget.lowPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
				case VFXAttributes.VFXPriority.Medium:
					return Mathf.Pow((float)VFXBudget.mediumPriorityCostThreshold.value / (float)num, VFXBudget.chanceFailurePower) > UnityEngine.Random.value;
				case VFXAttributes.VFXPriority.Always:
					return true;
				}
			}
			return true;
		}

		// Token: 0x04001A9F RID: 6815
		public static int totalCost = 0;

		// Token: 0x04001AA0 RID: 6816
		private static IntConVar lowPriorityCostThreshold = new IntConVar("vfxbudget_low_priority_cost_threshold", ConVarFlags.None, "50", "");

		// Token: 0x04001AA1 RID: 6817
		private static IntConVar mediumPriorityCostThreshold = new IntConVar("vfxbudget_medium_priority_cost_threshold", ConVarFlags.None, "200", "");

		// Token: 0x04001AA2 RID: 6818
		private static IntConVar particleCostBias = new IntConVar("vfxbudget_particle_cost_bias", ConVarFlags.None, "0", "");

		// Token: 0x04001AA3 RID: 6819
		private static float chanceFailurePower = 1f;
	}
}
