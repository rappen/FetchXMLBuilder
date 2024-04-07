using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.XmlEditorUtils
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

        public static string ControlsChecksum(Control.ControlCollection controls)
        {
            if (controls?.Count == 0)
            {
                return string.Empty;
            }
            var result = string.Join("|", controls.OfType<Control>().OrderBy(c => c.TabIndex).Select(c => GetValueFromControl(c) + "|" + ControlsChecksum(c.Controls)));
            while (result.Contains("||"))
            {
                result = result.Replace("||", "|");
            }
            return result;
        }

        public static string GetValueFromControl(Control control)
        {
            var result = "";
            if (control is CheckBox cb)
            {
                result = cb.Checked ? "true" : "false";
            }
            else if (control is TextBox tx)
            {
                result = tx.Text;
            }
            else if (control is ComboBox cb2)
            {
                var item = cb2.SelectedItem;
                if (item is IComboBoxItem ci)
                {
                    result = ci.GetValue();
                }
                else
                {
                    result = cb2.Text;
                }
            }
            return result;
        }

        public static Dictionary<string, string> GetAttributesCollection(Control.ControlCollection controls, bool validate = false)
        {
            if (controls?.Count == 0)
            {
                return null;
            }
            Dictionary<string, string> collection = new Dictionary<string, string>();

            foreach (Control control in controls.OfType<Control>().OrderBy(y => y.TabIndex))
            {
                if (control.Tag != null)
                {
                    string attribute;
                    bool required;
                    string defaultvalue;
                    if (GetControlDefinition(control, out attribute, out required, out defaultvalue))
                    {
                        var value = GetValueFromControl(control);
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
                if (GetAttributesCollection(control.Controls, validate) is Dictionary<string, string> children)
                {
                    foreach (var child in children)
                    {
                        collection.Add(child.Key, child.Value);
                    }
                }
            }
            return collection;
        }

        public static void FillControls(Dictionary<string, string> collection, Control.ControlCollection controls, IDefinitionSavable saveable)
        {
            if (controls?.Count == 0)
            {
                return;
            }
            controls.OfType<Control>().Where(y => y.Tag != null).OrderBy(y => y.TabIndex).ToList().ForEach(c => FillControl(collection, c, saveable));
            controls.OfType<Panel>().OrderBy(p => p.TabIndex).ToList().ForEach(p => FillControls(collection, p.Controls, saveable));
        }

        private class TextBoxEventHandler
        {
            private readonly TextBox txt;
            private readonly IDefinitionSavable saveable;
            private bool attachedValidated;

            public TextBoxEventHandler(TextBox txt, IDefinitionSavable saveable)
            {
                this.txt = txt;
                this.saveable = saveable;
            }

            public void Attach()
            {
                txt.TextChanged += OnTextChanged;
            }

            private void OnTextChanged(object sender, EventArgs e)
            {
                saveable.Save(true);

                if (!attachedValidated)
                {
                    txt.Validated += OnValidated;
                    attachedValidated = true;
                }
            }

            private void OnValidated(object sender, EventArgs e)
            {
                saveable.Save(false);
                txt.Validated -= OnValidated;
                attachedValidated = false;
            }
        }

        private class ComboBoxEventHandler
        {
            private readonly ComboBox cmb;
            private readonly IDefinitionSavable saveable;
            private bool attachedValidated;

            public ComboBoxEventHandler(ComboBox cmb, IDefinitionSavable saveable)
            {
                this.cmb = cmb;
                this.saveable = saveable;
            }

            public void Attach()
            {
                cmb.TextChanged += OnTextChanged;
                cmb.SelectedIndexChanged += OnSelectedIndexChanged;
            }

            private void OnSelectedIndexChanged(object sender, EventArgs e)
            {
                saveable.Save(false);
                cmb.Validated -= OnValidated;
                attachedValidated = false;
            }

            private void OnTextChanged(object sender, EventArgs e)
            {
                if (cmb.SelectedIndex != -1)
                {
                    return;
                }

                saveable.Save(true);

                if (!attachedValidated)
                {
                    cmb.Validated += OnValidated;
                    attachedValidated = true;
                }
            }

            private void OnValidated(object sender, EventArgs e)
            {
                saveable.Save(false);
                cmb.Validated -= OnValidated;
                attachedValidated = false;
            }
        }

        public static void FillControl(Dictionary<string, string> collection, Control control, IDefinitionSavable saveable)
        {
            if (control.Tag != null && control.Tag.ToString() != "uiname" && GetControlDefinition(control, out string attribute, out bool required, out string defaultvalue))
            {
                if (!collection.TryGetValue(attribute, out string value))
                {
                    value = defaultvalue;
                }
                if (control is CheckBox chkbox)
                {
                    bool.TryParse(value, out bool chk);
                    chkbox.Checked = chk;
                    if (saveable != null)
                    {
                        chkbox.CheckedChanged += (s, e) => saveable.Save(false);
                    }
                }
                else if (control is TextBox txt)
                {
                    txt.Text = value;
                    if (saveable != null)
                    {
                        new TextBoxEventHandler(txt, saveable).Attach();
                    }
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
                    else if (value != null && cmb.Items.IndexOf(value) >= 0)
                    {
                        cmb.SelectedItem = value;
                    }
                    else
                    {
                        cmb.Text = value;
                    }
                    if (saveable != null)
                    {
                        new ComboBoxEventHandler(cmb, saveable).Attach();
                    }
                }
            }
        }
    }
}