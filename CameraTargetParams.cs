using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200027E RID: 638
	public class CameraTargetParams : MonoBehaviour
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x000097E4 File Offset: 0x000079E4
		public void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
			this.targetRecoil += new Vector2(UnityEngine.Random.Range(horizontalMin, horizontalMax), UnityEngine.Random.Range(verticalMin, verticalMax));
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000C1D RID: 3101 RVA: 0x0000980B File Offset: 0x00007A0B
		public Vector3 cameraPivotPosition
		{
			get
			{
				return (this.cameraPivotTransform ? this.cameraPivotTransform : base.transform).position + new Vector3(0f, this.currentPivotVerticalOffset, 0f);
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0004EFF0 File Offset: 0x0004D1F0
		private void Awake()
		{
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component && this.cameraPivotTransform == null)
			{
				this.cameraPivotTransform = component.aimOriginTransform;
			}
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0004F028 File Offset: 0x0004D228
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

		// Token: 0x04001022 RID: 4130
		public CharacterCameraParams cameraParams;

		// Token: 0x04001023 RID: 4131
		public Transform cameraPivotTransform;

		// Token: 0x04001024 RID: 4132
		public CameraTargetParams.AimType aimMode;

		// Token: 0x04001025 RID: 4133
		[HideInInspector]
		public Vector2 recoil;

		// Token: 0x04001026 RID: 4134
		[HideInInspector]
		public Vector3 idealLocalCameraPos;

		// Token: 0x04001027 RID: 4135
		[HideInInspector]
		[NonSerialized]
		public float fovOverride = -1f;

		// Token: 0x04001028 RID: 4136
		[HideInInspector]
		public bool dontRaycastToPivot;

		// Token: 0x04001029 RID: 4137
		private float currentPivotVerticalOffset;

		// Token: 0x0400102A RID: 4138
		private float currentPivotVerticalOffsetVelocity;

		// Token: 0x0400102B RID: 4139
		private static float targetRecoilDampTime = 0.08f;

		// Token: 0x0400102C RID: 4140
		private static float recoilDampTime = 0.05f;

		// Token: 0x0400102D RID: 4141
		private Vector2 targetRecoil;

		// Token: 0x0400102E RID: 4142
		private Vector2 recoilVelocity;

		// Token: 0x0400102F RID: 4143
		private Vector2 targetRecoilVelocity;

		// Token: 0x04001030 RID: 4144
		private Vector3 aimVelocity;

		// Token: 0x0200027F RID: 639
		public enum AimType
		{
			// Token: 0x04001032 RID: 4146
			Standard,
			// Token: 0x04001033 RID: 4147
			FirstPerson,
			// Token: 0x04001034 RID: 4148
			Aura,
			// Token: 0x04001035 RID: 4149
			Sprinting,
			// Token: 0x04001036 RID: 4150
			AimThrow
		}
	}
}
