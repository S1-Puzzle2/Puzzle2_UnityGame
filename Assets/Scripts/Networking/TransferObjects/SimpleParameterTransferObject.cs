using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

    class SimpleParameterTransferObject : AbstractTransferObject
    {

        private Dictionary<String, System.Object> parameters;
        private bool parametersExist;
        private string clientID;

        public SimpleParameterTransferObject(Command cmd, string clientID, Dictionary<String, System.Object> parameters) 
            : base(cmd)
        {
            this.parameters = parameters;
            this.clientID = clientID;
        }

        public override string toJson()
        {

            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            writeJsonStart(writer);

            writer.WritePropertyName("appMsg");
            StringBuilder appMsgSb = new StringBuilder();
            JsonWriter appMsgWriter = new JsonWriter(appMsgSb);

            appMsgWriter.WriteObjectStart();

            appMsgWriter.WritePropertyName("clientID");
            appMsgWriter.Write(clientID);

            appMsgWriter.WritePropertyName("msgType");
            appMsgWriter.Write(CommandMethods.getString(msgType));

            appMsgWriter.WritePropertyName("msgData");
            appMsgWriter.WriteObjectStart();

            appMsgWriter.WritePropertyName("clientType");
            appMsgWriter.Write("Unity");

            if (parameters != null && parameters.Count != 0)
            {
                foreach (String s in parameters.Keys)
                {
                    appMsgWriter.WritePropertyName(s.ToString());

                    if (parameters[s] is int)
                    {
                        appMsgWriter.Write((int)parameters[s]);
                    }
                    else
                    {
                        appMsgWriter.Write(parameters[s].ToString());
                    }
                }
            }

            appMsgWriter.WriteObjectEnd();
            appMsgWriter.WriteObjectEnd();
            string appMsgText = Base64.Base64Encode(appMsgSb.ToString());
            writer.Write(appMsgText);

            return writeJsonEnd(sb, writer);
        }

    }