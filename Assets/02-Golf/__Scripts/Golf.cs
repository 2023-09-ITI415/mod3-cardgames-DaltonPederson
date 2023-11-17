using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Golf;

namespace Golf
{

	public class Golf : MonoBehaviour
	{

		static public Golf S;

		[Header("Set in Inspector")]
		public TextAsset deckXMLgolf;
		public TextAsset layoutXMLgolf;
		public float xOffset = 3;
		public float yOffset = -2.5f;
		public Vector3 layoutCenter;
		public Vector2 fsPosMid = new
			Vector2(0.5f, 0.90f);
		public Vector2 fsPosRun = new
			Vector2(0.5f, 0.90f);
		public Vector2 fsPosMid2 = new
			Vector2(0.4f, 1.0f);
		public Vector2 fsPosEnd = new
			Vector2(0.5f, 0.95f);


		[Header("Set Dynamically")]
		public DeckGolf deck;
		public GolfLayout layout;
		public List<GolfSolitaire> drawPile;
		public Transform layoutAnchor;
		public GolfSolitaire target;
		public List<GolfSolitaire> tableau;
		public List<GolfSolitaire> discardPile;
		public FloatingScoreGolf fsRun;
		private bool go;

		void Awake()
		{
			S = this;
		}

		void Start()
		{
			deck = GetComponent<DeckGolf>();
            deck.InitDeck(deckXMLgolf.text);

			if (go)
			{
				ScoreboardGolf.S.score = ScoreManagerGolf.SCORE;
			}
			else
			{
				Debug.Log("No game object called Scoreboard found");
			}

			Deck.Shuffle(ref deck.cards);

			Card c;
			for (int cNum = 0; cNum < deck.cards.Count; cNum++)
			{
				c = deck.cards[cNum];
				c.transform.localPosition = new Vector3((cNum % 13) *
					3, cNum / 13 * 4, 0);
			}

			layout = GetComponent<GolfLayout>();
			layout.ReadLayout(layoutXMLgolf.text);

			drawPile =
				ConvertListCardsToListGolfSolitaire(deck.cards);

			LayoutGame();
		}

		List<GolfSolitaire>
			ConvertListCardsToListGolfSolitaire(List<Card> ICD)
		{
			List<GolfSolitaire> ICP = new
				List<GolfSolitaire>();
			GolfSolitaire tCP;
			foreach (Card tCD in ICD)
			{
				tCP = tCD as
					GolfSolitaire;
				ICP.Add(tCP);
			}
			return (ICP);

		}

		GolfSolitaire Draw()
		{
			GolfSolitaire cd = drawPile[0];
			drawPile.RemoveAt(0);

			return (cd);
		}

		void LayoutGame()
		{
			if (layoutAnchor == null)
			{
				GameObject tGO = new
					GameObject("_LayoutAnchor");

				layoutAnchor =
					tGO.transform;
				layoutAnchor.transform.position =
					layoutCenter;
			}

			GolfSolitaire cp;

			foreach (SlotDef tSD in layout.slotDefs)
			{
				cp = Draw();

				cp.faceUp = tSD.faceUp;

				cp.transform.parent = layoutAnchor;

				cp.transform.localPosition = new Vector3(
					layout.multiplier.x * tSD.x,
					layout.multiplier.y * tSD.y,
					-tSD.layerID);

				cp.layoutID = tSD.id;
				cp.slotDef = tSD;

				cp.state = eCardState.tableau;

				cp.SetSortingLayerName(tSD.layerName);

				tableau.Add(cp);

			}

			foreach (GolfSolitaire tCP in tableau)
			{
				foreach (int hid in tCP.slotDef.hiddenBy)
				{
					cp = FindCardByLayoutID(hid);
					tCP.hiddenBy.Add(cp);
				}
			}

			MoveToTarget(Draw());

			UpdateDrawPile();
		}

		GolfSolitaire FindCardByLayoutID(int layoutID)
		{
			foreach (GolfSolitaire tCP in tableau)
			{
				if (tCP.layoutID == layoutID)
				{

					return (tCP);
				}
			}

			return (null);
		}

		void SetTableauFaces()
		{
			foreach (GolfSolitaire cd in tableau)
			{
				bool faceUp = true;
				foreach (GolfSolitaire cover in cd.hiddenBy)
				{
					if (cover.state == eCardState.tableau)
					{
						faceUp = false;
					}
				}

				cd.faceUp = faceUp;
			}
		}


		void MoveToDiscard(GolfSolitaire cd)
		{

			cd.state = eCardState.discard;
			discardPile.Add(cd);
			cd.transform.parent = layoutAnchor;

			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * layout.discardPile.x,
				layout.multiplier.y * layout.discardPile.y,
				-layout.discardPile.layerID + 0.5f);
			cd.faceUp = true;

			cd.SetSortingLayerName(layout.discardPile.layerName);
			cd.SetSortOrder(-100 + discardPile.Count);
		}

		void MoveToTarget(GolfSolitaire cd)
		{

			if (target != null) MoveToDiscard(target);
			target = cd;
			cd.state = eCardState.target;
			cd.transform.parent = layoutAnchor;

			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * layout.discardPile.x,
				layout.multiplier.y * layout.discardPile.y,
				-layout.discardPile.layerID);

			cd.faceUp = true;

			cd.SetSortingLayerName(layout.discardPile.layerName);
			cd.SetSortOrder(0);
		}

		//Arrange all cards of the drawPile to show
		//how many are left

		void UpdateDrawPile()
		{
			GolfSolitaire cd;
			//Go through all cards of the drawPile

			for (int i = 0; i < drawPile.Count; i++)
			{
				cd = drawPile[i];
				cd.transform.parent = layoutAnchor;

				//Position is correctly with layout.drawPile.stagger

				Vector2 dpStagger = layout.drawPile.stagger;
				cd.transform.localPosition = new Vector3(
					layout.multiplier.x * (layout.drawPile.x
					+ i * dpStagger.x),
					layout.multiplier.y * (layout.drawPile.y
					+ i * dpStagger.y),
					-layout.drawPile.layerID + 0.1f * i);

				cd.faceUp = false;
				cd.state = eCardState.drawpile;

				cd.SetSortingLayerName(layout.drawPile.layerName);
				cd.SetSortOrder(-10 * i);
			}
		}

		public void CardClicked(GolfSolitaire cd)
		{
			switch (cd.state)
			{
				case eCardState.target:

					break;

				case eCardState.drawpile:

					MoveToDiscard(target);

					MoveToTarget(Draw());

					UpdateDrawPile();

					ScoreManagerGolf.EVENT(eScoreEvent.draw);
					FloatingScoreHandler(eScoreEvent.draw);

					break;

				case eCardState.tableau:

					bool validMatch = true;
					if (!cd.faceUp)
					{
						validMatch = false;
					}
					if (!AdjacentRank(cd, target))
					{
						validMatch = false;
					}
					if (!validMatch) return;

					tableau.Remove(cd);

					MoveToTarget(cd);

					SetTableauFaces();

					ScoreManagerGolf.EVENT(eScoreEvent.mine);
					FloatingScoreHandler(eScoreEvent.mine);

					break;

			}

			CheckForGameOver();
		}

		void CheckForGameOver()
		{
			if (tableau.Count == 0)
			{
				GameOver(true);
				return;
			}

			if (drawPile.Count > 0)
			{
				return;
			}
			foreach (GolfSolitaire cd in tableau)
			{
				if (AdjacentRank(cd, target))
				{
					return;
				}
			}

			GameOver(false);
		}

		void GameOver(bool won)
		{
			if (won)
			{
				print("Game Over. You won! :)");
				ScoreManagerGolf.EVENT(eScoreEvent.gameWin);
				FloatingScoreHandler(eScoreEvent.gameWin);
			}
			else
			{
				print("Game Over. You Lost. :(");
				ScoreManagerGolf.EVENT(eScoreEvent.gameLoss);
				FloatingScoreHandler(eScoreEvent.gameLoss);
			}

			SceneManager.LoadScene("__Golf");
		}


		public bool AdjacentRank(GolfSolitaire cO,
			GolfSolitaire c1)
		{
			if (!cO.faceUp || !c1.faceUp) return (false);

			if (Mathf.Abs(cO.rank - c1.rank) == 1)
			{
				return (true);
			}

			if (cO.rank == 1 && c1.rank == 13)
				return (true);
			if (cO.rank == 13 && c1.rank == 1)
				return (true);

			return (false);
		}

		void FloatingScoreHandler(eScoreEvent evt)
		{
			List<Vector2> fsPts;
			switch (evt)
			{

				case eScoreEvent.draw:
				case eScoreEvent.gameWin:
				case eScoreEvent.gameLoss:
					if (fsRun != null)
					{
						fsPts = new List<Vector2>();
						fsPts.Add(fsPosMid2);
						fsPts.Add(fsPosMid2);
						fsPts.Add(fsPosEnd);
						fsRun.reportFinishTo =
							Scoreboard.S.gameObject;
						fsRun.fontSizes = new List<float>(new
							float[] { 28, 36, 4 });
						fsRun = null;
					}
					break;

				case eScoreEvent.mine:
					FloatingScoreGolf fs;

					Vector2 pO = Input.mousePosition;
					pO.x /= Screen.width;
					pO.y /= Screen.height;
					fsPts = new List<Vector2>();
					fsPts.Add(pO);
					fsPts.Add(fsPosMid);
					fsPts.Add(fsPosRun);

					fs =
						ScoreboardGolf.S.CreateFloatingScore
						(ScoreManagerGolf.CHAIN, fsPts);
					fs.fontSizes = new List<float>(new float[]
						{4,50,28});
					if (fsRun == null)
					{
						fsRun = fs;
						fsRun.reportFinishTo = null;
					}
					else
					{
						fs.reportFinishTo = fsRun.gameObject;
					}
					break;
			}
		}
	}

}
