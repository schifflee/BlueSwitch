﻿using System.Linq;
using BlueSwitch.Base.Components.Base;
using BlueSwitch.Base.Components.Switches.Base;

namespace BlueSwitch.Base.Components.Switches.Meta
{
    public class PrefabSwitch : SwitchBase
    {
        public Prefab Prefab { get; set; }

        protected override void OnInitialize(Engine renderingEngine)
        {
            base.OnInitialize(renderingEngine);
            //AutoDiscoverDisabled = true;

            LoadInputOutputs();
            LoadPrefabDetails();
        }

        private void LoadPrefabDetails()
        {
            if (Prefab != null)
            {
                DisplayName = Prefab.Name;
                Description = Prefab.Description;
            }
        }
        
        private void LoadInputOutputs()
        {
            if (Prefab != null)
            {
                var outputDefinition = Prefab.Project.Items.FirstOrDefault(x => x is OutputDefinitionSwitch);

                if (outputDefinition != null)
                {
                    foreach (var item in outputDefinition.Inputs)
                    {
                        AddOutput(item.Signature, item.UIComponent);
                    }
                }

                var inputDefinition = Prefab.Project.Items.FirstOrDefault(x => x is InputDefinitionSwitch);

                if (inputDefinition != null)
                {
                    foreach (var item in inputDefinition.Outputs)
                    {
                        AddInput(item.Signature, item.UIComponent);
                    }
                }
            }
        }

        public override GroupBase OnSetGroup()
        {
            return Groups.Prefabs;
        }
    }
}
