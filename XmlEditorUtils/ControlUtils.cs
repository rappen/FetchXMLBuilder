using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.XmlEditorUtils
{
    public class ControlUtils
    {
        public static bool GetControlDefinition(Control control, out string attribute, out bool required, out string defaultvalue)
        {
            var tags = control.Tag != null ? control.Tag.ToString().Split('|') : new string[] { };
            attribute = tags.Length > 0 ? tags[0] : "";
            required = tags.Length > 1 ? bool.Parse(tags[1]) : false;
            defaultvalue = tags.Length > 2 ? tags[2] : control is CheckBox ? "false" : "";
            return !string.IsNullOrWhiteSpace(attribute);
        }

        public static string ControlsChecksum(System.Windows.Forms.Control.ControlCollection controls)
        {
            var checksum = "";
            foreach (Control control in controls)
            {
                if (control.Tag == null) { continue; }
                checksum += GetValueFromControl(control) + "|";
            }
            return checksum;
        }

        public static string GetValueFromControl(Control control)
        {
            var result = "";
            if (control is CheckBox)
            {
                result = ((CheckBox)control).Checked ? "true" : "false";
            }
            else if (control is TextBox)
            {
                result = ((TextBox)control).Text;
            }
            else if (control is ComboBox)
            {
                var item = ((ComboBox)control).SelectedItem;
                if (item is IComboBoxItem)
                {
                    result = ((IComboBoxItem)item).GetValue();
                }
                else
                {
                    result = ((ComboBox)control).Text;
                }
            }
            return result;
        }

        public static Dictionary<string, string> GetAttributesCollection(System.Windows.Forms.Control.ControlCollection controls, bool validate = false)
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();

            foreach (Control control in controls.Cast<Control>().Where(y => y.Tag != null).OrderBy(y => y.TabIndex))
            {
                string attribute;
                bool required;
                string defaultvalue;
                if (ControlUtils.GetControlDefinition(control, out attribute, out required, out defaultvalue))
                {
                    var value = ControlUtils.GetValueFromControl(control);
                    if (validate && required && string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException(attribute, "Field cannot be empty");
                    }
                    if (required || value != defaultvalue)
                    {
                        collection.Add(attribute, value);
                    }
                }
            }
            return collection;
        }

        public static void FillControls(Dictionary<string, string> collection, System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (Control control in controls.Cast<Control>().Where(y => y.Tag != null).OrderBy(y => y.TabIndex))
            {
                FillControl(collection, control);
            }
        }

        public static void FillControl(Dictionary<string, string> collection, Control control)
        {
            string attribute;
            bool required;
            string defaultvalue;
            if (ControlUtils.GetControlDefinition(control, out attribute, out required, out defaultvalue))
            {
                var value = collection.ContainsKey(attribute) ? collection[attribute] : defaultvalue;
                if (control is CheckBox)
                {
                    bool.TryParse(value, out bool chk);
                    ((CheckBox)control).Checked = chk;
                }
                else if (control is TextBox)
                {
                    ((TextBox)control).Text = value;
                }
                else if (control is ComboBox cmb)
                {
                    object selitem = null;
                    foreach (var item in cmb.Items)
                    {
                        if (item is IComboBoxItem)
                        {
                            if (((IComboBoxItem)item).GetValue() == value)
                            {
                                selitem = item;
                                break;
                            }
                        }
                    }
                    if (selitem != null)
                    {
                        cmb.SelectedItem = selitem;
                    }
                    else if (cmb.Items.IndexOf(value) >= 0)
                    {
                        cmb.SelectedItem = value;
                    }
                    else
                    {
                        cmb.Text = value;
                    }
                }
            }
        }
    }
}
