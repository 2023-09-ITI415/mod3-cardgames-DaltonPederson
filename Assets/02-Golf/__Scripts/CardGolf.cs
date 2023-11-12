﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardGolf : MonoBehaviour {
	[Header("Set Dynamically")]

	public string suit;
	public int rank;
	public Color color = Color.black;
	public string colS = "Black";  // or "Red"

	public List<GameObject> decoGOs = new List<GameObject>();
	public List<GameObject> pipGOs = new List<GameObject>();

	public GameObject back;  // back of card;
	public CardDefinition def;  // from DeckXML.xml

	public SpriteRenderer[] spriteRenderers;

	void Start()
	{
		SetSortOrder(0);
	}

	public void PopulateSpriteRenderers()
	{

		if (spriteRenderers == null ||
			spriteRenderers.Length == 0)
		{
			spriteRenderers =
				GetComponentsInChildren<SpriteRenderer>();
		}
	}

	public void SetSortingLayerName(string tSLN)
	{
		PopulateSpriteRenderers();

		foreach (SpriteRenderer tSR in spriteRenderers)
		{
			tSR.sortingLayerName = tSLN;
		}

	}

	public void SetSortOrder(int sOrd)
	{
		PopulateSpriteRenderers();

		foreach (SpriteRenderer tSR in spriteRenderers)
		{
			if (tSR.gameObject == this.gameObject)
			{
				tSR.sortingOrder = sOrd;
				continue;
			}

			switch (tSR.gameObject.name)
			{
				case "back": //if the name is "back"

					tSR.sortingOrder = sOrd + 2;
					break;

				case "face": //if the name is "face"
				default: // or if name is anything else

					tSR.sortingOrder = sOrd + 1;
					break;
			}
		}
	}


	public bool faceUp {
		get {
			return (!back.activeSelf);
		}

		set {
			back.SetActive(!value);
		}
	}

	virtual public void OnMouseUpAsButton()
	{
		print(name);

	}

	// Update is called once per frame
	void Update () {
	
	}
} // class Card

[System.Serializable]
public class DecoratorGolf{
	public string	type;			// For card pips, type = "pip"
	public Vector3	loc;			// location of sprite on the card
	public bool		flip = false;	//whether to flip vertically
	public float 	scale = 1.0f;
}

[System.Serializable]
public class CardDefinitionGolf {
	public string face; //sprite to use for face cart
	public int rank;    // value from 1-13 (Ace-King)
	public List<DecoratorGolf>
		pips = new List<DecoratorGolf>();  // Pips Used
}




