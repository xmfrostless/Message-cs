# Message

## 使用方法

```C#
// 引用命名空间
using Message;

// 实例化一个派发器
var dispatcher = new Dispatcher();

// 定义消息
struct Custom {
	int value;
}

// 添加监听
dispatcher.AddListener<Custom>(this, msg => {
    Console.WriteLine(msg.value);
});
// 删除监听
dispatcher.RemoveListener<Custom>(this);

// 发送消息
dispatcher.Send(new Custom {
    value = 1000
});
```

```C#
// 支持任意结构的消息
int int_msg = 10;
dispatcher.Send(int_msg);

string str_msg = "hello";
dispatcher.Send(str_msg);
```