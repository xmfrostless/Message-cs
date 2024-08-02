/*
	By Jenocn
	https://jenocn.github.io/
*/

using System.Collections.Generic;

public static class MessageCenter {
	private static MessageDispatcher _messageDispatcher = new MessageDispatcher();

	public static void AddListener<T>(object obj, System.Action<T> func) where T : MessageBase<T> {
		_messageDispatcher.AddListener(obj, func);
	}

	public static void RemoveListener<T>(object obj) where T : MessageBase<T> {
		_messageDispatcher.RemoveListener<T>(obj);
	}

	public static void Send(IMessage message) {
		_messageDispatcher.Send(message);
	}

	public static void Clear() {
		_messageDispatcher.Clear();
	}
}