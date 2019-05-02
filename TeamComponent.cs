using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F5 RID: 1013
	[DisallowMultipleComponent]
	public class TeamComponent : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600163C RID: 5692 RVA: 0x00010952 File Offset: 0x0000EB52
		// (set) Token: 0x0600163B RID: 5691 RVA: 0x0001092A File Offset: 0x0000EB2A
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

		// Token: 0x0600163D RID: 5693 RVA: 0x0001095A File Offset: 0x0000EB5A
		private static bool TeamIsValid(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count;
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x00010966 File Offset: 0x0000EB66
		private void OnChangeTeam(TeamIndex newTeamIndex)
		{
			this.OnLeaveTeam(this.oldTeamIndex);
			this.OnJoinTeam(newTeamIndex);
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x0001097B File Offset: 0x0000EB7B
		private void OnLeaveTeam(TeamIndex oldTeamIndex)
		{
			if (TeamComponent.TeamIsValid(oldTeamIndex))
			{
				TeamComponent.teamsList[(int)oldTeamIndex].Remove(this);
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x00076918 File Offset: 0x00074B18
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

		// Token: 0x06001641 RID: 5697 RVA: 0x00076978 File Offset: 0x00074B78
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

		// Token: 0x06001642 RID: 5698 RVA: 0x00076B00 File Offset: 0x00074D00
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

		// Token: 0x06001643 RID: 5699 RVA: 0x00010993 File Offset: 0x0000EB93
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

		// Token: 0x06001644 RID: 5700 RVA: 0x000109B9 File Offset: 0x0000EBB9
		public void Start()
		{
			this.SetupIndicator();
			if (this.oldTeamIndex != this.teamIndex)
			{
				this.OnChangeTeam(this.teamIndex);
			}
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000109DB File Offset: 0x0000EBDB
		private void OnDestroy()
		{
			this.teamIndex = TeamIndex.None;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0000A05D File Offset: 0x0000825D
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x000109E4 File Offset: 0x0000EBE4
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.teamIndex);
			return initialState || base.syncVarDirtyBits > 0u;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x00010A00 File Offset: 0x0000EC00
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			this.teamIndex = reader.ReadTeamIndex();
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x00010A0E File Offset: 0x0000EC0E
		public static ReadOnlyCollection<TeamComponent> GetTeamMembers(TeamIndex teamIndex)
		{
			if (!TeamComponent.TeamIsValid(teamIndex))
			{
				return TeamComponent.emptyTeamMembers;
			}
			return TeamComponent.readonlyTeamsList[(int)teamIndex];
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00076B6C File Offset: 0x00074D6C
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

		// Token: 0x0600164B RID: 5707 RVA: 0x00076B98 File Offset: 0x00074D98
		[ConCommand(commandName = "kill_team", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Kills all characters on the named team.")]
		private static void CCKillTeam(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				return;
			}
			try
			{
				foreach (TeamComponent teamComponent in new List<TeamComponent>(TeamComponent.GetTeamMembers((TeamIndex)Enum.Parse(typeof(TeamIndex), args[0], true))))
				{
					if (teamComponent)
					{
						HealthComponent component = teamComponent.GetComponent<HealthComponent>();
						if (component)
						{
							component.Suicide(null);
						}
					}
				}
			}
			catch (ArgumentException)
			{
			}
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0400198A RID: 6538
		[SerializeField]
		private TeamIndex _teamIndex = TeamIndex.None;

		// Token: 0x0400198B RID: 6539
		private TeamIndex oldTeamIndex = TeamIndex.None;

		// Token: 0x0400198C RID: 6540
		private GameObject indicator;

		// Token: 0x0400198D RID: 6541
		private static List<TeamComponent>[] teamsList;

		// Token: 0x0400198E RID: 6542
		private static ReadOnlyCollection<TeamComponent>[] readonlyTeamsList;

		// Token: 0x0400198F RID: 6543
		private HurtBoxGroup hurtBoxGroup;

		// Token: 0x04001990 RID: 6544
		private static ReadOnlyCollection<TeamComponent> emptyTeamMembers = new List<TeamComponent>().AsReadOnly();
	}
}
