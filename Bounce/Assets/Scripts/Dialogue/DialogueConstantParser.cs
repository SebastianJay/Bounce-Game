using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DialogueConstantParser
{
	public static HashSet<string> constantSet = new HashSet<string>();

	public static void ExecuteEvent(string eventName)
	{
		/*
		switch(eventName)
		{
			//TODO: hard-code events
		}
		*/
	}

	/// <summary>
	/// Checks if the string is in our list of defined constants.
	/// If not, performs hard-coded checks to see if the constant is valid.
	/// </summary>
	/// <param name="constant">Some string value that stands for a unique constant</param>
	public static bool EvaluateConstant(string constant)
	{
		// first check our set
		if (constantSet.Contains(constant))
			return true;

		// for string literals in the dialogue
		// which cannot be found in the hashset
		/*
		switch(constant)
		{
			//TODO: hard-code checks
		}
		*/
		return false;
	}

	public static void SetConstant(string constant)
	{
		constantSet.Add(constant);
	}
}
