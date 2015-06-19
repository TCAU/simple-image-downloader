using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Downloader
{
    public partial class Form1 : Form
    {
        string saveDir;

        public Form1()
        {
            InitializeComponent();
            string exeDir = Application.ExecutablePath.ToString();
            string[] pieces = exeDir.Split('\\');
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < pieces.Count() - 1; i++)
            {
                stringBuilder.Append(pieces[i]);
                stringBuilder.Append("\\");
            }
            saveDir = stringBuilder.ToString() + "images\\";

            textBoxSaveDirectory.Text = saveDir;
        }

        public void Download(string url, WebClient wc)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            string html = wc.DownloadString(url);

            document.LoadHtml(html);

            foreach (var link in document.DocumentNode
                        .Descendants("img")
                        .Select(x => x.Attributes["src"]))
            {
                string urlPath = link.Value.ToString();
                string[] urlPathSegments = urlPath.Split('/');
                string lastSegment = urlPathSegments[urlPathSegments.Count() - 1];

                if (urlPath[urlPath.Length-4] == '.')
                {
                    if (urlPath.Contains("http"))
                    {
                        wc.DownloadFile(urlPath, String.Format("{0}{1}", saveDir, lastSegment));
                    }
                    else
                    {
                        wc.DownloadFile(String.Format("{0}{1}", url, urlPath),
                            String.Format("{0}{1}", saveDir, lastSegment));
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (WebClient wc = new WebClient())
                    wc.DownloadString(textBoxURL.Text);
                listView.Items.Add(textBoxURL.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid URL");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
                saveDir = folderBrowserDialog1.SelectedPath.ToString();
            if (saveDir.Substring(saveDir.Length - 1, 1) != "\\")
                saveDir += "\\";
            textBoxSaveDirectory.Text = saveDir;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (!System.IO.Directory.Exists(saveDir))
            {
                System.IO.Directory.CreateDirectory(saveDir);
            }

            using (WebClient wc = new WebClient())
            {
                foreach (ListViewItem item in listView.Items)
                {
                    Download(item.Text, wc);
                }
            }

            listView.Items.Clear();
        }
    }
}
