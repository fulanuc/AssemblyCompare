using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200035D RID: 861
	[RequireComponent(typeof(Camera))]
	public class ModelCamera : MonoBehaviour
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x0000D7FC File Offset: 0x0000B9FC
		// (set) Token: 0x060011BB RID: 4539 RVA: 0x0000D803 File Offset: 0x0000BA03
		public static ModelCamera instance { get; private set; }

		// Token: 0x060011BC RID: 4540 RVA: 0x0000D80B File Offset: 0x0000BA0B
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

		// Token: 0x060011BD RID: 4541 RVA: 0x0000D84B File Offset: 0x0000BA4B
		private void OnDisable()
		{
			if (ModelCamera.instance == this)
			{
				ModelCamera.instance = null;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x0000D860 File Offset: 0x0000BA60
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x0000D868 File Offset: 0x0000BA68
		public Camera attachedCamera { get; private set; }

		// Token: 0x060011C0 RID: 4544 RVA: 0x000670D0 File Offset: 0x000652D0
		private void Awake()
		{
			this.attachedCamera = base.GetComponent<Camera>();
			this.attachedCamera.enabled = false;
			this.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			UnityEngine.Object.Destroy(base.GetComponent<AkAudioListener>());
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x00067120 File Offset: 0x00065320
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

		// Token: 0x060011C2 RID: 4546 RVA: 0x00067188 File Offset: 0x00065388
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

		// Token: 0x060011C3 RID: 4547 RVA: 0x0000D871 File Offset: 0x0000BA71
		public void AddLight(Light light)
		{
			this.lights.Add(light);
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x0000D87F File Offset: 0x0000BA7F
		public void RemoveLight(Light light)
		{
			this.lights.Remove(light);
		}

		// Token: 0x040015C5 RID: 5573
		[NonSerialized]
		public RenderSettingsState renderSettings;

		// Token: 0x040015C7 RID: 5575
		public Color ambientLight;

		// Token: 0x040015C9 RID: 5577
		private readonly List<Light> lights = new List<Light>();

		// Token: 0x0200035E RID: 862
		private struct ObjectRestoreInfo
		{
			// Token: 0x040015CA RID: 5578
			public GameObject obj;

			// Token: 0x040015CB RID: 5579
			public int layer;
		}
	}
}
