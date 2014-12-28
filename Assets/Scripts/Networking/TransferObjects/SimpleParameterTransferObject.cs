using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

    class SimpleParameterTransferObject : AbstractTransferObject
    {

        private Dictionary<String, System.Object> parameters;
        private bool parametersExist;

        public SimpleParameterTransferObject(Command cmd, Dictionary<String, System.Object> parameters) 
            : base(cmd)
        {
            this.parameters = parameters;
        }

        public override string toJson()
        {

            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            writeJsonStart(writer);

            if (parameters != null && parameters.Count != 0)
            {
                foreach (String s in parameters.Keys)
                {
                    writer.WritePropertyName(s.ToString());

                    if (parameters[s] is int)
                    {
                        writer.Write((int)parameters[s]);
                    }
                    else
                    {
                        writer.Write(parameters[s].ToString());
                    }
                }
            }


            return writeJsonEnd(sb, writer);
        }

    }