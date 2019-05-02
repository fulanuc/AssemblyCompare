using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F6 RID: 758
	public class FireAuraController : NetworkBehaviour
	{
		// Token: 0x06000F5D RID: 3933 RVA: 0x0005C9AC File Offset: 0x0005ABAC
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			CharacterBody characterBody = null;
			float num = 0f;
			if (this.owner)
			{
				characterBody = this.owner.GetComponent<CharacterBody>();
				num = (characterBody ? Mathf.Lerp(characterBody.radius * 0.5f, characterBody.radius * 6f, 1f - Mathf.Abs(-1f + 2f * this.timer / 8f)) : 0f);
				base.transform.position = this.owner.transform.position;
				base.transform.localScale = new Vector3(num, num, num);
			}
			if (NetworkServer.active)
			{
				if (!this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				this.attackStopwatch += Time.fixedDeltaTime;
				if (characterBody && this.attackStopwatch >= 0.25f)
				{
					this.attackStopwatch = 0f;
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.attacker = this.owner;
					blastAttack.inflictor = base.gameObject;
					blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
					blastAttack.position = base.transform.position;
					blastAttack.procCoefficient = 0.1f;
					blastAttack.radius = num;
					blastAttack.baseForce = 0f;
					blastAttack.baseDamage = 1f * characterBody.damage;
					blastAttack.bonusForce = Vector3.zero;
					blastAttack.crit = false;
					blastAttack.damageType = DamageType.Generic;
					blastAttack.Fire();
				}
				if (this.timer >= 8f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000F60 RID: 3936 RVA: 0x0005CB6C File Offset: 0x0005AD6C
		// (set) Token: 0x06000F61 RID: 3937 RVA: 0x0000BCFD File Offset: 0x00009EFD
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 1u, ref this.___ownerNetId);
			}
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0005CB80 File Offset: 0x0005AD80
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
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
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0005CBEC File Offset: 0x0005ADEC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0000BD17 File Offset: 0x00009F17
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04001370 RID: 4976
		private const float fireAttackRadiusMin = 0.5f;

		// Token: 0x04001371 RID: 4977
		private const float fireAttackRadiusMax = 6f;

		// Token: 0x04001372 RID: 4978
		private const float fireDamageCoefficient = 1f;

		// Token: 0x04001373 RID: 4979
		private const float fireProcCoefficient = 0.1f;

		// Token: 0x04001374 RID: 4980
		private const float maxTimer = 8f;

		// Token: 0x04001375 RID: 4981
		private float timer;

		// Token: 0x04001376 RID: 4982
		private float attackStopwatch;

		// Token: 0x04001377 RID: 4983
		[SyncVar]
		public GameObject owner;

		// Token: 0x04001378 RID: 4984
		private NetworkInstanceId ___ownerNetId;
	}
}
