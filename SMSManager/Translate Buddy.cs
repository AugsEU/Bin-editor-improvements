using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSManager
{
    public partial class Translate_Buddy : Form
    {
        const string TRANSLATE_URL = "https://translate.google.com/";
        public const string LANGUAGE_JAPANESE = "ja";
        public const string LANGUAGE_ENGLISH = "en";

        public Translate_Buddy()
        {
            InitializeComponent();
        }
        public Translate_Buddy(string src, string dst, string text)
        {
            InitializeComponent();
            Translate(src, dst, text);
        }
        private void Translate_Buddy_Load(object sender, EventArgs e)
        {
            this.FormClosing += Translate_Buddy_FormClosing;
        }
        private void Translate_Buddy_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        public void Translate(string src, string dst, string text)
        {
            if (!this.Visible)
                this.Show();
            if (!this.Focused)
                this.Focus();
            webBrowser1.Navigate(TRANSLATE_URL + "#" + src + "/" + dst + "/" + text);
        }
    }
}
