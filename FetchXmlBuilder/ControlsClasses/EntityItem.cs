﻿using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.XmlEditorUtils;

namespace Rappen.XTB.FetchXmlBuilder.ControlsClasses
{
    public class EntityItem : IComboBoxItem
    {
        private EntityMetadata meta = null;

        public EntityMetadata Meta { get => meta; }

        public EntityItem(EntityMetadata Entity)
        {
            meta = Entity;
        }

        public override string ToString()
        {
            return FetchXmlBuilder.GetEntityDisplayName(meta);
        }

        public string GetValue()
        {
            return meta.LogicalName;
        }
    }
}