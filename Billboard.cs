using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000268 RID: 616
	public class Billboard : MonoBehaviour
	{
		// Token: 0x06000B9A RID: 2970 RVA: 0x00009359 File Offset: 0x00007559
		static Billboard()
		{
			SceneCamera.onSceneCameraPreCull += Billboard.OnSceneCameraPreCull;
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0004C2C8 File Offset: 0x0004A4C8
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Quaternion rotation = sceneCamera.transform.rotation;
			for (int i = 0; i < Billboard.instanceTransformsList.Count; i++)
			{
				Billboard.instanceTransformsList[i].rotation = rotation;
			}
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00009376 File Offset: 0x00007576
		private void OnEnable()
		{
			Billboard.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x00009388 File Offset: 0x00007588
		private void OnDisable()
		{
			Billboard.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x04000F7D RID: 3965
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
