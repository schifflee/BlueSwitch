﻿using System;
using BlueSwitch.Base.Components.Base;
using BlueSwitch.Base.Components.Switches.Base;
using BlueSwitch.Base.Components.UI;
using BlueSwitch.Base.Processing;

namespace BlueSwitch.Base.Components.Switches.Variables
{
    public class Int32Switch : SwitchBase
    {
        protected TextEdit TextEdit;

        protected override void OnInitialize(Engine renderingEngine)
        {
            TextEdit = new TextEdit();
            AddOutput(typeof(int), TextEdit);
            UniqueName = "BlueSwitch.Base.Components.Switches.Variables.Int32";
            DisplayName = "Int32";
            Description = "A Int32 variable";
            TextEdit.AllowDecimalPoint = false;
            TextEdit.NumberMode = true;
        }

        public override GroupBase OnSetGroup()
        {
            return Groups.Variable;
        }

        protected override void OnProcessData<T>(Processor p, ProcessingNode<T> node)
        {
            SetData(0, new DataContainer(Convert.ToInt32(TextEdit.NumberValue)));
            base.OnProcessData(p, node);
        }
    }
}
