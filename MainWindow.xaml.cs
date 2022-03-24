using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
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

using word = Microsoft.Office.Interop.Word;

namespace SaleWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string url;
        public List<SaleClass.Sale> sales = new List<SaleClass.Sale>();

        public ObservableCollection<List<SaleClass.Sale>> Students { get; set; }

        public MainWindow()
        {
            InitializeComponent();



        }

        public void ReturnTAble(string param1, string param2)
        {
            var cli = new WebClient();
            //cli.Headers[HttpRequestHeader.ContentType] = "application/json";
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("param1", param1);
            reqparm.Add("param2", param2);
            byte[] responsebytes = cli.UploadValues(url, "POST", reqparm);
            string responsebody = Encoding.UTF8.GetString(responsebytes);
            sales = JsonConvert.DeserializeObject<List<SaleClass.Sale>>(responsebody);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string datestr = Convert.ToDateTime($"{tb1.Text:O}").ToString();
                string dateend = Convert.ToDateTime($"{tb2.Text:O}").ToString();
                url = $"https://localhost:7100/api/Sale?dateStart={datestr}&dateEnd={datestr}";

                ReturnTAble(datestr, dateend);
                listSourse.ItemsSource = sales;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            var id = listSourse.SelectedItem as SaleClass.Sale;
            SaleClass.Sale adf = new SaleClass.Sale();

            foreach (var item in sales)
            {
                if (item.Client.LastN == id.Client.LastN)
                {
                    adf = item;
                }
            }


            var application = new word.Application();
            word.Document document = application.Documents.Add();

            word.Paragraph paragraph = document.Paragraphs.Add();
            word.Range tablerange = paragraph.Range;

            document.Paragraphs[1].Range.Text = "ТОВАРНЫЙ ЧЕК № ";
            document.Paragraphs[1].Range.Text = "Организация";
            document.Paragraphs[1].Range.Text = "_______";
            document.Paragraphs[1].Range.Text = "от ____ ___________ 20 __ г";
            document.Paragraphs[1].Range.Text = "Продавец _______________________ Адрес _______________________________ ОГРН _________________________________________";


            var perres = sales[adf.Telephones.Count].Telephones.ToList();
            int telcount = perres.Count();


            Object defaultTableBehavior =
                     word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior =
             word.WdAutoFitBehavior.wdAutoFitWindow;

            word.Table wordtable = document.Tables.Add(document.Paragraphs[1].Range, perres.Count() + 1, 6,
            ref defaultTableBehavior, ref autoFitBehavior);


            document.Tables[1].Cell(1, 1).Range.Text = "Наименование  товара";
            document.Tables[1].Cell(1, 2).Range.Text = "Артикул";
            document.Tables[1].Cell(1, 3).Range.Text = "Ед. изм";
            document.Tables[1].Cell(1, 4).Range.Text = "Кол-во";
            document.Tables[1].Cell(1, 5).Range.Text = "Цена";
            document.Tables[1].Cell(1, 6).Range.Text = "Сумма";



            for (int j = 2; j < perres.Count() + 2; j++)
            {
                for (int k = 1; k < 8; k++)
                {
                    if (k-1 == 0)
                    {
                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].NameTelephone.ToString();
                    }
                    else if (k-1 == 1)
                    {
                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].Articul.ToString();
                    }
                    else if (k-1 == 2)
                    {
                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].Category.ToString();
                    }
                    else if (k-1 == 3)
                    {
                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].Count.ToString();
                    }
                    else if (k-1 == 4)
                    {
                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].Cost.ToString();
                    }
                    else if (k-1 == 5)
                    {

                        document.Tables[1].Cell(j, k).Range.Text = perres[j - 2].Cost.ToString();
                    }
                }
            }




            application.Visible = true;
            document.SaveAs2(@"D:\otchetword.docx");


        }
    }
}
