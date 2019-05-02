using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200027E RID: 638
	public class CameraTargetParams : MonoBehaviour
	{
		// Token: 0x06000C11 RID: 3089 RVA: 0x000025F6 File Offset: 0x000007F6
		public void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000C12 RID: 3090 RVA: 0x000097B7 File Offset: 0x000079B7
		public Vector3 cameraPivotPosition
		{
			get
			{
				return (this.cameraPivotTransform ? this.cameraPivotTransform : base.transform).position + new Vector3(0f, this.currentPivotVerticalOffset, 0f);
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0004EDB0 File Offset: 0x0004CFB0
		private void Awake()
		{
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component && this.cameraPivotTransform == null)
			{
				this.cameraPivotTransform = component.aimOriginTransform;
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0004EDE8 File Offset: 0x0004CFE8
		private void Update()
		{
			this.targetRecoil = Vector2.SmoothDamp(this.targetRecoil, Vector2.zero, ref this.targetRecoilVelocity, CameraTargetParams.targetRecoilDampTime, 180f, Time.deltaTime);
			this.recoil = Vector2.SmoothDamp(this.recoil, this.targetRecoil, ref this.recoilVelocity, CameraTargetParams.recoilDampTime, 180f, Time.deltaTime);
			switch (this.aimMode)
			{
			case CameraTargetParams.AimType.Standard:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos, ref this.aimVelocity, 0.5f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.FirstPerson:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, Vector3.zero, ref this.aimVelocity, 0.4f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, 0f, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.Aura:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos + new Vector3(0f, 1.5f, -7f), ref this.aimVelocity, 0.5f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.Sprinting:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos + new Vector3(0f, 0f, 0f), ref this.aimVelocity, 1f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.1f);
				return;
			case CameraTargetParams.AimType.AimThrow:
			{
				Vector3 standardLocalCameraPos = this.cameraParams.standardLocalCameraPos;
				standardLocalCameraPos.z = -8f;
				standardLocalCameraPos.y = 1f;
				standardLocalCameraPos.x = 1f;
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, standardLocalCameraPos, ref this.aimVelocity, 0.4f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, 0f, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0400101B RID: 4123
		public CharacterCameraParams cameraParams;

		// Token: 0x0400101C RID: 4124
		public Transform cameraPivotTransform;

		// Token: 0x0400101D RID: 4125
		public CameraTargetParams.AimType aimMode;

		// Token: 0x0400101E RID: 4126
		[HideInInspector]
		public Vector2 recoil;

		// Token: 0x0400101F RID: 4127
		[HideInInspector]
		public Vector3 idealLocalCameraPos;

		// Token: 0x04001020 RID: 4128
		[HideInInspector]
		[NonSerialized]
		public float fovOverride = -1f;

		// Token: 0x04001021 RID: 4129
		[HideInInspector]
		public bool dontRaycastToPivot;

		// Token: 0x04001022 RID: 4130
		private float currentPivotVerticalOffset;

		// Token: 0x04001023 RID: 4131
		private float currentPivotVerticalOffsetVelocity;

		// Token: 0x04001024 RID: 4132
		private static float targetRecoilDampTime = 0.08f;

		// Token: 0x04001025 RID: 4133
		private static float recoilDampTime = 0.05f;

		// Token: 0x04001026 RID: 4134
		private Vector2 targetRecoil;

		// Token: 0x04001027 RID: 4135
		private Vector2 recoilVelocity;

		// Token: 0x04001028 RID: 4136
		private Vector2 targetRecoilVelocity;

		// Token: 0x04001029 RID: 4137
		private Vector3 aimVelocity;

		// Token: 0x0200027F RID: 639
		public enum AimType
		{
			// Token: 0x0400102B RID: 4139
			Standard,
			// Token: 0x0400102C RID: 4140
			FirstPerson,
			// Token: 0x0400102D RID: 4141
			Aura,
			// Token: 0x0400102E RID: 4142
			Sprinting,
			// Token: 0x0400102F RID: 4143
			AimThrow
		}
	}
}
