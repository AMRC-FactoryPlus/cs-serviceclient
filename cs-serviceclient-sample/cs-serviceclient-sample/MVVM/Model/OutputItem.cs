using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using utility_sample.Annotations;
using utility_sample.Core;

namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputItem
    {
        /// <summary>
        /// 
        /// </summary>m
        public DateTime Time { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; private set; }

        public OutputItem(string topic, string content, DateTime? dateTime = null)
        {
            Topic = topic;
            Time = dateTime ?? DateTime.Now;
            Content = content;
        }
    }
}
