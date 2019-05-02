using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200041B RID: 1051
	public class VFXAttributes : MonoBehaviour
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06001795 RID: 6037 RVA: 0x00011ADC File Offset: 0x0000FCDC
		public static ReadOnlyCollection<VFXAttributes> readonlyVFXList
		{
			get
			{
				return VFXAttributes._readonlyVFXList;
			}
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0007A738 File Offset: 0x00078938
		public int GetIntensityScore()
		{
			switch (this.vfxIntensity)
			{
			case VFXAttributes.VFXIntensity.Low:
				return 1;
			case VFXAttributes.VFXIntensity.Medium:
				return 5;
			case VFXAttributes.VFXIntensity.High:
				return 25;
			default:
				return 0;
			}
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x00011AE3 File Offset: 0x0000FCE3
		public void OnEnable()
		{
			VFXAttributes.vfxList.Add(this);
			VFXBudget.totalCost += this.GetIntensityScore();
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x00011B01 File Offset: 0x0000FD01
		public void OnDisable()
		{
			VFXAttributes.vfxList.Remove(this);
			VFXBudget.totalCost -= this.GetIntensityScore();
		}

		// Token: 0x04001A91 RID: 6801
		private static List<VFXAttributes> vfxList = new List<VFXAttributes>();

		// Token: 0x04001A92 RID: 6802
		private static ReadOnlyCollection<VFXAttributes> _readonlyVFXList = new ReadOnlyCollection<VFXAttributes>(VFXAttributes.vfxList);

		// Token: 0x04001A93 RID: 6803
		[Tooltip("Controls whether or not a VFX appears at all - consider if you would notice if this entire VFX never appeared. Also means it has a networking consequence.")]
		public VFXAttributes.VFXPriority vfxPriority;

		// Token: 0x04001A94 RID: 6804
		[Tooltip("Define how expensive a particle system is IF it appears.")]
		public VFXAttributes.VFXIntensity vfxIntensity;

		// Token: 0x04001A95 RID: 6805
		public Light[] optionalLights;

		// Token: 0x04001A96 RID: 6806
		[Tooltip("Particle systems that may be deactivated without impacting gameplay.")]
		public ParticleSystem[] secondaryParticleSystem;

		// Token: 0x0200041C RID: 1052
		public enum VFXPriority
		{
			// Token: 0x04001A98 RID: 6808
			Low,
			// Token: 0x04001A99 RID: 6809
			Medium,
			// Token: 0x04001A9A RID: 6810
			Always
		}

		// Token: 0x0200041D RID: 1053
		public enum VFXIntensity
		{
			// Token: 0x04001A9C RID: 6812
			Low,
			// Token: 0x04001A9D RID: 6813
			Medium,
			// Token: 0x04001A9E RID: 6814
			High
		}
	}
}
