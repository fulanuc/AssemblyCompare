using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200040A RID: 1034
	[DisallowMultipleComponent]
	public class PositionIndicator : MonoBehaviour
	{
		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06001715 RID: 5909 RVA: 0x00011464 File Offset: 0x0000F664
		// (set) Token: 0x06001716 RID: 5910 RVA: 0x0001146C File Offset: 0x0000F66C
		public Vector3 defaultPosition { get; set; }

		// Token: 0x06001717 RID: 5911 RVA: 0x000794F0 File Offset: 0x000776F0
		private void Start()
		{
			this.transform = base.transform;
			if (!this.generateDefaultPosition)
			{
				this.generateDefaultPosition = true;
				this.defaultPosition = base.transform.position;
			}
			if (this.targetTransform)
			{
				this.yOffset = PositionIndicator.CalcHeadOffset(this.targetTransform) + 1f;
			}
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x00079550 File Offset: 0x00077750
		private static float CalcHeadOffset(Transform transform)
		{
			Collider component = transform.GetComponent<Collider>();
			if (component)
			{
				return component.bounds.extents.y;
			}
			return 0f;
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x00011475 File Offset: 0x0000F675
		private void OnEnable()
		{
			PositionIndicator.instancesList.Add(this);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x00011482 File Offset: 0x0000F682
		private void OnDisable()
		{
			PositionIndicator.instancesList.Remove(this);
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x00079588 File Offset: 0x00077788
		private void OnValidate()
		{
			if (this.insideViewObject && this.insideViewObject.GetComponentInChildren<PositionIndicator>())
			{
				Debug.LogError("insideViewObject may not be assigned another object with another PositionIndicator in its heirarchy!");
				this.insideViewObject = null;
			}
			if (this.outsideViewObject && this.outsideViewObject.GetComponentInChildren<PositionIndicator>())
			{
				Debug.LogError("outsideViewObject may not be assigned another object with another PositionIndicator in its heirarchy!");
				this.outsideViewObject = null;
			}
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00011490 File Offset: 0x0000F690
		static PositionIndicator()
		{
			UICamera.onUICameraPreCull += PositionIndicator.UpdatePositions;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x000795F8 File Offset: 0x000777F8
		private static void UpdatePositions(UICamera uiCamera)
		{
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			Camera camera = uiCamera.camera;
			Rect pixelRect = camera.pixelRect;
			Vector2 center = pixelRect.center;
			pixelRect.size *= 0.95f;
			pixelRect.center = center;
			Vector2 center2 = pixelRect.center;
			float num = 1f / (pixelRect.width * 0.5f);
			float num2 = 1f / (pixelRect.height * 0.5f);
			Quaternion rotation = uiCamera.transform.rotation;
			CameraRigController cameraRigController = uiCamera.cameraRigController;
			Transform y = null;
			if (cameraRigController && cameraRigController.target)
			{
				CharacterBody component = cameraRigController.target.GetComponent<CharacterBody>();
				if (component)
				{
					y = component.coreTransform;
				}
				else
				{
					y = cameraRigController.target.transform;
				}
			}
			for (int i = 0; i < PositionIndicator.instancesList.Count; i++)
			{
				PositionIndicator positionIndicator = PositionIndicator.instancesList[i];
				bool flag = false;
				bool flag2 = false;
				bool active = false;
				float num3 = 0f;
				Vector3 a = positionIndicator.targetTransform ? positionIndicator.targetTransform.position : positionIndicator.defaultPosition;
				if (!positionIndicator.targetTransform || (positionIndicator.targetTransform && positionIndicator.targetTransform != y))
				{
					active = true;
					Vector3 vector = sceneCam.WorldToScreenPoint(a + new Vector3(0f, positionIndicator.yOffset, 0f));
					bool flag3 = vector.z <= 0f;
					bool flag4 = !flag3 && pixelRect.Contains(vector);
					if (!flag4)
					{
						Vector2 vector2 = vector - center2;
						float a2 = Mathf.Abs(vector2.x * num);
						float b = Mathf.Abs(vector2.y * num2);
						float d = Mathf.Max(a2, b);
						vector2 /= d;
						vector2 *= (flag3 ? -1f : 1f);
						vector = vector2 + center2;
						if (positionIndicator.shouldRotateOutsideViewObject)
						{
							num3 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
						}
					}
					flag = flag4;
					flag2 = !flag4;
					vector.z = 1f;
					positionIndicator.transform.position = camera.ScreenToWorldPoint(vector);
					positionIndicator.transform.rotation = rotation;
				}
				if (positionIndicator.alwaysVisibleObject)
				{
					positionIndicator.alwaysVisibleObject.SetActive(active);
				}
				if (positionIndicator.insideViewObject == positionIndicator.outsideViewObject)
				{
					if (positionIndicator.insideViewObject)
					{
						positionIndicator.insideViewObject.SetActive(flag || flag2);
					}
				}
				else
				{
					if (positionIndicator.insideViewObject)
					{
						positionIndicator.insideViewObject.SetActive(flag);
					}
					if (positionIndicator.outsideViewObject)
					{
						positionIndicator.outsideViewObject.SetActive(flag2);
						if (flag2 && positionIndicator.shouldRotateOutsideViewObject)
						{
							positionIndicator.outsideViewObject.transform.localEulerAngles = new Vector3(0f, 0f, num3 + positionIndicator.outsideViewRotationOffset);
						}
					}
				}
			}
		}

		// Token: 0x04001A38 RID: 6712
		public Transform targetTransform;

		// Token: 0x04001A39 RID: 6713
		private new Transform transform;

		// Token: 0x04001A3A RID: 6714
		private static readonly List<PositionIndicator> instancesList = new List<PositionIndicator>();

		// Token: 0x04001A3B RID: 6715
		[Tooltip("The child object to enable when the target is within the frame.")]
		public GameObject insideViewObject;

		// Token: 0x04001A3C RID: 6716
		[Tooltip("The child object to enable when the target is outside the frame.")]
		public GameObject outsideViewObject;

		// Token: 0x04001A3D RID: 6717
		[Tooltip("The child object to ALWAYS enable, IF its not my own position indicator.")]
		public GameObject alwaysVisibleObject;

		// Token: 0x04001A3E RID: 6718
		[Tooltip("Whether or not outsideViewObject should be rotated to point to the target.")]
		public bool shouldRotateOutsideViewObject;

		// Token: 0x04001A3F RID: 6719
		[Tooltip("The offset to apply to the rotation of the outside view object when shouldRotateOutsideViewObject is set.")]
		public float outsideViewRotationOffset;

		// Token: 0x04001A40 RID: 6720
		private float yOffset;

		// Token: 0x04001A41 RID: 6721
		private bool generateDefaultPosition;
	}
}
