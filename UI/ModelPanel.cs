using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200060C RID: 1548
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RawImage))]
	public class ModelPanel : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IScrollHandler, IEndDragHandler
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060022E0 RID: 8928 RVA: 0x000196E1 File Offset: 0x000178E1
		// (set) Token: 0x060022E1 RID: 8929 RVA: 0x000196E9 File Offset: 0x000178E9
		public GameObject modelPrefab
		{
			get
			{
				return this._modelPrefab;
			}
			set
			{
				if (this._modelPrefab == value)
				{
					return;
				}
				this.DestroyModelInstance();
				this._modelPrefab = value;
				this.BuildModelInstance();
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x0001970D File Offset: 0x0001790D
		private void DestroyModelInstance()
		{
			UnityEngine.Object.Destroy(this.modelInstance);
			this.modelInstance = null;
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x000A721C File Offset: 0x000A541C
		private void BuildModelInstance()
		{
			if (this._modelPrefab && base.enabled && !this.modelInstance)
			{
				this.modelInstance = UnityEngine.Object.Instantiate<GameObject>(this._modelPrefab, Vector3.zero, Quaternion.identity);
				Bounds bounds;
				Util.GuessRenderBoundsMeshOnly(this.modelInstance, out bounds);
				this.pivotPoint = bounds.center;
				this.minDistance = Mathf.Min(new float[]
				{
					bounds.size.x,
					bounds.size.y,
					bounds.size.z
				}) * 1f;
				this.maxDistance = Mathf.Max(new float[]
				{
					bounds.size.x,
					bounds.size.y,
					bounds.size.z
				}) * 2f;
				Renderer[] componentsInChildren = this.modelInstance.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.layer = LayerIndex.noDraw.intVal;
				}
				AimAnimator[] componentsInChildren2 = this.modelInstance.GetComponentsInChildren<AimAnimator>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].inputBank = null;
					componentsInChildren2[j].directionComponent = null;
					componentsInChildren2[j].enabled = false;
				}
				foreach (Animator animator in this.modelInstance.GetComponentsInChildren<Animator>())
				{
					animator.SetBool("isGrounded", true);
					animator.SetFloat("aimPitchCycle", 0.5f);
					animator.SetFloat("aimYawCycle", 0.5f);
					animator.Play("Idle");
					animator.Update(0f);
				}
				IKSimpleChain[] componentsInChildren4 = this.modelInstance.GetComponentsInChildren<IKSimpleChain>();
				for (int l = 0; l < componentsInChildren4.Length; l++)
				{
					componentsInChildren4[l].enabled = false;
				}
				DitherModel[] componentsInChildren5 = this.modelInstance.GetComponentsInChildren<DitherModel>();
				for (int m = 0; m < componentsInChildren5.Length; m++)
				{
					componentsInChildren5[m].enabled = false;
				}
				PrintController[] componentsInChildren6 = this.modelInstance.GetComponentsInChildren<PrintController>();
				for (int m = 0; m < componentsInChildren6.Length; m++)
				{
					componentsInChildren6[m].enabled = false;
				}
				foreach (LightIntensityCurve lightIntensityCurve in this.modelInstance.GetComponentsInChildren<LightIntensityCurve>())
				{
					if (!lightIntensityCurve.loop)
					{
						lightIntensityCurve.enabled = false;
					}
				}
				AkEvent[] componentsInChildren8 = this.modelInstance.GetComponentsInChildren<AkEvent>();
				for (int m = 0; m < componentsInChildren8.Length; m++)
				{
					componentsInChildren8[m].enabled = false;
				}
				this.desiredZoom = 0.5f;
				this.zoom = this.desiredZoom;
				this.zoomVelocity = 0f;
				this.ResetOrbitAndPan();
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060022E4 RID: 8932 RVA: 0x00019721 File Offset: 0x00017921
		// (set) Token: 0x060022E5 RID: 8933 RVA: 0x00019729 File Offset: 0x00017929
		public RenderTexture renderTexture { get; private set; }

		// Token: 0x060022E6 RID: 8934 RVA: 0x000A74F4 File Offset: 0x000A56F4
		private void ResetOrbitAndPan()
		{
			this.orbitPitch = 0f;
			this.orbitYaw = 0f;
			this.orbitalVelocity = Vector3.zero;
			this.orbitalVelocitySmoothDampVelocity = Vector3.zero;
			this.pan = Vector2.zero;
			this.panVelocity = Vector2.zero;
			this.panVelocitySmoothDampVelocity = Vector2.zero;
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000A7550 File Offset: 0x000A5750
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.rawImage = base.GetComponent<RawImage>();
			this.cameraRigController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Main Camera")).GetComponent<CameraRigController>();
			this.cameraRigController.gameObject.name = "ModelCamera";
			this.cameraRigController.uiCam.gameObject.SetActive(false);
			this.cameraRigController.createHud = false;
			this.cameraRigController.fadeStartDistance = float.PositiveInfinity;
			this.cameraRigController.fadeEndDistance = float.PositiveInfinity;
			GameObject gameObject = this.cameraRigController.sceneCam.gameObject;
			this.modelCamera = gameObject.AddComponent<ModelCamera>();
			this.cameraRigController.transform.position = -Vector3.forward * 10f;
			this.cameraRigController.transform.forward = Vector3.forward;
			CameraResolutionScaler component = gameObject.GetComponent<CameraResolutionScaler>();
			if (component)
			{
				component.enabled = false;
			}
			Camera sceneCam = this.cameraRigController.sceneCam;
			sceneCam.backgroundColor = Color.clear;
			sceneCam.clearFlags = CameraClearFlags.Color;
			if (this.disablePostProcessLayer)
			{
				PostProcessLayer component2 = sceneCam.GetComponent<PostProcessLayer>();
				if (component2)
				{
					component2.enabled = false;
				}
			}
			Vector3 eulerAngles = this.cameraRigController.transform.eulerAngles;
			this.orbitPitch = eulerAngles.x;
			this.orbitYaw = eulerAngles.y;
			this.modelCamera.attachedCamera.backgroundColor = this.camBackgroundColor;
			this.modelCamera.attachedCamera.clearFlags = CameraClearFlags.Color;
			this.modelCamera.attachedCamera.cullingMask = LayerIndex.manualRender.mask;
			if (this.headlightPrefab)
			{
				this.headlight = UnityEngine.Object.Instantiate<GameObject>(this.headlightPrefab, this.modelCamera.transform).GetComponent<Light>();
				if (this.headlight)
				{
					this.headlight.gameObject.SetActive(true);
					this.modelCamera.AddLight(this.headlight);
				}
			}
			for (int i = 0; i < this.lightPrefabs.Length; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.lightPrefabs[i]);
				Light component3 = gameObject2.GetComponent<Light>();
				gameObject2.SetActive(true);
				this.lights.Add(component3);
				this.modelCamera.AddLight(component3);
			}
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x00019732 File Offset: 0x00017932
		public void Start()
		{
			this.BuildRenderTexture();
			this.desiredZoom = 0.5f;
			this.zoom = this.desiredZoom;
			this.zoomVelocity = 0f;
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x000A77B0 File Offset: 0x000A59B0
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.renderTexture);
			if (this.cameraRigController)
			{
				UnityEngine.Object.Destroy(this.cameraRigController.gameObject);
			}
			foreach (Light light in this.lights)
			{
				UnityEngine.Object.Destroy(light.gameObject);
			}
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x0001975C File Offset: 0x0001795C
		private void OnDisable()
		{
			this.DestroyModelInstance();
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x00019764 File Offset: 0x00017964
		private void OnEnable()
		{
			this.BuildModelInstance();
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x0001976C File Offset: 0x0001796C
		public void Update()
		{
			this.UpdateForModelViewer(Time.unscaledDeltaTime);
		}

		// Token: 0x060022ED RID: 8941 RVA: 0x000A7830 File Offset: 0x000A5A30
		public void LateUpdate()
		{
			this.modelCamera.attachedCamera.aspect = (float)this.renderTexture.width / (float)this.renderTexture.height;
			this.cameraRigController.baseFov = this.fov;
			this.modelCamera.renderSettings = this.renderSettings;
			this.modelCamera.RenderItem(this.modelInstance, this.renderTexture);
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x00019779 File Offset: 0x00017979
		private void OnRectTransformDimensionsChange()
		{
			this.BuildRenderTexture();
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x000A78A0 File Offset: 0x000A5AA0
		private void BuildRenderTexture()
		{
			if (!this.rectTransform)
			{
				return;
			}
			Vector3[] fourCornersArray = new Vector3[4];
			this.rectTransform.GetLocalCorners(fourCornersArray);
			Vector2 size = this.rectTransform.rect.size;
			int num = Mathf.FloorToInt(size.x);
			int num2 = Mathf.FloorToInt(size.y);
			if (this.renderTexture && this.renderTexture.width == num && this.renderTexture.height == num2)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.renderTexture);
			this.renderTexture = null;
			if (num > 0 && num2 > 0)
			{
				this.renderTexture = new RenderTexture(new RenderTextureDescriptor(num, num2, RenderTextureFormat.ARGB32)
				{
					sRGB = true
				});
				this.renderTexture.useMipMap = false;
				this.renderTexture.filterMode = FilterMode.Bilinear;
			}
			this.rawImage.texture = this.renderTexture;
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000A7984 File Offset: 0x000A5B84
		private void UpdateForModelViewer(float deltaTime)
		{
			this.zoom = Mathf.SmoothDamp(this.zoom, this.desiredZoom, ref this.zoomVelocity, 0.1f);
			this.orbitPitch = Mathf.Clamp(this.orbitPitch + this.orbitalVelocity.x * deltaTime, -89f, 89f);
			this.orbitYaw += this.orbitalVelocity.y * deltaTime;
			this.orbitalVelocity = Vector3.SmoothDamp(this.orbitalVelocity, Vector3.zero, ref this.orbitalVelocitySmoothDampVelocity, 0.25f, 2880f, deltaTime);
			if (this.orbitDragCount > 0)
			{
				this.orbitalVelocity = Vector3.zero;
				this.orbitalVelocitySmoothDampVelocity = Vector3.zero;
			}
			this.pan += this.panVelocity * deltaTime;
			this.panVelocity = Vector2.SmoothDamp(this.panVelocity, Vector2.zero, ref this.panVelocitySmoothDampVelocity, 0.25f, 100f, deltaTime);
			if (this.panDragCount > 0)
			{
				this.panVelocity = Vector2.zero;
				this.panVelocitySmoothDampVelocity = Vector2.zero;
			}
			Quaternion rotation = Quaternion.Euler(this.orbitPitch, this.orbitYaw, 0f);
			this.cameraRigController.transform.forward = rotation * Vector3.forward;
			Vector3 forward = this.cameraRigController.transform.forward;
			Vector3 position = this.pivotPoint + forward * -Mathf.LerpUnclamped(this.minDistance, this.maxDistance, this.zoom) + this.cameraRigController.transform.up * this.pan.y + this.cameraRigController.transform.right * this.pan.x;
			this.cameraRigController.transform.position = position;
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x000A7B68 File Offset: 0x000A5D68
		public void SetAnglesForCharacterThumbnailForSeconds(float time, bool setZoom = false)
		{
			this.SetAnglesForCharacterThumbnail(setZoom);
			float t = time;
			Action func = null;
			func = delegate()
			{
				t -= Time.deltaTime;
				if (this)
				{
					this.SetAnglesForCharacterThumbnail(setZoom);
				}
				if (t <= 0f)
				{
					RoR2Application.onUpdate -= func;
				}
			};
			RoR2Application.onUpdate += func;
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x000A7BC0 File Offset: 0x000A5DC0
		public void SetAnglesForCharacterThumbnail(bool setZoom = false)
		{
			if (!this.modelInstance)
			{
				return;
			}
			ModelPanel.CameraFramingCalculator cameraFramingCalculator = new ModelPanel.CameraFramingCalculator(this.modelInstance);
			cameraFramingCalculator.GetCharacterThumbnailPosition(this.fov);
			this.pivotPoint = cameraFramingCalculator.outputPivotPoint;
			this.ResetOrbitAndPan();
			Vector3 eulerAngles = cameraFramingCalculator.outputCameraRotation.eulerAngles;
			this.orbitPitch = eulerAngles.x;
			this.orbitYaw = eulerAngles.y;
			if (setZoom)
			{
				this.zoom = Util.Remap(Vector3.Distance(cameraFramingCalculator.outputCameraPosition, cameraFramingCalculator.outputPivotPoint), this.minDistance, this.maxDistance, 0f, 1f);
				this.desiredZoom = this.zoom;
			}
			this.zoomVelocity = 0f;
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x000A7C78 File Offset: 0x000A5E78
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				this.orbitDragCount++;
				if (this.orbitDragCount == 1)
				{
					this.orbitDragPoint = eventData.pressPosition;
					return;
				}
			}
			else if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.panDragCount++;
				if (this.panDragCount == 1)
				{
					this.panDragPoint = eventData.pressPosition;
				}
			}
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x00019781 File Offset: 0x00017981
		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				this.orbitDragCount--;
			}
			else if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.panDragCount--;
			}
			this.OnDrag(eventData);
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000A7CE0 File Offset: 0x000A5EE0
		public void OnDrag(PointerEventData eventData)
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Vector2 vector = eventData.position - this.orbitDragPoint;
				this.orbitDragPoint = eventData.position;
				float num = 0.5f / unscaledDeltaTime;
				this.orbitalVelocity = new Vector3(-vector.y * num * 0.5f, vector.x * num, 0f);
				return;
			}
			Vector2 a = eventData.position - this.panDragPoint;
			this.panDragPoint = eventData.position;
			float d = -0.01f;
			this.panVelocity = a * d / unscaledDeltaTime;
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000197B9 File Offset: 0x000179B9
		public void OnScroll(PointerEventData eventData)
		{
			this.desiredZoom = Mathf.Clamp01(this.desiredZoom + eventData.scrollDelta.y * -0.05f);
		}

		// Token: 0x040025B0 RID: 9648
		private GameObject _modelPrefab;

		// Token: 0x040025B1 RID: 9649
		public RenderSettingsState renderSettings;

		// Token: 0x040025B2 RID: 9650
		public Color camBackgroundColor = Color.clear;

		// Token: 0x040025B3 RID: 9651
		public bool disablePostProcessLayer = true;

		// Token: 0x040025B4 RID: 9652
		private RectTransform rectTransform;

		// Token: 0x040025B5 RID: 9653
		private RawImage rawImage;

		// Token: 0x040025B6 RID: 9654
		private GameObject modelInstance;

		// Token: 0x040025B7 RID: 9655
		private CameraRigController cameraRigController;

		// Token: 0x040025B8 RID: 9656
		private ModelCamera modelCamera;

		// Token: 0x040025B9 RID: 9657
		public GameObject headlightPrefab;

		// Token: 0x040025BA RID: 9658
		public GameObject[] lightPrefabs;

		// Token: 0x040025BB RID: 9659
		private Light headlight;

		// Token: 0x040025BD RID: 9661
		public float fov = 60f;

		// Token: 0x040025BE RID: 9662
		private float zoom = 0.5f;

		// Token: 0x040025BF RID: 9663
		private float desiredZoom = 0.5f;

		// Token: 0x040025C0 RID: 9664
		private float zoomVelocity;

		// Token: 0x040025C1 RID: 9665
		private float minDistance = 0.5f;

		// Token: 0x040025C2 RID: 9666
		private float maxDistance = 10f;

		// Token: 0x040025C3 RID: 9667
		private float orbitPitch;

		// Token: 0x040025C4 RID: 9668
		private float orbitYaw = 180f;

		// Token: 0x040025C5 RID: 9669
		private Vector3 orbitalVelocity = Vector3.zero;

		// Token: 0x040025C6 RID: 9670
		private Vector3 orbitalVelocitySmoothDampVelocity = Vector3.zero;

		// Token: 0x040025C7 RID: 9671
		private Vector2 pan;

		// Token: 0x040025C8 RID: 9672
		private Vector2 panVelocity;

		// Token: 0x040025C9 RID: 9673
		private Vector2 panVelocitySmoothDampVelocity;

		// Token: 0x040025CA RID: 9674
		private Vector3 pivotPoint = Vector3.zero;

		// Token: 0x040025CB RID: 9675
		private List<Light> lights = new List<Light>();

		// Token: 0x040025CC RID: 9676
		private Vector2 orbitDragPoint;

		// Token: 0x040025CD RID: 9677
		private Vector2 panDragPoint;

		// Token: 0x040025CE RID: 9678
		private int orbitDragCount;

		// Token: 0x040025CF RID: 9679
		private int panDragCount;

		// Token: 0x0200060D RID: 1549
		private class CameraFramingCalculator
		{
			// Token: 0x060022F8 RID: 8952 RVA: 0x000A7E18 File Offset: 0x000A6018
			private static void GenerateBoneList(Transform rootBone, List<Transform> boneList)
			{
				boneList.Add(rootBone);
				for (int i = 0; i < boneList.Count; i++)
				{
					Transform transform = boneList[i];
					int j = 0;
					int childCount = transform.childCount;
					while (j < childCount)
					{
						boneList.Add(transform.GetChild(j));
						j++;
					}
				}
			}

			// Token: 0x060022F9 RID: 8953 RVA: 0x000A7E68 File Offset: 0x000A6068
			public CameraFramingCalculator(GameObject modelInstance)
			{
				this.modelInstance = modelInstance;
				this.root = modelInstance.transform;
				ModelPanel.CameraFramingCalculator.GenerateBoneList(this.root, this.boneList);
				this.hurtBoxGroup = modelInstance.GetComponent<HurtBoxGroup>();
				if (this.hurtBoxGroup)
				{
					this.hurtBoxes = this.hurtBoxGroup.hurtBoxes;
				}
			}

			// Token: 0x060022FA RID: 8954 RVA: 0x000A7EE0 File Offset: 0x000A60E0
			private bool FindBestEyePoint(out Vector3 result, out float approximateEyeRadius)
			{
				approximateEyeRadius = 1f;
				IEnumerable<Transform> source = from bone in this.boneList
				where bone.name.Equals("eye", StringComparison.OrdinalIgnoreCase) || bone.name.Equals("eyeball.1", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower(CultureInfo.InvariantCulture).Contains("eye")
					select bone;
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				return array.Length != 0;
			}

			// Token: 0x060022FB RID: 8955 RVA: 0x000A7F90 File Offset: 0x000A6190
			private bool FindBestHeadPoint(out Vector3 result, out float approximateRadius)
			{
				IEnumerable<Transform> source = from bone in this.boneList
				where string.Equals(bone.name, "head", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower(CultureInfo.InvariantCulture).Contains("head")
					select bone;
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				approximateRadius = 1f;
				IEnumerable<float> source2 = from hurtBox in (from bone in source
				select bone.GetComponentInChildren<HurtBox>() into hurtBox
				where hurtBox
				select hurtBox).Distinct<HurtBox>()
				select Util.SphereVolumeToRadius(hurtBox.volume);
				if (source2.Any<float>())
				{
					approximateRadius = source2.Max();
				}
				Transform transform = this.boneList.Find((Transform bone) => string.Equals(bone.name, "chest", StringComparison.OrdinalIgnoreCase));
				if (transform)
				{
					approximateRadius = Mathf.Max(Vector3.Distance(transform.position, result), approximateRadius);
					result = (result + transform.position) / 2f;
				}
				return array.Length != 0;
			}

			// Token: 0x060022FC RID: 8956 RVA: 0x000A8130 File Offset: 0x000A6330
			private static float CalcMagnitudeToFrameSphere(float sphereRadius, float fieldOfView)
			{
				float num = fieldOfView * 0.5f;
				float num2 = 90f;
				return Mathf.Tan((180f - num2 - num) * 0.0174532924f) * sphereRadius;
			}

			// Token: 0x060022FD RID: 8957 RVA: 0x000A8164 File Offset: 0x000A6364
			private bool FindBestCenterOfMass(out Vector3 result, out float approximateRadius)
			{
				from bone in this.boneList
				select bone.GetComponent<HurtBox>() into hurtBox
				where hurtBox
				select hurtBox;
				if (this.hurtBoxGroup && this.hurtBoxGroup.mainHurtBox)
				{
					result = this.hurtBoxGroup.mainHurtBox.transform.position;
					approximateRadius = Util.SphereVolumeToRadius(this.hurtBoxGroup.mainHurtBox.volume);
					return true;
				}
				result = Vector3.zero;
				approximateRadius = 1f;
				return false;
			}

			// Token: 0x060022FE RID: 8958 RVA: 0x000A8228 File Offset: 0x000A6428
			public void GetCharacterThumbnailPosition(float fov)
			{
				Vector3 vector = Vector3.zero;
				float sphereRadius = 1f;
				bool flag = this.FindBestHeadPoint(out vector, out sphereRadius);
				bool flag2 = false;
				float num = 1f;
				float num2 = 1f;
				Vector3 vector2;
				bool flag3 = this.FindBestEyePoint(out vector2, out num2);
				if (!flag)
				{
					sphereRadius = num2;
				}
				if (flag3)
				{
					vector = vector2;
				}
				if (!flag && !flag3)
				{
					flag2 = this.FindBestCenterOfMass(out vector, out sphereRadius);
				}
				float num3 = 1f;
				Bounds bounds;
				if (Util.GuessRenderBoundsMeshOnly(this.modelInstance, out bounds))
				{
					if (flag2)
					{
						sphereRadius = Util.SphereVolumeToRadius(bounds.size.x * bounds.size.y * bounds.size.z);
					}
					num3 = bounds.size.z / bounds.size.x;
					float num4 = Mathf.Max((vector.y - bounds.min.y) / bounds.size.y - 0.5f - 0.2f, 0f);
					vector.y -= num4 * 0.5f * bounds.size.y;
				}
				Vector3 vector3 = -this.root.forward;
				vector3 = Quaternion.Euler(0f, 57.29578f * Mathf.Atan(num3 - 1f) * 1f, 0f) * vector3;
				Vector3 b = -vector3 * (ModelPanel.CameraFramingCalculator.CalcMagnitudeToFrameSphere(sphereRadius, fov) + num);
				Vector3 b2 = vector + b;
				this.outputPivotPoint = vector;
				this.outputCameraPosition = b2;
				this.outputCameraRotation = Util.QuaternionSafeLookRotation(vector - b2);
			}

			// Token: 0x040025D0 RID: 9680
			private GameObject modelInstance;

			// Token: 0x040025D1 RID: 9681
			private Transform root;

			// Token: 0x040025D2 RID: 9682
			private readonly List<Transform> boneList = new List<Transform>();

			// Token: 0x040025D3 RID: 9683
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x040025D4 RID: 9684
			private HurtBox[] hurtBoxes = Array.Empty<HurtBox>();

			// Token: 0x040025D5 RID: 9685
			public Vector3 outputPivotPoint;

			// Token: 0x040025D6 RID: 9686
			public Vector3 outputCameraPosition;

			// Token: 0x040025D7 RID: 9687
			public Quaternion outputCameraRotation;
		}
	}
}
