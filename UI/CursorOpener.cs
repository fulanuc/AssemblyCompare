using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005CB RID: 1483
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CursorOpener : MonoBehaviour
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06002146 RID: 8518 RVA: 0x000183B8 File Offset: 0x000165B8
		// (set) Token: 0x06002147 RID: 8519 RVA: 0x000A0B00 File Offset: 0x0009ED00
		private bool opening
		{
			get
			{
				return this.linkedEventSystem;
			}
			set
			{
				MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
				if (value && this.linkedEventSystem != eventSystem && this.linkedEventSystem)
				{
					this.opening = false;
				}
				if (this.linkedEventSystem != value)
				{
					if (value)
					{
						if (this.eventSystemLocator.eventSystem)
						{
							if (this.openForAllEventSystems)
							{
								using (IEnumerator<MPEventSystem> enumerator = MPEventSystem.readOnlyInstancesList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										MPEventSystem mpeventSystem = enumerator.Current;
										mpeventSystem.cursorOpenerCount++;
									}
									goto IL_A4;
								}
							}
							eventSystem.cursorOpenerCount++;
							IL_A4:
							this.linkedEventSystem = eventSystem;
							return;
						}
					}
					else
					{
						if (this.openForAllEventSystems)
						{
							using (IEnumerator<MPEventSystem> enumerator = MPEventSystem.readOnlyInstancesList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MPEventSystem mpeventSystem2 = enumerator.Current;
									mpeventSystem2.cursorOpenerCount--;
								}
								goto IL_FB;
							}
						}
						this.linkedEventSystem.cursorOpenerCount--;
						IL_FB:
						this.linkedEventSystem = null;
					}
				}
			}
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000183C5 File Offset: 0x000165C5
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.openForAllEventSystems = this.eventSystemLocator.eventSystemProvider.fallBackToMainEventSystem;
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x000183E9 File Offset: 0x000165E9
		private void Start()
		{
			this.opening = true;
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x000183E9 File Offset: 0x000165E9
		private void OnEnable()
		{
			this.opening = true;
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x000183F2 File Offset: 0x000165F2
		private void OnDisable()
		{
			this.opening = false;
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x000183FB File Offset: 0x000165FB
		[AssetCheck(typeof(CursorOpener))]
		private static void CheckCursorOpener(ProjectIssueChecker projectIssueChecker, UnityEngine.Object asset)
		{
			if (!((CursorOpener)asset).GetComponent<MPEventSystemLocator>())
			{
				projectIssueChecker.Log("Missing MPEventSystemLocator.", null);
			}
		}

		// Token: 0x040023CA RID: 9162
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040023CB RID: 9163
		private MPEventSystem linkedEventSystem;

		// Token: 0x040023CC RID: 9164
		private bool openForAllEventSystems;
	}
}
