using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200020B RID: 523
	public static class BuffCatalog
	{
		// Token: 0x06000A31 RID: 2609 RVA: 0x0000838F File Offset: 0x0000658F
		private static void RegisterBuff(BuffIndex buffIndex, BuffDef buffDef)
		{
			buffDef.buffIndex = buffIndex;
			BuffCatalog.buffDefs[(int)buffIndex] = buffDef;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x000083A0 File Offset: 0x000065A0
		public static BuffDef GetBuffDef(BuffIndex buffIndex)
		{
			if (buffIndex < BuffIndex.Slow50 || buffIndex > BuffIndex.Count)
			{
				return null;
			}
			return BuffCatalog.buffDefs[(int)buffIndex];
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00046F6C File Offset: 0x0004516C
		static BuffCatalog()
		{
			BuffCatalog.RegisterBuff(BuffIndex.ArmorBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.227450982f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow50, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.917647064f, 0.407843143f, 0.419607848f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.ClayGoo, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.2f, 0.09019608f, 0.09019608f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AttackSpeedOnCrit, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffAttackSpeedOnCritIcon",
				buffColor = new Color(0.9098039f, 0.5058824f, 0.239215687f),
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.BeetleJuice, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffBeetleJuiceIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.227450982f),
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.RepairMode, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.MedkitHeal, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffMedkitHealIcon",
				buffColor = new Color(0.784313738f, 0.9372549f, 0.427450985f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Warbanner, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffWarbannerIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.227450982f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.EnrageAncientWisp, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.Cloak, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCloakIcon",
				buffColor = new Color(0.3764706f, 0.843137264f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.CloakSpeed, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
				buffColor = new Color(0.3764706f, 0.843137264f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.LightningShield, new BuffDef
			{
				iconPath = null
			});
			BuffCatalog.RegisterBuff(BuffIndex.FullCrit, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffFullCritIcon",
				buffColor = new Color(0.8392157f, 0.227450982f, 0.227450982f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.TempestSpeed, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTempestSpeedIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.227450982f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.EngiShield, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffEngiShieldIcon",
				buffColor = new Color(0.3764706f, 0.843137264f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.BugWings, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
				buffColor = new Color(0.3764706f, 0.843137264f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.TeslaField, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTeslaIcon",
				buffColor = new Color(0.858823538f, 0.533333361f, 0.9843137f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.WarCryBuff, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texWarcryBuffIcon",
				buffColor = new Color(0.827451f, 0.196078435f, 0.09803922f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow30, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.EngiTeamShield, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.CommandoBoost, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.GoldEmpowered, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffAttackSpeedOnCritIcon",
				buffColor = new Color(1f, 0.7882353f, 0.05490196f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Immune, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(1f, 0.7882353f, 0.05490196f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Cripple, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCrippleIcon"
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow80, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.647058845f, 0.870588243f, 0.929411769f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow60, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixRed, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixRed"
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixBlue, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixBlue"
			});
			BuffCatalog.RegisterBuff(BuffIndex.NoCooldowns, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texNoCooldownsBuffIcon",
				buffColor = new Color(0.733333349f, 0.545098066f, 0.9764706f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixWhite, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixWhite"
			});
			BuffCatalog.RegisterBuff(BuffIndex.Invincibility, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.8117647f, 0.6117647f, 0.8784314f)
			});
			for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
			{
				if (BuffCatalog.buffDefs[(int)buffIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered buff {0}!", new object[]
					{
						Enum.GetName(typeof(BuffIndex), buffIndex)
					});
				}
			}
			BuffCatalog.eliteBuffIndices = (from buffDef in BuffCatalog.buffDefs
			where buffDef.isElite
			select buffDef.buffIndex).ToArray<BuffIndex>();
		}

		// Token: 0x04000D92 RID: 3474
		private static BuffDef[] buffDefs = new BuffDef[31];

		// Token: 0x04000D93 RID: 3475
		public static readonly BuffIndex[] eliteBuffIndices;
	}
}
