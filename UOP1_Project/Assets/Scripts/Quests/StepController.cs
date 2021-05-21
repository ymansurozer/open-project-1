﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//this script needs to be put on the actor, and takes care of the current step to accomplish.
//the step contains a dialogue and maybe an event.

public class StepController : MonoBehaviour
{
	[Header("Data")]
	[SerializeField] private ActorSO _actor = default;
	[SerializeField] private DialogueDataSO _defaultDialogue = default;
	[SerializeField] private QuestManagerSO _questData = default;

	[Header("Listening to channels")]
	[SerializeField] private DialogueActorChannelSO _interactionEvent = default;
	[SerializeField] private VoidEventChannelSO _winDialogueEvent = default;
	[SerializeField] private VoidEventChannelSO _loseDialogueEvent = default;

	[Header("Broadcasting on channels")]
	//[SerializeField]
	public DialogueDataChannelSO _startDialogueEvent = default;
	[Header("Listening to")]
	//[SerializeField]
	public VoidEventChannelSO _endDialogueEvent = default;

	//check if character is actif. An actif character is the character concerned by the step.
	private DialogueDataSO _currentDialogue;

	public bool isInDialogue; //Consumed by the state machine

	private void Start()
	{

	}

	void PlayDefaultDialogue()
	{

		if (_defaultDialogue != null)
		{
			_currentDialogue = _defaultDialogue;
			StartDialogue();
		}

	}


	//start a dialogue when interaction
	//some Steps need to be instantanious. And do not need the interact button.
	//when interaction again, restart same dialogue.
	public void InteractWithCharacter()
	{

		DialogueDataSO displayDialogue = _questData.InteractWithCharacter(_actor, false, false);
		Debug.Log("dialogue " + displayDialogue + "actor" + _actor);
		if (displayDialogue != null)
		{
			_currentDialogue = displayDialogue;
			StartDialogue();
		}
		else
		{
			PlayDefaultDialogue();
		}

	}


	void StartDialogue()
	{
		_startDialogueEvent.RaiseEvent(_currentDialogue);
		_endDialogueEvent.OnEventRaised += EndDialogue;
		StopTalkingToCurrentActor();
		_winDialogueEvent.OnEventRaised += PlayWinDialogue;
		_loseDialogueEvent.OnEventRaised += PlayLoseDialogue;
		isInDialogue = true;
		gameObject.GetComponent<NPC>().hasSaidLine = false;
	}
	void EndDialogue()
	{
		_endDialogueEvent.OnEventRaised -= EndDialogue;
		_winDialogueEvent.OnEventRaised -= PlayWinDialogue;
		_loseDialogueEvent.OnEventRaised -= PlayLoseDialogue;
		ResumeTalkingToCurrentActor();
		isInDialogue = false;
	}

	void PlayLoseDialogue()
	{
		if (_questData != null)
		{
			DialogueDataSO displayDialogue = _questData.InteractWithCharacter(_actor, true, false);
			if (displayDialogue != null)
			{
				_currentDialogue = displayDialogue;
				StartDialogue();

			}

		}
	}

	void PlayWinDialogue()
	{
		if (_questData != null)
		{
			DialogueDataSO displayDialogue = _questData.InteractWithCharacter(_actor, true, true);
			if (displayDialogue != null)
			{
				_currentDialogue = displayDialogue;
				StartDialogue();
			}
		}
	}

	private void StopTalkingToCurrentActor()
	{
		GameObject[] talkingTo = gameObject.GetComponent<NPC>().talkingTo;
		if (talkingTo != null)
		{
			for (int i = 0; i < talkingTo.Length; ++i)
			{
				talkingTo[i].GetComponent<NPC>().npcState = NPCState.Idle;
			}
		}
	}

	private void ResumeTalkingToCurrentActor()
	{
		GameObject[] talkingTo = GetComponent<NPC>().talkingTo;
		if (talkingTo != null)
		{

			for (int i = 0; i < talkingTo.Length; ++i)
			{
				talkingTo[i].GetComponent<NPC>().npcState = NPCState.Talk;
			}
		}
	}
}
