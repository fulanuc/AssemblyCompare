using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C7 RID: 1479
	public class CarouselNavigationController : MonoBehaviour
	{
		// Token: 0x06002150 RID: 8528 RVA: 0x00018492 File Offset: 0x00016692
		private void Awake()
		{
			this.buttonAllocator = new UIElementAllocator<MPButton>(this.container, this.buttonPrefab);
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000A00C4 File Offset: 0x0009E2C4
		private void Start()
		{
			if (this.leftButton)
			{
				this.leftButton.onClick.AddListener(new UnityAction(this.NavigatePreviousPage));
			}
			if (this.rightButton)
			{
				this.rightButton.onClick.AddListener(new UnityAction(this.NavigateNextPage));
			}
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000184AB File Offset: 0x000166AB
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000184B3 File Offset: 0x000166B3
		public void SetDisplayData(CarouselNavigationController.DisplayData newDisplayData)
		{
			if (newDisplayData.Equals(this.displayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			this.Rebuild();
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000A0124 File Offset: 0x0009E324
		private void Rebuild()
		{
			this.buttonAllocator.AllocateElements(this.displayData.pageCount);
			int i = 0;
			int count = this.buttonAllocator.elements.Count;
			while (i < count)
			{
				MPButton mpbutton = this.buttonAllocator.elements[i];
				ColorBlock colors = mpbutton.colors;
				colors.colorMultiplier = 1f;
				mpbutton.colors = colors;
				mpbutton.onClick.RemoveAllListeners();
				CarouselNavigationController.DisplayData buttonDisplayData = new CarouselNavigationController.DisplayData(this.displayData.pageCount, i);
				mpbutton.onClick.AddListener(delegate()
				{
					this.SetDisplayData(buttonDisplayData);
					Action<int> action = this.onPageChangeSubmitted;
					if (action == null)
					{
						return;
					}
					action(this.displayData.pageIndex);
				});
				i++;
			}
			if (this.displayData.pageIndex >= 0 && this.displayData.pageIndex < this.displayData.pageCount)
			{
				MPButton mpbutton2 = this.buttonAllocator.elements[this.displayData.pageIndex];
				ColorBlock colors2 = mpbutton2.colors;
				colors2.colorMultiplier = 2f;
				mpbutton2.colors = colors2;
			}
			bool flag = this.displayData.pageCount > 1;
			bool interactable = flag && (this.allowLooping || this.displayData.pageIndex > 0);
			bool interactable2 = flag && (this.allowLooping || this.displayData.pageIndex < this.displayData.pageCount - 1);
			if (this.leftButton)
			{
				this.leftButton.gameObject.SetActive(flag);
				this.leftButton.interactable = interactable;
			}
			if (this.rightButton)
			{
				this.rightButton.gameObject.SetActive(flag);
				this.rightButton.interactable = interactable2;
			}
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x000A02E8 File Offset: 0x0009E4E8
		public void NavigateNextPage()
		{
			if (this.displayData.pageCount <= 0)
			{
				return;
			}
			int num = this.displayData.pageIndex + 1;
			bool flag = num >= this.displayData.pageCount;
			if (flag)
			{
				if (!this.allowLooping)
				{
					return;
				}
				num = 0;
			}
			Debug.LogFormat("NavigateNextPage() desiredPageIndex={0} overflow={1}", new object[]
			{
				num,
				flag
			});
			this.SetDisplayData(new CarouselNavigationController.DisplayData(this.displayData.pageCount, num));
			Action<int> action = this.onPageChangeSubmitted;
			if (action != null)
			{
				action(num);
			}
			if (flag)
			{
				Action action2 = this.onOverflow;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x000A0394 File Offset: 0x0009E594
		public void NavigatePreviousPage()
		{
			if (this.displayData.pageCount <= 0)
			{
				return;
			}
			int num = this.displayData.pageIndex - 1;
			bool flag = num < 0;
			if (flag)
			{
				if (!this.allowLooping)
				{
					return;
				}
				num = this.displayData.pageCount - 1;
			}
			Debug.LogFormat("NavigatePreviousPage() desiredPageIndex={0} underflow={1}", new object[]
			{
				num,
				flag
			});
			this.SetDisplayData(new CarouselNavigationController.DisplayData(this.displayData.pageCount, num));
			Action<int> action = this.onPageChangeSubmitted;
			if (action != null)
			{
				action(num);
			}
			if (flag)
			{
				Action action2 = this.onUnderflow;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x06002157 RID: 8535 RVA: 0x000A0440 File Offset: 0x0009E640
		// (remove) Token: 0x06002158 RID: 8536 RVA: 0x000A0478 File Offset: 0x0009E678
		public event Action<int> onPageChangeSubmitted;

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06002159 RID: 8537 RVA: 0x000A04B0 File Offset: 0x0009E6B0
		// (remove) Token: 0x0600215A RID: 8538 RVA: 0x000A04E8 File Offset: 0x0009E6E8
		public event Action onUnderflow;

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x0600215B RID: 8539 RVA: 0x000A0520 File Offset: 0x0009E720
		// (remove) Token: 0x0600215C RID: 8540 RVA: 0x000A0558 File Offset: 0x0009E758
		public event Action onOverflow;

		// Token: 0x04002392 RID: 9106
		public GameObject buttonPrefab;

		// Token: 0x04002393 RID: 9107
		public RectTransform container;

		// Token: 0x04002394 RID: 9108
		public MPButton leftButton;

		// Token: 0x04002395 RID: 9109
		public MPButton rightButton;

		// Token: 0x04002396 RID: 9110
		public bool allowLooping;

		// Token: 0x04002397 RID: 9111
		public UIElementAllocator<MPButton> buttonAllocator;

		// Token: 0x04002398 RID: 9112
		private int currentPageIndex;

		// Token: 0x04002399 RID: 9113
		private CarouselNavigationController.DisplayData displayData = new CarouselNavigationController.DisplayData(0, 0);

		// Token: 0x020005C8 RID: 1480
		public struct DisplayData : IEquatable<CarouselNavigationController.DisplayData>
		{
			// Token: 0x0600215E RID: 8542 RVA: 0x000184E7 File Offset: 0x000166E7
			public DisplayData(int pageCount, int pageIndex)
			{
				this.pageCount = pageCount;
				this.pageIndex = pageIndex;
			}

			// Token: 0x0600215F RID: 8543 RVA: 0x000184F7 File Offset: 0x000166F7
			public bool Equals(CarouselNavigationController.DisplayData other)
			{
				return this.pageCount == other.pageCount && this.pageIndex == other.pageIndex;
			}

			// Token: 0x06002160 RID: 8544 RVA: 0x000A0590 File Offset: 0x0009E790
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is CarouselNavigationController.DisplayData)
				{
					CarouselNavigationController.DisplayData other = (CarouselNavigationController.DisplayData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06002161 RID: 8545 RVA: 0x00018517 File Offset: 0x00016717
			public override int GetHashCode()
			{
				return this.pageCount * 397 ^ this.pageIndex;
			}

			// Token: 0x0400239D RID: 9117
			public readonly int pageCount;

			// Token: 0x0400239E RID: 9118
			public readonly int pageIndex;
		}
	}
}
