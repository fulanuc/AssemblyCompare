using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000242 RID: 578
	public static class EliteCatalog
	{
		// Token: 0x06000AE3 RID: 2787 RVA: 0x0004996C File Offset: 0x00047B6C
		static EliteCatalog()
		{
			EliteCatalog.RegisterElite(EliteIndex.Fire, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixRed,
				color = Color.red,
				prefix = "Blazing "
			});
			EliteCatalog.RegisterElite(EliteIndex.Lightning, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixBlue,
				color = Color.blue,
				prefix = "Overloading "
			});
			EliteCatalog.RegisterElite(EliteIndex.Ice, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixWhite,
				color = Color.white,
				prefix = "Glacial "
			});
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00008C69 File Offset: 0x00006E69
		private static void RegisterElite(EliteIndex eliteIndex, EliteDef eliteDef)
		{
			eliteDef.eliteIndex = eliteIndex;
			EliteCatalog.eliteList.Add(eliteIndex);
			EliteCatalog.eliteDefs[(int)eliteIndex] = eliteDef;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00049A18 File Offset: 0x00047C18
		public static EliteIndex IsEquipmentElite(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return EliteIndex.None;
			}
			foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
			{
				if (eliteDef.eliteEquipmentIndex == equipmentIndex)
				{
					return eliteDef.eliteIndex;
				}
			}
			return EliteIndex.None;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00008C85 File Offset: 0x00006E85
		public static EliteDef GetEliteDef(EliteIndex eliteIndex)
		{
			if (eliteIndex < EliteIndex.Fire || eliteIndex >= EliteIndex.Count)
			{
				return null;
			}
			return EliteCatalog.eliteDefs[(int)eliteIndex];
		}

		// Token: 0x04000EAA RID: 3754
		public static List<EliteIndex> eliteList = new List<EliteIndex>();

		// Token: 0x04000EAB RID: 3755
		private static EliteDef[] eliteDefs = new EliteDef[3];
	}
}
