using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.Core
{
    public static class ItemsUtility
    {
        public static bool Stackable(params ItemAmount[] itemAmounts)
        {
            return itemAmounts.Length > 0
                   && itemAmounts.All(i => !i.IsEmpty && i.SoItem == itemAmounts[0].SoItem);
        }
    }
}
