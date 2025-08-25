
/*
    https://github.com/xmfrostless/Message-cs
*/

using System;
using System.Reflection;

public class Test {

    struct Msg {
        public int value;

        public override string ToString() {
            return value.ToString();
        }
    }

    void Test1() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        Message.Dispatcher dispatcher = new Message.Dispatcher();
        dispatcher.AddListener<int>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.AddListener<float>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.AddListener<double>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.AddListener<uint>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.AddListener<Msg>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.Send(1);
        dispatcher.Send(2.0f);
        dispatcher.Send(3.0);
        dispatcher.Send(4u);
        dispatcher.Send(new Msg { value = 5 });
        dispatcher.RemoveListener<int>(this);
        dispatcher.RemoveListener<float>(this);
        dispatcher.RemoveListener<double>(this);
        dispatcher.RemoveListener<uint>(this);
        dispatcher.RemoveListener<Msg>(this);
        dispatcher.Send(6);
        dispatcher.Send(7.0f);
        dispatcher.Send(8.0);
        dispatcher.Send(9u);
        dispatcher.Send(new Msg { value = 10 });

        dispatcher.AddListener<string>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
        });
        dispatcher.Send("Hello");
        dispatcher.RemoveAllListeners(this);
        dispatcher.Send("Good");
    }

    void Test2() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        int binder1 = 0;
        int binder2 = 1;
        int binder3 = 2;
        Message.Dispatcher dispatcher = new Message.Dispatcher();

        dispatcher.AddListener<int>(binder1, msg1 => {
            Console.WriteLine($"binder1: {msg1}");
            dispatcher.AddListener<int>(binder2, msg2 => {
                Console.WriteLine($"binder1: {msg2}");
                dispatcher.AddListener<int>(binder3, msg3 => {
                    Console.WriteLine($"binder1: {msg3}");
                    dispatcher.RemoveListener<int>(binder3);
                });
                dispatcher.RemoveListener<int>(binder2);
            });
            dispatcher.RemoveListener<int>(binder1);
        });

        dispatcher.Send(10);
        dispatcher.Send(20);
        dispatcher.Send(30);
    }

    void Test3() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        Message.Dispatcher dispatcher = new Message.Dispatcher();
        dispatcher.AddListener<int>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
            dispatcher.Send(2.0f);
        });
        dispatcher.AddListener<float>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
            dispatcher.Send(3.0);
        });
        dispatcher.AddListener<double>(this, msg => {
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
            dispatcher.RemoveListener<int>(this);
            dispatcher.RemoveListener<float>(this);
            dispatcher.RemoveListener<double>(this);
        });
        dispatcher.Send(1);
        dispatcher.Send(10);
    }

    void Test4() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        Message.Dispatcher dispatcher = new Message.Dispatcher();
        dispatcher.AddListener<int>(this, msg => {
            dispatcher.RemoveListener<int>(this);
            Console.WriteLine($"{msg.GetType().Name}: {msg}");
            dispatcher.Send(2.0f);
        });
        dispatcher.AddListener<float>(this, msg => {
            dispatcher.RemoveListener<int>(this);
            Console.WriteLine($"A: {msg.GetType().Name}: {msg}");
        });
        dispatcher.AddListener<float>(this, msg => {
            Console.WriteLine($"B: {msg.GetType().Name}: {msg}");
        });
        dispatcher.Send(1);
        dispatcher.Send(2);
        dispatcher.Send(3.0f);
        dispatcher.Send(4.0f);
    }

    void Test5() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        Message.Dispatcher dispatcher = new Message.Dispatcher();

        string a = "a";
        string b = "b";
        string c = "c";
        string d = "d";

        dispatcher.AddListener<int>(a, msg => {
            Console.WriteLine($"{a}:{msg}");
        });
        dispatcher.AddListener<int>(b, msg => {
            Console.WriteLine($"{b}:{msg}");
        });
        dispatcher.AddListener<int>(c, msg => {
            Console.WriteLine($"{c}:{msg}");
        });
        dispatcher.AddListener<int>(d, msg => {
            dispatcher.RemoveListener<int>(b);
            dispatcher.RemoveListener<int>(d);
            Console.WriteLine($"{d}:{msg}");
        });
        dispatcher.Send(1);
        dispatcher.Send(2);
    }

    void Test6() {
        Console.WriteLine($"\n------ {MethodBase.GetCurrentMethod().Name} ------");
        Message.Dispatcher dispatcher = new Message.Dispatcher();
        dispatcher.AddListener<int>(this, msg => {
            Console.Write($"{msg.GetType().Name}: {msg}; ");
            dispatcher.Send((float)msg + 1);
        });
        dispatcher.AddListener<float>(this, msg => {
            if (msg > 500) {
                return;
            }
            Console.Write($"{msg.GetType().Name}: {msg}; ");
            dispatcher.Send((int)msg + 1);
        });
        dispatcher.Send(0);
        Console.WriteLine();
    }

    void Run() {
        Test1();
        Test2();
        Test3();
        Test4();
        Test5();
        // Test6();
    }

    public static void Main(string[] args) {
        new Test().Run();
    }

}
