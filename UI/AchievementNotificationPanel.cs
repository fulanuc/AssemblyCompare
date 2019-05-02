using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B7 RID: 1463
	public class AchievementNotificationPanel : MonoBehaviour
	{
		// Token: 0x06002103 RID: 8451 RVA: 0x000180E0 File Offset: 0x000162E0
		private void Awake()
		{
			AchievementNotificationPanel.instancesList.Add(this);
			this.onStart.Invoke();
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x000180F8 File Offset: 0x000162F8
		private void OnDestroy()
		{
			AchievementNotificationPanel.instancesList.Remove(this);
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000025DA File Offset: 0x000007DA
		private void Update()
		{
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x00018106 File Offset: 0x00016306
		public void SetAchievementDef(AchievementDef achievementDef)
		{
			this.achievementIconImage.sprite = achievementDef.GetAchievedIcon();
			this.achievementName.text = Language.GetString(achievementDef.nameToken);
			this.achievementDescription.text = Language.GetString(achievementDef.descriptionToken);
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x00018145 File Offset: 0x00016345
		private static Canvas GetUserCanvas(LocalUser localUser)
		{
			if (Run.instance)
			{
				return localUser.cameraRigController.hud.GetComponent<Canvas>();
			}
			return RoR2Application.instance.mainCanvas;
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x0001816E File Offset: 0x0001636E
		private static bool IsAppropriateTimeToDisplayUserAchievementNotification(LocalUser localUser)
		{
			return !GameOverController.instance;
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x0001817D File Offset: 0x0001637D
		private static void DispatchAchievementNotification(Canvas canvas, AchievementDef achievementDef)
		{
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/AchievementNotificationPanel"), canvas.transform).GetComponent<AchievementNotificationPanel>().SetAchievementDef(achievementDef);
			Util.PlaySound(achievementDef.GetAchievementSoundString(), RoR2Application.instance.gameObject);
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x000181B5 File Offset: 0x000163B5
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += AchievementNotificationPanel.StaticFixedUpdate;
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x0009F5AC File Offset: 0x0009D7AC
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

		// Token: 0x0400234B RID: 9035
		private static readonly List<AchievementNotificationPanel> instancesList = new List<AchievementNotificationPanel>();

		// Token: 0x0400234C RID: 9036
		public Image achievementIconImage;

		// Token: 0x0400234D RID: 9037
		public TextMeshProUGUI achievementName;

		// Token: 0x0400234E RID: 9038
		public TextMeshProUGUI achievementDescription;

		// Token: 0x0400234F RID: 9039
		public UnityEvent onStart;
	}
}
