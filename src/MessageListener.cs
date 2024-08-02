/*
	By Jenocn
	https://jenocn.github.io/
*/

public interface IMessageListener {
	void Invoke(IMessage message);
}

public sealed class MessageListener<T> : IMessageListener where T : MessageBase<T> {
	private System.Action<T> _func = null!;
	public MessageListener(System.Action<T> func) {
		_func = func;
	}

	public void Invoke(IMessage message) {
		_func.Invoke((T)message);
	}
}