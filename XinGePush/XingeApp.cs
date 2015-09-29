using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XinGePush.Res;
using XinGePush.Utility;

namespace XinGePush
{
    public class XingeApp
    {
        private long accessId;
        private string secretKey;
        public uint valid_time;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="accessId">accessId</param>
        /// <param name="secretKey">secretKey</param>
        /// <param name="valid_time">配合timestamp确定请求的有效期，单位为秒，
        /// 最大值为600。若不设置此参数或参数值非法，则按默认值600秒计算有效期</param>
        public XingeApp(long accessId, string secretKey, uint valid_time = 600)
        {
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("secretKey");
            }
            this.valid_time = valid_time;
            this.accessId = accessId;
            this.secretKey = secretKey;
        }

        /// <summary>
        /// 发起推送请求到信鸽并获得相应
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameters">字段</param>
        /// <returns>返回值json反序列化后的类</returns>
        private Ret CallRestful(String url, IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            try
            {
                parameters.Add("access_id", accessId.ToString());
                parameters.Add("timestamp", ((int)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(
                new System.DateTime(1970, 1, 1))).TotalSeconds).ToString());
                parameters.Add("valid_time", valid_time.ToString());
                string md5sing = SignUtility.GetSignature(parameters, this.secretKey, url);
                parameters.Add("sign", md5sing);
                var res = HttpWebResponseUtility.CreatePostHttpResponse(url, parameters, null, null, Encoding.UTF8, null);
                var resstr = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resstr);
                var resstring = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Ret>(resstring);
            }
            catch (Exception e)
            {
                return new Ret { ret_code = -1, err_msg = e.Message };
            }
        }

        #region 易用的api接口v1.1.4引入
        /// <summary>
        /// Android平台推送消息给单个设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Ret PushTokenAndroid(long accessId, String secretKey, String title, String content, String token)
        {
            var message = new Message()
            {
                Title = title,
                Type = Message.TYPE_NOTIFICATION,
                Content = content
            };

            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushSingleDevice(token, message);
        }

        /// <summary>
        /// Android平台推送消息给单个账号
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Ret PushAccountAndroid(long accessId, String secretKey, String title, String content, String account)
        {
            var message = new Message()
            {
                Title = title,
                Type = Message.TYPE_NOTIFICATION,
                Content = content
            };
            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushSingleAccount(account, message);
        }

        /// <summary>
        /// Android平台推送消息给所有设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Ret PushAllAndroid(long accessId, String secretKey, String title, String content)
        {
            var message = new Message()
            {
                Title = title,
                Type = Message.TYPE_NOTIFICATION,
                Content = content
            };
            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushAllDevice(message);
        }

        /// <summary>
        /// Android平台推送消息给标签选中设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Ret PushTagAndroid(long accessId, String secretKey, String title, String content, String tag)
        {
            var message = new Message()
            {
                Title = title,
                Type = Message.TYPE_NOTIFICATION,
                Content = content
            };

            XingeApp xinge = new XingeApp(accessId, secretKey);
            List<String> tagList = new List<String>();
            tagList.Add(tag);
            return xinge.PushTags(tagList, "OR", message);
        }

        /// <summary>
        /// IOS平台推送消息给单个设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Ret PushTokenIos(long accessId, String secretKey, String content, String token, uint env)
        {
            var message = new MessageIOS
            {
                AlertStr = content,
                Badge = 1,
                Sound = "beep.wave"
            };
            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushSingleDevice(token, message, env);
        }

        /// <summary>
        /// IOS平台推送消息给单个账号
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="content"></param>
        /// <param name="account"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Ret PushAccountIos(long accessId, String secretKey, String content, String account, uint env)
        {
            var message = new MessageIOS
            {
                AlertStr = content,
                Badge = 1,
                Sound = "beep.wave"
            };
            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushSingleAccount(account, message, env);
        }
        /// <summary>
        /// IOS平台推送消息给所有设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="content"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Ret PushAllIos(long accessId, String secretKey, String content, uint env)
        {
            var message = new MessageIOS
            {
                AlertStr = content,
                Badge = 1,
                Sound = "beep.wave"
            };

            XingeApp xinge = new XingeApp(accessId, secretKey);
            return xinge.PushAllDevice(message, env);
        }

        /// <summary>
        /// IOS平台推送消息给标签选中设备
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Ret PushTagIos(long accessId, String secretKey, String content, String tag, uint env)
        {
            var message = new MessageIOS
            {
                AlertStr = content, 
                Badge = 1, 
                Sound = "beep.wave"
            };

            XingeApp xinge = new XingeApp(accessId, secretKey);
            List<String> tagList = new List<String>();
            tagList.Add(tag);
            return xinge.PushTags(tagList, "OR", message, env);
        }

        #endregion

        /// <summary>
        /// 推送到 单个设备 IOS
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="msg"></param>
        /// <param name="environment"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushSingleDevice(string deviceToken, MessageIOS msg, uint environment)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (string.IsNullOrEmpty(deviceToken))
            {
                throw new ArgumentNullException("deviceToken");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("device_token", deviceToken);
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("environment", environment.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHSINGLEDEVICE, parameters);
        }

        /// <summary>
        /// 推送到 单个设备 安卓
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="message"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushSingleDevice(string deviceToken, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            
            if (string.IsNullOrEmpty(deviceToken))
            {
                throw new ArgumentNullException("deviceToken");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("device_token", deviceToken);
            parameters.Add("send_time", message.SendTime);
            parameters.Add("multi_pkg", message.MultiPkg.ToString());
            parameters.Add("expire_time", message.ExpireTime.ToString());
            parameters.Add("message", message.ToJson());
            parameters.Add("message_type", message.Type.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHSINGLEDEVICE, parameters);
        }






        /// <summary>
        /// 推送到 单个用户 IOS
        /// </summary>
        /// <param name="Account">账号</param>
        /// <param name="msg">信息</param>
        /// <param name="environment">推送环境(开发or在线)</param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushSingleAccount(string Account, MessageIOS msg, uint environment)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (string.IsNullOrEmpty(Account))
            {
                throw new ArgumentNullException("Account");
            }

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("account", Account);
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("environment", environment.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHSINGLEACCOUNT, parameters);
        }

        /// <summary>
        /// 推送到 单个用户 Android
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="msg"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushSingleAccount(string Account, Message msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (string.IsNullOrEmpty(Account))
            {
                throw new ArgumentNullException("Account");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("account", Account);
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("multi_pkg", msg.MultiPkg.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHSINGLEACCOUNT, parameters);
        }

        /// <summary>
        /// 推送消息给多个账号(Android)
        /// </summary>
        /// <param name="accountList">目标账号，最大限制100</param>
        /// <param name="msg"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushAccountList(List<String> accountList, Message msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (accountList.Count == 0)
            {
                throw new ArgumentNullException("accountList");
            }
            if (accountList.Count > 100)
            {
                throw new ArgumentOutOfRangeException("accountList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("account_list", JsonConvert.SerializeObject(accountList));
            parameters.Add("multi_pkg", msg.MultiPkg.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHACCOUNTLIST, parameters);
        }

        /// <summary>
        /// 推送消息给多个账号(iOS)
        /// </summary>
        /// <param name="accountList">目标账号，最大限制100</param>
        /// <param name="msg"></param>
        /// <param name="environment"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushAccountList(List<String> accountList, MessageIOS msg, uint environment)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            if (accountList.Count == 0)
            {
                throw new ArgumentNullException("accountList");
            }
            if (accountList.Count > 100)
            {
                throw new ArgumentOutOfRangeException("accountList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("account_list", JsonConvert.SerializeObject(accountList));
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("environment", environment.ToString());
            return CallRestful(XinGeConfig.RESTAPI_PUSHACCOUNTLIST, parameters);
        }





        /// <summary>
        /// 推送到所有android设备
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushAllDevice(Message msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("multi_pkg", msg.MultiPkg.ToString());
            if (msg.LoopInterval > 0 && msg.LoopTimes > 0)
            {
                parameters.Add("loop_interval", msg.LoopInterval.ToString());
                parameters.Add("loop_times", msg.LoopTimes.ToString());
            }
            return CallRestful(XinGeConfig.RESTAPI_PUSHALLDEVICE, parameters);
        }

        /// <summary>
        /// 推送到所有ios设备
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="environment"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushAllDevice(MessageIOS msg, uint environment)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("send_time", msg.SendTime);
            if (msg.LoopInterval > 0 && msg.LoopTimes > 0)
            {
                parameters.Add("loop_interval", msg.LoopInterval.ToString());
                parameters.Add("loop_times", msg.LoopTimes.ToString());
            }
            parameters.Add("environment", environment.ToString());

            return CallRestful(XinGeConfig.RESTAPI_PUSHALLDEVICE, parameters);
        }

        /// <summary>
        /// 通过Tag推送到android设备
        /// </summary>
        /// <param name="tagList"></param>
        /// <param name="tagOp"></param>
        /// <param name="msg"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushTags(List<String> tagList, String tagOp, Message msg)
        {
            if (tagList == null || tagList.Count == 0)
            {
                throw new ArgumentNullException("tagList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("tags_list", JsonConvert.SerializeObject(tagList));
            parameters.Add("tags_op", tagOp);
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("multi_pkg", msg.MultiPkg.ToString());
            if (msg.LoopInterval > 0 && msg.LoopTimes > 0)
            {
                parameters.Add("loop_interval", msg.LoopInterval.ToString());
                parameters.Add("loop_times", msg.LoopTimes.ToString());
            }

            return CallRestful(XinGeConfig.RESTAPI_PUSHTAGS, parameters);
        }

        /// <summary>
        /// 通过tag推送到ios设备
        /// </summary>
        /// <param name="tagList"></param>
        /// <param name="tagOp"></param>
        /// <param name="msg"></param>
        /// <param name="environment"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret PushTags(List<String> tagList, String tagOp, MessageIOS msg, uint environment)
        {
            if (tagList == null || tagList.Count == 0)
            {
                throw new ArgumentNullException("tagList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("tags_list", JsonConvert.SerializeObject(tagList));
            parameters.Add("tags_op", tagOp);
            parameters.Add("expire_time",msg.ExpireTime.ToString());
            parameters.Add("send_time", msg.SendTime);
            parameters.Add("environment", environment.ToString());
            if (msg.LoopInterval > 0 && msg.LoopTimes > 0)
            {
                parameters.Add("loop_interval", msg.LoopInterval.ToString());
                parameters.Add("loop_times", msg.LoopTimes.ToString());
            }
            return CallRestful(XinGeConfig.RESTAPI_PUSHTAGS, parameters);
        }

        /// <summary>
        /// 创建大批量推送消息
        /// </summary>
        /// <param name="msg">Android通知和透传消息</param>
        /// <returns></returns>
        public Ret CreateMultiPush(Message msg)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("multi_pkg", msg.MultiPkg.ToString());
            return CallRestful(XinGeConfig.RESTAPI_CREATEMULTIPUSH, parameters);
        }

        /// <summary>
        /// 创建大批量推送消息
        /// </summary>
        /// <param name="msg">IOS通知</param>
        /// <param name="environment">环境</param>
        /// <returns></returns>
        public Ret CreateMultiPush(MessageIOS msg, uint environment)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("message", msg.ToJson());
            parameters.Add("message_type", msg.Type.ToString());
            parameters.Add("expire_time", msg.ExpireTime.ToString());
            parameters.Add("environment", environment.ToString());
            return CallRestful(XinGeConfig.RESTAPI_CREATEMULTIPUSH, parameters);
        }

        /// <summary>
        /// 删除群发任务的离线消息
        /// </summary>
        /// <param name="pushId">CreateMultiPush返回的push_id</param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret DeleteOfflineMsg(String pushId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("push_id", pushId);
            return CallRestful(XinGeConfig.RESTAPI_DELETEOFFLINEMSG, parameters);
        }

        /// <summary>
        /// 推送消息给大批量账号(可多次)
        /// </summary>
        /// <param name="pushId">CreateMultiPush返回的push_id</param>
        /// <param name="accountList">目标账号，最大限制1000</param>
        /// <returns></returns>
        public Ret PushAccountListMultiple(int pushId, List<String> accountList)
        {
            if (accountList.Count > 1000)
            {
                throw new ArgumentOutOfRangeException("accountList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("push_id", pushId.ToString());
            parameters.Add("account_list", JsonConvert.SerializeObject(accountList));
            return CallRestful(XinGeConfig.RESTAPI_PUSHACCOUNTLISTMULTIPLE, parameters);
        }

        /// <summary>
        /// 推送消息给大批量设备(可多次)
        /// </summary>
        /// <param name="pushId">CreateMultiPush返回的push_id</param>
        /// <param name="deviceList">目标设备，最大限制1000</param>
        /// <returns></returns>
        public Ret PushDeviceListMultiple(int pushId, List<String> deviceList)
        {
            if (deviceList.Count > 1000)
            {
                throw new ArgumentOutOfRangeException("deviceList");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("push_id", pushId.ToString());
            parameters.Add("device_list", JsonConvert.SerializeObject(deviceList));
            return CallRestful(XinGeConfig.RESTAPI_PUSHDEVICELISTMULTIPLE, parameters);
        }

        /// <summary>
        /// 查询群发消息发送状态
        /// </summary>
        /// <param name="pushIdList"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryPushStatus(List<String> pushIdList)
        {
            
            JArray jArray=new JArray();
            foreach (var item in pushIdList)
            {
                JObject jObject = new JObject();
                jObject.Add("push_id", item);
                jArray.Add(jObject);
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("push_ids", jArray.ToString());
            return CallRestful(XinGeConfig.RESTAPI_QUERYPUSHSTATUS, parameters);
        }

        /// <summary>
        /// 查询应用覆盖的设备数
        /// </summary>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryDeviceCount()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            return CallRestful(XinGeConfig.RESTAPI_QUERYDEVICECOUNT, parameters);
        }

        /// <summary>
        /// 查询应用的Tags
        /// </summary>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryTags(int start, int limit)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("start", start.ToString());
            parameters.Add("limt", limit.ToString());
            return CallRestful(XinGeConfig.RESTAPI_QUERYTAGS, parameters);
        }

        /// <summary>
        /// 查询应用的Tags
        /// </summary>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryTags()
        {
            return QueryTags(0, 100);
        }


        /// <summary>
        /// 取消尚未触发的定时群发任务
        /// </summary>
        /// <param name="pushId"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret CancelTimingPush(String pushId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("push_id", pushId);
            return CallRestful(XinGeConfig.RESTAPI_CANCELTIMINGPUSH, parameters);
        }

        /// <summary>
        /// 批量设置标签
        /// </summary>
        /// <param name="tagTokenPairs"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret BatchSetTag(IDictionary<string, string> tagTokenPairs)
        {

            if (tagTokenPairs == null)
            {
                throw new ArgumentNullException("tagTokenPairs");
            }
            if (tagTokenPairs.Count > 20)
            {
                throw new ArgumentOutOfRangeException("tagTokenPairs");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            JArray jarray = new JArray();
            foreach (var item in tagTokenPairs)
            {
                JArray ja = new JArray();
                ja.Add(item.Key);
                ja.Add(item.Value);
                jarray.Add(ja);
            }
            parameters.Add("tag_token_list", jarray.ToString());
            return CallRestful(XinGeConfig.RESTAPI_BATCHSETTAG, parameters);
        }

        /// <summary>
        /// 批量删除标签
        /// </summary>
        /// <param name="tagTokenPairs"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret BatchDelTag(IDictionary<string, string> tagTokenPairs)
        {

            if (tagTokenPairs == null)
            {
                throw new ArgumentNullException("tagTokenPairs");
            }
            if (tagTokenPairs.Count > 20)
            {
                throw new ArgumentOutOfRangeException("tagTokenPairs");
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            JArray jarray = new JArray();
            foreach (var item in tagTokenPairs)
            {
                JArray ja = new JArray();
                ja.Add(item.Key);
                ja.Add(item.Value);
                jarray.Add(ja);
            }
            parameters.Add("tag_token_list", jarray.ToString());
            return CallRestful(XinGeConfig.RESTAPI_BATCHDELTAG, parameters);
        }

        /// <summary>
        /// 查询应用某token设置的标签
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryTokenTags(String deviceToken)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("device_token", deviceToken);
            return CallRestful(XinGeConfig.RESTAPI_QUERYTOKENTAGS, parameters);
        }

        /// <summary>
        /// 查询应用某标签关联的设备数量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>返回值json反序列化后的类</returns>
        public Ret QueryTagTokenNum(String tag)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tag", tag);
            return CallRestful(XinGeConfig.RESTAPI_QUERYTAGTOKENNUM, parameters);
        }
        /// <summary>
        /// 查询 token的相关信息
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public Ret QueryInfoOfToken(String deviceToken)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("device_token", deviceToken);
            return CallRestful(XinGeConfig.RESTAPI_QUERYINFOOFTOKEN, parameters);
        }

        /// <summary>
        /// 查询account绑定的token
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Ret QueryTokensOfAccount(String account)
        {
            IDictionary<string, string> paramaters = new Dictionary<string, string>();
            paramaters.Add("account", account);
            return CallRestful(XinGeConfig.RESTAPI_QUERYTOKENSOFACCOUNT, paramaters);
        }
    }
}
