using System;
using Items.Core;
using Cooking;

namespace Stations
{
    [Serializable]
    public class StationSlot
    {
        public bool occupied;
        public ItemAmount item;
        public CookingSession session;
    }
}
