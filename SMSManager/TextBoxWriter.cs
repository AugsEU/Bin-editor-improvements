using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSManager
{
    class TextBoxWriter : TextWriter
    {
        private TextBox textBox;
        public TextBoxWriter(TextBox textbox)
        {
            textBox = textbox;
        }

        public override void Write(char value)
        {
            MethodInvoker action = delegate {
                textBox.Text += value;
                textBox.Select(textBox.Text.Length - 1, 0);
                textBox.ScrollToCaret();
            };
            textBox.BeginInvoke(action);
        }

        public override void Write(string value)
        {
            MethodInvoker action = delegate
            {
                textBox.Text += value;
                textBox.Select(textBox.Text.Length - 1, 0);
                textBox.ScrollToCaret();
            };
            textBox.BeginInvoke(action);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}