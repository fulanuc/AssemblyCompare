using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200043F RID: 1087
	public class HealingFollowerController : NetworkBehaviour
	{
		// Token: 0x06001855 RID: 6229 RVA: 0x00012381 File Offset: 0x00010581
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

		// Token: 0x06001856 RID: 6230 RVA: 0x0007E0C8 File Offset: 0x0007C2C8
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

		// Token: 0x06001857 RID: 6231 RVA: 0x000123B5 File Offset: 0x000105B5
		private void OnTargetChanged()
		{
			this.cachedTargetHealthComponent = (this.cachedTargetBodyObject ? this.cachedTargetBodyObject.GetComponent<HealthComponent>() : null);
			if (NetworkServer.active)
			{
				this.DoHeal(this.fractionHealthBurst);
			}
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x0007E138 File Offset: 0x0007C338
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

		// Token: 0x06001859 RID: 6233 RVA: 0x0007E1B4 File Offset: 0x0007C3B4
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

		// Token: 0x0600185A RID: 6234 RVA: 0x0007E24C File Offset: 0x0007C44C
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

		// Token: 0x0600185B RID: 6235 RVA: 0x000123EB File Offset: 0x000105EB
		public override void OnStartClient()
		{
			base.OnStartClient();
			base.transform.position = this.GetDesiredPosition();
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0007E294 File Offset: 0x0007C494
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

		// Token: 0x0600185D RID: 6237 RVA: 0x0007E2F4 File Offset: 0x0007C4F4
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

		// Token: 0x0600185F RID: 6239 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06001860 RID: 6240 RVA: 0x0007E3A0 File Offset: 0x0007C5A0
		// (set) Token: 0x06001861 RID: 6241 RVA: 0x00012443 File Offset: 0x00010643
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

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06001862 RID: 6242 RVA: 0x0007E3B4 File Offset: 0x0007C5B4
		// (set) Token: 0x06001863 RID: 6243 RVA: 0x0001245D File Offset: 0x0001065D
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

		// Token: 0x06001864 RID: 6244 RVA: 0x0007E3C8 File Offset: 0x0007C5C8
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

		// Token: 0x06001865 RID: 6245 RVA: 0x0007E474 File Offset: 0x0007C674
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

		// Token: 0x06001866 RID: 6246 RVA: 0x0007E4DC File Offset: 0x0007C6DC
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

		// Token: 0x04001B80 RID: 7040
		public float fractionHealthHealing = 0.01f;

		// Token: 0x04001B81 RID: 7041
		public float fractionHealthBurst = 0.05f;

		// Token: 0x04001B82 RID: 7042
		public float healingInterval = 1f;

		// Token: 0x04001B83 RID: 7043
		public float rotationAngularVelocity;

		// Token: 0x04001B84 RID: 7044
		public float acceleration = 20f;

		// Token: 0x04001B85 RID: 7045
		public float damping = 0.1f;

		// Token: 0x04001B86 RID: 7046
		public bool enableSpringMotion;

		// Token: 0x04001B87 RID: 7047
		[SyncVar]
		public GameObject ownerBodyObject;

		// Token: 0x04001B88 RID: 7048
		[SyncVar]
		public GameObject targetBodyObject;

		// Token: 0x04001B89 RID: 7049
		public GameObject burstHealEffect;

		// Token: 0x04001B8A RID: 7050
		public GameObject indicator;

		// Token: 0x04001B8B RID: 7051
		private GameObject cachedTargetBodyObject;

		// Token: 0x04001B8C RID: 7052
		private HealthComponent cachedTargetHealthComponent;

		// Token: 0x04001B8D RID: 7053
		private float healingTimer;

		// Token: 0x04001B8E RID: 7054
		private Vector3 velocity;

		// Token: 0x04001B8F RID: 7055
		private NetworkInstanceId ___ownerBodyObjectNetId;

		// Token: 0x04001B90 RID: 7056
		private NetworkInstanceId ___targetBodyObjectNetId;
	}
}
