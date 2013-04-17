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
            DebugInfo.Text="start";
            //Uri requestUri = new Uri("http://www.cc98.org/list.asp?boardid=357");
            cookiestr="ASPSESSIONIDCASBAQDA=DHLCBHKBHOOKALCJMJEJOLLF; ASPSESSIONIDCCTDDSBA=OOKLPCHCEFJOEOBEICKEPEEJ; aspsky=username=%E7%BA%AF%E8%89%AF&usercookies=3&userid=335099&useranony=&userhidden=2&password=5ba3a42ac9cb3dc5; upNum=0; cc98Simple=1; BoardList=BoardID=Show";
            
            ////DebugInfo.Text = getNewsCount("纯良","30","1");
            //DateTime QueryTimeStart = new DateTime(int.Parse(QueryYear.Text),int.Parse(QueryMonth.Text),1).AddDays(-1);
            //DateTime QueryTimeEnd = new DateTime(int.Parse(QueryYear.Text), int.Parse(QueryMonth.Text)+1, 1);
            //QueryTimeEnd = QueryTimeEnd.AddDays(-1);
            //DateTime NowTime=System.DateTime.Now;
            //TimeSpan time_difference_start=NowTime.Subtract(QueryTimeStart);
            //TimeSpan time_difference_end = NowTime.Subtract(QueryTimeEnd);

            


            DebugInfo.Text = getIDNewsCount(QueryID.Text);
        }

        private string getIDNewsCount(String id)
        {
            DateTime QueryTimeStart = new DateTime(int.Parse(QueryYear.Text), int.Parse(QueryMonth.Text), 1).AddDays(-1);
            DateTime QueryTimeEnd = new DateTime(int.Parse(QueryYear.Text), int.Parse(QueryMonth.Text) + 1, 1);
            QueryTimeEnd = QueryTimeEnd.AddDays(-1);
            DateTime NowTime = System.DateTime.Now;
            TimeSpan time_difference_start = NowTime.Subtract(QueryTimeStart);
            TimeSpan time_difference_end = NowTime.Subtract(QueryTimeEnd);

            return getNewsCount(QueryID.Text, time_difference_start.Days.ToString(), time_difference_end.Days.ToString());
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

        private string getNewsCount(String id, String start, String end)
        {
            String result =null;
            result = (int.Parse(getSearchCount(id, start))-int.Parse(getSearchCount(id,end))).ToString();
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
