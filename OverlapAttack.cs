using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200046A RID: 1130
	public class OverlapAttack
	{
		// Token: 0x0600196A RID: 6506 RVA: 0x00082BB8 File Offset: 0x00080DB8
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

		// Token: 0x0600196B RID: 6507 RVA: 0x00082C30 File Offset: 0x00080E30
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

		// Token: 0x0600196C RID: 6508 RVA: 0x00082EE8 File Offset: 0x000810E8
		[NetworkMessageHandler(msgType = 71, client = false, server = true)]
		public static void HandleOverlapAttackHits(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<OverlapAttack.OverlapAttackMessage>(OverlapAttack.incomingMessage);
			OverlapAttack.PerformDamage(OverlapAttack.incomingMessage.attacker, OverlapAttack.incomingMessage.inflictor, OverlapAttack.incomingMessage.damage, OverlapAttack.incomingMessage.isCrit, OverlapAttack.incomingMessage.procChainMask, OverlapAttack.incomingMessage.procCoefficient, OverlapAttack.incomingMessage.damageColorIndex, OverlapAttack.incomingMessage.damageType, OverlapAttack.incomingMessage.forceVector, OverlapAttack.incomingMessage.pushAwayForce, OverlapAttack.incomingMessage.overlapInfoList);
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x00082F74 File Offset: 0x00081174
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

		// Token: 0x0600196E RID: 6510 RVA: 0x00083200 File Offset: 0x00081400
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

		// Token: 0x0600196F RID: 6511 RVA: 0x00012F47 File Offset: 0x00011147
		public void ResetIgnoredHealthComponents()
		{
			this.ignoredHealthComponentList.Clear();
		}

		// Token: 0x04001C9B RID: 7323
		public GameObject attacker;

		// Token: 0x04001C9C RID: 7324
		public GameObject inflictor;

		// Token: 0x04001C9D RID: 7325
		public TeamIndex teamIndex;

		// Token: 0x04001C9E RID: 7326
		public Vector3 forceVector = Vector3.zero;

		// Token: 0x04001C9F RID: 7327
		public float pushAwayForce;

		// Token: 0x04001CA0 RID: 7328
		public float damage = 1f;

		// Token: 0x04001CA1 RID: 7329
		public bool isCrit;

		// Token: 0x04001CA2 RID: 7330
		public ProcChainMask procChainMask;

		// Token: 0x04001CA3 RID: 7331
		public float procCoefficient = 1f;

		// Token: 0x04001CA4 RID: 7332
		public HitBoxGroup hitBoxGroup;

		// Token: 0x04001CA5 RID: 7333
		public GameObject hitEffectPrefab;

		// Token: 0x04001CA6 RID: 7334
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001CA7 RID: 7335
		public DamageType damageType;

		// Token: 0x04001CA8 RID: 7336
		private readonly List<HealthComponent> ignoredHealthComponentList = new List<HealthComponent>();

		// Token: 0x04001CA9 RID: 7337
		private readonly List<OverlapAttack.OverlapInfo> overlapList = new List<OverlapAttack.OverlapInfo>();

		// Token: 0x04001CAA RID: 7338
		private static readonly OverlapAttack.OverlapAttackMessage incomingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x04001CAB RID: 7339
		private static readonly OverlapAttack.OverlapAttackMessage outgoingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x0200046B RID: 1131
		private struct OverlapInfo
		{
			// Token: 0x04001CAC RID: 7340
			public HurtBox hurtBox;

			// Token: 0x04001CAD RID: 7341
			public Vector3 hitPosition;

			// Token: 0x04001CAE RID: 7342
			public Vector3 pushDirection;
		}

		// Token: 0x0200046C RID: 1132
		public struct AttackInfo
		{
			// Token: 0x04001CAF RID: 7343
			public GameObject attacker;

			// Token: 0x04001CB0 RID: 7344
			public GameObject inflictor;

			// Token: 0x04001CB1 RID: 7345
			public float damage;

			// Token: 0x04001CB2 RID: 7346
			public bool isCrit;

			// Token: 0x04001CB3 RID: 7347
			public float procCoefficient;

			// Token: 0x04001CB4 RID: 7348
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001CB5 RID: 7349
			public DamageType damageType;

			// Token: 0x04001CB6 RID: 7350
			public Vector3 forceVector;
		}

		// Token: 0x0200046D RID: 1133
		private class OverlapAttackMessage : MessageBase
		{
			// Token: 0x06001972 RID: 6514 RVA: 0x000832F8 File Offset: 0x000814F8
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

			// Token: 0x06001973 RID: 6515 RVA: 0x00083400 File Offset: 0x00081600
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

			// Token: 0x04001CB7 RID: 7351
			public GameObject attacker;

			// Token: 0x04001CB8 RID: 7352
			public GameObject inflictor;

			// Token: 0x04001CB9 RID: 7353
			public float damage;

			// Token: 0x04001CBA RID: 7354
			public bool isCrit;

			// Token: 0x04001CBB RID: 7355
			public ProcChainMask procChainMask;

			// Token: 0x04001CBC RID: 7356
			public float procCoefficient;

			// Token: 0x04001CBD RID: 7357
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001CBE RID: 7358
			public DamageType damageType;

			// Token: 0x04001CBF RID: 7359
			public Vector3 forceVector;

			// Token: 0x04001CC0 RID: 7360
			public float pushAwayForce;

			// Token: 0x04001CC1 RID: 7361
			public readonly List<OverlapAttack.OverlapInfo> overlapInfoList = new List<OverlapAttack.OverlapInfo>();
		}
	}
}
