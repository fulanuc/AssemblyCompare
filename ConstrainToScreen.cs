using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002B9 RID: 697
	public class ConstrainToScreen : MonoBehaviour
	{
		// Token: 0x06000E33 RID: 3635 RVA: 0x0000AF53 File Offset: 0x00009153
		static ConstrainToScreen()
		{
			SceneCamera.onSceneCameraPreCull += ConstrainToScreen.OnSceneCameraPreCull;
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x00057AD0 File Offset: 0x00055CD0
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Camera camera = sceneCamera.camera;
			for (int i = 0; i < ConstrainToScreen.instanceTransformsList.Count; i++)
			{
				Transform transform = ConstrainToScreen.instanceTransformsList[i];
				Vector3 vector = camera.WorldToViewportPoint(transform.position);
				vector.x = Mathf.Clamp(vector.x, ConstrainToScreen.boundaryUVSize, 1f - ConstrainToScreen.boundaryUVSize);
				vector.y = Mathf.Clamp(vector.y, ConstrainToScreen.boundaryUVSize, 1f - ConstrainToScreen.boundaryUVSize);
				transform.position = camera.ViewportToWorldPoint(vector);
			}
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0000AF7A File Offset: 0x0000917A
		private void OnEnable()
		{
			ConstrainToScreen.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0000AF8C File Offset: 0x0000918C
		private void OnDisable()
		{
			ConstrainToScreen.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x04001209 RID: 4617
		private static float boundaryUVSize = 0.05f;

		// Token: 0x0400120A RID: 4618
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
