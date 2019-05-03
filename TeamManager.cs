using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F7 RID: 1015
	public class TeamManager : NetworkBehaviour
	{
		// Token: 0x06001656 RID: 5718 RVA: 0x00076D04 File Offset: 0x00074F04
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

		// Token: 0x06001657 RID: 5719 RVA: 0x00010A61 File Offset: 0x0000EC61
		private static double InitialCalcLevel(double experience, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(Math.Log(1.0 - experience / experienceForFirstLevelUp * (1.0 - growthRate), growthRate) + 1.0, 1.0);
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00010A9A File Offset: 0x0000EC9A
		private static double InitialCalcExperience(double level, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(experienceForFirstLevelUp * ((1.0 - Math.Pow(growthRate, level - 1.0)) / (1.0 - growthRate)), 0.0);
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x00076DA0 File Offset: 0x00074FA0
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

		// Token: 0x0600165A RID: 5722 RVA: 0x00010AD3 File Offset: 0x0000ECD3
		private static ulong GetExperienceForLevel(uint level)
		{
			if (level > TeamManager.naturalLevelCap)
			{
				level = TeamManager.naturalLevelCap;
			}
			return TeamManager.levelToExperienceTable[(int)level];
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x0600165C RID: 5724 RVA: 0x00010AF3 File Offset: 0x0000ECF3
		// (set) Token: 0x0600165B RID: 5723 RVA: 0x00010AEB File Offset: 0x0000ECEB
		public static TeamManager instance { get; private set; }

		// Token: 0x0600165D RID: 5725 RVA: 0x00010AFA File Offset: 0x0000ECFA
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

		// Token: 0x0600165E RID: 5726 RVA: 0x00010B32 File Offset: 0x0000ED32
		private void OnDisable()
		{
			if (TeamManager.instance == this)
			{
				TeamManager.instance = null;
			}
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x00076DD4 File Offset: 0x00074FD4
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

		// Token: 0x06001660 RID: 5728 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00076E00 File Offset: 0x00075000
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

		// Token: 0x06001662 RID: 5730 RVA: 0x00076E9C File Offset: 0x0007509C
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

		// Token: 0x06001663 RID: 5731 RVA: 0x00076EDC File Offset: 0x000750DC
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

		// Token: 0x06001664 RID: 5732 RVA: 0x00076F90 File Offset: 0x00075190
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

		// Token: 0x06001665 RID: 5733 RVA: 0x00076FDC File Offset: 0x000751DC
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

		// Token: 0x06001666 RID: 5734 RVA: 0x00010B47 File Offset: 0x0000ED47
		public ulong GetTeamExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamExperience[(int)teamIndex];
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x00010B5C File Offset: 0x0000ED5C
		public ulong GetTeamCurrentLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamCurrentLevelExperience[(int)teamIndex];
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x00010B71 File Offset: 0x0000ED71
		public ulong GetTeamNextLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamNextLevelExperience[(int)teamIndex];
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00010B86 File Offset: 0x0000ED86
		public uint GetTeamLevel(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0u;
			}
			return this.teamLevels[(int)teamIndex];
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00010B9A File Offset: 0x0000ED9A
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

		// Token: 0x0600166B RID: 5739 RVA: 0x00010BBD File Offset: 0x0000EDBD
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

		// Token: 0x0600166C RID: 5740 RVA: 0x00010BEC File Offset: 0x0000EDEC
		public static bool IsTeamEnemy(TeamIndex teamA, TeamIndex teamB)
		{
			return teamA != teamB;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x04001992 RID: 6546
		public static readonly uint naturalLevelCap;

		// Token: 0x04001993 RID: 6547
		private static readonly ulong[] levelToExperienceTable;

		// Token: 0x04001994 RID: 6548
		public static readonly ulong hardExpCap;

		// Token: 0x04001996 RID: 6550
		private ulong[] teamExperience = new ulong[3];

		// Token: 0x04001997 RID: 6551
		private uint[] teamLevels = new uint[3];

		// Token: 0x04001998 RID: 6552
		private ulong[] teamCurrentLevelExperience = new ulong[3];

		// Token: 0x04001999 RID: 6553
		private ulong[] teamNextLevelExperience = new ulong[3];

		// Token: 0x0400199A RID: 6554
		private const uint teamExperienceDirtyBitsMask = 7u;
	}
}
