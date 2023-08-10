using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using HourlySalesReport.Model;
using Microsoft.ML;

namespace HourlySalesReport.ViewModel
{
    public class HourlySalesReportViewModel
    {

        public HourlySalesReportViewModel()
        {
            //
            HSActualData = new List<HourlySalesReportModel>();
            HSProjectedData = new List<HourlySalesReportModel>();
            ComparisionDataWithML = new List<ComparisionDataWithML>();

            LoadData();

            MLActivities();
            //HourlySalesOnScreen hls = new HourlySalesOnScreen();
            //hls.ShowOnScreen();
        }
        List<CommonDataModel> commonDataList { get; set; }

        public List<ComparisionDataWithML> ComparisionDataWithML { get; set; }

        List<CommonDataModel> HourlySalesForBODAG { get; set; }

        public List<CommonDataModel> HourlySalesForBODPG 
        {
            get; set;
        }


        void GetActualData()
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.connString))
            {
                if (conn == null)
                {
                    //
                }
                else
                {
                    conn.Open();
                   
                    SqlCommand query = new SqlCommand("GetActualGuestsPerHour", conn);
                    query.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query);
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        try
                        {
                            HistoriceDateTime = DateTime.ParseExact((string)row["timedate"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            HistoricActualGuestCount = (int)row["TotalActualGuestsPerHour"];
                        }
                        catch (Exception)
                        {
                            //
                        }

                        HSActualData.Add(new HourlySalesReportModel
                        {
                           HistoriceDateTime = HistoriceDateTime,
                           HistoricActualGuestCount = HistoricActualGuestCount
                        });
                        
                    }
                    conn.Close();

                }
            }
        }


        void GetProjectedData()
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.connString))
            {
                if (conn == null)
                {
                    //
                }
                else
                {
                    conn.Open();

                    SqlCommand query = new SqlCommand("GetProjectedGuestsPerHour", conn);
                    query.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query);
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        try
                        {
                            HistoriceDateTime = (DateTime)row["CombinedDateTime"];
                            HistoriceProjectedGuestCount = (int)row["TotalProjectedGuestsPerHour"];
                        }
                        catch (Exception ex)
                        {
                            //
                        }

                        HSProjectedData.Add(new HourlySalesReportModel
                        {
                            HistoriceDateTime = HistoriceDateTime,
                            HistoriceProjectedGuestCount = HistoriceProjectedGuestCount
                        });

                    }
                    conn.Close();

                }
            }
        }

        public void LoadData()
        {
            GetActualData();
            GetProjectedData();
 
            var commonData = from HSActualData in HSActualData
                             join HSProjectedData in HSProjectedData  on HSActualData.HistoriceDateTime equals HSProjectedData.HistoriceDateTime
                             select new CommonDataModel
                             {
                                 //DateTime = HSActualData.HistoriceDateTime,
                                 Year = HSProjectedData.HistoriceDateTime.Year,
                                 Month = HSProjectedData.HistoriceDateTime.Month,
                                 Day = HSProjectedData.HistoriceDateTime.Day,
                                 Hour = HSProjectedData.HistoriceDateTime.Hour,
                                 ProjectedGuest = HSProjectedData.HistoriceProjectedGuestCount,
                                 ActualGuest = HSActualData.HistoricActualGuestCount              
                             };

            // Use LINQ to project the data to a list of CommonDataModel instances
            commonDataList = commonData.Select(row => new CommonDataModel
            {
                Year = row.Year,
                Month = row.Month,
                Day = row.Day,
                Hour = row.Hour,
                ProjectedGuest = row.ProjectedGuest,
                ActualGuest = row.ActualGuest
            }).ToList();

        }

        void MLActivities()
        {
            var mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(commonDataList);

            // Split the data into a training set (80%) and a test set (20%)
            var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(CommonDataModel.Year),
                                                      nameof(CommonDataModel.Month),
                                                      nameof(CommonDataModel.Day),
                                                      nameof(CommonDataModel.Hour),
                                                      nameof(CommonDataModel.ActualGuest))
          .Append(mlContext.Transforms.NormalizeMinMax("Features"))
          .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(CommonDataModel.ProjectedGuest)))
          .Append(mlContext.Transforms.Conversion.MapKeyToValue("Label"))
          .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label"));
          //.Append(mlContext.Transforms.CopyColumns("Score", "Label"));

            // Train the model
            var model = pipeline.Fit(split.TrainSet);

            GetActualData(true);
            IDataView hourlySaleForBOD = mlContext.Data.LoadFromEnumerable(HourlySalesForBODAG);
            IDataView predictBOD = model.Transform(hourlySaleForBOD);
          
            var predictedValues = mlContext.Data.CreateEnumerable<PredictionResult>(predictBOD, reuseRowObject: false)
                                                .Select(prediction => prediction.Score)
                                                .ToList();

            foreach (var predictedValue in predictedValues)
            {
                Console.WriteLine($"Predicted Value: {predictedValue}");
            }

            var accuracy = EvaluateAccuracy(mlContext, predictBOD);

            IEnumerable<int> predictedValuesByML = predictedValues.Select(f => (int)Math.Round(f)).ToArray();
            GetProjectedData(true);

            //var predictedValuesBySDM = HourlySalesForBODPG;
            IEnumerable<int> actualGuestValuesFromSDM = HourlySalesForBODAG.Select(item => (int)item.ActualGuest);
            HourlySalesForBODPG.RemoveAt(0);
            HourlySalesForBODPG.Add(new CommonDataModel { DateTime = DateTime.Now, ProjectedGuest = 0 });
            IEnumerable<int> projectedGuestValuesFromSDM = HourlySalesForBODPG.Select(item => (int)item.ProjectedGuest);
            //IEnumerable<int> hourValues = new List<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 00 };
            IEnumerable < float> hourValues = commonDataList.Select(row => row.Hour).Distinct();


            IEnumerable<Tuple<float, int, int, int>> zippedLists = hourValues.Zip(actualGuestValuesFromSDM, (a, b) => Tuple.Create(a, b))
                                                                   .Zip(predictedValuesByML, (abTuple, c) => Tuple.Create(abTuple.Item1, abTuple.Item2, c))
                                                                   .Zip(projectedGuestValuesFromSDM, (abcTuple, d) => Tuple.Create(abcTuple.Item1, abcTuple.Item2, abcTuple.Item3, d));          
            foreach (var tuple in zippedLists)
            {
                Console.WriteLine($"Value1: {tuple.Item1}, Value2: {tuple.Item2}, Value3: {tuple.Item3}, Value4: {tuple.Item4}");
                ComparisionDataWithML.Add(new ComparisionDataWithML { Hours = tuple.Item1, ActualGuestThroughSDM = tuple.Item2, 
                    ProjectedGuestByML = tuple.Item3, ProjectedGuestThroughSDM = tuple.Item4, DiffProjectionMLndSDM = tuple.Item2-tuple.Item3,
                    DiffProjectionSDMndSDM = tuple.Item2 - tuple.Item4
                });
            }

            ComparisionDataWithML.RemoveAt(ComparisionDataWithML.Count - 1);
            ActualTotalFormatted = (int)ComparisionDataWithML.Sum(x => x.ActualGuestThroughSDM);
            ProjectedSDMTotalFormatted = (int)ComparisionDataWithML.Sum(x => x.ProjectedGuestThroughSDM);
            ProjectedMLTotalFormatted = (int)ComparisionDataWithML.Sum(x => x.ProjectedGuestByML);
            DifferenceMLTotalFormatted = (int)ComparisionDataWithML.Sum(x => x.DiffProjectionMLndSDM);
            DifferenceSDMTotalFormatted = (int)ComparisionDataWithML.Sum(x => x.DiffProjectionSDMndSDM);
        }

        void GetActualData(bool isBusinessDay)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.connString))
            {
                if (conn == null)
                {
                    //
                }
                else
                {
                    conn.Open();

                    SqlCommand query = new SqlCommand("GetHourlySalesDataToBeProjected", conn);
                    query.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query);
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);
                    HSActualData.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        try
                        {
                            HistoriceDateTime = DateTime.ParseExact((string)row["timedate"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            HistoricActualGuestCount = (int)row["TotalActualGuestsPerHour"];
                        }
                        catch (Exception)
                        {
                            //
                        }

                        HSActualData.Add(new HourlySalesReportModel
                        {
                            HistoriceDateTime = HistoriceDateTime,
                            HistoricActualGuestCount = HistoricActualGuestCount
                        });
                    }
                    conn.Close();
                    var commonData = from HSActualData in HSActualData

                                     select new CommonDataModel
                                     {
                                         DateTime = HSActualData.HistoriceDateTime,
                                         Year = HSActualData.HistoriceDateTime.Year,
                                         Month = HSActualData.HistoriceDateTime.Month,
                                         Day = HSActualData.HistoriceDateTime.Day,
                                         Hour = HSActualData.HistoriceDateTime.Hour,
                                         ActualGuest = HSActualData.HistoricActualGuestCount
                                     };

                    HourlySalesForBODAG = commonData.Select(row => new CommonDataModel
                    {
                        DateTime = row.DateTime,
                        ActualGuest = row.ActualGuest
                    }).ToList();
                }
            }
        }

        void GetProjectedData(bool isBusinessDay)
        {
            using (SqlConnection conn = new SqlConnection(Settings.Default.connString))
            {
                if (conn == null)
                {
                    //
                }
                else
                {
                    conn.Open();

                    SqlCommand query = new SqlCommand("GetHourlySalesDataProjectedSDM", conn);
                    query.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query);
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);
                    HSProjectedData.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        try
                        {
                            HistoriceDateTime = (DateTime)row["CombinedDateTime"];
                            HistoriceProjectedGuestCount = (int)row["TotalProjectedGuestsPerHour"];
                        }
                        catch (Exception ex)
                        {
                            //
                        }

                        HSProjectedData.Add(new HourlySalesReportModel
                        {
                            HistoriceDateTime = HistoriceDateTime,
                            HistoriceProjectedGuestCount = HistoriceProjectedGuestCount
                        });

                    }
                    conn.Close();
                    var commonData = from HSProjectedData in HSProjectedData

                                     select new CommonDataModel
                                     {
                                         DateTime = HSProjectedData.HistoriceDateTime,
                                         ProjectedGuest = HSProjectedData.HistoriceProjectedGuestCount
                                     };

                    HourlySalesForBODPG = commonData.Select(row => new CommonDataModel
                    {
                        DateTime = row.DateTime,
                        ProjectedGuest = row.ProjectedGuest
                    }).ToList();
                }
            }
        }

        static double EvaluateAccuracy(MLContext mlContext, IDataView predictions)
        {
            // Calculate the accuracy of the model
            var metrics = mlContext.Regression.Evaluate(predictions);
            return 1.0 - metrics.MeanAbsoluteError;
        }

        public int ActualTotalFormatted { get; set; } = 0;

        public int ProjectedSDMTotalFormatted { get; set; } = 0;

        public int ProjectedMLTotalFormatted { get; set; } = 0;

        public int DifferenceMLTotalFormatted { get; set; } = 0;

        public int DifferenceSDMTotalFormatted { get; set; } = 0;

        private List<HourlySalesReportModel> _HSActualData;
        public List<HourlySalesReportModel> HSActualData
        {
            get { return _HSActualData; }
            set
            {
                _HSActualData = value;
            }
        }

        private List<HourlySalesReportModel> _HSProjectedData;
        public List<HourlySalesReportModel> HSProjectedData
        {
            get { return _HSProjectedData; }
            set
            {
                _HSProjectedData = value;
            }
        }

        private float _HistoricActualGuestCount;

        public float HistoricActualGuestCount
        {
            get { return _HistoricActualGuestCount; }
            set { _HistoricActualGuestCount = value; }
        }


        //public DateTime HistoriceDateTime { get; set; }
        private DateTime _HistoriceDateTime;

        public DateTime HistoriceDateTime
        {
            get { return _HistoriceDateTime; }
            set { _HistoriceDateTime = value; }
        }


        //public int HistoriceProjectedGuestCount { get; set; }
        private float _HistoriceProjectedGuestCount;

        public float HistoriceProjectedGuestCount
        {
            get { return _HistoriceProjectedGuestCount; }
            set { _HistoriceProjectedGuestCount = value; }
        }

    }

    public class PredictionResult
    {
        public float Score { get; set; } // This property represents the predicted score
    }

    public class CommonDataModel
    {
        public DateTime DateTime { get; set; }
        public float ProjectedGuest { get; set; }
        public float ActualGuest { get; set; }
        public float Year { get; internal set; }
        public float Month { get; internal set; }
        public float Day { get; internal set; }
        public float Hour { get; internal set; }
    }

    public class ComparisionDataWithML
    {
        public float Hours { get; set; }

        public float ProjectedGuestThroughSDM { get; set; }

        public float ActualGuestThroughSDM { get; set; }

        public float ProjectedGuestByML { get; set; }

        public float DiffProjectionMLndSDM { get; set; }

        public float DiffProjectionSDMndSDM { get; set; }
    }

}
