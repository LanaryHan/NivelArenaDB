using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CardGroupTitle : EnhancedScrollerCellView
    {
        public TMP_Text lineText;
        public TMP_Text cardCountText;
        public GameObject line;

        public void Init(string typeName, int cardCount)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                gameObject.SetActive(false);
                line.SetActive(false);
            }
            else
            {
                lineText.text = typeName;
                line.SetActive(true);
            }

            cardCountText.text = cardCount.ToString();
        }
    }
}