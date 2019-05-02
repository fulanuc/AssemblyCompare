using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000639 RID: 1593
	public class RuleBookViewerStrip : MonoBehaviour
	{
		// Token: 0x060023EE RID: 9198 RVA: 0x0001A383 File Offset: 0x00018583
		private RuleChoiceController CreateChoice()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.choicePrefab, this.choiceContainer);
			gameObject.SetActive(true);
			RuleChoiceController component = gameObject.GetComponent<RuleChoiceController>();
			component.strip = this;
			return component;
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x0001A3A9 File Offset: 0x000185A9
		private void DestroyChoice(RuleChoiceController choiceController)
		{
			UnityEngine.Object.Destroy(choiceController.gameObject);
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x000AAB5C File Offset: 0x000A8D5C
		public void SetData(List<RuleChoiceDef> newChoices, int choiceIndex)
		{
			this.AllocateChoices(newChoices.Count);
			int num = this.currentDisplayChoiceIndex;
			int count = newChoices.Count;
			bool canVote = count > 1;
			for (int i = 0; i < count; i++)
			{
				this.choiceControllers[i].canVote = canVote;
				this.choiceControllers[i].SetChoice(newChoices[i]);
				if (newChoices[i].localIndex == choiceIndex)
				{
					num = i;
				}
			}
			this.currentDisplayChoiceIndex = num;
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x000AABD8 File Offset: 0x000A8DD8
		private void AllocateChoices(int desiredCount)
		{
			while (this.choiceControllers.Count > desiredCount)
			{
				int index = this.choiceControllers.Count - 1;
				this.DestroyChoice(this.choiceControllers[index]);
				this.choiceControllers.RemoveAt(index);
			}
			while (this.choiceControllers.Count < desiredCount)
			{
				this.choiceControllers.Add(this.CreateChoice());
			}
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x000AAC44 File Offset: 0x000A8E44
		public void Update()
		{
			if (this.choiceControllers.Count == 0)
			{
				return;
			}
			if (this.currentDisplayChoiceIndex >= this.choiceControllers.Count)
			{
				this.currentDisplayChoiceIndex = this.choiceControllers.Count - 1;
			}
			Vector3 localPosition = this.choiceControllers[this.currentDisplayChoiceIndex].transform.localPosition;
			float target = 0f;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					target = -localPosition.y;
				}
			}
			else
			{
				target = -localPosition.x;
			}
			this.currentPosition = Mathf.SmoothDamp(this.currentPosition, target, ref this.velocity, this.movementDuration);
			this.UpdatePosition();
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x0001A3B6 File Offset: 0x000185B6
		private void OnEnable()
		{
			this.UpdatePosition();
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x000AACF0 File Offset: 0x000A8EF0
		private void UpdatePosition()
		{
			Vector3 localPosition = this.choiceContainer.localPosition;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					localPosition.y = this.currentPosition;
				}
			}
			else
			{
				localPosition.x = this.currentPosition;
			}
			this.choiceContainer.localPosition = localPosition;
		}

		// Token: 0x0400269C RID: 9884
		public GameObject choicePrefab;

		// Token: 0x0400269D RID: 9885
		public RectTransform choiceContainer;

		// Token: 0x0400269E RID: 9886
		public RectTransform.Axis movementAxis = RectTransform.Axis.Vertical;

		// Token: 0x0400269F RID: 9887
		public float movementDuration = 0.1f;

		// Token: 0x040026A0 RID: 9888
		private RuleDef currentRuleDef;

		// Token: 0x040026A1 RID: 9889
		public readonly List<RuleChoiceController> choiceControllers = new List<RuleChoiceController>();

		// Token: 0x040026A2 RID: 9890
		public int currentDisplayChoiceIndex;

		// Token: 0x040026A3 RID: 9891
		private float velocity;

		// Token: 0x040026A4 RID: 9892
		private float currentPosition;
	}
}
