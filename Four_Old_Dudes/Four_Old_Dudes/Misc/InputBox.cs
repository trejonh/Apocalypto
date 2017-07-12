/*
 * Author: Tony Brix, http://tonybrix.info
 * License: MIT
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Four_Old_Dudes.Misc
{
    public enum InputBoxButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        Save,
        SaveCancel
    }

    public enum InputBoxResult
    {
        Cancel,
        Ok,
        Yes,
        No,
        Save
    }

    public struct InputBoxItem
    {
        public string Label;
        public string Text;
        public bool IsPassword;

        public InputBoxItem(string label)
        {
            Label = label;
            Text = "";
            IsPassword = false;
        }

        public InputBoxItem(string label, string text)
        {
            Label = label;
            Text = text;
            IsPassword = false;
        }

        public InputBoxItem(string label, bool isPassword)
        {
            Label = label;
            Text = "";
            IsPassword = isPassword;
        }

        public InputBoxItem(string label, string text, bool isPassword)
        {
            Label = label;
            Text = text;
            IsPassword = isPassword;
        }
    }

    //public struct InputBoxOptions
    //{
    //    public bool ShowCloseButton = true;
    //}

    public class InputBox
    {
        private InputBox(DialogForm dialog)
        {
            Result = dialog.InputResult;
            Items = new Dictionary<string, string>();
            for (var i = 0; i < dialog.Label.Length; i++)
                Items.Add(dialog.Label[i].Text, dialog.TextBox[i].Text);
        }

        private static DialogForm _dialog;

        public Dictionary<string, string> Items { get; }

        public InputBoxResult Result { get; }

        public static String Res;

        public static InputBox Show(string title, string label)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label)}, InputBoxButtons.Ok);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, string label, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label)}, buttons);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, string label, string text)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label, text)}, InputBoxButtons.Ok);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, string label, string text, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label, text)}, buttons);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, string[] labels)
        {
            var items = new InputBoxItem[labels.Length];
            for (var i = 0; i < labels.Length; i++)
                items[i] = new InputBoxItem(labels[i]);

            _dialog = new DialogForm(title, items, InputBoxButtons.Ok);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, string[] labels, InputBoxButtons buttons)
        {
            var items = new InputBoxItem[labels.Length];
            for (var i = 0; i < labels.Length; i++)
                items[i] = new InputBoxItem(labels[i]);

            _dialog = new DialogForm(title, items, buttons);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, InputBoxItem item)
        {
            _dialog = new DialogForm(title, new[] {item}, InputBoxButtons.Ok);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, InputBoxItem item, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {item}, buttons);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, InputBoxItem[] items)
        {
            _dialog = new DialogForm(title, items, InputBoxButtons.Ok);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(string title, InputBoxItem[] items, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, items, buttons);
            _dialog.ShowDialog();
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string label)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label)}, InputBoxButtons.Ok);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string label, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label)}, buttons);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string label, string text)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label, text)},
                InputBoxButtons.Ok);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string label, string text,
            InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {new InputBoxItem(label, text)}, buttons);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string[] labels)
        {
            var items = new InputBoxItem[labels.Length];
            for (var i = 0; i < labels.Length; i++)
                items[i] = new InputBoxItem(labels[i]);

            _dialog = new DialogForm(title, items, InputBoxButtons.Ok);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, string[] labels, InputBoxButtons buttons)
        {
            var items = new InputBoxItem[labels.Length];
            for (var i = 0; i < labels.Length; i++)
                items[i] = new InputBoxItem(labels[i]);

            _dialog = new DialogForm(title, items, buttons);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, InputBoxItem item)
        {
            _dialog = new DialogForm(title, new[] {item}, InputBoxButtons.Ok);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, InputBoxItem item, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, new[] {item}, buttons);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, InputBoxItem[] items)
        {
            _dialog = new DialogForm(title, items, InputBoxButtons.Ok);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        public static InputBox Show(IWin32Window window, string title, InputBoxItem[] items, InputBoxButtons buttons)
        {
            _dialog = new DialogForm(title, items, buttons);
            _dialog.ShowDialog(window);
            return new InputBox(_dialog);
        }

        private class DialogForm : Form
        {
            public readonly Label[] Label;
            public readonly TextBox[] TextBox;

            public DialogForm(string title, IList<InputBoxItem> items, InputBoxButtons buttons)
            {
                var minWidth = 312;
                Label = new Label[items.Count];
                for (var i = 0; i < Label.Length; i++)
                    Label[i] = new Label();
                TextBox = new TextBox[items.Count];
                for (var i = 0; i < TextBox.Length; i++)
                    TextBox[i] = new TextBox();
                var button2 = new Button();
                var button3 = new Button();
                var button1 = new Button();
                SuspendLayout();
                // 
                // label
                // 
                for (var i = 0; i < items.Count; i++)
                {
                    Label[i].AutoSize = true;
                    Label[i].Location = new Point(12, 9 + i * 39);
                    Label[i].Name = "label[" + i + "]";
                    Label[i].Text = items[i].Label;
                    if (Label[i].Width > minWidth)
                        minWidth = Label[i].Width;
                }
                // 
                // textBox
                // 
                for (var i = 0; i < items.Count; i++)
                {
                    TextBox[i].Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    TextBox[i].Location = new Point(12, 25 + i * 39);
                    TextBox[i].Name = "textBox[" + i + "]";
                    TextBox[i].Size = new Size(288, 20);
                    TextBox[i].TabIndex = i;
                    TextBox[i].Text = items[i].Text;
                    if (items[i].IsPassword)
                        TextBox[i].UseSystemPasswordChar = true;
                }
                // 
                // button1
                // 
                button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                button1.Location = new Point(208, 15 + 39 * Label.Length);
                button1.Name = "button1";
                button1.Size = new Size(92, 23);
                button1.TabIndex = items.Count + 2;
                button1.Text = "button1";
                button1.UseVisualStyleBackColor = true;
                // 
                // button2
                // 
                button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                button2.Location = new Point(110, 15 + 39 * Label.Length);
                button2.Name = "button2";
                button2.Size = new Size(92, 23);
                button2.TabIndex = items.Count + 1;
                button2.Text = "button2";
                button2.UseVisualStyleBackColor = true;
                // 
                // button3
                // 
                button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                button3.Location = new Point(12, 15 + 39 * Label.Length);
                button3.Name = "button3";
                button3.Size = new Size(92, 23);
                button3.TabIndex = items.Count;
                button3.Text = "button3";
                button3.UseVisualStyleBackColor = true;
                //
                // Evaluate MessageBoxButtons
                //
                switch (buttons)
                {
                    case InputBoxButtons.Ok:
                        button1.Text = "OK";
                        button1.Click += OK_Click;
                        button2.Visible = false;
                        button3.Visible = false;
                        AcceptButton = button1;
                        break;
                    case InputBoxButtons.OkCancel:
                        button1.Text = "Cancel";
                        button1.Click += Cancel_Click;
                        button2.Text = "OK";
                        button2.Click += OK_Click;
                        button3.Visible = false;
                        AcceptButton = button2;
                        break;
                    case InputBoxButtons.YesNo:
                        button1.Text = "No";
                        button1.Click += No_Click;
                        button2.Text = "Yes";
                        button2.Click += Yes_Click;
                        button3.Visible = false;
                        AcceptButton = button2;
                        break;
                    case InputBoxButtons.YesNoCancel:
                        button1.Text = "Cancel";
                        button1.Click += Cancel_Click;
                        button2.Text = "No";
                        button2.Click += No_Click;
                        button3.Text = "Yes";
                        button3.Click += Yes_Click;
                        AcceptButton = button3;
                        break;
                    case InputBoxButtons.Save:
                        button1.Text = "Save";
                        button1.Click += Save_Click;
                        button2.Visible = false;
                        button3.Visible = false;
                        AcceptButton = button1;
                        break;
                    case InputBoxButtons.SaveCancel:
                        button1.Text = "Cancel";
                        button1.Click += Cancel_Click;
                        button2.Text = "Save";
                        button2.Click += Save_Click;
                        button3.Visible = false;
                        AcceptButton = button2;
                        break;
                    default:
                        throw new Exception("Invalid InputBoxButton Value");
                }
                // 
                // dialogForm
                // 
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                ClientSize = new Size(312, 47 + 39 * items.Count);
                foreach (var t in Label)
                    Controls.Add(t);
                foreach (var t in TextBox)
                    Controls.Add(t);
                Controls.Add(button1);
                Controls.Add(button2);
                Controls.Add(button3);
                MaximizeBox = false;
                MinimizeBox = false;
                MaximumSize = new Size(99999, 85 + 39 * items.Count);
                Name = "dialogForm";
                ShowIcon = false;
                ShowInTaskbar = false;
                Text = title;
                ResumeLayout(false);
                PerformLayout();
                minWidth = Label.Select(l => l.Width).Concat(new[] {minWidth}).Max();
                ClientSize = new Size(minWidth + 24, 47 + 39 * items.Count);
                MinimumSize = new Size(minWidth + 40, 85 + 39 * items.Count);
            }

            public sealed override Size MinimumSize
            {
                get => base.MinimumSize;
                set => base.MinimumSize = value;
            }

            public sealed override string Text
            {
                get => base.Text;
                set => base.Text = value;
            }

            public sealed override Size MaximumSize
            {
                get => base.MaximumSize;
                set => base.MaximumSize = value;
            }

            public InputBoxResult InputResult { get; private set; } = InputBoxResult.Cancel;

            private void OK_Click(object sender, EventArgs e)
            {
                InputResult = InputBoxResult.Ok;
                InputBox.Res = TextBox[0].Text;
                Close();
                Dispose();
            }

            private void Cancel_Click(object sender, EventArgs e)
            {
                InputResult = InputBoxResult.Cancel;
                Close();
                Dispose();
            }

            private void Yes_Click(object sender, EventArgs e)
            {
                InputResult = InputBoxResult.Yes;
                InputBox.Res = TextBox[0].Text;
                Close();
                Dispose();
            }

            private void No_Click(object sender, EventArgs e)
            {
                InputResult = InputBoxResult.No;
                InputBox.Res = TextBox[0].Text;
                Close();
                Dispose();
            }

            private void Save_Click(object sender, EventArgs e)
            {
                InputResult = InputBoxResult.Save;
                InputBox.Res = string.Copy(TextBox[0].Text);
                Close();
                Dispose();
                Hide();
            }
        }
    }
}