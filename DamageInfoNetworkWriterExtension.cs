using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000237 RID: 567
	internal static class DamageInfoNetworkWriterExtension
	{
		// Token: 0x06000AC2 RID: 2754 RVA: 0x00049130 File Offset: 0x00047330
		public static void Write(this NetworkWriter writer, DamageInfo damageInfo)
		{
			writer.Write(damageInfo.damage);
			writer.Write(damageInfo.crit);
			writer.Write(damageInfo.attacker);
			writer.Write(damageInfo.inflictor);
			writer.Write(damageInfo.position);
			writer.Write(damageInfo.force);
			writer.Write(damageInfo.procChainMask);
			writer.Write(damageInfo.procCoefficient);
			writer.Write((byte)damageInfo.damageType);
			writer.Write((byte)damageInfo.damageColorIndex);
		}
	}
}
