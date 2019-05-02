using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000269 RID: 617
	public class BlueprintController : MonoBehaviour
	{
		// Token: 0x06000B9F RID: 2975 RVA: 0x0000939B File Offset: 0x0000759B
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0004C308 File Offset: 0x0004A508
		private void Update()
		{
			Material sharedMaterial = this.ok ? this.okMaterial : this.invalidMaterial;
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].sharedMaterial = sharedMaterial;
			}
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x000093A9 File Offset: 0x000075A9
		public void PushState(Vector3 position, Quaternion rotation, bool ok)
		{
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.ok = ok;
		}

		// Token: 0x04000F7E RID: 3966
		[NonSerialized]
		public bool ok;

		// Token: 0x04000F7F RID: 3967
		public Material okMaterial;

		// Token: 0x04000F80 RID: 3968
		public Material invalidMaterial;

		// Token: 0x04000F81 RID: 3969
		public Renderer[] renderers;

		// Token: 0x04000F82 RID: 3970
		private new Transform transform;
	}
}
