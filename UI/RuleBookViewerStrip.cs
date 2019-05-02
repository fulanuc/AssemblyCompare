using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000627 RID: 1575
	public class RuleBookViewerStrip : MonoBehaviour
	{
		// Token: 0x0600235E RID: 9054 RVA: 0x00019CB5 File Offset: 0x00017EB5
		private RuleChoiceController CreateChoice()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.choicePrefab, this.choiceContainer);
			gameObject.SetActive(true);
			RuleChoiceController component = gameObject.GetComponent<RuleChoiceController>();
			component.strip = this;
			return component;
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x00019CDB File Offset: 0x00017EDB
		private void DestroyChoice(RuleChoiceController choiceController)
		{
			UnityEngine.Object.Destroy(choiceController.gameObject);
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x000A94E0 File Offset: 0x000A76E0
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

		// Token: 0x06002361 RID: 9057 RVA: 0x000A955C File Offset: 0x000A775C
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

		// Token: 0x06002362 RID: 9058 RVA: 0x000A95C8 File Offset: 0x000A77C8
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

		// Token: 0x06002363 RID: 9059 RVA: 0x00019CE8 File Offset: 0x00017EE8
		private void OnEnable()
		{
			this.UpdatePosition();
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x000A9674 File Offset: 0x000A7874
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

		// Token: 0x04002641 RID: 9793
		public GameObject choicePrefab;

		// Token: 0x04002642 RID: 9794
		public RectTransform choiceContainer;

		// Token: 0x04002643 RID: 9795
		public RectTransform.Axis movementAxis = RectTransform.Axis.Vertical;

		// Token: 0x04002644 RID: 9796
		public float movementDuration = 0.1f;

		// Token: 0x04002645 RID: 9797
		private RuleDef currentRuleDef;

		// Token: 0x04002646 RID: 9798
		public readonly List<RuleChoiceController> choiceControllers = new List<RuleChoiceController>();

		// Token: 0x04002647 RID: 9799
		public int currentDisplayChoiceIndex;

		// Token: 0x04002648 RID: 9800
		private float velocity;

		// Token: 0x04002649 RID: 9801
		private float currentPosition;
	}
}
