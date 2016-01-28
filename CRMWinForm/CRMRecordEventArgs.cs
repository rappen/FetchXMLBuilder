using Microsoft.Xrm.Sdk;
using System;

namespace Cinteros.Xrm.CRMWinForm
{
    public class CRMRecordEventArgs : EventArgs
    {
        private Entity entity;

        public CRMRecordEventArgs(Entity entity)
        {
            this.entity = entity;
        }

        public Entity Entity { get { return entity; } }
    }
}
