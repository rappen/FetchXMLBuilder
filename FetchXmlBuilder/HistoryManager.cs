using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public class HistoryManager
    {
        private List<Tuple<object, string>> editHistory = new List<Tuple<object, string>>();
        private int historyIndex = 0;

        public void RecordHistory(string action, object data)
        {
            if (historyIndex > 0)
            {
                // New history to be recorded, so if we had undone anything, all redo possibilities must be removed.
                while (historyIndex > 0)
                {
                    historyIndex--;
                    editHistory.RemoveAt(0);
                }
            }
            editHistory.Insert(0, new Tuple<object, string>(data, action));
        }

        public object RestoreHistoryPosition(int positionDelta)
        {
            if (historyIndex + positionDelta < 0)
            {
                return null;
            }
            if (historyIndex + positionDelta >= editHistory.Count)
            {
                return null;
            }

            historyIndex += positionDelta;
            return editHistory[historyIndex].Item1;
        }

        internal void SetupUndoButton(ToolStripItem tsbUndo)
        {
            tsbUndo.Enabled = historyIndex < editHistory.Count - 1;
            if (tsbUndo.Enabled)
            {
                var undoitem = editHistory[historyIndex];
                tsbUndo.ToolTipText = "Undo (Ctrl+Z)\n\n" + undoitem.Item2;
            }
            else
            {
                tsbUndo.ToolTipText = "Nothing to undo (Ctrl+Z)";
            }
        }

        internal void SetupRedoButton(ToolStripItem tsbRedo)
        {
            tsbRedo.Enabled = historyIndex > 0;
            if (tsbRedo.Enabled)
            {
                var redoitem = editHistory[historyIndex - 1];
                tsbRedo.ToolTipText = "Redo (Ctrl+Y)\n\n" + redoitem.Item2;
            }
            else
            {
                tsbRedo.ToolTipText = "Nothing to redo (Ctrl+Y)";
            }
        }
    }
}
