using LitJson;
using System.Text;
using System.Security.Cryptography;

public abstract class AbstractTransferObject {

    public static int SEQ_ID = 0;

	public AbstractTransferObject(Command command) {
		this.msgType = command;
	}

	public Command msgType {
		get; private set;
	}
	
	public System.Object msgData {
		get; private set;
	}

    public void writeJsonStart(JsonWriter writer) {
        writer.WriteObjectStart();

        writer.WritePropertyName("seqID");
        writer.Write(SEQ_ID);

        writer.WritePropertyName("appMsg");
        writer.WriteObjectStart();

        writer.WritePropertyName("playerID");
        writer.Write("Unity");

        writer.WritePropertyName("msgType");
        writer.Write((int)msgType);
    }

    public string writeJsonEnd(StringBuilder builder, JsonWriter writer)
    {
        writer.WriteObjectEnd();
        writer.WriteObjectEnd();

        string temp = builder.ToString();

        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] textToHash = Encoding.Default.GetBytes(temp);
        byte[] result = md5.ComputeHash(textToHash);
        string hash = System.BitConverter.ToString(result);

        temp = temp.Remove(temp.Length - 1);

        return temp + ", \"checkSum\": \"" + hash + "\"}";

    }

    public abstract string toJson();

}