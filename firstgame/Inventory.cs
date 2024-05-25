using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcanum
{
    internal class Inventory
    {
        public List<int> inventoryItemID = new();
        public List<int> inventoryItemAmount = new();
        public int currentHotbarSlot, currentHotbarSlotId, lastScrollValue = 0;

        public Inventory(Game1 game) 
        {
            inventoryItemID.Add(0);
            inventoryItemID.Add(3);
            inventoryItemID.Add(7);
            inventoryItemID.Add(10);
            inventoryItemID.Add(11);
            inventoryItemID.Add(1);
            inventoryItemID.Add(2);
            inventoryItemID.Add(12);

            currentHotbarSlot = 0;

        }
        public void InventorySystem()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue != lastScrollValue)
            {
                if (mouseState.ScrollWheelValue - lastScrollValue < 0)
                {
                    currentHotbarSlot++;

                }
                else
                {
                    currentHotbarSlot--;

                }

                currentHotbarSlot = Math.Clamp(currentHotbarSlot, 0, 7);
                lastScrollValue = mouseState.ScrollWheelValue;

            }

            currentHotbarSlotId = inventoryItemID[currentHotbarSlot];

        }

    }

}
