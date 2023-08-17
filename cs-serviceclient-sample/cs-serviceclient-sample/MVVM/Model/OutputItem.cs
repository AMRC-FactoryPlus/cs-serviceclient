using System;

namespace utility_sample.MVVM.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputItem
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

        public OutputItem(string content, DateTime? dateTime = null)
        {
            Time = dateTime ?? DateTime.Now;
            Content = content;
        }
    }
}
