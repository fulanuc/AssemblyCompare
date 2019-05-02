using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000238 RID: 568
	internal static class DamageInfoNetworkReaderExtension
	{
		// Token: 0x06000AC3 RID: 2755 RVA: 0x000491B8 File Offset: 0x000473B8
		public static DamageInfo ReadDamageInfo(this NetworkReader reader)
		{
			return new DamageInfo
			{
				damage = reader.ReadSingle(),
				crit = reader.ReadBoolean(),
				attacker = reader.ReadGameObject(),
				inflictor = reader.ReadGameObject(),
				position = reader.ReadVector3(),
				force = reader.ReadVector3(),
				procChainMask = reader.ReadProcChainMask(),
				procCoefficient = reader.ReadSingle(),
				damageType = (DamageType)reader.ReadByte(),
				damageColorIndex = (DamageColorIndex)reader.ReadByte()
			};
		}
	}
}
