﻿using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200045F RID: 1119
	public class OverlapAttack
	{
		// Token: 0x0600190D RID: 6413 RVA: 0x00082210 File Offset: 0x00080410
		private bool HurtBoxPassesFilter(HurtBox hurtBox)
		{
			if (!hurtBox.healthComponent)
			{
				return true;
			}
			if (this.hitBoxGroup.transform.IsChildOf(hurtBox.healthComponent.transform))
			{
				return false;
			}
			if (this.ignoredHealthComponentList.Contains(hurtBox.healthComponent))
			{
				return false;
			}
			TeamComponent component = hurtBox.healthComponent.GetComponent<TeamComponent>();
			return !component || component.teamIndex != this.teamIndex;
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x00082288 File Offset: 0x00080488
		public bool Fire(List<HealthComponent> hitResults = null)
		{
			if (!this.hitBoxGroup)
			{
				return false;
			}
			HitBox[] hitBoxes = this.hitBoxGroup.hitBoxes;
			for (int i = 0; i < hitBoxes.Length; i++)
			{
				Transform transform = hitBoxes[i].transform;
				Vector3 position = transform.position;
				Vector3 vector = transform.lossyScale * 0.5f;
				Quaternion rotation = transform.rotation;
				if (float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are infinite.");
				}
				else if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are NaN.");
				}
				else if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is infinite.");
				}
				else if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is NaN.");
				}
				else if (float.IsInfinity(rotation.x) || float.IsInfinity(rotation.y) || float.IsInfinity(rotation.z) || float.IsInfinity(rotation.w))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is infinite.");
				}
				else if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) || float.IsNaN(rotation.w))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is NaN.");
				}
				else
				{
					Collider[] array = Physics.OverlapBox(position, vector, rotation, LayerIndex.entityPrecise.mask);
					int num = array.Length;
					for (int j = 0; j < num; j++)
					{
						HurtBox component = array[j].GetComponent<HurtBox>();
						if (component && this.HurtBoxPassesFilter(component))
						{
							Vector3 position2 = component.transform.position;
							this.overlapList.Add(new OverlapAttack.OverlapInfo
							{
								hurtBox = component,
								hitPosition = position2,
								pushDirection = (position2 - position).normalized
							});
							this.ignoredHealthComponentList.Add(component.healthComponent);
							if (hitResults != null)
							{
								hitResults.Add(component.healthComponent);
							}
						}
					}
				}
			}
			this.ProcessHits(this.overlapList);
			bool result = this.overlapList.Count > 0;
			this.overlapList.Clear();
			return result;
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x00082540 File Offset: 0x00080740
		[NetworkMessageHandler(msgType = 71, client = false, server = true)]
		public static void HandleOverlapAttackHits(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<OverlapAttack.OverlapAttackMessage>(OverlapAttack.incomingMessage);
			OverlapAttack.PerformDamage(OverlapAttack.incomingMessage.attacker, OverlapAttack.incomingMessage.inflictor, OverlapAttack.incomingMessage.damage, OverlapAttack.incomingMessage.isCrit, OverlapAttack.incomingMessage.procChainMask, OverlapAttack.incomingMessage.procCoefficient, OverlapAttack.incomingMessage.damageColorIndex, OverlapAttack.incomingMessage.damageType, OverlapAttack.incomingMessage.forceVector, OverlapAttack.incomingMessage.pushAwayForce, OverlapAttack.incomingMessage.overlapInfoList);
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x000825CC File Offset: 0x000807CC
		private void ProcessHits(List<OverlapAttack.OverlapInfo> hitList)
		{
			if (hitList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < hitList.Count; i++)
			{
				OverlapAttack.OverlapInfo overlapInfo = hitList[i];
				if (this.hitEffectPrefab)
				{
					EffectManager.instance.SimpleImpactEffect(this.hitEffectPrefab, overlapInfo.hitPosition, -hitList[i].pushDirection, true);
				}
				SurfaceDefProvider component = hitList[i].hurtBox.GetComponent<SurfaceDefProvider>();
				if (component && component.surfaceDef)
				{
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitList[i].hurtBox.collider, hitList[i].hitPosition);
					if (objectSurfaceDef)
					{
						if (objectSurfaceDef.impactEffectPrefab)
						{
							EffectManager.instance.SpawnEffect(objectSurfaceDef.impactEffectPrefab, new EffectData
							{
								origin = overlapInfo.hitPosition,
								rotation = ((overlapInfo.pushDirection == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(overlapInfo.pushDirection)),
								color = objectSurfaceDef.approximateColor,
								scale = 2f
							}, true);
						}
						if (objectSurfaceDef.impactSoundString != null && objectSurfaceDef.impactSoundString.Length != 0)
						{
							Util.PlaySound(objectSurfaceDef.impactSoundString, hitList[i].hurtBox.gameObject);
						}
					}
				}
			}
			if (NetworkServer.active)
			{
				OverlapAttack.PerformDamage(this.attacker, this.inflictor, this.damage, this.isCrit, this.procChainMask, this.procCoefficient, this.damageColorIndex, this.damageType, this.forceVector, this.pushAwayForce, hitList);
				return;
			}
			OverlapAttack.outgoingMessage.attacker = this.attacker;
			OverlapAttack.outgoingMessage.inflictor = this.inflictor;
			OverlapAttack.outgoingMessage.damage = this.damage;
			OverlapAttack.outgoingMessage.isCrit = this.isCrit;
			OverlapAttack.outgoingMessage.procChainMask = this.procChainMask;
			OverlapAttack.outgoingMessage.procCoefficient = this.procCoefficient;
			OverlapAttack.outgoingMessage.damageColorIndex = this.damageColorIndex;
			OverlapAttack.outgoingMessage.damageType = this.damageType;
			OverlapAttack.outgoingMessage.forceVector = this.forceVector;
			OverlapAttack.outgoingMessage.pushAwayForce = this.pushAwayForce;
			Util.CopyList<OverlapAttack.OverlapInfo>(hitList, OverlapAttack.outgoingMessage.overlapInfoList);
			GameNetworkManager.singleton.client.connection.SendByChannel(71, OverlapAttack.outgoingMessage, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x00082858 File Offset: 0x00080A58
		private static void PerformDamage(GameObject attacker, GameObject inflictor, float damage, bool isCrit, ProcChainMask procChainMask, float procCoefficient, DamageColorIndex damageColorIndex, DamageType damageType, Vector3 forceVector, float pushAwayForce, List<OverlapAttack.OverlapInfo> hitList)
		{
			for (int i = 0; i < hitList.Count; i++)
			{
				OverlapAttack.OverlapInfo overlapInfo = hitList[i];
				if (overlapInfo.hurtBox)
				{
					HealthComponent healthComponent = overlapInfo.hurtBox.healthComponent;
					if (healthComponent)
					{
						DamageInfo damageInfo = new DamageInfo();
						damageInfo.attacker = attacker;
						damageInfo.inflictor = inflictor;
						damageInfo.force = forceVector + pushAwayForce * overlapInfo.pushDirection;
						damageInfo.damage = damage;
						damageInfo.crit = isCrit;
						damageInfo.position = overlapInfo.hitPosition;
						damageInfo.procChainMask = procChainMask;
						damageInfo.procCoefficient = procCoefficient;
						damageInfo.damageColorIndex = damageColorIndex;
						damageInfo.damageType = damageType;
						damageInfo.ModifyDamageInfo(overlapInfo.hurtBox.damageModifier);
						healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
						GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
					}
				}
			}
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00012A2D File Offset: 0x00010C2D
		public void ResetIgnoredHealthComponents()
		{
			this.ignoredHealthComponentList.Clear();
		}

		// Token: 0x04001C67 RID: 7271
		public GameObject attacker;

		// Token: 0x04001C68 RID: 7272
		public GameObject inflictor;

		// Token: 0x04001C69 RID: 7273
		public TeamIndex teamIndex;

		// Token: 0x04001C6A RID: 7274
		public Vector3 forceVector = Vector3.zero;

		// Token: 0x04001C6B RID: 7275
		public float pushAwayForce;

		// Token: 0x04001C6C RID: 7276
		public float damage = 1f;

		// Token: 0x04001C6D RID: 7277
		public bool isCrit;

		// Token: 0x04001C6E RID: 7278
		public ProcChainMask procChainMask;

		// Token: 0x04001C6F RID: 7279
		public float procCoefficient = 1f;

		// Token: 0x04001C70 RID: 7280
		public HitBoxGroup hitBoxGroup;

		// Token: 0x04001C71 RID: 7281
		public GameObject hitEffectPrefab;

		// Token: 0x04001C72 RID: 7282
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001C73 RID: 7283
		public DamageType damageType;

		// Token: 0x04001C74 RID: 7284
		private readonly List<HealthComponent> ignoredHealthComponentList = new List<HealthComponent>();

		// Token: 0x04001C75 RID: 7285
		private readonly List<OverlapAttack.OverlapInfo> overlapList = new List<OverlapAttack.OverlapInfo>();

		// Token: 0x04001C76 RID: 7286
		private static readonly OverlapAttack.OverlapAttackMessage incomingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x04001C77 RID: 7287
		private static readonly OverlapAttack.OverlapAttackMessage outgoingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x02000460 RID: 1120
		private struct OverlapInfo
		{
			// Token: 0x04001C78 RID: 7288
			public HurtBox hurtBox;

			// Token: 0x04001C79 RID: 7289
			public Vector3 hitPosition;

			// Token: 0x04001C7A RID: 7290
			public Vector3 pushDirection;
		}

		// Token: 0x02000461 RID: 1121
		public struct AttackInfo
		{
			// Token: 0x04001C7B RID: 7291
			public GameObject attacker;

			// Token: 0x04001C7C RID: 7292
			public GameObject inflictor;

			// Token: 0x04001C7D RID: 7293
			public float damage;

			// Token: 0x04001C7E RID: 7294
			public bool isCrit;

			// Token: 0x04001C7F RID: 7295
			public float procCoefficient;

			// Token: 0x04001C80 RID: 7296
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001C81 RID: 7297
			public DamageType damageType;

			// Token: 0x04001C82 RID: 7298
			public Vector3 forceVector;
		}

		// Token: 0x02000462 RID: 1122
		private class OverlapAttackMessage : MessageBase
		{
			// Token: 0x06001915 RID: 6421 RVA: 0x00082950 File Offset: 0x00080B50
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.attacker);
				writer.Write(this.inflictor);
				writer.Write(this.damage);
				writer.Write(this.isCrit);
				writer.Write(this.procChainMask);
				writer.Write(this.procCoefficient);
				writer.Write(this.damageColorIndex);
				writer.Write(this.damageType);
				writer.Write(this.forceVector);
				writer.Write(this.pushAwayForce);
				writer.WritePackedUInt32((uint)this.overlapInfoList.Count);
				foreach (OverlapAttack.OverlapInfo overlapInfo in this.overlapInfoList)
				{
					writer.Write(HurtBoxReference.FromHurtBox(overlapInfo.hurtBox));
					writer.Write(overlapInfo.hitPosition);
					writer.Write(overlapInfo.pushDirection);
				}
			}

			// Token: 0x06001916 RID: 6422 RVA: 0x00082A58 File Offset: 0x00080C58
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.attacker = reader.ReadGameObject();
				this.inflictor = reader.ReadGameObject();
				this.damage = reader.ReadSingle();
				this.isCrit = reader.ReadBoolean();
				this.procChainMask = reader.ReadProcChainMask();
				this.procCoefficient = reader.ReadSingle();
				this.damageColorIndex = reader.ReadDamageColorIndex();
				this.damageType = reader.ReadDamageType();
				this.forceVector = reader.ReadVector3();
				this.pushAwayForce = reader.ReadSingle();
				this.overlapInfoList.Clear();
				int i = 0;
				int num = (int)reader.ReadPackedUInt32();
				while (i < num)
				{
					OverlapAttack.OverlapInfo item = default(OverlapAttack.OverlapInfo);
					GameObject gameObject = reader.ReadHurtBoxReference().ResolveGameObject();
					item.hurtBox = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
					item.hitPosition = reader.ReadVector3();
					item.pushDirection = reader.ReadVector3();
					this.overlapInfoList.Add(item);
					i++;
				}
			}

			// Token: 0x04001C83 RID: 7299
			public GameObject attacker;

			// Token: 0x04001C84 RID: 7300
			public GameObject inflictor;

			// Token: 0x04001C85 RID: 7301
			public float damage;

			// Token: 0x04001C86 RID: 7302
			public bool isCrit;

			// Token: 0x04001C87 RID: 7303
			public ProcChainMask procChainMask;

			// Token: 0x04001C88 RID: 7304
			public float procCoefficient;

			// Token: 0x04001C89 RID: 7305
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001C8A RID: 7306
			public DamageType damageType;

			// Token: 0x04001C8B RID: 7307
			public Vector3 forceVector;

			// Token: 0x04001C8C RID: 7308
			public float pushAwayForce;

			// Token: 0x04001C8D RID: 7309
			public readonly List<OverlapAttack.OverlapInfo> overlapInfoList = new List<OverlapAttack.OverlapInfo>();
		}
	}
}
