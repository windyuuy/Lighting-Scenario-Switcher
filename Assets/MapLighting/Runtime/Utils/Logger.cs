using System;
using UnityEngine;

namespace MapLighting
{
	public class Logger
	{
		public static bool IsVerbose = false;

		public static void LogError(string msg)
		{
			if (IsVerbose)
			{
				Debug.LogError(msg);
			}
		}

		public static void Log(string msg)
		{
			if (IsVerbose)
			{
				Debug.Log(msg);
			}
		}

		public static void LogWarn(string msg)
		{
			if (IsVerbose)
			{
				Debug.LogWarning(msg);
			}
		}

		public static void LogError(Func<string> msg)
		{
			if (IsVerbose)
			{
				Debug.LogError(msg());
			}
		}

		public static void Log(Func<string> msg)
		{
			if (IsVerbose)
			{
				Debug.Log(msg());
			}
		}

		public static void LogWarn(Func<string> msg)
		{
			if (IsVerbose)
			{
				Debug.LogWarning(msg());
			}
		}
	}
}