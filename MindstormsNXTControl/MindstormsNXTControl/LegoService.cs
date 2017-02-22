using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MindstormsNXTControl
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LegoService" in both code and config file together.
    public class LegoService : ILegoService
    {
        public void Forward()
        {
            StaticVariables.MoveForward();
        }

        public void Stop()
        {
            StaticVariables.Stop();
        }

        public void Back()
        {
            StaticVariables.MoveBack();
        }

        public void Left()
        {
            StaticVariables.MoveLeft();
        }

        public void Right()
        {
            StaticVariables.MoveRight();
        }
    }
}
