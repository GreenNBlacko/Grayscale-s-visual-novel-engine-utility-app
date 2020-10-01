using UnityEngine;
using TMPro;

public class BGM : MonoBehaviour {
  public string BGMName;
  public string BGMPath;
  
  public TMP_InputField BGMNameInput;
  public TMP_InputField BGMPathInput;
  
  void Start() {
    SetValues();
  }
  
  public void SetValues() {
    BGMName = BGMNameInput.text;
    BGMPath = BGMPathInput.text;
  }
  
  public void RemoveItem() {
    Destroy(this.gameObject);
  }
}