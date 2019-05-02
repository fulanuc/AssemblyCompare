using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000668 RID: 1640
	public class UIElementAllocator<T> where T : Component
	{
		// Token: 0x060024DD RID: 9437 RVA: 0x0001ADEA File Offset: 0x00018FEA
		public UIElementAllocator([NotNull] RectTransform containerTransform, [NotNull] GameObject elementPrefab)
		{
			this.containerTransform = containerTransform;
			this.elementPrefab = elementPrefab;
			this.elementControllerComponentsList = new List<T>();
			this.elements = new ReadOnlyCollection<T>(this.elementControllerComponentsList);
		}

		// Token: 0x060024DE RID: 9438 RVA: 0x000ADF30 File Offset: 0x000AC130
		public void AllocateElements(int desiredCount)
		{
			if (desiredCount < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = this.elementControllerComponentsList.Count - 1; i >= desiredCount; i--)
			{
				T t = this.elementControllerComponentsList[i];
				UIElementAllocator<T>.ElementOperationDelegate elementOperationDelegate = this.onDestroyElement;
				if (elementOperationDelegate != null)
				{
					elementOperationDelegate(i, t);
				}
				UnityEngine.Object.Destroy(t.gameObject);
				this.elementControllerComponentsList.RemoveAt(i);
			}
			for (int j = this.elementControllerComponentsList.Count; j < desiredCount; j++)
			{
				T component = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab, this.containerTransform).GetComponent<T>();
				this.elementControllerComponentsList.Add(component);
				component.gameObject.SetActive(true);
				UIElementAllocator<T>.ElementOperationDelegate elementOperationDelegate2 = this.onCreateElement;
				if (elementOperationDelegate2 != null)
				{
					elementOperationDelegate2(j, component);
				}
			}
		}

		// Token: 0x04002795 RID: 10133
		public readonly RectTransform containerTransform;

		// Token: 0x04002796 RID: 10134
		public readonly GameObject elementPrefab;

		// Token: 0x04002797 RID: 10135
		[NotNull]
		private List<T> elementControllerComponentsList;

		// Token: 0x04002798 RID: 10136
		[NotNull]
		public readonly ReadOnlyCollection<T> elements;

		// Token: 0x04002799 RID: 10137
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onCreateElement;

		// Token: 0x0400279A RID: 10138
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onDestroyElement;

		// Token: 0x02000669 RID: 1641
		// (Invoke) Token: 0x060024E0 RID: 9440
		public delegate void ElementOperationDelegate(int index, T element);
	}
}
