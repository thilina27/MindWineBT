// © Thilina

using System;

namespace BT.Runtime.Attributes
{
    public class CreateNodeMenu : Attribute
    {
        public string MenuPath { get; private set; }
        public string ItemName { get; private set; }

        public CreateNodeMenu(string menuPath = "", string itemName = "")
        {
            MenuPath = menuPath;
            ItemName = itemName;
        }
    }
}
