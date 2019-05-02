using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C7 RID: 967
	[RequireComponent(typeof(Camera))]
	public class SceneCamera : MonoBehaviour
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x0600150A RID: 5386 RVA: 0x00071E2C File Offset: 0x0007002C
		// (remove) Token: 0x0600150B RID: 5387 RVA: 0x00071E60 File Offset: 0x00070060
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreCull;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x0600150C RID: 5388 RVA: 0x00071E94 File Offset: 0x00070094
		// (remove) Token: 0x0600150D RID: 5389 RVA: 0x00071EC8 File Offset: 0x000700C8
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreRender;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x0600150E RID: 5390 RVA: 0x00071EFC File Offset: 0x000700FC
		// (remove) Token: 0x0600150F RID: 5391 RVA: 0x00071F30 File Offset: 0x00070130
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPostRender;

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06001510 RID: 5392 RVA: 0x0000FF18 File Offset: 0x0000E118
		// (set) Token: 0x06001511 RID: 5393 RVA: 0x0000FF20 File Offset: 0x0000E120
		public Camera camera { get; private set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06001512 RID: 5394 RVA: 0x0000FF29 File Offset: 0x0000E129
		// (set) Token: 0x06001513 RID: 5395 RVA: 0x0000FF31 File Offset: 0x0000E131
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001514 RID: 5396 RVA: 0x0000FF3A File Offset: 0x0000E13A
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x0000FF54 File Offset: 0x0000E154
		private void OnPreCull()
		{
			if (SceneCamera.onSceneCameraPreCull != null)
			{
				SceneCamera.onSceneCameraPreCull(this);
			}
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x0000FF68 File Offset: 0x0000E168
		private void OnPreRender()
		{
			if (SceneCamera.onSceneCameraPreRender != null)
			{
				SceneCamera.onSceneCameraPreRender(this);
			}
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x0000FF7C File Offset: 0x0000E17C
		private void OnPostRender()
		{
			if (SceneCamera.onSceneCameraPostRender != null)
			{
				SceneCamera.onSceneCameraPostRender(this);
			}
		}

		// Token: 0x020003C8 RID: 968
		// (Invoke) Token: 0x0600151A RID: 5402
		public delegate void SceneCameraDelegate(SceneCamera sceneCamera);
	}
}
