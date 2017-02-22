namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the Touch sensor.</para>
    /// </summary>
    public class NxtTouchSensor: NxtPassiveSensor
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtTouchSensor()
            : base(NxtSensorType.SWITCH, NxtSensorMode.BOOLEANMODE)
        {
        }

        #region Sensor readings

        /// <summary>
        /// <para>Indicating if the sensor-button is pressed or not.</para>
        /// </summary>
        public bool? IsPressed
        {
            get
            {
                if (pollData != null)
                    return (pollData.Value.scaledValue == 1);
                else
                    return null;
            }
        }

        #endregion

        #region NXT-G like events & NxtPollable overrides

        /// <summary>
        /// <para>This event is fired when the touch sensor is pressed.</para>
        /// </summary>
        /// <seealso cref="OnReleased"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnPressed;

        /// <summary>
        /// <para>This event is fired when the touch sensor is released.</para>
        /// </summary>
        /// <seealso cref="OnPressed"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnReleased;

        private object pollDataLock = new object();

        /// <summary>
        /// <para>Polls the sensor, and fires the NXT-G like events if appropriate.</para>
        /// </summary>
        /// <seealso cref="OnPressed"/>
        /// <seealso cref="OnReleased"/>
        public override void Poll()
        {
            TraceUtil.MethodEnter();

            if (Brick.IsConnected)
            {
                bool? oldIsPressed, newIsPressed;
                lock (pollDataLock)
                {
                    oldIsPressed = this.IsPressed;
                    base.Poll();
                    newIsPressed = this.IsPressed;
                }

                if (oldIsPressed != null && newIsPressed != null)
                {
                    if (OnPressed != null &&
                        oldIsPressed.Value == false && newIsPressed.Value == true)
                        OnPressed(this);
                    else if (OnReleased != null &&
                        oldIsPressed.Value == true && newIsPressed.Value == false)
                        OnReleased(this);
                }
            }

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}