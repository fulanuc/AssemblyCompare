using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003CD RID: 973
	[RequireComponent(typeof(Camera))]
	public class SceneCamera : MonoBehaviour
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06001531 RID: 5425 RVA: 0x0007218C File Offset: 0x0007038C
		// (remove) Token: 0x06001532 RID: 5426 RVA: 0x000721C0 File Offset: 0x000703C0
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreCull;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06001533 RID: 5427 RVA: 0x000721F4 File Offset: 0x000703F4
		// (remove) Token: 0x06001534 RID: 5428 RVA: 0x00072228 File Offset: 0x00070428
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPreRender;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06001535 RID: 5429 RVA: 0x0007225C File Offset: 0x0007045C
		// (remove) Token: 0x06001536 RID: 5430 RVA: 0x00072290 File Offset: 0x00070490
		public static event SceneCamera.SceneCameraDelegate onSceneCameraPostRender;

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x00010188 File Offset: 0x0000E388
		// (set) Token: 0x06001538 RID: 5432 RVA: 0x00010190 File Offset: 0x0000E390
		public Camera camera { get; private set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x00010199 File Offset: 0x0000E399
		// (set) Token: 0x0600153A RID: 5434 RVA: 0x000101A1 File Offset: 0x0000E3A1
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x0600153B RID: 5435 RVA: 0x000101AA File Offset: 0x0000E3AA
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x000101C4 File Offset: 0x0000E3C4
		private void OnPreCull()
		{
			if (SceneCamera.onSceneCameraPreCull != null)
			{
				SceneCamera.onSceneCameraPreCull(this);
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x000101D8 File Offset: 0x0000E3D8
		private void OnPreRender()
		{
			if (SceneCamera.onSceneCameraPreRender != null)
			{
				SceneCamera.onSceneCameraPreRender(this);
			}
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x000101EC File Offset: 0x0000E3EC
		private void OnPostRender()
		{
			if (SceneCamera.onSceneCameraPostRender != null)
			{
				SceneCamera.onSceneCameraPostRender(this);
			}
		}

		// Token: 0x020003CE RID: 974
		// (Invoke) Token: 0x06001541 RID: 5441
		public delegate void SceneCameraDelegate(SceneCamera sceneCamera);
	}
}
