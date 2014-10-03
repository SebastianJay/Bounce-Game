using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A parser for evaluating conditional statements in text files
/// </summary>
public static class ConditionalParser
{
	public static bool EvaluateStatement(string statement)
	{
		while (statement.IndexOf('(') != -1)
		{
			int parenStart = statement.IndexOf('(');
			int nextParen = statement.IndexOf('(', parenStart+1);
			int parenEnd = statement.IndexOf(')', parenStart+1);
			while (nextParen != -1 && nextParen < parenEnd)
			{
				nextParen = statement.IndexOf('(', nextParen+1);
				parenEnd = statement.IndexOf(')', parenEnd+1);
			}
			if (parenEnd == -1)
			{
				UnityEngine.Debug.LogError("Unclosed parentheses in: " + statement);
				return false;
			}
			string substate = statement.Substring(parenStart + 1, (parenEnd-parenStart)-1);
			statement = statement.Replace("("+substate+")", Convert.ToString(EvaluateStatement(substate)));
		}

		string[] args = statement.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
		bool startSet = false;
		bool retVal = false;
		bool not_on = false;
		bool and_on = false;
		bool or_on = false;
		foreach (string arg in args)
		{
			if (arg == "and")
				and_on = true;
			else if (arg == "or")
				or_on = true;
			else if (arg == "not")
				not_on = true;
			else
			{
				bool truth = false;
				if (arg == "True" || arg == "true")
					truth = true;
				else if (arg == "False" || arg == "false")
					truth = false;
				else
					truth = DialogueConstantParser.EvaluateConstant(arg);

				if (not_on)
				{
					not_on = false;
					truth = !truth;
				}
				if (!startSet)
				{
					startSet = true;
					retVal = truth;
				}
				else if (and_on)
				{
					and_on = false;
					retVal = retVal && truth;
				}
				else if (or_on)
				{
					or_on = false;
					retVal = retVal || truth;
				}
			}
		}
		return retVal;
	}
}
