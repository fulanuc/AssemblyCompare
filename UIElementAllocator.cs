using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000656 RID: 1622
	public class UIElementAllocator<T> where T : Component
	{
		// Token: 0x0600244C RID: 9292 RVA: 0x0001A6F1 File Offset: 0x000188F1
		public UIElementAllocator([NotNull] RectTransform containerTransform, [NotNull] GameObject elementPrefab)
		{
			this.containerTransform = containerTransform;
			this.elementPrefab = elementPrefab;
			this.elementControllerComponentsList = new List<T>();
			this.elements = new ReadOnlyCollection<T>(this.elementControllerComponentsList);
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x000AC8B0 File Offset: 0x000AAAB0
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

		// Token: 0x0400273A RID: 10042
		public readonly RectTransform containerTransform;

		// Token: 0x0400273B RID: 10043
		public readonly GameObject elementPrefab;

		// Token: 0x0400273C RID: 10044
		[NotNull]
		private List<T> elementControllerComponentsList;

		// Token: 0x0400273D RID: 10045
		[NotNull]
		public readonly ReadOnlyCollection<T> elements;

		// Token: 0x0400273E RID: 10046
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onCreateElement;

		// Token: 0x0400273F RID: 10047
		[CanBeNull]
		public UIElementAllocator<T>.ElementOperationDelegate onDestroyElement;

		// Token: 0x02000657 RID: 1623
		// (Invoke) Token: 0x0600244F RID: 9295
		public delegate void ElementOperationDelegate(int index, T element);
	}
}
