using Android.App;
using Android.Widget;
using Android.OS;
using SQLite;
using Com.Syncfusion.Calendar;
using System.IO;
using System;
using Java.Util;
using System.Linq;
using Java.Text;
using Android.Graphics;

namespace AndroidSql
{
    [Activity(Label = "AndroidSql", MainLauncher = true)]
    public class MainActivity : Activity
    {
        SfCalendar calendar;
        CalendarEventCollection appointments;
        SQLiteConnection dataBase;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.CreateDatabase();
            calendar = new SfCalendar(this);
            calendar.ShowEventsInline = true;
            calendar.DataSource = appointments;
            SetContentView(calendar);
        }

        private void CreateDatabase()
        {
            dataBase = this.GetConnection();
            dataBase.CreateTable<AppointmentDB>();

            var currentDate = (Calendar)Calendar.Instance.Clone();
            dataBase.Query<AppointmentDB>("DELETE From AppointmentDB");

            var startTime = (Calendar)currentDate.Clone();
            startTime.Set(CalendarField.Hour, 1);
            var endTime = (Calendar)currentDate.Clone();
            endTime.Set(CalendarField.Hour, 2);

            var startTime1 = (Calendar)currentDate.Clone();
            var endTime1 = (Calendar)currentDate.Clone();
            endTime1.Set(CalendarField.Hour, 1);

            var startTime2 = (Calendar)currentDate.Clone();
            startTime2.Set(CalendarField.Hour, 5);
            var endTime2 = (Calendar)currentDate.Clone();
            endTime2.Set(CalendarField.Hour, 6);

            //Insert data in to table 
            dataBase.Query<AppointmentDB>("INSERT INTO AppointmentDB (Subject,StartTime,EndTime,AllDay,Color)values ('Yoga Therapy','" + startTime.Time.ToString() + "', '" + endTime.Time.ToString() + "','false','#ff0000')");
            dataBase.Query<AppointmentDB>("INSERT INTO AppointmentDB (Subject,StartTime,EndTime,AllDay,Color)values ('Client Meeting','" + startTime1.Time.ToString() + "', '" + endTime1.Time.ToString() + "','true','#0000ff')");
            dataBase.Query<AppointmentDB>("INSERT INTO AppointmentDB (Subject,StartTime,EndTime,AllDay,Color)values ('Client Meeting','" + startTime2.Time.ToString() + "', '" + endTime2.Time.ToString() + "','false','#ffa500')");
            this.AddAppointments();
        }

        /// <summary>
        /// Creates meetings and stores in a collection.  
        /// </summary>
        private void AddAppointments()
        {
            var table = (from i in dataBase.Table<AppointmentDB>() select i);
            appointments = new CalendarEventCollection();
            foreach (var order in table)
            {
                var EventName = order.Subject;
                var From = this.ConvertToCalendar(order.StartTime);
                var To = this.ConvertToCalendar(order.EndTime);
                var AllDay = Convert.ToBoolean(order.AllDay);

                appointments.Add(new CalendarInlineEvent()
                {
                    Subject = order.Subject,
                    StartTime =this.ConvertToCalendar(order.StartTime),
                    EndTime = this.ConvertToCalendar(order.EndTime),
                    Color = Color.ParseColor(order.Color),
                    IsAllDay = Convert.ToBoolean(order.AllDay),
                });
            }           
        }

        public Calendar ConvertToCalendar(string stringDate)
        {
            SimpleDateFormat sdf = new SimpleDateFormat("EEE MMM dd HH:mm:ss z yyyy");
            var date = sdf.Parse(stringDate);
            return sdf.Calendar;
        }

        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "SampleSQLites.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = System.IO.Path.Combine(documentsPath, sqliteFilename);

            // This is where we copy in the prepopulated database
            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                var s = this.Resources.OpenRawResource(Resource.Raw.SampleSQLites);  // RESOURCE NAME ###

                // create a write stream
                FileStream writeStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                // write to the stream
                ReadWriteStream(s, writeStream);
            }

            var conn = new SQLite.SQLiteConnection(path);

            // Return the database connection 
            return conn;
        }
        void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}

