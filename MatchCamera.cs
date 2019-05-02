using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035A RID: 858
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class MatchCamera : MonoBehaviour
	{
		// Token: 0x060011BD RID: 4541 RVA: 0x0000D7D7 File Offset: 0x0000B9D7
		private void Awake()
		{
			this.destCamera = base.GetComponent<Camera>();
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0000D7E5 File Offset: 0x0000B9E5
		private void LateUpdate()
		{
			if (this.srcCamera)
			{
				this.destCamera.rect = this.srcCamera.rect;
				this.destCamera.fieldOfView = this.srcCamera.fieldOfView;
			}
		}

		// Token: 0x040015B8 RID: 5560
		private Camera destCamera;

		// Token: 0x040015B9 RID: 5561
		public Camera srcCamera;
	}
}
