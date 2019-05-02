using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F1 RID: 753
	public class EyeFlare : MonoBehaviour
	{
		// Token: 0x06000F43 RID: 3907 RVA: 0x0000BBE2 File Offset: 0x00009DE2
		static EyeFlare()
		{
			SceneCamera.onSceneCameraPreCull += EyeFlare.OnSceneCameraPreCull;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0000BBFF File Offset: 0x00009DFF
		private void OnEnable()
		{
			EyeFlare.instancesList.Add(this);
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0000BC0C File Offset: 0x00009E0C
		private void OnDisable()
		{
			EyeFlare.instancesList.Remove(this);
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0005C498 File Offset: 0x0005A698
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Transform transform = Camera.current.transform;
			Quaternion rotation = transform.rotation;
			Vector3 forward = transform.forward;
			for (int i = 0; i < EyeFlare.instancesList.Count; i++)
			{
				EyeFlare eyeFlare = EyeFlare.instancesList[i];
				float num = eyeFlare.localScale;
				if (eyeFlare.directionSource)
				{
					float num2 = Vector3.Dot(forward, eyeFlare.directionSource.forward) * -0.5f + 0.5f;
					num *= num2 * num2;
				}
				eyeFlare.transform.localScale = new Vector3(num, num, num);
				eyeFlare.transform.rotation = rotation;
			}
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0000BC1A File Offset: 0x00009E1A
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0400134A RID: 4938
		[Tooltip("The transform whose forward vector will be tested against the camera angle to determine scaling. This is usually the parent, and never this object since billboarding will affect the direction.")]
		public Transform directionSource;

		// Token: 0x0400134B RID: 4939
		public float localScale = 1f;

		// Token: 0x0400134C RID: 4940
		private static List<EyeFlare> instancesList = new List<EyeFlare>();

		// Token: 0x0400134D RID: 4941
		private new Transform transform;
	}
}
