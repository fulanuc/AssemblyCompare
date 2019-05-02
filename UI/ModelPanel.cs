using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FA RID: 1530
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RawImage))]
	public class ModelPanel : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IScrollHandler, IEndDragHandler
	{
		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x00019034 File Offset: 0x00017234
		// (set) Token: 0x06002251 RID: 8785 RVA: 0x0001903C File Offset: 0x0001723C
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

		// Token: 0x06002252 RID: 8786 RVA: 0x00019060 File Offset: 0x00017260
		private void DestroyModelInstance()
		{
			UnityEngine.Object.Destroy(this.modelInstance);
			this.modelInstance = null;
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x000A5BA0 File Offset: 0x000A3DA0
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

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06002254 RID: 8788 RVA: 0x00019074 File Offset: 0x00017274
		// (set) Token: 0x06002255 RID: 8789 RVA: 0x0001907C File Offset: 0x0001727C
		public RenderTexture renderTexture { get; private set; }

		// Token: 0x06002256 RID: 8790 RVA: 0x000A5E78 File Offset: 0x000A4078
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

		// Token: 0x06002257 RID: 8791 RVA: 0x000A5ED4 File Offset: 0x000A40D4
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

		// Token: 0x06002258 RID: 8792 RVA: 0x00019085 File Offset: 0x00017285
		public void Start()
		{
			this.BuildRenderTexture();
			this.desiredZoom = 0.5f;
			this.zoom = this.desiredZoom;
			this.zoomVelocity = 0f;
		}

		// Token: 0x06002259 RID: 8793 RVA: 0x000A6134 File Offset: 0x000A4334
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

		// Token: 0x0600225A RID: 8794 RVA: 0x000190AF File Offset: 0x000172AF
		private void OnDisable()
		{
			this.DestroyModelInstance();
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x000190B7 File Offset: 0x000172B7
		private void OnEnable()
		{
			this.BuildModelInstance();
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x000190BF File Offset: 0x000172BF
		public void Update()
		{
			this.UpdateForModelViewer(Time.unscaledDeltaTime);
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x000A61B4 File Offset: 0x000A43B4
		public void LateUpdate()
		{
			this.modelCamera.attachedCamera.aspect = (float)this.renderTexture.width / (float)this.renderTexture.height;
			this.cameraRigController.baseFov = this.fov;
			this.modelCamera.renderSettings = this.renderSettings;
			this.modelCamera.RenderItem(this.modelInstance, this.renderTexture);
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x000190CC File Offset: 0x000172CC
		private void OnRectTransformDimensionsChange()
		{
			this.BuildRenderTexture();
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x000A6224 File Offset: 0x000A4424
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

		// Token: 0x06002260 RID: 8800 RVA: 0x000A6308 File Offset: 0x000A4508
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

		// Token: 0x06002261 RID: 8801 RVA: 0x000A64EC File Offset: 0x000A46EC
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

		// Token: 0x06002262 RID: 8802 RVA: 0x000A6544 File Offset: 0x000A4744
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

		// Token: 0x06002263 RID: 8803 RVA: 0x000A65FC File Offset: 0x000A47FC
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

		// Token: 0x06002264 RID: 8804 RVA: 0x000190D4 File Offset: 0x000172D4
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

		// Token: 0x06002265 RID: 8805 RVA: 0x000A6664 File Offset: 0x000A4864
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

		// Token: 0x06002266 RID: 8806 RVA: 0x0001910C File Offset: 0x0001730C
		public void OnScroll(PointerEventData eventData)
		{
			this.desiredZoom = Mathf.Clamp01(this.desiredZoom + eventData.scrollDelta.y * -0.05f);
		}

		// Token: 0x04002555 RID: 9557
		private GameObject _modelPrefab;

		// Token: 0x04002556 RID: 9558
		public RenderSettingsState renderSettings;

		// Token: 0x04002557 RID: 9559
		public Color camBackgroundColor = Color.clear;

		// Token: 0x04002558 RID: 9560
		public bool disablePostProcessLayer = true;

		// Token: 0x04002559 RID: 9561
		private RectTransform rectTransform;

		// Token: 0x0400255A RID: 9562
		private RawImage rawImage;

		// Token: 0x0400255B RID: 9563
		private GameObject modelInstance;

		// Token: 0x0400255C RID: 9564
		private CameraRigController cameraRigController;

		// Token: 0x0400255D RID: 9565
		private ModelCamera modelCamera;

		// Token: 0x0400255E RID: 9566
		public GameObject headlightPrefab;

		// Token: 0x0400255F RID: 9567
		public GameObject[] lightPrefabs;

		// Token: 0x04002560 RID: 9568
		private Light headlight;

		// Token: 0x04002562 RID: 9570
		public float fov = 60f;

		// Token: 0x04002563 RID: 9571
		private float zoom = 0.5f;

		// Token: 0x04002564 RID: 9572
		private float desiredZoom = 0.5f;

		// Token: 0x04002565 RID: 9573
		private float zoomVelocity;

		// Token: 0x04002566 RID: 9574
		private float minDistance = 0.5f;

		// Token: 0x04002567 RID: 9575
		private float maxDistance = 10f;

		// Token: 0x04002568 RID: 9576
		private float orbitPitch;

		// Token: 0x04002569 RID: 9577
		private float orbitYaw = 180f;

		// Token: 0x0400256A RID: 9578
		private Vector3 orbitalVelocity = Vector3.zero;

		// Token: 0x0400256B RID: 9579
		private Vector3 orbitalVelocitySmoothDampVelocity = Vector3.zero;

		// Token: 0x0400256C RID: 9580
		private Vector2 pan;

		// Token: 0x0400256D RID: 9581
		private Vector2 panVelocity;

		// Token: 0x0400256E RID: 9582
		private Vector2 panVelocitySmoothDampVelocity;

		// Token: 0x0400256F RID: 9583
		private Vector3 pivotPoint = Vector3.zero;

		// Token: 0x04002570 RID: 9584
		private List<Light> lights = new List<Light>();

		// Token: 0x04002571 RID: 9585
		private Vector2 orbitDragPoint;

		// Token: 0x04002572 RID: 9586
		private Vector2 panDragPoint;

		// Token: 0x04002573 RID: 9587
		private int orbitDragCount;

		// Token: 0x04002574 RID: 9588
		private int panDragCount;

		// Token: 0x020005FB RID: 1531
		private class CameraFramingCalculator
		{
			// Token: 0x06002268 RID: 8808 RVA: 0x000A679C File Offset: 0x000A499C
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

			// Token: 0x06002269 RID: 8809 RVA: 0x000A67EC File Offset: 0x000A49EC
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

			// Token: 0x0600226A RID: 8810 RVA: 0x000A6864 File Offset: 0x000A4A64
			private bool FindBestEyePoint(out Vector3 result, out float approximateEyeRadius)
			{
				approximateEyeRadius = 1f;
				IEnumerable<Transform> source = from bone in this.boneList
				where bone.name.Equals("eye", StringComparison.OrdinalIgnoreCase) || bone.name.Equals("eyeball.1", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower().Contains("eye")
					select bone;
				}
				Vector3[] array = (from bone in source
				select bone.position).ToArray<Vector3>();
				result = HGMath.Average<Vector3[]>(array);
				return array.Length != 0;
			}

			// Token: 0x0600226B RID: 8811 RVA: 0x000A6914 File Offset: 0x000A4B14
			private bool FindBestHeadPoint(out Vector3 result, out float approximateRadius)
			{
				IEnumerable<Transform> source = from bone in this.boneList
				where string.Equals(bone.name, "head", StringComparison.OrdinalIgnoreCase)
				select bone;
				if (!source.Any<Transform>())
				{
					source = from bone in this.boneList
					where bone.name.ToLower().Contains("head")
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

			// Token: 0x0600226C RID: 8812 RVA: 0x000A6AB4 File Offset: 0x000A4CB4
			private static float CalcMagnitudeToFrameSphere(float sphereRadius, float fieldOfView)
			{
				float num = fieldOfView * 0.5f;
				float num2 = 90f;
				return Mathf.Tan((180f - num2 - num) * 0.0174532924f) * sphereRadius;
			}

			// Token: 0x0600226D RID: 8813 RVA: 0x000A6AE8 File Offset: 0x000A4CE8
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

			// Token: 0x0600226E RID: 8814 RVA: 0x000A6BAC File Offset: 0x000A4DAC
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

			// Token: 0x04002575 RID: 9589
			private GameObject modelInstance;

			// Token: 0x04002576 RID: 9590
			private Transform root;

			// Token: 0x04002577 RID: 9591
			private readonly List<Transform> boneList = new List<Transform>();

			// Token: 0x04002578 RID: 9592
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x04002579 RID: 9593
			private HurtBox[] hurtBoxes = Array.Empty<HurtBox>();

			// Token: 0x0400257A RID: 9594
			public Vector3 outputPivotPoint;

			// Token: 0x0400257B RID: 9595
			public Vector3 outputCameraPosition;

			// Token: 0x0400257C RID: 9596
			public Quaternion outputCameraRotation;
		}
	}
}
