# 腾讯信鸽.NET SDK
参照官方Java SDK V1.1.6
详细说明可查阅信鸽官方wiki </br>
http://developer.xg.qq.com/index.php/Java_SDK </br>
http://developer.xg.qq.com/index.php/Rest_API </br>
移植过程中参考了 https://github.com/yeanzhi/XinGePushSDK.NET 在此向yeanzhi致敬</br>

修复了一些bug,同时添加了部分API，如引入快捷方式、创建大批量推送消息并推送的接口</br>
1 快捷方式</br>
1.1 Android平台推送消息给单个设备</br>
XingeApp.PushTokenAndroid(000, "myKey", "标题", "大家好!", "3dc4gcd98sdc");</br>
1.2 Android平台推送消息给单个帐号</br>
XingeApp.PushAccountAndroid(000, "myKey", "标题", "大家好!", "nickName");</br>
1.3 Android平台推送消息给所有设备</br>
XingeApp.PushAllAndroid(000, "myKey", "标题", "大家好!");</br>
1.4 Android平台推送消息给标签选中设备</br>
XingeApp.PushTagAndroid(000, "myKey", "标题", "大家好!", "beijing");</br>
1.5 iOS平台推送消息给单个设备</br>
XingeApp.PushTokenIos(000, "myKey", "你好!", "3dc4gcd98sdc", XingeApp.IOSENV_PROD);</br>
1.6 iOS平台推送消息给单个帐号</br>
XingeApp.PushAccountIos(000, "myKey", "你好", "nickName", XingeApp.IOSENV_PROD);</br>
1.7 iOS平台推送消息给所有设备</br>
XingeApp.PushAllIos(000, "myKey", "大家好!", XingeApp.IOSENV_PROD);</br>
1.8 iOS平台推送消息给标签选中设备</br>
XingeApp.PushTagIos(000, "myKey", "大家好!", "beijing", XingeApp.IOSENV_PROD);</br>