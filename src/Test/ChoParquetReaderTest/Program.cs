﻿using ChoETL;
using System;
using System.Data;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace ChoParquetReaderTest
{
    class Program
    {
        static void Test1()
        {
            StringBuilder csv = new StringBuilder();
            using (var r = new ChoParquetReader(@"test1.parquet")
                .ParquetOptions(o => o.TreatByteArrayAsString = true))
            {
                using (var w = new ChoCSVWriter(csv)
                    .WithFirstLineHeader()
                    .UseNestedKeyFormat(false)
                    )
                    w.Write(r);
            }

            Console.WriteLine(csv.ToString());
        }
        static void DataTableTest()
        {
            StringBuilder csv = new StringBuilder();
            using (var r = new ChoParquetReader(@"test1.parquet")
                .ParquetOptions(o => o.TreatByteArrayAsString = true))
            {
                var dt = r.AsDataTable();
            }

            Console.WriteLine(csv.ToString());
        }

        static void ByteArrayTest()
        {
            StringBuilder csv = new StringBuilder();
            using (var r = new ChoParquetReader(@"ByteArrayTest.parquet")
                .ParquetOptions(o => o.TreatByteArrayAsString = true)
                )
            {
                var dt = r.AsDataTable("x");
                Console.WriteLine(ChoJSONWriter.ToText(dt));
                return;
                using (var w = new ChoCSVWriter(csv)
                    .WithFirstLineHeader()
                    .UseNestedKeyFormat(false)
                    )
                    w.Write(r);
            }

            Console.WriteLine(csv.ToString());
        }

        static void ReadParquet52()
        {
            using (var r = new ChoParquetReader("myData52.parquet"))
            {
                foreach (var rec in r.Take(1))
                    Console.WriteLine(rec.Dump());
            }
        }

        public class Trade
        {
            [DisplayName("Column1")]
            public string Id { get; set; }
            [DisplayName("Column2")]
            public string Price { get; set; }
            [DisplayName("Column3")]
            public string Quantity { get; set; }
        }

        static void CSV2ParquetTest()
        {
            using (var r = new ChoCSVReader(@"..\..\..\..\..\..\data\XBTUSD.csv")
                .Configure(c => c.LiteParsing = true)
                .NotifyAfter(100000)
                .OnRowsLoaded((o, e) => $"Rows Loaded: {e.RowsLoaded} <-- {DateTime.Now}".Print())
                .ThrowAndStopOnMissingField(false)
                )
            {
                //r.Loop();
                //return;
                using (var w = new ChoParquetWriter(@"..\..\..\..\..\..\data\XBTUSD.parquet")
                    .Configure(c => c.RowGroupSize = 100000)
                .Configure(c => c.LiteParsing = true)
                    )
                    w.Write(r);
            }
        }

        static void ParseLargeParquetTest()
        {
            using (var r = new ChoParquetReader<Trade>(@"..\..\..\..\..\..\data\XBTUSD-Copy.parquet")
                .Configure(c => c.LiteParsing = true)
                .NotifyAfter(100000)
                .OnRowsLoaded((o, e) => $"Rows Loaded: {e.RowsLoaded} <-- {DateTime.Now}".Print())
                .ThrowAndStopOnMissingField(false)
                )
            {
                r.Skip(200001).Take(1).Print(); // Loop();
            }

        }

        static void Main(string[] args)
        {
            ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Off;
            ParseLargeParquetTest();
        }
    }
}
