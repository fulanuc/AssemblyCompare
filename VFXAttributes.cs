using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000415 RID: 1045
	public class VFXAttributes : MonoBehaviour
	{
		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x000116B0 File Offset: 0x0000F8B0
		public static ReadOnlyCollection<VFXAttributes> readonlyVFXList
		{
			get
			{
				return VFXAttributes._readonlyVFXList;
			}
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x0007A178 File Offset: 0x00078378
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

		// Token: 0x06001754 RID: 5972 RVA: 0x000116B7 File Offset: 0x0000F8B7
		public void OnEnable()
		{
			VFXAttributes.vfxList.Add(this);
			VFXBudget.totalCost += this.GetIntensityScore();
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x000116D5 File Offset: 0x0000F8D5
		public void OnDisable()
		{
			VFXAttributes.vfxList.Remove(this);
			VFXBudget.totalCost -= this.GetIntensityScore();
		}

		// Token: 0x04001A68 RID: 6760
		private static List<VFXAttributes> vfxList = new List<VFXAttributes>();

		// Token: 0x04001A69 RID: 6761
		private static ReadOnlyCollection<VFXAttributes> _readonlyVFXList = new ReadOnlyCollection<VFXAttributes>(VFXAttributes.vfxList);

		// Token: 0x04001A6A RID: 6762
		[Tooltip("Controls whether or not a VFX appears at all - consider if you would notice if this entire VFX never appeared. Also means it has a networking consequence.")]
		public VFXAttributes.VFXPriority vfxPriority;

		// Token: 0x04001A6B RID: 6763
		[Tooltip("Define how expensive a particle system is IF it appears.")]
		public VFXAttributes.VFXIntensity vfxIntensity;

		// Token: 0x04001A6C RID: 6764
		public Light[] optionalLights;

		// Token: 0x04001A6D RID: 6765
		[Tooltip("Particle systems that may be deactivated without impacting gameplay.")]
		public ParticleSystem[] secondaryParticleSystem;

		// Token: 0x02000416 RID: 1046
		public enum VFXPriority
		{
			// Token: 0x04001A6F RID: 6767
			Low,
			// Token: 0x04001A70 RID: 6768
			Medium,
			// Token: 0x04001A71 RID: 6769
			Always
		}

		// Token: 0x02000417 RID: 1047
		public enum VFXIntensity
		{
			// Token: 0x04001A73 RID: 6771
			Low,
			// Token: 0x04001A74 RID: 6772
			Medium,
			// Token: 0x04001A75 RID: 6773
			High
		}
	}
}
