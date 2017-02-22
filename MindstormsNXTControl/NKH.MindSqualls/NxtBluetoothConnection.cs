using System.IO.Ports;
// using System; // DELETE

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the bluetooth protocol.</para>
    /// </summary>
    public class NxtBluetoothConnection: NxtCommunicationProtocol
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="serialPortName">The COM port used by the Bluetooth link</param>
        public NxtBluetoothConnection(string serialPortName)
        {
            this.serialPortName = serialPortName;
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="serialPortNo">The COM port used by the Bluetooth link</param>
        public NxtBluetoothConnection(byte serialPortNo)
        {
            this.serialPortName = string.Format("COM{0}", serialPortNo);
        }

        #region Serial port.

        /// <summary>
        /// <para>The name of the serial port, e.g. COM40.</para>
        /// </summary>
        private string serialPortName;

        /// <summary>
        /// <para>The serial port used by the bluetooth connection.</para>
        /// </summary>
        private SerialPort serialPort = null;

        /// <summary>
        /// <para>Object to control mutex locking on the serial port.</para>
        /// </summary>
        private object serialPortLock = new object();

        #endregion

        #region Bluetooth connection.

        /// <summary>
        /// <para>Connect to the NXT brick via bluetooth.</para>
        /// </summary>
        public override void Connect()
        {
            TraceUtil.MethodEnter();

            lock (serialPortLock)
            {
                if (!IsConnected)
                {
                    TraceUtil.Note("Not connected");

                    // Dispose of the serialPort if it isn't null.
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                        serialPort = null;
                    }

                    TraceUtil.Note("Initializing serialPort");
                    // Initialize the COM port.
                    serialPort = new SerialPort(serialPortName);
                    serialPort.Open();

                    // Set timeouts - 5000ms.
                    serialPort.WriteTimeout = 5000;
                    serialPort.ReadTimeout = 5000;
                }
            }

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>Disconnect from the brick.</para>
        /// </summary>
        public override void Disconnect()
        {
            TraceUtil.MethodEnter();

            lock (serialPortLock)
            {
                if (IsConnected)
                {
                    TraceUtil.Note("Call serialPort.Close");
                    serialPort.Close();
                }

                if (serialPort != null)
                {
                    TraceUtil.Note("Call serialPort.Dispose");
                    serialPort.Dispose();
                    serialPort = null;
                }
            }

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>Indicates if connected to the NXT brick.</para>
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                TraceUtil.MethodVisit();

                try
                {
                    return
                        serialPort != null &&
                        serialPort.IsOpen &&
                        serialPort.CtsHolding;  // Necessary, or a NXT that is turned of will report as Connected!
                }
                catch (System.ObjectDisposedException)
                {
                    return false;
                }
            }
        }

        #endregion

        /// <summary>
        /// <para>Send a request for the NXT brick, and if applicable, receive the reply.</para>
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The reply as a byte-array, or null</returns>
        protected override byte[] Send(byte[] request)
        {
            TraceUtil.MethodEnter();

            lock (serialPortLock)
            {
                if (!IsConnected)
                {
                    TraceUtil.MethodExit("throw new NxtConnectionException");
                    throw new NxtConnectionException("Can't send message: Not connected.");
                }

                int length = request.Length;

                // Create a Bluetooth request.
                byte[] btRequest = new byte[request.Length + 2];
                btRequest[0] = (byte) (length & 0xFF);
                btRequest[1] = (byte) ((length & 0xFF00) >> 8);
                request.CopyTo(btRequest, 2);

                // Write the request to the NXT brick.
                serialPort.Write(btRequest, 0, btRequest.Length);

                // 0x80 indicate that we should expect a reply.
                if ((request[0] & 0x80) == 0)
                {
                    int lsb = serialPort.ReadByte();
                    int msb = serialPort.ReadByte();
                    length = msb * 256 + lsb;

                    byte[] reply = new byte[length];
                    serialPort.Read(reply, 0, length);

                    NxtCommand command = (NxtCommand) request[1];
                    NxtCommand commandEcho = (NxtCommand) reply[1];

                    if (reply[0] != 0x02)
                    {
                        TraceUtil.MethodExit("throw new NxtCommunicationProtocolException - 1");
                        throw new NxtCommunicationProtocolException(command, NxtErrorMessage.UndefinedError, string.Format("Expected a Reply Command byte (0x02); Got 0x{0:X2} instead.", reply[0]));
                    }

                    if (commandEcho != command)
                    {
                        TraceUtil.MethodExit("throw new NxtCommunicationProtocolException - 2");
                        throw new NxtCommunicationProtocolException(command, NxtErrorMessage.UndefinedError, string.Format("Expected a matching return Command byte (0x{0:X2}); Got 0x{1:X2} instead.", command, commandEcho));
                    }

                    NxtErrorMessage statusByte = (NxtErrorMessage) reply[2];
                    if (statusByte != NxtErrorMessage.Succes)
                    {
                        string errorText = "The Status byte indicates an error: Request:";
                        foreach (byte requestByte in request)
                            errorText += string.Format(" 0x{0:X2}", requestByte);

                        errorText += "; Reply:";
                        foreach (byte replyByte in reply)
                            errorText += string.Format(" 0x{0:X2}", replyByte);

                        TraceUtil.MethodExit("throw new NxtCommunicationProtocolException - 3");
                        throw new NxtCommunicationProtocolException(command, statusByte, errorText);
                    }

                    TraceUtil.MethodExit("return reply");
                    return reply;
                }
                else
                {
                    TraceUtil.MethodExit("return null");
                    return null;
                }
            }
        }
    }
}