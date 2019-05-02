using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000360 RID: 864
	public class MonsterCounter : MonoBehaviour
	{
		// Token: 0x060011CE RID: 4558 RVA: 0x0000D94A File Offset: 0x0000BB4A
		private int CountEnemies()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x0000D957 File Offset: 0x0000BB57
		private void Update()
		{
			this.enemyList = this.CountEnemies();
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x0000D965 File Offset: 0x0000BB65
		private void OnGUI()
		{
			GUI.Label(new Rect(12f, 160f, 200f, 25f), "Living Monsters: " + this.enemyList);
		}

		// Token: 0x040015DA RID: 5594
		private int enemyList;
	}
}
