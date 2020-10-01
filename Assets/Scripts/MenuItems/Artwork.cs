using UnityEngine;
using TMPro;

public class Artwork : MonoBehaviour {
  public string ArtworkName;
  public int ArtworkType;
  public string ArtworkPath;
  
  public TMP_InputField ArtworkNameInput;
  public TMP_Dropdown ArtworkTypeDropdown;
  public TMP_InputField ArtworkPathInput;
  
  void Start() {
    SetValues();
  }
  
  public void SetValues() {
    ArtworkName = ArtworkNameInput.text;
    ArtworkType = ArtworkTypeDropdown.value;
    ArtworkPath = ArtworkPathInput.text;
  }
  
  public void RemoveItem() {
    Destroy(this.gameObject);
  }
}