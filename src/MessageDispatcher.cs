/*
	By Jenocn
	https://jenocn.github.io/
*/

using System.Collections.Generic;
using System.Diagnostics;

public class MessageDispatcher {
	private Dictionary<int, Dictionary<int, IMessageListener>> _listenerDict = new Dictionary<int, Dictionary<int, IMessageListener>>();
	private HashSet<int> _invokePool = new HashSet<int>();
	private LinkedList<Tuple<int, int, IMessageListener?>> _listenerCacheQueue = new LinkedList<Tuple<int, int, IMessageListener?>>();

	public void AddListener<T>(object sender, System.Action<T> func) where T : MessageBase<T> {
		if (sender == null) {
			Debug.Fail("MessageDispatcher.AddListener: Sender is null!");
			return;
		}
		if (func == null) {
			Debug.Fail("MessageDispatcher.AddListener: Func is null!");
			return;
		}
		var messageId = MessageTypeId<T>.id;
		var senderKey = sender.GetHashCode();
		if (_invokePool.Count > 0) {
			_listenerCacheQueue.AddLast(new Tuple<int, int, IMessageListener?>(messageId, senderKey, new MessageListener<T>(func)));
			return;
		}

		if (_listenerDict.TryGetValue(messageId, out var dict)) {
			if (dict.ContainsKey(senderKey)) {
				Debug.Fail("MessageDispatcher.AddListener: The sender is exist!");
				return;
			}
		} else {
			dict = new Dictionary<int, IMessageListener>();
			_listenerDict.Add(messageId, dict);
		}
		dict.Add(senderKey, new MessageListener<T>(func));
	}

	public void RemoveListener<T>(object sender) where T : MessageBase<T> {
		if (sender == null) {
			Debug.Fail("MessageDispatcher.RemoveListener: Sender is null!");
			return;
		}
		var messageId = MessageTypeId<T>.id;
		var senderKey = sender.GetHashCode();
		if (_invokePool.Count > 0) {
			_listenerCacheQueue.AddLast(new Tuple<int, int, IMessageListener?>(messageId, senderKey, null));
			return;
		}
		_RemoveListener(messageId, senderKey);
	}

	public void Send(IMessage message) {
		if (message == null) {
			Debug.Fail("MessageDispatcher.Send: Message is null!");
			return;
		}

		var messageId = message.GetMessageID();
		if (!_listenerDict.TryGetValue(messageId, out var listenerDict) || listenerDict.Count == 0) {
			return;
		}

		if (_invokePool.Contains(messageId)) {
			Debug.Fail("MessageDispatcher.Send: Message is recursive send! -> " + message.GetType().FullName);
			return;
		}

		_invokePool.Add(messageId);
		foreach (var item in listenerDict) {
			item.Value.Invoke(message);
		}
		_invokePool.Remove(messageId);

		if (_invokePool.Count > 0 || _listenerCacheQueue.Count == 0) {
			return;
		}

		foreach (var item in _listenerCacheQueue) {
			if (item.Item3 != null) {
				_AddListener(item.Item1, item.Item2, item.Item3);
			} else {
				_RemoveListener(item.Item1, item.Item2);
			}
		}
		_listenerCacheQueue.Clear();
	}

	public void Clear() {
		if (_invokePool.Count > 0) {
			Debug.Fail("MessageDispatcher.Clear: Cannot perform this operation while processing the message.");
			return;
		}
		_listenerDict.Clear();
	}

	private void _RemoveListener(int messageId, int senderKey) {
		if (_listenerDict.TryGetValue(messageId, out var dict)) {
			dict.Remove(senderKey);
		}
	}

	private void _AddListener(int messageId, int senderKey, IMessageListener listener) {
		if (_listenerDict.TryGetValue(messageId, out var dict)) {
			if (dict.ContainsKey(senderKey)) {
				Debug.Fail("MessageDispatcher.AddListener: The sender is exist!");
				return;
			}
		} else {
			dict = new Dictionary<int, IMessageListener>();
			_listenerDict.Add(messageId, dict);
		}
		dict.Add(senderKey, listener);
	}
}