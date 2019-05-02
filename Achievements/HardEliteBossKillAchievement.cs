using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x0200069D RID: 1693
	[RegisterAchievement("HardEliteBossKill", "Items.KillEliteFrenzy", null, typeof(HardEliteBossKillAchievement.EliteBossKillServerAchievement))]
	internal class HardEliteBossKillAchievement : BaseAchievement
	{
		// Token: 0x060025B6 RID: 9654 RVA: 0x0001B857 File Offset: 0x00019A57
		public override void OnInstall()
		{
			base.OnInstall();
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStart;
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x060025B7 RID: 9655 RVA: 0x0001B881 File Offset: 0x00019A81
		public override void OnUninstall()
		{
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStart;
			Run.onRunStartGlobal -= this.OnRunStart;
			base.OnUninstall();
		}

		// Token: 0x060025B8 RID: 9656 RVA: 0x0001B8AB File Offset: 0x00019AAB
		private void UpdateTracking()
		{
			base.SetServerTracked(Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Hard);
		}

		// Token: 0x060025B9 RID: 9657 RVA: 0x0001B8D2 File Offset: 0x00019AD2
		private void OnPostNetworkUserStart(NetworkUser networkUser)
		{
			this.UpdateTracking();
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x0001B8D2 File Offset: 0x00019AD2
		private void OnRunStart(Run run)
		{
			this.UpdateTracking();
		}

		// Token: 0x0200069E RID: 1694
		private class EliteBossKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025BC RID: 9660 RVA: 0x0001B8DA File Offset: 0x00019ADA
			public override void OnInstall()
			{
				base.OnInstall();
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Add(this);
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal += HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
			}

			// Token: 0x060025BD RID: 9661 RVA: 0x0001B90B File Offset: 0x00019B0B
			public override void OnUninstall()
			{
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal -= HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Remove(this);
				base.OnUninstall();
			}

			// Token: 0x060025BE RID: 9662 RVA: 0x000B0778 File Offset: 0x000AE978
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

			// Token: 0x04002849 RID: 10313
			private static readonly List<HardEliteBossKillAchievement.EliteBossKillServerAchievement> instancesList = new List<HardEliteBossKillAchievement.EliteBossKillServerAchievement>();
		}
	}
}
