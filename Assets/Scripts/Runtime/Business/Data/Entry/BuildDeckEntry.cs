using System.Collections.Generic;

namespace Runtime.Business.Data.Entry
{
    public class BuildDeckEntry
    {
        public string DeckName;
        public List<string> CardIds;

        public BuildDeckEntry(string deckName, List<string> cardIds)
        {
            DeckName = deckName;
            CardIds = cardIds;
        }
    }
}