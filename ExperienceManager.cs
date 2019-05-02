using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002EC RID: 748
	public class ExperienceManager : MonoBehaviour
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x0000BA79 File Offset: 0x00009C79
		// (set) Token: 0x06000F29 RID: 3881 RVA: 0x0000BA80 File Offset: 0x00009C80
		public static ExperienceManager instance { get; private set; }

		// Token: 0x06000F2A RID: 3882 RVA: 0x0000BA88 File Offset: 0x00009C88
		private static float CalcOrbTravelTime(float timeOffset)
		{
			return 0.5f + 1.5f * timeOffset;
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0000BA97 File Offset: 0x00009C97
		private void OnEnable()
		{
			if (ExperienceManager.instance && ExperienceManager.instance != this)
			{
				Debug.LogError("Only one ExperienceManager can exist at a time.");
				return;
			}
			ExperienceManager.instance = this;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0000BAC3 File Offset: 0x00009CC3
		private void OnDisable()
		{
			if (ExperienceManager.instance == this)
			{
				ExperienceManager.instance = null;
			}
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0000BAD8 File Offset: 0x00009CD8
		private void Start()
		{
			this.localTime = 0f;
			this.nextAward = float.PositiveInfinity;
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0005BEFC File Offset: 0x0005A0FC
		private void FixedUpdate()
		{
			this.localTime += Time.fixedDeltaTime;
			if (this.pendingAwards.Count > 0 && this.nextAward <= this.localTime)
			{
				this.nextAward = float.PositiveInfinity;
				for (int i = this.pendingAwards.Count - 1; i >= 0; i--)
				{
					if (this.pendingAwards[i].awardTime <= this.localTime)
					{
						if (TeamManager.instance)
						{
							TeamManager.instance.GiveTeamExperience(this.pendingAwards[i].recipient, this.pendingAwards[i].awardAmount);
						}
						this.pendingAwards.RemoveAt(i);
					}
					else if (this.pendingAwards[i].awardTime < this.nextAward)
					{
						this.nextAward = this.pendingAwards[i].awardTime;
					}
				}
			}
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0005BFF8 File Offset: 0x0005A1F8
		public void AwardExperience(Vector3 origin, CharacterBody body, ulong amount)
		{
			CharacterMaster master = body.master;
			if (!master)
			{
				return;
			}
			TeamIndex teamIndex = master.teamIndex;
			List<ulong> list = this.CalculateDenominations(amount);
			uint num = 0u;
			for (int i = 0; i < list.Count; i++)
			{
				this.AddPendingAward(this.localTime + 0.5f + 1.5f * ExperienceManager.orbTimeOffsetSequence[(int)num], teamIndex, list[i]);
				num += 1u;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0u;
				}
			}
			ExperienceManager.currentOutgoingCreateExpEffectMessage.awardAmount = amount;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.origin = origin;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.targetBody = body.gameObject;
			NetworkServer.SendToAll(55, ExperienceManager.currentOutgoingCreateExpEffectMessage);
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0005C0AC File Offset: 0x0005A2AC
		private void AddPendingAward(float awardTime, TeamIndex recipient, ulong awardAmount)
		{
			this.pendingAwards.Add(new ExperienceManager.TimedExpAward
			{
				awardTime = awardTime,
				recipient = recipient,
				awardAmount = awardAmount
			});
			if (this.nextAward > awardTime)
			{
				this.nextAward = awardTime;
			}
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0005C0F8 File Offset: 0x0005A2F8
		public List<ulong> CalculateDenominations(ulong total)
		{
			List<ulong> list = new List<ulong>();
			while (total > 0UL)
			{
				ulong num = (ulong)Math.Pow(6.0, (double)Mathf.Floor(Mathf.Log(total, 6f)));
				total = Math.Max(total - num, 0UL);
				list.Add(num);
			}
			return list;
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0000BAF0 File Offset: 0x00009CF0
		[NetworkMessageHandler(msgType = 55, client = true)]
		private static void HandleCreateExpEffect(NetworkMessage netMsg)
		{
			if (ExperienceManager.instance)
			{
				ExperienceManager.instance.HandleCreateExpEffectInternal(netMsg);
			}
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0005C14C File Offset: 0x0005A34C
		private void HandleCreateExpEffectInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ExperienceManager.CreateExpEffectMessage>(ExperienceManager.currentIncomingCreateExpEffectMessage);
			GameObject targetBody = ExperienceManager.currentIncomingCreateExpEffectMessage.targetBody;
			if (!targetBody)
			{
				return;
			}
			HurtBox hurtBox = Util.FindBodyMainHurtBox(targetBody);
			Transform targetTransform = ((hurtBox != null) ? hurtBox.transform : null) ?? targetBody.transform;
			List<ulong> list = this.CalculateDenominations(ExperienceManager.currentIncomingCreateExpEffectMessage.awardAmount);
			uint num = 0u;
			for (int i = 0; i < list.Count; i++)
			{
				ExperienceOrbBehavior component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ExpOrb"), ExperienceManager.currentIncomingCreateExpEffectMessage.origin, Quaternion.identity).GetComponent<ExperienceOrbBehavior>();
				component.targetTransform = targetTransform;
				component.travelTime = ExperienceManager.CalcOrbTravelTime(ExperienceManager.orbTimeOffsetSequence[(int)num]);
				component.exp = list[i];
				num += 1u;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0u;
				}
			}
		}

		// Token: 0x0400132B RID: 4907
		private float localTime;

		// Token: 0x0400132C RID: 4908
		private List<ExperienceManager.TimedExpAward> pendingAwards = new List<ExperienceManager.TimedExpAward>();

		// Token: 0x0400132D RID: 4909
		private float nextAward;

		// Token: 0x0400132E RID: 4910
		private const float minOrbTravelTime = 0.5f;

		// Token: 0x0400132F RID: 4911
		public const float maxOrbTravelTime = 2f;

		// Token: 0x04001330 RID: 4912
		public static readonly float[] orbTimeOffsetSequence = new float[]
		{
			0.841f,
			0.394f,
			0.783f,
			0.799f,
			0.912f,
			0.197f,
			0.335f,
			0.768f,
			0.278f,
			0.554f,
			0.477f,
			0.629f,
			0.365f,
			0.513f,
			0.953f,
			0.917f
		};

		// Token: 0x04001331 RID: 4913
		private static ExperienceManager.CreateExpEffectMessage currentOutgoingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x04001332 RID: 4914
		private static ExperienceManager.CreateExpEffectMessage currentIncomingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x020002ED RID: 749
		[Serializable]
		private struct TimedExpAward
		{
			// Token: 0x04001333 RID: 4915
			public float awardTime;

			// Token: 0x04001334 RID: 4916
			public ulong awardAmount;

			// Token: 0x04001335 RID: 4917
			public TeamIndex recipient;
		}

		// Token: 0x020002EE RID: 750
		private class CreateExpEffectMessage : MessageBase
		{
			// Token: 0x06000F37 RID: 3895 RVA: 0x0000BB49 File Offset: 0x00009D49
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.origin);
				writer.Write(this.targetBody);
				writer.WritePackedUInt64(this.awardAmount);
			}

			// Token: 0x06000F38 RID: 3896 RVA: 0x0000BB6F File Offset: 0x00009D6F
			public override void Deserialize(NetworkReader reader)
			{
				this.origin = reader.ReadVector3();
				this.targetBody = reader.ReadGameObject();
				this.awardAmount = reader.ReadPackedUInt64();
			}

			// Token: 0x04001336 RID: 4918
			public Vector3 origin;

			// Token: 0x04001337 RID: 4919
			public GameObject targetBody;

			// Token: 0x04001338 RID: 4920
			public ulong awardAmount;
		}
	}
}
