using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CardGroupTitle : EnhancedScrollerCellView
    {
        public TMP_Text lineText;
        public GameObject line;

        public void Init(string typeName)
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
        }
    }
}