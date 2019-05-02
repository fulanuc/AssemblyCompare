using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C5 RID: 965
	public class ScaleSpriteByCamDistance : MonoBehaviour
	{
		// Token: 0x06001504 RID: 5380 RVA: 0x0000FEBF File Offset: 0x0000E0BF
		static ScaleSpriteByCamDistance()
		{
			SceneCamera.onSceneCameraPreCull += ScaleSpriteByCamDistance.OnSceneCameraPreCull;
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x0000FEDC File Offset: 0x0000E0DC
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x0000FEEA File Offset: 0x0000E0EA
		private void OnEnable()
		{
			ScaleSpriteByCamDistance.instancesList.Add(this);
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x0000FEF7 File Offset: 0x0000E0F7
		private void OnDisable()
		{
			ScaleSpriteByCamDistance.instancesList.Remove(this);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x00071D58 File Offset: 0x0006FF58
		private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
		{
			Vector3 position = sceneCamera.transform.position;
			for (int i = 0; i < ScaleSpriteByCamDistance.instancesList.Count; i++)
			{
				ScaleSpriteByCamDistance scaleSpriteByCamDistance = ScaleSpriteByCamDistance.instancesList[i];
				Transform transform = scaleSpriteByCamDistance.transform;
				float num = 1f;
				float num2 = Vector3.Distance(position, transform.position);
				switch (scaleSpriteByCamDistance.scalingMode)
				{
				case ScaleSpriteByCamDistance.ScalingMode.Direct:
					num = num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Square:
					num = num2 * num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Sqrt:
					num = Mathf.Sqrt(num2);
					break;
				case ScaleSpriteByCamDistance.ScalingMode.Cube:
					num = num2 * num2 * num2;
					break;
				case ScaleSpriteByCamDistance.ScalingMode.CubeRoot:
					num = Mathf.Pow(num2, 0.333333343f);
					break;
				}
				num *= scaleSpriteByCamDistance.scaleFactor;
				transform.localScale = new Vector3(num, num, num);
			}
		}

		// Token: 0x04001844 RID: 6212
		private static List<ScaleSpriteByCamDistance> instancesList = new List<ScaleSpriteByCamDistance>();

		// Token: 0x04001845 RID: 6213
		private new Transform transform;

		// Token: 0x04001846 RID: 6214
		[Tooltip("The amount by which to scale.")]
		public float scaleFactor = 1f;

		// Token: 0x04001847 RID: 6215
		public ScaleSpriteByCamDistance.ScalingMode scalingMode;

		// Token: 0x020003C6 RID: 966
		public enum ScalingMode
		{
			// Token: 0x04001849 RID: 6217
			Direct,
			// Token: 0x0400184A RID: 6218
			Square,
			// Token: 0x0400184B RID: 6219
			Sqrt,
			// Token: 0x0400184C RID: 6220
			Cube,
			// Token: 0x0400184D RID: 6221
			CubeRoot
		}
	}
}
