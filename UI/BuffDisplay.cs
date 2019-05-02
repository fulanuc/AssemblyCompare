using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B0 RID: 1456
	[RequireComponent(typeof(RectTransform))]
	public class BuffDisplay : MonoBehaviour
	{
		// Token: 0x060020AA RID: 8362 RVA: 0x00017CD5 File Offset: 0x00015ED5
		private void Awake()
		{
			this.rectTranform = base.GetComponent<RectTransform>();
		}

		// Token: 0x060020AB RID: 8363 RVA: 0x0009E690 File Offset: 0x0009C890
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

		// Token: 0x060020AC RID: 8364 RVA: 0x0009E760 File Offset: 0x0009C960
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

		// Token: 0x060020AD RID: 8365 RVA: 0x00017CE3 File Offset: 0x00015EE3
		private void Update()
		{
			this.UpdateLayout();
		}

		// Token: 0x04002325 RID: 8997
		private RectTransform rectTranform;

		// Token: 0x04002326 RID: 8998
		public CharacterBody source;

		// Token: 0x04002327 RID: 8999
		public GameObject buffIconPrefab;

		// Token: 0x04002328 RID: 9000
		public float iconWidth = 24f;

		// Token: 0x04002329 RID: 9001
		[SerializeField]
		[HideInInspector]
		private List<BuffIcon> buffIcons;
	}
}
