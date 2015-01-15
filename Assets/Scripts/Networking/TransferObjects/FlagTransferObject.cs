using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class FlagTransferObject : AbstractTransferObject
{
    private bool ack;
    private bool close;
    private bool error;

    public FlagTransferObject(bool ack, bool close, bool error, int seqID) : base(Command.NoCommand)
    {
        this.ack = ack;
        this.close = close;
        this.error = error;
        this.seqID = seqID;
    }

    public override string toJson()
    {
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);

        writeJsonStart(writer);

        writer.WritePropertyName("flags");
        writer.WriteObjectStart();

        writer.WritePropertyName("ack");
        writer.Write(ack);

        writer.WritePropertyName("close");
        writer.Write(close);

        writer.WritePropertyName("error");
        writer.Write(error);

        writer.WriteObjectEnd();

        return writeJsonEnd(sb, writer);
    }
        
}
