using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SentenceAction : MonoBehaviour {
	public JsonFileIO.ActionsV2 ActionType;
	public string CharacterName;
	public string StateName;
	public float Delay;
	public bool FadeIn;
	public bool FadeOut;
	public bool Transition;
	public float FadeSpeed;
	public int BG;
	public int CG;
	public int CharacterIndex;
	public bool RunOnlyOnce;
	public bool EnterScene;
	public bool ExitScene;
	public int StartingPosition;
	public Vector2 StartPosition;
	public float TransitionSpeed;
	public Vector2 Position;
	public int BGMName;

	public TMP_Dropdown ActionTypeDropdown;
	public TMP_Dropdown CharacterNameDropdown;
	public TMP_Dropdown StateNameDropdown;
	public TMP_InputField DelayInput;
	public Toggle FadeInToggle;
	public Toggle FadeOutToggle;
	public Toggle TransitionToggle;
	public Slider FadeSpeedSlider;
	public TMP_InputField FadeSpeedInput;
	public TMP_Dropdown BGDropdown;
	public TMP_Dropdown CGDropdown;
	public TMP_InputField CharacterIndexInput; 
	public Toggle RunOnlyOnceToggle; 
	public Toggle EnterSceneToggle; 
	public Toggle ExitSceneToggle;
	public TMP_Dropdown StartingPositionDropdown;
	public TMP_InputField[] StartingPositionInput = new TMP_InputField[2];
	public Slider TransitionSpeedSlider;
	public TMP_InputField TransitionSpeedInput;
	public TMP_InputField[] PositionInput = new TMP_InputField[2];
	public TMP_Dropdown BGMNameDropdown;
	


	private JsonFileIO.ActionsV2 currentActionType = 0;
	public MenuSystem menuSystem;
	private JsonFileIO fileIO;

	void Start() {
		menuSystem = FindObjectOfType<MenuSystem>();
		fileIO = FindObjectOfType<JsonFileIO>();
		ManageVariables(ActionType, true);
		SetValues();
	}

	public void SetDropdownValues() {
		if (fileIO == null) {
			fileIO = FindObjectOfType<JsonFileIO>();
		}

		TMP_Dropdown.DropdownEvent tmp1 = CharacterNameDropdown.onValueChanged;
		TMP_Dropdown.DropdownEvent tmp2 = StateNameDropdown.onValueChanged;
		TMP_Dropdown.DropdownEvent tmp3 = BGMNameDropdown.onValueChanged;
		TMP_Dropdown.DropdownEvent tmp4 = CGDropdown.onValueChanged;
		TMP_Dropdown.DropdownEvent tmp5 = BGDropdown.onValueChanged;

		CharacterNameDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();
		StateNameDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();
		BGMNameDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();
		CGDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();
		BGDropdown.onValueChanged = new TMP_Dropdown.DropdownEvent();

		List<string> options = fileIO.GetCharacterList();

		int tmp = fileIO.ReturnCharacterIndex(CharacterName);

		if (options.Count != CharacterNameDropdown.options.Count) {
			CharacterNameDropdown.ClearOptions();
			CharacterNameDropdown.AddOptions(options);
		}

		for (int i = 0; i < CharacterNameDropdown.options.Count; i++) {
			if (i >= options.Count) {
				CharacterNameDropdown.ClearOptions();
				CharacterNameDropdown.AddOptions(options);
				break;
			}
			if (options[i] != CharacterNameDropdown.options[i].text) {
				CharacterNameDropdown.ClearOptions();
				CharacterNameDropdown.AddOptions(options);
			}
		}

		CharacterNameDropdown.value = tmp;

		if (CharacterNameDropdown.value >= CharacterNameDropdown.options.Count) {
			CharacterNameDropdown.value = 0;
		}

		options = fileIO.GetCharacterStates(CharacterName);

		tmp = fileIO.ReturnCharacterStateIndex(CharacterName, StateName);

		if (options.Count != StateNameDropdown.options.Count) {
			StateNameDropdown.ClearOptions();
			StateNameDropdown.AddOptions(options);
		}

		for (int i = 0; i < StateNameDropdown.options.Count; i++) {
			if (i >= options.Count) {
				StateNameDropdown.ClearOptions();
				StateNameDropdown.AddOptions(options);
				break;
			}
			if (options[i] != StateNameDropdown.options[i].text) {
				StateNameDropdown.ClearOptions();
				StateNameDropdown.AddOptions(options);
			}
		}

		StateNameDropdown.value = tmp;

		if (StateNameDropdown.value >= StateNameDropdown.options.Count) {
			StateNameDropdown.value = 0;
		}

		options = fileIO.GetBGMList();

		tmp = BGMName;

		if (options.Count != BGMNameDropdown.options.Count) {
			BGMNameDropdown.ClearOptions();
			BGMNameDropdown.AddOptions(options);
		}

		for (int i = 0; i < BGMNameDropdown.options.Count; i++) {
			if (i >= options.Count) {
				BGMNameDropdown.ClearOptions();
				BGMNameDropdown.AddOptions(options);
				break;
			}
			if (options[i] != BGMNameDropdown.options[i].text) {
				BGMNameDropdown.ClearOptions();
				BGMNameDropdown.AddOptions(options);
			}
		}

		BGMNameDropdown.value = tmp;

		if (BGMNameDropdown.value >= BGMNameDropdown.options.Count) {
			BGMNameDropdown.value = 0;
		}

		options = fileIO.GetArtworkList(0);

		tmp = CG;

		if (options.Count != CGDropdown.options.Count) {
			CGDropdown.ClearOptions();
			CGDropdown.AddOptions(options);
		}

		for (int i = 0; i < CGDropdown.options.Count; i++) {
			if (i >= options.Count) {
				CGDropdown.ClearOptions();
				CGDropdown.AddOptions(options);
				break;
			}
			if (options[i] != CGDropdown.options[i].text) {
				CGDropdown.ClearOptions();
				CGDropdown.AddOptions(options);
			}
		}

		CGDropdown.value = tmp;

		if (CGDropdown.value >= CGDropdown.options.Count) {
			CGDropdown.value = 0;
		}

		options = fileIO.GetArtworkList(1);

		tmp = BG;

		if (options.Count != BGDropdown.options.Count) {
			BGDropdown.ClearOptions();
			BGDropdown.AddOptions(options);
		}

		for (int i = 0; i < BGDropdown.options.Count; i++) {
			if (i >= options.Count) {
				BGDropdown.ClearOptions();
				BGDropdown.AddOptions(options);
				break;
			}
			if (options[i] != BGDropdown.options[i].text) {
				BGDropdown.ClearOptions();
				BGDropdown.AddOptions(options);
			}
		}

		BGDropdown.value = tmp;

		if (BGDropdown.value >= BGDropdown.options.Count) {
			BGDropdown.value = 0;
		}

		CharacterNameDropdown.onValueChanged = tmp1;
		StateNameDropdown.onValueChanged = tmp2;
		BGMNameDropdown.onValueChanged = tmp3;
		CGDropdown.onValueChanged = tmp4;
		BGDropdown.onValueChanged = tmp5;
	}

	public void SetValues() {

		ActionType = (JsonFileIO.ActionsV2)ActionTypeDropdown.value;
		CharacterName = CharacterNameDropdown.captionText.text;

		StateName = StateNameDropdown.captionText.text;
		Transition = TransitionToggle.isOn;
		EnterScene = EnterSceneToggle.isOn;
		ExitScene = ExitSceneToggle.isOn;
		RunOnlyOnce = RunOnlyOnceToggle.isOn;
		CharacterIndex = int.Parse(CharacterIndexInput.text);

		ManageVariables(ActionType);

		if (Transition && ActionType == 0) {
			StartingPositionDropdown.transform.parent.gameObject.SetActive(true);
		} else {
			StartingPositionDropdown.transform.parent.gameObject.SetActive(false);
		}
		StartingPosition = StartingPositionDropdown.value;

		if (StartingPosition == 2 && StartingPositionDropdown.transform.parent.gameObject.activeInHierarchy) {
			StartingPositionInput[0].transform.parent.parent.gameObject.SetActive(true);
		} else {
			StartingPositionInput[0].transform.parent.parent.gameObject.SetActive(false);
		}

		if (Transition && TransitionToggle.transform.parent.gameObject.activeInHierarchy || EnterScene && EnterSceneToggle.transform.parent.gameObject.activeInHierarchy || ExitScene && ExitSceneToggle.transform.parent.gameObject.activeInHierarchy) {
			TransitionSpeedSlider.transform.parent.gameObject.SetActive(true);
		} else {
			TransitionSpeedSlider.transform.parent.gameObject.SetActive(false);
		}

		StartPosition.x = float.Parse(StartingPositionInput[0].text);
		StartPosition.y = float.Parse(StartingPositionInput[1].text);

		Position.x = float.Parse(PositionInput[0].text);
		Position.y = float.Parse(PositionInput[1].text);

		TransitionSpeedSlider.value = (float)Math.Round((double)TransitionSpeedSlider.value, 2);

		if (TransitionSpeedSlider.value != TransitionSpeed) {
			TransitionSpeedInput.text = TransitionSpeedSlider.value.ToString();
			TransitionSpeed = TransitionSpeedSlider.value;
		}

		if (TransitionSpeedInput.text == "") { TransitionSpeedInput.text = "0"; }

		if (float.Parse(TransitionSpeedInput.text) != TransitionSpeed) {
			TransitionSpeedSlider.value = float.Parse(TransitionSpeedInput.text);
		}

		TransitionSpeed = TransitionSpeedSlider.value;

		FadeSpeedSlider.value = (float)Math.Round((double)FadeSpeedSlider.value, 2);

		if (FadeSpeedSlider.value != FadeSpeed) {
			FadeSpeedInput.text = FadeSpeedSlider.value.ToString();
			FadeSpeed = FadeSpeedSlider.value;
		}

		if (FadeSpeedInput.text == "") { FadeSpeedInput.text = "0"; }

		if (float.Parse(FadeSpeedInput.text) != FadeSpeed) {
			FadeSpeedSlider.value = float.Parse(FadeSpeedInput.text);
		}

		FadeSpeed = FadeSpeedSlider.value;

		FadeIn = FadeInToggle.isOn;
		FadeOut = FadeOutToggle.isOn;

		if (FadeIn && FadeInToggle.transform.parent.gameObject.activeInHierarchy || FadeOut && FadeOutToggle.transform.parent.gameObject.activeInHierarchy) {
			FadeSpeedSlider.transform.parent.gameObject.SetActive(true);
		} else {
			FadeSpeedSlider.transform.parent.gameObject.SetActive(false);
		}

		if (ActionType == JsonFileIO.ActionsV2.PlayBGM) {
			BGMNameDropdown.transform.parent.gameObject.SetActive(true);
		} else {
			BGMNameDropdown.transform.parent.gameObject.SetActive(false);
		}

		BGMName = BGMNameDropdown.value;

		if (ActionType == JsonFileIO.ActionsV2.Delay) {
			DelayInput.transform.parent.gameObject.SetActive(true);
		} else {
			DelayInput.transform.parent.gameObject.SetActive(false);
		}
	}

	public void ManageVariables(JsonFileIO.ActionsV2 actionType, bool overrideValues = false) {
		if (currentActionType == actionType && !overrideValues) { return; }
		DisableVariables();

		switch(actionType) {
			case JsonFileIO.ActionsV2.AddCharacterToScene: {
					CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
					StateNameDropdown.transform.parent.gameObject.SetActive(true);
					PositionInput[0].transform.parent.parent.gameObject.SetActive(true);
					EnterSceneToggle.transform.parent.gameObject.SetActive(true);
					FadeInToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.MoveCharacter: {
					CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
					PositionInput[0].transform.parent.parent.gameObject.SetActive(false);
					TransitionToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.ShowTransition: {
					
					break;
				}
			case JsonFileIO.ActionsV2.ShowBG: {
					BGDropdown.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.ShowCG: {
					CGDropdown.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.RemoveCharacterFromScene: {
					CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
					PositionInput[0].transform.parent.parent.gameObject.SetActive(true);
					ExitSceneToggle.transform.parent.gameObject.SetActive(true);
					FadeOutToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.ChangeCharacterState: {
					CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
					StateNameDropdown.transform.parent.gameObject.SetActive(true);
					TransitionToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.Delay: {
					DelayInput.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.WaitUntilActionIsFinished: {
					RunOnlyOnceToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.SetCharacterIndex: {
					CharacterIndexInput.transform.parent.gameObject.SetActive(true);
					break;
				}
			case JsonFileIO.ActionsV2.PlayBGM: {
					BGMNameDropdown.transform.parent.gameObject.SetActive(true);
					TransitionToggle.transform.parent.gameObject.SetActive(true);
					break;
				}
		}

		currentActionType = actionType;
	}

	public void DisableVariables() {
		CharacterNameDropdown.transform.parent.gameObject.SetActive(false);
		StateNameDropdown.transform.parent.gameObject.SetActive(false);
		StartingPositionDropdown.transform.parent.gameObject.SetActive(false);
		StartingPositionInput[0].transform.parent.parent.gameObject.SetActive(false);
		PositionInput[0].transform.parent.parent.gameObject.SetActive(false);
		TransitionToggle.transform.parent.gameObject.SetActive(false);
		FadeInToggle.transform.parent.gameObject.SetActive(false);
		FadeOutToggle.transform.parent.gameObject.SetActive(false);
		EnterSceneToggle.transform.parent.gameObject.SetActive(false);
		ExitSceneToggle.transform.parent.gameObject.SetActive(false);
		RunOnlyOnceToggle.transform.parent.gameObject.SetActive(false);
		CharacterIndexInput.transform.parent.gameObject.SetActive(false);
		BGMNameDropdown.transform.parent.gameObject.SetActive(false);
		DelayInput.transform.parent.gameObject.SetActive(false);
	}

	public void RemoveItem() {
		Destroy(this.gameObject);
	}
}
