namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the Light sensor.</para>
    /// </summary>
    public class NxtLightSensor: NxtPassiveSensor
    {        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtLightSensor()
            : base(NxtSensorType.LIGHT_INACTIVE, NxtSensorMode.PCTFULLSCALEMODE)
        {
        }

        #region Sensor readings.

        /// <summary>
        /// <para>Indicates if the sensor should supply its own light.</para>
        /// </summary>
        public bool GenerateLight
        {
            get { return (sensorType == NxtSensorType.LIGHT_ACTIVE); }
            set
            {
                sensorType = (value) ? NxtSensorType.LIGHT_ACTIVE : NxtSensorType.LIGHT_INACTIVE;
                InitSensor();
            }
        }

        /// <summary>
        /// <para>The intensity of the measured light.</para>
        /// </summary>
        public byte? Intensity
        {
            get
            {
                if (pollData != null)
                    return (byte) pollData.Value.scaledValue;
                else
                    return null;
            }
        }

        #endregion

        #region NXT-G like events & NxtPollable overrides.

        private byte compareIntensity = 100;

        /// <summary>
        /// <para>Trigger intensity.</para>
        /// </summary>
        /// <seealso cref="OnAboveIntensity"/>
        /// <seealso cref="OnBelowIntensity"/>
        /// <seealso cref="Poll"/>
        public byte CompareIntensity
        {
            get { return compareIntensity; }
            set
            {
                compareIntensity = value;
                if (compareIntensity > 100) compareIntensity = 100;
            }
        }

        /// <summary>
        /// <para>This event is fired whenever the measured light intensity passes above the compare-intensity.</para>
        /// </summary>
        /// <seealso cref="CompareIntensity"/>
        /// <seealso cref="OnBelowIntensity"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnAboveIntensity;

        /// <summary>
        /// <para>This event is fired whenever the measured light intensity passes below the compare-intensity.</para>
        /// </summary>
        /// <seealso cref="CompareIntensity"/>
        /// <seealso cref="OnAboveIntensity"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnBelowIntensity;

        private object pollDataLock = new object();

        /// <summary>
        /// <para>Polls the sensor, and fires the NXT-G like events if appropriate.</para>
        /// </summary>
        /// <seealso cref="CompareIntensity"/>
        /// <seealso cref="OnAboveIntensity"/>
        /// <seealso cref="OnBelowIntensity"/>
        public override void Poll()
        {
            TraceUtil.MethodEnter();

            if (Brick.IsConnected)
            {
                byte? oldIntensity, newIntensity;
                lock (pollDataLock)
                {
                    oldIntensity = Intensity;
                    base.Poll();
                    newIntensity = Intensity;
                }

                if (oldIntensity != null && newIntensity != null)
                {
                    if (OnAboveIntensity != null &&
                        oldIntensity < CompareIntensity && CompareIntensity <= newIntensity)
                        OnAboveIntensity(this);
                    else if (OnBelowIntensity != null &&
                        oldIntensity >= CompareIntensity && CompareIntensity > newIntensity)
                        OnBelowIntensity(this);
                }
            }

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}
