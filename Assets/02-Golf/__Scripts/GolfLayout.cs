    using System.Collections.Generic;
    using UnityEngine;
    using Golf;

namespace Golf
{

    [System.Serializable]
    public class SlotDefGolf
    {
        public float x;
        public float y;
        public bool faceUp = false;
        public string layerName = "Default";
        public int layerID = 0;
        public int id;
        public List<int> hiddenBy = new List<int>();
        public string type = "slot";
        public Vector2 stagger;
    }

    public class GolfLayout : MonoBehaviour
    {
        public PT_XMLReader xmlr; //just like Deck, this has
                                  //a PT_XMLReader

        public PT_XMLHashtable xml; //This variable is for
                                    //faster xml access

        public Vector2 multiplier; //offset of tableau's center

        //slotDef references

        public List<SlotDef> slotDefs; //All the SlotDefs for Row0-Row3

        public SlotDef drawPile;
        public SlotDef discardPile;

        //This holds all possible names for layers set by layerID

        public string[] sortingLayerNames =
            new string[] {"Row0", "Row1", "Row2", "Row3", "Row4",
        "Discard", "Draw"};

        //This function is called to read in the LayoutXML.xml file

        public void ReadLayout(string xmlText)
        {
            xmlr = new PT_XMLReader();
            xmlr.Parse(xmlText); //XML is parsed
            xml = xmlr.xml["xml"][0]; //and xml is set as
                                      //shortcut to xml

            //Read in the multiplier, which sets card spacing

            multiplier.x = float.Parse(xml["multiplier"][0]
                .att("x"));
            multiplier.y = float.Parse(xml["multiplier"][0]
                .att("y"));

            //read in slots

            SlotDef tSD;
            //slotsX is used as a shortcut to all <slot>s

            PT_XMLHashList slotsX = xml["slot"];

            for (int i = 0; i < slotsX.Count; i++)
            {
                tSD = new SlotDef(); //create a new SlotDef instance

                if (slotsX[i].HasAtt("type"))
                {
                    //if this <slot> has a type attribute parse it
                    tSD.type = slotsX[i].att("type");

                }
                else
                {
                    tSD.type = "slot";

                    //various attributes are parsed into
                    //numerical values
                }

                tSD.x = float.Parse(slotsX[i].att("x"));
                tSD.y = float.Parse(slotsX[i].att("y"));
                tSD.layerID =
                    int.Parse(slotsX[i].att("layer"));

                //this converts the number of the layerID
                //into a text layerName

                tSD.layerName =
                    sortingLayerNames[tSD.layerID];

                switch (tSD.type)
                {
                    //pull additional attributes based
                    //on type of this <slot>

                    case "slot":
                        tSD.faceUp = (slotsX[i].att("faceup")
                            == "1");

                        tSD.id =
                            int.Parse(slotsX[i].att("id"));
                        if (slotsX[i].HasAtt("hiddenby"))
                        {
                            string[] hiding =
                              slotsX[i].att("hiddenby").Split(',');
                            foreach (string s in hiding)
                            {
                                tSD.hiddenBy.Add(int.Parse(s));
                            }
                        }
                        slotDefs.Add(tSD);
                        break;

                    case "drawpile":
                        tSD.stagger.x =
                            float.Parse(slotsX[i].att("xstagger"));
                        drawPile = tSD;

                        break;

                    case "discardpile":
                        discardPile = tSD;
                        break;
                }
            }
        }


    }
}