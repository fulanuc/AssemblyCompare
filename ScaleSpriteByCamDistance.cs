using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003CB RID: 971
	public class ScaleSpriteByCamDistance : MonoBehaviour
	{
		// Token: 0x0600152B RID: 5419 RVA: 0x0001012F File Offset: 0x0000E32F
		static ScaleSpriteByCamDistance()
		{
			SceneCamera.onSceneCameraPreCull += ScaleSpriteByCamDistance.OnSceneCameraPreCull;
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x0001014C File Offset: 0x0000E34C
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0001015A File Offset: 0x0000E35A
		private void OnEnable()
		{
			ScaleSpriteByCamDistance.instancesList.Add(this);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00010167 File Offset: 0x0000E367
		private void OnDisable()
		{
			ScaleSpriteByCamDistance.instancesList.Remove(this);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x000720B8 File Offset: 0x000702B8
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

		// Token: 0x04001863 RID: 6243
		private static List<ScaleSpriteByCamDistance> instancesList = new List<ScaleSpriteByCamDistance>();

		// Token: 0x04001864 RID: 6244
		private new Transform transform;

		// Token: 0x04001865 RID: 6245
		[Tooltip("The amount by which to scale.")]
		public float scaleFactor = 1f;

		// Token: 0x04001866 RID: 6246
		public ScaleSpriteByCamDistance.ScalingMode scalingMode;

		// Token: 0x020003CC RID: 972
		public enum ScalingMode
		{
			// Token: 0x04001868 RID: 6248
			Direct,
			// Token: 0x04001869 RID: 6249
			Square,
			// Token: 0x0400186A RID: 6250
			Sqrt,
			// Token: 0x0400186B RID: 6251
			Cube,
			// Token: 0x0400186C RID: 6252
			CubeRoot
		}
	}
}
