using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2.Orbs
{
	// Token: 0x02000518 RID: 1304
	public static class OrbCatalog
	{
		// Token: 0x06001D68 RID: 7528 RVA: 0x000904DC File Offset: 0x0008E6DC
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

		// Token: 0x06001D69 RID: 7529 RVA: 0x00015863 File Offset: 0x00013A63
		static OrbCatalog()
		{
			OrbCatalog.GenerateCatalog();
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x00090588 File Offset: 0x0008E788
		public static int FindIndex(Type type)
		{
			int result;
			if (OrbCatalog.typeToIndex.TryGetValue(type, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x0001587E File Offset: 0x00013A7E
		public static Type FindType(int index)
		{
			if (index < 0 || index >= OrbCatalog.indexToType.Length)
			{
				return null;
			}
			return OrbCatalog.indexToType[index];
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x00015897 File Offset: 0x00013A97
		public static Orb Instantiate(int index)
		{
			return OrbCatalog.Instantiate(OrbCatalog.FindType(index));
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x000158A4 File Offset: 0x00013AA4
		public static Orb Instantiate(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return (Orb)Activator.CreateInstance(type);
		}

		// Token: 0x04001F9D RID: 8093
		private static readonly Dictionary<Type, int> typeToIndex = new Dictionary<Type, int>();

		// Token: 0x04001F9E RID: 8094
		private static Type[] indexToType = Array.Empty<Type>();
	}
}
