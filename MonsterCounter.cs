using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000363 RID: 867
	public class MonsterCounter : MonoBehaviour
	{
		// Token: 0x060011E5 RID: 4581 RVA: 0x0000DA33 File Offset: 0x0000BC33
		private int CountEnemies()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x0000DA40 File Offset: 0x0000BC40
		private void Update()
		{
			this.enemyList = this.CountEnemies();
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x0000DA4E File Offset: 0x0000BC4E
		private void OnGUI()
		{
			GUI.Label(new Rect(12f, 160f, 200f, 25f), "Living Monsters: " + this.enemyList);
		}

		// Token: 0x040015F3 RID: 5619
		private int enemyList;
	}
}
