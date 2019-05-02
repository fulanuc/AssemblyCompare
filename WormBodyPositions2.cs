using System;
using System.Collections.Generic;
using EntityStates.MagmaWorm;
using RoR2.Projectile;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000430 RID: 1072
	[RequireComponent(typeof(CharacterBody))]
	public class WormBodyPositions2 : NetworkBehaviour, ITeleportHandler, IEventSystemHandler, ILifeBehavior, IPainAnimationHandler
	{
		// Token: 0x06001802 RID: 6146 RVA: 0x0007BF5C File Offset: 0x0007A15C
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.boneTransformationBuffer = new WormBodyPositions2.PositionRotationPair[this.bones.Length + 1];
			this.totalLength = 0f;
			for (int i = 0; i < this.segmentLengths.Length; i++)
			{
				this.totalLength += this.segmentLengths[i];
			}
			if (NetworkServer.active)
			{
				this.travelCallbacks = new List<WormBodyPositions2.TravelCallback>();
			}
			this.boneDisplacements = new Vector3[this.bones.Length];
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00012079 File Offset: 0x00010279
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.PopulateInitialKeyFrames();
				this.previousSurfaceTestEnd = this.chasePosition;
				this.velocity = this.characterDirection.forward;
			}
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x000120A5 File Offset: 0x000102A5
		private void OnDestroy()
		{
			this.travelCallbacks = null;
			Util.PlaySound("Stop_magmaWorm_burrowed_loop", this.bones[0].gameObject);
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x0007BFFC File Offset: 0x0007A1FC
		private void BakeSegmentLengths()
		{
			this.segmentLengths = new float[this.bones.Length];
			Vector3 a = this.bones[0].position;
			for (int i = 1; i < this.bones.Length; i++)
			{
				Vector3 position = this.bones[i].position;
				float magnitude = (a - position).magnitude;
				this.segmentLengths[i - 1] = magnitude;
				a = position;
			}
			this.segmentLengths[this.bones.Length - 1] = 2f;
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0007C080 File Offset: 0x0007A280
		[Server]
		private void PopulateInitialKeyFrames()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::PopulateInitialKeyFrames()' called on client");
				return;
			}
			Vector3 vector = base.transform.position + Vector3.down * this.spawnDepth;
			this.chasePosition = vector;
			for (int i = this.bones.Length - 1; i >= 0; i--)
			{
				this.AttemptToGenerateKeyFrame(vector + Vector3.down * (float)(i + 1) * this.segmentLengths[i]);
			}
			this.AttemptToGenerateKeyFrame(vector);
			this.headDistance = 0f;
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x0007C118 File Offset: 0x0007A318
		private Vector3 EvaluatePositionAlongCurve(float positionDownBody)
		{
			float num = 0f;
			foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
			{
				float b = num;
				num += keyFrame.length;
				if (num >= positionDownBody)
				{
					float t = Mathf.InverseLerp(num, b, positionDownBody);
					CubicBezier3 curve = keyFrame.curve;
					return curve.Evaluate(t);
				}
			}
			if (this.keyFrames.Count > 0)
			{
				return this.keyFrames[this.keyFrames.Count - 1].curve.Evaluate(1f);
			}
			return Vector3.zero;
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0007C1DC File Offset: 0x0007A3DC
		private void UpdateBones()
		{
			float num = this.totalLength;
			this.boneTransformationBuffer[this.boneTransformationBuffer.Length - 1] = new WormBodyPositions2.PositionRotationPair
			{
				position = this.EvaluatePositionAlongCurve(this.headDistance + num),
				rotation = Quaternion.identity
			};
			for (int i = this.boneTransformationBuffer.Length - 2; i >= 0; i--)
			{
				num -= this.segmentLengths[i];
				Vector3 vector = this.EvaluatePositionAlongCurve(this.headDistance + num);
				Quaternion rotation = Util.QuaternionSafeLookRotation(vector - this.boneTransformationBuffer[i + 1].position, Vector3.up);
				this.boneTransformationBuffer[i] = new WormBodyPositions2.PositionRotationPair
				{
					position = vector,
					rotation = rotation
				};
			}
			Vector3 forward = this.bones[0].forward;
			for (int j = 0; j < this.bones.Length; j++)
			{
				this.bones[j].position = this.boneTransformationBuffer[j].position + this.boneDisplacements[j];
				this.bones[j].forward = forward;
				this.bones[j].up = this.boneTransformationBuffer[j].rotation * -Vector3.forward;
				forward = this.bones[j].forward;
			}
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007C358 File Offset: 0x0007A558
		[Server]
		private void AttemptToGenerateKeyFrame(Vector3 position)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::AttemptToGenerateKeyFrame(UnityEngine.Vector3)' called on client");
				return;
			}
			WormBodyPositions2.KeyFrame keyFrame = this.newestKeyFrame;
			CubicBezier3 curve = CubicBezier3.FromVelocities(keyFrame.curve.p1, -keyFrame.curve.v1, position, -this.velocity * (this.keyFrameGenerationInterval * 0.25f));
			float length = curve.ApproximateLength(50);
			WormBodyPositions2.KeyFrame keyFrame2 = new WormBodyPositions2.KeyFrame
			{
				curve = curve,
				length = length,
				time = WormBodyPositions2.GetSynchronizedTimeStamp()
			};
			if (keyFrame2.length >= 0f)
			{
				this.headDistance += keyFrame2.length;
				this.AddKeyFrame(ref keyFrame2);
			}
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x0007C41C File Offset: 0x0007A61C
		private void AddKeyFrame(ref WormBodyPositions2.KeyFrame newKeyFrame)
		{
			this.newestKeyFrame = newKeyFrame;
			this.keyFrames.Insert(0, newKeyFrame);
			this.keyFramesTotalLength += newKeyFrame.length;
			bool flag = false;
			float num = this.keyFramesTotalLength;
			float num2 = this.totalLength + this.headDistance + 4f;
			while (this.keyFrames.Count > 0 && (num -= this.keyFrames[this.keyFrames.Count - 1].length) > num2)
			{
				this.keyFrames.RemoveAt(this.keyFrames.Count - 1);
				flag = true;
			}
			if (flag)
			{
				this.keyFramesTotalLength = 0f;
				foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
				{
					this.keyFramesTotalLength += keyFrame.length;
				}
			}
			if (NetworkServer.active)
			{
				this.CallRpcSendKeyFrame(newKeyFrame);
			}
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x000120C6 File Offset: 0x000102C6
		[ClientRpc]
		private void RpcSendKeyFrame(WormBodyPositions2.KeyFrame newKeyFrame)
		{
			if (!NetworkServer.active)
			{
				this.AddKeyFrame(ref newKeyFrame);
			}
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x000120D7 File Offset: 0x000102D7
		private void Update()
		{
			this.UpdateBoneDisplacements(Time.deltaTime);
			this.UpdateHeadOffset();
			if (this.animateJaws)
			{
				this.UpdateJaws();
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x0007C53C File Offset: 0x0007A73C
		private void UpdateJaws()
		{
			if (this.animator)
			{
				float value = Mathf.Clamp01(Util.Remap((this.bones[0].position - base.transform.position).magnitude, this.jawClosedDistance, this.jawOpenDistance, 0f, 1f));
				this.animator.SetFloat(this.jawMecanimCycleParameter, value, this.jawMecanimDampTime, Time.deltaTime);
			}
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x0007C5BC File Offset: 0x0007A7BC
		private void UpdateHeadOffset()
		{
			float num = this.headDistance;
			int num2 = this.keyFrames.Count - 1;
			float num3 = 0f;
			float num4 = WormBodyPositions2.GetSynchronizedTimeStamp() - this.followDelay;
			for (int i = 0; i < num2; i++)
			{
				float time = this.keyFrames[i + 1].time;
				float length = this.keyFrames[i].length;
				if (time < num4)
				{
					num = num3 + length * Mathf.InverseLerp(this.keyFrames[i].time, time, num4);
					break;
				}
				num3 += length;
			}
			this.OnTravel(this.headDistance - num);
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x000120F8 File Offset: 0x000102F8
		private void OnTravel(float distance)
		{
			this.headDistance -= distance;
			this.UpdateBones();
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0007C664 File Offset: 0x0007A864
		[Server]
		private void SurfaceTest()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::SurfaceTest()' called on client");
				return;
			}
			Vector3 vector = this.chasePosition;
			Vector3 vector2 = vector - this.previousSurfaceTestEnd;
			float magnitude = vector2.magnitude;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.previousSurfaceTestEnd, vector2, out raycastHit, magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				if (!this.entranceCollider)
				{
					this.OnChaserEnterSurface(raycastHit.point, raycastHit.normal);
				}
				this.entranceCollider = raycastHit.collider;
			}
			else
			{
				this.entranceCollider = null;
			}
			if (Physics.Raycast(vector, -vector2, out raycastHit, magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				if (!this.exitCollider)
				{
					this.OnChaserExitSurface(raycastHit.point, raycastHit.normal);
				}
				this.exitCollider = raycastHit.collider;
			}
			else
			{
				this.exitCollider = null;
			}
			this.previousSurfaceTestEnd = vector;
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x0007C764 File Offset: 0x0007A964
		private void AddTravelCallback(WormBodyPositions2.TravelCallback newTravelCallback)
		{
			int index = this.travelCallbacks.Count;
			float time = newTravelCallback.time;
			for (int i = 0; i < this.travelCallbacks.Count; i++)
			{
				if (time < this.travelCallbacks[i].time)
				{
					index = i;
					break;
				}
			}
			this.travelCallbacks.Insert(index, newTravelCallback);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x0007C7C0 File Offset: 0x0007A9C0
		[Server]
		private void OnChaserEnterSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnChaserEnterSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay,
				callback = delegate()
				{
					this.OnEnterSurface(point, surfaceNormal);
				}
			});
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay - 0.5f,
				callback = new Action(this.RpcPlaySurfaceImpactSound)
			});
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x0007C870 File Offset: 0x0007AA70
		[Server]
		private void OnChaserExitSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnChaserExitSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.warningEffectPrefab)
			{
				EffectManager.instance.SpawnEffect(this.warningEffectPrefab, new EffectData
				{
					origin = point,
					rotation = Util.QuaternionSafeLookRotation(surfaceNormal)
				}, true);
			}
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay,
				callback = delegate()
				{
					this.OnExitSurface(point, surfaceNormal);
				}
			});
			this.AddTravelCallback(new WormBodyPositions2.TravelCallback
			{
				time = WormBodyPositions2.GetSynchronizedTimeStamp() + this.followDelay - 0.5f,
				callback = new Action(this.RpcPlaySurfaceImpactSound)
			});
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x0001210E File Offset: 0x0001030E
		[ClientRpc]
		private void RpcPlaySurfaceImpactSound()
		{
			Util.PlaySound("Play_magmaWorm_M1", this.bones[0].gameObject);
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x0007C960 File Offset: 0x0007AB60
		[Server]
		private void OnEnterSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnEnterSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.enterTriggerCooldownTimer > 0f)
			{
				return;
			}
			if (this.shouldTriggerDeathEffectOnNextImpact && Run.instance.fixedTime - this.deathTime >= DeathState.duration - 3f)
			{
				this.shouldTriggerDeathEffectOnNextImpact = false;
				return;
			}
			this.enterTriggerCooldownTimer = this.impactCooldownDuration;
			EffectManager.instance.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			if (this.shouldFireMeatballsOnImpact)
			{
				this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
			}
			if (this.shouldFireBlastAttackOnImpact)
			{
				this.FireImpactBlastAttack(point + surfaceNormal);
			}
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x00012128 File Offset: 0x00010328
		public void OnDeathStart()
		{
			this.deathTime = Run.instance.fixedTime;
			this.shouldTriggerDeathEffectOnNextImpact = true;
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x0007CA50 File Offset: 0x0007AC50
		[Server]
		private void PerformDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::PerformDeath()' called on client");
				return;
			}
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i])
				{
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MagmaWormDeathDust"), new EffectData
					{
						origin = this.bones[i].position,
						rotation = UnityEngine.Random.rotation,
						scale = 1f
					}, true);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x0007CAE4 File Offset: 0x0007ACE4
		[Server]
		private void OnExitSurface(Vector3 point, Vector3 surfaceNormal)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.WormBodyPositions2::OnExitSurface(UnityEngine.Vector3,UnityEngine.Vector3)' called on client");
				return;
			}
			if (this.exitTriggerCooldownTimer > 0f)
			{
				return;
			}
			this.exitTriggerCooldownTimer = this.impactCooldownDuration;
			EffectManager.instance.SpawnEffect(this.burrowEffectPrefab, new EffectData
			{
				origin = point,
				rotation = Util.QuaternionSafeLookRotation(surfaceNormal),
				scale = 1f
			}, true);
			if (this.shouldFireMeatballsOnImpact)
			{
				this.FireMeatballs(surfaceNormal, point + surfaceNormal * 3f, this.characterDirection.forward, this.meatballCount, this.meatballAngle, this.meatballForce);
			}
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0007CB94 File Offset: 0x0007AD94
		private void FireMeatballs(Vector3 impactNormal, Vector3 impactPosition, Vector3 forward, int meatballCount, float meatballAngle, float meatballForce)
		{
			float num = 360f / (float)meatballCount;
			Vector3 normalized = Vector3.ProjectOnPlane(forward, impactNormal).normalized;
			Vector3 point = Vector3.RotateTowards(impactNormal, normalized, meatballAngle * 0.0174532924f, float.PositiveInfinity);
			for (int i = 0; i < meatballCount; i++)
			{
				Vector3 forward2 = Quaternion.AngleAxis(num * (float)i, impactNormal) * point;
				ProjectileManager.instance.FireProjectile(this.meatballProjectile, impactPosition, Util.QuaternionSafeLookRotation(forward2), base.gameObject, this.characterBody.damage * this.meatballDamageCoefficient, meatballForce, Util.CheckRoll(this.characterBody.crit, this.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0007CC48 File Offset: 0x0007AE48
		private void FireImpactBlastAttack(Vector3 impactPosition)
		{
			BlastAttack blastAttack = new BlastAttack();
			blastAttack.baseDamage = this.characterBody.damage * this.blastAttackDamageCoefficient;
			blastAttack.procCoefficient = this.blastAttackProcCoefficient;
			blastAttack.baseForce = this.blastAttackForce;
			blastAttack.bonusForce = Vector3.up * this.blastAttackBonusVerticalForce;
			blastAttack.crit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
			blastAttack.radius = this.blastAttackRadius;
			blastAttack.damageType = DamageType.PercentIgniteOnHit;
			blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
			blastAttack.attacker = base.gameObject;
			blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
			blastAttack.position = impactPosition;
			blastAttack.Fire();
			if (NetworkServer.active)
			{
				EffectManager.instance.SpawnEffect(this.blastAttackEffect, new EffectData
				{
					origin = impactPosition,
					scale = this.blastAttackRadius
				}, true);
			}
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x0007CD38 File Offset: 0x0007AF38
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.enterTriggerCooldownTimer -= Time.fixedDeltaTime;
				this.exitTriggerCooldownTimer -= Time.fixedDeltaTime;
				Vector3 position = this.referenceTransform.position;
				float d = this.characterBody.moveSpeed * this.speedMultiplier;
				Vector3 normalized = (position - this.chasePosition).normalized;
				float num = (this.underground ? this.maxTurnSpeed : (this.maxTurnSpeed * this.turnRateCoefficientAboveGround)) * 0.0174532924f;
				Vector3 vector = new Vector3(this.velocity.x, 0f, this.velocity.z);
				Vector3 a = new Vector3(normalized.x, 0f, normalized.z);
				vector = Vector3.RotateTowards(vector, a * d, num * Time.fixedDeltaTime, float.PositiveInfinity);
				vector = vector.normalized * d;
				float num2 = position.y - this.chasePosition.y;
				float num3 = -this.velocity.y * this.yDamperConstant;
				float num4 = num2 * this.ySpringConstant;
				if (this.allowShoving && Mathf.Abs(this.velocity.y) < this.yShoveVelocityThreshold && num2 > this.yShovePositionThreshold)
				{
					this.velocity.y = this.velocity.y + this.yShoveForce * Time.fixedDeltaTime;
				}
				if (!this.underground)
				{
					num4 *= this.wormForceCoefficientAboveGround;
					num3 *= this.wormForceCoefficientAboveGround;
				}
				this.velocity.y = this.velocity.y + (num4 + num3) * Time.fixedDeltaTime;
				this.velocity += Physics.gravity * Time.fixedDeltaTime;
				this.velocity = new Vector3(vector.x, this.velocity.y, vector.z);
				this.chasePosition += this.velocity * Time.fixedDeltaTime;
				this.chasePositionVisualizer.position = this.chasePosition;
				this.underground = (-num2 < this.undergroundTestYOffset);
				this.keyFrameGenerationTimer -= Time.deltaTime;
				if (this.keyFrameGenerationTimer <= 0f)
				{
					this.keyFrameGenerationTimer = this.keyFrameGenerationInterval;
					this.AttemptToGenerateKeyFrame(this.chasePosition);
				}
				this.SurfaceTest();
				float synchronizedTimeStamp = WormBodyPositions2.GetSynchronizedTimeStamp();
				while (this.travelCallbacks.Count > 0 && this.travelCallbacks[0].time <= synchronizedTimeStamp)
				{
					ref WormBodyPositions2.TravelCallback ptr = this.travelCallbacks[0];
					this.travelCallbacks.RemoveAt(0);
					ptr.callback();
				}
			}
			bool flag = this.bones[0].transform.position.y - base.transform.position.y < this.undergroundTestYOffset;
			if (flag != this.playingBurrowSound)
			{
				if (flag)
				{
					Util.PlaySound("Play_magmaWorm_burrowed_loop", this.bones[0].gameObject);
				}
				else
				{
					Util.PlaySound("Stop_magmaWorm_burrowed_loop", this.bones[0].gameObject);
				}
				this.playingBurrowSound = flag;
			}
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x0007D06C File Offset: 0x0007B26C
		private void DrawKeyFrame(WormBodyPositions2.KeyFrame keyFrame)
		{
			Gizmos.color = Color.Lerp(Color.green, Color.black, 0.5f);
			Gizmos.DrawRay(keyFrame.curve.p0, keyFrame.curve.v0);
			Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f);
			Gizmos.DrawRay(keyFrame.curve.p1, keyFrame.curve.v1);
			for (int i = 1; i <= 20; i++)
			{
				float num = (float)i * 0.05f;
				Gizmos.color = Color.Lerp(Color.red, Color.green, num);
				Vector3 vector = keyFrame.curve.Evaluate(num - 0.05f);
				Vector3 a = keyFrame.curve.Evaluate(num);
				Gizmos.DrawRay(vector, a - vector);
			}
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0007D140 File Offset: 0x0007B340
		private void OnDrawGizmos()
		{
			foreach (WormBodyPositions2.KeyFrame keyFrame in this.keyFrames)
			{
				this.DrawKeyFrame(keyFrame);
			}
			for (int i = 0; i < this.boneTransformationBuffer.Length; i++)
			{
				Gizmos.matrix = Matrix4x4.TRS(this.boneTransformationBuffer[i].position, this.boneTransformationBuffer[i].rotation, Vector3.one * 3f);
				Gizmos.DrawRay(-Vector3.forward, Vector3.forward * 2f);
				Gizmos.DrawRay(-Vector3.right, Vector3.right * 2f);
				Gizmos.DrawRay(-Vector3.up, Vector3.up * 2f);
			}
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x0007D244 File Offset: 0x0007B444
		public void OnTeleport(Vector3 oldPosition, Vector3 newPosition)
		{
			Vector3 b = newPosition - oldPosition;
			for (int i = 0; i < this.keyFrames.Count; i++)
			{
				WormBodyPositions2.KeyFrame keyFrame = this.keyFrames[i];
				CubicBezier3 curve = keyFrame.curve;
				curve.a += b;
				curve.b += b;
				curve.c += b;
				curve.d += b;
				keyFrame.curve = curve;
				this.keyFrames[i] = keyFrame;
			}
			this.chasePosition += b;
			this.previousSurfaceTestEnd += b;
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x0007D324 File Offset: 0x0007B524
		private int FindNearestBone(Vector3 worldPosition)
		{
			int result = -1;
			float num = float.PositiveInfinity;
			for (int i = 0; i < this.bones.Length; i++)
			{
				float sqrMagnitude = (this.bones[i].transform.position - worldPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x0007D378 File Offset: 0x0007B578
		private void UpdateBoneDisplacements(float deltaTime)
		{
			int i = 0;
			int num = this.boneDisplacements.Length;
			while (i < num)
			{
				this.boneDisplacements[i] = Vector3.MoveTowards(this.boneDisplacements[i], Vector3.zero, this.painDisplacementRecoverySpeed * deltaTime);
				i++;
			}
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0007D3C4 File Offset: 0x0007B5C4
		void IPainAnimationHandler.HandlePain(float damage, Vector3 damagePosition)
		{
			int num = this.FindNearestBone(damagePosition);
			if (num != -1)
			{
				this.boneDisplacements[num] = UnityEngine.Random.onUnitSphere * this.maxPainDisplacementMagnitude;
			}
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x00012141 File Offset: 0x00010341
		private static float GetSynchronizedTimeStamp()
		{
			return Run.instance.time;
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x0007D3FC File Offset: 0x0007B5FC
		private static void WriteKeyFrame(NetworkWriter writer, WormBodyPositions2.KeyFrame keyFrame)
		{
			writer.Write(keyFrame.curve.a);
			writer.Write(keyFrame.curve.b);
			writer.Write(keyFrame.curve.c);
			writer.Write(keyFrame.curve.d);
			writer.Write(keyFrame.length);
			writer.Write(keyFrame.time);
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x0007D468 File Offset: 0x0007B668
		private static WormBodyPositions2.KeyFrame ReadKeyFrame(NetworkReader reader)
		{
			WormBodyPositions2.KeyFrame result = default(WormBodyPositions2.KeyFrame);
			result.curve.a = reader.ReadVector3();
			result.curve.b = reader.ReadVector3();
			result.curve.c = reader.ReadVector3();
			result.curve.d = reader.ReadVector3();
			result.length = reader.ReadSingle();
			result.time = reader.ReadSingle();
			return result;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0007D4E0 File Offset: 0x0007B6E0
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint syncVarDirtyBits = base.syncVarDirtyBits;
			if (initialState)
			{
				writer.Write((ushort)this.keyFrames.Count);
				for (int i = 0; i < this.keyFrames.Count; i++)
				{
					WormBodyPositions2.WriteKeyFrame(writer, this.keyFrames[i]);
				}
			}
			return !initialState && syncVarDirtyBits > 0u;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x0007D53C File Offset: 0x0007B73C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.keyFrames.Clear();
				int num = (int)reader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					this.keyFrames.Add(WormBodyPositions2.ReadKeyFrame(reader));
				}
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007D62C File Offset: 0x0007B82C
		static WormBodyPositions2()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcSendKeyFrame, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcSendKeyFrame));
			WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound = 2010133795;
			NetworkBehaviour.RegisterRpcDelegate(typeof(WormBodyPositions2), WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound, new NetworkBehaviour.CmdDelegate(WormBodyPositions2.InvokeRpcRpcPlaySurfaceImpactSound));
			NetworkCRC.RegisterBehaviour("WormBodyPositions2", 0);
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0001214D File Offset: 0x0001034D
		protected static void InvokeRpcRpcSendKeyFrame(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSendKeyFrame called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcSendKeyFrame(GeneratedNetworkCode._ReadKeyFrame_WormBodyPositions2(reader));
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x00012176 File Offset: 0x00010376
		protected static void InvokeRpcRpcPlaySurfaceImpactSound(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcPlaySurfaceImpactSound called on server.");
				return;
			}
			((WormBodyPositions2)obj).RpcPlaySurfaceImpactSound();
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x0007D6B8 File Offset: 0x0007B8B8
		public void CallRpcSendKeyFrame(WormBodyPositions2.KeyFrame newKeyFrame)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSendKeyFrame called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)WormBodyPositions2.kRpcRpcSendKeyFrame);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteKeyFrame_WormBodyPositions2(networkWriter, newKeyFrame);
			this.SendRPCInternal(networkWriter, 0, "RpcSendKeyFrame");
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007D72C File Offset: 0x0007B92C
		public void CallRpcPlaySurfaceImpactSound()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcPlaySurfaceImpactSound called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)WormBodyPositions2.kRpcRpcPlaySurfaceImpactSound);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcPlaySurfaceImpactSound");
		}

		// Token: 0x04001B09 RID: 6921
		private Vector3 velocity;

		// Token: 0x04001B0A RID: 6922
		private Vector3 chasePosition;

		// Token: 0x04001B0B RID: 6923
		public Transform referenceTransform;

		// Token: 0x04001B0C RID: 6924
		public Transform[] bones;

		// Token: 0x04001B0D RID: 6925
		private WormBodyPositions2.PositionRotationPair[] boneTransformationBuffer;

		// Token: 0x04001B0E RID: 6926
		private Vector3[] boneDisplacements;

		// Token: 0x04001B0F RID: 6927
		public float[] segmentLengths;

		// Token: 0x04001B10 RID: 6928
		private float headDistance;

		// Token: 0x04001B11 RID: 6929
		[Tooltip("How far behind the chaser the head is, in seconds.")]
		public float followDelay = 2f;

		// Token: 0x04001B12 RID: 6930
		[Tooltip("Whether or not the jaw will close/open.")]
		public bool animateJaws;

		// Token: 0x04001B13 RID: 6931
		public Animator animator;

		// Token: 0x04001B14 RID: 6932
		public string jawMecanimCycleParameter;

		// Token: 0x04001B15 RID: 6933
		public float jawMecanimDampTime;

		// Token: 0x04001B16 RID: 6934
		public float jawClosedDistance;

		// Token: 0x04001B17 RID: 6935
		public float jawOpenDistance;

		// Token: 0x04001B18 RID: 6936
		public GameObject warningEffectPrefab;

		// Token: 0x04001B19 RID: 6937
		public GameObject burrowEffectPrefab;

		// Token: 0x04001B1A RID: 6938
		public float maxPainDisplacementMagnitude = 2f;

		// Token: 0x04001B1B RID: 6939
		public float painDisplacementRecoverySpeed = 8f;

		// Token: 0x04001B1C RID: 6940
		public bool shouldFireMeatballsOnImpact = true;

		// Token: 0x04001B1D RID: 6941
		public bool shouldFireBlastAttackOnImpact = true;

		// Token: 0x04001B1E RID: 6942
		private float totalLength;

		// Token: 0x04001B1F RID: 6943
		private const float endBonusLength = 4f;

		// Token: 0x04001B20 RID: 6944
		private const float fakeEndSegmentLength = 2f;

		// Token: 0x04001B21 RID: 6945
		private CharacterBody characterBody;

		// Token: 0x04001B22 RID: 6946
		private CharacterDirection characterDirection;

		// Token: 0x04001B23 RID: 6947
		private ModelLocator modelLocator;

		// Token: 0x04001B24 RID: 6948
		private readonly List<WormBodyPositions2.KeyFrame> keyFrames = new List<WormBodyPositions2.KeyFrame>();

		// Token: 0x04001B25 RID: 6949
		private float keyFramesTotalLength;

		// Token: 0x04001B26 RID: 6950
		private WormBodyPositions2.KeyFrame newestKeyFrame;

		// Token: 0x04001B27 RID: 6951
		public float spawnDepth = 30f;

		// Token: 0x04001B28 RID: 6952
		private static readonly Quaternion boneFixRotation = Quaternion.Euler(-90f, 0f, 0f);

		// Token: 0x04001B29 RID: 6953
		public float keyFrameGenerationInterval = 0.25f;

		// Token: 0x04001B2A RID: 6954
		private float keyFrameGenerationTimer;

		// Token: 0x04001B2B RID: 6955
		public bool underground;

		// Token: 0x04001B2C RID: 6956
		private Collider entranceCollider;

		// Token: 0x04001B2D RID: 6957
		private Collider exitCollider;

		// Token: 0x04001B2E RID: 6958
		private Vector3 previousSurfaceTestEnd;

		// Token: 0x04001B2F RID: 6959
		private List<WormBodyPositions2.TravelCallback> travelCallbacks;

		// Token: 0x04001B30 RID: 6960
		private const float impactSoundPrestartDuration = 0.5f;

		// Token: 0x04001B31 RID: 6961
		public float impactCooldownDuration = 0.1f;

		// Token: 0x04001B32 RID: 6962
		private float enterTriggerCooldownTimer;

		// Token: 0x04001B33 RID: 6963
		private float exitTriggerCooldownTimer;

		// Token: 0x04001B34 RID: 6964
		private bool shouldTriggerDeathEffectOnNextImpact;

		// Token: 0x04001B35 RID: 6965
		private float deathTime = float.NegativeInfinity;

		// Token: 0x04001B36 RID: 6966
		public GameObject meatballProjectile;

		// Token: 0x04001B37 RID: 6967
		public GameObject blastAttackEffect;

		// Token: 0x04001B38 RID: 6968
		public int meatballCount;

		// Token: 0x04001B39 RID: 6969
		public float meatballAngle;

		// Token: 0x04001B3A RID: 6970
		public float meatballDamageCoefficient;

		// Token: 0x04001B3B RID: 6971
		public float meatballProcCoefficient;

		// Token: 0x04001B3C RID: 6972
		public float meatballForce;

		// Token: 0x04001B3D RID: 6973
		public float blastAttackDamageCoefficient;

		// Token: 0x04001B3E RID: 6974
		public float blastAttackProcCoefficient;

		// Token: 0x04001B3F RID: 6975
		public float blastAttackRadius;

		// Token: 0x04001B40 RID: 6976
		public float blastAttackForce;

		// Token: 0x04001B41 RID: 6977
		public float blastAttackBonusVerticalForce;

		// Token: 0x04001B42 RID: 6978
		public Transform chasePositionVisualizer;

		// Token: 0x04001B43 RID: 6979
		public float maxTurnSpeed = 180f;

		// Token: 0x04001B44 RID: 6980
		public float speedMultiplier = 2f;

		// Token: 0x04001B45 RID: 6981
		public float verticalTurnSquashFactor = 2f;

		// Token: 0x04001B46 RID: 6982
		public float ySpringConstant = 100f;

		// Token: 0x04001B47 RID: 6983
		public float yDamperConstant = 1f;

		// Token: 0x04001B48 RID: 6984
		public bool allowShoving;

		// Token: 0x04001B49 RID: 6985
		public float yShoveVelocityThreshold;

		// Token: 0x04001B4A RID: 6986
		public float yShovePositionThreshold;

		// Token: 0x04001B4B RID: 6987
		public float yShoveForce;

		// Token: 0x04001B4C RID: 6988
		public float turnRateCoefficientAboveGround;

		// Token: 0x04001B4D RID: 6989
		public float wormForceCoefficientAboveGround;

		// Token: 0x04001B4E RID: 6990
		public float undergroundTestYOffset;

		// Token: 0x04001B4F RID: 6991
		private bool playingBurrowSound;

		// Token: 0x04001B50 RID: 6992
		private static int kRpcRpcSendKeyFrame = 874152969;

		// Token: 0x04001B51 RID: 6993
		private static int kRpcRpcPlaySurfaceImpactSound;

		// Token: 0x02000431 RID: 1073
		private struct PositionRotationPair
		{
			// Token: 0x04001B52 RID: 6994
			public Vector3 position;

			// Token: 0x04001B53 RID: 6995
			public Quaternion rotation;
		}

		// Token: 0x02000432 RID: 1074
		[Serializable]
		private struct KeyFrame
		{
			// Token: 0x04001B54 RID: 6996
			public CubicBezier3 curve;

			// Token: 0x04001B55 RID: 6997
			public float length;

			// Token: 0x04001B56 RID: 6998
			public float time;
		}

		// Token: 0x02000433 RID: 1075
		private struct TravelCallback
		{
			// Token: 0x04001B57 RID: 6999
			public float time;

			// Token: 0x04001B58 RID: 7000
			public Action callback;
		}
	}
}
