using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using utility_sample.Annotations;
using utility_sample.Core;

namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// Class for items to be displayed on UI
    /// </summary>
    public class OutputItem
    {
        /// <summary>
        /// Time of the message
        /// </summary>m
        public DateTime Time { get; private set; }
        /// <summary>
        /// Topic of the message
        /// </summary>
        public string Topic { get; private set; }
        /// <summary>
        /// Content of the message
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Constructor to make a new message for display. DateTime optional
        /// </summary>
        /// <param name="topic">Topic to show</param>
        /// <param name="content">Content to show</param>
        /// <param name="dateTime">Time to show. Defaults to now if not provided</param>
        public OutputItem(string topic, string content, DateTime? dateTime = null)
        {
            Topic = topic;
            Time = dateTime ?? DateTime.Now;
            Content = content;
        }
    }
}
