using System.Collections.Generic;
using Items.Core;

namespace Progress
{
    // Holds the player's inventory across one scene load. It is taken (cleared) when restored,
    // so a later load that wasn't a travel (e.g. back to the main menu and a new game) starts empty.
    public sealed class InventoryCarryOver
    {
        private List<ItemAmount> _stored;

        public void Store(IReadOnlyList<ItemAmount> items)
        {
            _stored = new List<ItemAmount>(items.Count);
            for (var i = 0; i < items.Count; i++) _stored.Add(new ItemAmount(items[i]));
        }

        public bool TryTake(out List<ItemAmount> items)
        {
            items = _stored;
            _stored = null;
            return items != null;
        }
    }
}
