using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace XinGePush
{
    public class ClickAction
    {
        public const int TYPE_ACTIVITY = 1;
	    public const int TYPE_URL = 2;
	    public const int TYPE_INTENT = 3;
        public ClickAction()
        {
            Url = "";
            ActionType = 1;
            Activity = "";
            AtyAttrIntentFlag = 0;
            AtyAttrPendingIntentFlag = 0;
            PackageDownloadUrl = "";
            ConfirmOnPackageDownloadUrl = 1;
            PackageName = "";
        }
        public int ActionType { private get; set; }
        public string Activity { private get; set; }
        public string Url { private get; set; }
        public int ConfirmOnUrl { private get; set; }
        public string Intent { private get; set; }
        public int AtyAttrIntentFlag { private get; set; }
        public int AtyAttrPendingIntentFlag { private get; set; }
        public string PackageDownloadUrl { private get; set; }
        public int ConfirmOnPackageDownloadUrl { private get; set; }
        public string PackageName { private get; set; }

        public bool IsValid()
        {
            if (ActionType < TYPE_ACTIVITY || ActionType > TYPE_INTENT) return false;

            if (ActionType == TYPE_URL)
            {
                if (string.IsNullOrEmpty(Url) || ConfirmOnUrl < 0 || ConfirmOnUrl > 1) return false;
                return true;
            }
            if (ActionType == TYPE_INTENT)
            {
                if (string.IsNullOrEmpty(Intent)) return false;
                return true;
            }
            return true;
        }
        public JObject ToJsonObject()
        {
            JObject json = new JObject();
            json.Add("action_type", ActionType);
            JObject browser = new JObject();
            browser.Add("url", Url);
            browser.Add("confirm", ConfirmOnUrl);
            json.Add("browser", browser);
            json.Add("activity", Activity);
            json.Add("intent", Intent);
            JObject aty_attr = new JObject();
            aty_attr.Add("if", AtyAttrIntentFlag);
            aty_attr.Add("pf", AtyAttrPendingIntentFlag);
            json.Add("aty_attr", aty_attr);
            return json;
        }
        public string ToJson()
        {
            return ToJsonObject().ToString();
        }

    }
}
