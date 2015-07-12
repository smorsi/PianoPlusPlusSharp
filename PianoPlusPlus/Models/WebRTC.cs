using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PianoPlusPlus.Models
{
    public class WebRTC
    {
        public class Tutors {
            public string ID { get; set; }
            public string tutor { get; set; }
            public string label { get; set; }
            public string roomToken { get; set; }
            public string Sender { get; set; }
            public bool isProcessed { get; set; }
        }



    }
}