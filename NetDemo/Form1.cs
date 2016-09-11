using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace NetDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DateTime time = DateTime.Now;
        private DateTime now = DateTime.Now;
        private TimeSpan ts = new TimeSpan(0, 0, 0);

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            string text = "";
            textBox1.Text = "";
            
            wc.Encoding = Encoding.GetEncoding("utf-8");

            try
            {
                text = wc.DownloadString(urltextBox.Text);
            }
            catch(WebException exc)
            {
                MessageBox.Show(exc.Message);
            }

            if(text != "")
            {
                var Doc = new HtmlAgilityPack.HtmlDocument();
                Doc.LoadHtml(text);

                //var nodes = Doc.DocumentNode.SelectNodes("//table[@class='table table-default table-striped table-hover table-condensed']");
                var nodes = Doc.DocumentNode.SelectNodes("//div[2]/table[@class='table table-default table-striped table-hover table-condensed']/tbody/tr/td/small/a")
                                        .Select(a => new
                                        {
                                            Url = a.Attributes["href"].Value.Trim(),
                                            Title = a.InnerText.Trim(),
                                        });

                int i = 1;
                foreach (var node in nodes.Take(nodes.Count()))
                {
                    textBox1.Text += node.Title + "\r\n";
                    if (i % 3 == 1)
                    {
                        time = DateTime.Parse(textBox1.Lines[i - 1]);
                        ts = time - now;
                    }
                    if(i % 2 == 0)
                        textBox1.Text += node.Url + "\r\n";
                    i++;
                }

                try
                {
                    StreamReader sr = new StreamReader("contests.txt", Encoding.GetEncoding("utf-8"));
                    string contests = sr.ReadToEnd();
                    sr.Close();
                    if (textBox1.Text != contests)
                    {
                        //ここでbotの出番
                        MessageBox.Show("新しいコンテストです\r\n" + textBox1.Text);
                        StreamWriter sw = new StreamWriter("contests.txt");
                        sw.Write(textBox1.Text);
                        sw.Close();
                        MessageBox.Show("書き込みが完了しました");
                    }
                    else MessageBox.Show("新しいコンテストはありません");

                    timer1.Start();
                    timer1.Interval = 1000;
                }
                catch(FileNotFoundException)
                {
                    MessageBox.Show("contests.txtが見つからなかったので作成します");
                    StreamWriter sw = new StreamWriter("contests.txt");
                    sw.Write(textBox1.Text);
                    sw.Close();
                    MessageBox.Show("作成しました");
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ts.TotalSeconds > 0)
            {
                timeBox.Text = "残り" + ts.Days + "日" + ts.Hours + "時間" + ts.Minutes + "分" + ts.Seconds + "秒\r\n";
                timeBox.Refresh();
                now = DateTime.Now;
                ts = time - now;
            }
            else
                timeBox.Text = "既に始まっているか、終了しています";
        }
    }
    
}
