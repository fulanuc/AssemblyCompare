using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005DD RID: 1501
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CursorOpener : MonoBehaviour
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060021D7 RID: 8663 RVA: 0x00018AB2 File Offset: 0x00016CB2
		// (set) Token: 0x060021D8 RID: 8664 RVA: 0x000A20D4 File Offset: 0x000A02D4
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

		// Token: 0x060021D9 RID: 8665 RVA: 0x00018ABF File Offset: 0x00016CBF
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.openForAllEventSystems = this.eventSystemLocator.eventSystemProvider.fallBackToMainEventSystem;
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x00018AE3 File Offset: 0x00016CE3
		private void Start()
		{
			this.opening = true;
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x00018AE3 File Offset: 0x00016CE3
		private void OnEnable()
		{
			this.opening = true;
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x00018AEC File Offset: 0x00016CEC
		private void OnDisable()
		{
			this.opening = false;
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x00018AF5 File Offset: 0x00016CF5
		[AssetCheck(typeof(CursorOpener))]
		private static void CheckCursorOpener(ProjectIssueChecker projectIssueChecker, UnityEngine.Object asset)
		{
			if (!((CursorOpener)asset).GetComponent<MPEventSystemLocator>())
			{
				projectIssueChecker.Log("Missing MPEventSystemLocator.", null);
			}
		}

		// Token: 0x0400241E RID: 9246
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400241F RID: 9247
		private MPEventSystem linkedEventSystem;

		// Token: 0x04002420 RID: 9248
		private bool openForAllEventSystems;
	}
}
