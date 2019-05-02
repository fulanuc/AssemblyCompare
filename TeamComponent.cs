using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FB RID: 1019
	[DisallowMultipleComponent]
	public class TeamComponent : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x17000207 RID: 519
		// (get) Token: 0x0600167C RID: 5756 RVA: 0x00010D6B File Offset: 0x0000EF6B
		// (set) Token: 0x0600167B RID: 5755 RVA: 0x00010D43 File Offset: 0x0000EF43
		public TeamIndex teamIndex
		{
			get
			{
				return this._teamIndex;
			}
			set
			{
				if (this._teamIndex == value)
				{
					return;
				}
				this._teamIndex = value;
				base.SetDirtyBit(1u);
				if (Application.isPlaying)
				{
					this.OnChangeTeam(value);
				}
			}
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00010D73 File Offset: 0x0000EF73
		private static bool TeamIsValid(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00010D7F File Offset: 0x0000EF7F
		private void OnChangeTeam(TeamIndex newTeamIndex)
		{
			this.OnLeaveTeam(this.oldTeamIndex);
			this.OnJoinTeam(newTeamIndex);
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x00010D94 File Offset: 0x0000EF94
		private void OnLeaveTeam(TeamIndex oldTeamIndex)
		{
			if (TeamComponent.TeamIsValid(oldTeamIndex))
			{
				TeamComponent.teamsList[(int)oldTeamIndex].Remove(this);
			}
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00076F50 File Offset: 0x00075150
		private void OnJoinTeam(TeamIndex newTeamIndex)
		{
			if (TeamComponent.TeamIsValid(newTeamIndex))
			{
				TeamComponent.teamsList[(int)newTeamIndex].Add(this);
			}
			this.SetupIndicator();
			HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
			HurtBox[] array = (hurtBoxGroup != null) ? hurtBoxGroup.hurtBoxes : null;
			if (array != null)
			{
				HurtBox[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].teamIndex = newTeamIndex;
				}
			}
			this.oldTeamIndex = newTeamIndex;
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00076FB0 File Offset: 0x000751B0
		private void SetupIndicator()
		{
			if (this.indicator)
			{
				return;
			}
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component)
			{
				TeamComponent component2 = component.GetComponent<TeamComponent>();
				if (component2)
				{
					CharacterMaster master = component.master;
					bool flag = master && master.isBoss;
					GameObject gameObject = null;
					if (master && component2.teamIndex == TeamIndex.Player)
					{
						bool flag2 = false;
						PlayerCharacterMasterController component3 = master.GetComponent<PlayerCharacterMasterController>();
						if (component3)
						{
							flag2 = true;
							GameObject networkUserObject = component3.networkUserObject;
							if (networkUserObject)
							{
								NetworkIdentity component4 = networkUserObject.GetComponent<NetworkIdentity>();
								if (component4)
								{
									bool isLocalPlayer = component4.isLocalPlayer;
								}
							}
						}
						Vector3 position = component.transform.position;
						component.GetComponent<Collider>();
						if (flag2)
						{
							gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/PlayerPositionIndicator");
						}
						else
						{
							gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/NPCPositionIndicator");
						}
						this.indicator = UnityEngine.Object.Instantiate<GameObject>(gameObject, position, Quaternion.identity, component.transform);
					}
					else if (flag)
					{
						gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/BossPositionIndicator");
					}
					if (this.indicator)
					{
						UnityEngine.Object.Destroy(this.indicator);
						this.indicator = null;
					}
					if (gameObject)
					{
						this.indicator = UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform);
						this.indicator.GetComponent<PositionIndicator>().targetTransform = component.coreTransform;
						Nameplate component5 = this.indicator.GetComponent<Nameplate>();
						if (component5)
						{
							component5.SetBody(component);
						}
					}
				}
			}
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00077138 File Offset: 0x00075338
		static TeamComponent()
		{
			TeamComponent.teamsList = new List<TeamComponent>[3];
			TeamComponent.readonlyTeamsList = new ReadOnlyCollection<TeamComponent>[TeamComponent.teamsList.Length];
			for (int i = 0; i < TeamComponent.teamsList.Length; i++)
			{
				TeamComponent.teamsList[i] = new List<TeamComponent>();
				TeamComponent.readonlyTeamsList[i] = TeamComponent.teamsList[i].AsReadOnly();
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00010DAC File Offset: 0x0000EFAC
		private void Awake()
		{
			ModelLocator component = base.GetComponent<ModelLocator>();
			HurtBoxGroup hurtBoxGroup;
			if (component == null)
			{
				hurtBoxGroup = null;
			}
			else
			{
				Transform modelTransform = component.modelTransform;
				hurtBoxGroup = ((modelTransform != null) ? modelTransform.GetComponent<HurtBoxGroup>() : null);
			}
			this.hurtBoxGroup = hurtBoxGroup;
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00010DD2 File Offset: 0x0000EFD2
		public void Start()
		{
			this.SetupIndicator();
			if (this.oldTeamIndex != this.teamIndex)
			{
				this.OnChangeTeam(this.teamIndex);
			}
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00010DF4 File Offset: 0x0000EFF4
		private void OnDestroy()
		{
			this.teamIndex = TeamIndex.None;
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x0000A0C2 File Offset: 0x000082C2
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x00010DFD File Offset: 0x0000EFFD
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.teamIndex);
			return initialState || base.syncVarDirtyBits > 0u;
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x00010E19 File Offset: 0x0000F019
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			this.teamIndex = reader.ReadTeamIndex();
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00010E27 File Offset: 0x0000F027
		public static ReadOnlyCollection<TeamComponent> GetTeamMembers(TeamIndex teamIndex)
		{
			if (!TeamComponent.TeamIsValid(teamIndex))
			{
				return TeamComponent.emptyTeamMembers;
			}
			return TeamComponent.readonlyTeamsList[(int)teamIndex];
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x000771A4 File Offset: 0x000753A4
		public static TeamIndex GetObjectTeam(GameObject gameObject)
		{
			if (gameObject)
			{
				TeamComponent component = gameObject.GetComponent<TeamComponent>();
				if (component)
				{
					return component.teamIndex;
				}
			}
			return TeamIndex.Neutral;
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x040019B3 RID: 6579
		[SerializeField]
		private TeamIndex _teamIndex = TeamIndex.None;

		// Token: 0x040019B4 RID: 6580
		private TeamIndex oldTeamIndex = TeamIndex.None;

		// Token: 0x040019B5 RID: 6581
		private GameObject indicator;

		// Token: 0x040019B6 RID: 6582
		private static List<TeamComponent>[] teamsList;

		// Token: 0x040019B7 RID: 6583
		private static ReadOnlyCollection<TeamComponent>[] readonlyTeamsList;

		// Token: 0x040019B8 RID: 6584
		private HurtBoxGroup hurtBoxGroup;

		// Token: 0x040019B9 RID: 6585
		private static ReadOnlyCollection<TeamComponent> emptyTeamMembers = new List<TeamComponent>().AsReadOnly();
	}
}
