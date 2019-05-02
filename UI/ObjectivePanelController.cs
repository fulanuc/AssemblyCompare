using System;
using System.Collections.Generic;
using EntityStates.Missions.Goldshores;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000620 RID: 1568
	public class ObjectivePanelController : MonoBehaviour
	{
		// Token: 0x0600238C RID: 9100 RVA: 0x000A9564 File Offset: 0x000A7764
		public void SetCurrentMaster(CharacterMaster newMaster)
		{
			if (newMaster == this.currentMaster)
			{
				return;
			}
			for (int i = this.objectiveTrackers.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this.objectiveTrackers[i].stripObject);
			}
			this.objectiveTrackers.Clear();
			this.currentMaster = newMaster;
			this.RefreshObjectiveTrackers();
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x000A95C8 File Offset: 0x000A77C8
		private void AddObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.objectiveTrackerPrefab, this.objectiveTrackerContainer);
			gameObject.SetActive(true);
			objectiveTracker.owner = this;
			objectiveTracker.SetStrip(gameObject);
			this.objectiveTrackers.Add(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Add(objectiveTracker.sourceDescriptor, objectiveTracker);
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x00019EE6 File Offset: 0x000180E6
		private void RemoveObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.objectiveTrackers.Remove(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Remove(objectiveTracker.sourceDescriptor);
			objectiveTracker.Retire();
			this.AddExitAnimation(objectiveTracker);
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x000A961C File Offset: 0x000A781C
		private void RefreshObjectiveTrackers()
		{
			foreach (ObjectivePanelController.ObjectiveTracker objectiveTracker in this.objectiveTrackers)
			{
				objectiveTracker.isRelevant = false;
			}
			if (this.currentMaster)
			{
				this.GetObjectiveSources(this.currentMaster, this.objectiveSourceDescriptors);
				foreach (ObjectivePanelController.ObjectiveSourceDescriptor objectiveSourceDescriptor in this.objectiveSourceDescriptors)
				{
					ObjectivePanelController.ObjectiveTracker objectiveTracker2;
					if (this.objectiveSourceToTrackerDictionary.TryGetValue(objectiveSourceDescriptor, out objectiveTracker2))
					{
						objectiveTracker2.isRelevant = true;
					}
					else
					{
						ObjectivePanelController.ObjectiveTracker objectiveTracker3 = ObjectivePanelController.ObjectiveTracker.Instantiate(objectiveSourceDescriptor);
						objectiveTracker3.isRelevant = true;
						this.AddObjectiveTracker(objectiveTracker3);
					}
				}
			}
			for (int i = this.objectiveTrackers.Count - 1; i >= 0; i--)
			{
				if (!this.objectiveTrackers[i].isRelevant)
				{
					this.RemoveObjectiveTracker(this.objectiveTrackers[i]);
				}
			}
			foreach (ObjectivePanelController.ObjectiveTracker objectiveTracker4 in this.objectiveTrackers)
			{
				objectiveTracker4.UpdateStrip();
			}
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x000A977C File Offset: 0x000A797C
		private void GetObjectiveSources(CharacterMaster master, [NotNull] List<ObjectivePanelController.ObjectiveSourceDescriptor> output)
		{
			output.Clear();
			WeeklyRun weeklyRun = Run.instance as WeeklyRun;
			if (weeklyRun && weeklyRun.crystalsRequiredToKill > weeklyRun.crystalsKilled)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = Run.instance,
					master = master,
					objectiveType = typeof(ObjectivePanelController.DestroyTimeCrystals)
				});
			}
			TeleporterInteraction instance = TeleporterInteraction.instance;
			if (instance)
			{
				Type type = null;
				if (instance.isCharging)
				{
					type = typeof(ObjectivePanelController.ChargeTeleporterObjectiveTracker);
				}
				else if (instance.isCharged && !instance.isInFinalSequence)
				{
					type = typeof(ObjectivePanelController.FinishTeleporterObjectiveTracker);
				}
				else if (instance.isIdle)
				{
					type = typeof(ObjectivePanelController.FindTeleporterObjectiveTracker);
				}
				if (type != null)
				{
					output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
					{
						source = instance,
						master = master,
						objectiveType = type
					});
				}
			}
			if (BossGroup.instance && BossGroup.instance.readOnlyMembersList.Count != 0)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = BossGroup.instance,
					master = master,
					objectiveType = typeof(ObjectivePanelController.DefeatBossObjectiveTracker)
				});
			}
			if (GoldshoresMissionController.instance)
			{
				Type type2 = GoldshoresMissionController.instance.entityStateMachine.state.GetType();
				if (type2 == typeof(ActivateBeacons) || type2 == typeof(GoldshoresBossfight))
				{
					output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
					{
						source = GoldshoresMissionController.instance,
						master = master,
						objectiveType = typeof(ObjectivePanelController.ActivateGoldshoreBeaconTracker)
					});
				}
			}
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x00019F14 File Offset: 0x00018114
		private void Update()
		{
			this.RefreshObjectiveTrackers();
			this.RunExitAnimations();
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x00019F22 File Offset: 0x00018122
		private void AddExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.exitAnimations.Add(new ObjectivePanelController.StripExitAnimation(objectiveTracker));
		}

		// Token: 0x06002393 RID: 9107 RVA: 0x000A9938 File Offset: 0x000A7B38
		private void RunExitAnimations()
		{
			float deltaTime = Time.deltaTime;
			float num = 7f;
			float num2 = deltaTime / num;
			for (int i = this.exitAnimations.Count - 1; i >= 0; i--)
			{
				float num3 = Mathf.Min(this.exitAnimations[i].t + num2, 1f);
				this.exitAnimations[i].SetT(num3);
				if (num3 >= 1f)
				{
					UnityEngine.Object.Destroy(this.exitAnimations[i].objectiveTracker.stripObject);
					this.exitAnimations.RemoveAt(i);
				}
			}
		}

		// Token: 0x04002636 RID: 9782
		public RectTransform objectiveTrackerContainer;

		// Token: 0x04002637 RID: 9783
		public GameObject objectiveTrackerPrefab;

		// Token: 0x04002638 RID: 9784
		public Sprite checkboxActiveSprite;

		// Token: 0x04002639 RID: 9785
		public Sprite checkboxSuccessSprite;

		// Token: 0x0400263A RID: 9786
		public Sprite checkboxFailSprite;

		// Token: 0x0400263B RID: 9787
		private CharacterMaster currentMaster;

		// Token: 0x0400263C RID: 9788
		private readonly List<ObjectivePanelController.ObjectiveTracker> objectiveTrackers = new List<ObjectivePanelController.ObjectiveTracker>();

		// Token: 0x0400263D RID: 9789
		private Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker> objectiveSourceToTrackerDictionary = new Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker>(EqualityComparer<ObjectivePanelController.ObjectiveSourceDescriptor>.Default);

		// Token: 0x0400263E RID: 9790
		private readonly List<ObjectivePanelController.ObjectiveSourceDescriptor> objectiveSourceDescriptors = new List<ObjectivePanelController.ObjectiveSourceDescriptor>();

		// Token: 0x0400263F RID: 9791
		private readonly List<ObjectivePanelController.StripExitAnimation> exitAnimations = new List<ObjectivePanelController.StripExitAnimation>();

		// Token: 0x02000621 RID: 1569
		public struct ObjectiveSourceDescriptor : IEquatable<ObjectivePanelController.ObjectiveSourceDescriptor>
		{
			// Token: 0x06002395 RID: 9109 RVA: 0x000A99CC File Offset: 0x000A7BCC
			public override int GetHashCode()
			{
				return (((this.source != null) ? this.source.GetHashCode() : 0) * 397 ^ ((this.master != null) ? this.master.GetHashCode() : 0)) * 397 ^ ((this.objectiveType != null) ? this.objectiveType.GetHashCode() : 0);
			}

			// Token: 0x06002396 RID: 9110 RVA: 0x00019F6E File Offset: 0x0001816E
			public static bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor a, ObjectivePanelController.ObjectiveSourceDescriptor b)
			{
				return a.source == b.source && a.master == b.master && a.objectiveType == b.objectiveType;
			}

			// Token: 0x06002397 RID: 9111 RVA: 0x00019F6E File Offset: 0x0001816E
			public bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor other)
			{
				return this.source == other.source && this.master == other.master && this.objectiveType == other.objectiveType;
			}

			// Token: 0x06002398 RID: 9112 RVA: 0x00019FA9 File Offset: 0x000181A9
			public override bool Equals(object obj)
			{
				return obj != null && obj is ObjectivePanelController.ObjectiveSourceDescriptor && this.Equals((ObjectivePanelController.ObjectiveSourceDescriptor)obj);
			}

			// Token: 0x04002640 RID: 9792
			public UnityEngine.Object source;

			// Token: 0x04002641 RID: 9793
			public CharacterMaster master;

			// Token: 0x04002642 RID: 9794
			public Type objectiveType;
		}

		// Token: 0x02000622 RID: 1570
		private class ObjectiveTracker
		{
			// Token: 0x17000322 RID: 802
			// (get) Token: 0x06002399 RID: 9113 RVA: 0x00019FC6 File Offset: 0x000181C6
			// (set) Token: 0x0600239A RID: 9114 RVA: 0x00019FCE File Offset: 0x000181CE
			public GameObject stripObject { get; private set; }

			// Token: 0x0600239B RID: 9115 RVA: 0x000A9A3C File Offset: 0x000A7C3C
			public void SetStrip(GameObject stripObject)
			{
				this.stripObject = stripObject;
				this.label = stripObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				this.checkbox = stripObject.transform.Find("Checkbox").GetComponent<Image>();
				this.UpdateStrip();
			}

			// Token: 0x0600239C RID: 9116 RVA: 0x00019FD7 File Offset: 0x000181D7
			public string GetString()
			{
				if (this.IsDirty())
				{
					this.cachedString = this.GenerateString();
				}
				return this.cachedString;
			}

			// Token: 0x0600239D RID: 9117 RVA: 0x00019FF3 File Offset: 0x000181F3
			protected virtual string GenerateString()
			{
				return Language.GetString(this.baseToken);
			}

			// Token: 0x0600239E RID: 9118 RVA: 0x0001A000 File Offset: 0x00018200
			protected virtual bool IsDirty()
			{
				return this.cachedString == null;
			}

			// Token: 0x0600239F RID: 9119 RVA: 0x0001A00B File Offset: 0x0001820B
			public void Retire()
			{
				this.retired = true;
				this.OnRetired();
				this.UpdateStrip();
			}

			// Token: 0x060023A0 RID: 9120 RVA: 0x000025DA File Offset: 0x000007DA
			protected virtual void OnRetired()
			{
			}

			// Token: 0x060023A1 RID: 9121 RVA: 0x000A9A8C File Offset: 0x000A7C8C
			public virtual void UpdateStrip()
			{
				if (this.label)
				{
					this.label.text = this.GetString();
					this.label.color = (this.retired ? Color.gray : Color.white);
					if (this.retired)
					{
						this.label.fontStyle |= FontStyles.Strikethrough;
					}
				}
				if (this.checkbox)
				{
					this.checkbox.sprite = (this.retired ? this.owner.checkboxSuccessSprite : this.owner.checkboxActiveSprite);
					this.checkbox.color = (this.retired ? Color.yellow : Color.white);
				}
			}

			// Token: 0x060023A2 RID: 9122 RVA: 0x000A9B4C File Offset: 0x000A7D4C
			public static ObjectivePanelController.ObjectiveTracker Instantiate(ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor)
			{
				if (sourceDescriptor.objectiveType != null && sourceDescriptor.objectiveType.IsSubclassOf(typeof(ObjectivePanelController.ObjectiveTracker)))
				{
					ObjectivePanelController.ObjectiveTracker objectiveTracker = (ObjectivePanelController.ObjectiveTracker)Activator.CreateInstance(sourceDescriptor.objectiveType);
					objectiveTracker.sourceDescriptor = sourceDescriptor;
					return objectiveTracker;
				}
				string format = "Bad objectiveType {0}";
				object[] array = new object[1];
				int num = 0;
				Type objectiveType = sourceDescriptor.objectiveType;
				array[num] = ((objectiveType != null) ? objectiveType.FullName : null);
				Debug.LogFormat(format, array);
				return null;
			}

			// Token: 0x04002643 RID: 9795
			public ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor;

			// Token: 0x04002644 RID: 9796
			public ObjectivePanelController owner;

			// Token: 0x04002645 RID: 9797
			public bool isRelevant;

			// Token: 0x04002647 RID: 9799
			protected Image checkbox;

			// Token: 0x04002648 RID: 9800
			protected TextMeshProUGUI label;

			// Token: 0x04002649 RID: 9801
			protected string cachedString;

			// Token: 0x0400264A RID: 9802
			protected string baseToken = "";

			// Token: 0x0400264B RID: 9803
			protected bool retired;
		}

		// Token: 0x02000623 RID: 1571
		private class FindTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023A4 RID: 9124 RVA: 0x0001A033 File Offset: 0x00018233
			public FindTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FIND_TELEPORTER";
			}
		}

		// Token: 0x02000624 RID: 1572
		private class ActivateGoldshoreBeaconTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023A5 RID: 9125 RVA: 0x0001A046 File Offset: 0x00018246
			public ActivateGoldshoreBeaconTracker()
			{
				this.baseToken = "OBJECTIVE_GOLDSHORES_ACTIVATE_BEACONS";
			}

			// Token: 0x060023A6 RID: 9126 RVA: 0x0001A059 File Offset: 0x00018259
			protected override string GenerateString()
			{
				return string.Format(Language.GetString(this.baseToken), GoldshoresMissionController.instance.beaconsActive, GoldshoresMissionController.instance.beaconsToSpawnOnMap);
			}

			// Token: 0x060023A7 RID: 9127 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000625 RID: 1573
		private class DestroyTimeCrystals : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023A8 RID: 9128 RVA: 0x0001A089 File Offset: 0x00018289
			public DestroyTimeCrystals()
			{
				this.baseToken = "OBJECTIVE_WEEKLYRUN_DESTROY_CRYSTALS";
			}

			// Token: 0x060023A9 RID: 9129 RVA: 0x000A9BBC File Offset: 0x000A7DBC
			protected override string GenerateString()
			{
				WeeklyRun weeklyRun = Run.instance as WeeklyRun;
				return string.Format(Language.GetString(this.baseToken), weeklyRun.crystalsKilled, weeklyRun.crystalsRequiredToKill);
			}

			// Token: 0x060023AA RID: 9130 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000626 RID: 1574
		private class ChargeTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023AB RID: 9131 RVA: 0x0001A09C File Offset: 0x0001829C
			public ChargeTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_CHARGE_TELEPORTER";
			}

			// Token: 0x060023AC RID: 9132 RVA: 0x000A9BFC File Offset: 0x000A7DFC
			private bool ShouldBeFlashing()
			{
				bool flag = true;
				if (TeleporterInteraction.instance)
				{
					CharacterMaster master = this.sourceDescriptor.master;
					if (master)
					{
						CharacterBody body = master.GetBody();
						if (body)
						{
							flag = TeleporterInteraction.instance.IsInChargingRange(body);
						}
					}
				}
				return !flag;
			}

			// Token: 0x060023AD RID: 9133 RVA: 0x000A9C4C File Offset: 0x000A7E4C
			protected override string GenerateString()
			{
				this.lastPercent = ObjectivePanelController.ChargeTeleporterObjectiveTracker.GetTeleporterPercent();
				string text = string.Format(Language.GetString(this.baseToken), this.lastPercent);
				if (this.ShouldBeFlashing())
				{
					text = string.Format(Language.GetString("OBJECTIVE_CHARGE_TELEPORTER_OOB"), this.lastPercent);
					if ((int)(Time.time * 12f) % 2 == 0)
					{
						text = string.Format("<style=cDeath>{0}</style>", text);
					}
				}
				return text;
			}

			// Token: 0x060023AE RID: 9134 RVA: 0x0001A0B6 File Offset: 0x000182B6
			private static int GetTeleporterPercent()
			{
				if (!TeleporterInteraction.instance)
				{
					return 0;
				}
				return Mathf.CeilToInt(TeleporterInteraction.instance.chargeFraction * 100f);
			}

			// Token: 0x060023AF RID: 9135 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}

			// Token: 0x0400264C RID: 9804
			private int lastPercent = -1;
		}

		// Token: 0x02000627 RID: 1575
		private class FinishTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023B0 RID: 9136 RVA: 0x0001A0DB File Offset: 0x000182DB
			public FinishTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FINISH_TELEPORTER";
			}
		}

		// Token: 0x02000628 RID: 1576
		private class DefeatBossObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060023B1 RID: 9137 RVA: 0x0001A0EE File Offset: 0x000182EE
			public DefeatBossObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_DEFEAT_BOSS";
			}
		}

		// Token: 0x02000629 RID: 1577
		private class StripExitAnimation
		{
			// Token: 0x060023B2 RID: 9138 RVA: 0x000A9CC0 File Offset: 0x000A7EC0
			public StripExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
			{
				this.objectiveTracker = objectiveTracker;
				this.layoutElement = objectiveTracker.stripObject.GetComponent<LayoutElement>();
				this.canvasGroup = objectiveTracker.stripObject.GetComponent<CanvasGroup>();
				this.originalHeight = this.layoutElement.minHeight;
			}

			// Token: 0x060023B3 RID: 9139 RVA: 0x000A9D10 File Offset: 0x000A7F10
			public void SetT(float newT)
			{
				this.t = newT;
				float alpha = Mathf.Clamp01(Util.Remap(this.t, 0.5f, 0.75f, 1f, 0f));
				this.canvasGroup.alpha = alpha;
				float num = Mathf.Clamp01(Util.Remap(this.t, 0.75f, 1f, 1f, 0f));
				num *= num;
				this.layoutElement.minHeight = num * this.originalHeight;
				this.layoutElement.preferredHeight = this.layoutElement.minHeight;
				this.layoutElement.flexibleHeight = 0f;
			}

			// Token: 0x0400264D RID: 9805
			public float t;

			// Token: 0x0400264E RID: 9806
			private readonly float originalHeight;

			// Token: 0x0400264F RID: 9807
			public readonly ObjectivePanelController.ObjectiveTracker objectiveTracker;

			// Token: 0x04002650 RID: 9808
			private readonly LayoutElement layoutElement;

			// Token: 0x04002651 RID: 9809
			private readonly CanvasGroup canvasGroup;
		}
	}
}
