using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class CharacterState : MonoBehaviour {
	public string StateName;
	public int StateType;
	public string BaseLayerPath;
	public string ExpressionLayerPath;

	public TMP_InputField StateNameInput;
	public TMP_Dropdown StateTypeDropdown;
	public TMP_InputField BaseLayerInput;
	public TMP_InputField ExpressionLayerInput;

	private int currentStateType;

	// Start is called before the first frame update
	void Start() {
		SetValues();
	}

	public void SetValues() {
		StateName = StateNameInput.text;
		StateType = StateTypeDropdown.value;
		BaseLayerPath = BaseLayerInput.text;
		ExpressionLayerPath = ExpressionLayerInput.text;

		if (currentStateType != StateType) {
			if (StateType == 0) {
				ExpressionLayerInput.transform.parent.gameObject.SetActive(false);
			} else {
				ExpressionLayerInput.transform.parent.gameObject.SetActive(true);
			}
		}

		currentStateType = StateType;
	}

	public void RemoveItem() {
		Destroy(gameObject);
	}
}
