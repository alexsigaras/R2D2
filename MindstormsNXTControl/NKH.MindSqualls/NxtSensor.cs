namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Delegate used for the NXT-G like events implemented in the various sensors.</para>
    /// </summary>
    /// <param name="sensor">The sensor</param>
    public delegate void NxtSensorEvent(NxtSensor sensor);

    /// <summary>
    /// <para>Superclass representing a generic sensor.</para>
    /// </summary>
    /// <remarks>
    /// <para>Used as base class for the specific sensor classes.</para>
    /// </remarks>
    public abstract class NxtSensor: NxtPollable
    {
        /// <summary>
        /// <para>The port the sensor is attached to.</para>
        /// </summary>
        protected NxtSensorPort sensorPort;

        private NxtSensorMode sensorMode;

        /// <summary>
        /// <para>The type of the sensor.</para>
        /// </summary>
        protected NxtSensorType sensorType;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="sensorType">The type of the sensor</param>
        /// <param name="sensorMode">The sensors mode</param>
        public NxtSensor(NxtSensorType sensorType, NxtSensorMode sensorMode)
        {
            this.sensorMode = sensorMode;
            this.sensorType = sensorType;
        }

        /// <summary>
        /// <para>Initialize the sensor.</para>
        /// </summary>
        internal virtual void InitSensor()
        {
            TraceUtil.MethodEnter();

            if (Brick.IsConnected)
                Brick.CommLink.SetInputMode(sensorPort, sensorType, sensorMode);

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>The port on the NXT brick that the sensor is attached to.</para>
        /// </summary>
        internal NxtSensorPort Port
        {
            // get { return sensorPort; }
            set { sensorPort = value; }
        }

        #region NxtPollable overrides

        /// <summary>
        /// <para>Poll the sensor</para>
        /// </summary>
        public override void Poll()
        {
            TraceUtil.MethodEnter();

            base.Poll();

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}
