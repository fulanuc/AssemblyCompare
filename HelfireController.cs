using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200030D RID: 781
	[RequireComponent(typeof(NetworkedBodyAttachment))]
	public class HelfireController : NetworkBehaviour
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x0000C6C3 File Offset: 0x0000A8C3
		// (set) Token: 0x0600103F RID: 4159 RVA: 0x0000C6CB File Offset: 0x0000A8CB
		public NetworkedBodyAttachment networkedBodyAttachment { get; private set; }

		// Token: 0x06001040 RID: 4160 RVA: 0x0000C6D4 File Offset: 0x0000A8D4
		private void Awake()
		{
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			this.auraEffectTransform.SetParent(null);
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x0000C6EE File Offset: 0x0000A8EE
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

		// Token: 0x06001042 RID: 4162 RVA: 0x0000C72D File Offset: 0x0000A92D
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.bullseyeSearch = new BullseyeSearch
				{
					teamMaskFilter = TeamMask.all
				};
			}
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x00061D2C File Offset: 0x0005FF2C
		private void FixedUpdate()
		{
			this.radius = this.baseRadius * (1f + (float)(this.stack - 1) * 0.5f);
			if (NetworkServer.active)
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					float damageMultiplier = 1f + (float)(this.stack - 1) * 0.5f;
					this.timer = this.interval;
					this.bullseyeSearch.searchOrigin = base.transform.position;
					this.bullseyeSearch.maxDistanceFilter = this.radius;
					this.bullseyeSearch.RefreshCandidates();
					foreach (GameObject victimObject in (from hurtBox in this.bullseyeSearch.GetResults()
					where hurtBox.healthComponent
					select hurtBox.healthComponent.gameObject).Distinct<GameObject>())
					{
						DotController.InflictDot(victimObject, this.networkedBodyAttachment.attachedBodyObject, DotController.DotIndex.Helfire, this.dotDuration, damageMultiplier);
					}
				}
			}
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00061E80 File Offset: 0x00060080
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

		// Token: 0x06001046 RID: 4166 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x00061F04 File Offset: 0x00060104
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x0000C75B File Offset: 0x0000A95B
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

		// Token: 0x06001049 RID: 4169 RVA: 0x00061F18 File Offset: 0x00060118
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

		// Token: 0x0600104A RID: 4170 RVA: 0x00061F84 File Offset: 0x00060184
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

		// Token: 0x04001423 RID: 5155
		[SyncVar]
		public int stack = 1;

		// Token: 0x04001424 RID: 5156
		[FormerlySerializedAs("radius")]
		public float baseRadius;

		// Token: 0x04001425 RID: 5157
		public float dotDuration;

		// Token: 0x04001426 RID: 5158
		public float interval;

		// Token: 0x04001427 RID: 5159
		public Transform auraEffectTransform;

		// Token: 0x04001428 RID: 5160
		private float timer;

		// Token: 0x04001429 RID: 5161
		private float radius;

		// Token: 0x0400142B RID: 5163
		private BullseyeSearch bullseyeSearch;

		// Token: 0x0400142C RID: 5164
		private CameraTargetParams cameraTargetParams;
	}
}
