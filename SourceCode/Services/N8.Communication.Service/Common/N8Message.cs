using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8.Communication.Common
{
    public class N8Message
    {
        public Guid QueueId { get; set; }

        public string From { get; set; }
        
        public string To { get; set; }

        public string Content { get; set; }

        public string Title { get; set; }

        public string Method { get; set; }
    }
}
