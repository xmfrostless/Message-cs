/*
	By Jenocn
	https://jenocn.github.io/
*/

public sealed class MessageTypeId<T> {
	public static readonly int id;
	static MessageTypeId() {
		id = typeof(T).GUID.GetHashCode();
	}
};