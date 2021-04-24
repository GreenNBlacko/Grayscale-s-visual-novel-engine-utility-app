using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class DialoguePreview : MonoBehaviour {

	public JsonFileIO IOScript;

	public ItemMenu Item;

	[Header("Prefabs")]
	public GameObject ChapterPrefab;
	public GameObject SentencePrefab;
	public GameObject SentenceActionPrefab;
	public GameObject ChoicePrefab;
	public GameObject ChoiceOptionPrefab;


	public void OpenMenu() {
		SentencesDataV2 data = IOScript.GetData();

		Dictionary<int, ChapterDataV2> chapters = new Dictionary<int, ChapterDataV2>();

		int nextchapter = 0;

		for(int i = 0; i < data.chapters.Length; i++) {
			chapters.Add(i, data.chapters[i]);
		}

		foreach(ChapterDataV2 temp in data.chapters) {
			if(chapters.TryGetValue(nextchapter, out ChapterDataV2 chapter)) {
				GameObject chapterGO = Instantiate(ChapterPrefab, Item.ItemList);

				PreviewChapter chapterscript = chapterGO.transform.GetComponent<PreviewChapter>();

				chapterGO.name = chapter.ChapterName;
				chapterscript.Title.text = chapter.ChapterName;

				nextchapter = chapter.NextChapter;

				foreach(SentenceDataV2 sentence in chapter.sentences) {
					GameObject sentenceGO = Instantiate(SentencePrefab, chapterscript.ItemList);
					sentenceGO.name = sentence.Name;

					PreviewSentence sentenceScript = sentenceGO.transform.GetComponent<PreviewSentence>();

					CharacterDataV2 character = ReturnCharacter(data, sentence.Name);

					VertexGradient NameColor = new VertexGradient(Color.white);
					VertexGradient TextColor = new VertexGradient(Color.white);

					if (character.useSeperateColors) {
						if(character.useSeperateGradientColors) {
							switch (character.gradientType) {
								case 0: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex));
										break;
									}
								case 1: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex), HexToColor(character.nameColorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex));
										break;
									}
								case 2: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex), HexToColor(character.textColorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
								case 3: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex), HexToColor(character.nameColorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex), HexToColor(character.textColorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
							}
						} else {
							switch (character.gradientType) {
								case 0: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex));
										break;
									}
								case 1: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex), HexToColor(character.nameColorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex));
										break;
									}
								case 2: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex), HexToColor(character.textColorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
								case 3: {
										NameColor = new VertexGradient(HexToColor(character.nameColorHex), HexToColor(character.nameColorHex), HexToColor(character.colorGradientHex), HexToColor(character.colorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.textColorHex), HexToColor(character.textColorHex), HexToColor(character.colorGradientHex), HexToColor(character.colorGradientHex));
										break;
									}
							}
						}
					} else {
						if (character.useSeperateGradientColors) {
							switch (character.gradientType) {
								case 0: {
										NameColor = new VertexGradient(HexToColor(character.colorHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex));
										break;
									}
								case 1: {
										NameColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex));
										break;
									}
								case 2: {
										NameColor = new VertexGradient(HexToColor(character.colorHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
								case 3: {
										NameColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
							}
						} else {
							switch (character.gradientType) {
								case 0: {
										NameColor = new VertexGradient(HexToColor(character.colorHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex));
										break;
									}
								case 1: {
										NameColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.nameColorGradientHex), HexToColor(character.nameColorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex));
										break;
									}
								case 2: {
										NameColor = new VertexGradient(HexToColor(character.colorHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.textColorGradientHex), HexToColor(character.textColorGradientHex));
										break;
									}
								case 3: {
										NameColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.colorGradientHex), HexToColor(character.colorGradientHex));
										TextColor = new VertexGradient(HexToColor(character.colorHex), HexToColor(character.colorHex), HexToColor(character.colorGradientHex), HexToColor(character.colorGradientHex));
										break;
									}
							}
						}
					}

					foreach(SentenceActionDataV2 action in sentence.sentenceActions) {
						GameObject actionGO = Instantiate(SentenceActionPrefab, sentenceScript.ItemList);

						PreviewSentenceAction actionScript = actionGO.GetComponent<PreviewSentenceAction>();

						var r = new Regex(@"
						(?<=[A-Z])(?=[A-Z][a-z]) |
						(?<=[^A-Z])(?=[A-Z]) |
						(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

						actionScript.SetValues(r.Replace(action.ActionType.ToString(), " "));
					}

					sentenceScript.SetValues((sentence.OverrideName) ? sentence.DisplayName : sentence.Name, sentence.Text, NameColor, TextColor);
				}
			}
		}
	}

	public void CloseMenu() {
		foreach(Transform menu in Item.ItemList) {
			Destroy(menu.gameObject);
		}
	}

	public CharacterDataV2 ReturnCharacter(SentencesDataV2 data, string name) {
		CharacterDataV2 character = new CharacterDataV2();

		foreach(CharacterDataV2 temp in data.characterData) {
			if(temp.characterName == name) {
				character = temp;
				break;
			}
		}

		return character;
	}

	public Color32 HexToColor(string hexValue) {
		Color temp = new Color(1,1,1,1);
		try {
			temp = new Color { r = System.Convert.ToInt32(hexValue.Substring(0, 2), 16), g = System.Convert.ToInt32(hexValue.Substring(2, 2), 16), b = System.Convert.ToInt32(hexValue.Substring(4, 2), 16), a = 255 };
		} catch {}
		return new Color32((byte)temp.r, (byte)temp.g, (byte)temp.b, (byte)temp.a);
	}
}
