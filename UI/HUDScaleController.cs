using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E5 RID: 1509
	public class HUDScaleController : MonoBehaviour
	{
		// Token: 0x060021DD RID: 8669 RVA: 0x00018A73 File Offset: 0x00016C73
		public void OnEnable()
		{
			HUDScaleController.instancesList.Add(this);
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x00018A80 File Offset: 0x00016C80
		public void OnDisable()
		{
			HUDScaleController.instancesList.Remove(this);
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x00018A8E File Offset: 0x00016C8E
		private void Start()
		{
			this.SetScale();
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x000A452C File Offset: 0x000A272C
		private void SetScale()
		{
			BaseConVar baseConVar = Console.instance.FindConVar("hud_scale");
			float num;
			if (baseConVar != null && TextSerialization.TryParseInvariant(baseConVar.GetString(), out num))
			{
				Vector3 localScale = new Vector3(num / 100f, num / 100f, num / 100f);
				RectTransform[] array = this.rectTransforms;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].localScale = localScale;
				}
			}
		}

		// Token: 0x040024DA RID: 9434
		public RectTransform[] rectTransforms;

		// Token: 0x040024DB RID: 9435
		private static List<HUDScaleController> instancesList = new List<HUDScaleController>();

		// Token: 0x020005E6 RID: 1510
		private class HUDScaleConVar : BaseConVar
		{
			// Token: 0x060021E3 RID: 8675 RVA: 0x000090A8 File Offset: 0x000072A8
			private HUDScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060021E4 RID: 8676 RVA: 0x000A459C File Offset: 0x000A279C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && num != 0)
				{
					this.intValue = num;
					foreach (HUDScaleController hudscaleController in HUDScaleController.instancesList)
					{
						hudscaleController.SetScale();
					}
				}
			}

			// Token: 0x060021E5 RID: 8677 RVA: 0x00018AA2 File Offset: 0x00016CA2
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(this.intValue);
			}

			// Token: 0x040024DC RID: 9436
			public static HUDScaleController.HUDScaleConVar instance = new HUDScaleController.HUDScaleConVar("hud_scale", ConVarFlags.Archive, "100", "Scales the size of HUD elements in-game. Defaults to 100.");

			// Token: 0x040024DD RID: 9437
			private int intValue;
		}
	}
}
