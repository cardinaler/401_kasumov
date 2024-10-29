using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using GenAlgorithm_Kasumov;
using System.Windows.Automation.Peers;
using System.Data;
using System.Windows.Media;

namespace ViewModel
{
    public class ViewData: ViewModelBase, IDataErrorInfo
    {
        bool KeepRunning;

        bool StillRunning;

        public List<List<int>> distance { get; set; }
        public List<string> BestScoreList { get; set; }

        public List<List<double>> ChartBestScoreList { get; set; }
        public int IndividNums { get; set; }         //Число особей в начальной популяции
        public double CrossingShare { get; set; }    //Доля скрещиваний
        public double TurnamentsShare { get; set; }  //Доля турниров в селекции
        public double MutationShare { get; set; }    //Доля мутаций
        bool debug = true;

        public int MatrixSize { get; set; }

        public GenAlg? Alg { get; set; }

        public ICommand RunCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public ICommand GenerateMatrixCommand { get; set; }



        public CartesianChart ChartModelSpline { get; set; } // Требуемый график
        public DataTable MatrixDt { get; set; }

        public DataTable BestIndiDt { get; set; }

        public ViewData()
        {
            MatrixSize = 0;

            RunCommand = new RelayCommand(ExecuteHandler, CanExecuteHandler);

            StopCommand = new RelayCommand(StopHandler);

            GenerateMatrixCommand = new RelayCommand(GenerateMatrixHandler, CanGenerateMatrix);
            KeepRunning = false;
            StillRunning = false;
            Alg = null;
            this.ChartModelSpline = new CartesianChart();
            BestScoreList = new List<string>();
            ChartBestScoreList = new List<List<double>>();
            distance = new List<List<int>>();
            MatrixDt = new DataTable();
            BestIndiDt = new DataTable();
            
            /*
            distance = new List<List<int>>
            {
                            new List<int> {  0, 34,  2,  6,  1, 32,  5,  7,  1, 11       },
                            new List<int> {  34, 0,  7,  8,  4, 12, 23, 45, 12, 24    },
                            new List<int> {  2,  7,  0, 11, 45, 23,  1,  2,  3, 13      },
                            new List<int> {  6,  8,  11, 0, 12, 13, 14, 15, 16, 17   },
                            new List<int> {  1,  4,  45,12,  0, 18, 19, 20, 21, 22   },
                            new List<int> { 32,12,  23,13, 17,  0, 23, 24, 25, 26 },
                            new List<int> {  5,23,   1,14, 18, 23,  0, 28, 29, 30   },
                            new List<int> {  7, 45, 2, 15, 20, 24, 28, 0, 33, 34   },
                            new List<int> {  1, 12, 3, 16, 21, 25, 29, 33, 0, 40   },
                            new List<int> { 11, 24, 13, 17, 22, 26, 30, 34, 40, 0 }
             };
            */

            //distance = new List<List<int>>
            //{
            //    new List<int> {1, 2},
            //    new List<int> {3, 4}
            //};
        }

        

        public bool CanGenerateMatrix(object sender)
        {
            List<string> vars = ["MatrixSize"];
            for (int i = 0; i < vars.Count(); ++i)
            {
                if (this[vars[i]] != "")
                {
                    return false;
                }
            }
            return true;
        }

        public void GenerateMatrixHandler(object sender)
        {
            InitDistance(MatrixSize);
            InitDataTable(MatrixSize);
            RaisePropertyChanged(nameof(MatrixDt));
        }

        public void InitDistance(int N)
        {
            int L = 10;
            int R = 100;
            Random rnd = new Random();
            List<int> tmp = new List<int>();
            distance = new List<List<int>>();
            for(int i = 0; i < N; ++i)
            {
                List<int> ints = new List<int>();
                for(int j = 0; j < N; ++j)
                {
                    ints.Add(0);
                }
                distance.Add(ints);
            }
            for(int i = 0; i < N; ++i)
            {
                tmp.Add(R);
            }
            for(int i = 0; i < N; ++i)
            {
                for(int j = i + 1; j < N; ++j)
                {
                    if(i == 0)
                    {
                        distance[i][j] = rnd.Next(L, R);
                    }
                    else
                    {
                        distance[i][j] = rnd.Next(L, tmp[i] + tmp[j]);
                    }
                }
                distance[i][i] = 0;
                for(int j = 0; j < N; ++j)
                {
                    distance[j][i] = distance[i][j];
                }

                for (int j = i + 1; j < N; ++j)
                {
                    tmp[j] = Math.Min(tmp[j], distance[i][j]);
                }
            }
        }

        
        public IEnumerable<string>? Listbox_BestScoreList
        {
            get
            {
                if (BestScoreList.Count == 0)
                {
                    return null;
                }
                else
                {
                    return BestScoreList;
                }
            }
        }

        public string this[string ColumnName]
        {
            get
            {
                string error = string.Empty;
                switch (ColumnName)
                {
                    case "StillRunning":
                        if(StillRunning == true)
                        {
                            error += "Дождитесь завершения алгоритма\n";
                        }
                        break;
                    case "KeepRunning":
                        if(KeepRunning == true)
                        {
                            error += "Дождитесь завершения алгоритма\n";
                        }
                        break;
                    case "CrossingShare":
                        if (this.CrossingShare < 0 || this.CrossingShare > 1)
                        {
                            error += "Доля скрещиваний должна лежать в отрезке [0, 1]\n";
                        }
                        break;

                    case "TurnamentsShare":
                        if (this.TurnamentsShare < 0 || this.TurnamentsShare > 1)
                        {
                            error += "Доля турниров должна лежать в отрезке [0, 1]\n";
                        }
                        break;

                    case "MutationShare":
                        if (this.MutationShare < 0 || this.MutationShare > 1)
                        {
                            error += "Доля мутаций должна лежать в отрезке [0, 1]\n";
                        }
                        break;

                    case "IndividNums":
                        if (this.IndividNums < 1)
                        {
                            error += "Число особей должно быть положительно.\n";
                        }
                        break;

                    case "MatrixSize":
                        if(MatrixSize <= 0)
                        {
                            error += "Размер матрицы не задан\n";
                        }
                        break;

                    case "distance":
                        if(distance.Count == 0)
                        {
                            error += "Матрица еще не задана\n";
                        }
                        break;

                }
                return error;
            }
        }

        public bool CanExecuteHandler(object sender)
        {
            List<string> vars = ["distance", "StillRunning", "CrossingShare", "TurnamentsShare", "MutationShare", "IndividNums", "KeepRunning"];
            for (int i = 0; i < vars.Count(); ++i)
            {
                if (this[vars[i]] != "")
                {
                    return false;
                }
            }
            return true;
        }

        public void Execute()
        {
            Alg = new GenAlg(distance, IndividNums, TurnamentsShare, CrossingShare, MutationShare);
            double prev = Double.MaxValue;
            int cnt = 0;
            BestScoreList = new List<string>();
            ChartBestScoreList = new List<List<double>>();
            while (this.KeepRunning && Alg.population.Count > 2)
            {
                cnt++;
                Alg.LifeCycle();
                if (Alg.best_score < prev)
                {
                    BestScoreList = new List<string>(BestScoreList);
                    BestScoreList.Add(Alg.best_score.ToString("f5") + "    " + cnt.ToString());
                    ChartBestScoreList.Add(new List<double> {cnt, Alg.best_score});
                    RaisePropertyChanged(nameof(Listbox_BestScoreList));
                    
                    prev = Alg.best_score;
                }
            }
            if(Alg.population.Count <= 2)
            {
                MessageBox.Show("Популяция вымерла");
                KeepRunning = false;
            }
            InitChartBestScore();
            InitBestScoreDataTable();
            StillRunning = false;
        }

        public void ExecuteHandler(object sender)
        {
            Thread thread = new Thread(Execute);
            KeepRunning = true;
            StillRunning = true;
            thread.IsBackground = true;
            thread.Start();
            
        }

        public void StopHandler(object sender)
        {
            KeepRunning = false;
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        public void InitDataTable(int N)
        {
            MatrixDt = new DataTable();
            MatrixDt.Columns.Add("-");
            for (int i = 0; i < N; i++)
            {
                MatrixDt.Columns.Add(i.ToString(), typeof(double));
            }
            for (int i = 0; i < N; i++)
            {
                DataRow dr = MatrixDt.NewRow();
                dr[0] = i;
                for (int j = 1; j < N + 1; j++)
                {
                    dr[j] = distance[i][j - 1];
                }
                MatrixDt.Rows.Add(dr);
            }

        }
        public void InitBestScoreDataTable()
        {
            BestIndiDt = new DataTable();
            for(int i = 0; i < Alg.best_indi.Count + 1; ++i)
            {
                BestIndiDt.Columns.Add(i.ToString());
            }
            BestIndiDt.Columns.Add("Score");

            DataRow dr = BestIndiDt.NewRow();
            dr[0] = 0;
            dr[Alg.best_indi.Count + 1] = 0;
            int cnt = 0;
            for (int i = 1; i <  Alg.best_indi.Count; ++i)
            {
                dr[i] = distance[Alg.best_indi[i]][Alg.best_indi[i - 1]];
                cnt += distance[Alg.best_indi[i]][Alg.best_indi[i - 1]];
            }
            dr[Alg.best_indi.Count] = distance[Alg.best_indi.Last()][Alg.best_indi[0]];
            cnt += distance[Alg.best_indi.Last()][Alg.best_indi[0]];
            dr[Alg.best_indi.Count + 1] = cnt;
            BestIndiDt.Rows.Add(dr);


            dr = BestIndiDt.NewRow();
            for(int i = 0; i < Alg.best_indi.Count; ++i)
            {
                dr[i] = Alg.best_indi[i];
            }
            dr[Alg.best_indi.Count] = Alg.best_indi[0];
            
            BestIndiDt.Rows.Add(dr);
            RaisePropertyChanged(nameof(BestIndiDt));
            
        }


        public void InitChartBestScore()
        {
            var BestScoreValues = new ChartValues<Point>();
            int cnt = 0;
            for(int i = Math.Max(ChartBestScoreList.Count - 5, 0); i < ChartBestScoreList.Count ; ++i)
            {
                cnt++;
                var point = new Point() { X = ChartBestScoreList[i][0], Y = ChartBestScoreList[i][1]};
                BestScoreValues.Add(point);
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChartModelSpline.Series.Clear();

                this.ChartModelSpline.Series = new SeriesCollection
                {
                    new ScatterSeries
                    {
                        Configuration = new CartesianMapper<Point>()
                        .X(point => point.X) // Define a function that returns a value that should map to the x-axis
                        .Y(point => point.Y), // Define a function that returns a value that should map to the y-axis
                        Title = "Series",
                        Values = BestScoreValues,
                    },
                };
            });      
        }
    }
}
