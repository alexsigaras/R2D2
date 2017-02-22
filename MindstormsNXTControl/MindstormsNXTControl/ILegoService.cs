using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MindstormsNXTControl
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILegoService" in both code and config file together.
    [ServiceContract]
    public interface ILegoService
    {
        [OperationContract]
        void Forward();

        [OperationContract]
        void Back();

        [OperationContract]
        void Left();

        [OperationContract]
        void Right();


        [OperationContract]
        void Stop();
    }
}
