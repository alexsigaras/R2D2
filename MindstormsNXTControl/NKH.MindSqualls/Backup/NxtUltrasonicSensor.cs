using System;
using System.Text;

// TODO: Add support for single-shot mode.
// TODO: Add support for event-capture mode. Detect when other Ultrasonic sensors is nearby.

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the Ultrasonic sensor.</para>
    /// </summary>
    /// <remarks>
    /// <para>The I2C protocol for the Ultrasonic sensor is specified in the document:</para>
    /// <para>Lego Mindstorms NXT,<br/>
    /// Hardware Developer Kit,<br/>
    /// Appendix 7: LEGO MINDSTORMS NXT Ultrasonic Sensor I2C communication protocol</para>
    /// <para>which can be downloaded at:<br/>
    /// <a href="http://mindstorms.lego.com/overview/NXTreme.aspx" target="_blank">http://mindstorms.lego.com/overview/NXTreme.aspx</a></para>
    /// <para>A special thanks goes to Dick Swan and Templar for their invaluable explanations in:<br/>
    /// <a href="http://forums.nxtasy.org/index.php?showtopic=141" target="_blank">http://forums.nxtasy.org/index.php?showtopic=141</a></para>
    /// <para>The resulting code stated out being part of the NxtUltrasonicSensor class but has since been moved to the NxtDigitalSensor class.</para>
    /// </remarks>
    public class NxtUltrasonicSensor: NxtDigitalSensor
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtUltrasonicSensor()
            : base()
        { }

        #region Sensor readings.

        /// <summary>
        /// <para>Returns the measured distance in cm.</para>
        /// </summary>
        public byte? DistanceCm
        {
            get { return pollData; }
        }

        #endregion

        #region I2C protocol.

        #region Constants.

        /// <summary>
        /// <para>Returns the "factory zero" of the sensor.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public byte ReadFactoryZero()  // Cal 1
        {
            byte[] request = new byte[] { deviceAddress, 0x11 };
            byte[] reply = Send(1, request);
            return reply[0];
        }

        /// <summary>
        /// <para>Returns the "factory scale factor" of the sensor.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public byte ReadFactoryScaleFactor()  // Cal 2
        {
            byte[] request = new byte[] { deviceAddress, 0x12 };
            byte[] reply = Send(1, request);
            return reply[0];
        }

        /// <summary>
        /// <para>Returns the factory scale divisor of the sensor.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public byte ReadFactoryScaleDivisor()
        {
            byte[] request = new byte[] { deviceAddress, 0x13 };
            byte[] reply = Send(1, request);
            return reply[0];
        }

        /// <summary>
        /// <para>Returns the measurement units if the sensor, e.g. "10E-2m".</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public string ReadMeasurementUnits()
        {
            byte[] request = new byte[] { deviceAddress, 0x14 };
            byte[] reply = Send(7, request);
            return Encoding.ASCII.GetString(reply, 0, reply.Length).TrimEnd('\0');
        }

        #endregion

        #region Variables.

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 2.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        /// <seealso cref="ContinuousMeasurementCommand"/>
        /// <seealso cref="SetContinuousMeasurementInterval"/>
        public byte? ReadContinuousMeasurementsInterval()
        {
            return ReadByteFromAddress(0x40);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 2.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        public byte? ReadCommandState()
        {
            return ReadByteFromAddress(0x41);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 2.</para>
        /// </summary>
        /// <param name="x">The byte no.</param>
        /// <returns>... TBD ...</returns>
        public byte? ReadMeasurementByteX(byte x)
        {
            if (x < 0 || 7 < x)
                throw new ArgumentException("The measurement byte number must be in the interval 0-7.");

            x += 0x42;  // 0 -> 0x42, 7 -> 0x49
            return ReadByteFromAddress(x);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 2.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        /// <seealso cref="SetActualZero"/>
        public byte? ReadActualZero()  // Cal1
        {
            return ReadByteFromAddress(0x50);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        /// <seealso cref="SetActualScaleFactor"/>
        public byte? ReadActualScaleFactor()  // Cal 2
        {
            return ReadByteFromAddress(0x51);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <returns>... TBD ...</returns>
        /// <seealso cref="SetActualScaleDivisor"/>
        public byte? ReadActualScaleDivisor()
        {
            return ReadByteFromAddress(0x52);
        }

        #endregion

        #region Commands.

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        public void OffCommand()
        {
            CommandToAddress(0x41, 0x00);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <seealso cref="ContinuousMeasurementCommand"/>
        public void SingleShotCommand()
        {
            CommandToAddress(0x41, 0x01);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <seealso cref="ReadContinuousMeasurementsInterval"/>
        /// <seealso cref="SetContinuousMeasurementInterval"/>
        /// <seealso cref="SingleShotCommand"/>
        public void ContinuousMeasurementCommand()
        {
            CommandToAddress(0x41, 0x02);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        public void EventCaptureCommand()
        {
            CommandToAddress(0x41, 0x03);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        public void RequestWarmReset()
        {
            CommandToAddress(0x41, 0x04);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <param name="interval">The interval</param>
        /// <seealso cref="ContinuousMeasurementCommand"/>
        /// <seealso cref="ReadContinuousMeasurementsInterval"/>
        public void SetContinuousMeasurementInterval(byte interval)
        {
            CommandToAddress(0x40, interval);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <param name="value">The value</param>
        /// <seealso cref="ReadActualZero"/>
        public void SetActualZero(byte value)  // Cal 1
        {
            CommandToAddress(0x50, value);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <param name="value">The value</param>
        /// <seealso cref="ReadActualScaleFactor"/>
        public void SetActualScaleFactor(byte value)  // Cal 2
        {
            CommandToAddress(0x51, value);
        }

        /// <summary>
        /// <para>Reference: HDK, Appendix 7, p. 3.</para>
        /// </summary>
        /// <remarks>
        /// See the remark at ReadActualScaleDivisor().
        /// </remarks>
        /// <param name="value">The value</param>
        /// <seealso cref="ReadActualScaleDivisor"/>
        public void SetActualScaleDivisor(byte value)
        {
            CommandToAddress(0x52, value);
        }

        #endregion

        #endregion

        #region NXT-G like events & NxtPollable overrides.

        private byte triggerDistanceCm = byte.MaxValue;

        /// <summary>
        /// <para>Trigger distance.</para>
        /// </summary>
        /// <seealso cref="OnInsideDistance"/>
        /// <seealso cref="OnOutsideDistance"/>
        public byte TriggerDistanceCm
        {
            get { return triggerDistanceCm; }
            set { triggerDistanceCm = value; }
        }

        /// <summary>
        /// <para>This event is fired if the sensor detects an object within the trigger distance.</para>
        /// </summary>
        /// <seealso cref="TriggerDistanceCm"/>
        /// <seealso cref="OnOutsideDistance"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnInsideDistance;

        /// <summary>
        /// <para>This event is fired if the sensor does not detect an object within the trigger distance.</para>
        /// </summary>
        /// <seealso cref="TriggerDistanceCm"/>
        /// <seealso cref="OnInsideDistance"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnOutsideDistance;

        /// <summary>
        /// <para>The data from the previous poll of the sensor.</para>
        /// </summary>
        /// <seealso cref="Poll"/>
        protected byte? pollData;

        private object pollDataLock = new object();

        /// <summary>
        /// <para>Polls the sensor, and fires the NXT-G like events if appropriate.</para>
        /// </summary>
        /// <seealso cref="TriggerDistanceCm"/>
        /// <seealso cref="OnInsideDistance"/>
        /// <seealso cref="OnOutsideDistance"/>
        public override void Poll()
        {
            TraceUtil.MethodEnter();

            if (Brick.IsConnected)
            {
                byte? oldDistance, newDistance;
                lock (pollDataLock)
                {
                    oldDistance = DistanceCm;
                    pollData = ReadMeasurementByteX(0);
                    base.Poll();
                    newDistance = DistanceCm;
                }

                if (oldDistance.HasValue && newDistance.HasValue)
                {
                    if (OnInsideDistance != null &&
                        oldDistance > TriggerDistanceCm && TriggerDistanceCm >= newDistance)
                        OnInsideDistance(this);
                    else if (OnOutsideDistance != null &&
                        oldDistance <= TriggerDistanceCm && TriggerDistanceCm < newDistance)
                        OnOutsideDistance(this);
                }
            }

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}