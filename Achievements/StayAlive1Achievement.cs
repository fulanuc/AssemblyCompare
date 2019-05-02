using System;
using System.Linq;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006C4 RID: 1732
	[RegisterAchievement("StayAlive1", "Items.ExtraLife", null, null)]
	public class StayAlive1Achievement : BaseAchievement
	{
		// Token: 0x060026A1 RID: 9889 RVA: 0x0001C45A File Offset: 0x0001A65A
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.Check;
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x0001C473 File Offset: 0x0001A673
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x000B229C File Offset: 0x000B049C
		private void Check()
		{
			NetworkUser networkUser = NetworkUser.readOnlyLocalPlayersList.FirstOrDefault((NetworkUser v) => v.localUser == this.localUser);
			if (networkUser)
			{
				GameObject masterObject = networkUser.masterObject;
				if (masterObject)
				{
					CharacterMaster component = masterObject.GetComponent<CharacterMaster>();
					if (component && component.currentLifeStopwatch >= 1800f)
					{
						base.Grant();
					}
				}
			}
		}

		// Token: 0x040028B1 RID: 10417
		private const float requirement = 1800f;
	}
}
