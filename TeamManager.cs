using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FD RID: 1021
	public class TeamManager : NetworkBehaviour
	{
		// Token: 0x06001695 RID: 5781 RVA: 0x00077294 File Offset: 0x00075494
		static TeamManager()
		{
			List<ulong> list = new List<ulong>();
			list.Add(0UL);
			list.Add(0UL);
			TeamManager.naturalLevelCap = 2u;
			for (;;)
			{
				ulong num = (ulong)TeamManager.InitialCalcExperience(TeamManager.naturalLevelCap, 20.0, 1.55);
				if (num <= list[list.Count - 1])
				{
					break;
				}
				list.Add(num);
				TeamManager.naturalLevelCap += 1u;
			}
			TeamManager.naturalLevelCap -= 1u;
			TeamManager.levelToExperienceTable = list.ToArray();
			TeamManager.hardExpCap = TeamManager.levelToExperienceTable[TeamManager.levelToExperienceTable.Length - 1];
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x00010E7A File Offset: 0x0000F07A
		private static double InitialCalcLevel(double experience, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(Math.Log(1.0 - experience / experienceForFirstLevelUp * (1.0 - growthRate), growthRate) + 1.0, 1.0);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x00010EB3 File Offset: 0x0000F0B3
		private static double InitialCalcExperience(double level, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(experienceForFirstLevelUp * ((1.0 - Math.Pow(growthRate, level - 1.0)) / (1.0 - growthRate)), 0.0);
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00077330 File Offset: 0x00075530
		private static uint FindLevelForExperience(ulong experience)
		{
			uint num = 1u;
			while ((ulong)num < (ulong)((long)TeamManager.levelToExperienceTable.Length))
			{
				if (TeamManager.levelToExperienceTable[(int)num] > experience)
				{
					return num - 1u;
				}
				num += 1u;
			}
			return TeamManager.naturalLevelCap;
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00010EEC File Offset: 0x0000F0EC
		private static ulong GetExperienceForLevel(uint level)
		{
			if (level > TeamManager.naturalLevelCap)
			{
				level = TeamManager.naturalLevelCap;
			}
			return TeamManager.levelToExperienceTable[(int)level];
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x0600169B RID: 5787 RVA: 0x00010F0C File Offset: 0x0000F10C
		// (set) Token: 0x0600169A RID: 5786 RVA: 0x00010F04 File Offset: 0x0000F104
		public static TeamManager instance { get; private set; }

		// Token: 0x0600169C RID: 5788 RVA: 0x00010F13 File Offset: 0x0000F113
		private void OnEnable()
		{
			if (TeamManager.instance)
			{
				Debug.LogErrorFormat(this, "Only one {0} may exist at a time.", new object[]
				{
					typeof(TeamManager).Name
				});
				return;
			}
			TeamManager.instance = this;
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x00010F4B File Offset: 0x0000F14B
		private void OnDisable()
		{
			if (TeamManager.instance == this)
			{
				TeamManager.instance = null;
			}
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00077364 File Offset: 0x00075564
		private void Start()
		{
			if (NetworkServer.active)
			{
				for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
				{
					this.SetTeamExperience(teamIndex, 0UL);
				}
			}
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00077390 File Offset: 0x00075590
		[Server]
		public void GiveTeamMoney(TeamIndex teamIndex, uint money)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeamManager::GiveTeamMoney(RoR2.TeamIndex,System.UInt32)' called on client");
				return;
			}
			int num = Run.instance ? Run.instance.livingPlayerCount : 0;
			if (num != 0)
			{
				money = (uint)Mathf.CeilToInt(money / (float)num);
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
				if (component && component.isPlayerControlled)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.GiveMoney(money);
					}
				}
			}
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0007742C File Offset: 0x0007562C
		[Server]
		public void GiveTeamExperience(TeamIndex teamIndex, ulong experience)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeamManager::GiveTeamExperience(RoR2.TeamIndex,System.UInt64)' called on client");
				return;
			}
			ulong num = this.teamExperience[(int)teamIndex];
			ulong num2 = num + experience;
			if (num2 < num)
			{
				num2 = ulong.MaxValue;
			}
			this.SetTeamExperience(teamIndex, num2);
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0007746C File Offset: 0x0007566C
		private void SetTeamExperience(TeamIndex teamIndex, ulong newExperience)
		{
			if (newExperience > TeamManager.hardExpCap)
			{
				newExperience = TeamManager.hardExpCap;
			}
			this.teamExperience[(int)teamIndex] = newExperience;
			uint num = this.teamLevels[(int)teamIndex];
			uint num2 = TeamManager.FindLevelForExperience(newExperience);
			if (num != num2)
			{
				ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
				for (int i = 0; i < teamMembers.Count; i++)
				{
					CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
					if (component)
					{
						component.OnLevelChanged();
					}
				}
				this.teamLevels[(int)teamIndex] = num2;
				this.teamCurrentLevelExperience[(int)teamIndex] = TeamManager.GetExperienceForLevel(num2);
				this.teamNextLevelExperience[(int)teamIndex] = TeamManager.GetExperienceForLevel(num2 + 1u);
				if (num < num2)
				{
					GlobalEventManager.OnTeamLevelUp(teamIndex);
				}
			}
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1u << (int)teamIndex);
			}
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00077520 File Offset: 0x00075720
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = initialState ? 7u : base.syncVarDirtyBits;
			writer.WritePackedUInt32(num);
			if (num == 0u)
			{
				return false;
			}
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1u << (int)teamIndex) != 0u)
				{
					writer.WritePackedUInt64(this.teamExperience[(int)teamIndex]);
				}
			}
			return true;
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x0007756C File Offset: 0x0007576C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			uint num = reader.ReadPackedUInt32();
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1u << (int)teamIndex) != 0u)
				{
					ulong newExperience = reader.ReadPackedUInt64();
					this.SetTeamExperience(teamIndex, newExperience);
				}
			}
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x00010F60 File Offset: 0x0000F160
		public ulong GetTeamExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamExperience[(int)teamIndex];
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x00010F75 File Offset: 0x0000F175
		public ulong GetTeamCurrentLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamCurrentLevelExperience[(int)teamIndex];
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x00010F8A File Offset: 0x0000F18A
		public ulong GetTeamNextLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamNextLevelExperience[(int)teamIndex];
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00010F9F File Offset: 0x0000F19F
		public uint GetTeamLevel(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0u;
			}
			return this.teamLevels[(int)teamIndex];
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00010FB3 File Offset: 0x0000F1B3
		public void SetTeamLevel(TeamIndex teamIndex, uint newLevel)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			if (this.GetTeamLevel(teamIndex) == newLevel)
			{
				return;
			}
			this.SetTeamExperience(teamIndex, TeamManager.GetExperienceForLevel(newLevel));
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x00010FD6 File Offset: 0x0000F1D6
		public static GameObject GetTeamLevelUpEffect(TeamIndex teamIndex)
		{
			switch (teamIndex)
			{
			case TeamIndex.Neutral:
				return null;
			case TeamIndex.Player:
				return Resources.Load<GameObject>("Prefabs/Effects/LevelUpEffect");
			case TeamIndex.Monster:
				return Resources.Load<GameObject>("Prefabs/Effects/LevelUpEffectEnemy");
			default:
				return null;
			}
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00011005 File Offset: 0x0000F205
		public static bool IsTeamEnemy(TeamIndex teamA, TeamIndex teamB)
		{
			return teamA != teamB;
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x040019BB RID: 6587
		public static readonly uint naturalLevelCap;

		// Token: 0x040019BC RID: 6588
		private static readonly ulong[] levelToExperienceTable;

		// Token: 0x040019BD RID: 6589
		public static readonly ulong hardExpCap;

		// Token: 0x040019BF RID: 6591
		private ulong[] teamExperience = new ulong[3];

		// Token: 0x040019C0 RID: 6592
		private uint[] teamLevels = new uint[3];

		// Token: 0x040019C1 RID: 6593
		private ulong[] teamCurrentLevelExperience = new ulong[3];

		// Token: 0x040019C2 RID: 6594
		private ulong[] teamNextLevelExperience = new ulong[3];

		// Token: 0x040019C3 RID: 6595
		private const uint teamExperienceDirtyBitsMask = 7u;
	}
}
