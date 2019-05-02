using System;

namespace RoR2
{
	// Token: 0x0200023E RID: 574
	public static class DifficultyCatalog
	{
		// Token: 0x06000AD2 RID: 2770 RVA: 0x000495B4 File Offset: 0x000477B4
		static DifficultyCatalog()
		{
			DifficultyCatalog.difficultyDefs[0] = new DifficultyDef(1f, "DIFFICULTY_EASY_NAME", "Textures/DifficultyIcons/texDifficultyEasyIcon", "DIFFICULTY_EASY_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.EasyDifficulty));
			DifficultyCatalog.difficultyDefs[1] = new DifficultyDef(2f, "DIFFICULTY_NORMAL_NAME", "Textures/DifficultyIcons/texDifficultyNormalIcon", "DIFFICULTY_NORMAL_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty));
			DifficultyCatalog.difficultyDefs[2] = new DifficultyDef(3f, "DIFFICULTY_HARD_NAME", "Textures/DifficultyIcons/texDifficultyHardIcon", "DIFFICULTY_HARD_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty));
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00008BC4 File Offset: 0x00006DC4
		public static DifficultyDef GetDifficultyDef(DifficultyIndex difficultyIndex)
		{
			return DifficultyCatalog.difficultyDefs[(int)difficultyIndex];
		}

		// Token: 0x04000E95 RID: 3733
		private static readonly DifficultyDef[] difficultyDefs = new DifficultyDef[3];
	}
}
