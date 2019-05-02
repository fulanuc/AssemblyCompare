using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B5 RID: 1461
	public class CarouselNavigationController : MonoBehaviour
	{
		// Token: 0x060020BF RID: 8383 RVA: 0x00017D98 File Offset: 0x00015F98
		private void Awake()
		{
			this.buttonAllocator = new UIElementAllocator<MPButton>(this.container, this.buttonPrefab);
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x0009EAF0 File Offset: 0x0009CCF0
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

		// Token: 0x060020C1 RID: 8385 RVA: 0x00017DB1 File Offset: 0x00015FB1
		private void OnEnable()
		{
			this.Rebuild();
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x00017DB9 File Offset: 0x00015FB9
		public void SetDisplayData(CarouselNavigationController.DisplayData newDisplayData)
		{
			if (newDisplayData.Equals(this.displayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			this.Rebuild();
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x0009EB50 File Offset: 0x0009CD50
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

		// Token: 0x060020C4 RID: 8388 RVA: 0x0009ED14 File Offset: 0x0009CF14
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

		// Token: 0x060020C5 RID: 8389 RVA: 0x0009EDC0 File Offset: 0x0009CFC0
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

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x060020C6 RID: 8390 RVA: 0x0009EE6C File Offset: 0x0009D06C
		// (remove) Token: 0x060020C7 RID: 8391 RVA: 0x0009EEA4 File Offset: 0x0009D0A4
		public event Action<int> onPageChangeSubmitted;

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x060020C8 RID: 8392 RVA: 0x0009EEDC File Offset: 0x0009D0DC
		// (remove) Token: 0x060020C9 RID: 8393 RVA: 0x0009EF14 File Offset: 0x0009D114
		public event Action onUnderflow;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x060020CA RID: 8394 RVA: 0x0009EF4C File Offset: 0x0009D14C
		// (remove) Token: 0x060020CB RID: 8395 RVA: 0x0009EF84 File Offset: 0x0009D184
		public event Action onOverflow;

		// Token: 0x0400233E RID: 9022
		public GameObject buttonPrefab;

		// Token: 0x0400233F RID: 9023
		public RectTransform container;

		// Token: 0x04002340 RID: 9024
		public MPButton leftButton;

		// Token: 0x04002341 RID: 9025
		public MPButton rightButton;

		// Token: 0x04002342 RID: 9026
		public bool allowLooping;

		// Token: 0x04002343 RID: 9027
		public UIElementAllocator<MPButton> buttonAllocator;

		// Token: 0x04002344 RID: 9028
		private int currentPageIndex;

		// Token: 0x04002345 RID: 9029
		private CarouselNavigationController.DisplayData displayData = new CarouselNavigationController.DisplayData(0, 0);

		// Token: 0x020005B6 RID: 1462
		public struct DisplayData : IEquatable<CarouselNavigationController.DisplayData>
		{
			// Token: 0x060020CD RID: 8397 RVA: 0x00017DED File Offset: 0x00015FED
			public DisplayData(int pageCount, int pageIndex)
			{
				this.pageCount = pageCount;
				this.pageIndex = pageIndex;
			}

			// Token: 0x060020CE RID: 8398 RVA: 0x00017DFD File Offset: 0x00015FFD
			public bool Equals(CarouselNavigationController.DisplayData other)
			{
				return this.pageCount == other.pageCount && this.pageIndex == other.pageIndex;
			}

			// Token: 0x060020CF RID: 8399 RVA: 0x0009EFBC File Offset: 0x0009D1BC
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

			// Token: 0x060020D0 RID: 8400 RVA: 0x00017E1D File Offset: 0x0001601D
			public override int GetHashCode()
			{
				return this.pageCount * 397 ^ this.pageIndex;
			}

			// Token: 0x04002349 RID: 9033
			public readonly int pageCount;

			// Token: 0x0400234A RID: 9034
			public readonly int pageIndex;
		}
	}
}
