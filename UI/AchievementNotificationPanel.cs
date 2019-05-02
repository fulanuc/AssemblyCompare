using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A5 RID: 1445
	public class AchievementNotificationPanel : MonoBehaviour
	{
		// Token: 0x06002072 RID: 8306 RVA: 0x000179E6 File Offset: 0x00015BE6
		private void Awake()
		{
			AchievementNotificationPanel.instancesList.Add(this);
			this.onStart.Invoke();
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x000179FE File Offset: 0x00015BFE
		private void OnDestroy()
		{
			AchievementNotificationPanel.instancesList.Remove(this);
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Update()
		{
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00017A0C File Offset: 0x00015C0C
		public void SetAchievementDef(AchievementDef achievementDef)
		{
			this.achievementIconImage.sprite = achievementDef.GetAchievedIcon();
			this.achievementName.text = Language.GetString(achievementDef.nameToken);
			this.achievementDescription.text = Language.GetString(achievementDef.descriptionToken);
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00017A4B File Offset: 0x00015C4B
		private static Canvas GetUserCanvas(LocalUser localUser)
		{
			if (Run.instance)
			{
				return localUser.cameraRigController.hud.GetComponent<Canvas>();
			}
			return RoR2Application.instance.mainCanvas;
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x00017A74 File Offset: 0x00015C74
		private static bool IsAppropriateTimeToDisplayUserAchievementNotification(LocalUser localUser)
		{
			return !GameOverController.instance;
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x00017A83 File Offset: 0x00015C83
		private static void DispatchAchievementNotification(Canvas canvas, AchievementDef achievementDef)
		{
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/AchievementNotificationPanel"), canvas.transform).GetComponent<AchievementNotificationPanel>().SetAchievementDef(achievementDef);
			Util.PlaySound(achievementDef.GetAchievementSoundString(), RoR2Application.instance.gameObject);
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x00017ABB File Offset: 0x00015CBB
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += AchievementNotificationPanel.StaticFixedUpdate;
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x0009DFD8 File Offset: 0x0009C1D8
		private static void StaticFixedUpdate()
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser != null && localUser.userProfile.hasUnviewedAchievement)
				{
					Canvas canvas = AchievementNotificationPanel.GetUserCanvas(localUser);
					if (!AchievementNotificationPanel.instancesList.Any((AchievementNotificationPanel instance) => instance.transform.parent == canvas.transform) && AchievementNotificationPanel.IsAppropriateTimeToDisplayUserAchievementNotification(localUser))
					{
						string text = (localUser != null) ? localUser.userProfile.PopNextUnviewedAchievementName() : null;
						if (text != null)
						{
							AchievementDef achievementDef = AchievementManager.GetAchievementDef(text);
							if (achievementDef != null)
							{
								AchievementNotificationPanel.DispatchAchievementNotification(AchievementNotificationPanel.GetUserCanvas(localUser), achievementDef);
							}
						}
					}
				}
			}
		}

		// Token: 0x040022F7 RID: 8951
		private static readonly List<AchievementNotificationPanel> instancesList = new List<AchievementNotificationPanel>();

		// Token: 0x040022F8 RID: 8952
		public Image achievementIconImage;

		// Token: 0x040022F9 RID: 8953
		public TextMeshProUGUI achievementName;

		// Token: 0x040022FA RID: 8954
		public TextMeshProUGUI achievementDescription;

		// Token: 0x040022FB RID: 8955
		public UnityEvent onStart;
	}
}
