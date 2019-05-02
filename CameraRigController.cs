using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using RoR2.ConVar;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200027A RID: 634
	public class CameraRigController : MonoBehaviour
	{
		// Token: 0x06000BF0 RID: 3056 RVA: 0x0004D7F8 File Offset: 0x0004B9F8
		private void StartStateLerp(float lerpDuration)
		{
			this.lerpCameraState = this.currentCameraState;
			if (lerpDuration > 0f)
			{
				this.lerpCameraTime = 0f;
				this.lerpCameraTimeScale = 1f / lerpDuration;
				return;
			}
			this.lerpCameraTime = 1f;
			this.lerpCameraTimeScale = 0f;
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x00009660 File Offset: 0x00007860
		public Vector3 desiredPosition
		{
			get
			{
				return this.desiredCameraState.position;
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0000966D File Offset: 0x0000786D
		public void SetOverrideCam(ICameraStateProvider newOverrideCam, float lerpDuration = 1f)
		{
			if (newOverrideCam == this.overrideCam)
			{
				return;
			}
			if (this.overrideCam != null && newOverrideCam == null)
			{
				this.SetPitchYawFromLookVector(this.currentCameraState.rotation * Vector3.forward);
			}
			this.overrideCam = newOverrideCam;
			this.StartStateLerp(lerpDuration);
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x000096AD File Offset: 0x000078AD
		public bool IsOverrideCam(ICameraStateProvider testOverrideCam)
		{
			return this.overrideCam == testOverrideCam;
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0004D848 File Offset: 0x0004BA48
		private void SetPitchYawFromLookVector(Vector3 lookVector)
		{
			float x = Mathf.Sqrt(lookVector.x * lookVector.x + lookVector.z * lookVector.z);
			this.pitch = Mathf.Atan2(-lookVector.y, x) * 57.29578f;
			this.yaw = Mathf.Repeat(Mathf.Atan2(lookVector.x, lookVector.z) * 57.29578f, 360f);
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x000096B8 File Offset: 0x000078B8
		private void SetPitchYaw(PitchYawPair pitchYawPair)
		{
			this.pitch = pitchYawPair.pitch;
			this.yaw = pitchYawPair.yaw;
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x000096D2 File Offset: 0x000078D2
		// (set) Token: 0x06000BF7 RID: 3063 RVA: 0x000096DA File Offset: 0x000078DA
		public NetworkUser viewer
		{
			get
			{
				return this._viewer;
			}
			set
			{
				this._viewer = value;
				this.localUserViewer = (this._viewer ? this._viewer.localUser : null);
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x00009704 File Offset: 0x00007904
		// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x0004D8B8 File Offset: 0x0004BAB8
		public LocalUser localUserViewer
		{
			get
			{
				return this._localUserViewer;
			}
			private set
			{
				if (this._localUserViewer == value)
				{
					return;
				}
				if (this._localUserViewer != null)
				{
					this._localUserViewer.cameraRigController = null;
				}
				this._localUserViewer = value;
				if (this._localUserViewer != null)
				{
					this._localUserViewer.cameraRigController = this;
				}
				if (this.hud)
				{
					this.hud.localUserViewer = this._localUserViewer;
				}
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x0000970C File Offset: 0x0000790C
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x00009714 File Offset: 0x00007914
		public GameObject firstPersonTarget { get; private set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x0000971D File Offset: 0x0000791D
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x00009725 File Offset: 0x00007925
		public TeamIndex targetTeamIndex { get; private set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0000972E File Offset: 0x0000792E
		// (set) Token: 0x06000BFF RID: 3071 RVA: 0x00009736 File Offset: 0x00007936
		public Vector3 crosshairWorldPosition { get; private set; }

		// Token: 0x06000C00 RID: 3072 RVA: 0x0000973F File Offset: 0x0000793F
		private static bool CanUserSpectateBody(NetworkUser viewer, CharacterBody body)
		{
			return Util.LookUpBodyNetworkUser(body.gameObject);
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0004D91C File Offset: 0x0004BB1C
		public static GameObject GetNextSpectateGameObject(NetworkUser viewer, GameObject currentGameObject)
		{
			ReadOnlyCollection<CharacterBody> readOnlyCollection = CharacterBody.readOnlyInstancesList;
			if (readOnlyCollection.Count == 0)
			{
				return null;
			}
			CharacterBody characterBody = currentGameObject ? currentGameObject.GetComponent<CharacterBody>() : null;
			int num = characterBody ? readOnlyCollection.IndexOf(characterBody) : 0;
			for (int i = num + 1; i < readOnlyCollection.Count; i++)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[i]))
				{
					return readOnlyCollection[i].gameObject;
				}
			}
			for (int j = 0; j <= num; j++)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[j]))
				{
					return readOnlyCollection[j].gameObject;
				}
			}
			return null;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0004D9BC File Offset: 0x0004BBBC
		public static GameObject GetPreviousSpectateGameObject(NetworkUser viewer, GameObject currentGameObject)
		{
			ReadOnlyCollection<CharacterBody> readOnlyCollection = CharacterBody.readOnlyInstancesList;
			if (readOnlyCollection.Count == 0)
			{
				return null;
			}
			CharacterBody characterBody = currentGameObject ? currentGameObject.GetComponent<CharacterBody>() : null;
			int num = characterBody ? readOnlyCollection.IndexOf(characterBody) : 0;
			for (int i = num - 1; i >= 0; i--)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[i]))
				{
					return readOnlyCollection[i].gameObject;
				}
			}
			for (int j = readOnlyCollection.Count - 1; j >= num; j--)
			{
				if (CameraRigController.CanUserSpectateBody(viewer, readOnlyCollection[j]))
				{
					return readOnlyCollection[j].gameObject;
				}
			}
			return null;
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0004DA60 File Offset: 0x0004BC60
		private void Start()
		{
			if (this.createHud)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/HUDSimple"));
				this.hud = gameObject.GetComponent<HUD>();
				this.hud.cameraRigController = this;
				this.hud.GetComponent<Canvas>().worldCamera = this.uiCam;
				this.hud.GetComponent<CrosshairManager>().cameraRigController = this;
				this.hud.localUserViewer = this.localUserViewer;
			}
			this.currentFov = this.baseFov;
			if (this.uiCam)
			{
				this.uiCam.transform.parent = null;
				this.uiCam.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
			this.desiredCameraState = new CameraState
			{
				position = base.transform.position,
				rotation = base.transform.rotation,
				fov = this.currentFov
			};
			if (!DamageNumberManager.instance)
			{
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/DamageNumberManager"));
			}
			this.currentCameraState = this.desiredCameraState;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0004DB84 File Offset: 0x0004BD84
		private void Update()
		{
			if (Time.deltaTime == 0f)
			{
				return;
			}
			if (this.target != this.previousTarget)
			{
				this.previousTarget = this.target;
				Action<CameraRigController, GameObject> action = CameraRigController.onCameraTargetChanged;
				if (action != null)
				{
					action(this, this.target);
				}
			}
			this.lerpCameraTime += Time.deltaTime * this.lerpCameraTimeScale;
			this.firstPersonTarget = null;
			float num = this.baseFov;
			this.sceneCam.rect = this.viewport;
			Player player = null;
			UserProfile userProfile = null;
			bool flag = false;
			if (this.viewer && this.viewer.localUser != null)
			{
				player = this.viewer.localUser.inputPlayer;
				userProfile = this.viewer.localUser.userProfile;
				flag = this.viewer.localUser.isUIFocused;
			}
			if (this.cameraMode == CameraRigController.CameraMode.SpectateUser && player != null)
			{
				if (player.GetButtonDown("PrimarySkill"))
				{
					this.target = CameraRigController.GetNextSpectateGameObject(this.viewer, this.target);
				}
				if (player.GetButtonDown("SecondarySkill"))
				{
					this.target = CameraRigController.GetPreviousSpectateGameObject(this.viewer, this.target);
				}
			}
			LocalUser localUserViewer = this.localUserViewer;
			MPEventSystem mpeventSystem = (localUserViewer != null) ? localUserViewer.eventSystem : null;
			float num14;
			float num15;
			if ((!mpeventSystem || !mpeventSystem.isCursorVisible) && player != null && userProfile != null && !flag && this.overrideCam == null)
			{
				float mouseLookSensitivity = userProfile.mouseLookSensitivity;
				float num2 = userProfile.stickLookSensitivity * CameraRigController.aimStickGlobalScale.value * 45f;
				Vector2 vector = new Vector2(player.GetAxisRaw(2), player.GetAxisRaw(3));
				Vector2 vector2 = new Vector2(player.GetAxisRaw(16), player.GetAxisRaw(17));
				CameraRigController.<Update>g__ConditionalNegate|69_0(ref vector.x, userProfile.mouseLookInvertX);
				CameraRigController.<Update>g__ConditionalNegate|69_0(ref vector.y, userProfile.mouseLookInvertY);
				CameraRigController.<Update>g__ConditionalNegate|69_0(ref vector2.x, userProfile.stickLookInvertX);
				CameraRigController.<Update>g__ConditionalNegate|69_0(ref vector2.y, userProfile.stickLookInvertY);
				float magnitude = vector2.magnitude;
				float num3 = magnitude;
				this.aimStickPostSmoothing = Vector2.zero;
				this.aimStickPostDualZone = Vector2.zero;
				this.aimStickPostExponent = Vector2.zero;
				if (CameraRigController.aimStickDualZoneSmoothing.value != 0f)
				{
					float maxDelta = Time.deltaTime / CameraRigController.aimStickDualZoneSmoothing.value;
					num3 = Mathf.Min(Mathf.MoveTowards(this.stickAimPreviousAcceleratedMagnitude, magnitude, maxDelta), magnitude);
					this.stickAimPreviousAcceleratedMagnitude = num3;
					this.aimStickPostSmoothing = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				}
				float num4 = num3;
				float value = CameraRigController.aimStickDualZoneSlope.value;
				float num5;
				if (num4 <= CameraRigController.aimStickDualZoneThreshold.value)
				{
					num5 = 0f;
				}
				else
				{
					num5 = 1f - value;
				}
				num3 = value * num4 + num5;
				this.aimStickPostDualZone = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				num3 = Mathf.Pow(num3, CameraRigController.aimStickExponent.value);
				this.aimStickPostExponent = ((magnitude != 0f) ? (vector2 * (num3 / magnitude)) : Vector2.zero);
				if (magnitude != 0f)
				{
					vector2 *= num3 / magnitude;
				}
				if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic && this.targetBody && !this.targetBody.isSprinting)
				{
					AimAssistTarget exists = null;
					AimAssistTarget exists2 = null;
					float value2 = CameraRigController.aimStickAssistMinSize.value;
					float num6 = value2 * CameraRigController.aimStickAssistMaxSize.value;
					float value3 = CameraRigController.aimStickAssistMaxSlowdownScale.value;
					float value4 = CameraRigController.aimStickAssistMinSlowdownScale.value;
					float num7 = 0f;
					float value5 = 0f;
					float num8 = 0f;
					Vector2 v = Vector2.zero;
					Vector2 zero = Vector2.zero;
					Vector2 normalized = vector2.normalized;
					Vector2 vector3 = new Vector2(0.5f, 0.5f);
					for (int i = 0; i < AimAssistTarget.instancesList.Count; i++)
					{
						AimAssistTarget aimAssistTarget = AimAssistTarget.instancesList[i];
						if (aimAssistTarget.teamComponent.teamIndex != this.targetTeamIndex)
						{
							Vector3 vector4 = this.sceneCam.WorldToViewportPoint(aimAssistTarget.point0.position);
							Vector3 vector5 = this.sceneCam.WorldToViewportPoint(aimAssistTarget.point1.position);
							float num9 = Mathf.Lerp(vector4.z, vector5.z, 0.5f);
							if (num9 > 3f)
							{
								float num10 = 1f / num9;
								Vector2 vector6 = Util.ClosestPointOnLine(vector4, vector5, vector3) - vector3;
								float num11 = Mathf.Clamp01(Util.Remap(vector6.magnitude, value2 * aimAssistTarget.assistScale * num10, num6 * aimAssistTarget.assistScale * num10, 1f, 0f));
								float num12 = Mathf.Clamp01(Vector3.Dot(vector6, vector2.normalized));
								float num13 = num12 * num11;
								if (num7 < num11)
								{
									num7 = num11;
									exists2 = aimAssistTarget;
								}
								if (num13 > num8)
								{
									num7 = num11;
									value5 = num12;
									exists = aimAssistTarget;
									v = vector6;
								}
							}
						}
					}
					Vector2 vector7 = vector2;
					if (exists2)
					{
						float magnitude2 = vector2.magnitude;
						float d = Mathf.Clamp01(Util.Remap(1f - num7, 0f, 1f, value3, value4));
						vector7 *= d;
					}
					if (exists)
					{
						vector7 = Vector3.RotateTowards(vector7, v, Util.Remap(value5, 1f, 0f, CameraRigController.aimStickAssistMaxDelta.value, CameraRigController.aimStickAssistMinDelta.value), 0f);
					}
					vector2 = vector7;
				}
				num14 = vector.x * mouseLookSensitivity * userProfile.mouseLookScaleX + vector2.x * num2 * userProfile.stickLookScaleX * Time.deltaTime;
				num15 = vector.y * mouseLookSensitivity * userProfile.mouseLookScaleY + vector2.y * num2 * userProfile.stickLookScaleY * Time.deltaTime;
			}
			else
			{
				num14 = 0f;
				num15 = 0f;
			}
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(this.target);
			NetworkedViewAngles networkedViewAngles = null;
			if (networkUser)
			{
				networkedViewAngles = networkUser.GetComponent<NetworkedViewAngles>();
			}
			this.targetTeamIndex = TeamIndex.None;
			bool flag2 = false;
			if (this.target)
			{
				this.targetBody = this.target.GetComponent<CharacterBody>();
				flag2 = this.targetBody.isSprinting;
				TeamComponent component = this.target.GetComponent<TeamComponent>();
				if (component)
				{
					this.targetTeamIndex = component.teamIndex;
				}
				this.targetParams = this.target.GetComponent<CameraTargetParams>();
			}
			Vector3 vector8 = this.desiredCameraState.position;
			if (this.targetParams)
			{
				Vector3 position = this.target.transform.position;
				Vector3 cameraPivotPosition = this.targetParams.cameraPivotPosition;
				if (this.targetParams.dontRaycastToPivot)
				{
					vector8 = cameraPivotPosition;
				}
				else
				{
					Vector3 direction = cameraPivotPosition - position;
					Ray ray = new Ray(position, direction);
					float distance = this.Raycast(ray, direction.magnitude, this.targetParams.cameraParams.wallCushion);
					vector8 = ray.GetPoint(distance);
				}
			}
			if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic || this.cameraMode == CameraRigController.CameraMode.SpectateUser)
			{
				float min = -89.9f;
				float max = 89.9f;
				Vector3 idealLocalCameraPos = new Vector3(0f, 0f, 0f);
				float wallCushion = 0.1f;
				Vector2 vector9 = Vector2.zero;
				if (this.targetParams)
				{
					min = this.targetParams.cameraParams.minPitch;
					max = this.targetParams.cameraParams.maxPitch;
					idealLocalCameraPos = this.targetParams.idealLocalCameraPos;
					wallCushion = this.targetParams.cameraParams.wallCushion;
					vector9 = this.targetParams.recoil;
					if (this.targetParams.aimMode == CameraTargetParams.AimType.FirstPerson)
					{
						this.firstPersonTarget = this.target;
					}
					if (this.targetParams.fovOverride >= 0f)
					{
						num = this.targetParams.fovOverride;
						num14 *= num / this.baseFov;
						num15 *= num / this.baseFov;
					}
					if (this.targetBody && flag2)
					{
						num14 *= 0.5f;
						num15 *= 0.5f;
					}
				}
				if (this.sprintingParticleSystem)
				{
					ParticleSystem.MainModule main = this.sprintingParticleSystem.main;
					if (flag2)
					{
						main.loop = true;
						if (!this.sprintingParticleSystem.isPlaying)
						{
							this.sprintingParticleSystem.Play();
						}
					}
					else
					{
						main.loop = false;
					}
				}
				if (this.cameraMode == CameraRigController.CameraMode.PlayerBasic)
				{
					float num16 = this.pitch - num15;
					float num17 = this.yaw + num14;
					num16 += vector9.y;
					num17 += vector9.x;
					this.pitch = Mathf.Clamp(num16, min, max);
					this.yaw = Mathf.Repeat(num17, 360f);
				}
				else if (this.cameraMode == CameraRigController.CameraMode.SpectateUser && this.target)
				{
					if (networkedViewAngles)
					{
						this.SetPitchYaw(networkedViewAngles.viewAngles);
					}
					else
					{
						InputBankTest component2 = this.target.GetComponent<InputBankTest>();
						if (component2)
						{
							this.SetPitchYawFromLookVector(component2.aimDirection);
						}
					}
				}
				this.desiredCameraState.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
				Vector3 direction2 = vector8 + this.desiredCameraState.rotation * idealLocalCameraPos - vector8;
				float num18 = direction2.magnitude;
				float num19 = (1f + this.pitch / -90f) * 0.5f;
				num18 *= Mathf.Sqrt(1f - num19);
				if (num18 < 0.25f)
				{
					num18 = 0.25f;
				}
				float a = this.Raycast(new Ray(vector8, direction2), num18, wallCushion);
				this.currentCameraDistance = Mathf.Min(a, Mathf.SmoothDamp(this.currentCameraDistance, a, ref this.cameraDistanceVelocity, 0.5f));
				this.desiredCameraState.position = vector8 + direction2.normalized * this.currentCameraDistance;
				this.pitch -= vector9.y;
				this.yaw -= vector9.x;
				if (networkedViewAngles && networkedViewAngles.hasEffectiveAuthority)
				{
					networkedViewAngles.viewAngles = new PitchYawPair(this.pitch, this.yaw);
				}
			}
			if (this.targetBody)
			{
				num *= (this.targetBody.isSprinting ? 1.3f : 1f);
			}
			this.desiredCameraState.fov = Mathf.SmoothDamp(this.desiredCameraState.fov, num, ref this.fovVelocity, 0.2f, float.PositiveInfinity, Time.deltaTime);
			if (this.hud)
			{
				CharacterMaster targetMaster = this.targetBody ? this.targetBody.master : null;
				this.hud.targetMaster = targetMaster;
			}
			this.UpdateCrosshair(vector8);
			CameraState cameraState = this.desiredCameraState;
			if (this.overrideCam != null)
			{
				if ((UnityEngine.Object)this.overrideCam)
				{
					cameraState = this.overrideCam.GetCameraState(this);
				}
				this.overrideCam = null;
			}
			if (this.lerpCameraTime >= 1f)
			{
				this.currentCameraState = cameraState;
			}
			else
			{
				this.currentCameraState = CameraState.Lerp(ref this.lerpCameraState, ref cameraState, CameraRigController.RemapLerpTime(this.lerpCameraTime));
			}
			this.SetCameraState(this.currentCameraState);
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0004E724 File Offset: 0x0004C924
		private float Raycast(Ray ray, float maxDistance, float wallCushion)
		{
			RaycastHit[] array = Physics.SphereCastAll(ray, wallCushion, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
			float num = maxDistance;
			for (int i = 0; i < array.Length; i++)
			{
				float distance = array[i].distance;
				if (distance < num)
				{
					Collider collider = array[i].collider;
					if (collider && !collider.GetComponent<NonSolidToCamera>())
					{
						num = distance;
					}
				}
			}
			return num;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0004E79C File Offset: 0x0004C99C
		private static float RemapLerpTime(float t)
		{
			float num = 1f;
			float num2 = 0f;
			float num3 = 1f;
			if ((t /= num / 2f) < 1f)
			{
				return num3 / 2f * t * t + num2;
			}
			return -num3 / 2f * ((t -= 1f) * (t - 2f) - 1f) + num2;
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000C07 RID: 3079 RVA: 0x00009751 File Offset: 0x00007951
		public bool hasOverride
		{
			get
			{
				return this.overrideCam != null;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000C08 RID: 3080 RVA: 0x0000975C File Offset: 0x0000795C
		// (set) Token: 0x06000C09 RID: 3081 RVA: 0x00009764 File Offset: 0x00007964
		public Vector3 rawScreenShakeDisplacement { get; private set; }

		// Token: 0x06000C0A RID: 3082 RVA: 0x0004E800 File Offset: 0x0004CA00
		private void SetCameraState(CameraState cameraState)
		{
			this.currentCameraState = cameraState;
			float d = (this.localUserViewer == null) ? 1f : this.localUserViewer.userProfile.screenShakeScale;
			Vector3 position = cameraState.position;
			this.rawScreenShakeDisplacement = ShakeEmitter.ComputeTotalShakeAtPoint(cameraState.position);
			Vector3 vector = this.rawScreenShakeDisplacement * d;
			Vector3 position2 = position + vector;
			if (vector != Vector3.zero)
			{
				Vector3 origin = position;
				Vector3 direction = vector;
				RaycastHit raycastHit;
				if (Physics.SphereCast(origin, this.sceneCam.nearClipPlane, direction, out raycastHit, vector.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					position2 = position + vector.normalized * raycastHit.distance;
				}
			}
			base.transform.SetPositionAndRotation(position2, cameraState.rotation);
			this.currentFov = cameraState.fov;
			if (this.sceneCam)
			{
				this.sceneCam.fieldOfView = this.currentFov;
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0004E8FC File Offset: 0x0004CAFC
		private void UpdateCrosshair(Vector3 raycastStartPlanePoint)
		{
			this.lastAimAssist = this.aimAssist;
			Vector2 zero = Vector2.zero;
			Ray crosshairRaycastRay = this.GetCrosshairRaycastRay(zero, raycastStartPlanePoint);
			bool flag = false;
			this.lastCrosshairHurtBox = null;
			RaycastHit raycastHit = default(RaycastHit);
			RaycastHit[] array = Physics.RaycastAll(crosshairRaycastRay, this.maxAimRaycastDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
			float num = float.PositiveInfinity;
			int num2 = -1;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit2 = array[i];
				HurtBox component = raycastHit2.collider.GetComponent<HurtBox>();
				float distance = raycastHit2.distance;
				if (distance < num && distance > 3f && (!component || !component.healthComponent || !component.healthComponent.body || component.healthComponent.body.teamComponent.teamIndex != this.targetTeamIndex))
				{
					num = distance;
					num2 = i;
					this.lastCrosshairHurtBox = component;
				}
			}
			if (num2 != -1)
			{
				flag = true;
				raycastHit = array[num2];
			}
			this.aimAssist.aimAssistHurtbox = null;
			if (flag)
			{
				this.crosshairWorldPosition = raycastHit.point;
				float num3 = 1000f;
				if (raycastHit.distance < num3)
				{
					HurtBox component2 = raycastHit.collider.GetComponent<HurtBox>();
					if (component2)
					{
						HealthComponent healthComponent = component2.healthComponent;
						if (healthComponent)
						{
							TeamComponent component3 = healthComponent.GetComponent<TeamComponent>();
							if (component3 && component3.teamIndex != this.targetTeamIndex && component3.teamIndex != TeamIndex.None)
							{
								CharacterBody body = healthComponent.body;
								HurtBox hurtBox = (body != null) ? body.mainHurtBox : null;
								if (hurtBox)
								{
									this.aimAssist.aimAssistHurtbox = hurtBox;
									this.aimAssist.worldPosition = raycastHit.point;
									this.aimAssist.localPositionOnHurtbox = hurtBox.transform.InverseTransformPoint(raycastHit.point);
									return;
								}
							}
						}
					}
				}
			}
			else
			{
				this.crosshairWorldPosition = crosshairRaycastRay.GetPoint(this.maxAimRaycastDistance);
			}
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0004EB28 File Offset: 0x0004CD28
		public static Ray ModifyAimRayIfApplicable(Ray originalAimRay, GameObject target, out float extraRaycastDistance)
		{
			CameraRigController cameraRigController = null;
			foreach (CameraRigController cameraRigController2 in CameraRigController.readOnlyInstancesList)
			{
				if (cameraRigController2.target == target && cameraRigController2._localUserViewer.cachedBodyObject == target)
				{
					cameraRigController = cameraRigController2;
					break;
				}
			}
			if (cameraRigController)
			{
				Camera camera = cameraRigController.sceneCam;
				extraRaycastDistance = (originalAimRay.origin - camera.transform.position).magnitude;
				return camera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
			}
			extraRaycastDistance = 0f;
			return originalAimRay;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0004EBE8 File Offset: 0x0004CDE8
		private Ray GetCrosshairRaycastRay(Vector2 crosshairOffset, Vector3 raycastStartPlanePoint)
		{
			if (!this.sceneCam)
			{
				return default(Ray);
			}
			float fieldOfView = this.sceneCam.fieldOfView;
			float num = fieldOfView * this.sceneCam.aspect;
			Quaternion quaternion = Quaternion.Euler(crosshairOffset.y * fieldOfView, crosshairOffset.x * num, 0f);
			quaternion = this.desiredCameraState.rotation * quaternion;
			return new Ray(Vector3.ProjectOnPlane(this.desiredCameraState.position - raycastStartPlanePoint, this.desiredCameraState.rotation * Vector3.forward) + raycastStartPlanePoint, quaternion * Vector3.forward);
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0000976D File Offset: 0x0000796D
		private void OnEnable()
		{
			CameraRigController.instancesList.Add(this);
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0000977A File Offset: 0x0000797A
		private void OnDisable()
		{
			CameraRigController.instancesList.Remove(this);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0004EC94 File Offset: 0x0004CE94
		private void OnDestroy()
		{
			if (this.uiCam)
			{
				UnityEngine.Object.Destroy(this.uiCam.gameObject);
			}
			if (this.hud)
			{
				UnityEngine.Object.Destroy(this.hud.gameObject);
			}
			LocalUser localUserViewer = this.localUserViewer;
			Player player = (localUserViewer != null) ? localUserViewer.inputPlayer : null;
			if (player != null)
			{
				player.SetVibration(0, 0f);
				player.SetVibration(1, 0f);
			}
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0004ED0C File Offset: 0x0004CF0C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SceneCamera.onSceneCameraPreCull += delegate(SceneCamera sceneCam)
			{
				sceneCam.cameraRigController.sprintingParticleSystem.gameObject.layer = LayerIndex.defaultLayer.intVal;
			};
			SceneCamera.onSceneCameraPostRender += delegate(SceneCamera sceneCam)
			{
				sceneCam.cameraRigController.sprintingParticleSystem.gameObject.layer = LayerIndex.noDraw.intVal;
			};
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0004ED64 File Offset: 0x0004CF64
		public static bool IsObjectSpectatedByAnyCamera(GameObject gameObject)
		{
			for (int i = 0; i < CameraRigController.instancesList.Count; i++)
			{
				if (CameraRigController.instancesList[i].target == gameObject)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000C13 RID: 3091 RVA: 0x0004EDA4 File Offset: 0x0004CFA4
		// (remove) Token: 0x06000C14 RID: 3092 RVA: 0x0004EDD8 File Offset: 0x0004CFD8
		public static event Action<CameraRigController, GameObject> onCameraTargetChanged;

		// Token: 0x04000FD7 RID: 4055
		[Tooltip("The main camera for rendering the scene.")]
		public Camera sceneCam;

		// Token: 0x04000FD8 RID: 4056
		[Tooltip("The UI camera.")]
		public Camera uiCam;

		// Token: 0x04000FD9 RID: 4057
		[Tooltip("The skybox camera.")]
		public Camera skyboxCam;

		// Token: 0x04000FDA RID: 4058
		public ParticleSystem sprintingParticleSystem;

		// Token: 0x04000FDB RID: 4059
		public float baseFov = 60f;

		// Token: 0x04000FDC RID: 4060
		private float currentFov;

		// Token: 0x04000FDD RID: 4061
		private float fovVelocity;

		// Token: 0x04000FDE RID: 4062
		public float fadeStartDistance = 1f;

		// Token: 0x04000FDF RID: 4063
		public float fadeEndDistance = 4f;

		// Token: 0x04000FE0 RID: 4064
		public bool disableSpectating;

		// Token: 0x04000FE1 RID: 4065
		[Tooltip("The maximum distance of the raycast used to determine the aim vector.")]
		public float maxAimRaycastDistance = 1000f;

		// Token: 0x04000FE2 RID: 4066
		private CameraState desiredCameraState;

		// Token: 0x04000FE3 RID: 4067
		private CameraState currentCameraState;

		// Token: 0x04000FE4 RID: 4068
		private CameraState lerpCameraState;

		// Token: 0x04000FE5 RID: 4069
		private float lerpCameraTime = 1f;

		// Token: 0x04000FE6 RID: 4070
		private float lerpCameraTimeScale = 1f;

		// Token: 0x04000FE7 RID: 4071
		private Vector3 cameraStateVelocityPosition;

		// Token: 0x04000FE8 RID: 4072
		private float cameraStateVelocityAngle;

		// Token: 0x04000FE9 RID: 4073
		private float cameraStateVelocityFov;

		// Token: 0x04000FEA RID: 4074
		private ICameraStateProvider overrideCam;

		// Token: 0x04000FEB RID: 4075
		public CameraRigController.CameraMode cameraMode = CameraRigController.CameraMode.PlayerBasic;

		// Token: 0x04000FEC RID: 4076
		private NetworkUser _viewer;

		// Token: 0x04000FED RID: 4077
		private LocalUser _localUserViewer;

		// Token: 0x04000FEE RID: 4078
		public Rect viewport = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04000FEF RID: 4079
		public HUD hud;

		// Token: 0x04000FF0 RID: 4080
		private GameObject previousTarget;

		// Token: 0x04000FF1 RID: 4081
		public GameObject target;

		// Token: 0x04000FF2 RID: 4082
		private CharacterBody targetBody;

		// Token: 0x04000FF4 RID: 4084
		[Tooltip("Whether or not to create a HUD.")]
		public bool createHud = true;

		// Token: 0x04000FF6 RID: 4086
		private CameraTargetParams targetParams;

		// Token: 0x04000FF7 RID: 4087
		private float pitch;

		// Token: 0x04000FF8 RID: 4088
		private float yaw;

		// Token: 0x04000FFA RID: 4090
		private float currentCameraDistance;

		// Token: 0x04000FFB RID: 4091
		private float cameraDistanceVelocity;

		// Token: 0x04000FFC RID: 4092
		private Vector2 aimStickVelocity;

		// Token: 0x04000FFD RID: 4093
		private float stickAimPreviousAcceleratedMagnitude;

		// Token: 0x04000FFE RID: 4094
		public Vector2 aimStickPostSmoothing;

		// Token: 0x04000FFF RID: 4095
		public Vector2 aimStickPostDualZone;

		// Token: 0x04001000 RID: 4096
		public Vector2 aimStickPostExponent;

		// Token: 0x04001002 RID: 4098
		public CameraRigController.AimAssistInfo lastAimAssist;

		// Token: 0x04001003 RID: 4099
		public CameraRigController.AimAssistInfo aimAssist;

		// Token: 0x04001004 RID: 4100
		public HurtBox lastCrosshairHurtBox;

		// Token: 0x04001005 RID: 4101
		private static List<CameraRigController> instancesList = new List<CameraRigController>();

		// Token: 0x04001006 RID: 4102
		public static readonly ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.instancesList.AsReadOnly();

		// Token: 0x04001008 RID: 4104
		private static FloatConVar aimStickExponent = new FloatConVar("aim_stick_exponent", ConVarFlags.None, "1", "The exponent for stick input used for aiming.");

		// Token: 0x04001009 RID: 4105
		private static FloatConVar aimStickDualZoneThreshold = new FloatConVar("aim_stick_dual_zone_threshold", ConVarFlags.None, "0.90", "The threshold for stick dual zone behavior.");

		// Token: 0x0400100A RID: 4106
		private static FloatConVar aimStickDualZoneSlope = new FloatConVar("aim_stick_dual_zone_slope", ConVarFlags.None, "0.40", "The slope value for stick dual zone behavior.");

		// Token: 0x0400100B RID: 4107
		private static FloatConVar aimStickDualZoneSmoothing = new FloatConVar("aim_stick_smoothing", ConVarFlags.None, "0.05", "The smoothing value for stick aiming.");

		// Token: 0x0400100C RID: 4108
		private static FloatConVar aimStickGlobalScale = new FloatConVar("aim_stick_global_scale", ConVarFlags.Archive, "1.00", "The global sensitivity scale for stick aiming.");

		// Token: 0x0400100D RID: 4109
		private static FloatConVar aimStickAssistMinSlowdownScale = new FloatConVar("aim_stick_assist_min_slowdown_scale", ConVarFlags.None, "1", "The MAX amount the sensitivity scales down when passing over an enemy.");

		// Token: 0x0400100E RID: 4110
		private static FloatConVar aimStickAssistMaxSlowdownScale = new FloatConVar("aim_stick_assist_max_slowdown_scale", ConVarFlags.None, "0.4", "The MAX amount the sensitivity scales down when passing over an enemy.");

		// Token: 0x0400100F RID: 4111
		private static FloatConVar aimStickAssistMinDelta = new FloatConVar("aim_stick_assist_min_delta", ConVarFlags.None, "0", "The MIN amount in radians the aim assist will turn towards");

		// Token: 0x04001010 RID: 4112
		private static FloatConVar aimStickAssistMaxDelta = new FloatConVar("aim_stick_assist_max_delta", ConVarFlags.None, "1.57", "The MAX amount in radians the aim assist will turn towards");

		// Token: 0x04001011 RID: 4113
		private static FloatConVar aimStickAssistMaxInputHelp = new FloatConVar("aim_stick_assist_max_input_help", ConVarFlags.None, "0.2", "The amount, from 0-1, that the aim assist will actually ADD magnitude towards. Helps you keep target while strafing. CURRENTLY UNUSED.");

		// Token: 0x04001012 RID: 4114
		public static FloatConVar aimStickAssistMaxSize = new FloatConVar("aim_stick_assist_max_size", ConVarFlags.None, "3", "The size, as a coefficient, of the aim assist 'white' zone.");

		// Token: 0x04001013 RID: 4115
		public static FloatConVar aimStickAssistMinSize = new FloatConVar("aim_stick_assist_min_size", ConVarFlags.None, "1", "The minimum size, as a percentage of the GUI, of the aim assist 'red' zone.");

		// Token: 0x04001014 RID: 4116
		private float hitmarkerAlpha;

		// Token: 0x04001015 RID: 4117
		private float hitmarkerTimer;

		// Token: 0x0200027B RID: 635
		public enum CameraMode
		{
			// Token: 0x04001017 RID: 4119
			None,
			// Token: 0x04001018 RID: 4120
			PlayerBasic,
			// Token: 0x04001019 RID: 4121
			Fly,
			// Token: 0x0400101A RID: 4122
			SpectateOrbit,
			// Token: 0x0400101B RID: 4123
			SpectateUser
		}

		// Token: 0x0200027C RID: 636
		public struct AimAssistInfo
		{
			// Token: 0x0400101C RID: 4124
			public HurtBox aimAssistHurtbox;

			// Token: 0x0400101D RID: 4125
			public Vector3 localPositionOnHurtbox;

			// Token: 0x0400101E RID: 4126
			public Vector3 worldPosition;
		}
	}
}
