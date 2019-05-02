using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000349 RID: 841
	public class JitterBones : MonoBehaviour
	{
		// Token: 0x0600118C RID: 4492 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00066430 File Offset: 0x00064630
		private void LateUpdate()
		{
			if (this.skinnedMeshRenderer)
			{
				this.age += Time.deltaTime;
				for (int i = 0; i < this.skinnedMeshRenderer.bones.Length; i++)
				{
					Transform transform = this.skinnedMeshRenderer.bones[i];
					float num = this.age * this.perlinNoiseFrequency;
					float y = (float)i;
					float num2 = Mathf.PerlinNoise(num, y);
					float num3 = Mathf.PerlinNoise(num + 10f, y);
					float num4 = Mathf.PerlinNoise(num + 20f, y);
					num2 = Util.Remap(num2, 0f, 1f, -this.perlinNoiseStrength, this.perlinNoiseStrength);
					num3 = Util.Remap(num3, 0f, 1f, -this.perlinNoiseStrength, this.perlinNoiseStrength);
					num4 = Util.Remap(num4, 0f, 1f, -this.perlinNoiseStrength, this.perlinNoiseStrength);
					transform.rotation *= Quaternion.Euler(num2, num3, num4);
				}
			}
		}

		// Token: 0x04001583 RID: 5507
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04001584 RID: 5508
		public float perlinNoiseFrequency;

		// Token: 0x04001585 RID: 5509
		public float perlinNoiseStrength;

		// Token: 0x04001586 RID: 5510
		private float age;
	}
}
