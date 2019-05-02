using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F7 RID: 1527
	public class HUDScaleController : MonoBehaviour
	{
		// Token: 0x0600226E RID: 8814 RVA: 0x0001916D File Offset: 0x0001736D
		public void OnEnable()
		{
			HUDScaleController.instancesList.Add(this);
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x0001917A File Offset: 0x0001737A
		public void OnDisable()
		{
			HUDScaleController.instancesList.Remove(this);
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x00019188 File Offset: 0x00017388
		private void Start()
		{
			this.SetScale();
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x000A5AE0 File Offset: 0x000A3CE0
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

		// Token: 0x0400252F RID: 9519
		public RectTransform[] rectTransforms;

		// Token: 0x04002530 RID: 9520
		private static List<HUDScaleController> instancesList = new List<HUDScaleController>();

		// Token: 0x020005F8 RID: 1528
		private class HUDScaleConVar : BaseConVar
		{
			// Token: 0x06002274 RID: 8820 RVA: 0x000090CD File Offset: 0x000072CD
			private HUDScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002275 RID: 8821 RVA: 0x000A5B50 File Offset: 0x000A3D50
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

			// Token: 0x06002276 RID: 8822 RVA: 0x0001919C File Offset: 0x0001739C
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(this.intValue);
			}

			// Token: 0x04002531 RID: 9521
			public static HUDScaleController.HUDScaleConVar instance = new HUDScaleController.HUDScaleConVar("hud_scale", ConVarFlags.Archive, "100", "Scales the size of HUD elements in-game. Defaults to 100.");

			// Token: 0x04002532 RID: 9522
			private int intValue;
		}
	}
}
