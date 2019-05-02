using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B1 RID: 1457
	[RequireComponent(typeof(RectTransform))]
	public class BuffIcon : MonoBehaviour
	{
		// Token: 0x170002DA RID: 730
		// (get) Token: 0x060020AF RID: 8367 RVA: 0x00017CFE File Offset: 0x00015EFE
		// (set) Token: 0x060020B0 RID: 8368 RVA: 0x00017D06 File Offset: 0x00015F06
		public RectTransform rectTransform { get; private set; }

		// Token: 0x060020B1 RID: 8369 RVA: 0x00017D0F File Offset: 0x00015F0F
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.UpdateIcon();
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x0009E804 File Offset: 0x0009CA04
		public void Flash()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			this.iconImage.color = Color.white;
			this.iconImage.CrossFadeColor(buffDef.buffColor, 0.25f, true, false);
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x0009E848 File Offset: 0x0009CA48
		public void UpdateIcon()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			if (buffDef == null)
			{
				this.iconImage.sprite = null;
				return;
			}
			this.iconImage.sprite = Resources.Load<Sprite>(buffDef.iconPath);
			this.iconImage.color = buffDef.buffColor;
			if (buffDef.canStack)
			{
				BuffIcon.sharedStringBuilder.Clear();
				BuffIcon.sharedStringBuilder.Append("x");
				BuffIcon.sharedStringBuilder.AppendInt(this.buffCount, 0u, uint.MaxValue);
				this.stackCount.enabled = true;
				this.stackCount.SetText(BuffIcon.sharedStringBuilder);
				return;
			}
			this.stackCount.enabled = false;
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x00017D23 File Offset: 0x00015F23
		private void Update()
		{
			if (this.lastBuffIndex != this.buffIndex)
			{
				this.lastBuffIndex = this.buffIndex;
			}
		}

		// Token: 0x0400232A RID: 9002
		private BuffIndex lastBuffIndex;

		// Token: 0x0400232B RID: 9003
		public BuffIndex buffIndex = BuffIndex.None;

		// Token: 0x0400232C RID: 9004
		public Image iconImage;

		// Token: 0x0400232D RID: 9005
		public TextMeshProUGUI stackCount;

		// Token: 0x0400232E RID: 9006
		public int buffCount;

		// Token: 0x0400232F RID: 9007
		private float stopwatch;

		// Token: 0x04002330 RID: 9008
		private const float flashDuration = 0.25f;

		// Token: 0x04002331 RID: 9009
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
