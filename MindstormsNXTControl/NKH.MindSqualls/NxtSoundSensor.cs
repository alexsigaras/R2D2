namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the Sound sensor.</para>
    /// </summary>
    public class NxtSoundSensor: NxtPassiveSensor
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public NxtSoundSensor()
            : base(NxtSensorType.SOUND_DB, NxtSensorMode.PCTFULLSCALEMODE)
        {
        }

        #region Sensor readings.

        /// <summary>
        /// <para>Indicating if the sensor should return results as dB or as dBA (adjusted for the human ear).</para>
        /// </summary>
        public bool dBA
        {
            get { return (sensorType == NxtSensorType.SOUND_DBA); }
            set
            {
                sensorType = (value) ? NxtSensorType.SOUND_DBA : NxtSensorType.SOUND_DB;
                InitSensor();
            }
        }

        /// <summary>
        /// <para>The measured sound level</para>
        /// </summary>
        /// <seealso cref="dBA"/>
        public byte? SoundLevel
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

        private byte compareSoundLevel = 100;

        /// <summary>
        /// <para>Trigger sound level.</para>
        /// </summary>
        /// <seealso cref="OnAboveSoundLevel"/>
        /// <seealso cref="OnBelowSoundLevel"/>
        public byte CompareSoundLevel
        {
            get { return compareSoundLevel; }
            set
            {
                compareSoundLevel = value;
                if (compareSoundLevel > 100) compareSoundLevel = 100;
            }
        }

        /// <summary>
        /// <para>This event is fired whenever the measured sound level passes above the compare-level.</para>
        /// </summary>
        /// <seealso cref="CompareSoundLevel"/>
        /// <seealso cref="OnBelowSoundLevel"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnAboveSoundLevel;

        /// <summary>
        /// <para>This event is fired whenever the measured sound level passes below the compare-level.</para>
        /// </summary>
        /// <seealso cref="CompareSoundLevel"/>
        /// <seealso cref="OnAboveSoundLevel"/>
        /// <seealso cref="Poll"/>
        public event NxtSensorEvent OnBelowSoundLevel;

        private object pollDataLock = new object();

        /// <summary>
        /// <para>Polls the sensor, and fires the NXT-G like events if appropriate.</para>
        /// </summary>
        /// <seealso cref="CompareSoundLevel"/>
        /// <seealso cref="OnAboveSoundLevel"/>
        /// <seealso cref="OnBelowSoundLevel"/>
        public override void Poll()
        {
            TraceUtil.MethodEnter();

            if (Brick.IsConnected)
            {
                byte? oldSoundLevel, newSoundLevel;
                lock (pollDataLock)
                {
                    oldSoundLevel = this.SoundLevel;
                    base.Poll();
                    newSoundLevel = this.SoundLevel;
                }

                if (oldSoundLevel != null && newSoundLevel != null)
                {
                    if (OnAboveSoundLevel != null &&
                        oldSoundLevel < CompareSoundLevel && CompareSoundLevel <= newSoundLevel)
                        OnAboveSoundLevel(this);
                    else if (OnBelowSoundLevel != null &&
                        oldSoundLevel >= CompareSoundLevel && CompareSoundLevel > newSoundLevel)
                        OnBelowSoundLevel(this);
                }
            }

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}