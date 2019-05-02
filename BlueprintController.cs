using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000269 RID: 617
	public class BlueprintController : MonoBehaviour
	{
		// Token: 0x06000B96 RID: 2966 RVA: 0x00009343 File Offset: 0x00007543
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x0004C0FC File Offset: 0x0004A2FC
		private void Update()
		{
			Material sharedMaterial = this.ok ? this.okMaterial : this.invalidMaterial;
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].sharedMaterial = sharedMaterial;
			}
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x00009351 File Offset: 0x00007551
		public void PushState(Vector3 position, Quaternion rotation, bool ok)
		{
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.ok = ok;
		}

		// Token: 0x04000F78 RID: 3960
		[NonSerialized]
		public bool ok;

		// Token: 0x04000F79 RID: 3961
		public Material okMaterial;

		// Token: 0x04000F7A RID: 3962
		public Material invalidMaterial;

		// Token: 0x04000F7B RID: 3963
		public Renderer[] renderers;

		// Token: 0x04000F7C RID: 3964
		private new Transform transform;
	}
}
