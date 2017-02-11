﻿using Logging.Server.Alerting;
using Logging.Server.Entitys;
using Logging.Server.Metric;
using Logging.Server.Metric.Writer;
using Logging.Server.Reciver;
using Logging.Server.Viewer;
using Logging.ThriftContract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Logging.Server.Site
{
    [Route("api")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet, Route("GetLogOnOff")]
        public dynamic GetLogOnOff(int appId = 0)
        {
            string resp = string.Empty;

            var on_off = LogViewerManager.GetLogViewer().GetLogOnOff(appId);
            if (on_off != null)
            {
                resp += on_off.Debug + "," + on_off.Info + "," + on_off.Warn + "," + on_off.Error;
            }

            #region 计数

            TMetricEntity metric = new TMetricEntity();
            metric.Name = "logging_client_getLogOnOff_count";
            metric.Time = Utils.GetTimeStamp(DateTime.Now) / 10000000;
            metric.Value = 1;
            metric.Tags = new Dictionary<string, string>();
            metric.Tags.Add("AppId", appId.ToString());
            List<TMetricEntity> metrics = new List<TMetricEntity>();
            metrics.Add(metric);

            TLogPackage logPackage = new TLogPackage();
            logPackage.AppId = appId;
            logPackage.IP = 0;
            logPackage.MetricItems = metrics;

            LogReciverBase LogReciver = new LogReciverBase();

            LogReciver.Log(logPackage);

            #endregion 计数

            return resp;
        }

        [HttpGet, Route("GetLogOnOffs")]
        public dynamic GetLogOnOffs()
        {
            var on_offs = LogOnOffManager.GetALLLogOnOff();
            return Newtonsoft.Json.JsonConvert.SerializeObject(on_offs);
        }

        [HttpGet, Route("GetLogOnOffs")]
        public dynamic SetLogOnOff(int appId, string appName, int debug = 1, int info = 1, int warn = 1, int error = 1)
        {
            LogOnOff on_off = new LogOnOff();
            on_off.AppId = appId;
            on_off.Debug = (byte)debug;
            on_off.Info = (byte)info;
            on_off.Warn = (byte)warn;
            on_off.Error = (byte)error;
            on_off.AppName = appName;

            LogOnOffManager.SetLogOnOff(on_off);

            return 0;
        }

        [HttpGet, Route("LogViewer")]
        public dynamic LogViewer()
        {
            long start = Convert.ToInt64(Request.Query["start"]);
            long end = Convert.ToInt64(Request.Query["end"]);
            int appId = Convert.ToInt32(Request.Query["appId"]);

            string level_str = Request.Query["level"];
            if (string.IsNullOrWhiteSpace(level_str))
            {
                level_str = "1,2,3,4";
            }
            List<int> level = new List<int>();

            level_str.Split(',').ToList().ForEach(x => level.Add(Convert.ToInt32(x)));

            string title = Request.Query["title"];
            string msg = Request.Query["msg"];
            string ip = Request.Query["ip"];
            string source = Request.Query["source"];
            int limit = Convert.ToInt32(Request.Query["limit"]);
            string tags_str = Request.Query["tags"];
            var viewer = LogViewerManager.GetLogViewer();

            var result = new LogVM();

            long ipNum = Utils.IPToNumber(ip);

            List<string> tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(tags_str))
            {
                tags = tags_str.Split(',').Distinct().ToList();
            }

            var lst = viewer.GetLogs(start, end, appId, level.ToArray(), title, msg, source, ipNum, tags, limit);

            result.List = lst;
            result.Start = start;
            result.End = end;
            var last = lst.LastOrDefault();
            if (last != null)
            {
                result.Cursor = lst.Min(x => x.Time);
                long min = result.List.Min(x => x.Time);
                long max = result.List.Max(x => x.Time);

                long first = result.List.FirstOrDefault().Time;
                long lastt = result.List.LastOrDefault().Time;
            }
            var json_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return json_result;
        }

        [HttpGet, Route("StatisticsPeriod")]
        public dynamic StatisticsPeriod()
        {
            long period = Convert.ToInt64(Request.Query["period"]);
            int appId = Convert.ToInt32(Request.Query["appId"]);
            long start_num = Utils.GetTimeStamp(DateTime.Now.AddMinutes(-period));
            long end_num = Utils.GetTimeStamp(DateTime.Now);
            var viewer = LogViewerManager.GetLogViewer();
            var s = viewer.GetStatistics(start_num, end_num, appId);
            JObject obj = new JObject();
            obj.Add("Data", JToken.FromObject(s));
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        [HttpGet, Route("StatisticsViewer")]
        public dynamic StatisticsViewer()
        {
            long start = Convert.ToInt64(Request.Query["start"]);
            long end = Convert.ToInt64(Request.Query["end"]);
            int appId = Convert.ToInt32(Request.Query["appId"]);
            var viewer = LogViewerManager.GetLogViewer();
            var s = viewer.GetStatistics(start, end, appId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(s);
        }

        [HttpGet, Route("GetAppErrOpts")]
        public dynamic GetAppErrOpts()
        {
            var opts = AppErrorthAlerting.GetOptions();
            return Newtonsoft.Json.JsonConvert.SerializeObject(opts);
        }

        [HttpGet, Route("SetAppErrOpts")]
        public dynamic SetAppErrOpts()
        {
            var interval = Convert.ToInt32(Request.Query["interval"]);
            var errorCountLimit = Convert.ToInt32(Request.Query["errorCountLimit"]);
            var errorGrowthLimit = Convert.ToInt32(Request.Query["errorGrowthLimit"]);
            var emailReceivers = Request.Query["emailReceivers"];
            AppErrorthAlerting.SetOptions(interval, errorCountLimit, errorGrowthLimit, emailReceivers);
            return "{'status':1}";
        }

        [HttpGet, Route("MetricsQuery")]
        public dynamic MetricsQuery()
        {
            var req = Request;

            var ac = AppContext.BaseDirectory;

           var pb= Request.PathBase.ToString();
       //   Request.HttpContext.
       
            string InfluxdbConnectionString = "";
            if (Config.MetricInfluxdbVer == "0.8")
            {
                InfluxdbConnectionString = $"http://{Config.MetricInfluxdbHost}:{Config.MetricInfluxdbPort}/db/{Config.MetricInfluxdbDBName}/series?u={Config.MetricInfluxdbUser}&p={Config.MetricInfluxdbPwd}";
            }
            else
            {
                InfluxdbConnectionString = $"http://{Config.MetricInfluxdbHost}:{Config.MetricInfluxdbPort}/query?db={Config.MetricInfluxdbDBName}&u={Config.MetricInfluxdbUser}&p={Config.MetricInfluxdbPwd}";
            }

            string queryCmd = Request.Query["cmd"];

            string queryUrl = InfluxdbConnectionString + "&q=" + queryCmd;

            string resp = Utils.HttpGet(queryUrl);
            return resp;
        }

        [HttpGet, Route("MetricTags")]
        public dynamic MetricTags()
        {
            string InfluxdbConnectionString = $"{Config.MetricInfluxdbHost}:{Config.MetricInfluxdbPort}/db/{Config.MetricInfluxdbDBName}/series?u={Config.MetricInfluxdbUser}&p={Config.MetricInfluxdbPwd}";

            string metricName = Request.Query["metricName"];

            string queryUrl = $"{InfluxdbConnectionString}&q=select * from {metricName} limit 1";

            string resp = Utils.HttpGet(queryUrl);
            return resp;
        }

        [HttpGet, Route("Point")]
        public dynamic Point()
        {
            string name = Request.Query["name"];
            double value = Convert.ToDouble(Request.Query["value"]);
            string tags_str = Request.Query["tags"];

            var tags = new Dictionary<string, string>();

            List<MetricEntity> lst = new List<MetricEntity>();
            MetricEntity m = new MetricEntity();
            m.Name = name;
            m.Value = value;
            if (!string.IsNullOrWhiteSpace(tags_str))
            {
                var arr_tags = tags_str.Split('&');
                for (int i = 0; i < arr_tags.Length; i++)
                {
                    string[] tag = arr_tags[i].Split('=');
                    string tag_key = tag[0];
                    string tag_value = tag[1];
                    if (!string.IsNullOrWhiteSpace(tag_key))
                    {
                        tags[tag_key] = tag_value;
                    }
                }
            }
            m.Tags = tags;
            m.Time = Utils.GetUnixTime(DateTime.Now);
            lst.Add(m);
            var w = new InfluxdbReport();
            w.Write(lst);

            return "point success :" + Request.QueryString;
        }
    }
}