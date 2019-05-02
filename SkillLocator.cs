using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003E4 RID: 996
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillLocator : MonoBehaviour
	{
		// Token: 0x060015C5 RID: 5573 RVA: 0x0001054A File Offset: 0x0000E74A
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.allSkills = base.GetComponents<GenericSkill>();
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x00074A04 File Offset: 0x00072C04
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

		// Token: 0x060015C7 RID: 5575 RVA: 0x00010564 File Offset: 0x0000E764
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

		// Token: 0x060015C8 RID: 5576 RVA: 0x00074A44 File Offset: 0x00072C44
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

		// Token: 0x060015C9 RID: 5577 RVA: 0x00074A9C File Offset: 0x00072C9C
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

		// Token: 0x060015CA RID: 5578 RVA: 0x00074B1C File Offset: 0x00072D1C
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

		// Token: 0x060015CB RID: 5579 RVA: 0x00074BFC File Offset: 0x00072DFC
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

		// Token: 0x060015CC RID: 5580 RVA: 0x00074C3C File Offset: 0x00072E3C
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

		// Token: 0x0400190E RID: 6414
		[FormerlySerializedAs("skill1")]
		public GenericSkill primary;

		// Token: 0x0400190F RID: 6415
		[FormerlySerializedAs("skill2")]
		public GenericSkill secondary;

		// Token: 0x04001910 RID: 6416
		[FormerlySerializedAs("skill3")]
		public GenericSkill utility;

		// Token: 0x04001911 RID: 6417
		[FormerlySerializedAs("skill4")]
		public GenericSkill special;

		// Token: 0x04001912 RID: 6418
		public SkillLocator.PassiveSkill passiveSkill;

		// Token: 0x04001913 RID: 6419
		private NetworkIdentity networkIdentity;

		// Token: 0x04001914 RID: 6420
		private GenericSkill[] allSkills;

		// Token: 0x020003E5 RID: 997
		[Serializable]
		public struct PassiveSkill
		{
			// Token: 0x04001915 RID: 6421
			public bool enabled;

			// Token: 0x04001916 RID: 6422
			public string skillNameToken;

			// Token: 0x04001917 RID: 6423
			public string skillDescriptionToken;

			// Token: 0x04001918 RID: 6424
			public Sprite icon;
		}
	}
}
