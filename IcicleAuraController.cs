using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032D RID: 813
	public class IcicleAuraController : NetworkBehaviour
	{
		// Token: 0x060010C9 RID: 4297 RVA: 0x0000CD59 File Offset: 0x0000AF59
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x000634A0 File Offset: 0x000616A0
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

		// Token: 0x060010CB RID: 4299 RVA: 0x0006372C File Offset: 0x0006192C
		private void UpdateRadius()
		{
			if (this.owner)
			{
				this.actualRadius = (this.cachedOwnerInfo.characterBody ? (this.cachedOwnerInfo.characterBody.radius + 10f) : 0f);
				this.transform.localScale = new Vector3(this.actualRadius, this.actualRadius, this.actualRadius);
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x000637A0 File Offset: 0x000619A0
		private void UpdatePosition()
		{
			if (this.cachedOwnerInfo.gameObject)
			{
				this.transform.position = (this.cachedOwnerInfo.characterBody ? this.cachedOwnerInfo.characterBody.corePosition : this.cachedOwnerInfo.transform.position);
			}
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00063800 File Offset: 0x00061A00
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

		// Token: 0x060010CE RID: 4302 RVA: 0x00063868 File Offset: 0x00061A68
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

		// Token: 0x060010CF RID: 4303 RVA: 0x000638D8 File Offset: 0x00061AD8
		private void OnIcicleGained()
		{
			ParticleSystem[] array = this.procParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0000CD67 File Offset: 0x0000AF67
		private void LateUpdate()
		{
			this.UpdatePosition();
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0000CD6F File Offset: 0x0000AF6F
		public void OnOwnerKillOther()
		{
			this.icicleLifetimes.Add(5f);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0000CD81 File Offset: 0x0000AF81
		public void OnDestroy()
		{
			this.OnIciclesDeactivated();
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x00063904 File Offset: 0x00061B04
		// (set) Token: 0x060010D6 RID: 4310 RVA: 0x0000CD9C File Offset: 0x0000AF9C
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

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x00063918 File Offset: 0x00061B18
		// (set) Token: 0x060010D8 RID: 4312 RVA: 0x0000CDB0 File Offset: 0x0000AFB0
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

		// Token: 0x060010D9 RID: 4313 RVA: 0x0006392C File Offset: 0x00061B2C
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

		// Token: 0x060010DA RID: 4314 RVA: 0x000639D8 File Offset: 0x00061BD8
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

		// Token: 0x060010DB RID: 4315 RVA: 0x0000CDCA File Offset: 0x0000AFCA
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x040014CE RID: 5326
		private const float baseIcicleAttackInterval = 1f;

		// Token: 0x040014CF RID: 5327
		private const float icicleAttackRadius = 10f;

		// Token: 0x040014D0 RID: 5328
		private const float icicleDamageCoefficient = 0.25f;

		// Token: 0x040014D1 RID: 5329
		private const float icicleDuration = 5f;

		// Token: 0x040014D2 RID: 5330
		private const float icicleProcCoefficient = 0.05f;

		// Token: 0x040014D3 RID: 5331
		private const int maxIcicleCount = 5;

		// Token: 0x040014D4 RID: 5332
		private List<float> icicleLifetimes = new List<float>();

		// Token: 0x040014D5 RID: 5333
		private float attackStopwatch;

		// Token: 0x040014D6 RID: 5334
		private int lastIcicleCount;

		// Token: 0x040014D7 RID: 5335
		[SyncVar]
		private int finalIcicleCount;

		// Token: 0x040014D8 RID: 5336
		[SyncVar]
		public GameObject owner;

		// Token: 0x040014D9 RID: 5337
		private IcicleAuraController.OwnerInfo cachedOwnerInfo;

		// Token: 0x040014DA RID: 5338
		public ParticleSystem[] auraParticles;

		// Token: 0x040014DB RID: 5339
		public ParticleSystem[] procParticles;

		// Token: 0x040014DC RID: 5340
		private new Transform transform;

		// Token: 0x040014DD RID: 5341
		private float actualRadius;

		// Token: 0x040014DE RID: 5342
		private NetworkInstanceId ___ownerNetId;

		// Token: 0x0200032E RID: 814
		private struct OwnerInfo
		{
			// Token: 0x060010DC RID: 4316 RVA: 0x00063A40 File Offset: 0x00061C40
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

			// Token: 0x040014DF RID: 5343
			public readonly GameObject gameObject;

			// Token: 0x040014E0 RID: 5344
			public readonly Transform transform;

			// Token: 0x040014E1 RID: 5345
			public readonly CharacterBody characterBody;

			// Token: 0x040014E2 RID: 5346
			public readonly CameraTargetParams cameraTargetParams;
		}
	}
}
