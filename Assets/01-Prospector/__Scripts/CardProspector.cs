using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    //this is how you use enum eCardState

    public eCardState state = eCardState.drawpile;
    //hiddenBy list stores which other cards
    //will keep this one face down

    public List<CardProspector> hiddenBy =
        new List<CardProspector>();
    //layout ID matches this card to the
    //tableau XML if it's a tableau card

    public int layoutID;
    //the slowDef class stores information pulled
    //in from the LayoutXML <slot>

    public SlotDef slotDef;

    override public void OnMouseUpAsButton()
    {
        Prospector.S.CardClicked(this);

        base.OnMouseUpAsButton();
    }

}
