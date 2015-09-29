using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XinGePush;
using XinGePush.Res;
using Newtonsoft.Json.Linq;

namespace XinGeConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            XingeApp xinge = new XingeApp(2200123456, "4ab910b0e8af8b892e0b0bb31af68119");
            Message mandroid = new Message
            {
                Title = "title",
                Content = "content",
                Type = Message.TYPE_NOTIFICATION
            };
            MessageIOS mios=new MessageIOS
            {
                AlertStr = "这是一个简单的alert",
                Badge = 1,
                Sound = "beep.wave"
            };
            string deviceToken = "2e9742d51d1fb1e7d2a7897035cfb93da8f7d9c60e6259adfcf78b878bd956f8";
            //Push消息（包括通知和透传消息）给单个设备
            Ret ret = xinge.PushSingleDevice(deviceToken, mios, XinGeConfig.IOSENV_DEV);
            Console.WriteLine(ret.ret_code);
            xinge.PushSingleDevice("deviceToken", mandroid);
            //Push消息（包括通知和透传消息）给单个账户或别名
            xinge.PushSingleAccount("account", mandroid);
            xinge.PushSingleAccount("account", mios, XinGeConfig.IOSENV_DEV);
            //Push消息（包括通知和透传消息）给app的所有设备
            xinge.PushAllDevice(mandroid);
            xinge.PushAllDevice(mios, XinGeConfig.IOSENV_DEV);

            //创建大批量推送消息
            Ret pushRet = xinge.CreateMultiPush(mios, XinGeConfig.IOSENV_DEV);
            if (pushRet.ret_code == 0)
            {
                JObject json = pushRet.result;
                int pushId = json.GetValue("push_id").Value<int>();
              
                //Push消息给大批量账号
                xinge.PushAccountListMultiple(pushId, new List<string>() { "account1", "account2" });
                //Push消息给大批量设备
                xinge.PushDeviceListMultiple(pushId, new List<string>() { "token1", "token2" });
            }

            //查询群发消息发送状态
            xinge.QueryPushStatus(new List<string>() { "pushId1", "pushId1" });
            //查询应用覆盖的设备数
            xinge.QueryDeviceCount();
            //Push消息（包括通知和透传消息）给tags指定的设备
            xinge.PushTags(new List<string>() { "tag1", "tag2" }, "OR", mandroid);
            xinge.PushTags(new List<string>() { "tag1", "tag2" }, "OR", mios, XinGeConfig.IOSENV_DEV);
            //查询应用的Tags
            xinge.QueryTags(0, 100);
            //取消尚未触发的定时群发任务
            xinge.CancelTimingPush("pushId1");
            //批量设置标签
            var tags = new Dictionary<string, string>();
            tags.Add("tag1", "token1");
            xinge.BatchSetTag(tags);
            //批量删除标签
            xinge.BatchDelTag(tags);
            //查询应用某token设置的标签
            xinge.QueryTokenTags("deviceToken");
            //查询应用某标签关联的设备数量
            xinge.QueryTagTokenNum("tag");
        }
    }
}
