﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000410 RID: 1040
	[DisallowMultipleComponent]
	public class PositionIndicator : MonoBehaviour
	{
		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x00011890 File Offset: 0x0000FA90
		// (set) Token: 0x06001759 RID: 5977 RVA: 0x00011898 File Offset: 0x0000FA98
		public Vector3 defaultPosition { get; set; }

		// Token: 0x0600175A RID: 5978 RVA: 0x00079AB0 File Offset: 0x00077CB0
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

		// Token: 0x0600175B RID: 5979 RVA: 0x00079B10 File Offset: 0x00077D10
		private static float CalcHeadOffset(Transform transform)
		{
			Collider component = transform.GetComponent<Collider>();
			if (component)
			{
				return component.bounds.extents.y;
			}
			return 0f;
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x000118A1 File Offset: 0x0000FAA1
		private void OnEnable()
		{
			PositionIndicator.instancesList.Add(this);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x000118AE File Offset: 0x0000FAAE
		private void OnDisable()
		{
			PositionIndicator.instancesList.Remove(this);
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x00079B48 File Offset: 0x00077D48
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

		// Token: 0x0600175F RID: 5983 RVA: 0x000118BC File Offset: 0x0000FABC
		static PositionIndicator()
		{
			UICamera.onUICameraPreCull += PositionIndicator.UpdatePositions;
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x00079BB8 File Offset: 0x00077DB8
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

		// Token: 0x04001A61 RID: 6753
		public Transform targetTransform;

		// Token: 0x04001A62 RID: 6754
		private new Transform transform;

		// Token: 0x04001A63 RID: 6755
		private static readonly List<PositionIndicator> instancesList = new List<PositionIndicator>();

		// Token: 0x04001A64 RID: 6756
		[Tooltip("The child object to enable when the target is within the frame.")]
		public GameObject insideViewObject;

		// Token: 0x04001A65 RID: 6757
		[Tooltip("The child object to enable when the target is outside the frame.")]
		public GameObject outsideViewObject;

		// Token: 0x04001A66 RID: 6758
		[Tooltip("The child object to ALWAYS enable, IF its not my own position indicator.")]
		public GameObject alwaysVisibleObject;

		// Token: 0x04001A67 RID: 6759
		[Tooltip("Whether or not outsideViewObject should be rotated to point to the target.")]
		public bool shouldRotateOutsideViewObject;

		// Token: 0x04001A68 RID: 6760
		[Tooltip("The offset to apply to the rotation of the outside view object when shouldRotateOutsideViewObject is set.")]
		public float outsideViewRotationOffset;

		// Token: 0x04001A69 RID: 6761
		private float yOffset;

		// Token: 0x04001A6A RID: 6762
		private bool generateDefaultPosition;
	}
}
