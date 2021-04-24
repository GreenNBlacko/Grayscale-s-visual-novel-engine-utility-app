using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterState : MonoBehaviour {
	public string StateName;
	public int StateType;
	public string BaseLayerPath;
	public string ExpressionLayerPath;
	public bool Advanced;
	public Vector2 Offset;

	public TMP_InputField StateNameInput;
	public TMP_Dropdown StateTypeDropdown;
	public TMP_InputField BaseLayerInput;
	public TMP_InputField ExpressionLayerInput;
	public Toggle AdvancedToggle;
	public TMP_InputField[] OffsetInput = new TMP_InputField[2];

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
		Advanced = AdvancedToggle.isOn;
		Offset = new Vector2 { x = float.Parse(OffsetInput[0].text), y = float.Parse(OffsetInput[1].text) };

		if (currentStateType != StateType) {
			if (StateType == 0) {
				ExpressionLayerInput.transform.parent.gameObject.SetActive(false);
				AdvancedToggle.transform.parent.gameObject.SetActive(false);
			} else {
				ExpressionLayerInput.transform.parent.gameObject.SetActive(true);
				AdvancedToggle.transform.parent.gameObject.SetActive(true);
			}
		}

		if(Advanced && AdvancedToggle.transform.parent.gameObject.activeInHierarchy) {
			OffsetInput[0].transform.parent.parent.gameObject.SetActive(true);
		} else {
			OffsetInput[0].transform.parent.parent.gameObject.SetActive(false);
		}

		currentStateType = StateType;
	}

	public void RemoveItem() {
		Destroy(gameObject);
	}
}
