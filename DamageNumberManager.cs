using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002C2 RID: 706
	[ExecuteAlways]
	public class DamageNumberManager : MonoBehaviour
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x0000B18B File Offset: 0x0000938B
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x0000B192 File Offset: 0x00009392
		public static DamageNumberManager instance { get; private set; }

		// Token: 0x06000E5E RID: 3678 RVA: 0x0000B19A File Offset: 0x0000939A
		private void OnEnable()
		{
			DamageNumberManager.instance = SingletonHelper.Assign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x0000B1AC File Offset: 0x000093AC
		private void OnDisable()
		{
			DamageNumberManager.instance = SingletonHelper.Unassign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x0000B1BE File Offset: 0x000093BE
		private void Start()
		{
			this.ps = base.GetComponent<ParticleSystem>();
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x00058598 File Offset: 0x00056798
		public void SpawnDamageNumber(float amount, Vector3 position, bool crit, TeamIndex teamIndex, DamageColorIndex damageColorIndex)
		{
			Color a = DamageColor.FindColor(damageColorIndex);
			Color white = Color.white;
			if (teamIndex == TeamIndex.Monster)
			{
				white = new Color(0.5568628f, 0.294117659f, 0.6039216f);
			}
			this.ps.Emit(new ParticleSystem.EmitParams
			{
				position = position,
				startColor = a * white,
				applyShapeToPosition = true
			}, 1);
			this.ps.GetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
			this.customData[this.customData.Count - 1] = new Vector4(1f, 0f, amount, crit ? 1f : 0f);
			this.ps.SetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
		}

		// Token: 0x0400123B RID: 4667
		public float damageValueMin;

		// Token: 0x0400123C RID: 4668
		public float damageValueMax;

		// Token: 0x0400123D RID: 4669
		private List<Vector4> customData = new List<Vector4>();

		// Token: 0x0400123E RID: 4670
		private ParticleSystem ps;
	}
}
