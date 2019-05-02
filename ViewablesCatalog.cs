using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004F4 RID: 1268
	public static class ViewablesCatalog
	{
		// Token: 0x06001CE5 RID: 7397 RVA: 0x0008DA44 File Offset: 0x0008BC44
		public static void AddNodeToRoot(ViewablesCatalog.Node node)
		{
			node.SetParent(ViewablesCatalog.rootNode);
			foreach (ViewablesCatalog.Node node2 in node.Descendants())
			{
				if (ViewablesCatalog.fullNameToNodeMap.ContainsKey(node2.fullName))
				{
					Debug.LogFormat("Tried to add duplicate node {0}", new object[]
					{
						node2.fullName
					});
				}
				else
				{
					ViewablesCatalog.fullNameToNodeMap.Add(node2.fullName, node2);
				}
			}
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x0008DAD4 File Offset: 0x0008BCD4
		public static ViewablesCatalog.Node FindNode(string fullName)
		{
			ViewablesCatalog.Node result;
			if (ViewablesCatalog.fullNameToNodeMap.TryGetValue(fullName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x0008DAF4 File Offset: 0x0008BCF4
		[ConCommand(commandName = "viewables_list", flags = ConVarFlags.None, helpText = "Displays the full names of all viewables.")]
		private static void CCViewablesList(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from node in ViewablesCatalog.rootNode.Descendants()
			select node.fullName).ToArray<string>()));
		}

		// Token: 0x04001EB7 RID: 7863
		private static readonly ViewablesCatalog.Node rootNode = new ViewablesCatalog.Node("", true, null);

		// Token: 0x04001EB8 RID: 7864
		private static readonly Dictionary<string, ViewablesCatalog.Node> fullNameToNodeMap = new Dictionary<string, ViewablesCatalog.Node>();

		// Token: 0x020004F5 RID: 1269
		public class Node
		{
			// Token: 0x17000298 RID: 664
			// (get) Token: 0x06001CE9 RID: 7401 RVA: 0x000153C4 File Offset: 0x000135C4
			// (set) Token: 0x06001CEA RID: 7402 RVA: 0x000153CC File Offset: 0x000135CC
			public ViewablesCatalog.Node parent { get; private set; }

			// Token: 0x17000299 RID: 665
			// (get) Token: 0x06001CEB RID: 7403 RVA: 0x000153D5 File Offset: 0x000135D5
			public string fullName
			{
				get
				{
					if (this.fullNameDirty)
					{
						this.GenerateFullName();
					}
					return this._fullName;
				}
			}

			// Token: 0x06001CEC RID: 7404 RVA: 0x0008DB44 File Offset: 0x0008BD44
			public Node(string name, bool isFolder, ViewablesCatalog.Node parent = null)
			{
				this.name = name;
				this.isFolder = isFolder;
				this.shouldShowUnviewed = new Func<UserProfile, bool>(this.DefaultShouldShowUnviewedTest);
				this.children = this._children.AsReadOnly();
				this.SetParent(parent);
			}

			// Token: 0x06001CED RID: 7405 RVA: 0x0008DBA4 File Offset: 0x0008BDA4
			public void SetParent(ViewablesCatalog.Node newParent)
			{
				if (this.parent == newParent)
				{
					return;
				}
				ViewablesCatalog.Node parent = this.parent;
				if (parent != null)
				{
					parent._children.Remove(this);
				}
				this.parent = newParent;
				ViewablesCatalog.Node parent2 = this.parent;
				if (parent2 != null)
				{
					parent2._children.Add(this);
				}
				this.fullNameDirty = true;
			}

			// Token: 0x06001CEE RID: 7406 RVA: 0x0008DBF8 File Offset: 0x0008BDF8
			private void GenerateFullName()
			{
				string text = this.name;
				if (this.parent != null)
				{
					text = this.parent.fullName + text;
				}
				if (this.isFolder)
				{
					text += "/";
				}
				this._fullName = text;
				this.fullNameDirty = false;
			}

			// Token: 0x06001CEF RID: 7407 RVA: 0x0008DC48 File Offset: 0x0008BE48
			public bool DefaultShouldShowUnviewedTest(UserProfile userProfile)
			{
				if (!this.isFolder && userProfile.HasViewedViewable(this.fullName))
				{
					return false;
				}
				using (IEnumerator<ViewablesCatalog.Node> enumerator = this.children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.shouldShowUnviewed(userProfile))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x06001CF0 RID: 7408 RVA: 0x000153EB File Offset: 0x000135EB
			public IEnumerable<ViewablesCatalog.Node> Descendants()
			{
				yield return this;
				foreach (ViewablesCatalog.Node node in this._children)
				{
					foreach (ViewablesCatalog.Node node2 in node.Descendants())
					{
						yield return node2;
					}
					IEnumerator<ViewablesCatalog.Node> enumerator2 = null;
				}
				List<ViewablesCatalog.Node>.Enumerator enumerator = default(List<ViewablesCatalog.Node>.Enumerator);
				yield break;
				yield break;
			}

			// Token: 0x04001EB9 RID: 7865
			public readonly string name;

			// Token: 0x04001EBA RID: 7866
			public readonly bool isFolder;

			// Token: 0x04001EBC RID: 7868
			private readonly List<ViewablesCatalog.Node> _children = new List<ViewablesCatalog.Node>();

			// Token: 0x04001EBD RID: 7869
			public ReadOnlyCollection<ViewablesCatalog.Node> children;

			// Token: 0x04001EBE RID: 7870
			private string _fullName;

			// Token: 0x04001EBF RID: 7871
			private bool fullNameDirty = true;

			// Token: 0x04001EC0 RID: 7872
			public Func<UserProfile, bool> shouldShowUnviewed;
		}
	}
}
