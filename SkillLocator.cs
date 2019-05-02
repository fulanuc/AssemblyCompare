using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003EA RID: 1002
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillLocator : MonoBehaviour
	{
		// Token: 0x06001602 RID: 5634 RVA: 0x00010953 File Offset: 0x0000EB53
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.allSkills = base.GetComponents<GenericSkill>();
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x0007503C File Offset: 0x0007323C
		public GenericSkill FindSkill(string skillName)
		{
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				if (this.allSkills[i].skillName == skillName)
				{
					return this.allSkills[i];
				}
			}
			return null;
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x0001096D File Offset: 0x0000EB6D
		public GenericSkill GetSkill(SkillSlot skillSlot)
		{
			switch (skillSlot)
			{
			case SkillSlot.Primary:
				return this.primary;
			case SkillSlot.Secondary:
				return this.secondary;
			case SkillSlot.Utility:
				return this.utility;
			case SkillSlot.Special:
				return this.special;
			default:
				return null;
			}
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0007507C File Offset: 0x0007327C
		public SkillSlot FindSkillSlot(GenericSkill skillComponent)
		{
			if (!skillComponent)
			{
				return SkillSlot.None;
			}
			if (skillComponent == this.primary)
			{
				return SkillSlot.Primary;
			}
			if (skillComponent == this.secondary)
			{
				return SkillSlot.Secondary;
			}
			if (skillComponent == this.utility)
			{
				return SkillSlot.Utility;
			}
			if (skillComponent == this.special)
			{
				return SkillSlot.Special;
			}
			return SkillSlot.None;
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x000750D4 File Offset: 0x000732D4
		public void ResetSkills()
		{
			if (NetworkServer.active && this.networkIdentity.clientAuthorityOwner != null)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(56);
				networkWriter.Write(base.gameObject);
				networkWriter.FinishMessage();
				this.networkIdentity.clientAuthorityOwner.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			}
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				this.allSkills[i].Reset();
			}
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00075154 File Offset: 0x00073354
		public void ApplyAmmoPack()
		{
			if (NetworkServer.active && !this.networkIdentity.hasAuthority)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(63);
				networkWriter.Write(base.gameObject);
				networkWriter.FinishMessage();
				NetworkConnection clientAuthorityOwner = this.networkIdentity.clientAuthorityOwner;
				if (clientAuthorityOwner != null)
				{
					clientAuthorityOwner.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
					return;
				}
			}
			else
			{
				GenericSkill[] array = new GenericSkill[]
				{
					this.primary,
					this.secondary,
					this.utility,
					this.special
				};
				Util.ShuffleArray<GenericSkill>(array);
				foreach (GenericSkill genericSkill in array)
				{
					if (genericSkill && genericSkill.CanApplyAmmoPack())
					{
						Debug.LogFormat("Resetting skill {0}", new object[]
						{
							genericSkill.skillName
						});
						genericSkill.AddOneStock();
					}
				}
			}
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00075234 File Offset: 0x00073434
		[NetworkMessageHandler(msgType = 56, client = true)]
		private static void HandleResetSkills(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!NetworkServer.active && gameObject)
			{
				SkillLocator component = gameObject.GetComponent<SkillLocator>();
				if (component)
				{
					component.ResetSkills();
				}
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00075274 File Offset: 0x00073474
		[NetworkMessageHandler(msgType = 63, client = true)]
		private static void HandleAmmoPackPickup(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!NetworkServer.active && gameObject)
			{
				SkillLocator component = gameObject.GetComponent<SkillLocator>();
				if (component)
				{
					component.ApplyAmmoPack();
				}
			}
		}

		// Token: 0x04001937 RID: 6455
		[FormerlySerializedAs("skill1")]
		public GenericSkill primary;

		// Token: 0x04001938 RID: 6456
		[FormerlySerializedAs("skill2")]
		public GenericSkill secondary;

		// Token: 0x04001939 RID: 6457
		[FormerlySerializedAs("skill3")]
		public GenericSkill utility;

		// Token: 0x0400193A RID: 6458
		[FormerlySerializedAs("skill4")]
		public GenericSkill special;

		// Token: 0x0400193B RID: 6459
		public SkillLocator.PassiveSkill passiveSkill;

		// Token: 0x0400193C RID: 6460
		private NetworkIdentity networkIdentity;

		// Token: 0x0400193D RID: 6461
		private GenericSkill[] allSkills;

		// Token: 0x020003EB RID: 1003
		[Serializable]
		public struct PassiveSkill
		{
			// Token: 0x0400193E RID: 6462
			public bool enabled;

			// Token: 0x0400193F RID: 6463
			public string skillNameToken;

			// Token: 0x04001940 RID: 6464
			public string skillDescriptionToken;

			// Token: 0x04001941 RID: 6465
			public Sprite icon;
		}
	}
}
