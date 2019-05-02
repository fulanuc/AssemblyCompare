using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200043F RID: 1087
	public class Indicator
	{
		// Token: 0x06001830 RID: 6192 RVA: 0x00012199 File Offset: 0x00010399
		public Indicator(GameObject owner, GameObject visualizerPrefab)
		{
			this.owner = owner;
			this._visualizerPrefab = visualizerPrefab;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x000121C1 File Offset: 0x000103C1
		// (set) Token: 0x06001832 RID: 6194 RVA: 0x000121C9 File Offset: 0x000103C9
		public GameObject visualizerPrefab
		{
			get
			{
				return this._visualizerPrefab;
			}
			set
			{
				if (this._visualizerPrefab == value)
				{
					return;
				}
				if (this.visualizerInstance)
				{
					this.DestroyVisualizer();
				}
				this._visualizerPrefab = value;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x000121F4 File Offset: 0x000103F4
		// (set) Token: 0x06001834 RID: 6196 RVA: 0x000121FC File Offset: 0x000103FC
		private protected GameObject visualizerInstance { protected get; private set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06001835 RID: 6197 RVA: 0x00012205 File Offset: 0x00010405
		// (set) Token: 0x06001836 RID: 6198 RVA: 0x0001220D File Offset: 0x0001040D
		private protected Transform visualizerTransform { protected get; private set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06001837 RID: 6199 RVA: 0x00012216 File Offset: 0x00010416
		// (set) Token: 0x06001838 RID: 6200 RVA: 0x0001221E File Offset: 0x0001041E
		private protected Renderer[] visualizerRenderers { protected get; private set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001839 RID: 6201 RVA: 0x00012227 File Offset: 0x00010427
		public bool hasVisualizer
		{
			get
			{
				return this.visualizerInstance;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600183A RID: 6202 RVA: 0x00012234 File Offset: 0x00010434
		// (set) Token: 0x0600183B RID: 6203 RVA: 0x0001223C File Offset: 0x0001043C
		public bool active
		{
			get
			{
				return this._active;
			}
			set
			{
				if (this._active == value)
				{
					return;
				}
				this._active = value;
				if (this.active)
				{
					Indicator.IndicatorManager.AddIndicator(this);
					return;
				}
				Indicator.IndicatorManager.RemoveIndicator(this);
			}
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x00012264 File Offset: 0x00010464
		public void SetVisualizerInstantiated(bool newVisualizerInstantiated)
		{
			if (this.visualizerInstance != newVisualizerInstantiated)
			{
				if (newVisualizerInstantiated)
				{
					this.InstantiateVisualizer();
					return;
				}
				this.DestroyVisualizer();
			}
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x00012284 File Offset: 0x00010484
		private void InstantiateVisualizer()
		{
			this.visualizerInstance = UnityEngine.Object.Instantiate<GameObject>(this.visualizerPrefab);
			this.OnInstantiateVisualizer();
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x0001229D File Offset: 0x0001049D
		private void DestroyVisualizer()
		{
			this.OnDestroyVisualizer();
			UnityEngine.Object.Destroy(this.visualizerInstance);
			this.visualizerInstance = null;
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x000122B7 File Offset: 0x000104B7
		public void OnInstantiateVisualizer()
		{
			this.visualizerTransform = this.visualizerInstance.transform;
			this.visualizerRenderers = this.visualizerInstance.GetComponentsInChildren<Renderer>();
			this.SetVisibleInternal(this.visible);
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x000122E7 File Offset: 0x000104E7
		public virtual void OnDestroyVisualizer()
		{
			this.visualizerTransform = null;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x000025F6 File Offset: 0x000007F6
		public virtual void UpdateVisualizer()
		{
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x0007E0E4 File Offset: 0x0007C2E4
		public virtual void PositionForUI(Camera sceneCamera, Camera uiCamera)
		{
			if (this.targetTransform)
			{
				Vector3 position = this.targetTransform.position;
				Vector3 vector = sceneCamera.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = uiCamera.ScreenToWorldPoint(vector);
				this.visualizerTransform.position = position2;
			}
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000122FB File Offset: 0x000104FB
		public void SetVisible(bool newVisible)
		{
			newVisible &= this.targetTransform;
			if (this.visible != newVisible)
			{
				this.SetVisibleInternal(newVisible);
			}
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x0007E148 File Offset: 0x0007C348
		private void SetVisibleInternal(bool newVisible)
		{
			this.visible = newVisible;
			Renderer[] visualizerRenderers = this.visualizerRenderers;
			for (int i = 0; i < visualizerRenderers.Length; i++)
			{
				visualizerRenderers[i].enabled = newVisible;
			}
		}

		// Token: 0x04001B79 RID: 7033
		private GameObject _visualizerPrefab;

		// Token: 0x04001B7A RID: 7034
		public readonly GameObject owner;

		// Token: 0x04001B7B RID: 7035
		public Transform targetTransform;

		// Token: 0x04001B7F RID: 7039
		private bool _active;

		// Token: 0x04001B80 RID: 7040
		private bool visible = true;

		// Token: 0x02000440 RID: 1088
		private static class IndicatorManager
		{
			// Token: 0x06001845 RID: 6213 RVA: 0x0001231C File Offset: 0x0001051C
			public static void AddIndicator([NotNull] Indicator indicator)
			{
				Indicator.IndicatorManager.runningIndicators.Add(indicator);
				Indicator.IndicatorManager.RebuildVisualizer(indicator);
			}

			// Token: 0x06001846 RID: 6214 RVA: 0x0001232F File Offset: 0x0001052F
			public static void RemoveIndicator([NotNull] Indicator indicator)
			{
				indicator.SetVisualizerInstantiated(false);
				Indicator.IndicatorManager.runningIndicators.Remove(indicator);
			}

			// Token: 0x06001847 RID: 6215 RVA: 0x0007E17C File Offset: 0x0007C37C
			static IndicatorManager()
			{
				CameraRigController.onCameraTargetChanged += delegate(CameraRigController cameraRigController, GameObject target)
				{
					Indicator.IndicatorManager.RebuildVisualizerForAll();
				};
				UICamera.onUICameraPreRender += Indicator.IndicatorManager.OnPreRenderUI;
				UICamera.onUICameraPostRender += Indicator.IndicatorManager.OnPostRenderUI;
				RoR2Application.onUpdate += Indicator.IndicatorManager.Update;
			}

			// Token: 0x06001848 RID: 6216 RVA: 0x0007E1DC File Offset: 0x0007C3DC
			private static void RebuildVisualizerForAll()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					Indicator.IndicatorManager.RebuildVisualizer(indicator);
				}
			}

			// Token: 0x06001849 RID: 6217 RVA: 0x0007E22C File Offset: 0x0007C42C
			private static void Update()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					if (indicator.hasVisualizer)
					{
						indicator.UpdateVisualizer();
					}
				}
			}

			// Token: 0x0600184A RID: 6218 RVA: 0x0007E288 File Offset: 0x0007C488
			private static void RebuildVisualizer(Indicator indicator)
			{
				bool visualizerInstantiated = false;
				using (IEnumerator<CameraRigController> enumerator = CameraRigController.readOnlyInstancesList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.target == indicator.owner)
						{
							visualizerInstantiated = true;
							break;
						}
					}
				}
				indicator.SetVisualizerInstantiated(visualizerInstantiated);
			}

			// Token: 0x0600184B RID: 6219 RVA: 0x0007E2EC File Offset: 0x0007C4EC
			private static void OnPreRenderUI(UICamera uiCam)
			{
				GameObject target = uiCam.cameraRigController.target;
				Camera sceneCam = uiCam.cameraRigController.sceneCam;
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					bool flag = target == indicator.owner;
					indicator.SetVisible(target == indicator.owner);
					if (flag)
					{
						indicator.PositionForUI(sceneCam, uiCam.camera);
					}
				}
			}

			// Token: 0x0600184C RID: 6220 RVA: 0x0007E37C File Offset: 0x0007C57C
			private static void OnPostRenderUI(UICamera uiCamera)
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					indicator.SetVisible(true);
				}
			}

			// Token: 0x04001B81 RID: 7041
			private static readonly List<Indicator> runningIndicators = new List<Indicator>();
		}
	}
}
