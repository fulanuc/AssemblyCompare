using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006AF RID: 1711
	[RegisterAchievement("HardEliteBossKill", "Items.KillEliteFrenzy", null, typeof(HardEliteBossKillAchievement.EliteBossKillServerAchievement))]
	internal class HardEliteBossKillAchievement : BaseAchievement
	{
		// Token: 0x0600264D RID: 9805 RVA: 0x0001BF92 File Offset: 0x0001A192
		public override void OnInstall()
		{
			base.OnInstall();
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStart;
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x0001BFBC File Offset: 0x0001A1BC
		public override void OnUninstall()
		{
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStart;
			Run.onRunStartGlobal -= this.OnRunStart;
			base.OnUninstall();
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x0001BFE6 File Offset: 0x0001A1E6
		private void UpdateTracking()
		{
			base.SetServerTracked(Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Hard);
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x0001C00D File Offset: 0x0001A20D
		private void OnPostNetworkUserStart(NetworkUser networkUser)
		{
			this.UpdateTracking();
		}

		// Token: 0x06002651 RID: 9809 RVA: 0x0001C00D File Offset: 0x0001A20D
		private void OnRunStart(Run run)
		{
			this.UpdateTracking();
		}

		// Token: 0x020006B0 RID: 1712
		private class EliteBossKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002653 RID: 9811 RVA: 0x0001C015 File Offset: 0x0001A215
			public override void OnInstall()
			{
				base.OnInstall();
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Add(this);
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal += HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
			}

			// Token: 0x06002654 RID: 9812 RVA: 0x0001C046 File Offset: 0x0001A246
			public override void OnUninstall()
			{
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal -= HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Remove(this);
				base.OnUninstall();
			}

			// Token: 0x06002655 RID: 9813 RVA: 0x000B1E70 File Offset: 0x000B0070
			private static void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victim)
				{
					return;
				}
				CharacterBody component = damageReport.victim.GetComponent<CharacterBody>();
				if (!component || !component.isChampion || !component.isElite)
				{
					return;
				}
				foreach (HardEliteBossKillAchievement.EliteBossKillServerAchievement eliteBossKillServerAchievement in HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList)
				{
					GameObject masterObject = eliteBossKillServerAchievement.serverAchievementTracker.networkUser.masterObject;
					if (masterObject)
					{
						CharacterMaster component2 = masterObject.GetComponent<CharacterMaster>();
						if (component2)
						{
							CharacterBody body = component2.GetBody();
							if (body && body.healthComponent && body.healthComponent.alive)
							{
								eliteBossKillServerAchievement.Grant();
							}
						}
					}
				}
			}

			// Token: 0x040028A5 RID: 10405
			private static readonly List<HardEliteBossKillAchievement.EliteBossKillServerAchievement> instancesList = new List<HardEliteBossKillAchievement.EliteBossKillServerAchievement>();
		}
	}
}
