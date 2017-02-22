using System;
using System.Text;
using System.Threading;

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Abstract class representing a Digital sensor, i.e. a sensor using the I<sup>2</sup>C protocol.</para>
    /// </summary>
    /// <remarks>
    /// <para>The "Device Memory Arrangement" for a Digital sensor is specified at p. 10 of the document:</para>
    /// <para>LEGO MINDSTORMS NXT,<br/>
    /// Hardware Developer Kit</para>
    /// <para>which can be downloaded at:<br/>
    /// <a href="http://mindstorms.lego.com/overview/NXTreme.aspx" target="_blank">http://mindstorms.lego.com/overview/NXTreme.aspx</a></para>
    /// <para>The Ultrasonic sensor protocol is specifed in appendix 7 of the HDK.</para>
    /// <para>"Digital I/O Communication Methods" is also explained in the Executable File Specification document, p. 70-72.</para>
    /// <para>.oOo.</para>
    /// <para>According to the LEGO MINDSTORMS NXT Hardware Developer Kit p. 7, sensors is divided into three types: active sensors (e.g. Robotics Invention Systems sensors), passive sensors (e.g. the NXT touch, light, and sound sensors), and digital sensors (e.g. the NXT ultrasonic sensor and the HiTechnic compass sensor). The three abstract classes NxtActiveSensor, NxtPassiveSensor, and NxtDigitalSensor reflect this.</para>
    /// </remarks>
    /// <seealso cref="NxtActiveSensor"/>
    /// <seealso cref="NxtPassiveSensor"/>
    public abstract class NxtDigitalSensor: NxtSensor
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtDigitalSensor()
            : base(NxtSensorType.LOWSPEED_9V, NxtSensorMode.RAWMODE)
        {
        }

        /// <summary>
        /// <para>Initializes the digital sensor.</para>
        /// </summary>
        internal override void InitSensor()
        {
            if (Brick.IsConnected)
            {
                base.InitSensor();

                // Clear any garbage bytes from the digital sensor.
                byte? bytesReady = Brick.CommLink.LsGetStatus(sensorPort);
                byte[] garbage = (bytesReady > 0) ? Brick.CommLink.LsRead(sensorPort) : null;
            }
        }

        /// <summary>
        /// <para>Sends an I<sup>2</sup>C request to the Ultrasonic sensor, and receive the reply if applicable.</para>
        /// </summary>
        /// <param name="rxDataLength">Length of the expected reply</param>
        /// <param name="request">The I2C request</param>
        /// <returns>The reply from the sensor, or a null-value</returns>
        internal byte[] Send(byte rxDataLength, byte[] request)
        {
            TraceUtil.MethodEnter();

            // Send the I2C request to the sensor.
            Brick.CommLink.LsWrite(sensorPort, rxDataLength, request);

            // Return null if no reply is expected.
            if (rxDataLength == 0)
            {
                TraceUtil.MethodExit("return null - 1");
                return null;
            }

            // Wait until the reply is ready in the sensor.
            byte? bytesReady = 0;
            do
            {
                try
                {
                    // Query how many bytes are ready in the sensor.
                    bytesReady = Brick.CommLink.LsGetStatus(sensorPort);
                }
                catch (NxtCommunicationProtocolException ex)
                {
                    // An error has occured.

                    // The port is still busy. Try again.
                    if (ex.errorMessage == NxtErrorMessage.PendingCommunicationTransactionInProgress)
                    {
                        bytesReady = 0;
                        Thread.Sleep(10);

                        TraceUtil.Note("continue");
                        continue;
                    }

                    // Rethrow if the error is not a CommunicationBusError.
                    if (ex.errorMessage != NxtErrorMessage.CommunicationBusError)
                    {
                        TraceUtil.MethodExit("throw");
                        throw;
                    }

                    // Clears error condition - any LsWrite should do.
                    DoAnyLsWrite();

                    // Exit Send().
                    TraceUtil.MethodExit("return null - 2");
                    return null;
                }
            }
            while (bytesReady < rxDataLength);

            // Read, and return, the reply from the sensor.
            TraceUtil.MethodExit("Brick.CommLink.LsRead(sensorPort)");
            return Brick.CommLink.LsRead(sensorPort);
        }

        #region I2C protocol.

        // WAS: protected abstract void DoAnyLsWrite();
        private void DoAnyLsWrite()
        {
            ReadByteFromAddress(0x42);
        }

        /// <summary>
        /// <para>Both the Ultrasonic sensor and the Compass plus Color sensors from HiTecnic uses 0x02 as the device address. However the documentation do not put any limits to this value.</para>
        /// </summary>
        internal byte deviceAddress = 0x02;

        #region Constants.

        /// <summary>
        /// <para>Returns the version of the sensor.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public string ReadVersion()
        {
            byte[] request = new byte[] { deviceAddress, 0x00 };
            byte[] reply = Send(8, request);

            return (reply != null)
                ? Encoding.ASCII.GetString(reply, 0, reply.Length).TrimEnd('\0', '?', ' ')
                : null;
        }

        /// <summary>
        /// <para>Returns the name of the manufacturer, e.g. "LEGO".</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public string ReadProductId()
        {
            byte[] request = new byte[] { deviceAddress, 0x08 };
            byte[] reply = Send(8, request);

            return (reply != null)
                ? Encoding.ASCII.GetString(reply, 0, reply.Length).TrimEnd('\0', '?')
                : null;
        }

        /// <summary>
        /// <para>Returns the sensor type, e.g. "Sonar".</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public string ReadSensorType()
        {
            byte[] request = new byte[] { deviceAddress, 0x10 };
            byte[] reply = Send(8, request);

            return (reply != null)
                ? Encoding.ASCII.GetString(reply, 0, reply.Length).TrimEnd('\0', '?', ' ')
                : null;
        }

        #endregion

        #region Variables.
        #endregion

        #region Commands.
        #endregion

        #region Utilities.

        /// <summary>
        /// <para>Reads the value of the variable stored at the address.</para>
        /// </summary>
        /// <param name="address">The address of the variable</param>
        /// <returns>The value of the variable</returns>
        internal byte? ReadByteFromAddress(byte address)
        {
            byte[] request = new byte[] { deviceAddress, address };
            byte[] reply = Send(1, request);

            if (reply != null && reply.Length >= 1)
                return reply[0];
            else
                return null;
        }

        /// <summary>
        /// <para>Reads the value of the variable stored at the two consecutive bytes at the address.</para>
        /// </summary>
        /// <param name="address">The address of the variable</param>
        /// <returns>The value of the variable</returns>
        internal UInt16? ReadWordFromAdress(byte address)
        {
            byte[] request = new byte[] { deviceAddress, address };
            byte[] reply = Send(2, request);

            if (reply != null && reply.Length >= 2)
                return Util.GetUInt16(reply, 0);
            else
                return null;
        }

        internal void CommandToAddress(byte address, byte command)
        {
            byte[] request = new byte[] { deviceAddress, address, command };
            Send(0, request);
        }

        #endregion

        #endregion
    }
}