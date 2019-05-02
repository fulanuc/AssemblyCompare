using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000268 RID: 616
	public class Billboard : MonoBehaviour
	{
		// Token: 0x06000B91 RID: 2961 RVA: 0x00009301 File Offset: 0x00007501
		static Billboard()
		{
			SceneCamera.onSceneCameraPreCull += Billboard.OnSceneCameraPreCull;
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0004C0BC File Offset: 0x0004A2BC
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Quaternion rotation = sceneCamera.transform.rotation;
			for (int i = 0; i < Billboard.instanceTransformsList.Count; i++)
			{
				Billboard.instanceTransformsList[i].rotation = rotation;
			}
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0000931E File Offset: 0x0000751E
		private void OnEnable()
		{
			Billboard.instanceTransformsList.Add(base.transform);
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x00009330 File Offset: 0x00007530
		private void OnDisable()
		{
			Billboard.instanceTransformsList.Remove(base.transform);
		}

		// Token: 0x04000F77 RID: 3959
		private static List<Transform> instanceTransformsList = new List<Transform>();
	}
}
