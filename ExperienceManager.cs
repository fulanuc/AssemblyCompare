using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002EF RID: 751
	public class ExperienceManager : MonoBehaviour
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x0000BB27 File Offset: 0x00009D27
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x0000BB2E File Offset: 0x00009D2E
		public static ExperienceManager instance { get; private set; }

		// Token: 0x06000F3A RID: 3898 RVA: 0x0000BB36 File Offset: 0x00009D36
		private static float CalcOrbTravelTime(float timeOffset)
		{
			return 0.5f + 1.5f * timeOffset;
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0000BB45 File Offset: 0x00009D45
		private void OnEnable()
		{
			if (ExperienceManager.instance && ExperienceManager.instance != this)
			{
				Debug.LogError("Only one ExperienceManager can exist at a time.");
				return;
			}
			ExperienceManager.instance = this;
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0000BB71 File Offset: 0x00009D71
		private void OnDisable()
		{
			if (ExperienceManager.instance == this)
			{
				ExperienceManager.instance = null;
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0000BB86 File Offset: 0x00009D86
		private void Start()
		{
			this.localTime = 0f;
			this.nextAward = float.PositiveInfinity;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0005C11C File Offset: 0x0005A31C
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

		// Token: 0x06000F3F RID: 3903 RVA: 0x0005C218 File Offset: 0x0005A418
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

		// Token: 0x06000F40 RID: 3904 RVA: 0x0005C2CC File Offset: 0x0005A4CC
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

		// Token: 0x06000F41 RID: 3905 RVA: 0x0005C318 File Offset: 0x0005A518
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

		// Token: 0x06000F42 RID: 3906 RVA: 0x0000BB9E File Offset: 0x00009D9E
		[NetworkMessageHandler(msgType = 55, client = true)]
		private static void HandleCreateExpEffect(NetworkMessage netMsg)
		{
			if (ExperienceManager.instance)
			{
				ExperienceManager.instance.HandleCreateExpEffectInternal(netMsg);
			}
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0005C36C File Offset: 0x0005A56C
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

		// Token: 0x04001342 RID: 4930
		private float localTime;

		// Token: 0x04001343 RID: 4931
		private List<ExperienceManager.TimedExpAward> pendingAwards = new List<ExperienceManager.TimedExpAward>();

		// Token: 0x04001344 RID: 4932
		private float nextAward;

		// Token: 0x04001345 RID: 4933
		private const float minOrbTravelTime = 0.5f;

		// Token: 0x04001346 RID: 4934
		public const float maxOrbTravelTime = 2f;

		// Token: 0x04001347 RID: 4935
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

		// Token: 0x04001348 RID: 4936
		private static ExperienceManager.CreateExpEffectMessage currentOutgoingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x04001349 RID: 4937
		private static ExperienceManager.CreateExpEffectMessage currentIncomingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x020002F0 RID: 752
		[Serializable]
		private struct TimedExpAward
		{
			// Token: 0x0400134A RID: 4938
			public float awardTime;

			// Token: 0x0400134B RID: 4939
			public ulong awardAmount;

			// Token: 0x0400134C RID: 4940
			public TeamIndex recipient;
		}

		// Token: 0x020002F1 RID: 753
		private class CreateExpEffectMessage : MessageBase
		{
			// Token: 0x06000F47 RID: 3911 RVA: 0x0000BBF7 File Offset: 0x00009DF7
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.origin);
				writer.Write(this.targetBody);
				writer.WritePackedUInt64(this.awardAmount);
			}

			// Token: 0x06000F48 RID: 3912 RVA: 0x0000BC1D File Offset: 0x00009E1D
			public override void Deserialize(NetworkReader reader)
			{
				this.origin = reader.ReadVector3();
				this.targetBody = reader.ReadGameObject();
				this.awardAmount = reader.ReadPackedUInt64();
			}

			// Token: 0x0400134D RID: 4941
			public Vector3 origin;

			// Token: 0x0400134E RID: 4942
			public GameObject targetBody;

			// Token: 0x0400134F RID: 4943
			public ulong awardAmount;
		}
	}
}
