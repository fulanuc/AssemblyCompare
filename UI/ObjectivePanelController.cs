using System;
using System.Collections.Generic;
using EntityStates.Missions.Goldshores;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200060E RID: 1550
	public class ObjectivePanelController : MonoBehaviour
	{
		// Token: 0x060022FC RID: 8956 RVA: 0x000A7EE8 File Offset: 0x000A60E8
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

		// Token: 0x060022FD RID: 8957 RVA: 0x000A7F4C File Offset: 0x000A614C
		private void AddObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.objectiveTrackerPrefab, this.objectiveTrackerContainer);
			gameObject.SetActive(true);
			objectiveTracker.owner = this;
			objectiveTracker.SetStrip(gameObject);
			this.objectiveTrackers.Add(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Add(objectiveTracker.sourceDescriptor, objectiveTracker);
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x0001982F File Offset: 0x00017A2F
		private void RemoveObjectiveTracker(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.objectiveTrackers.Remove(objectiveTracker);
			this.objectiveSourceToTrackerDictionary.Remove(objectiveTracker.sourceDescriptor);
			objectiveTracker.Retire();
			this.AddExitAnimation(objectiveTracker);
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000A7FA0 File Offset: 0x000A61A0
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

		// Token: 0x06002300 RID: 8960 RVA: 0x000A8100 File Offset: 0x000A6300
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

		// Token: 0x06002301 RID: 8961 RVA: 0x0001985D File Offset: 0x00017A5D
		private void Update()
		{
			this.RefreshObjectiveTrackers();
			this.RunExitAnimations();
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x0001986B File Offset: 0x00017A6B
		private void AddExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
		{
			this.exitAnimations.Add(new ObjectivePanelController.StripExitAnimation(objectiveTracker));
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x000A82BC File Offset: 0x000A64BC
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

		// Token: 0x040025DB RID: 9691
		public RectTransform objectiveTrackerContainer;

		// Token: 0x040025DC RID: 9692
		public GameObject objectiveTrackerPrefab;

		// Token: 0x040025DD RID: 9693
		public Sprite checkboxActiveSprite;

		// Token: 0x040025DE RID: 9694
		public Sprite checkboxSuccessSprite;

		// Token: 0x040025DF RID: 9695
		public Sprite checkboxFailSprite;

		// Token: 0x040025E0 RID: 9696
		private CharacterMaster currentMaster;

		// Token: 0x040025E1 RID: 9697
		private readonly List<ObjectivePanelController.ObjectiveTracker> objectiveTrackers = new List<ObjectivePanelController.ObjectiveTracker>();

		// Token: 0x040025E2 RID: 9698
		private Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker> objectiveSourceToTrackerDictionary = new Dictionary<ObjectivePanelController.ObjectiveSourceDescriptor, ObjectivePanelController.ObjectiveTracker>(EqualityComparer<ObjectivePanelController.ObjectiveSourceDescriptor>.Default);

		// Token: 0x040025E3 RID: 9699
		private readonly List<ObjectivePanelController.ObjectiveSourceDescriptor> objectiveSourceDescriptors = new List<ObjectivePanelController.ObjectiveSourceDescriptor>();

		// Token: 0x040025E4 RID: 9700
		private readonly List<ObjectivePanelController.StripExitAnimation> exitAnimations = new List<ObjectivePanelController.StripExitAnimation>();

		// Token: 0x0200060F RID: 1551
		public struct ObjectiveSourceDescriptor : IEquatable<ObjectivePanelController.ObjectiveSourceDescriptor>
		{
			// Token: 0x06002305 RID: 8965 RVA: 0x000A8350 File Offset: 0x000A6550
			public override int GetHashCode()
			{
				return (((this.source != null) ? this.source.GetHashCode() : 0) * 397 ^ ((this.master != null) ? this.master.GetHashCode() : 0)) * 397 ^ ((this.objectiveType != null) ? this.objectiveType.GetHashCode() : 0);
			}

			// Token: 0x06002306 RID: 8966 RVA: 0x000198B7 File Offset: 0x00017AB7
			public static bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor a, ObjectivePanelController.ObjectiveSourceDescriptor b)
			{
				return a.source == b.source && a.master == b.master && a.objectiveType == b.objectiveType;
			}

			// Token: 0x06002307 RID: 8967 RVA: 0x000198B7 File Offset: 0x00017AB7
			public bool Equals(ObjectivePanelController.ObjectiveSourceDescriptor other)
			{
				return this.source == other.source && this.master == other.master && this.objectiveType == other.objectiveType;
			}

			// Token: 0x06002308 RID: 8968 RVA: 0x000198F2 File Offset: 0x00017AF2
			public override bool Equals(object obj)
			{
				return obj != null && obj is ObjectivePanelController.ObjectiveSourceDescriptor && this.Equals((ObjectivePanelController.ObjectiveSourceDescriptor)obj);
			}

			// Token: 0x040025E5 RID: 9701
			public UnityEngine.Object source;

			// Token: 0x040025E6 RID: 9702
			public CharacterMaster master;

			// Token: 0x040025E7 RID: 9703
			public Type objectiveType;
		}

		// Token: 0x02000610 RID: 1552
		private class ObjectiveTracker
		{
			// Token: 0x17000310 RID: 784
			// (get) Token: 0x06002309 RID: 8969 RVA: 0x0001990F File Offset: 0x00017B0F
			// (set) Token: 0x0600230A RID: 8970 RVA: 0x00019917 File Offset: 0x00017B17
			public GameObject stripObject { get; private set; }

			// Token: 0x0600230B RID: 8971 RVA: 0x000A83C0 File Offset: 0x000A65C0
			public void SetStrip(GameObject stripObject)
			{
				this.stripObject = stripObject;
				this.label = stripObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				this.checkbox = stripObject.transform.Find("Checkbox").GetComponent<Image>();
				this.UpdateStrip();
			}

			// Token: 0x0600230C RID: 8972 RVA: 0x00019920 File Offset: 0x00017B20
			public string GetString()
			{
				if (this.IsDirty())
				{
					this.cachedString = this.GenerateString();
				}
				return this.cachedString;
			}

			// Token: 0x0600230D RID: 8973 RVA: 0x0001993C File Offset: 0x00017B3C
			protected virtual string GenerateString()
			{
				return Language.GetString(this.baseToken);
			}

			// Token: 0x0600230E RID: 8974 RVA: 0x00019949 File Offset: 0x00017B49
			protected virtual bool IsDirty()
			{
				return this.cachedString == null;
			}

			// Token: 0x0600230F RID: 8975 RVA: 0x00019954 File Offset: 0x00017B54
			public void Retire()
			{
				this.retired = true;
				this.OnRetired();
				this.UpdateStrip();
			}

			// Token: 0x06002310 RID: 8976 RVA: 0x000025F6 File Offset: 0x000007F6
			protected virtual void OnRetired()
			{
			}

			// Token: 0x06002311 RID: 8977 RVA: 0x000A8410 File Offset: 0x000A6610
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

			// Token: 0x06002312 RID: 8978 RVA: 0x000A84D0 File Offset: 0x000A66D0
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

			// Token: 0x040025E8 RID: 9704
			public ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor;

			// Token: 0x040025E9 RID: 9705
			public ObjectivePanelController owner;

			// Token: 0x040025EA RID: 9706
			public bool isRelevant;

			// Token: 0x040025EC RID: 9708
			protected Image checkbox;

			// Token: 0x040025ED RID: 9709
			protected TextMeshProUGUI label;

			// Token: 0x040025EE RID: 9710
			protected string cachedString;

			// Token: 0x040025EF RID: 9711
			protected string baseToken = "";

			// Token: 0x040025F0 RID: 9712
			protected bool retired;
		}

		// Token: 0x02000611 RID: 1553
		private class FindTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002314 RID: 8980 RVA: 0x0001997C File Offset: 0x00017B7C
			public FindTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FIND_TELEPORTER";
			}
		}

		// Token: 0x02000612 RID: 1554
		private class ActivateGoldshoreBeaconTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002315 RID: 8981 RVA: 0x0001998F File Offset: 0x00017B8F
			public ActivateGoldshoreBeaconTracker()
			{
				this.baseToken = "OBJECTIVE_GOLDSHORES_ACTIVATE_BEACONS";
			}

			// Token: 0x06002316 RID: 8982 RVA: 0x000199A2 File Offset: 0x00017BA2
			protected override string GenerateString()
			{
				return string.Format(Language.GetString(this.baseToken), GoldshoresMissionController.instance.beaconsActive, GoldshoresMissionController.instance.beaconsToSpawnOnMap);
			}

			// Token: 0x06002317 RID: 8983 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000613 RID: 1555
		private class DestroyTimeCrystals : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002318 RID: 8984 RVA: 0x000199D2 File Offset: 0x00017BD2
			public DestroyTimeCrystals()
			{
				this.baseToken = "OBJECTIVE_WEEKLYRUN_DESTROY_CRYSTALS";
			}

			// Token: 0x06002319 RID: 8985 RVA: 0x000A8540 File Offset: 0x000A6740
			protected override string GenerateString()
			{
				WeeklyRun weeklyRun = Run.instance as WeeklyRun;
				return string.Format(Language.GetString(this.baseToken), weeklyRun.crystalsKilled, weeklyRun.crystalsRequiredToKill);
			}

			// Token: 0x0600231A RID: 8986 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}
		}

		// Token: 0x02000614 RID: 1556
		private class ChargeTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x0600231B RID: 8987 RVA: 0x000199E5 File Offset: 0x00017BE5
			public ChargeTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_CHARGE_TELEPORTER";
			}

			// Token: 0x0600231C RID: 8988 RVA: 0x000A8580 File Offset: 0x000A6780
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

			// Token: 0x0600231D RID: 8989 RVA: 0x000A85D0 File Offset: 0x000A67D0
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

			// Token: 0x0600231E RID: 8990 RVA: 0x000199FF File Offset: 0x00017BFF
			private static int GetTeleporterPercent()
			{
				if (!TeleporterInteraction.instance)
				{
					return 0;
				}
				return Mathf.CeilToInt(TeleporterInteraction.instance.chargeFraction * 100f);
			}

			// Token: 0x0600231F RID: 8991 RVA: 0x000038B4 File Offset: 0x00001AB4
			protected override bool IsDirty()
			{
				return true;
			}

			// Token: 0x040025F1 RID: 9713
			private int lastPercent = -1;
		}

		// Token: 0x02000615 RID: 1557
		private class FinishTeleporterObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002320 RID: 8992 RVA: 0x00019A24 File Offset: 0x00017C24
			public FinishTeleporterObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_FINISH_TELEPORTER";
			}
		}

		// Token: 0x02000616 RID: 1558
		private class DefeatBossObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002321 RID: 8993 RVA: 0x00019A37 File Offset: 0x00017C37
			public DefeatBossObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_DEFEAT_BOSS";
			}
		}

		// Token: 0x02000617 RID: 1559
		private class StripExitAnimation
		{
			// Token: 0x06002322 RID: 8994 RVA: 0x000A8644 File Offset: 0x000A6844
			public StripExitAnimation(ObjectivePanelController.ObjectiveTracker objectiveTracker)
			{
				this.objectiveTracker = objectiveTracker;
				this.layoutElement = objectiveTracker.stripObject.GetComponent<LayoutElement>();
				this.canvasGroup = objectiveTracker.stripObject.GetComponent<CanvasGroup>();
				this.originalHeight = this.layoutElement.minHeight;
			}

			// Token: 0x06002323 RID: 8995 RVA: 0x000A8694 File Offset: 0x000A6894
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

			// Token: 0x040025F2 RID: 9714
			public float t;

			// Token: 0x040025F3 RID: 9715
			private readonly float originalHeight;

			// Token: 0x040025F4 RID: 9716
			public readonly ObjectivePanelController.ObjectiveTracker objectiveTracker;

			// Token: 0x040025F5 RID: 9717
			private readonly LayoutElement layoutElement;

			// Token: 0x040025F6 RID: 9718
			private readonly CanvasGroup canvasGroup;
		}
	}
}
