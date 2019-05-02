using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2.Orbs
{
	// Token: 0x02000527 RID: 1319
	public static class OrbCatalog
	{
		// Token: 0x06001DD0 RID: 7632 RVA: 0x00091250 File Offset: 0x0008F450
		private static void GenerateCatalog()
		{
			OrbCatalog.indexToType = (from t in typeof(Orb).Assembly.GetTypes()
			where t.IsSubclassOf(typeof(Orb))
			orderby t.Name
			select t).ToArray<Type>();
			OrbCatalog.typeToIndex.Clear();
			foreach (Type key in OrbCatalog.indexToType)
			{
				OrbCatalog.typeToIndex[key] = OrbCatalog.typeToIndex.Count;
			}
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x00015D2C File Offset: 0x00013F2C
		static OrbCatalog()
		{
			OrbCatalog.GenerateCatalog();
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x000912FC File Offset: 0x0008F4FC
		public static int FindIndex(Type type)
		{
			int result;
			if (OrbCatalog.typeToIndex.TryGetValue(type, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x00015D47 File Offset: 0x00013F47
		public static Type FindType(int index)
		{
			if (index < 0 || index >= OrbCatalog.indexToType.Length)
			{
				return null;
			}
			return OrbCatalog.indexToType[index];
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x00015D60 File Offset: 0x00013F60
		public static Orb Instantiate(int index)
		{
			return OrbCatalog.Instantiate(OrbCatalog.FindType(index));
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x00015D6D File Offset: 0x00013F6D
		public static Orb Instantiate(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return (Orb)Activator.CreateInstance(type);
		}

		// Token: 0x04001FDB RID: 8155
		private static readonly Dictionary<Type, int> typeToIndex = new Dictionary<Type, int>();

		// Token: 0x04001FDC RID: 8156
		private static Type[] indexToType = Array.Empty<Type>();
	}
}
