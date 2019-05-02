using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FA RID: 1018
	[RequireComponent(typeof(BezierCurveLine))]
	public class TarTetherController : NetworkBehaviour
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x00010CEE File Offset: 0x0000EEEE
		private void Awake()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00076A0C File Offset: 0x00074C0C
		private void DoDamageTick(bool mulch)
		{
			if (!this.targetHealthComponent)
			{
				this.targetHealthComponent = this.targetRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerHealthComponent)
			{
				this.ownerHealthComponent = this.ownerRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerBody)
			{
				this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();
			}
			if (this.ownerRoot)
			{
				DamageInfo damageInfo = new DamageInfo
				{
					position = this.targetRoot.transform.position,
					attacker = null,
					inflictor = null,
					damage = (mulch ? (this.damageCoefficientPerTick * this.mulchDamageScale) : this.damageCoefficientPerTick) * this.ownerBody.damage,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					crit = false,
					force = Vector3.zero,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0f
				};
				this.targetHealthComponent.TakeDamage(damageInfo);
				if (!damageInfo.rejected)
				{
					this.ownerHealthComponent.Heal(damageInfo.damage, default(ProcChainMask), true);
				}
				if (!this.targetHealthComponent.alive)
				{
					this.NetworktargetRoot = null;
				}
			}
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00076B54 File Offset: 0x00074D54
		private Vector3 GetTargetRootPosition()
		{
			if (this.targetRoot)
			{
				Vector3 result = this.targetRoot.transform.position;
				if (this.targetHealthComponent)
				{
					result = this.targetHealthComponent.body.corePosition;
				}
				return result;
			}
			return base.transform.position;
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00076BAC File Offset: 0x00074DAC
		private void Update()
		{
			this.age += Time.deltaTime;
			Vector3 position = this.ownerRoot.transform.position;
			if (!this.beginSiphon)
			{
				Vector3 position2 = Vector3.Lerp(position, this.GetTargetRootPosition(), this.age / this.attachTime);
				this.bezierCurveLine.endTransform.position = position2;
				return;
			}
			if (this.targetRoot)
			{
				this.bezierCurveLine.endTransform.position = this.targetRoot.transform.position;
			}
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00076C40 File Offset: 0x00074E40
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.targetRoot && this.ownerRoot)
			{
				Vector3 targetRootPosition = this.GetTargetRootPosition();
				if (!this.beginSiphon && this.fixedAge >= this.attachTime)
				{
					this.beginSiphon = true;
					return;
				}
				Vector3 vector = this.ownerRoot.transform.position - targetRootPosition;
				if (NetworkServer.active)
				{
					float sqrMagnitude = vector.sqrMagnitude;
					bool flag = sqrMagnitude < this.mulchDistanceSqr;
					this.tickTimer -= Time.fixedDeltaTime;
					if (this.tickTimer <= 0f)
					{
						this.tickTimer += (flag ? (this.tickInterval * this.mulchTickIntervalScale) : this.tickInterval);
						this.DoDamageTick(flag);
					}
					if (sqrMagnitude > this.breakDistanceSqr)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
				}
				if (Util.HasEffectiveAuthority(this.targetRoot))
				{
					Vector3 b = vector.normalized * (this.reelSpeed * Time.fixedDeltaTime);
					CharacterMotor component = this.targetRoot.GetComponent<CharacterMotor>();
					if (component)
					{
						component.rootMotion += b;
						return;
					}
					Rigidbody component2 = this.targetRoot.GetComponent<Rigidbody>();
					if (component2)
					{
						component2.velocity += b;
						return;
					}
				}
			}
			else if (NetworkServer.active)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x00076DC0 File Offset: 0x00074FC0
		// (set) Token: 0x06001675 RID: 5749 RVA: 0x00010D0F File Offset: 0x0000EF0F
		public GameObject NetworktargetRoot
		{
			get
			{
				return this.targetRoot;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetRoot, 1u, ref this.___targetRootNetId);
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001676 RID: 5750 RVA: 0x00076DD4 File Offset: 0x00074FD4
		// (set) Token: 0x06001677 RID: 5751 RVA: 0x00010D29 File Offset: 0x0000EF29
		public GameObject NetworkownerRoot
		{
			get
			{
				return this.ownerRoot;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerRoot, 2u, ref this.___ownerRootNetId);
			}
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00076DE8 File Offset: 0x00074FE8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetRoot);
				writer.Write(this.ownerRoot);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.targetRoot);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.ownerRoot);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00076E94 File Offset: 0x00075094
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetRootNetId = reader.ReadNetworkId();
				this.___ownerRootNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.targetRoot = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.ownerRoot = reader.ReadGameObject();
			}
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00076EFC File Offset: 0x000750FC
		public override void PreStartClient()
		{
			if (!this.___targetRootNetId.IsEmpty())
			{
				this.NetworktargetRoot = ClientScene.FindLocalObject(this.___targetRootNetId);
			}
			if (!this.___ownerRootNetId.IsEmpty())
			{
				this.NetworkownerRoot = ClientScene.FindLocalObject(this.___ownerRootNetId);
			}
		}

		// Token: 0x0400199F RID: 6559
		[SyncVar]
		public GameObject targetRoot;

		// Token: 0x040019A0 RID: 6560
		[SyncVar]
		public GameObject ownerRoot;

		// Token: 0x040019A1 RID: 6561
		public float reelSpeed = 12f;

		// Token: 0x040019A2 RID: 6562
		[NonSerialized]
		public float mulchDistanceSqr;

		// Token: 0x040019A3 RID: 6563
		[NonSerialized]
		public float breakDistanceSqr;

		// Token: 0x040019A4 RID: 6564
		[NonSerialized]
		public float mulchDamageScale;

		// Token: 0x040019A5 RID: 6565
		[NonSerialized]
		public float mulchTickIntervalScale;

		// Token: 0x040019A6 RID: 6566
		[NonSerialized]
		public float damageCoefficientPerTick;

		// Token: 0x040019A7 RID: 6567
		[NonSerialized]
		public float tickInterval;

		// Token: 0x040019A8 RID: 6568
		[NonSerialized]
		public float tickTimer;

		// Token: 0x040019A9 RID: 6569
		public float attachTime;

		// Token: 0x040019AA RID: 6570
		private float fixedAge;

		// Token: 0x040019AB RID: 6571
		private float age;

		// Token: 0x040019AC RID: 6572
		private bool beginSiphon;

		// Token: 0x040019AD RID: 6573
		private BezierCurveLine bezierCurveLine;

		// Token: 0x040019AE RID: 6574
		private HealthComponent targetHealthComponent;

		// Token: 0x040019AF RID: 6575
		private HealthComponent ownerHealthComponent;

		// Token: 0x040019B0 RID: 6576
		private CharacterBody ownerBody;

		// Token: 0x040019B1 RID: 6577
		private NetworkInstanceId ___targetRootNetId;

		// Token: 0x040019B2 RID: 6578
		private NetworkInstanceId ___ownerRootNetId;
	}
}
