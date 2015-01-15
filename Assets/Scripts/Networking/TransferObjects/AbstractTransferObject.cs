using LitJson;
using System.Text;
using System.Security.Cryptography;
using System;
using Vbaccelerator.Components.Algorithms;

public abstract class AbstractTransferObject {

    public static int SEQ_ID = 0;
    public int seqID;

	public AbstractTransferObject(Command command) {
		this.msgType = command;
        seqID = ++SEQ_ID;
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
        writer.Write(seqID);

    }

    public string writeJsonEnd(StringBuilder builder, JsonWriter writer)
    {
        writer.WriteObjectEnd();

        string temp = builder.ToString();
        temp = temp.Replace(@"\", "");

        byte[] textToHash = Encoding.UTF8.GetBytes(temp);
        long crcVal = CRC32.calcCrc32(textToHash);

        temp = temp.Remove(temp.Length - 1);

        return temp + ", \"checkSum\": " + crcVal.ToString() + "}\0";

    }

    public abstract string toJson();

}