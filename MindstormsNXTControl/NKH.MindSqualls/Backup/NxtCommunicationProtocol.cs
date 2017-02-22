using System;
using System.Text;

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Abstract class representing the NXT communication protocol.</para>
    /// </summary>
    /// <remarks>
    /// <para>The communication protocol for the NXT brick is specified in the documents:</para>
    /// 
    /// <para>Lego Mindstorms NXT,<br/>
    /// Bluetooth Developer Kit,<br/>
    /// Appendix 1: LEGO MINDSTORMS NXT Communication protocol<br/>
    /// - and:<br/>
    /// Appendix 2: LEGO MINDSTORMS NXT Direct commands</para>
    /// 
    /// <para>- which can be downloaded at:<br/>
    /// <a href="http://mindstorms.lego.com/overview/NXTreme.aspx" target="_blank">http://mindstorms.lego.com/overview/NXTreme.aspx</a></para>
    /// 
    /// <para>A special thanks goes to Bram Fokke for his NXT# project:<br/>
    /// <a href="http://nxtsharp.fokke.net/" target="_blank">http://nxtsharp.fokke.net/</a></para>
    /// 
    /// <para>Without his work, I doubt very much if my own work would have gotten off the ground. I have even st... ehem... borrowed a bit of his code here and there. However the major part of the code is my own.</para>
    /// </remarks>
    public abstract class NxtCommunicationProtocol
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtCommunicationProtocol()
            : base()
        { }

        #region Communication.

        /// <summary>
        /// <para>Flag indicating if a reply should always be recieved.</para>
        /// </summary>
        public bool ReplyRequired = false;

        /// <summary>
        /// <para>Connect to the NXT brick.</para>
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// <para>Disconnect from the NXT brick.</para>
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// <para>Returns a boolean indicating if the NXT brick is connected.</para>
        /// </summary>
        public abstract bool IsConnected
        {
            get;
        }

        /// <summary>
        /// <para>Send a request to the NXT brick, and return a reply.</para>
        /// </summary>
        /// <param name="request">The request to the NXT brick</param>
        /// <returns>The reply from the NXT brick, or null</returns>
        protected abstract byte[] Send(byte[] request);

        #endregion

        #region Protocol.

        /// <summary>
        /// <para>STARTPROGRAM</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 5.</para>
        /// </remarks>
        /// <param name="fileName">File name</param>
        public void StartProgram(string fileName)
        {
            ValidateFilename(fileName);

            byte[] fileNameByteArr = Encoding.ASCII.GetBytes(fileName);

            byte[] request = new byte[22];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.StartProgram;
            fileNameByteArr.CopyTo(request, 2);

            Send(request);
        }

        /// <summary>
        /// <para>STOPPROGRAM</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 5.</para>
        /// </remarks>
        public void StopProgram()
        {
            byte[] request = new byte[] {
                (byte) (ReplyRequired ? 0x00 : 0x80),
                (byte) NxtCommand.StopProgram
            };

            Send(request);
        }

        /// <summary>
        /// <para>PLAYSOUNDFILE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 5.</para>
        /// </remarks>
        /// <param name="loop">Loop sound file indefinately?</param>
        /// <param name="fileName">File name</param>
        public void PlaySoundfile(bool loop, string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[23];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.PlaySoundfile;
            request[2] = (byte) (loop ? 0xFF : 0x00);
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 3);

            Send(request);
        }

        /// <summary>
        /// <para>PLAYTONE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 6.</para>
        /// </remarks>
        /// <param name="frequency">Frequency for the tone, Hz</param>
        /// <param name="duration">Duration of the tone, ms</param>
        public void PlayTone(UInt16 frequency, UInt16 duration)
        {
            if (frequency < 200) frequency = 200;
            if (frequency > 14000) frequency = 14000;

            byte[] request = new byte[6];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.PlayTone;
            Util.SetUInt16(frequency, request, 2);
            Util.SetUInt16(duration, request, 4);

            Send(request);
        }

        /// <summary>
        /// <para>SETOUTPUTSTATE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 6.</para>
        /// </remarks>
        /// <param name="motorPort">Motor port</param>
        /// <param name="power">Power set point</param>
        /// <param name="mode">Mode</param>
        /// <param name="regulationMode">Regulation mode</param>
        /// <param name="turnRatio">Turn ratio</param>
        /// <param name="runState">Run state</param>
        /// <param name="tachoLimit">Tacho limit, 0: run forever</param>
        public void SetOutputState(NxtMotorPort motorPort, sbyte power, NxtMotorMode mode, NxtMotorRegulationMode regulationMode, sbyte turnRatio, NxtMotorRunState runState, UInt32 tachoLimit)
        {
            TraceUtil.MethodEnter();

            if (power < -100) power = -100;
            if (power > 100) power = 100;

            if (turnRatio < -100) turnRatio = -100;
            if (turnRatio > 100) turnRatio = 100;

            byte[] request = new byte[12];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.SetOutputState;
            request[2] = (byte) motorPort;
            request[3] = (byte) power;
            request[4] = (byte) mode;
            request[5] = (byte) regulationMode;
            request[6] = (byte) turnRatio;
            request[7] = (byte) runState;
            Util.SetUInt32(tachoLimit, request, 8);

            Send(request);

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>SETINPUTMODE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 7.</para>
        /// </remarks>
        /// <param name="sensorPort">Input Port</param>
        /// <param name="sensorType">Sensor Type</param>
        /// <param name="sensorMode">Sensor Mode</param>
        public void SetInputMode(NxtSensorPort sensorPort, NxtSensorType sensorType, NxtSensorMode sensorMode)
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                (byte) (ReplyRequired ? 0x00 : 0x80),
                (byte) NxtCommand.SetInputMode,
                (byte) sensorPort,
                (byte) sensorType,
                (byte) sensorMode
            };

            Send(request);

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>GETOUTPUTSTATE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 8.</para>
        /// </remarks>
        /// <param name="motorPort">Ourput Port</param>
        /// <returns>Returns a parsed NxtGetOutputStateReply with the reply</returns>
        public NxtGetOutputStateReply? GetOutputState(NxtMotorPort motorPort)
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.GetOutputState,
                (byte) motorPort
            };

            byte[] reply = Send(request);

            if (reply == null)
            {
                TraceUtil.MethodExit("return null");
                return null;
            }

            byte motorPortOut = reply[3];
            if (motorPortOut != (byte) motorPort)
            {
                TraceUtil.MethodExit("throw new NxtException");
                throw new NxtException(string.Format("Output motor port, {0}, was different from input motor port, {1}.", motorPortOut, motorPort));
            }

            NxtGetOutputStateReply result;
            result.power = (sbyte) reply[4];
            result.mode = (NxtMotorMode) reply[5];
            result.regulationMode = (NxtMotorRegulationMode) reply[6];
            result.turnRatio = (sbyte) reply[7];
            result.runState = (NxtMotorRunState) reply[8];
            result.tachoLimit = Util.GetUInt32(reply, 9);
            result.tachoCount = Util.GetInt32(reply, 13);
            result.blockTachoCount = Util.GetInt32(reply, 17);
            result.rotationCount = Util.GetInt32(reply, 21);

            TraceUtil.MethodExit("return result");
            return result;
        }

        /// <summary>
        /// <para>GETINPUTVALUES</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 8.</para>
        /// <para>Used with passive sensors like the Touch, Light and Sound sensors.</para>
        /// </remarks>
        /// <param name="sensorPort">Input Port</param>
        /// <returns>Returns a NxtGetInputValues with the parsed reply</returns>
        public NxtGetInputValuesReply? GetInputValues(NxtSensorPort sensorPort)
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.GetInputValues,
                (byte) sensorPort
            };

            byte[] reply = Send(request);

            if (reply == null)
            {
                TraceUtil.MethodExit("return null");
                return null;
            }

            byte sensorPortOut = reply[3];
            if (sensorPortOut != (byte) sensorPort)
            {
                TraceUtil.MethodExit("throw new NxtException");
                throw new NxtException(string.Format("Output sensor port, {0}, was different from input sensor port, {1}.", sensorPortOut, sensorPort));
            }

            NxtGetInputValuesReply result;
            result.valid = (reply[4] != 0x00);
            result.calibrated = (reply[5] != 0x00);
            result.sensorType = (NxtSensorType) reply[6];
            result.sensorMode = (NxtSensorMode) reply[7];
            result.rawAdValue = Util.GetUInt16(reply, 8);
            result.normalizedAdValue = Util.GetUInt16(reply, 10);
            result.scaledValue = Util.GetInt16(reply, 12);
            result.calibratedValue = Util.GetInt16(reply, 14);

            TraceUtil.MethodExit("return result");
            return result;
        }

        /// <summary>
        /// <para>RESETINPUTSCALEDVALUE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 8.</para>
        /// </remarks>
        /// <param name="sensorPort">Input Port</param>
        public void ResetInputScaledValue(NxtSensorPort sensorPort)
        {
            byte[] request = new byte[] {
                (byte) (ReplyRequired ? 0x00 : 0x80),
                (byte) NxtCommand.ResetInputScaledValue,
                (byte) sensorPort
            };

            Send(request);
        }

        /// <summary>
        /// <para>MESSAGEWRITE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 9.</para>
        /// </remarks>
        /// <param name="inBox">Inbox number</param>
        /// <param name="messageData">Message data</param>
        public void MessageWrite(NxtMailbox inBox, string messageData)
        {
            if (!messageData.EndsWith("\0"))
                messageData += '\0';

            byte messageSize = (byte) messageData.Length;
            if (messageSize > 59)
                throw new ArgumentException("Message may not exceed 59 characters.");

            byte[] request = new byte[4 + messageSize];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.MessageWrite;
            request[2] = (byte) inBox;
            request[3] = messageSize;
            Encoding.ASCII.GetBytes(messageData).CopyTo(request, 4);

            Send(request);
        }

        /// <summary>
        /// <para>RESETMOTORPOSITION</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 9.</para>
        /// </remarks>
        /// <param name="motorPort">Output port</param>
        /// <param name="relative">Relative? True: position relative to last movement, False: absolute position</param>
        public void ResetMotorPosition(NxtMotorPort motorPort, bool relative)
        {
            byte[] request = new byte[] {
                (byte) (ReplyRequired ? 0x00 : 0x80),
                (byte) NxtCommand.ResetMotorPosition,
                (byte) motorPort,
                (byte) (relative ? 0xFF : 0x00)
            };

            Send(request);
        }

        /// <summary>
        /// <para>GETBATTERYLEVEL</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 9.</para>
        /// </remarks>
        /// <returns>Voltage in millivolts</returns>
        public UInt16? GetBatteryLevel()
        {
            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.GetBatteryLevel
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            UInt16 voltage = Util.GetUInt16(reply, 3);
            return voltage;
        }

        /// <summary>
        /// <para>STOPSOUNDPLAYBACK</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 9.</para>
        /// </remarks>
        public void StopSoundPlayback()
        {
            byte[] request = new byte[] {
                (byte) (ReplyRequired ? 0x00 : 0x80),
                (byte) NxtCommand.StopSoundPlayback
            };

            Send(request);
        }

        /// <summary>
        /// <para>KEEPALIVE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 10.</para>
        /// </remarks>
        /// <returns>Current sleep time limit, ms</returns>
        public UInt32? KeepAlive()
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.KeepAlive
            };

            byte[] reply = Send(request);

            if (reply == null)
            {
                TraceUtil.MethodExit("return null");
                return null;
            }

            UInt32 currentSleepLimit = Util.GetUInt32(reply, 3);

            TraceUtil.MethodExit("return currentSleepLimit");
            return currentSleepLimit;
        }

        /// <summary>
        /// <para>LSGETSTATUS</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 10.</para>
        /// <para>Returns the number of bytes ready in the LowSpeed port. Used with digital sensors like the Ultrasonic sensor.</para>
        /// </remarks>
        /// <param name="sensorPort">Sensor port</param>
        /// <returns>Bytes Ready (count of available bytes to read)</returns>
        /// <seealso cref="LsRead"/>
        /// <seealso cref="LsWrite"/>
        public byte? LsGetStatus(NxtSensorPort sensorPort)
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.LsGetStatus,
                (byte) sensorPort
            };

            byte[] reply = Send(request);

            if (reply == null)
            {
                TraceUtil.MethodExit("return null");
                return null;
            }

            byte bytesReady = reply[3];

            TraceUtil.MethodExit("return bytesReady");
            return bytesReady;
        }

        /// <summary>
        /// <para>LSWRITE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 10.</para>
        /// <para>Writes data to the LowSpeed port. Used with digital sensors like the Ultrasonic sensor.</para>
        /// </remarks>
        /// <param name="sensorPort">Sensor port</param>
        /// <param name="rxDataLength">Rx Data Length</param>
        /// <param name="txData">Tx Data</param>
        /// <seealso cref="LsRead"/>
        /// <seealso cref="LsGetStatus"/>
        public void LsWrite(NxtSensorPort sensorPort, byte rxDataLength, byte[] txData)
        {
            TraceUtil.MethodEnter();

            byte txDataLength = (byte) txData.Length;
            if (txDataLength == 0)
            {
                TraceUtil.MethodExit("throw new ArgumentException - 1");
                throw new ArgumentException("No data to send.");
            }

            if (txDataLength > 16)
            {
                TraceUtil.MethodExit("throw new ArgumentException - 2");
                throw new ArgumentException("Tx data may not exceed 16 bytes.");
            }

            if (rxDataLength < 0 || 16 < rxDataLength)
            {
                TraceUtil.MethodExit("throw new ArgumentException - 3");
                throw new ArgumentException("Rx data length should be in the interval 0-16.");
            }

            byte[] request = new byte[5 + txDataLength];
            request[0] = (byte) (ReplyRequired ? 0x00 : 0x80);
            request[1] = (byte) NxtCommand.LsWrite;
            request[2] = (byte) sensorPort;
            request[3] = txDataLength;
            request[4] = rxDataLength;
            txData.CopyTo(request, 5);

            Send(request);

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>LSREAD</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 10.</para>
        /// <para>Reads data from the LowSpeed port. Used with digital sensors like the Ultrasonic sensor.</para>
        /// </remarks>
        /// <param name="sensorPort">The sensor port</param>
        /// <returns>The data read from the port</returns>
        /// <seealso cref="LsGetStatus"/>
        /// <seealso cref="LsWrite"/>
        public byte[] LsRead(NxtSensorPort sensorPort)
        {
            TraceUtil.MethodEnter();

            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.LsRead,
                (byte) sensorPort
            };

            byte[] reply = Send(request);

            if (reply == null)
            {
                TraceUtil.MethodExit("return null");
                return null;
            }

            byte bytesRead = reply[3];

            byte[] rxData = new byte[bytesRead];
            Array.Copy(reply, 4, rxData, 0, bytesRead);

            TraceUtil.MethodExit("return rxData");
            return rxData;
        }

        /// <summary>
        /// <para>GETCURRENTPROGRAMNAME</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 11.</para>
        /// </remarks>
        /// <returns>File name of the running program</returns>
        public string GetCurrentProgramName()
        {
            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.GetCurrentProgramName
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            string fileName = Encoding.ASCII.GetString(reply, 3, 20).TrimEnd('\0');
            return fileName;
        }

        /// <summary>
        /// <para>MESSAGEREAD</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 2, p. 11.</para>
        /// </remarks>
        /// <param name="remoteInboxNo">Remote Inbox number</param>
        /// <param name="localInboxNo">Local Inbox number</param>
        /// <param name="remove">Remove? True: clears message from Remote Inbox</param>
        /// <returns>Message data</returns>
        public string MessageRead(NxtMailbox2 remoteInboxNo, NxtMailbox localInboxNo, bool remove)
        {
            byte[] request = new byte[] {
                0x00,
                (byte) NxtCommand.MessageRead,
                (byte) remoteInboxNo,
                (byte) localInboxNo,
                (byte) (remove ? 0xFF : 0x00)
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            byte localInboxNoOut = reply[3];  // TODO: Validate on this?

            byte messageSize = reply[4];

            string message = Encoding.ASCII.GetString(reply, 5, messageSize).TrimEnd('\0');
            return message;
        }

        /// <summary>
        /// <para>OPEN READ COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 7.</para>
        /// <para>Close the handle after use. The handle is automatically closed if an error occurs.</para>
        /// </remarks>
        /// <param name="fileName">The name of the file</param>
        /// <returns>A NxtOpenReadReply with the result of the request</returns>
        public NxtOpenReadReply? OpenRead(string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenRead;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);

            byte[] reply = Send(request);

            if (reply == null) return null;

            NxtOpenReadReply result;
            result.handle = reply[3];
            result.fileSize = Util.GetUInt32(reply, 4);
            return result;
        }

        /// <summary>
        /// <para>OPEN WRITE COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 7.</para>
        /// <para>Close the handle after use. The handle is automatically closed if an error occurs.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        /// <param name="fileSize">The file size</param>
        /// <returns>A handle to the file</returns>
        public byte? OpenWrite(string fileName, UInt32 fileSize)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[26];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenWrite;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);
            Util.SetUInt32(fileSize, request, 22);

            byte[] reply = Send(request);

            if (reply == null) return null;

            return reply[3];
        }

        /// <summary>
        /// <para>READ COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 8.</para>
        /// </remarks>
        /// <param name="handle">A handle to the file</param>
        /// <param name="bytesToRead">Number of data bytes to be read</param>
        /// <returns>A byte array with the read data</returns>
        public byte[] Read(byte handle, UInt16 bytesToRead)
        {
            byte[] request = new byte[5];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.Read;
            request[2] = handle;
            Util.SetUInt16(bytesToRead, request, 3);

            byte[] reply = Send(request);

            if (reply == null) return null;

            byte handleOut = reply[3];
            if (handleOut != handle)
                throw new NxtException(string.Format("Output handle, {0}, was different from input handle, {1}.", handleOut, handle));

            UInt16 bytesRead = Util.GetUInt16(reply, 4);

            byte[] respons = new byte[bytesRead];
            Array.Copy(reply, 6, respons, 0, bytesRead);

            return respons;
        }

        /// <summary>
        /// <para>WRITE COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 8.</para>
        /// </remarks>
        /// <param name="handle">File handle to write to</param>
        /// <param name="data">Data to write</param>
        /// <returns>Number of bytes written</returns>
        public UInt16? Write(byte handle, byte[] data)
        {
            byte[] request = new byte[3 + data.Length];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.Write;
            request[2] = handle;
            data.CopyTo(request, 3);

            byte[] reply = Send(request);

            if (reply == null) return null;

            byte handleOut = reply[3];
            if (handleOut != handle)
                throw new NxtException(string.Format("Output handle, {0}, was different from input handle, {1}.", handleOut, handle));

            UInt16 bytesWritten = Util.GetUInt16(reply, 4);

            return bytesWritten;
        }

        /// <summary>
        /// <para>CLOSE COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 8.</para>
        /// </remarks>
        /// <param name="handle">File handle to close</param>
        public void Close(byte handle)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.Close,
                handle
            };

            byte[] reply = Send(request);

            if (reply == null) return;

            byte handleOut = reply[3];
            if (handleOut != handle)
                throw new NxtException(string.Format("Output handle, {0}, was different from input handle, {1}.", handleOut, handle));
        }

        /// <summary>
        /// <para>DELETE COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 9.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        public void Delete(string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.Delete;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);

            byte[] reply = Send(request);

            if (reply == null) return;

            string fileNameOut = Encoding.ASCII.GetString(reply, 3, 20);
            if (fileNameOut != fileName)
                throw new NxtException(string.Format("The file reported as deleted, '{0}', was different from the file requested, '{1}'.", fileNameOut, fileName));
        }

        /// <summary>
        /// <para>FIND FIRST</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 9.</para>
        /// <para>Close the handle after use. The handle is automatically closed if an error occurs.</para>
        /// </remarks>
        /// <param name="fileName">Filename or -mask to search</param>
        /// <returns>A NxtFindFileReply containing the result for the search</returns>
        /// <seealso cref="FindNext"/>
        public NxtFindFileReply? FindFirst(string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.FindFirst;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);

            return SendAndParseNxtFindFileReply(request);
        }

        /// <summary>
        /// <para>FIND NEXT</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 10.</para>
        /// <para>Close the handle after use. The handle is automatically closed if an error occurs.</para>
        /// </remarks>
        /// <param name="handle">Handle from the previous found file or from the FindFirst command</param>
        /// <returns>A NxtFindFileReply containing the result for the search</returns>
        /// <seealso cref="FindFirst"/>
        public NxtFindFileReply? FindNext(byte handle)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.FindNext,
                handle
            };

            return SendAndParseNxtFindFileReply(request);
        }

        /// <summary>
        /// <para>Sends a FindFirst- or FindNext-request and parses the result as a NxtFindFileReply.</para>
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>A NxtFindFileReply containing the parsed result</returns>
        /// <seealso cref="FindFirst"/>
        /// <seealso cref="FindNext"/>
        private NxtFindFileReply? SendAndParseNxtFindFileReply(byte[] request)
        {
            byte[] reply;
            NxtFindFileReply result;

            try
            {
                reply = Send(request);

                if (reply == null) return null;
            }
            catch (NxtCommunicationProtocolException ex)
            {
                if (ex.errorMessage == NxtErrorMessage.FileNotFound)
                {
                    result.FileFound = false;
                    result.handle = 0;
                    result.fileName = "";
                    result.fileSize = 0;
                    return result;
                }

                // Rethrow if not a FileNotFound error.
                throw;
            }

            result.FileFound = true;
            result.handle = reply[3];
            result.fileName = Encoding.ASCII.GetString(reply, 4, 20).TrimEnd('\0');
            result.fileSize = Util.GetUInt32(reply, 24);
            return result;
        }

        /// <summary>
        /// <para>GET FIRMWARE VERSION</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 11.</para>
        /// </remarks>
        /// <returns>A NxtGetFirmwareVersionReply with the protocol-, and the firmware versions.</returns>
        public NxtGetFirmwareVersionReply? GetFirmwareVersion()
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.GetFirmwareVersion
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            NxtGetFirmwareVersionReply result;
            result.protocolVersion = new Version(reply[4], reply[3]);
            result.firmwareVersion = new Version(reply[6], reply[5]);
            return result;
        }

        /// <summary>
        /// <para>OPEN WRITE LINEAR COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 11.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        /// <param name="fileSize">The file size</param>
        /// <returns>A handle to the file</returns>
        public byte? OpenWriteLinear(string fileName, UInt32 fileSize)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[26];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenWriteLinear;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);  // NOTE: I'm not sure if the documentation is 100%
            Util.SetUInt32(fileSize, request, 22);  // ... correct here, since it do not allow space for the null terminator.

            byte[] reply = Send(request);

            if (reply == null) return null;

            byte handle = reply[3];
            return handle;
        }

        /// <summary>
        /// <para>OPEN READ LINEAR COMMAND (INTERNAL COMMAND)</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 12.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        /// <returns>Pointer to linear memory segment</returns>
        public UInt32? OpenReadLinear(string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenReadLinear;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);

            byte[] reply = Send(request);

            if (reply == null) return null;

            UInt32 pointer = Util.GetUInt32(reply, 3);
            return pointer;
        }

        /// <summary>
        /// <para>OPEN WRITE DATA COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 12.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        /// <param name="fileSize">The file size</param>
        /// <returns>A handle to the file</returns>
        public byte? OpenWriteData(string fileName, UInt32 fileSize)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[26];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenWriteData;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);  // NOTE: I'm not sure if the documentation is 100%
            Util.SetUInt32(fileSize, request, 22);  // ... correct here, since it do not allow space for the null terminator.

            byte[] reply = Send(request);

            if (reply == null) return null;

            byte handle = reply[3];
            return handle;
        }

        /// <summary>
        /// <para>OPEN APPEND DATA COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 13.</para>
        /// </remarks>
        /// <param name="fileName">The file name</param>
        /// <returns>A NxtOpenAppendDataReply withe the parsed reply</returns>
        public NxtOpenAppendDataReply? OpenAppendData(string fileName)
        {
            ValidateFilename(fileName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.OpenAppendData;
            Encoding.ASCII.GetBytes(fileName).CopyTo(request, 2);

            byte[] reply = Send(request);

            if (reply == null) return null;

            NxtOpenAppendDataReply result;
            result.handle = reply[3];
            result.availableFilesize = Util.GetUInt32(reply, 4);
            return result;
        }

        /// <summary>
        /// <para>REQUEST FIRST MODULE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 19.</para>
        /// </remarks>
        /// <param name="resourceName">Modulename or -mask to search</param>
        /// <returns>A NxtRequestModuleReply containing the result for the search</returns>
        /// <seealso cref="RequestNextModule"/>
        /// <seealso cref="CloseModuleHandle"/>
        public NxtRequestModuleReply? RequestFirstModule(string resourceName)
        {
            ValidateFilename(resourceName);

            byte[] request = new byte[22];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.RequestFirstModule;
            Encoding.ASCII.GetBytes(resourceName).CopyTo(request, 2);

            return SendAndParseNxtRequestModuleReply(request);
        }

        /// <summary>
        /// <para>REQUEST NEXT MODULE</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 19.</para>
        /// </remarks>
        /// <param name="handle">Handle number from the previous Request Next Module command or from the very first Request First Module command</param>
        /// <returns>A NxtRequestModuleReply containing the result for the search</returns>
        /// <seealso cref="RequestFirstModule"/>
        /// <seealso cref="CloseModuleHandle"/>
        public NxtRequestModuleReply? RequestNextModule(byte handle)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.RequestNextModule,
                handle
            };

            return SendAndParseNxtRequestModuleReply(request);
        }

        /// <summary>
        /// <para>Sends a RequestFirstModule- or RequestNextModule-request and parses the result as a NxtRequestModule.</para>
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>A NxtRequestModuleReply containing the parsed result</returns>
        /// <seealso cref="RequestFirstModule"/>
        /// <seealso cref="RequestNextModule"/>
        private NxtRequestModuleReply? SendAndParseNxtRequestModuleReply(byte[] request)
        {
            byte[] reply;
            NxtRequestModuleReply result;

            try
            {
                reply = Send(request);

                if (reply == null) return null;
            }
            catch (NxtCommunicationProtocolException ex)
            {
                if (ex.errorMessage == NxtErrorMessage.NoMoreHandles || ex.errorMessage == NxtErrorMessage.ModuleNotFound)
                {
                    result.ModuleFound = false;
                    result.handle = 0;
                    result.moduleName = "";
                    result.moduleId = 0;
                    result.moduleIdCC = 0;
                    result.moduleIdFF = 0;
                    result.moduleIdPP = 0;
                    result.moduleIdTT = 0;
                    result.moduleSize = 0;
                    result.moduleIoMapSize = 0;
                    return result;
                }

                // Rethrow if not a NoMoreHandles- or a ModuleNotFound error.
                throw;
            }

            result.ModuleFound = true;
            result.handle = reply[3];
            result.moduleName = Encoding.ASCII.GetString(reply, 4, 20).TrimEnd('\0');
            result.moduleId = Util.GetUInt32(reply, 24);
            // NOTE: There seems to be some inconsistency in the documentation about PP TT CC FF, Appendix 1, p. 18.
            result.moduleIdFF = 0; // reply[24];
            result.moduleIdCC = 0; // reply[25];
            result.moduleIdTT = reply[26]; // reply[26];
            result.moduleIdPP = reply[24]; // reply[27]
            result.moduleSize = Util.GetUInt32(reply, 28);
            result.moduleIoMapSize = Util.GetUInt16(reply, 32);
            return result;
        }

        /// <summary>
        /// <para>CLOSE MODULE HANDLE COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 20.</para>
        /// </remarks>
        /// <param name="handle">Handle number</param>
        /// <seealso cref="RequestFirstModule"/>
        /// <seealso cref="RequestNextModule"/>
        public void CloseModuleHandle(byte handle)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.CloseModuleHandle,
                handle
            };

            Send(request);
        }

        /// <summary>
        /// <para>READ IO MAP COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 20.</para>
        /// </remarks>
        /// <param name="moduleId">The module ID</param>
        /// <param name="offset">The offset to read from</param>
        /// <param name="bytesToRead">Number of bytes to be read</param>
        /// <returns>IO-map content</returns>
        public byte[] ReadIoMap(UInt32 moduleId, UInt16 offset, UInt16 bytesToRead)
        {
            byte[] request = new byte[10];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.ReadIoMap;
            Util.SetUInt32(moduleId, request, 2);
            Util.SetUInt16(offset, request, 6);
            Util.SetUInt16(bytesToRead, request, 8);

            byte[] reply = Send(request);

            if (reply == null) return null;

            UInt32 moduleIdOut = Util.GetUInt32(reply, 3);
            if (moduleIdOut != moduleId)
                throw new NxtException(string.Format("Output module Id, {0}, was different from input module Id, {1}.", moduleIdOut, moduleId));

            UInt16 bytesRead = Util.GetUInt16(reply, 7);

            byte[] ioMapContent = new byte[bytesRead];
            Array.Copy(reply, 9, ioMapContent, 0, bytesRead);

            return ioMapContent;
        }

        /// <summary>
        /// <para>WRITE IO MAP COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 21.</para>
        /// </remarks>
        /// <param name="moduleId">The module ID</param>
        /// <param name="offset">The offset to write to</param>
        /// <param name="ioMapContent">IO-map content to be stored in IO-map[index]...IO-map[index+N]</param>
        /// <returns>The number of data that have been written</returns>
        public UInt16? WriteIoMap(UInt32 moduleId, UInt16 offset, byte[] ioMapContent)
        {
            UInt16 bytesToWrite = (UInt16) ioMapContent.Length;

            byte[] request = new byte[10 + ioMapContent.Length];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.WriteIoMap;
            Util.SetUInt32(moduleId, request, 2);
            Util.SetUInt16(offset, request, 6);
            Util.SetUInt16(bytesToWrite, request, 8);
            ioMapContent.CopyTo(request, 10);

            byte[] reply = Send(request);

            if (reply == null) return null;

            UInt32 moduleIdOut = Util.GetUInt32(reply, 3);
            if (moduleIdOut != moduleId)
                throw new NxtException(string.Format("Output module Id, {0}, was different from input module Id, {1}.", moduleIdOut, moduleId));

            UInt16 bytesWritten = Util.GetUInt16(reply, 7);
            return bytesWritten;
        }

        /// <summary>
        /// <para>BOOT COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 13.</para>
        /// <para>This command can only be accepted by USB.</para>
        /// </remarks>
        public void Boot()
        {
            byte[] request = new byte[21];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.Boot;
            Encoding.ASCII.GetBytes("Let's dance: SAMBA").CopyTo(request, 2);

            byte[] reply = Send(request);

            if (reply == null) return;

            string result = Encoding.ASCII.GetString(reply, 3, 4).TrimEnd('\0');
            if (result != "Yes")
                throw new NxtException("The reply, '{0}', was not the expected, 'Yes'.");
        }

        /// <summary>
        /// <para>SET BRICK NAME COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 13.</para>
        /// <para>For some reason only the first 8 characters is remembered when the NXT is turned off. This is with version 1.4 of the firmware, and it may be fixed with newer versions.</para>
        /// </remarks>
        /// <param name="brickName">The new name of the NXT brick</param>
        public void SetBrickName(string brickName)
        {
            if (brickName.Length > 15)
                brickName = brickName.Substring(0, 15);

            byte[] request = new byte[18];
            request[0] = 0x01;
            request[1] = (byte) NxtCommand.SetBrickName;
            Encoding.ASCII.GetBytes(brickName).CopyTo(request, 2);

            Send(request);
        }

        /// <summary>
        /// <para>GET DEVICE INFO</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 14.</para>
        /// <para>For some reason only the first 8 characters of the BXT name is remembered when the NXT is turned off. This is with version 1.4 of the firmware, and it may be fixed with newer versions.</para>
        /// </remarks>
        /// <returns>A NxtGetDeviceInfoReply containing the parsed result</returns>
        public NxtGetDeviceInfoReply? GetDeviceInfo()
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.GetDeviceInfo
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            NxtGetDeviceInfoReply result;

            result.nxtName = Encoding.ASCII.GetString(reply, 3, 15).TrimEnd('\0');

            result.btAdress = new byte[7];
            Array.Copy(reply, 18, result.btAdress, 0, 7);

            result.bluetoothSignalStrength = Util.GetUInt32(reply, 25);

            result.freeUserFlash = Util.GetUInt32(reply, 29);

            return result;
        }

        /// <summary>
        /// <para>DELETE USER FLASH</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 14.</para>
        /// </remarks>
        public void DeleteUserFlash()
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.DeleteUserFlash
            };

            Send(request);
        }

        /// <summary>
        /// <para>POLL COMMAND LENGTH</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 15.</para>
        /// </remarks>
        /// <param name="bufferNo">Buffer Number: 0x00 = Poll Buffer, 0x01 = High Speed buffer</param>
        /// <returns>Number of bytes for the command ready in the buffer</returns>
        public byte? PollCommandLength(byte bufferNo)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.PollCommandLength,
                bufferNo
            };

            byte[] reply = Send(request);

            if (reply == null) return null;

            // NOTE: My guess is that the documentation has switched meaning between bytes 2 and 3.
            byte bufferNoOut = reply[3];
            if (bufferNoOut != bufferNo)
                throw new NxtException(string.Format("Output buffer no-, {0}, was different from input buffer no., {1}.", bufferNoOut, bufferNo));

            byte bytesReady = reply[4];
            return bytesReady;
        }

        /// <summary>
        /// <para>POLL COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 15.</para>
        /// </remarks>
        /// <param name="bufferNo">Buffer Number: 0x00 = Poll Buffer, 0x01 = High Speed buffer</param>
        /// <param name="commandLength">Command length</param>
        public void PollCommand(byte bufferNo, byte commandLength)
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.PollCommand,
                bufferNo,
                commandLength
            };

            byte[] reply = Send(request);

            if (reply == null) return;

            // NOTE: My guess is that the documentathion has switched meaning between bytes 2 and 3.
            byte bufferNoOut = reply[3];
            if (bufferNoOut != bufferNo)
                throw new NxtException(string.Format("Output buffer no-, {0}, was different from input buffer no., {1}.", bufferNoOut, bufferNo));

            // TODO: Parse the reply, and return the result.
        }

        /// <summary>
        /// <para>BLUETOOTH FACTORY RESET COMMAND</para>
        /// </summary>
        /// <remarks>
        /// <para>Reference: BDK, Appendix 1, p. 15.</para>
        /// <para>This command cannot be transmitted via Bluetooth because all Bluetooth functionality is reset by this command!</para>
        /// </remarks>
        public void BluetoothFactoryReset()
        {
            byte[] request = new byte[] {
                0x01,
                (byte) NxtCommand.BluetoothFactoryReset
            };

            Send(request);
        }

        #endregion

        #region Utilities.

        /// <summary>
        /// This function validates that the filename is correct for the NXT brick.
        /// </summary>
        /// <param name="fileName">The file name</param>
        private void ValidateFilename(string fileName)
        {
            if (fileName.Length > 19)
                throw new ArgumentException("File name is to long. Maximum length is 19 characters (15.3).");
        }

        #endregion
    }

    #region Reply types.

    /// <summary>
    /// <para>Reply type for the FindFirst() and FindNext() functions.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.FindFirst"/>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.FindNext"/>
    public struct NxtFindFileReply
    {
        /// <summary>
        /// <para>Boolean indicating if a file was found or not.</para>
        /// </summary>
        public bool FileFound;

        /// <summary>
        /// <para>The file handle.</para>
        /// </summary>
        public byte handle;

        /// <summary>
        /// <para>The file name.</para>
        /// </summary>
        public string fileName;

        /// <summary>
        /// <para>The filesize.</para>
        /// </summary>
        public UInt32 fileSize;
    }

    /// <summary>
    /// 	<para>Reply type for the FindFirst() and FindNext() functions.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.FindFirst"/>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.FindNext"/>
    public struct CopyOfNxtFindFileReply
    {
        /// <summary>
        /// <para>Boolean indicating if a file was found or not.</para>
        /// </summary>
        public bool FileFound;

        /// <summary>
        /// <para>The file handle.</para>
        /// </summary>
        public byte handle;

        /// <summary>
        /// <para>The file name.</para>
        /// </summary>
        public string fileName;

        /// <summary>
        /// <para>The filesize.</para>
        /// </summary>
        public UInt32 fileSize;
    }

    /// <summary>
    /// <para>Reply type for the GetDeviceInfo() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.GetDeviceInfo"/>
    public struct NxtGetDeviceInfoReply
    {
        /// <summary>
        /// <para>The name of the NXT brick.</para>
        /// </summary>
        public string nxtName;

        /// <summary>
        /// <para>The bluetooth address.</para>
        /// </summary>
        public byte[] btAdress;

        /// <summary>
        /// <para>The bluetooth signal strength.</para>
        /// </summary>
        public UInt32 bluetoothSignalStrength;

        /// <summary>
        /// <para>The size of the free user flash.</para>
        /// </summary>
        public UInt32 freeUserFlash;
    }

    /// <summary>
    /// <para>Reply type for the GetFirmwareVersion() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.GetFirmwareVersion"/>
    public struct NxtGetFirmwareVersionReply
    {
        /// <summary>
        /// <para>The protocol version.</para>
        /// </summary>
        public Version protocolVersion;

        /// <summary>
        /// <para>The firmware version.</para>
        /// </summary>
        public Version firmwareVersion;
    }

    /// <summary>
    /// <para>Reply type for the GetInputValues() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.GetInputValues"/>
    public struct NxtGetInputValuesReply
    {
        /// <summary>
        /// <para>A boolean indicating if the returned result is valid or not.</para>
        /// </summary>
        public bool valid;

        /// <summary>
        /// <para>A boolean indicating if the returned result is calibrated or not.</para>
        /// </summary>
        public bool calibrated;

        /// <summary>
        /// <para>The sensor type.</para>
        /// </summary>
        public NxtSensorType sensorType;

        /// <summary>
        /// <para>The sensor mode.</para>
        /// </summary>
        public NxtSensorMode sensorMode;

        /// <summary>
        /// <para>The raw A/D value.</para>
        /// </summary>
        public UInt16 rawAdValue;

        /// <summary>
        /// <para>The normalized A/D value.</para>
        /// </summary>
        public UInt16 normalizedAdValue;

        /// <summary>
        /// <para>The scaled value.</para>
        /// </summary>
        public Int16 scaledValue;

        /// <summary>
        /// <para>The calibrated value.</para>
        /// </summary>
        public Int16 calibratedValue;
    }

    /// <summary>
    /// 	<para>Reply type for the GetOutputState() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.GetOutputState"/>
    public struct CopyOfNxtGetOutputStateReply
    {
        /// <summary>
        /// <para>The motor power.</para>
        /// </summary>
        public sbyte power;

        /// <summary>
        /// <para>The motor mode.</para>
        /// </summary>
        public NxtMotorMode mode;

        /// <summary>
        /// <para>The regulation mode.</para>
        /// </summary>
        public NxtMotorRegulationMode regulationMode;

        /// <summary>
        /// <para>The turn ratio.</para>
        /// </summary>
        public sbyte turnRatio;

        /// <summary>
        /// <para>The run state.</para>
        /// </summary>
        public NxtMotorRunState runState;

        /// <summary>
        /// <para>The tacho limit in degrees, 0 means unlimited.</para>
        /// </summary>
        public UInt32 tachoLimit;

        /// <summary>
        /// <para>The tacho count.</para>
        /// </summary>
        public Int32 tachoCount;

        /// <summary>
        /// <para>The block tacho count.</para>
        /// </summary>
        public Int32 blockTachoCount;

        /// <summary>
        /// <para>The rotation count.</para>
        /// </summary>
        public Int32 rotationCount;

        /// <summary>
        /// <para>ToString()-override.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public override string ToString()
        {
            return string.Format("[P:{0,4}|M:{1,25}|RM:{2,27}|TR:{3,4}|RS:{4,24}|TL:{5,3}|TC:{6,4}|BTC:{7,4}|RC:{8,4}]",
                power,
                mode,
                regulationMode,
                turnRatio,
                runState,
                tachoLimit,
                tachoCount,
                blockTachoCount,
                rotationCount);
        }
    }

    /// <summary>
    /// <para>Reply type for the GetOutputState() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.GetOutputState"/>
    public struct NxtGetOutputStateReply
    {
        /// <summary>
        /// <para>The motor power.</para>
        /// </summary>
        public sbyte power;

        /// <summary>
        /// <para>The motor mode.</para>
        /// </summary>
        public NxtMotorMode mode;

        /// <summary>
        /// <para>The regulation mode.</para>
        /// </summary>
        public NxtMotorRegulationMode regulationMode;

        /// <summary>
        /// <para>The turn ratio.</para>
        /// </summary>
        public sbyte turnRatio;

        /// <summary>
        /// <para>The run state.</para>
        /// </summary>
        public NxtMotorRunState runState;

        /// <summary>
        /// <para>The tacho limit in degrees, 0 means unlimited.</para>
        /// </summary>
        public UInt32 tachoLimit;

        /// <summary>
        /// <para>The tacho count.</para>
        /// </summary>
        public Int32 tachoCount;

        /// <summary>
        /// <para>The block tacho count.</para>
        /// </summary>
        public Int32 blockTachoCount;

        /// <summary>
        /// <para>The rotation count.</para>
        /// </summary>
        public Int32 rotationCount;

        /// <summary>
        /// <para>ToString()-override.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public override string ToString()
        {
            return string.Format("[P:{0,4}|M:{1,25}|RM:{2,27}|TR:{3,4}|RS:{4,24}|TL:{5,3}|TC:{6,4}|BTC:{7,4}|RC:{8,4}]",
                power,
                mode,
                regulationMode,
                turnRatio,
                runState,
                tachoLimit,
                tachoCount,
                blockTachoCount,
                rotationCount);
        }
    }

    /// <summary>
    /// <para>Reply type for the OpenAppendData() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.OpenAppendData"/>
    public struct NxtOpenAppendDataReply
    {
        /// <summary>
        /// <para>File handle.</para>
        /// </summary>
        public byte handle;

        /// <summary>
        /// <para>Available file size.</para>
        /// </summary>
        public UInt32 availableFilesize;
    }

    /// <summary>
    /// <para>Reply type for the OpenRead() function.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.OpenRead"/>
    public struct NxtOpenReadReply
    {
        /// <summary>
        /// <para>File handle.</para>
        /// </summary>
        public byte handle;

        /// <summary>
        /// <para>File size.</para>
        /// </summary>
        public UInt32 fileSize;
    }

    /// <summary>
    /// <para>Reply type for the RequestFirstModule() and RequestNextModule() functions.</para>
    /// </summary>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.RequestFirstModule"/>
    /// <seealso cref="M:NKH.MindSqualls.NxtCommunicationProtocol.RequestNextModule"/>
    public struct NxtRequestModuleReply
    {
        /// <summary>
        /// <para>Boolean indicating if a module was found, or not.</para>
        /// </summary>
        public bool ModuleFound;

        /// <summary>
        /// <para>Module handle.</para>
        /// </summary>
        public byte handle;

        /// <summary>
        /// <para>Module name.</para>
        /// </summary>
        public string moduleName;

        /// <summary>
        /// <para>Module ID.</para>
        /// </summary>
        public UInt32 moduleId;

        /// <summary>
        /// <para>Module ID, PP-value.</para>
        /// </summary>
        public byte moduleIdPP;

        /// <summary>
        /// <para>Module ID, TT-value.</para>
        /// </summary>
        public byte moduleIdTT;

        /// <summary>
        /// <para>Module ID, CC-value.</para>
        /// </summary>
        public byte moduleIdCC;

        /// <summary>
        /// <para>Module ID, FF-value.</para>
        /// </summary>
        public byte moduleIdFF;

        /// <summary>
        /// <para>Module size.</para>
        /// </summary>
        public UInt32 moduleSize;

        /// <summary>
        /// <para>Module IO-map size.</para>
        /// </summary>
        public UInt16 moduleIoMapSize;
    }

    #endregion

    #region Enumerations.

    /// <summary>
    /// <para>Commands to the NXT brick.</para>
    /// </summary>
    /// <remarks>
    /// <para>Reference: BDK, Appendix 1 &amp; 2.</para>
    /// </remarks>
    public enum NxtCommand: byte
    {
        /// <summary>
        /// <para>BDK, Appendix 2, p. 5.</para>
        /// </summary>
        StartProgram = 0x00,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 5.</para>
        /// </summary>
        StopProgram = 0x01,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 5.</para>
        /// </summary>
        PlaySoundfile = 0x02,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 6.</para>
        /// </summary>
        PlayTone = 0x03,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 6.</para>
        /// </summary>
        SetOutputState = 0x04,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 7.</para>
        /// </summary>
        SetInputMode = 0x05,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 8.</para>
        /// </summary>
        GetOutputState = 0x06,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 8.</para>
        /// </summary>
        GetInputValues = 0x07,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 8.</para>
        /// </summary>
        ResetInputScaledValue = 0x08,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 9.</para>
        /// </summary>
        MessageWrite = 0x09,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 9.</para>
        /// </summary>
        ResetMotorPosition = 0x0A,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 9.</para>
        /// </summary>
        GetBatteryLevel = 0x0B,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 9.</para>
        /// </summary>
        StopSoundPlayback = 0x0C,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 10.</para>
        /// </summary>
        KeepAlive = 0x0D,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 10.</para>
        /// </summary>
        LsGetStatus = 0x0E,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 10.</para>
        /// </summary>
        LsWrite = 0x0F,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 10.</para>
        /// </summary>
        LsRead = 0x10,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 11.</para>
        /// </summary>
        GetCurrentProgramName = 0x11,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 11.</para>
        /// </summary>
        MessageRead = 0x13,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 7.</para>
        /// </summary>
        OpenRead = 0x80,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 7.</para>
        /// </summary>
        OpenWrite = 0x81,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 8.</para>
        /// </summary>
        Read = 0x82,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 8.</para>
        /// </summary>
        Write = 0x83,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 8.</para>
        /// </summary>
        Close = 0x84,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 9.</para>
        /// </summary>
        Delete = 0x85,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 9.</para>
        /// </summary>
        FindFirst = 0x86,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 10.</para>
        /// </summary>
        FindNext = 0x87,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 11.</para>
        /// </summary>
        GetFirmwareVersion = 0x88,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 11.</para>
        /// </summary>
        OpenWriteLinear = 0x89,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 12.</para>
        /// </summary>
        OpenReadLinear = 0x8A,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 12.</para>
        /// </summary>
        OpenWriteData = 0x8B,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 13.</para>
        /// </summary>
        OpenAppendData = 0x8C,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 19.</para>
        /// </summary>
        RequestFirstModule = 0x90,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 19.</para>
        /// </summary>
        RequestNextModule = 0x91,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 20.</para>
        /// </summary>
        CloseModuleHandle = 0x92,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 20.</para>
        /// </summary>
        ReadIoMap = 0x94,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 21.</para>
        /// </summary>
        WriteIoMap = 0x95,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 13.</para>
        /// </summary>
        Boot = 0x97,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 13.</para>
        /// </summary>
        SetBrickName = 0x98,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 14.</para>
        /// </summary>
        GetDeviceInfo = 0x9B,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 14.</para>
        /// </summary>
        DeleteUserFlash = 0xA0,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 15.</para>
        /// </summary>
        PollCommandLength = 0xA1,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 15.</para>
        /// </summary>
        PollCommand = 0xA2,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 15.</para>
        /// </summary>
        BluetoothFactoryReset = 0xA4
    }

    /// <summary>
    /// <para>ERROR MESSAGE BACK TO THE HOST</para>
    /// </summary>
    /// <remarks>
    /// <para>Reference: BDK, Appendix 1, p. 16.<br/>
    /// Reference: BDK, Appendix 2, p. 12.</para>
    /// </remarks>
    public enum NxtErrorMessage: byte
    {
        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        Succes = 0x00,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        PendingCommunicationTransactionInProgress = 0x20,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        SpecifiedMailboxQueueIsEmpty = 0x40,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NoMoreHandles = 0x81,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NoSpace = 0x82,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NoMoreFiles = 0x83,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        EndOfFileExpected = 0x84,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        EndOfFile = 0x85,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NotALinearFile = 0x86,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        FileNotFound = 0x87,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        HandleAllReadyClosed = 0x88,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NoLinearSpace = 0x89,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        UndefinedError = 0x8A,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        FileIsBusy = 0x8B,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        NoWriteBuffers = 0x8C,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        AppendNotPossible = 0x8D,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        FileIsFull = 0x8E,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        FileExists = 0x8F,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        ModuleNotFound = 0x90,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        OutIfBoundary = 0x91,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        IllegalFileName = 0x92,

        /// <summary>
        /// <para>BDK, Appendix 1, p. 16.</para>
        /// </summary>
        IllegalHandle = 0x93,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        RequestFailed = 0xBD, // i.e. specified file not found

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        UnknownCommandOpcode = 0xBE,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        InsanePacket = 0xBF,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        DataContainsOutOfRangeValues = 0xC0,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        CommunicationBusError = 0xDD,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        NoFreeMemoryInCommunicationBuffer = 0xDE,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        SpecifiedChannelOrConnectionIsNotValid = 0xDF,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        SpecifiedChannelOrConnectionNotConfiguredOrBusy = 0xE0,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        NoActiveProgram = 0xEC,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        IllegalSizeSpecified = 0xED,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        IllegalMailboxQueueIdSpecified = 0xEE,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        AttemptedToAccessInvalidFieldOfAStructure = 0xEF,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        BadInputOrOutputSpecified = 0xF0,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        InsufficientMemoryAvailable = 0xFB,

        /// <summary>
        /// <para>BDK, Appendix 2, p. 12.</para>
        /// </summary>
        BadArguments = 0xFF
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 6.</para>
    /// </summary>
    [Flags]
    public enum NxtMotorMode: byte
    {
        /// <summary>
        /// <para>Turn on the specified motor.</para>
        /// </summary>
        MOTORON = 0x01,

        /// <summary>
        /// <para>Use run/break instead of run/float in PWM.</para>
        /// </summary>
        BRAKE = 0x02,

        /// <summary>
        /// <para>Turns on the regulation.</para>
        /// </summary>
        REGULATED = 0x04
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 6.</para>
    /// </summary>
    public enum NxtMotorPort: byte
    {
        /// <summary>
        /// <para>Motor port A.</para>
        /// </summary>
        PortA,

        /// <summary>
        /// <para>Motor port B.</para>
        /// </summary>
        PortB,

        /// <summary>
        /// <para>Motor port C.</para>
        /// </summary>
        PortC,

        /// <summary>
        /// <para>All motor ports.</para>
        /// </summary>
        All = 0xFF
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 6.</para>
    /// </summary>
    public enum NxtMotorRegulationMode: byte
    {
        /// <summary>
        /// <para>No regulation will be enabled.</para>
        /// </summary>
        REGULATION_MODE_IDLE = 0x00,

        /// <summary>
        /// <para>Power control will be enabled on specified output.</para>
        /// </summary>
        REGULATION_MODE_MOTOR_SPEED = 0x01,

        /// <summary>
        /// <para>Syncronization will be enabled (Needs enabled on two output).</para>
        /// </summary>
        REGULATION_MODE_MOTOR_SYNC = 0x02
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 6.</para>
    /// </summary>
    [Flags]
    public enum NxtMotorRunState: byte
    {
        /// <summary>
        /// <para>Output will be idle.</para>
        /// </summary>
        MOTOR_RUN_STATE_IDLE = 0x00,

        /// <summary>
        /// <para>Output will ramp-up.</para>
        /// </summary>
        MOTOR_RUN_STATE_RAMPUP = 0x10,

        /// <summary>
        /// <para>Output will be running.</para>
        /// </summary>
        MOTOR_RUN_STATE_RUNNING = 0x20,

        /// <summary>
        /// <para>Output will ramp-down.</para>
        /// </summary>
        MOTOR_RUN_STATE_RAMPDOWN = 0x40
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 7.</para>
    /// </summary>
    public enum NxtSensorMode: byte
    {
        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        RAWMODE = 0x00,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        BOOLEANMODE = 0x20,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        TRANSITIONCNTMODE = 0x40,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        PERIODCOUNTERMODE = 0x60,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        PCTFULLSCALEMODE = 0x80,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        CELSIUSMODE = 0xA0,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        FAHRENHEITMODE = 0xC0,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        ANGLESTEPMODE = 0xE0
        // SLOPEMASK = 0x1F (?)
        // MODEMASK = 0xE0 (?) 
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 7.</para>
    /// </summary>
    public enum NxtSensorPort: byte
    {
        /// <summary>
        /// <para>Sensor port 1.</para>
        /// </summary>
        Port1,

        /// <summary>
        /// <para>Sensor port 2.</para>
        /// </summary>
        Port2,

        /// <summary>
        /// <para>Sensor port 3.</para>
        /// </summary>
        Port3,

        /// <summary>
        /// <para>Sensor port 4.</para>
        /// </summary>
        Port4
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 7.</para>
    /// </summary>
    /// <remarks>
    /// <para>Explantion of the values is found on p. 46 of the Executable File Specification document.</para>
    /// </remarks>
    public enum NxtSensorType: byte
    {
        /// <summary>
        /// <para>No sensor configured.</para>
        /// </summary>
        NO_SENSOR = 0x00,

        /// <summary>
        /// <para>NXT or RCX touch sensor.</para>
        /// </summary>
        SWITCH = 0x01,

        /// <summary>
        /// <para>RCX temperature sensor.</para>
        /// </summary>
        TEMPERATURE = 0x02,

        /// <summary>
        /// <para>RCX light sensor.</para>
        /// </summary>
        REFLECTION = 0x03,

        /// <summary>
        /// <para>RCX rotation sensor.</para>
        /// </summary>
        ANGLE = 0x04,

        /// <summary>
        /// <para>NXT light sensor with floodlight enabled.</para>
        /// </summary>
        LIGHT_ACTIVE = 0x05,

        /// <summary>
        /// <para>NXT light sensor with floodlight disabled.</para>
        /// </summary>
        LIGHT_INACTIVE = 0x06,

        /// <summary>
        /// <para>NXT sound sensor; dB scaling.</para>
        /// </summary>
        SOUND_DB = 0x07,

        /// <summary>
        /// <para>NXT sound sensor; dBA scaling.</para>
        /// </summary>
        SOUND_DBA = 0x08,

        /// <summary>
        /// <para>Unused in NXT programs.</para>
        /// </summary>
        CUSTOM = 0x09,

        /// <summary>
        /// <para>I<sup>2</sup>C digital sensor.</para>
        /// </summary>
        LOWSPEED = 0x0A,

        /// <summary>
        /// <para>I<sup>2</sup>C digital sensor; 9V power.</para>
        /// </summary>
        LOWSPEED_9V = 0x0B,

        /// <summary>
        /// <para>"HIGHSPEED". Unused in NXT programs.</para>
        /// </summary>
        NO_OF_SENSOR_TYPES = 0x0C
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 9.</para>
    /// </summary>
    public enum NxtMailbox: byte
    {
        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box0,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box1,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box2,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box3,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box4,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box5,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box6,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box7,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box8,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box9
    }

    /// <summary>
    /// <para>Reference: BDK, Appendix 2, p. 9.<br/>
    /// Reference: BDK, Appendix 2, p. 11. (see MessageRead() function).</para>
    /// </summary>
    public enum NxtMailbox2: byte
    {
        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box0,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box1,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box2,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box3,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box4,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box5,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box6,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box7,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box8,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box9,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box10,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box11,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box12,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box13,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box14,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box15,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box16,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box17,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box18,

        /// <summary>
        /// <para>... TBD ...</para>
        /// </summary>
        Box19
    }

    #endregion
}