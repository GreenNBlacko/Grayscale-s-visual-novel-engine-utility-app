using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SentenceAction : MonoBehaviour {
	public int ActionType;
	public string CharacterName;
	public string StateName;
	public bool Transition;
	public int StartingPosition;
	public Vector2 StartPosition;
	public Vector2 Postion;
	public float TransitionSpeed;
	public bool FadeIn;
	public bool FadeOut;
	public float FadeSpeed;
	public int BGMName;
	public float Delay;

	public TMP_Dropdown ActionTypeDropdown;
	public TMP_Dropdown CharacterNameDropdown;
	public TMP_Dropdown StateNameDropdown;
	public Toggle TransitionToggle;
	public TMP_Dropdown StartingPositionDropdown;
	public TMP_InputField[] StartingPositionInput = new TMP_InputField[2];
	public TMP_InputField[] PositionInput = new TMP_InputField[2];
	public Slider TransitionSpeedSlider;
	public TMP_InputField TransitionSpeedInput;
	public Toggle FadeInToggle;
	public Toggle FadeOutToggle;
	public Slider FadeSpeedSlider;
	public TMP_InputField FadeSpeedInput;
	public TMP_Dropdown BGMNameDropdown;
	public TMP_InputField DelayInput;

	void Start() {
		SetValues();
	}

	public void SetValues() {
		ActionType = ActionTypeDropdown.value;
		CharacterName = CharacterNameDropdown.captionText.text;
		StateName = StateNameDropdown.captionText.text;
		Transition = TransitionToggle.isOn;

		ManageVariables(ActionTypeDropdown.value);

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

		if (Transition && TransitionToggle.transform.parent.gameObject.activeInHierarchy) {
			TransitionSpeedSlider.transform.parent.gameObject.SetActive(true);
		} else {
			TransitionSpeedSlider.transform.parent.gameObject.SetActive(false);
		}

		StartPosition.x = float.Parse(StartingPositionInput[0].text);
		StartPosition.y = float.Parse(StartingPositionInput[1].text);

		Postion.x = float.Parse(PositionInput[0].text);
		Postion.y = float.Parse(PositionInput[1].text);

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

		if (ActionType == 4) {
			BGMNameDropdown.transform.parent.gameObject.SetActive(true);
		} else {
			BGMNameDropdown.transform.parent.gameObject.SetActive(false);
		}

		BGMName = BGMNameDropdown.value;

		if (ActionType == 5) {
			DelayInput.transform.parent.gameObject.SetActive(true);
		} else {
			DelayInput.transform.parent.gameObject.SetActive(false);
		}
	}

	public void ManageVariables(int actionType) {
		DisableVariables();
		if (actionType == 0) {
			CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
			StateNameDropdown.transform.parent.gameObject.SetActive(true);
			PositionInput[0].transform.parent.parent.gameObject.SetActive(true);
			TransitionToggle.transform.parent.gameObject.SetActive(true);
			FadeInToggle.transform.parent.gameObject.SetActive(true);
		} else if (actionType == 1) {
			CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
			PositionInput[0].transform.parent.parent.gameObject.SetActive(false);
			TransitionToggle.transform.parent.gameObject.SetActive(true);
		} else if (actionType == 2) {
			CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
			StateNameDropdown.transform.parent.gameObject.SetActive(true);
			TransitionToggle.transform.parent.gameObject.SetActive(true);
		} else if (actionType == 3) {
			CharacterNameDropdown.transform.parent.gameObject.SetActive(true);
			PositionInput[0].transform.parent.parent.gameObject.SetActive(true);
			TransitionToggle.transform.parent.gameObject.SetActive(true);
			FadeOutToggle.transform.parent.gameObject.SetActive(true);
		} else if (actionType == 4) {
			BGMNameDropdown.transform.parent.gameObject.SetActive(true);
			TransitionToggle.transform.parent.gameObject.SetActive(true);

		} else if (actionType == 5) {
			DelayInput.transform.parent.gameObject.SetActive(true);
		}
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
		BGMNameDropdown.transform.parent.gameObject.SetActive(false);
		DelayInput.transform.parent.gameObject.SetActive(false);
	}

	public void RemoveItem() {
		Destroy(this.gameObject);
	}
}
