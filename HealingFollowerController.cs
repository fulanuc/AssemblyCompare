using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000437 RID: 1079
	public class HealingFollowerController : NetworkBehaviour
	{
		// Token: 0x06001808 RID: 6152 RVA: 0x00011F0D File Offset: 0x0001010D
		private void FixedUpdate()
		{
			if (this.cachedTargetBodyObject != this.targetBodyObject)
			{
				this.cachedTargetBodyObject = this.targetBodyObject;
				this.OnTargetChanged();
			}
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007D90C File Offset: 0x0007BB0C
		public void AssignNewTarget(GameObject target)
		{
			this.NetworktargetBodyObject = (target ? target : this.ownerBodyObject);
			this.cachedTargetBodyObject = this.targetBodyObject;
			this.OnTargetChanged();
			CharacterBody component = this.targetBodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				EffectManager.instance.SimpleImpactEffect(this.burstHealEffect, component.mainHurtBox.transform.position, Vector3.up, true);
			}
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x00011F41 File Offset: 0x00010141
		private void OnTargetChanged()
		{
			this.cachedTargetHealthComponent = (this.cachedTargetBodyObject ? this.cachedTargetBodyObject.GetComponent<HealthComponent>() : null);
			if (NetworkServer.active)
			{
				this.DoHeal(this.fractionHealthBurst);
			}
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x0007D97C File Offset: 0x0007BB7C
		private void FixedUpdateServer()
		{
			this.healingTimer -= Time.fixedDeltaTime;
			if (this.healingTimer <= 0f)
			{
				this.healingTimer = this.healingInterval;
				this.DoHeal(this.fractionHealthHealing * this.healingInterval);
			}
			if (!this.targetBodyObject)
			{
				this.NetworktargetBodyObject = this.ownerBodyObject;
			}
			if (!this.ownerBodyObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x0007D9F8 File Offset: 0x0007BBF8
		private void Update()
		{
			this.UpdateMotion();
			base.transform.position += this.velocity * Time.deltaTime;
			base.transform.rotation = Quaternion.AngleAxis(this.rotationAngularVelocity * Time.deltaTime, Vector3.up) * base.transform.rotation;
			if (this.targetBodyObject)
			{
				this.indicator.transform.position = this.targetBodyObject.transform.position;
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x0007DA90 File Offset: 0x0007BC90
		[Server]
		private void DoHeal(float healFraction)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealingFollowerController::DoHeal(System.Single)' called on client");
				return;
			}
			if (!this.cachedTargetHealthComponent)
			{
				return;
			}
			this.cachedTargetHealthComponent.HealFraction(healFraction, default(ProcChainMask));
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00011F77 File Offset: 0x00010177
		public override void OnStartClient()
		{
			base.OnStartClient();
			base.transform.position = this.GetDesiredPosition();
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x0007DAD8 File Offset: 0x0007BCD8
		private Vector3 GetDesiredPosition()
		{
			GameObject gameObject = this.targetBodyObject ?? this.ownerBodyObject;
			if (!gameObject)
			{
				return base.transform.position + UnityEngine.Random.onUnitSphere;
			}
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			if (!component)
			{
				return gameObject.transform.position;
			}
			return component.corePosition;
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0007DB38 File Offset: 0x0007BD38
		private void UpdateMotion()
		{
			Vector3 desiredPosition = this.GetDesiredPosition();
			if (this.enableSpringMotion)
			{
				Vector3 lhs = desiredPosition - base.transform.position;
				if (lhs != Vector3.zero)
				{
					Vector3 a = lhs.normalized * this.acceleration;
					Vector3 b = this.velocity * -this.damping;
					this.velocity += (a + b) * Time.deltaTime;
					return;
				}
			}
			else
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, desiredPosition, ref this.velocity, this.damping);
			}
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06001813 RID: 6163 RVA: 0x0007DBE4 File Offset: 0x0007BDE4
		// (set) Token: 0x06001814 RID: 6164 RVA: 0x00011FCF File Offset: 0x000101CF
		public GameObject NetworkownerBodyObject
		{
			get
			{
				return this.ownerBodyObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerBodyObject, 1u, ref this.___ownerBodyObjectNetId);
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06001815 RID: 6165 RVA: 0x0007DBF8 File Offset: 0x0007BDF8
		// (set) Token: 0x06001816 RID: 6166 RVA: 0x00011FE9 File Offset: 0x000101E9
		public GameObject NetworktargetBodyObject
		{
			get
			{
				return this.targetBodyObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetBodyObject, 2u, ref this.___targetBodyObjectNetId);
			}
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x0007DC0C File Offset: 0x0007BE0C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.ownerBodyObject);
				writer.Write(this.targetBodyObject);
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
				writer.Write(this.ownerBodyObject);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.targetBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x0007DCB8 File Offset: 0x0007BEB8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___ownerBodyObjectNetId = reader.ReadNetworkId();
				this.___targetBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.ownerBodyObject = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.targetBodyObject = reader.ReadGameObject();
			}
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0007DD20 File Offset: 0x0007BF20
		public override void PreStartClient()
		{
			if (!this.___ownerBodyObjectNetId.IsEmpty())
			{
				this.NetworkownerBodyObject = ClientScene.FindLocalObject(this.___ownerBodyObjectNetId);
			}
			if (!this.___targetBodyObjectNetId.IsEmpty())
			{
				this.NetworktargetBodyObject = ClientScene.FindLocalObject(this.___targetBodyObjectNetId);
			}
		}

		// Token: 0x04001B50 RID: 6992
		public float fractionHealthHealing = 0.01f;

		// Token: 0x04001B51 RID: 6993
		public float fractionHealthBurst = 0.05f;

		// Token: 0x04001B52 RID: 6994
		public float healingInterval = 1f;

		// Token: 0x04001B53 RID: 6995
		public float rotationAngularVelocity;

		// Token: 0x04001B54 RID: 6996
		public float acceleration = 20f;

		// Token: 0x04001B55 RID: 6997
		public float damping = 0.1f;

		// Token: 0x04001B56 RID: 6998
		public bool enableSpringMotion;

		// Token: 0x04001B57 RID: 6999
		[SyncVar]
		public GameObject ownerBodyObject;

		// Token: 0x04001B58 RID: 7000
		[SyncVar]
		public GameObject targetBodyObject;

		// Token: 0x04001B59 RID: 7001
		public GameObject burstHealEffect;

		// Token: 0x04001B5A RID: 7002
		public GameObject indicator;

		// Token: 0x04001B5B RID: 7003
		private GameObject cachedTargetBodyObject;

		// Token: 0x04001B5C RID: 7004
		private HealthComponent cachedTargetHealthComponent;

		// Token: 0x04001B5D RID: 7005
		private float healingTimer;

		// Token: 0x04001B5E RID: 7006
		private Vector3 velocity;

		// Token: 0x04001B5F RID: 7007
		private NetworkInstanceId ___ownerBodyObjectNetId;

		// Token: 0x04001B60 RID: 7008
		private NetworkInstanceId ___targetBodyObjectNetId;
	}
}
