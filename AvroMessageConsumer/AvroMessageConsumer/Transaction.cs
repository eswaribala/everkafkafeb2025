using Avro.Specific;
using Avro;

namespace AvroMessageConsumer
{
    public class Transaction : ISpecificRecord
    {
        public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"Transaction\",\"fields\":[{\"name\":\"id\",\"type\":\"string\"},{\"name\":\"amount\",\"type\":\"double\"}]}");

        public string id { get; set; }
        public double amount { get; set; }

        public Schema Schema => _SCHEMA;

        public object Get(int fieldPos)
        {
            return fieldPos switch
            {
                0 => id,
                1 => amount,
                _ => throw new AvroRuntimeException("Bad index")
            };
        }

        public void Put(int fieldPos, object value)
        {
            switch (fieldPos)
            {
                case 0: id = (string)value; break;
                case 1: amount = (double)value; break;
                default: throw new AvroRuntimeException("Bad index");
            }
        }
    }
}