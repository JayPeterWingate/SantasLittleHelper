using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestPlayerScript : MonoBehaviour {

	public Image image;
	public TextMeshProUGUI text;

	public Sprite onImage;
	public Sprite offImage;


	public void SetText(string name)
	{
		text.text = name;
	}
	public void SetValue(bool value)
	{
		image.sprite = value ? onImage : offImage;
	}
}