using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000310 RID: 784
	[RequireComponent(typeof(NetworkedBodyAttachment))]
	public class HelfireController : NetworkBehaviour
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x0000C7BB File Offset: 0x0000A9BB
		// (set) Token: 0x06001056 RID: 4182 RVA: 0x0000C7C3 File Offset: 0x0000A9C3
		public NetworkedBodyAttachment networkedBodyAttachment { get; private set; }

		// Token: 0x06001057 RID: 4183 RVA: 0x0000C7CC File Offset: 0x0000A9CC
		private void Awake()
		{
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			this.auraEffectTransform.SetParent(null);
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0000C7E6 File Offset: 0x0000A9E6
		private void OnDestroy()
		{
			if (this.auraEffectTransform)
			{
				UnityEngine.Object.Destroy(this.auraEffectTransform.gameObject);
				this.auraEffectTransform = null;
			}
			if (this.cameraTargetParams)
			{
				this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0000C825 File Offset: 0x0000AA25
		private void FixedUpdate()
		{
			this.radius = this.baseRadius * (1f + (float)(this.stack - 1) * 0.5f);
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0006200C File Offset: 0x0006020C
		private void ServerFixedUpdate()
		{
			this.timer -= Time.fixedDeltaTime;
			if (this.timer <= 0f)
			{
				float damageMultiplier = 1f + (float)(this.stack - 1) * 0.5f;
				this.timer = this.interval;
				Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Collide);
				GameObject[] array2 = new GameObject[array.Length];
				int count = 0;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = HelfireController.<ServerFixedUpdate>g__HurtBoxColliderToBodyObject|15_0(array[i]);
					if (gameObject && Array.IndexOf<GameObject>(array2, gameObject, 0, count) == -1)
					{
						DotController.InflictDot(gameObject, this.networkedBodyAttachment.attachedBodyObject, DotController.DotIndex.Helfire, this.dotDuration, damageMultiplier);
						array2[count++] = gameObject;
					}
				}
			}
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x000620EC File Offset: 0x000602EC
		private void LateUpdate()
		{
			CharacterBody attachedBody = this.networkedBodyAttachment.attachedBody;
			if (attachedBody)
			{
				this.auraEffectTransform.position = this.networkedBodyAttachment.attachedBody.corePosition;
				this.auraEffectTransform.localScale = new Vector3(this.radius, this.radius, this.radius);
				if (!this.cameraTargetParams)
				{
					this.cameraTargetParams = attachedBody.GetComponent<CameraTargetParams>();
					return;
				}
				this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600105F RID: 4191 RVA: 0x000621A8 File Offset: 0x000603A8
		// (set) Token: 0x06001060 RID: 4192 RVA: 0x0000C865 File Offset: 0x0000AA65
		public int Networkstack
		{
			get
			{
				return this.stack;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.stack, 1u);
			}
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x000621BC File Offset: 0x000603BC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.stack);
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
				writer.WritePackedUInt32((uint)this.stack);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x00062228 File Offset: 0x00060428
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.stack = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.stack = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x0400143B RID: 5179
		[SyncVar]
		public int stack = 1;

		// Token: 0x0400143C RID: 5180
		[FormerlySerializedAs("radius")]
		public float baseRadius;

		// Token: 0x0400143D RID: 5181
		public float dotDuration;

		// Token: 0x0400143E RID: 5182
		public float interval;

		// Token: 0x0400143F RID: 5183
		public Transform auraEffectTransform;

		// Token: 0x04001440 RID: 5184
		private float timer;

		// Token: 0x04001441 RID: 5185
		private float radius;

		// Token: 0x04001443 RID: 5187
		private CameraTargetParams cameraTargetParams;
	}
}
