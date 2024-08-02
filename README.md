# MessageCenter-CSharp

## 使用方法

```C#
// 定义消息
class YourMessage : MessageBase<YourMessage>
{
	// 一些参数
    public string param;
	// ...
};
```

```C#
// 创建消息
var msg = new YourMessage();
msg.param = "some";
```

```C#
// 发送立即生效
MessageCenter.Send(msg);
```

```C#
// 添加监听
MessageCenter.AddListener<YourMessage>(this, (YourMessage msg) => {
	// 使用参数
	msg.param;
});
// 删除监听
MessageCenter.RemoveListener<YourMessage>(this);
```

```C#
// 使用自定义的消息派发器
// 方法和MessageCenter相同
var dispather = new MessageDispatcher();
dispather.AddListener<YourMessage>(....);
dispather.Send(....);
dispather.RemoveListener<YourMessage>(....);
// ......
```