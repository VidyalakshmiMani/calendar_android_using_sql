using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidSql
{
    public class AppointmentDB
    {
        public string Subject { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string AllDay { get; set; }
        public string Color { get; set; }
    }
}