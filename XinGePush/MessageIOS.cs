using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XinGePush
{
    /// <summary>
    /// iOS通知
    /// </summary>
    public class MessageIOS
    {
        public MessageIOS()
        {
            SendTime = "2014-03-13 16:13:00";
            AcceptTimes=new List<TimeInterval>();
            Raw = "";
            AlertStr = "";
            AlertJo=new JObject();
            Badge = 0;
            Category = "";
            LoopInterval = -1;
            LoopTimes = -1;
            Custom = new Dictionary<string, object>();
        }
        public int ExpireTime { get; set; }
        public string SendTime { get; set; }
        public List<TimeInterval> AcceptTimes { get; set; }
        public string Raw { get; set; }
        public string AlertStr { get; set; }
        public JObject AlertJo { get; set; }
        public int Badge { get; set; }
        public string Sound { get; set; }
        public string Category { get; set; }
        public int LoopInterval { get; set; }
        public int LoopTimes { get; set; }
        public int Type { get { return 0; } }
        public IDictionary<string, object> Custom { get; set; }

        public bool IsValid()
        {
            if (ExpireTime < 0 || ExpireTime > 3*24*60*60)
                return false;
            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("zh-cn", true);
                DateTime.Parse(SendTime, format);
            }
            catch (FormatException e)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(Raw))
                return true;
            foreach (TimeInterval ti in AcceptTimes)
            {
                if (!ti.IsValid())
                    return false;
            }
            if (string.IsNullOrEmpty(AlertStr) && AlertJo.Count == 0)
                return false;

            return true;
        }

        public String ToJson()
        {
            if (!string.IsNullOrEmpty(Raw)) 
                return Raw;
            JObject json = new JObject();
            
            if (AcceptTimes.Count > 0)
            {
                json.Add("accept_time", JsonConvert.SerializeObject(AcceptTimes));
            }
            JObject aps = new JObject();
            if (AlertJo.Count != 0)
                aps.Add("alert",AlertJo);
            else
                aps.Add("alert",AlertStr);
            if (Badge != 0)
                aps.Add("badge", Badge);
            if (!string.IsNullOrEmpty(Sound))
                aps.Add("sound", Sound);
            if(!string.IsNullOrEmpty(Category))
                aps.Add("category",Category);
            json.Add("aps", aps);

            foreach (KeyValuePair<string, object> pair in Custom)
            {
                json.Add(pair.Key, pair.Value.ToString());
            }
            return json.ToString();
        }

    }
}
