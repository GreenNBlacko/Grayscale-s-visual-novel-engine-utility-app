using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class HexInput : MonoBehaviour {
	public string value;

	public TMP_InputField HexInputField;

	public void SetValue(TMP_InputField input) {
		value = input.text;
	}

	public void SetValue(string input) {
		TMP_InputField.OnChangeEvent bak = HexInputField.onValueChanged;

		HexInputField.onValueChanged = new TMP_InputField.OnChangeEvent();

		HexInputField.text = input;
		value = input;

		HexInputField.onValueChanged = bak;
	}

	public string GetValue() {
		return value;
	}
}
