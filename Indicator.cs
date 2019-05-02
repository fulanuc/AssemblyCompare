using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000447 RID: 1095
	public class Indicator
	{
		// Token: 0x0600187D RID: 6269 RVA: 0x0001260D File Offset: 0x0001080D
		public Indicator(GameObject owner, GameObject visualizerPrefab)
		{
			this.owner = owner;
			this._visualizerPrefab = visualizerPrefab;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x0600187E RID: 6270 RVA: 0x00012635 File Offset: 0x00010835
		// (set) Token: 0x0600187F RID: 6271 RVA: 0x0001263D File Offset: 0x0001083D
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

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06001880 RID: 6272 RVA: 0x00012668 File Offset: 0x00010868
		// (set) Token: 0x06001881 RID: 6273 RVA: 0x00012670 File Offset: 0x00010870
		private protected GameObject visualizerInstance { protected get; private set; }

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06001882 RID: 6274 RVA: 0x00012679 File Offset: 0x00010879
		// (set) Token: 0x06001883 RID: 6275 RVA: 0x00012681 File Offset: 0x00010881
		private protected Transform visualizerTransform { protected get; private set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06001884 RID: 6276 RVA: 0x0001268A File Offset: 0x0001088A
		// (set) Token: 0x06001885 RID: 6277 RVA: 0x00012692 File Offset: 0x00010892
		private protected Renderer[] visualizerRenderers { protected get; private set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x0001269B File Offset: 0x0001089B
		public bool hasVisualizer
		{
			get
			{
				return this.visualizerInstance;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06001887 RID: 6279 RVA: 0x000126A8 File Offset: 0x000108A8
		// (set) Token: 0x06001888 RID: 6280 RVA: 0x000126B0 File Offset: 0x000108B0
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

		// Token: 0x06001889 RID: 6281 RVA: 0x000126D8 File Offset: 0x000108D8
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

		// Token: 0x0600188A RID: 6282 RVA: 0x000126F8 File Offset: 0x000108F8
		private void InstantiateVisualizer()
		{
			this.visualizerInstance = UnityEngine.Object.Instantiate<GameObject>(this.visualizerPrefab);
			this.OnInstantiateVisualizer();
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x00012711 File Offset: 0x00010911
		private void DestroyVisualizer()
		{
			this.OnDestroyVisualizer();
			UnityEngine.Object.Destroy(this.visualizerInstance);
			this.visualizerInstance = null;
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0001272B File Offset: 0x0001092B
		public void OnInstantiateVisualizer()
		{
			this.visualizerTransform = this.visualizerInstance.transform;
			this.visualizerRenderers = this.visualizerInstance.GetComponentsInChildren<Renderer>();
			this.SetVisibleInternal(this.visible);
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0001275B File Offset: 0x0001095B
		public virtual void OnDestroyVisualizer()
		{
			this.visualizerTransform = null;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x000025DA File Offset: 0x000007DA
		public virtual void UpdateVisualizer()
		{
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007E8A0 File Offset: 0x0007CAA0
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

		// Token: 0x06001890 RID: 6288 RVA: 0x0001276F File Offset: 0x0001096F
		public void SetVisible(bool newVisible)
		{
			newVisible &= this.targetTransform;
			if (this.visible != newVisible)
			{
				this.SetVisibleInternal(newVisible);
			}
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0007E904 File Offset: 0x0007CB04
		private void SetVisibleInternal(bool newVisible)
		{
			this.visible = newVisible;
			Renderer[] visualizerRenderers = this.visualizerRenderers;
			for (int i = 0; i < visualizerRenderers.Length; i++)
			{
				visualizerRenderers[i].enabled = newVisible;
			}
		}

		// Token: 0x04001BA9 RID: 7081
		private GameObject _visualizerPrefab;

		// Token: 0x04001BAA RID: 7082
		public readonly GameObject owner;

		// Token: 0x04001BAB RID: 7083
		public Transform targetTransform;

		// Token: 0x04001BAF RID: 7087
		private bool _active;

		// Token: 0x04001BB0 RID: 7088
		private bool visible = true;

		// Token: 0x02000448 RID: 1096
		private static class IndicatorManager
		{
			// Token: 0x06001892 RID: 6290 RVA: 0x00012790 File Offset: 0x00010990
			public static void AddIndicator([NotNull] Indicator indicator)
			{
				Indicator.IndicatorManager.runningIndicators.Add(indicator);
				Indicator.IndicatorManager.RebuildVisualizer(indicator);
			}

			// Token: 0x06001893 RID: 6291 RVA: 0x000127A3 File Offset: 0x000109A3
			public static void RemoveIndicator([NotNull] Indicator indicator)
			{
				indicator.SetVisualizerInstantiated(false);
				Indicator.IndicatorManager.runningIndicators.Remove(indicator);
			}

			// Token: 0x06001894 RID: 6292 RVA: 0x0007E938 File Offset: 0x0007CB38
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

			// Token: 0x06001895 RID: 6293 RVA: 0x0007E998 File Offset: 0x0007CB98
			private static void RebuildVisualizerForAll()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					Indicator.IndicatorManager.RebuildVisualizer(indicator);
				}
			}

			// Token: 0x06001896 RID: 6294 RVA: 0x0007E9E8 File Offset: 0x0007CBE8
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

			// Token: 0x06001897 RID: 6295 RVA: 0x0007EA44 File Offset: 0x0007CC44
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

			// Token: 0x06001898 RID: 6296 RVA: 0x0007EAA8 File Offset: 0x0007CCA8
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

			// Token: 0x06001899 RID: 6297 RVA: 0x0007EB38 File Offset: 0x0007CD38
			private static void OnPostRenderUI(UICamera uiCamera)
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					indicator.SetVisible(true);
				}
			}

			// Token: 0x04001BB1 RID: 7089
			private static readonly List<Indicator> runningIndicators = new List<Indicator>();
		}
	}
}
