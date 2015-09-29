using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XinGePush
{
    public class Message
    {
        public const int TYPE_NOTIFICATION = 1;
        public const int TYPE_MESSAGE = 2;
        public Message()
        {
            Title = "";
            Content = "";
            SendTime = "2013-12-20 18:31:00";
            AcceptTimes = new List<TimeInterval>();
            MultiPkg = 0;
            Raw = "";
            LoopInterval = -1;
            LoopTimes = -1;
            Action = new ClickAction();
            Style = new Style(0);
            CustomContent = new Dictionary<string, object>();
        }

        public void AddAcceptTime(TimeInterval acceptTime)
        {
            AcceptTimes.Add(acceptTime);
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public int ExpireTime { get; set; }
        public string SendTime { get; set; }
        public List<TimeInterval> AcceptTimes { get; set; }
        public int Type { get; set; }
        public int MultiPkg { get; set; }
        public Style Style { get; set; }
        public ClickAction Action { get; set; }
        public string Raw { get; set; }
        public int LoopInterval { get; set; }
        public int LoopTimes { get; set; }
        public IDictionary<string, object> CustomContent { get; set; }

        public bool IsValid()
        {
            if (!string.IsNullOrEmpty(Raw)) return true;
            if (Type < TYPE_NOTIFICATION || Type > TYPE_MESSAGE)
                return false;
            if (MultiPkg < 0 || MultiPkg > 1)
                return false;
            if (Type == TYPE_NOTIFICATION)
            {
                if (!Style.IsValid()) return false;
                if (!Action.IsValid()) return false;
            }
            if (ExpireTime < 0 || ExpireTime > 3 * 24 * 60 * 60)
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
            foreach (TimeInterval ti in AcceptTimes)
            {
                if (!ti.IsValid())
                    return false;
            }
            if (LoopInterval > 0 && LoopTimes > 0
                    && ((LoopTimes - 1) * LoopInterval + 1) > 15)
            {
                return false;
            }

            return true;
        }

        public String ToJson()
        {
            if (!string.IsNullOrEmpty(Raw)) return Raw;
            JObject json = new JObject();
            if (Type == TYPE_NOTIFICATION)
            {
                json.Add("title", Title);
                json.Add("content", Content);
                if (AcceptTimes.Count > 0)
                {
                    json.Add("accept_time", JsonConvert.SerializeObject(AcceptTimes));
                }
                json.Add("builder_id", Style.BuilderId);
                json.Add("ring", Style.Ring);
                json.Add("vibrate", Style.Vibrate);
                json.Add("clearable", Style.Clearable);
                json.Add("n_id", Style.NId);
                json.Add("ring_raw", Style.RingRaw);
                json.Add("lights", Style.Lights);
                json.Add("icon_type", Style.IconType);
                json.Add("icon_res", Style.IconRes);
                json.Add("style_id", Style.StyleId);
                json.Add("small_icon", Style.SmallIcon);
                json.Add("action", Action.ToJsonObject());
            }
            else if (Type == TYPE_MESSAGE)
            {
                json.Add("title", Title);
                json.Add("content", Content);
                if (AcceptTimes.Count > 0)
                {
                    json.Add("accept_time", JsonConvert.SerializeObject(AcceptTimes));
                }
            }
            if (CustomContent.Count > 0)
            {
                json.Add("custom_content", JsonConvert.SerializeObject(CustomContent));
            }
            return json.ToString();
        }
    }
}
