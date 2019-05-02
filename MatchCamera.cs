using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000357 RID: 855
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class MatchCamera : MonoBehaviour
	{
		// Token: 0x060011A6 RID: 4518 RVA: 0x0000D6EE File Offset: 0x0000B8EE
		private void Awake()
		{
			this.destCamera = base.GetComponent<Camera>();
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0000D6FC File Offset: 0x0000B8FC
		private void LateUpdate()
		{
			if (this.srcCamera)
			{
				this.destCamera.rect = this.srcCamera.rect;
				this.destCamera.fieldOfView = this.srcCamera.fieldOfView;
			}
		}

		// Token: 0x0400159F RID: 5535
		private Camera destCamera;

		// Token: 0x040015A0 RID: 5536
		public Camera srcCamera;
	}
}
