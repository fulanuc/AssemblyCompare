using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C3 RID: 1475
	[RequireComponent(typeof(RectTransform))]
	public class BuffIcon : MonoBehaviour
	{
		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x000183F8 File Offset: 0x000165F8
		// (set) Token: 0x06002141 RID: 8513 RVA: 0x00018400 File Offset: 0x00016600
		public RectTransform rectTransform { get; private set; }

		// Token: 0x06002142 RID: 8514 RVA: 0x00018409 File Offset: 0x00016609
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.UpdateIcon();
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x0009FDD8 File Offset: 0x0009DFD8
		public void Flash()
		{
			BuffDef buffDef = BuffCatalog.GetBuffDef(this.buffIndex);
			this.iconImage.color = Color.white;
			this.iconImage.CrossFadeColor(buffDef.buffColor, 0.25f, true, false);
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x0009FE1C File Offset: 0x0009E01C
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

		// Token: 0x06002145 RID: 8517 RVA: 0x0001841D File Offset: 0x0001661D
		private void Update()
		{
			if (this.lastBuffIndex != this.buffIndex)
			{
				this.lastBuffIndex = this.buffIndex;
			}
		}

		// Token: 0x0400237E RID: 9086
		private BuffIndex lastBuffIndex;

		// Token: 0x0400237F RID: 9087
		public BuffIndex buffIndex = BuffIndex.None;

		// Token: 0x04002380 RID: 9088
		public Image iconImage;

		// Token: 0x04002381 RID: 9089
		public TextMeshProUGUI stackCount;

		// Token: 0x04002382 RID: 9090
		public int buffCount;

		// Token: 0x04002383 RID: 9091
		private float stopwatch;

		// Token: 0x04002384 RID: 9092
		private const float flashDuration = 0.25f;

		// Token: 0x04002385 RID: 9093
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
