﻿using Logging.Server.Metric.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Server.Metric.Writer
{
    public partial class InfluxdbReport
    {
        private readonly Uri influxdb;

        public InfluxdbReport(Uri influxdb)
        {
            this.influxdb = influxdb;
        }

        private static string host = Config.MetricInfluxdbHost;// ConfigurationManager.AppSettings["MetricInfluxdbHost"];
        private static string port = Config.MetricInfluxdbPort;// ConfigurationManager.AppSettings["MetricInfluxdbPort"];
        private static string database = Config.MetricInfluxdbDBName;// ConfigurationManager.AppSettings["MetricInfluxdbDBName"];
        private static string user = Config.MetricInfluxdbUser;// ConfigurationManager.AppSettings["MetricInfluxdbUser"];
        private static string pass = Config.MetricInfluxdbPwd;// ConfigurationManager.AppSettings["MetricInfluxdbPwd"];

        public InfluxdbReport()
        {
            this.influxdb = new Uri(string.Format(@"http://{0}:{1}/db/{2}/series?u={3}&p={4}&time_precision=s", host, port, database, user, pass));
        }

        private class InfluxRecord
        {
            public InfluxRecord(string name, long timestamp, IEnumerable<string> columns, IEnumerable<JsonValue> data)
            {
                var points = Enumerable.Concat(new[] { new LongJsonValue(timestamp) }, data);

                this.Json = new JsonObject(new[] {
                    new JsonProperty("name",name),
                    new JsonProperty("columns", Enumerable.Concat(new []{"time"}, columns)),
                    new JsonProperty("points", new JsonValueArray( new [] { new JsonValueArray( points)}))
                });
            }

            public JsonObject Json { get; private set; }
        }

        private string GetWriteJsonString(IList<MetricEntity> logs)
        {
            List<InfluxRecord> data = new List<InfluxRecord>();
            foreach (var item in logs)
            {
                IEnumerable<string> columns = new string[] { "value" };

                IEnumerable<JsonValue> points = new JsonValue[] { new DoubleJsonValue(item.Value) };

                if (item.Tags != null && item.Tags.Count > 0)
                {
                    var tagKeys = item.Tags.Keys.ToArray();

                    JsonValue[] tagVals = new JsonValue[item.Tags.Count];

                    columns = Enumerable.Concat(columns, tagKeys);

                    for (int i = 0; i < tagKeys.Length; i++)
                    {
                        var tagVal = item.Tags[tagKeys[i]];
                        tagVals[i] = new StringJsonValue(tagVal);
                    }
                    points = Enumerable.Concat(points, tagVals);
                }
                var record = new InfluxRecord(item.Name, item.Time, columns, points);
                data.Add(record);
            }
            var jsonstr = new CollectionJsonValue(data.Select(d => d.Json)).AsJson();
            return jsonstr;
        }

        /// <summary>
        /// 描述：异步将LogMetric写入Influxdb数据库
        /// 作者：徐明祥
        /// 日期：20150531
        /// </summary>
        /// <param name="logs"></param>
        public void WriteAsync(IList<MetricEntity> logs)
        {
            if (logs == null || logs.Count == 0) { return; }

            var jsonstr = GetWriteJsonString(logs);
            var client = new WebClient();

            client.UploadStringAsync(this.influxdb, jsonstr).ConfigureAwait(false);
        }

        /// <summary>
        /// 描述：将LogMetric写入Influxdb数据库
        /// 作者：徐明祥
        /// 日期：20150531
        /// </summary>
        /// <param name="logs"></param>
        public string Write(IList<MetricEntity> logs)
        {
            if (logs == null || logs.Count == 0) { return string.Empty; }
            string jsonstr = GetWriteJsonString(logs);
            var client = new WebClient();
            return client.UploadString(this.influxdb, jsonstr);
        }
    }
}