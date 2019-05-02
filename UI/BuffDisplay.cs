using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C2 RID: 1474
	[RequireComponent(typeof(RectTransform))]
	public class BuffDisplay : MonoBehaviour
	{
		// Token: 0x0600213B RID: 8507 RVA: 0x000183CF File Offset: 0x000165CF
		private void Awake()
		{
			this.rectTranform = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x0009FC64 File Offset: 0x0009DE64
		private void AllocateIcons()
		{
			int num = 0;
			if (this.source)
			{
				for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
				{
					if (this.source.HasBuff(buffIndex))
					{
						num++;
					}
				}
			}
			if (num != this.buffIcons.Count)
			{
				while (this.buffIcons.Count > num)
				{
					UnityEngine.Object.Destroy(this.buffIcons[this.buffIcons.Count - 1].gameObject);
					this.buffIcons.RemoveAt(this.buffIcons.Count - 1);
				}
				while (this.buffIcons.Count < num)
				{
					BuffIcon component = UnityEngine.Object.Instantiate<GameObject>(this.buffIconPrefab, this.rectTranform).GetComponent<BuffIcon>();
					this.buffIcons.Add(component);
				}
				this.UpdateLayout();
			}
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x0009FD34 File Offset: 0x0009DF34
		private void UpdateLayout()
		{
			this.AllocateIcons();
			float width = this.rectTranform.rect.width;
			if (this.source)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
				{
					if (this.source.HasBuff(buffIndex))
					{
						BuffIcon buffIcon = this.buffIcons[num];
						buffIcon.buffIndex = buffIndex;
						buffIcon.rectTransform.anchoredPosition = zero;
						buffIcon.buffCount = this.source.GetBuffCount(buffIndex);
						zero.x += this.iconWidth;
						buffIcon.UpdateIcon();
						num++;
					}
				}
			}
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000183DD File Offset: 0x000165DD
		private void Update()
		{
			this.UpdateLayout();
		}

		// Token: 0x04002379 RID: 9081
		private RectTransform rectTranform;

		// Token: 0x0400237A RID: 9082
		public CharacterBody source;

		// Token: 0x0400237B RID: 9083
		public GameObject buffIconPrefab;

		// Token: 0x0400237C RID: 9084
		public float iconWidth = 24f;

		// Token: 0x0400237D RID: 9085
		[SerializeField]
		[HideInInspector]
		private List<BuffIcon> buffIcons;
	}
}
