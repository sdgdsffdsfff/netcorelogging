﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Client
{
    /// <summary>
    /// 多线程消费队列。将输入元素打包输出。消费端可以有多个线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ThreadedTimerActionBlock<T> : ITimerActionBlock<T>
    {
        private Task[] Tasks { get; set; }

        private Action<IList<T>> Action { get; set; }

        private BlockingCollection<T> s_Queue;

        /// <summary>
        /// 最近一次异常报告时间
        /// </summary>
        private DateTime LastReportTime { get; set; }

        /// <summary>
        /// 出现的异常数量
        /// </summary>
        private int ExceptionCount;

        /// <summary>
        /// 溢出数量
        /// </summary>
        private int OverCount = 0;

        /// <summary>
        /// 最近一次异常
        /// </summary>
        private Exception LastException { get; set; }

        /// <summary>
        /// 阻塞队列的最大长度
        /// </summary>
        private int QueueMaxLength { get; set; }

        private ConcurrentStack<T> Buffer { get; set; }

        /// <summary>
        /// 元素包的大小
        /// </summary>
        private int BufferSize { get; set; }

        /// <summary>
        /// 上一次打包处理的时间
        /// </summary>
        private DateTime LastActionTime { get; set; }

        private int BlockElapsed { get; set; }

        /// <summary>
        /// 多线程消费队列。将输入元素打包输出。消费端可以有多个线程
        /// </summary>
        /// <param name="taskNum">处理队列出队的线程数量</param>
        /// <param name="action">处理委托</param>
        /// <param name="queueMaxLength">设置队列最大长度</param>
        /// <param name="bufferSize">元素包的大小</param>
        /// <param name="blockElapsed">阻塞的时间，达到该时间间隔，也会出队</param>
        public ThreadedTimerActionBlock(int taskNum, Action<IList<T>> action, int queueMaxLength, int bufferSize, int blockElapsed)
        {
            this.s_Queue = new BlockingCollection<T>();
            this.Buffer = new ConcurrentStack<T>();
            this.LastActionTime = DateTime.Now;
            this.LastReportTime = DateTime.Now;
            this.BufferSize = bufferSize;
            this.BlockElapsed = blockElapsed;
            this.Action = action;
            this.QueueMaxLength = queueMaxLength;
            this.Tasks = new Task[taskNum];
            for (int i = 0; i < taskNum; i++)
            {
                int temp_i = i;
                this.Tasks[temp_i] = Task.Factory.StartNew(this.DequeueProcess);
            }
        }

        /// <summary>
        /// 入队处理
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            int queueLen = s_Queue.Count;
            int over_count = 0;
            if (queueLen >= this.QueueMaxLength)
            {
                over_count = (queueLen - this.QueueMaxLength) + 1;
                for (int i = 0; i < over_count; i++)
                {
                    this.s_Queue.Take();//超过队列长度，扔掉
                }
                Interlocked.Add(ref this.OverCount, over_count);
            }
            this.s_Queue.Add(item);
        }

        /// <summary>
        /// 出队处理函数
        /// </summary>
        private void DequeueProcess()
        {
            while (true)
            {
                try
                {
                    T item;
                    bool hasItem = s_Queue.TryTake(out item, 200);
                    if (hasItem)
                    {
                        this.Buffer.Push(item);
                    }

                    if (this.Buffer.Count > 0)
                    {
                        if ((this.Buffer.Count >= this.BufferSize || (DateTime.Now - this.LastActionTime).TotalMilliseconds > this.BlockElapsed))
                        {
                            this.Action(this.Buffer.ToList());
                            this.Buffer.Clear();
                            this.LastActionTime = DateTime.Now;
                        }
                    }
                }
                //catch (ThreadAbortException tae)
                //{
                //    Thread.CurrentThread.ResetAbort();
                //    Interlocked.Increment(ref this.ExceptionCount);
                //    this.LastException = tae;
                //}
                //catch (TTransportDataSizeOverflowException tdoe)
                //{
                //    Interlocked.Increment(ref this.ExceptionCount);
                //    this.LastException = tdoe;
                //   // LoggingClientReport.ReportTransportOver(tdoe);
                //}
                catch (Exception ex)
                {
                    Interlocked.Increment(ref this.ExceptionCount);
                    this.LastException = ex;
                }
                finally
                {
                    this.Report();
                }
            }
        }

        private static readonly object report_lock = new object();
        private static readonly int ReportElapsed = 30;//报告时间间隔

        /// <summary>
        /// 报告Logging.Client自身异常
        /// </summary>
        private void Report()
        {
            var _now = DateTime.Now;
            var reportElapsed = (_now - this.LastReportTime).TotalSeconds;
            if (reportElapsed >= ReportElapsed)
            {
                lock (report_lock)
                {
                    LoggingClientReport.ReportException(this.LastException, this.ExceptionCount);
                    LoggingClientReport.ReportOver(this.OverCount, s_Queue.Count, this.QueueMaxLength);
                    this.ExceptionCount = 0;
                    this.LastException = null;
                    this.OverCount = 0;
                    this.LastReportTime = DateTime.Now;
                }
            }
        }

        public void Flush()
        {
            if (this.s_Queue.Count >= 0)
            {
                this.Action(this.s_Queue.ToList());
                for (int i = 0; i < s_Queue.Count; i++)
                {
                    this.s_Queue.Take();
                }
            }
            if (this.Buffer.Count > 0)
            {
                this.Action(this.Buffer.ToList());
                this.Buffer.Clear();
            }
            this.LastActionTime = DateTime.Now;
        }
    }
}