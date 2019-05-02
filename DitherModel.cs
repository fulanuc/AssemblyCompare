using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D9 RID: 729
	public class DitherModel : MonoBehaviour
	{
		// Token: 0x06000E9B RID: 3739 RVA: 0x0000B430 File Offset: 0x00009630
		static DitherModel()
		{
			SceneCamera.onSceneCameraPreRender += DitherModel.OnSceneCameraPreRender;
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x0000B44D File Offset: 0x0000964D
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				DitherModel.RefreshObstructorsForCamera(sceneCamera.cameraRigController);
			}
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x000595C4 File Offset: 0x000577C4
		private static void RefreshObstructorsForCamera(CameraRigController cameraRigController)
		{
			Vector3 position = cameraRigController.transform.position;
			for (int i = 0; i < DitherModel.instancesList.Count; i++)
			{
				DitherModel ditherModel = DitherModel.instancesList[i];
				if (ditherModel.bounds)
				{
					Vector3 a = ditherModel.bounds.ClosestPointOnBounds(position);
					ditherModel.fade = Mathf.Clamp01(Util.Remap(Vector3.Distance(a, position), cameraRigController.fadeStartDistance, cameraRigController.fadeEndDistance, 0f, 1f));
					ditherModel.UpdateDither();
				}
				else
				{
					Debug.LogFormat("{0} has missing collider for dither model", new object[]
					{
						ditherModel.gameObject
					});
				}
			}
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0005966C File Offset: 0x0005786C
		private void UpdateDither()
		{
			for (int i = this.renderers.Length - 1; i >= 0; i--)
			{
				Renderer renderer = this.renderers[i];
				renderer.GetPropertyBlock(this.propertyStorage);
				this.propertyStorage.SetFloat("_Fade", this.fade);
				renderer.SetPropertyBlock(this.propertyStorage);
			}
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0000B467 File Offset: 0x00009667
		private void Awake()
		{
			this.propertyStorage = new MaterialPropertyBlock();
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x0000B474 File Offset: 0x00009674
		private void OnEnable()
		{
			DitherModel.instancesList.Add(this);
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0000B481 File Offset: 0x00009681
		private void OnDisable()
		{
			DitherModel.instancesList.Remove(this);
		}

		// Token: 0x040012A0 RID: 4768
		[HideInInspector]
		public float fade;

		// Token: 0x040012A1 RID: 4769
		public Collider bounds;

		// Token: 0x040012A2 RID: 4770
		public Renderer[] renderers;

		// Token: 0x040012A3 RID: 4771
		private MaterialPropertyBlock propertyStorage;

		// Token: 0x040012A4 RID: 4772
		private static List<DitherModel> instancesList = new List<DitherModel>();
	}
}
