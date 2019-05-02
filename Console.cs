using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002AD RID: 685
	public class Console : MonoBehaviour
	{
		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000DED RID: 3565 RVA: 0x0000AC8C File Offset: 0x00008E8C
		// (set) Token: 0x06000DEE RID: 3566 RVA: 0x0000AC93 File Offset: 0x00008E93
		public static Console instance { get; private set; }

		// Token: 0x06000DEF RID: 3567 RVA: 0x0000AC9B File Offset: 0x00008E9B
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterLogHandler()
		{
			Application.logMessageReceived += Console.HandleLog;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x00056690 File Offset: 0x00054890
		private static void HandleLog(string message, string stackTrace, LogType logType)
		{
			if (logType == LogType.Error)
			{
				message = string.Format(CultureInfo.InvariantCulture, "<color=#FF0000>{0}</color>", message);
			}
			else if (logType == LogType.Warning)
			{
				message = string.Format(CultureInfo.InvariantCulture, "<color=#FFFF00>{0}</color>", message);
			}
			Console.Log log = new Console.Log
			{
				message = message,
				stackTrace = stackTrace,
				logType = logType
			};
			Console.logs.Add(log);
			if (Console.maxMessages.value > 0)
			{
				while (Console.logs.Count > Console.maxMessages.value)
				{
					Console.logs.RemoveAt(0);
				}
			}
			if (Console.onLogReceived != null)
			{
				Console.onLogReceived(log);
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000DF1 RID: 3569 RVA: 0x0005673C File Offset: 0x0005493C
		// (remove) Token: 0x06000DF2 RID: 3570 RVA: 0x00056770 File Offset: 0x00054970
		public static event Console.LogReceivedDelegate onLogReceived;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000DF3 RID: 3571 RVA: 0x000567A4 File Offset: 0x000549A4
		// (remove) Token: 0x06000DF4 RID: 3572 RVA: 0x000567D8 File Offset: 0x000549D8
		public static event Action onClear;

		// Token: 0x06000DF5 RID: 3573 RVA: 0x0005680C File Offset: 0x00054A0C
		private string GetVstrValue(NetworkUser user, string identifier)
		{
			string result;
			if (!(user == null))
			{
				result = "";
				return result;
			}
			if (this.vstrs.TryGetValue(identifier, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00056844 File Offset: 0x00054A44
		private void InitConVars()
		{
			this.allConVars = new Dictionary<string, BaseConVar>();
			this.archiveConVars = new List<BaseConVar>();
			foreach (Type type in typeof(BaseConVar).Assembly.GetTypes())
			{
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (fieldInfo.FieldType.IsSubclassOf(typeof(BaseConVar)))
					{
						if (fieldInfo.IsStatic)
						{
							BaseConVar conVar = (BaseConVar)fieldInfo.GetValue(null);
							this.RegisterConVarInternal(conVar);
						}
						else if (type.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
						{
							Debug.LogErrorFormat("ConVar defined as {0}.{1} could not be registered. ConVars must be static fields.", new object[]
							{
								type.Name,
								fieldInfo.Name
							});
						}
					}
				}
				foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo.GetCustomAttribute<ConVarProviderAttribute>() != null)
					{
						if (methodInfo.ReturnType != typeof(IEnumerable<BaseConVar>) || methodInfo.GetParameters().Length != 0)
						{
							Debug.LogErrorFormat("ConVar provider {0}.{1} does not match the signature \"static IEnumerable<ConVar.BaseConVar>()\".", new object[]
							{
								type.Name,
								methodInfo.Name
							});
						}
						else if (!methodInfo.IsStatic)
						{
							Debug.LogErrorFormat("ConVar provider {0}.{1} could not be invoked. Methods marked with the ConVarProvider attribute must be static.", new object[]
							{
								type.Name,
								methodInfo.Name
							});
						}
						else
						{
							foreach (BaseConVar conVar2 in ((IEnumerable<BaseConVar>)methodInfo.Invoke(null, Array.Empty<object>())))
							{
								this.RegisterConVarInternal(conVar2);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in this.allConVars)
			{
				BaseConVar value = keyValuePair.Value;
				if ((value.flags & ConVarFlags.Engine) != ConVarFlags.None)
				{
					value.defaultValue = value.GetString();
				}
				else if (value.defaultValue != null)
				{
					value.SetString(value.defaultValue);
				}
			}
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x0000ACAE File Offset: 0x00008EAE
		private void RegisterConVarInternal(BaseConVar conVar)
		{
			if (conVar == null)
			{
				Debug.LogWarning("Attempted to register null ConVar");
				return;
			}
			this.allConVars[conVar.name] = conVar;
			if ((conVar.flags & ConVarFlags.Archive) != ConVarFlags.None)
			{
				this.archiveConVars.Add(conVar);
			}
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x00056A94 File Offset: 0x00054C94
		public BaseConVar FindConVar(string name)
		{
			BaseConVar result;
			if (this.allConVars.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00056AB4 File Offset: 0x00054CB4
		public void SubmitCmd(NetworkUser sender, string cmd, bool recordSubmit = false)
		{
			if (recordSubmit)
			{
				Console.Log log = new Console.Log
				{
					message = string.Format(CultureInfo.InvariantCulture, "<color=#C0C0C0>] {0}</color>", cmd),
					stackTrace = "",
					logType = LogType.Log
				};
				Console.logs.Add(log);
				if (Console.onLogReceived != null)
				{
					Console.onLogReceived(log);
				}
				Console.userCmdHistory.Add(cmd);
			}
			Queue<string> tokens = new Console.Lexer(cmd).GetTokens();
			List<string> list = new List<string>();
			bool flag = false;
			while (tokens.Count != 0)
			{
				string text = tokens.Dequeue();
				if (text == ";")
				{
					flag = false;
					if (list.Count > 0)
					{
						string concommandName = list[0].ToLower();
						list.RemoveAt(0);
						this.RunCmd(sender, concommandName, list);
						list.Clear();
					}
				}
				else
				{
					if (flag)
					{
						text = this.GetVstrValue(sender, text);
						flag = false;
					}
					if (text == "vstr")
					{
						flag = true;
					}
					else
					{
						list.Add(text);
					}
				}
			}
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00056BB4 File Offset: 0x00054DB4
		private void ForwardCmdToServer(ConCommandArgs args)
		{
			if (!args.sender)
			{
				return;
			}
			Console.sendCmdBuilder.Append(args.commandName);
			Console.sendCmdBuilder.Append(" ");
			foreach (string value in args.userArgs)
			{
				Console.sendCmdBuilder.Append("\"");
				Console.sendCmdBuilder.Append(value);
				Console.sendCmdBuilder.Append("\"");
			}
			string cmd = Console.sendCmdBuilder.ToString();
			Console.sendCmdBuilder.Length = 0;
			args.sender.CallCmdSendConsoleCommand(cmd);
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00056C80 File Offset: 0x00054E80
		private void RunCmd(NetworkUser sender, string concommandName, List<string> userArgs)
		{
			bool active = NetworkServer.active;
			Console.ConCommand conCommand;
			if (this.concommandCatalog.TryGetValue(concommandName, out conCommand))
			{
				if (!active && (conCommand.flags & ConVarFlags.ExecuteOnServer) > ConVarFlags.None)
				{
					this.ForwardCmdToServer(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
					return;
				}
				if (NetworkServer.active && sender && !sender.isLocalPlayer && (conCommand.flags & ConVarFlags.SenderMustBeServer) != ConVarFlags.None)
				{
					return;
				}
				if ((conCommand.flags & ConVarFlags.Cheat) != ConVarFlags.None && !RoR2Application.cvCheats.boolValue)
				{
					Debug.LogFormat("Command \"{0}\" cannot be used while cheats are disabled.", new object[]
					{
						concommandName
					});
					return;
				}
				try
				{
					conCommand.action(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
				}
				catch (ConCommandException ex)
				{
					Debug.LogFormat("Command \"{0}\" failed: {1}", new object[]
					{
						concommandName,
						ex.Message
					});
				}
				return;
			}
			else
			{
				BaseConVar baseConVar = this.FindConVar(concommandName);
				if (baseConVar == null)
				{
					Debug.LogFormat("\"{0}\" is not a recognized ConCommand or ConVar.", new object[]
					{
						concommandName
					});
					return;
				}
				if (!active && (baseConVar.flags & ConVarFlags.ExecuteOnServer) > ConVarFlags.None)
				{
					this.ForwardCmdToServer(new ConCommandArgs
					{
						sender = sender,
						commandName = concommandName,
						userArgs = userArgs
					});
					return;
				}
				if (NetworkServer.active && sender && !sender.isLocalPlayer && (baseConVar.flags & ConVarFlags.SenderMustBeServer) != ConVarFlags.None)
				{
					return;
				}
				if (userArgs.Count <= 0)
				{
					Debug.LogFormat("\"{0}\" = \"{1}\"\n{2}", new object[]
					{
						concommandName,
						baseConVar.GetString(),
						baseConVar.helpText
					});
					return;
				}
				if ((baseConVar.flags & ConVarFlags.Cheat) != ConVarFlags.None && !RoR2Application.cvCheats.boolValue)
				{
					Debug.LogFormat("Command \"{0}\" cannot be changed while cheats are disabled.", new object[]
					{
						concommandName
					});
					return;
				}
				baseConVar.SetString(userArgs[0]);
				return;
			}
		}

		// Token: 0x06000DFC RID: 3580
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		// Token: 0x06000DFD RID: 3581
		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		// Token: 0x06000DFE RID: 3582
		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int processId);

		// Token: 0x06000DFF RID: 3583
		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		// Token: 0x06000E00 RID: 3584
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x06000E01 RID: 3585 RVA: 0x0000ACE6 File Offset: 0x00008EE6
		private static string ReadInputStream()
		{
			if (Console.stdInQueue.Count > 0)
			{
				return Console.stdInQueue.Dequeue();
			}
			return null;
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00056E80 File Offset: 0x00055080
		private static void ThreadedInputQueue()
		{
			string item;
			while (Console.systemConsoleType != Console.SystemConsoleType.None && (item = Console.ReadLine()) != null)
			{
				Console.stdInQueue.Enqueue(item);
			}
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x00056EAC File Offset: 0x000550AC
		private static void SetupSystemConsole()
		{
			bool flag = false;
			bool flag2 = false;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i] == "-console")
				{
					flag = true;
				}
				if (commandLineArgs[i] == "-console_detach")
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				Console.systemConsoleType = Console.SystemConsoleType.Attach;
				if (flag2)
				{
					Console.systemConsoleType = Console.SystemConsoleType.Alloc;
				}
			}
			switch (Console.systemConsoleType)
			{
			case Console.SystemConsoleType.Attach:
				Console.AttachConsole(-1);
				break;
			case Console.SystemConsoleType.Alloc:
				Console.AllocConsole();
				break;
			}
			if (Console.systemConsoleType != Console.SystemConsoleType.None)
			{
				Console.SetIn(new StreamReader(Console.OpenStandardInput()));
				Console.stdInReaderThread = new Thread(new ThreadStart(Console.ThreadedInputQueue));
				Console.stdInReaderThread.Start();
			}
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x00056F68 File Offset: 0x00055168
		private void Awake()
		{
			Console.instance = this;
			Console.SetupSystemConsole();
			this.InitConVars();
			Type[] types = base.GetType().Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(false);
					for (int k = 0; k < customAttributes.Length; k++)
					{
						ConCommandAttribute conCommandAttribute = ((Attribute)customAttributes[k]) as ConCommandAttribute;
						if (conCommandAttribute != null)
						{
							this.concommandCatalog[conCommandAttribute.commandName.ToLower()] = new Console.ConCommand
							{
								flags = conCommandAttribute.flags,
								action = (Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(Console.ConCommandDelegate), methodInfo),
								helpText = conCommandAttribute.helpText
							};
						}
					}
				}
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int l = 0; l < commandLineArgs.Length; l++)
			{
				Debug.LogFormat("arg[{0}]=\"{1}\"", new object[]
				{
					l,
					commandLineArgs[l]
				});
			}
			MPEventSystemManager.availability.CallWhenAvailable(new Action(this.LoadStartupConfigs));
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x0000AD01 File Offset: 0x00008F01
		private void LoadStartupConfigs()
		{
			this.SubmitCmd(null, "exec config", false);
			this.SubmitCmd(null, "exec autoexec", false);
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x000570A8 File Offset: 0x000552A8
		private void Update()
		{
			string cmd;
			while ((cmd = Console.ReadInputStream()) != null)
			{
				this.SubmitCmd(null, cmd, true);
			}
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x000570CC File Offset: 0x000552CC
		private void OnDestroy()
		{
			if (Console.stdInReaderThread != null)
			{
				Console.stdInReaderThread = null;
			}
			if (Console.systemConsoleType != Console.SystemConsoleType.None)
			{
				Console.systemConsoleType = Console.SystemConsoleType.None;
				IntPtr consoleWindow = Console.GetConsoleWindow();
				if (consoleWindow != IntPtr.Zero)
				{
					Console.PostMessage(consoleWindow, 256u, 13, 0);
				}
				if (Console.stdInReaderThread != null)
				{
					Console.stdInReaderThread.Join();
					Console.stdInReaderThread = null;
				}
				Console.SetIn(null);
				Console.SetOut(null);
				Console.SetError(null);
				Console.FreeConsole();
			}
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x0000AD1D File Offset: 0x00008F1D
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void SetConfigFolder()
		{
			Console.configFolder = Application.dataPath + "/Config/";
		}

		// Token: 0x06000E09 RID: 3593 RVA: 0x00057144 File Offset: 0x00055344
		private static string LoadConfig(string fileName)
		{
			string path = string.Format(CultureInfo.InvariantCulture, "{0}{1}.cfg", Console.configFolder, fileName);
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			return null;
		}

		// Token: 0x06000E0A RID: 3594 RVA: 0x00057178 File Offset: 0x00055378
		public void SaveArchiveConVars()
		{
			string path = Console.configFolder + "config.cfg";
			string[] array = new string[this.archiveConVars.Count + 1];
			for (int i = 0; i < this.archiveConVars.Count; i++)
			{
				BaseConVar baseConVar = this.archiveConVars[i];
				array[i] = string.Format(CultureInfo.InvariantCulture, "{0} {1};", baseConVar.name, baseConVar.GetString());
			}
			array[array.Length - 1] = "echo \"Loaded archived convars.\";";
			File.WriteAllText(path, string.Join("\n", array));
		}

		// Token: 0x06000E0B RID: 3595 RVA: 0x0000AD33 File Offset: 0x00008F33
		[ConCommand(commandName = "set_vstr", flags = ConVarFlags.None, helpText = "Sets the specified vstr to the specified value.")]
		private static void CCSetVstr(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			Console.instance.vstrs.Add(args[0], args[1]);
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x00057208 File Offset: 0x00055408
		[ConCommand(commandName = "exec", flags = ConVarFlags.None, helpText = "Executes a named config from the \"Config/\" folder.")]
		private static void CCExec(ConCommandArgs args)
		{
			if (args.Count > 0)
			{
				string text = Console.LoadConfig(args[0]);
				if (text == null)
				{
					Debug.LogFormat("Failed to find config \"{0}{1}.cfg\"", new object[]
					{
						Console.configFolder,
						args[0]
					});
					return;
				}
				Console.instance.SubmitCmd(args.sender, text, false);
			}
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x0000AD5C File Offset: 0x00008F5C
		[ConCommand(commandName = "echo", flags = ConVarFlags.None, helpText = "Echoes the given text to the console.")]
		private static void CCEcho(ConCommandArgs args)
		{
			if (args.Count > 0)
			{
				Debug.Log(args[0]);
				return;
			}
			Console.ShowHelpText(args.commandName);
		}

		// Token: 0x06000E0E RID: 3598 RVA: 0x00057268 File Offset: 0x00055468
		[ConCommand(commandName = "cvarlist", flags = ConVarFlags.None, helpText = "Print all available convars and concommands.")]
		private static void CCCvarList(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in Console.instance.allConVars)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in Console.instance.concommandCatalog)
			{
				list.Add(keyValuePair2.Key);
			}
			list.Sort();
			Debug.Log(string.Join("\n", list.ToArray()));
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x0000AD81 File Offset: 0x00008F81
		[ConCommand(commandName = "help", flags = ConVarFlags.None, helpText = "Show help text for the named convar or concommand.")]
		private static void CCHelp(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				Console.instance.SubmitCmd(args.sender, "find \"*\"", false);
				return;
			}
			Console.ShowHelpText(args[0]);
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x00057334 File Offset: 0x00055534
		[ConCommand(commandName = "find", flags = ConVarFlags.None, helpText = "Find all concommands and convars with the specified substring.")]
		private static void CCFind(ConCommandArgs args)
		{
			if (args.Count == 0)
			{
				Console.ShowHelpText("find");
				return;
			}
			string text = args[0].ToLower();
			bool flag = text == "*";
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BaseConVar> keyValuePair in Console.instance.allConVars)
			{
				if (flag || keyValuePair.Key.ToLower().Contains(text) || keyValuePair.Value.helpText.ToLower().Contains(text))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in Console.instance.concommandCatalog)
			{
				if (flag || keyValuePair2.Key.ToLower().Contains(text) || keyValuePair2.Value.helpText.ToLower().Contains(text))
				{
					list.Add(keyValuePair2.Key);
				}
			}
			list.Sort();
			string[] array = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = Console.GetHelpText(list[i]);
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x0000ADB0 File Offset: 0x00008FB0
		[ConCommand(commandName = "clear", flags = ConVarFlags.None, helpText = "Clears the console output.")]
		private static void CCClear(ConCommandArgs args)
		{
			Console.logs.Clear();
			Action action = Console.onClear;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x000574C0 File Offset: 0x000556C0
		private static string GetHelpText(string commandName)
		{
			Console.ConCommand conCommand;
			if (Console.instance.concommandCatalog.TryGetValue(commandName, out conCommand))
			{
				return string.Format(CultureInfo.InvariantCulture, "<color=#FF7F7F>\"{0}\"</color>\n- {1}", commandName, conCommand.helpText);
			}
			BaseConVar baseConVar = Console.instance.FindConVar(commandName);
			if (baseConVar != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "<color=#FF7F7F>\"{0}\" = \"{1}\"</color>\n - {2}", commandName, baseConVar.GetString(), baseConVar.helpText);
			}
			return "";
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x0000ADCB File Offset: 0x00008FCB
		public static void ShowHelpText(string commandName)
		{
			Debug.Log(Console.GetHelpText(commandName));
		}

		// Token: 0x040011DE RID: 4574
		public static List<Console.Log> logs = new List<Console.Log>();

		// Token: 0x040011E1 RID: 4577
		private Dictionary<string, string> vstrs = new Dictionary<string, string>();

		// Token: 0x040011E2 RID: 4578
		private Dictionary<string, Console.ConCommand> concommandCatalog = new Dictionary<string, Console.ConCommand>();

		// Token: 0x040011E3 RID: 4579
		private Dictionary<string, BaseConVar> allConVars;

		// Token: 0x040011E4 RID: 4580
		private List<BaseConVar> archiveConVars;

		// Token: 0x040011E5 RID: 4581
		public static List<string> userCmdHistory = new List<string>();

		// Token: 0x040011E6 RID: 4582
		private static StringBuilder sendCmdBuilder = new StringBuilder();

		// Token: 0x040011E7 RID: 4583
		private const int VK_RETURN = 13;

		// Token: 0x040011E8 RID: 4584
		private const int WM_KEYDOWN = 256;

		// Token: 0x040011E9 RID: 4585
		private static byte[] inputStreamBuffer = new byte[256];

		// Token: 0x040011EA RID: 4586
		private static Queue<string> stdInQueue = new Queue<string>();

		// Token: 0x040011EB RID: 4587
		private static Thread stdInReaderThread = null;

		// Token: 0x040011EC RID: 4588
		private static Console.SystemConsoleType systemConsoleType = Console.SystemConsoleType.None;

		// Token: 0x040011ED RID: 4589
		private static string configFolder = null;

		// Token: 0x040011EE RID: 4590
		private static IntConVar maxMessages = new IntConVar("max_messages", ConVarFlags.Archive, "25", "Maximum number of messages that can be held in the console log.");

		// Token: 0x020002AE RID: 686
		public struct Log
		{
			// Token: 0x040011EF RID: 4591
			public string message;

			// Token: 0x040011F0 RID: 4592
			public string stackTrace;

			// Token: 0x040011F1 RID: 4593
			public LogType logType;
		}

		// Token: 0x020002AF RID: 687
		// (Invoke) Token: 0x06000E17 RID: 3607
		public delegate void LogReceivedDelegate(Console.Log log);

		// Token: 0x020002B0 RID: 688
		private class Lexer
		{
			// Token: 0x06000E1A RID: 3610 RVA: 0x0000ADF6 File Offset: 0x00008FF6
			public Lexer(string srcString)
			{
				this.srcString = srcString;
				this.readIndex = 0;
			}

			// Token: 0x06000E1B RID: 3611 RVA: 0x0000AE17 File Offset: 0x00009017
			private static bool IsIgnorableCharacter(char character)
			{
				return !Console.Lexer.IsSeparatorCharacter(character) && !Console.Lexer.IsQuoteCharacter(character) && !Console.Lexer.IsIdentifierCharacter(character) && character != '/';
			}

			// Token: 0x06000E1C RID: 3612 RVA: 0x0000AE3B File Offset: 0x0000903B
			private static bool IsSeparatorCharacter(char character)
			{
				return character == ';' || character == '\n';
			}

			// Token: 0x06000E1D RID: 3613 RVA: 0x0000AE49 File Offset: 0x00009049
			private static bool IsQuoteCharacter(char character)
			{
				return character == '\'' || character == '"';
			}

			// Token: 0x06000E1E RID: 3614 RVA: 0x0000AE57 File Offset: 0x00009057
			private static bool IsIdentifierCharacter(char character)
			{
				return char.IsLetterOrDigit(character) || character == '_' || character == '.' || character == '-';
			}

			// Token: 0x06000E1F RID: 3615 RVA: 0x0005759C File Offset: 0x0005579C
			private bool TrimComment()
			{
				if (this.readIndex >= this.srcString.Length)
				{
					return false;
				}
				if (this.srcString[this.readIndex] == '/' && this.readIndex + 1 < this.srcString.Length)
				{
					char c = this.srcString[this.readIndex + 1];
					if (c == '/')
					{
						while (this.readIndex < this.srcString.Length)
						{
							if (this.srcString[this.readIndex] == '\n')
							{
								this.readIndex++;
								return true;
							}
							this.readIndex++;
						}
						return true;
					}
					if (c == '*')
					{
						while (this.readIndex < this.srcString.Length - 1)
						{
							if (this.srcString[this.readIndex] == '*' && this.srcString[this.readIndex + 1] == '/')
							{
								this.readIndex += 2;
								return true;
							}
							this.readIndex++;
						}
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000E20 RID: 3616 RVA: 0x0000AE72 File Offset: 0x00009072
			private void TrimWhitespace()
			{
				while (this.readIndex < this.srcString.Length && Console.Lexer.IsIgnorableCharacter(this.srcString[this.readIndex]))
				{
					this.readIndex++;
				}
			}

			// Token: 0x06000E21 RID: 3617 RVA: 0x0000AEAF File Offset: 0x000090AF
			private void TrimUnused()
			{
				do
				{
					this.TrimWhitespace();
				}
				while (this.TrimComment());
			}

			// Token: 0x06000E22 RID: 3618 RVA: 0x000576BC File Offset: 0x000558BC
			private static int UnescapeNext(string srcString, int startPos, out char result)
			{
				result = '\\';
				int num = startPos + 1;
				if (num < srcString.Length)
				{
					char c = srcString[num];
					if (c <= '\'')
					{
						if (c != '"' && c != '\'')
						{
							return 1;
						}
					}
					else if (c != '\\')
					{
						if (c != 'n')
						{
							return 1;
						}
						result = '\n';
						return 2;
					}
					result = c;
					return 2;
				}
				return 1;
			}

			// Token: 0x06000E23 RID: 3619 RVA: 0x00057710 File Offset: 0x00055910
			public string NextToken()
			{
				this.TrimUnused();
				if (this.readIndex == this.srcString.Length)
				{
					return null;
				}
				Console.Lexer.TokenType tokenType = Console.Lexer.TokenType.Identifier;
				char c = this.srcString[this.readIndex];
				char c2 = '\0';
				if (Console.Lexer.IsQuoteCharacter(c))
				{
					tokenType = Console.Lexer.TokenType.NestedString;
					c2 = c;
					this.readIndex++;
				}
				else if (Console.Lexer.IsSeparatorCharacter(c))
				{
					this.readIndex++;
					return ";";
				}
				while (this.readIndex < this.srcString.Length)
				{
					char c3 = this.srcString[this.readIndex];
					if (tokenType == Console.Lexer.TokenType.Identifier)
					{
						if (!Console.Lexer.IsIdentifierCharacter(c3))
						{
							break;
						}
					}
					else if (tokenType == Console.Lexer.TokenType.NestedString)
					{
						if (c3 == '\\')
						{
							this.readIndex += Console.Lexer.UnescapeNext(this.srcString, this.readIndex, out c3) - 1;
						}
						else if (c3 == c2)
						{
							this.readIndex++;
							break;
						}
					}
					this.stringBuilder.Append(c3);
					this.readIndex++;
				}
				string result = this.stringBuilder.ToString();
				this.stringBuilder.Length = 0;
				return result;
			}

			// Token: 0x06000E24 RID: 3620 RVA: 0x00057834 File Offset: 0x00055A34
			public Queue<string> GetTokens()
			{
				Queue<string> queue = new Queue<string>();
				for (string item = this.NextToken(); item != null; item = this.NextToken())
				{
					queue.Enqueue(item);
				}
				queue.Enqueue(";");
				return queue;
			}

			// Token: 0x040011F2 RID: 4594
			private string srcString;

			// Token: 0x040011F3 RID: 4595
			private int readIndex;

			// Token: 0x040011F4 RID: 4596
			private StringBuilder stringBuilder = new StringBuilder();

			// Token: 0x020002B1 RID: 689
			private enum TokenType
			{
				// Token: 0x040011F6 RID: 4598
				Identifier,
				// Token: 0x040011F7 RID: 4599
				NestedString
			}
		}

		// Token: 0x020002B2 RID: 690
		private class Substring
		{
			// Token: 0x17000130 RID: 304
			// (get) Token: 0x06000E25 RID: 3621 RVA: 0x0000AEBF File Offset: 0x000090BF
			public int endIndex
			{
				get
				{
					return this.startIndex + this.length;
				}
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x06000E26 RID: 3622 RVA: 0x0000AECE File Offset: 0x000090CE
			public string str
			{
				get
				{
					return this.srcString.Substring(this.startIndex, this.length);
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x06000E27 RID: 3623 RVA: 0x0000AEE7 File Offset: 0x000090E7
			public Console.Substring nextToken
			{
				get
				{
					return new Console.Substring
					{
						srcString = this.srcString,
						startIndex = this.startIndex + this.length,
						length = 0
					};
				}
			}

			// Token: 0x040011F8 RID: 4600
			public string srcString;

			// Token: 0x040011F9 RID: 4601
			public int startIndex;

			// Token: 0x040011FA RID: 4602
			public int length;
		}

		// Token: 0x020002B3 RID: 691
		private class ConCommand
		{
			// Token: 0x040011FB RID: 4603
			public ConVarFlags flags;

			// Token: 0x040011FC RID: 4604
			public Console.ConCommandDelegate action;

			// Token: 0x040011FD RID: 4605
			public string helpText;
		}

		// Token: 0x020002B4 RID: 692
		// (Invoke) Token: 0x06000E2B RID: 3627
		public delegate void ConCommandDelegate(ConCommandArgs args);

		// Token: 0x020002B5 RID: 693
		private enum SystemConsoleType
		{
			// Token: 0x040011FF RID: 4607
			None,
			// Token: 0x04001200 RID: 4608
			Attach,
			// Token: 0x04001201 RID: 4609
			Alloc
		}

		// Token: 0x020002B6 RID: 694
		public class AutoComplete
		{
			// Token: 0x06000E2E RID: 3630 RVA: 0x00057870 File Offset: 0x00055A70
			public AutoComplete(Console console)
			{
				HashSet<string> hashSet = new HashSet<string>();
				for (int i = 0; i < Console.userCmdHistory.Count; i++)
				{
					hashSet.Add(Console.userCmdHistory[i]);
				}
				foreach (KeyValuePair<string, BaseConVar> keyValuePair in console.allConVars)
				{
					hashSet.Add(keyValuePair.Key);
				}
				foreach (KeyValuePair<string, Console.ConCommand> keyValuePair2 in console.concommandCatalog)
				{
					hashSet.Add(keyValuePair2.Key);
				}
				foreach (string item in hashSet)
				{
					this.searchableStrings.Add(item);
				}
				this.searchableStrings.Sort();
			}

			// Token: 0x06000E2F RID: 3631 RVA: 0x000579B0 File Offset: 0x00055BB0
			public bool SetSearchString(string newSearchString)
			{
				newSearchString = newSearchString.ToLower();
				if (newSearchString == this.searchString)
				{
					return false;
				}
				this.searchString = newSearchString;
				List<Console.AutoComplete.MatchInfo> list = new List<Console.AutoComplete.MatchInfo>();
				for (int i = 0; i < this.searchableStrings.Count; i++)
				{
					string text = this.searchableStrings[i];
					int num = Math.Min(text.Length, this.searchString.Length);
					int num2 = 0;
					while (num2 < num && char.ToLower(text[num2]) == this.searchString[num2])
					{
						num2++;
					}
					if (num2 > 1)
					{
						list.Add(new Console.AutoComplete.MatchInfo
						{
							str = text,
							similarity = num2
						});
					}
				}
				list.Sort(delegate(Console.AutoComplete.MatchInfo a, Console.AutoComplete.MatchInfo b)
				{
					if (a.similarity == b.similarity)
					{
						return string.CompareOrdinal(a.str, b.str);
					}
					if (a.similarity <= b.similarity)
					{
						return 1;
					}
					return -1;
				});
				this.resultsList = new List<string>();
				for (int j = 0; j < list.Count; j++)
				{
					this.resultsList.Add(list[j].str);
				}
				return true;
			}

			// Token: 0x04001202 RID: 4610
			private List<string> searchableStrings = new List<string>();

			// Token: 0x04001203 RID: 4611
			private string searchString;

			// Token: 0x04001204 RID: 4612
			public List<string> resultsList = new List<string>();

			// Token: 0x020002B7 RID: 695
			private struct MatchInfo
			{
				// Token: 0x04001205 RID: 4613
				public string str;

				// Token: 0x04001206 RID: 4614
				public int similarity;
			}
		}
	}
}
