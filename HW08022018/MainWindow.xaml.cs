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
using HW08022018.Model;

namespace HW08022018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static CRCMS_newEntities1 db = new CRCMS_newEntities1();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetData_Click(object sender, RoutedEventArgs e)
        {
           

            //------------------------



            int index = GetTaskas.SelectedIndex;

            switch(index) 
            {
                case 0: 
                { 
                #region Task1
                        //1.Подключиться к таблицам Document, вывести информацию по тем документам по которым завершена работа.
                        //Для того что бы понять завершены ли работы по наряду, необходимо, чтобы в таблице TimerArchive,
                        //были записи по данному наряду. (связь между таблицами можно посмотреть в сущностях загруженных таблиц)

                        var query1 = db.TimerArchive
                        .Where(w => w.DateFinish != null && w.DateStart != null)
                        .Select(s => new
                        {
                            s.Document,
                            s.DateStart,
                            s.DateFinish,
                            s.DurationInSeconds

                        }).ToList();
                        AddColumns(query1.ToArray());
                        DataListView.ItemsSource = query1;
                        // 1.end
                        #endregion
                } break;
                case 1:
                {
                        #region Task2

                        //2.Имея список завершенных нарядов(завершенные наряды хранятся в таблице TimerArchive), 
                        //необходимо посчитать количество полезного времени, проведенного пользователями в работе и бесполезного.
                        //Полезным временем считаться данные которые храниться в таблице Timer, 
                        //а бесполезное   время считаются данные которые лежат в таблице TimerInactivity.
                        //Время, затраченное по каждой записи в таблице TimerArchive можно рассчитать, 
                        //как DateFinish – DateStart.

                        DateTime start = DatePrikerStart.SelectedDate.Value;
                        DateTime end   = DatePrikerEnd.  SelectedDate.Value; 

                        var query2 = db.TimerArchive
                        .Where(w => w.DateFinish <= end && w.DateFinish >= start)
                         .Select(s => new
                            {
                                s.DateStart,
                                s.DateFinish,
                                s.DocumentId,
                                s.DurationInSeconds
                             });

                        var finishDoc = query2.Select(s => s.DocumentId).Distinct();
                        var doc = db.Document
                        .Where(w => finishDoc.Contains(w.DocumentId))
                        .Select(s => new
                        {
                            UseFullTime = query2
                            .Where(w1 => w1.DocumentId == s.DocumentId).Sum(sum => sum.DurationInSeconds),
                            s.DocumentNumber,
                            s.ModelId,
                            s.DocumentCreateDate,
                            s.SmcsCode   
                        })
                        .ToList();

                        AddColumns(query2.ToArray());
                        DataListView.ItemsSource = query2;
                        // 1.end
                        #endregion
                    }
                    break;

                case 2:
                    {

                     List<int> users = db.Timer
                                         .Where(w =>w .UserId != 0)
                                         .Select(s => (int)s.UserId)
                                         .Distinct()
                                         .ToList();

                        List<Timer> timers = new List<Timer>();

                        foreach (var item in users)
                        {
                            var sum = db.Timer
                                        .Where(w => w.UserId == item)
                                        .Sum(s => s.DurationInSeconds);

                            Timer t = new Timer();
                            t.UserId = item;
                            t.DurationInSeconds = sum;
                            timers.Add(t);
                        }
                        var result = timers.Select(s => new
                        {
                            s.UserId,
                            s.DurationInSeconds,  
                        }).ToArray();


                        AddColumns(result.ToArray());
                        DataListView.ItemsSource = result;


                    } break;
            }

         


        }
        private void AddColumns(object[] arr) 
        {
            DataGridView.Columns.Clear();
            foreach (var item in arr[0].GetType().GetProperties())
            {
                GridViewColumn column = new GridViewColumn()
                {
                    Header = item.Name,
                    DisplayMemberBinding = new Binding(item.Name)
                };
                DataGridView.Columns.Add(column);
            }
            
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
