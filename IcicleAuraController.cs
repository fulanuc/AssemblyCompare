using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032B RID: 811
	public class IcicleAuraController : NetworkBehaviour
	{
		// Token: 0x060010B5 RID: 4277 RVA: 0x0000CC70 File Offset: 0x0000AE70
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00063214 File Offset: 0x00061414
		private void FixedUpdate()
		{
			if (this.cachedOwnerInfo.gameObject != this.owner)
			{
				this.cachedOwnerInfo = new IcicleAuraController.OwnerInfo(this.owner);
			}
			this.UpdateRadius();
			this.UpdatePosition();
			if (NetworkServer.active)
			{
				if (!this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				for (int i = this.icicleLifetimes.Count - 1; i >= 0; i--)
				{
					List<float> list = this.icicleLifetimes;
					int index = i;
					list[index] -= Time.fixedDeltaTime;
					if (this.icicleLifetimes[i] <= 0f)
					{
						this.icicleLifetimes.RemoveAt(i);
					}
				}
				this.NetworkfinalIcicleCount = Mathf.Min((this.icicleLifetimes.Count > 0) ? (2 + this.icicleLifetimes.Count) : 0, 5);
				this.attackStopwatch += Time.fixedDeltaTime;
			}
			if (this.cachedOwnerInfo.characterBody)
			{
				if (this.finalIcicleCount > 0)
				{
					if (this.lastIcicleCount == 0)
					{
						this.OnIciclesActivated();
					}
					if (this.lastIcicleCount < this.finalIcicleCount)
					{
						this.OnIcicleGained();
					}
				}
				else if (this.lastIcicleCount > 0)
				{
					this.OnIciclesDeactivated();
				}
				this.lastIcicleCount = this.finalIcicleCount;
			}
			if (NetworkServer.active && this.cachedOwnerInfo.characterBody && this.finalIcicleCount > 0)
			{
				float num = 0f;
				if (this.cachedOwnerInfo.characterBody.inventory)
				{
					num = 0.5f + 0.5f * (float)this.cachedOwnerInfo.characterBody.inventory.GetItemCount(ItemIndex.Icicle);
				}
				if (this.attackStopwatch >= 0.25f)
				{
					this.attackStopwatch = 0f;
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.attacker = this.owner;
					blastAttack.inflictor = base.gameObject;
					blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
					blastAttack.position = this.transform.position;
					blastAttack.procCoefficient = 0.05f;
					blastAttack.radius = this.actualRadius;
					blastAttack.baseForce = 0f;
					blastAttack.baseDamage = num * 0.25f * this.cachedOwnerInfo.characterBody.damage * (float)this.finalIcicleCount;
					blastAttack.bonusForce = Vector3.zero;
					blastAttack.crit = false;
					blastAttack.damageType = DamageType.Generic;
					blastAttack.falloffModel = BlastAttack.FalloffModel.None;
					blastAttack.damageColorIndex = DamageColorIndex.Item;
					blastAttack.Fire();
				}
			}
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000634A0 File Offset: 0x000616A0
		private void UpdateRadius()
		{
			if (this.owner)
			{
				this.actualRadius = (this.cachedOwnerInfo.characterBody ? (this.cachedOwnerInfo.characterBody.radius + 10f) : 0f);
				this.transform.localScale = new Vector3(this.actualRadius, this.actualRadius, this.actualRadius);
			}
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00063514 File Offset: 0x00061714
		private void UpdatePosition()
		{
			if (this.cachedOwnerInfo.gameObject)
			{
				this.transform.position = (this.cachedOwnerInfo.characterBody ? this.cachedOwnerInfo.characterBody.corePosition : this.cachedOwnerInfo.transform.position);
			}
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00063574 File Offset: 0x00061774
		private void OnIciclesDeactivated()
		{
			Util.PlaySound("Stop_item_proc_icicle", base.gameObject);
			ParticleSystem[] array = this.auraParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].main.loop = false;
			}
			if (this.cachedOwnerInfo.cameraTargetParams)
			{
				this.cachedOwnerInfo.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x000635DC File Offset: 0x000617DC
		private void OnIciclesActivated()
		{
			Util.PlaySound("Play_item_proc_icicle", base.gameObject);
			if (this.cachedOwnerInfo.cameraTargetParams)
			{
				this.cachedOwnerInfo.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			foreach (ParticleSystem particleSystem in this.auraParticles)
			{
				particleSystem.main.loop = true;
				particleSystem.Play();
			}
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0006364C File Offset: 0x0006184C
		private void OnIcicleGained()
		{
			ParticleSystem[] array = this.procParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x0000CC7E File Offset: 0x0000AE7E
		private void LateUpdate()
		{
			this.UpdatePosition();
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0000CC86 File Offset: 0x0000AE86
		public void OnOwnerKillOther()
		{
			this.icicleLifetimes.Add(5f);
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0000CC98 File Offset: 0x0000AE98
		public void OnDestroy()
		{
			this.OnIciclesDeactivated();
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060010C1 RID: 4289 RVA: 0x00063678 File Offset: 0x00061878
		// (set) Token: 0x060010C2 RID: 4290 RVA: 0x0000CCB3 File Offset: 0x0000AEB3
		public int NetworkfinalIcicleCount
		{
			get
			{
				return this.finalIcicleCount;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.finalIcicleCount, 1u);
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060010C3 RID: 4291 RVA: 0x0006368C File Offset: 0x0006188C
		// (set) Token: 0x060010C4 RID: 4292 RVA: 0x0000CCC7 File Offset: 0x0000AEC7
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 2u, ref this.___ownerNetId);
			}
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x000636A0 File Offset: 0x000618A0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.finalIcicleCount);
				writer.Write(this.owner);
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
				writer.WritePackedUInt32((uint)this.finalIcicleCount);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0006374C File Offset: 0x0006194C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.finalIcicleCount = (int)reader.ReadPackedUInt32();
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.finalIcicleCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0000CCE1 File Offset: 0x0000AEE1
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x040014BA RID: 5306
		private const float baseIcicleAttackInterval = 1f;

		// Token: 0x040014BB RID: 5307
		private const float icicleAttackRadius = 10f;

		// Token: 0x040014BC RID: 5308
		private const float icicleDamageCoefficient = 0.25f;

		// Token: 0x040014BD RID: 5309
		private const float icicleDuration = 5f;

		// Token: 0x040014BE RID: 5310
		private const float icicleProcCoefficient = 0.05f;

		// Token: 0x040014BF RID: 5311
		private const int maxIcicleCount = 5;

		// Token: 0x040014C0 RID: 5312
		private List<float> icicleLifetimes = new List<float>();

		// Token: 0x040014C1 RID: 5313
		private float attackStopwatch;

		// Token: 0x040014C2 RID: 5314
		private int lastIcicleCount;

		// Token: 0x040014C3 RID: 5315
		[SyncVar]
		private int finalIcicleCount;

		// Token: 0x040014C4 RID: 5316
		[SyncVar]
		public GameObject owner;

		// Token: 0x040014C5 RID: 5317
		private IcicleAuraController.OwnerInfo cachedOwnerInfo;

		// Token: 0x040014C6 RID: 5318
		public ParticleSystem[] auraParticles;

		// Token: 0x040014C7 RID: 5319
		public ParticleSystem[] procParticles;

		// Token: 0x040014C8 RID: 5320
		private new Transform transform;

		// Token: 0x040014C9 RID: 5321
		private float actualRadius;

		// Token: 0x040014CA RID: 5322
		private NetworkInstanceId ___ownerNetId;

		// Token: 0x0200032C RID: 812
		private struct OwnerInfo
		{
			// Token: 0x060010C8 RID: 4296 RVA: 0x000637B4 File Offset: 0x000619B4
			public OwnerInfo(GameObject gameObject)
			{
				this.gameObject = gameObject;
				if (gameObject)
				{
					this.transform = gameObject.transform;
					this.characterBody = gameObject.GetComponent<CharacterBody>();
					this.cameraTargetParams = gameObject.GetComponent<CameraTargetParams>();
					return;
				}
				this.transform = null;
				this.characterBody = null;
				this.cameraTargetParams = null;
			}

			// Token: 0x040014CB RID: 5323
			public readonly GameObject gameObject;

			// Token: 0x040014CC RID: 5324
			public readonly Transform transform;

			// Token: 0x040014CD RID: 5325
			public readonly CharacterBody characterBody;

			// Token: 0x040014CE RID: 5326
			public readonly CameraTargetParams cameraTargetParams;
		}
	}
}
