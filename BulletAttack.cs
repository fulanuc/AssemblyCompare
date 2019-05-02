using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200020D RID: 525
	public class BulletAttack
	{
		// Token: 0x06000A38 RID: 2616 RVA: 0x000474C8 File Offset: 0x000456C8
		public BulletAttack()
		{
			this.filterCallback = new BulletAttack.FilterCallback(this.DefaultFilterCallback);
			this.hitCallback = new BulletAttack.HitCallback(this.DefaultHitCallback);
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x000083D0 File Offset: 0x000065D0
		// (set) Token: 0x06000A3A RID: 2618 RVA: 0x000083D8 File Offset: 0x000065D8
		public Vector3 aimVector
		{
			get
			{
				return this._aimVector;
			}
			set
			{
				this._aimVector = value;
				this._aimVector.Normalize();
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x000083EC File Offset: 0x000065EC
		// (set) Token: 0x06000A3C RID: 2620 RVA: 0x000083F4 File Offset: 0x000065F4
		public float maxDistance
		{
			get
			{
				return this._maxDistance;
			}
			set
			{
				if (!float.IsInfinity(value) && !float.IsNaN(value))
				{
					this._maxDistance = value;
					return;
				}
				Debug.LogFormat("BulletAttack.maxDistance was assigned a value other than a finite number. value={0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00047580 File Offset: 0x00045780
		public bool DefaultHitCallback(ref BulletAttack.BulletHit hitInfo)
		{
			bool result = false;
			if (hitInfo.collider)
			{
				result = ((1 << hitInfo.collider.gameObject.layer & this.stopperMask) == 0);
			}
			if (this.hitEffectPrefab)
			{
				EffectManager.instance.SimpleImpactEffect(this.hitEffectPrefab, hitInfo.point, this.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
			}
			if (hitInfo.collider)
			{
				SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitInfo.collider, hitInfo.point);
				if (objectSurfaceDef)
				{
					if (objectSurfaceDef.impactEffectPrefab)
					{
						EffectManager.instance.SimpleImpactEffect(objectSurfaceDef.impactEffectPrefab, hitInfo.point, hitInfo.surfaceNormal, objectSurfaceDef.approximateColor, true);
					}
					if (objectSurfaceDef.impactSoundString != null && objectSurfaceDef.impactSoundString.Length != 0)
					{
						Util.PlaySound(objectSurfaceDef.impactSoundString, hitInfo.collider.gameObject);
					}
				}
			}
			if (this.isCrit)
			{
				EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Critspark"), hitInfo.point, this.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
			}
			GameObject entityObject = hitInfo.entityObject;
			if (entityObject)
			{
				float num = 1f;
				switch (this.falloffModel)
				{
				case BulletAttack.FalloffModel.None:
					num = 1f;
					break;
				case BulletAttack.FalloffModel.DefaultBullet:
					num = 0.5f + Mathf.Clamp01(Mathf.InverseLerp(60f, 25f, hitInfo.distance)) * 0.5f;
					break;
				case BulletAttack.FalloffModel.Buckshot:
					num = 0.25f + Mathf.Clamp01(Mathf.InverseLerp(25f, 7f, hitInfo.distance)) * 0.75f;
					break;
				}
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = this.damage * num;
				damageInfo.crit = this.isCrit;
				damageInfo.attacker = this.owner;
				damageInfo.inflictor = this.weapon;
				damageInfo.position = hitInfo.point;
				damageInfo.force = hitInfo.direction * (this.force * num);
				damageInfo.procChainMask = this.procChainMask;
				damageInfo.procCoefficient = this.procCoefficient;
				damageInfo.damageType = this.damageType;
				damageInfo.damageColorIndex = this.damageColorIndex;
				damageInfo.ModifyDamageInfo(hitInfo.damageModifier);
				TeamIndex teamIndex = TeamIndex.Neutral;
				if (this.owner)
				{
					TeamComponent component = this.owner.GetComponent<TeamComponent>();
					if (component)
					{
						teamIndex = component.teamIndex;
					}
				}
				TeamIndex teamIndex2 = TeamIndex.Neutral;
				TeamComponent component2 = hitInfo.entityObject.GetComponent<TeamComponent>();
				if (component2)
				{
					teamIndex2 = component2.teamIndex;
				}
				bool flag = teamIndex == teamIndex2;
				HealthComponent healthComponent = null;
				if (!flag)
				{
					healthComponent = entityObject.GetComponent<HealthComponent>();
				}
				if (NetworkServer.active)
				{
					if (healthComponent)
					{
						healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, hitInfo.entityObject);
					}
					GlobalEventManager.instance.OnHitAll(damageInfo, hitInfo.entityObject);
				}
				else if (ClientScene.ready)
				{
					BulletAttack.messageWriter.StartMessage(53);
					BulletAttack.messageWriter.Write(entityObject);
					BulletAttack.messageWriter.Write(damageInfo);
					BulletAttack.messageWriter.Write(healthComponent != null);
					BulletAttack.messageWriter.FinishMessage();
					ClientScene.readyConnection.SendWriter(BulletAttack.messageWriter, QosChannelIndex.defaultReliable.intVal);
				}
			}
			return result;
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x000478F8 File Offset: 0x00045AF8
		public bool DefaultFilterCallback(ref BulletAttack.BulletHit hitInfo)
		{
			HurtBox component = hitInfo.collider.GetComponent<HurtBox>();
			return (!component || !component.healthComponent || !(component.healthComponent.gameObject == this.weapon)) && hitInfo.entityObject != this.weapon;
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00047954 File Offset: 0x00045B54
		private void InitBulletHitFromOriginHit(ref BulletAttack.BulletHit bulletHit, Vector3 direction, Collider hitCollider)
		{
			bulletHit.direction = direction;
			bulletHit.point = this.origin;
			bulletHit.surfaceNormal = -direction;
			bulletHit.distance = 0f;
			bulletHit.collider = hitCollider;
			HurtBox component = bulletHit.collider.GetComponent<HurtBox>();
			bulletHit.entityObject = ((component && component.healthComponent) ? component.healthComponent.gameObject : bulletHit.collider.gameObject);
			bulletHit.damageModifier = (component ? component.damageModifier : HurtBox.DamageModifier.Normal);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x000479E8 File Offset: 0x00045BE8
		private void InitBulletHitFromRaycastHit(ref BulletAttack.BulletHit bulletHit, Vector3 origin, Vector3 direction, ref RaycastHit raycastHit)
		{
			bulletHit.direction = direction;
			bulletHit.point = raycastHit.point;
			bulletHit.surfaceNormal = raycastHit.normal;
			bulletHit.distance = raycastHit.distance;
			bulletHit.collider = raycastHit.collider;
			bulletHit.point = ((bulletHit.distance == 0f) ? origin : raycastHit.point);
			HurtBox component = bulletHit.collider.GetComponent<HurtBox>();
			bulletHit.entityObject = ((component && component.healthComponent) ? component.healthComponent.gameObject : bulletHit.collider.gameObject);
			bulletHit.damageModifier = (component ? component.damageModifier : HurtBox.DamageModifier.Normal);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00008427 File Offset: 0x00006627
		private bool ProcessHit(ref BulletAttack.BulletHit hitInfo)
		{
			if (this.sniper && hitInfo.damageModifier == HurtBox.DamageModifier.SniperTarget)
			{
				hitInfo.damageModifier = HurtBox.DamageModifier.Weak;
			}
			return !this.filterCallback(ref hitInfo) || this.hitCallback(ref hitInfo);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00047AA4 File Offset: 0x00045CA4
		private GameObject ProcessHitList(List<BulletAttack.BulletHit> hits, ref Vector3 endPosition, List<GameObject> ignoreList)
		{
			int count = hits.Count;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
			for (int j = 0; j < count; j++)
			{
				float num = this.maxDistance;
				int num2 = j;
				for (int k = j; k < count; k++)
				{
					int index = array[k];
					if (hits[index].distance < num)
					{
						num = hits[index].distance;
						num2 = k;
					}
				}
				GameObject entityObject = hits[array[num2]].entityObject;
				if (!ignoreList.Contains(entityObject))
				{
					ignoreList.Add(entityObject);
					BulletAttack.BulletHit bulletHit = hits[array[num2]];
					if (!this.ProcessHit(ref bulletHit))
					{
						endPosition = hits[array[num2]].point;
						return entityObject;
					}
				}
				array[num2] = array[j];
			}
			return null;
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x00047B80 File Offset: 0x00045D80
		private static GameObject LookUpColliderEntityObject(Collider collider)
		{
			HurtBox component = collider.GetComponent<HurtBox>();
			if (!component || !component.healthComponent)
			{
				return collider.gameObject;
			}
			return component.healthComponent.gameObject;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0000845D File Offset: 0x0000665D
		private static Collider[] PhysicsOverlapPoint(Vector3 point, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
			return Physics.OverlapBox(point, Vector3.zero, Quaternion.identity, layerMask, queryTriggerInteraction);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00047BBC File Offset: 0x00045DBC
		public void Fire()
		{
			Vector3[] array = new Vector3[this.bulletCount];
			Vector3 up = Vector3.up;
			Vector3 axis = Vector3.Cross(up, this.aimVector);
			int num = 0;
			while ((long)num < (long)((ulong)this.bulletCount))
			{
				float x = UnityEngine.Random.Range(this.minSpread, this.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f) * this.spreadYawScale;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f * this.spreadPitchScale;
				array[num] = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimVector);
				num++;
			}
			int muzzleIndex = -1;
			Vector3 vector2 = this.origin;
			if (!this.weapon)
			{
				this.weapon = this.owner;
			}
			if (this.weapon)
			{
				ModelLocator component = this.weapon.GetComponent<ModelLocator>();
				if (component && component.modelTransform)
				{
					ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
					if (component2)
					{
						muzzleIndex = component2.FindChildIndex(this.muzzleName);
					}
				}
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.bulletCount))
			{
				this.FireSingle(array[num2], muzzleIndex);
				num2++;
			}
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00047D78 File Offset: 0x00045F78
		private void FireSingle(Vector3 normal, int muzzleIndex)
		{
			float num = this.maxDistance;
			Vector3 vector = this.origin + normal * this.maxDistance;
			List<BulletAttack.BulletHit> list = new List<BulletAttack.BulletHit>();
			bool flag = this.radius == 0f || this.smartCollision;
			bool flag2 = this.radius != 0f;
			HashSet<GameObject> hashSet = null;
			if (this.smartCollision)
			{
				hashSet = new HashSet<GameObject>();
			}
			if (flag)
			{
				LayerMask mask = this.hitMask;
				mask &= ~LayerIndex.world.mask;
				RaycastHit[] array = Physics.RaycastAll(this.origin, normal, num, mask, this.queryTriggerInteraction);
				for (int i = 0; i < array.Length; i++)
				{
					BulletAttack.BulletHit bulletHit = default(BulletAttack.BulletHit);
					this.InitBulletHitFromRaycastHit(ref bulletHit, this.origin, normal, ref array[i]);
					list.Add(bulletHit);
					if (this.smartCollision)
					{
						hashSet.Add(bulletHit.entityObject);
					}
					if (bulletHit.distance < num)
					{
						num = bulletHit.distance;
					}
				}
			}
			if (flag2)
			{
				LayerMask mask2 = this.hitMask;
				mask2 &= ~LayerIndex.world.mask;
				RaycastHit[] array2 = Physics.SphereCastAll(this.origin, this.radius, normal, num, mask2, this.queryTriggerInteraction);
				for (int j = 0; j < array2.Length; j++)
				{
					BulletAttack.BulletHit bulletHit2 = default(BulletAttack.BulletHit);
					this.InitBulletHitFromRaycastHit(ref bulletHit2, this.origin, normal, ref array2[j]);
					if (!this.smartCollision || !hashSet.Contains(bulletHit2.entityObject))
					{
						list.Add(bulletHit2);
					}
				}
			}
			this.ProcessHitList(list, ref vector, new List<GameObject>());
			if (this.tracerEffectPrefab)
			{
				EffectData effectData = new EffectData
				{
					origin = vector,
					start = this.origin
				};
				effectData.SetChildLocatorTransformReference(this.weapon, muzzleIndex);
				EffectManager.instance.SpawnEffect(this.tracerEffectPrefab, effectData, true);
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00047F90 File Offset: 0x00046190
		[NetworkMessageHandler(msgType = 53, server = true)]
		private static void HandleBulletDamage(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			GameObject gameObject = reader.ReadGameObject();
			DamageInfo damageInfo = reader.ReadDamageInfo();
			if (reader.ReadBoolean() && gameObject)
			{
				HealthComponent component = gameObject.GetComponent<HealthComponent>();
				if (component)
				{
					component.TakeDamage(damageInfo);
				}
				GlobalEventManager.instance.OnHitEnemy(damageInfo, gameObject);
			}
			GlobalEventManager.instance.OnHitAll(damageInfo, gameObject);
		}

		// Token: 0x04000D95 RID: 3477
		public GameObject owner;

		// Token: 0x04000D96 RID: 3478
		public GameObject weapon;

		// Token: 0x04000D97 RID: 3479
		public float damage = 1f;

		// Token: 0x04000D98 RID: 3480
		public bool isCrit;

		// Token: 0x04000D99 RID: 3481
		public float force = 1f;

		// Token: 0x04000D9A RID: 3482
		public ProcChainMask procChainMask;

		// Token: 0x04000D9B RID: 3483
		public float procCoefficient = 1f;

		// Token: 0x04000D9C RID: 3484
		public DamageType damageType;

		// Token: 0x04000D9D RID: 3485
		public DamageColorIndex damageColorIndex;

		// Token: 0x04000D9E RID: 3486
		public bool sniper;

		// Token: 0x04000D9F RID: 3487
		public BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.DefaultBullet;

		// Token: 0x04000DA0 RID: 3488
		public GameObject tracerEffectPrefab;

		// Token: 0x04000DA1 RID: 3489
		public GameObject hitEffectPrefab;

		// Token: 0x04000DA2 RID: 3490
		public string muzzleName = "";

		// Token: 0x04000DA3 RID: 3491
		public bool HitEffectNormal = true;

		// Token: 0x04000DA4 RID: 3492
		public Vector3 origin;

		// Token: 0x04000DA5 RID: 3493
		private Vector3 _aimVector;

		// Token: 0x04000DA6 RID: 3494
		private float _maxDistance = 200f;

		// Token: 0x04000DA7 RID: 3495
		public float radius;

		// Token: 0x04000DA8 RID: 3496
		public uint bulletCount = 1u;

		// Token: 0x04000DA9 RID: 3497
		public float minSpread;

		// Token: 0x04000DAA RID: 3498
		public float maxSpread;

		// Token: 0x04000DAB RID: 3499
		public float spreadPitchScale = 1f;

		// Token: 0x04000DAC RID: 3500
		public float spreadYawScale = 1f;

		// Token: 0x04000DAD RID: 3501
		public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

		// Token: 0x04000DAE RID: 3502
		private static readonly LayerMask defaultHitMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;

		// Token: 0x04000DAF RID: 3503
		public LayerMask hitMask = BulletAttack.defaultHitMask;

		// Token: 0x04000DB0 RID: 3504
		private static readonly LayerMask defaultStopperMask = BulletAttack.defaultHitMask;

		// Token: 0x04000DB1 RID: 3505
		public LayerMask stopperMask = BulletAttack.defaultStopperMask;

		// Token: 0x04000DB2 RID: 3506
		public bool smartCollision;

		// Token: 0x04000DB3 RID: 3507
		public BulletAttack.HitCallback hitCallback;

		// Token: 0x04000DB4 RID: 3508
		private static NetworkWriter messageWriter = new NetworkWriter();

		// Token: 0x04000DB5 RID: 3509
		public BulletAttack.FilterCallback filterCallback;

		// Token: 0x0200020E RID: 526
		public enum FalloffModel
		{
			// Token: 0x04000DB7 RID: 3511
			None,
			// Token: 0x04000DB8 RID: 3512
			DefaultBullet,
			// Token: 0x04000DB9 RID: 3513
			Buckshot
		}

		// Token: 0x0200020F RID: 527
		// (Invoke) Token: 0x06000A4A RID: 2634
		public delegate bool HitCallback(ref BulletAttack.BulletHit hitInfo);

		// Token: 0x02000210 RID: 528
		// (Invoke) Token: 0x06000A4E RID: 2638
		public delegate bool FilterCallback(ref BulletAttack.BulletHit hitInfo);

		// Token: 0x02000211 RID: 529
		public struct BulletHit
		{
			// Token: 0x04000DBA RID: 3514
			public Vector3 direction;

			// Token: 0x04000DBB RID: 3515
			public Vector3 point;

			// Token: 0x04000DBC RID: 3516
			public Vector3 surfaceNormal;

			// Token: 0x04000DBD RID: 3517
			public float distance;

			// Token: 0x04000DBE RID: 3518
			public Collider collider;

			// Token: 0x04000DBF RID: 3519
			public GameObject entityObject;

			// Token: 0x04000DC0 RID: 3520
			public HurtBox.DamageModifier damageModifier;
		}
	}
}
