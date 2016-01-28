using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Cinteros.Xrm.CRMWinForm
{
    public partial class CRMGridView : DataGridView
    {
        private IOrganizationService organizationService;
        private EntityCollection entityCollection;
        private bool autoRefresh = true;
        private bool showFriendlyNames = false;
        private bool showIdColumn = true;
        private bool showIndexColumn = true;

        public CRMGridView()
        {
            InitializeComponent();
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = true;
            AllowUserToResizeRows = false;
            CellClick += HandleRecordClick;
            CellDoubleClick += HandleRecordDoubleClick;
        }

        [Category("CRM")]
        public IOrganizationService OrganizationService
        {
            get { return organizationService; }
            set
            {
                organizationService = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        [Category("CRM")]
        [Description("Set this run-time to populate the grid with CRM data.")]
        public EntityCollection EntityCollection
        {
            get { return entityCollection; }
            set
            {
                entityCollection = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        [Category("CRM")]
        [Description("Specify if content shall be automatically refreshed when entitycollection, service, flags etc are changed.")]
        public bool AutoRefresh
        {
            get { return autoRefresh; }
            set
            {
                autoRefresh = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("True to show friendly names, False to show logical names and guid etc.")]
        public bool ShowFriendlyNames
        {
            get { return showFriendlyNames; }
            set
            {
                showFriendlyNames = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Set this to show the id of each record first in the grid.")]
        public bool ShowIdColumn
        {
            get { return showIdColumn; }
            set
            {
                showIdColumn = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Set this to display a counter column first in the grid.")]
        public bool ShowIndexColumn
        {
            get { return showIndexColumn; }
            set
            {
                showIndexColumn = value;
                if (autoRefresh)
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Refresh the contents of the gridview based on associated Entities and flags
        /// </summary>
        public override void Refresh()
        {
            Columns.Clear();
            DataSource = null;
            if (entityCollection != null)
            {
                var cols = GetTableColumns(entityCollection);
                var data = GetDataTable(entityCollection, cols);
                BindData(data);
            }
            base.Refresh();
        }

        [Category("CRM")]
        public event CRMRecordEventHandler RecordClick;

        [Category("CRM")]
        public event CRMRecordEventHandler RecordDoubleClick;

        private void HandleRecordClick(object sender, DataGridViewCellEventArgs e)
        {
            Entity entity = GetRecordFromCellEvent(e);
            OnRecordClick(new CRMRecordEventArgs(entity));
        }

        private void HandleRecordDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Entity entity = GetRecordFromCellEvent(e);
            OnRecordDoubleClick(new CRMRecordEventArgs(entity));
        }

        protected virtual void OnRecordClick(CRMRecordEventArgs e)
        {
            var handler = RecordClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRecordDoubleClick(CRMRecordEventArgs e)
        {
            var handler = RecordDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Entity GetRecordFromCellEvent(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return null;
            }
            var rowno = e.RowIndex;
            var row = Rows[rowno];
            var entity = row.Cells["#entity"].Value as Entity;
            return entity;
        }

        private List<DataColumn> GetTableColumns(EntityCollection entities)
        {
            var columns = new List<DataColumn>();
            if (showIndexColumn)
            {
                columns.Add(new DataColumn("#no", typeof(int)) { Caption = "#", AutoIncrement = true, AutoIncrementSeed = 1 });
            }
            columns.Add(new DataColumn("#id", typeof(Guid)) { Caption = "Id" });
            var addedColumns = new List<string>();
            foreach (var entity in entities.Entities)
            {
                foreach (var attribute in entity.Attributes.Keys)
                {
                    if (entity[attribute] == null)
                    {
                        continue;
                    }
                    if (entity[attribute] is Guid && (Guid)entity[attribute] == entity.Id)
                    {
                        continue;
                    }
                    if (addedColumns.Contains(attribute))
                    {
                        continue;
                    }

                    var meta = MetadataHelper.GetAttribute(organizationService, entities.EntityName, attribute);
                    var value = EntitySerializer.AttributeToBaseType(entity[attribute]);
                    var type = showFriendlyNames ? typeof(string) : value.GetType();
                    var dataColumn = new DataColumn(attribute, type);
                    dataColumn.ColumnName = attribute;
                    dataColumn.Caption =
                        showFriendlyNames &&
                        meta != null &&
                        meta.DisplayName != null &&
                        meta.DisplayName.UserLocalizedLabel != null ? meta.DisplayName.UserLocalizedLabel.Label : attribute;
                    dataColumn.ExtendedProperties.Add("Metadata", meta);
                    dataColumn.ExtendedProperties.Add("OriginalType", value.GetType());
                    columns.Add(dataColumn);
                    addedColumns.Add(attribute);
                }
            }
            columns.Add(new DataColumn("#entity", typeof(Entity)));
            return columns;
        }

        private DataTable GetDataTable(EntityCollection entities, List<DataColumn> columns)
        {
            var dTable = new DataTable();
            dTable.Columns.AddRange(columns.ToArray());
            foreach (var entity in entities.Entities)
            {
                var dRow = dTable.NewRow();
                foreach (DataColumn column in dTable.Columns)
                {
                    var col = column.ColumnName;
                    try
                    {
                        object value = null;
                        if (col == "#no")
                        {   // Sequence column
                            continue;
                        }
                        else if (col == "#id")
                        {
                            value = entity.Id;
                        }
                        else if (col == "#entity")
                        {
                            value = entity;
                        }
                        else if (entity.Contains(col) && entity[col] != null)
                        {
                            value = entity[col];
                            if (showFriendlyNames)
                            {
                                if (column.ExtendedProperties.ContainsKey("Metadata"))
                                {
                                    value = EntitySerializer.AttributeToString(value, column.ExtendedProperties["Metadata"] as AttributeMetadata);
                                }
                                else
                                {
                                    value = EntitySerializer.AttributeToBaseType(value).ToString();
                                }
                            }
                            else
                            {
                                value = EntitySerializer.AttributeToBaseType(value);
                            }
                        }
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        dRow[column] = value;
                    }
                    catch
                    {
                        MessageBox.Show("Attribute " + col + " failed, value: " + entity[col].ToString());
                    }
                }
                dTable.Rows.Add(dRow);
            }
            return dTable;
        }

        private void BindData(DataTable dTable)
        {
            SuspendLayout();
            DataSource = dTable;
            foreach (DataGridViewColumn col in Columns)
            {
                var datacolumn = dTable.Columns[col.Name];
                col.HeaderText = datacolumn.Caption;
                var type = datacolumn.DataType;
                if (datacolumn.ExtendedProperties.ContainsKey("OriginalType"))
                {
                    type = datacolumn.ExtendedProperties["OriginalType"] as Type;
                }
                if (type == typeof(int) || type == typeof(decimal))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (datacolumn.ColumnName == "#id" && !showIdColumn)
                {
                    col.Visible = false;
                }
                if (datacolumn.ColumnName == "#entity")
                {
                    col.Visible = false;
                }
            }
            AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
            ResumeLayout();
        }
    }

    public delegate void CRMRecordEventHandler(object sender, CRMRecordEventArgs e);
}
