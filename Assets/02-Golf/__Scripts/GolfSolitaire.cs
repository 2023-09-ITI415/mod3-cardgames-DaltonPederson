using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Golf;

namespace Golf
{

    public enum eCardState
    {
        drawpile,
        tableau,
        target,
        discard
    }

    public class GolfSolitaire : Card
    {
        [Header("Set Dynamically: GolfSolitaire")]
        //this is how you use enum eCardState

        public eCardState state = eCardState.drawpile;
        //hiddenBy list stores which other cards
        //will keep this one face down

        public List<GolfSolitaire> hiddenBy =
            new List<GolfSolitaire>();
        //layout ID matches this card to the
        //tableau XML if it's a tableau card

        public int layoutID;
        //the slowDef class stores information pulled
        //in from the LayoutXML <slot>

        public SlotDef slotDef;

        override public void OnMouseUpAsButton()
        {
            Golf.S.CardClicked(this);

            base.OnMouseUpAsButton();
        }

    }

}
