using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F4 RID: 756
	public class EyeFlare : MonoBehaviour
	{
		// Token: 0x06000F53 RID: 3923 RVA: 0x0000BC90 File Offset: 0x00009E90
		static EyeFlare()
		{
			SceneCamera.onSceneCameraPreCull += EyeFlare.OnSceneCameraPreCull;
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0000BCAD File Offset: 0x00009EAD
		private void OnEnable()
		{
			EyeFlare.instancesList.Add(this);
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x0000BCBA File Offset: 0x00009EBA
		private void OnDisable()
		{
			EyeFlare.instancesList.Remove(this);
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x0005C6B8 File Offset: 0x0005A8B8
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

		// Token: 0x06000F57 RID: 3927 RVA: 0x0000BCC8 File Offset: 0x00009EC8
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x04001361 RID: 4961
		[Tooltip("The transform whose forward vector will be tested against the camera angle to determine scaling. This is usually the parent, and never this object since billboarding will affect the direction.")]
		public Transform directionSource;

		// Token: 0x04001362 RID: 4962
		public float localScale = 1f;

		// Token: 0x04001363 RID: 4963
		private static List<EyeFlare> instancesList = new List<EyeFlare>();

		// Token: 0x04001364 RID: 4964
		private new Transform transform;
	}
}
