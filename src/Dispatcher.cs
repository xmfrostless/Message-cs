/*
    https://github.com/xmfrostless/Message-CSharp
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Message;

public class Dispatcher {
    public void AddListener<T>(object binder, Action<T> callback) {
        if (binder == null) {
            MESSAGE_WARNING(Hash<T>.TYPE.Name, "The binder is null!");
            return;
        }
        if (callback == null) {
            MESSAGE_WARNING(Hash<T>.TYPE.Name, "Func is null!");
            return;
        }
        var messageCode = Hash<T>.TYPE_ID;
        if (_listenerMap.TryGetValue(messageCode, out var listeners)) {
            var exists = _removeIndexes.TryGetValue(messageCode, out var removeIndexes);
            for (var i = 0; i < listeners.Count; ++i) {
                if (listeners[i].binder.Equals(binder)) {
                    if (!exists || !removeIndexes.Contains(i)) {
                        MESSAGE_WARNING(Hash<T>.TYPE.Name, "The binder is exist!");
                        return;
                    }
                }
            }
        } else {
            listeners = new List<ListenerBase>();
            _listenerMap.Add(messageCode, listeners);
        }
        listeners.Add(new Listener<T>(binder, callback));
    }

    public void RemoveListener<T>(object binder) {
        if (binder == null) {
            MESSAGE_WARNING(Hash<T>.TYPE.Name, "The binder is null!");
            return;
        }
        var messageCode = Hash<T>.TYPE_ID;
        if (!_listenerMap.TryGetValue(messageCode, out var listeners)) {
            MESSAGE_WARNING(Hash<T>.TYPE.Name, "The listener list is empty!");
            return;
        }
        if (_invokeLevel == 0) {
            var index = listeners.FindIndex(value => value.binder.Equals(binder));
            if (index != -1) {
                listeners.RemoveAt(index);
                return;
            }
        } else {
            for (var i = 0; i < listeners.Count; ++i) {
                if (listeners[i].binder.Equals(binder)) {
                    if (_removeIndexes.TryGetValue(messageCode, out var removeIndexes)) {
                        if (removeIndexes.Add(i)) {
                            _shouldRemove = true;
                            return;
                        }
                    } else {
                        _removeIndexes.Add(messageCode, [i]);
                        _shouldRemove = true;
                        return;
                    }
                }
            }
        }
        MESSAGE_WARNING(Hash<T>.TYPE.Name, "The binder not found!");
    }

    public void Send<T>(T message) {
        var messageCode = Hash<T>.TYPE_ID;
        if (!_listenerMap.TryGetValue(messageCode, out var listeners)) {
            return;
        }
        if (listeners.Count == 0) {
            return;
        }

        MESSAGE_INVOKE_PUSH(Hash<T>.TYPE);
        ++_invokeLevel;
        var size = listeners.Count;
        var hasRemoveIndexes = _removeIndexes.TryGetValue(messageCode, out var removeIndexes);
        for (var i = 0; i < size; ++i) {
            if (hasRemoveIndexes && removeIndexes.Contains(i)) {
                continue;
            }
            (listeners[i] as Listener<T>).callback.Invoke(message);
        }
        --_invokeLevel;
        MESSAGE_INVOKE_POP();

        if (_invokeLevel == 0 && _shouldRemove) {
            foreach (var item in _removeIndexes) {
                if (item.Value.Count == 0) {
                    continue;
                }
                if (_listenerMap.TryGetValue(item.Key, out var tempList)) {
                    if (tempList.Count != 0) {
                        var tail = tempList.Count;
                        foreach (var index in item.Value) {
                            --tail;
                            (tempList[index], tempList[tail]) = (tempList[tail], tempList[index]);
                        }
                        tempList.RemoveRange(tail, tempList.Count - tail);
                    }
                }
                item.Value.Clear();
            }
            _shouldRemove = false;
        }
    }

    private static class Hash<T> {
        public static readonly Type TYPE = typeof(T);
        public static readonly Guid TYPE_ID = TYPE.GUID;
    }

    private class ListenerBase {
        public object binder { get; protected set; }
    }
    private sealed class Listener<T> : ListenerBase {
        public Action<T> callback { get; private set; }
        public Listener(object key, Action<T> func) {
            binder = key;
            callback = func;
        }
    }

    private Dictionary<Guid, List<ListenerBase>> _listenerMap = [];
    private Dictionary<Guid, HashSet<int>> _removeIndexes = [];
    private int _invokeLevel = 0;
    private bool _shouldRemove = false;

#if DEBUG
    private Stack<Type> _invokeStack = [];
    private void MESSAGE_WARNING(string info, string detail, [CallerMemberName] string funcName = "") {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Error.WriteLine($"[Message] Warn: {funcName} / {info} / {detail}");
        Console.ResetColor();
    }
    private void MESSAGE_ERROR(string info, string detail, [CallerMemberName] string funcName = "") {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Error.WriteLine($"[Message] Error: {funcName} / {info} / {detail}");
        Console.ResetColor();
    }
    private void MESSAGE_ASSERT(string info, string detail, [CallerMemberName] string funcName = "") {
        MESSAGE_ERROR(info, detail, funcName);
        Debug.Assert(false);
    }
    private void MESSAGE_INVOKE_PUSH(Type cur, [CallerMemberName] string funcName = "") {
        if (_invokeStack.Count > 200) {
            MESSAGE_ERROR(cur.Name, "The number of message recursion exceeds the upper limit");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in _invokeStack) {
                Console.Error.Write($"{item.Name} -> ");
            }
            Console.Error.WriteLine($"(*){cur.Name}");
            Console.ResetColor();
            Debug.Assert(false);
        }
        _invokeStack.Push(cur);
    }
    private void MESSAGE_INVOKE_POP() {
        _invokeStack.Pop();
    }
#else
	[Conditional("DEBUG")]
	private void MESSAGE_WARNING(string info, string detail, [CallerMemberName] string funcName = "") { }
	[Conditional("DEBUG")]
	private void MESSAGE_ERROR(string info, string detail, [CallerMemberName] string funcName = "") { }
	[Conditional("DEBUG")]
	private void MESSAGE_ASSERT(string info, string detail, [CallerMemberName] string funcName = "") { }
	[Conditional("DEBUG")]
	private void MESSAGE_INVOKE_PUSH(Type cur, [CallerMemberName] string funcName = "") { }
	[Conditional("DEBUG")]
	private void MESSAGE_INVOKE_POP() { }
#endif
}