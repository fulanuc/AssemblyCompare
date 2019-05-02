using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000360 RID: 864
	[RequireComponent(typeof(Camera))]
	public class ModelCamera : MonoBehaviour
	{
		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060011D1 RID: 4561 RVA: 0x0000D8E5 File Offset: 0x0000BAE5
		// (set) Token: 0x060011D2 RID: 4562 RVA: 0x0000D8EC File Offset: 0x0000BAEC
		public static ModelCamera instance { get; private set; }

		// Token: 0x060011D3 RID: 4563 RVA: 0x0000D8F4 File Offset: 0x0000BAF4
		private void OnEnable()
		{
			if (ModelCamera.instance && ModelCamera.instance != this)
			{
				Debug.LogErrorFormat("Only one {0} instance can be active at a time.", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			ModelCamera.instance = this;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x0000D934 File Offset: 0x0000BB34
		private void OnDisable()
		{
			if (ModelCamera.instance == this)
			{
				ModelCamera.instance = null;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060011D5 RID: 4565 RVA: 0x0000D949 File Offset: 0x0000BB49
		// (set) Token: 0x060011D6 RID: 4566 RVA: 0x0000D951 File Offset: 0x0000BB51
		public Camera attachedCamera { get; private set; }

		// Token: 0x060011D7 RID: 4567 RVA: 0x00067408 File Offset: 0x00065608
		private void Awake()
		{
			this.attachedCamera = base.GetComponent<Camera>();
			this.attachedCamera.enabled = false;
			this.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			UnityEngine.Object.Destroy(base.GetComponent<AkAudioListener>());
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00067458 File Offset: 0x00065658
		private static void PrepareObjectForRendering(Transform objTransform, List<ModelCamera.ObjectRestoreInfo> objectRestorationList)
		{
			GameObject gameObject = objTransform.gameObject;
			objectRestorationList.Add(new ModelCamera.ObjectRestoreInfo
			{
				obj = gameObject,
				layer = gameObject.layer
			});
			gameObject.layer = LayerIndex.manualRender.intVal;
			int childCount = objTransform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				ModelCamera.PrepareObjectForRendering(objTransform.GetChild(i), objectRestorationList);
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x000674C0 File Offset: 0x000656C0
		public void RenderItem(GameObject obj, RenderTexture targetTexture)
		{
			for (int i = 0; i < this.lights.Count; i++)
			{
				this.lights[i].cullingMask = LayerIndex.manualRender.mask;
			}
			RenderSettingsState renderSettingsState = RenderSettingsState.FromCurrent();
			this.renderSettings.Apply();
			List<ModelCamera.ObjectRestoreInfo> list = new List<ModelCamera.ObjectRestoreInfo>();
			if (obj)
			{
				ModelCamera.PrepareObjectForRendering(obj.transform, list);
			}
			this.attachedCamera.targetTexture = targetTexture;
			this.attachedCamera.Render();
			for (int j = 0; j < list.Count; j++)
			{
				list[j].obj.layer = list[j].layer;
			}
			for (int k = 0; k < this.lights.Count; k++)
			{
				this.lights[k].cullingMask = 0;
			}
			renderSettingsState.Apply();
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x0000D95A File Offset: 0x0000BB5A
		public void AddLight(Light light)
		{
			this.lights.Add(light);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0000D968 File Offset: 0x0000BB68
		public void RemoveLight(Light light)
		{
			this.lights.Remove(light);
		}

		// Token: 0x040015DE RID: 5598
		[NonSerialized]
		public RenderSettingsState renderSettings;

		// Token: 0x040015E0 RID: 5600
		public Color ambientLight;

		// Token: 0x040015E2 RID: 5602
		private readonly List<Light> lights = new List<Light>();

		// Token: 0x02000361 RID: 865
		private struct ObjectRestoreInfo
		{
			// Token: 0x040015E3 RID: 5603
			public GameObject obj;

			// Token: 0x040015E4 RID: 5604
			public int layer;
		}
	}
}
