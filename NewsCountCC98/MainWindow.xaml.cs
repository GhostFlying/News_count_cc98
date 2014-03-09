using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;

namespace NewsCountCC98
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string cookiestr = null;
        
        List<string> idList=new List<string>();
        List<int> countList=new List<int>();
        public MainWindow()
        {
            InitializeComponent();
            QueryYear.Text = System.DateTime.Now.Year.ToString();
            QueryMonth.Text = (System.DateTime.Now.Month -1).ToString();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            
            
            
            //Uri requestUri = new Uri("http://www.cc98.org/list.asp?boardid=357");
            cookiestr=QueryCookie.Text;


 
            scanForPage();
            MessageBox.Show("统计已经完成，准备写入文件");
            writeResult();
            MessageBox.Show("请查看result.txt");
            //DebugInfo.Text = getIDNewsCount(QueryID.Text);
        }
        private void writeResult()
        {
            FileStream fs = File.Create("result.txt");
            int count = idList.Count();
            for (int i = 0; i < count; i++)
            {
                AddText(fs, idList.ElementAt(i)+"                   ");
                AddText(fs, countList.ElementAt(i) + "\r\n");
            }
            fs.Close();
            
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }


        private void scanForPage()
        {
            int i = 1;
            bool flag=false;
            string response = "";
            while (!flag)
            {
                i++;
                response = getWebContent(new Uri("http://www.cc98.org/list.asp?boardid=357&page=" + i.ToString()));
                flag = isPageNeeded(response);
                //process.Text="正在确认第"+i+"页";
            }
            
            while (flag){
                i++;
                //process.Text = ("正在统计第" + i + "页");
                
                perPageCount(response);                
                response = getWebContent(new Uri("http://www.cc98.org/list.asp?boardid=357&page="+i.ToString()));
                flag = isPageNeeded(response);
                //process.Text = ("正在确认第" + i + "页");
            }
        }

        private void perPageCount(string response)
        {
            
            int start = response.IndexOf("<!-- 显示作者 -->");
            int end = response.LastIndexOf("<!-- 最后回复时间和作者 -->");
            response = response.Substring(start, end - start);
            string id = "";
            for (int i = 0; i < 20; i++)
            {
                start = response.IndexOf("target=\"_blank\">");
                response = response.Substring(start + 16);
                end = response.IndexOf("</a>");
                id = response.Substring(0, end);
                if (idList.IndexOf(id) == -1)
                {
                    idList.Add(id);
                    countList.Add(getIDNewsCount(id));
                }
            }  

        }

        private Boolean isPageNeeded(string response)
        {
            Boolean result=false;

            if (response.IndexOf(">"+ QueryYear.Text + "/" + QueryMonth.Text + "/") != -1)
                result = true;

            return result;
        }

        private int getIDNewsCount(String id)
        {
            DateTime QueryTimeStart = new DateTime(int.Parse(QueryYear.Text), int.Parse(QueryMonth.Text), 1).AddDays(-1);
            DateTime QueryTimeEnd;
            if (QueryMonth.Text == "12")
            {
                QueryTimeEnd = new DateTime(int.Parse(QueryYear.Text) + 1, 1, 1);
            }
            else {
                QueryTimeEnd = new DateTime(int.Parse(QueryYear.Text), int.Parse(QueryMonth.Text) + 1, 1);
            }            
            QueryTimeEnd = QueryTimeEnd.AddDays(-1);
            DateTime NowTime = System.DateTime.Now;
            TimeSpan time_difference_start = NowTime.Subtract(QueryTimeStart);
            TimeSpan time_difference_end = NowTime.Subtract(QueryTimeEnd);

            return getNewsCount(id, time_difference_start.Days.ToString(), time_difference_end.Days.ToString());
        }
        private string getWebContent(Uri requestUri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Timeout = 30000;
            //CookieContainer mCookie = new CookieContainer();
            //mCookie.SetCookies(new Uri("http://www.cc98.org"),cookiestr );

            request.Headers.Add("Cookie", cookiestr);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //request.CookieContainer = mCookie;
            Stream streamReceive = response.GetResponseStream();

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            String strResult = streamReader.ReadToEnd();
            return strResult;
        }

        private int getNewsCount(String id, String start, String end)
        {
            int result;
            result = int.Parse(getSearchCount(id, start))-int.Parse(getSearchCount(id,end));
            return result;
        }

        private string getSearchCount(String id, String daynum)
        {
            String result = null;
            Uri requestUri = new Uri("http://www.cc98.org/queryresult.asp?page=1&stype=1&pSearch=&nSearch=&keyword=" + id + "&SearchDate=" + daynum + "&boardid=357&stable=bbs4&sertype=1");
            result = getWebContent(requestUri);
            if (result.IndexOf("没有找到您要查询的内容。") == -1)
            {
                int prefix = result.IndexOf("<font color=\"#FF0000\">");
                int suffix = result.IndexOf("</font>", prefix);
                result = result.Substring(prefix + 22, suffix - prefix - 22);
            }
            else
            {
                result = "0";
            }

            return result;
        }
    }
}
