// PROJECT : MsCrmTools.ViewLayoutReplicator
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using System;
using System.Collections;
using System.Windows.Forms;

namespace Cinteros.Xrm.XmlEditorUtils
{
    /// <summary>
    /// Compares two listview items for sorting
    /// </summary>
    public class ListViewItemComparer : IComparer
    {
        #region Variables

        /// <summary>
        /// Index of sorting column
        /// </summary>
        private readonly int col;

        /// <summary>
        /// Sort order
        /// </summary>
        private readonly SortOrder innerOrder;

        #endregion Variables

        #region Constructors

        /// <summary>
        /// Initializes a new instance of class ListViewItemComparer
        /// </summary>
        public ListViewItemComparer()
        {
            col = 0;
            innerOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Initializes a new instance of class ListViewItemComparer
        /// </summary>
        /// <param name="column">Index of sorting column</param>
        /// <param name="order">Sort order</param>
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            innerOrder = order;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Compare tow objects
        /// </summary>
        /// <param name="x">object 1</param>
        /// <param name="y">object 2</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            return Compare((ListViewItem)x, (ListViewItem)y);
        }

        /// <summary>
        /// Compare tow listview items
        /// </summary>
        /// <param name="x">Listview item 1</param>
        /// <param name="y">Listview item 2</param>
        /// <returns></returns>
        public int Compare(ListViewItem x, ListViewItem y)
        {
            if (innerOrder == SortOrder.Ascending)
            {
                return Compare(x.SubItems[col].Text, y.SubItems[col].Text);
            }
            else
            {
                return Compare(y.SubItems[col].Text, x.SubItems[col].Text);
            }
        }

        public int Compare(string x, string y)
        {
            var xdec = 0M;
            var ydec = 0M;
            if (decimal.TryParse(x, out xdec) && decimal.TryParse(y, out ydec))
            {
                return xdec == ydec ? 0 : xdec > ydec ? 1 : -1;
            }
            return String.CompareOrdinal(x, y);
        }

        #endregion Methods
    }
}