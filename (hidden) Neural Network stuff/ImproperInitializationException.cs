using System;
using System.Collections.Generic;
using System.Text;

namespace TBNeuralNetwork
{
    [Serializable]
    public class ImproperInitializationException : Exception
    {
        public ImproperInitializationException()
        {

        }
        public ImproperInitializationException(string message) : base(message)
        {

        }
        public ImproperInitializationException(string message, Exception inner) : base(message, inner)
        {

        }
        protected ImproperInitializationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }
}
