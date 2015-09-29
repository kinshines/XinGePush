using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace XinGePush
{
    public class TimeInterval
    {
        public Moment start { get; set; }
        public Moment end { get; set; }
        public TimeInterval(int startHour,int startMin,int endHour,int endMin)
        {
            start = new Moment()
            {
                hour = startHour,
                min = startMin
            };
            end = new Moment()
            {
                hour = endHour,
                min = endMin
            };
        }

        public bool IsValid()
        {
            return start.hour >= 0 && start.hour <= 23 && start.min >= 0 && start.min <= 59 && start.hour >= 0 &&
                   end.hour <= 23 && end.min >= 0 && end.min <= 59;
        }
    }

    public class Moment
    {
        public int hour { get; set; }
        public int min { get; set; }
    } 
}
