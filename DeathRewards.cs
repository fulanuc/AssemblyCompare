using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C5 RID: 709
	[RequireComponent(typeof(CharacterBody))]
	public class DeathRewards : MonoBehaviour
	{
		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0000B23A File Offset: 0x0000943A
		// (set) Token: 0x06000E6E RID: 3694 RVA: 0x0000B265 File Offset: 0x00009465
		[HideInInspector]
		public uint goldReward
		{
			get
			{
				if (!this.characterBody.master)
				{
					return this.fallbackGold;
				}
				return this.characterBody.master.money;
			}
			set
			{
				if (this.characterBody.master)
				{
					this.characterBody.master.money = value;
					return;
				}
				this.fallbackGold = value;
			}
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x0000B292 File Offset: 0x00009492
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x00058BD8 File Offset: 0x00056DD8
		private void OnKilled(DamageInfo damageInfo)
		{
			CharacterBody component = base.GetComponent<CharacterBody>();
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter"), new EffectData
			{
				origin = base.transform.position,
				genericFloat = this.goldReward,
				scale = (component ? component.radius : 1f)
			}, true);
			if (damageInfo.attacker)
			{
				TeamComponent component2 = base.GetComponent<TeamComponent>();
				CharacterBody component3 = damageInfo.attacker.GetComponent<CharacterBody>();
				if (component3)
				{
					CharacterMaster master = component3.master;
					TeamIndex objectTeam = TeamComponent.GetObjectTeam(component3.gameObject);
					TeamManager.instance.GiveTeamMoney(objectTeam, this.goldReward);
					float num = 1f;
					if (component2)
					{
						num = 1f + (TeamManager.instance.GetTeamLevel(component2.teamIndex) - 1f) * 0.3f;
					}
					ExperienceManager.instance.AwardExperience(base.transform.position, component3.GetComponent<CharacterBody>(), (ulong)((uint)(this.expReward * num)));
				}
				if (this.logUnlockableName != "" && Run.instance.selectedDifficulty > DifficultyIndex.Easy && Run.instance.CanUnlockableBeGrantedThisRun(this.logUnlockableName))
				{
					CharacterBody component4 = base.GetComponent<CharacterBody>();
					if (Util.CheckRoll((component4 && component4.isChampion) ? 3f : 1f, component3.master))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/LogPickup"), base.transform.position, UnityEngine.Random.rotation);
						gameObject.GetComponentInChildren<UnlockPickup>().unlockableName = this.logUnlockableName;
						gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
						NetworkServer.Spawn(gameObject);
					}
				}
			}
		}

		// Token: 0x04001250 RID: 4688
		private uint fallbackGold;

		// Token: 0x04001251 RID: 4689
		[HideInInspector]
		public uint expReward;

		// Token: 0x04001252 RID: 4690
		public string logUnlockableName = "";

		// Token: 0x04001253 RID: 4691
		public SerializablePickupIndex bossPickup;

		// Token: 0x04001254 RID: 4692
		private CharacterBody characterBody;
	}
}
