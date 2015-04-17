using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Enumerates all the events possible from a line of dialogue
/// </summary>
[Flags]
public enum InteractionEvents
{
	None        = 0x0,  //should never be used
	NextLine	= 0x1,	//specifies that dialogue continues
	SetConst	= 0x2,	//specifies to set a game constant
	Special		= 0x4,  //specifies to trigger an event in-game
	End			= 0x8,	//specifies that dialogue will end
}

/// <summary>
/// The basic interaction node, used for hero responses. 
/// Contains event type and arguments for each event.
/// </summary>
public class InteractionTreeNode
{
	public InteractionEvents eventType;	//combination of flags
	public string line;			//the actual text in dialogue
	public string gameConstant;	//for use when eventType is SetConst
	public string gameEvent;	//for use when eventType is Special
	public int nextLine;        //for use when eventType is NextLine

}

/// <summary>
/// An extension of the interaction node with a list of responses. 
/// Used for NPC lines (which are linked to hero responses).
/// </summary>
public class InteractionLinkedTreeNode : InteractionTreeNode
{
	public List<InteractionTreeNode> responses
		= new List<InteractionTreeNode>();
}

/// <summary>
/// A struct for if-statements in the interaction file, as well as choose statements
/// </summary>
public class InteractionControlPath
{
	public string statement = "";	//for if-statements
	public int lineNumber = -1;		//for if-statements
	public int randomStart = -1;	//for choose statements
	public int randomEnd = -1;		//for choose statements
}

/// <summary>
/// Wrapper class for Exception designed for when a line in a dialogue file isn't valid.
/// </summary>
sealed class LineException : Exception, ISerializable
{
	public LineException() : base() { }
	public LineException(string message) : base(message) { }
	public LineException(string message, Exception inner) : base(message, inner) { }
	public LineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

/// <summary>
/// Main interaction class. Responsible for loading dialogue from an interaction file, presenting list of lines.
/// </summary>
public class Interaction
{
	private string name;
	private List<InteractionLinkedTreeNode> dialogueList;
	private List<InteractionControlPath> controlList;
	private InteractionControlPath chooseStatement = null;
	private System.Random rand = new System.Random();

	private bool steppingThrough = false;
	private int currentLine = 0;

	public Interaction(TextAsset file)
	{
		string[] lines = Regex.Split (file.text, @"\r\n|\r|\n");
	    dialogueList = new List<InteractionLinkedTreeNode>();
	    controlList = new List<InteractionControlPath>();
		name = file.name.Split ('.') [0];
	    int lineCounter = 1;

	    foreach (string line in lines)
	    {
	        // ignore empty lines and comments
	        if (line.Length == 0)
	            continue;
	        if (line.StartsWith("#"))
				continue;

	        // conditional statements go here
	        // If the statement is true, then the line of dialogue directly below
	        //  will be the first line of the conversation
	        if (line.StartsWith("if "))
	        {
	            string conditional = line.Substring("if ".Length);
				InteractionControlPath ic = new InteractionControlPath();
				if (conditional.IndexOf("choose ") != -1) {
					string chooseStr = conditional.Substring(conditional.IndexOf("choose "));
					string[] chooseParams = chooseStr.Substring("choose ".Length).Split(',');
					ic.randomStart = Int32.Parse(chooseParams[0]);
					ic.randomEnd = Int32.Parse(chooseParams[1]);
					conditional = conditional.Substring(0, conditional.Length - chooseStr.Length);
				}
				ic.statement = conditional;
				ic.lineNumber = lineCounter;
	            controlList.Add(ic);
	        }
			else if (line.StartsWith ("choose "))
			{
				string[] chooseParams = line.Substring("choose ".Length).Split(',');
				InteractionControlPath ic = new InteractionControlPath();
				ic.randomStart = Int32.Parse(chooseParams[0]);
				ic.randomEnd = Int32.Parse(chooseParams[1]);
				chooseStatement = ic;
			}
	        else // otherwise we assume it's a line of dialogue
	        {
	            InteractionTreeNode linkedNode = new InteractionLinkedTreeNode();
				int lineIndex = 0;
				ParseDialogueLine(line, ref linkedNode, ref lineIndex);
				if (linkedNode == null)
				{
					throw new LineException("Bad format in line "+lineCounter+" in file "+file.name);
				}
				else
				{
					while(true)
					{
						if (lineIndex >= line.Length || line.IndexOf('[', lineIndex) == -1)
							break;
						InteractionTreeNode node = new InteractionTreeNode();
						ParseDialogueLine(line, ref node, ref lineIndex);
						if (node == null)
							throw new LineException("Bad format for line "+lineCounter+" in file "+file.name);
						else
							((InteractionLinkedTreeNode)linkedNode).responses.Add(node);
					}
				}

	            dialogueList.Add((InteractionLinkedTreeNode)linkedNode);
	            lineCounter++;
	        }
	    }
	}

	public string GetName()
	{
		return name;
	}

	public bool IsInteracting()
	{
		return steppingThrough;
	}

	/// <summary>
	/// Step through the dialogue, starting and stopping it if necessary.
	/// The return value is a list of strings where the first string is 
	/// the NPC line, and all following strings are player responses. The
	/// parameter is the index (starting from 0) of the player-selected 
	/// response, if applicable. If the list is empty dialogue has ended.
	/// </summary>
	/// <param name="responseIndex">Index (starting from 0) of the player-
	/// selected response, if applicable. If there are no responses no
	/// parameter is needed. </param>
	public List<string> Step(int responseIndex = 0)
	{
		List<string> lines = new List<string>();
		if (!steppingThrough)	//if we haven't started yet
		{
			steppingThrough = true;
			currentLine = GetStartingLine();
		}
		else
		{
			//execute the events for the NPC line and player response (if applicable)
			int nextLine = -1;
			bool willEnd = false;
			InteractionEvents npcEv = dialogueList[currentLine].eventType;
			InteractionEvents respEv = InteractionEvents.None;
			if (dialogueList[currentLine].responses.Count > 0 && responseIndex >= 0 
			    && responseIndex < dialogueList[currentLine].responses.Count)
				respEv = dialogueList[currentLine].responses[responseIndex].eventType;

			if ((npcEv & InteractionEvents.NextLine) > 0)
				nextLine = dialogueList[currentLine].nextLine - 1;
			if ((respEv & InteractionEvents.NextLine) > 0)
				nextLine = dialogueList[currentLine].responses[responseIndex].nextLine - 1;

			if ((npcEv & InteractionEvents.End) > 0)
				willEnd = true;
			if ((respEv & InteractionEvents.End) > 0)
				willEnd = true;

			if ((npcEv & InteractionEvents.SetConst) > 0)
				DialogueConstantParser.SetConstant(dialogueList[currentLine].gameConstant);
			if ((respEv & InteractionEvents.SetConst) > 0)
				DialogueConstantParser.SetConstant(dialogueList[currentLine].responses[responseIndex].gameConstant);

			if ((npcEv & InteractionEvents.Special) > 0)
				DialogueConstantParser.ExecuteEvent(dialogueList[currentLine].gameEvent);
			if ((respEv & InteractionEvents.Special) > 0)
				DialogueConstantParser.ExecuteEvent(dialogueList[currentLine].responses[responseIndex].gameEvent);

			if (nextLine == -1 || willEnd)
			{
				steppingThrough = false;
				return lines;
			}
			else
				currentLine = nextLine;
		}
		lines.Add(dialogueList[currentLine].line);
		foreach (InteractionTreeNode child in dialogueList[currentLine].responses)
			lines.Add(child.line);
		return lines;
	}
	
	private int GetStartingLine()
	{
		for (int i = controlList.Count - 1; i >= 0; i--)	//we look in reverse order in the file
		{
			if (ConditionalParser.EvaluateStatement(controlList[i].statement)) {
				if (controlList[i].randomStart != -1 && controlList[i].randomEnd != -1)
					return rand.Next(controlList[i].randomStart, controlList[i].randomEnd + 1) - 1;
				return controlList[i].lineNumber - 1;
			}
		}
		if (chooseStatement != null)
		{
			return rand.Next(chooseStatement.randomStart, chooseStatement.randomEnd + 1) - 1;
		}
	    return 0;
	}

	private void ParseDialogueLine(string fullLine, ref InteractionTreeNode node, ref int index)
	{
		int lineStartInd = fullLine.IndexOf('[', index);
		int lineEndInd = fullLine.IndexOf(']', lineStartInd+1);
		if (lineStartInd == -1 || lineEndInd == -1)
		{
			node = null;
			return;
		}
		
		int eventStartInd = fullLine.IndexOf('{', lineStartInd+1);
		if (!(eventStartInd == -1 || eventStartInd > lineEndInd)) 
		{
			node.line = fullLine.Substring(lineStartInd+1, (eventStartInd-lineStartInd)-1);

			int eventEndInd = fullLine.IndexOf ('}', eventStartInd+1);
			string eventText = fullLine.Substring(eventStartInd+1, (eventEndInd-eventStartInd)-1);
			string[] eventParams = eventText.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string eventParam in eventParams)
			{
				string[] eventArgs = eventParam.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				switch(eventArgs[0])
				{
				case "End":
				case "end":
					node.eventType |= InteractionEvents.End;
					break;
				case "Line":
				case "line":
					node.eventType |= InteractionEvents.NextLine;
					node.nextLine = Int32.Parse(eventArgs[1]);
					break;
				case "Set":
				case "set":
					node.eventType |= InteractionEvents.SetConst;
					node.gameConstant = eventArgs[1];
					break;
				case "Event":
				case "event":
					node.eventType |= InteractionEvents.Special;
					node.gameEvent = eventArgs[1];
					break;
				}
			}
		}
		else
			node.line = fullLine.Substring(lineStartInd+1, (lineEndInd-lineStartInd)-1);
		
		index = lineEndInd + 1;
	}
}
