/*
	By Jenocn
	https://jenocn.github.io/
*/

public interface IMessage {
	int GetMessageID();
}

public class MessageBase<T> : IMessage where T : MessageBase<T> {
	public int GetMessageID() {
		return MessageTypeId<T>.id;
	}
}