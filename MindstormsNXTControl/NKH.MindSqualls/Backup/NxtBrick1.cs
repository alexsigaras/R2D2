using System.Threading;

// The NxtBrick-class is divided over three files:
//
// NxtBrick1.cs - contains all the connectivity-related code.
// NxtBrick2.cs - contains all the general-purpose methods and properties of a NXT brick.
// NxtBrick3.cs - contains all the sensor- and motor-related code.

// TODO: Add support for an USB connection as an alternative to Bluetooth.

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>Class representing the NXT brick.</para>
    /// </summary>
    public partial class NxtBrick
    {
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="serialPortNo">The COM port used by the Bluetooth link</param>
        public NxtBrick(byte serialPortNo) : this()
        {
            commLink = new NxtBluetoothConnection(serialPortNo);
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="serialPortName">The COM port used by the Bluetooth link</param>
        public NxtBrick(string serialPortName) : this()
        {
            commLink = new NxtBluetoothConnection(serialPortName);
        }

        /// <summary>
        /// <para>Private constructor.</para>
        /// </summary>
        private NxtBrick()
        {
            keepAliveTimer = new Timer(keepAliveTimer_Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        #region Communication link to the NXT.

        // If, and when, the API is expanded to provide support for
        // USB, this part of the code will have to be partially rewritten.

        /// <summary>
        /// <para>The bluetooth link.</para>
        /// </summary>
        /// <remarks>
        /// <para>Notice that commLink is an instance of NxtCommunicationProtocol, not NxtBluetoothConnection as you might expect. This is to ensure that commLink could just as well be a NxtUsbConnection if and when that class is created in the future. The future  NxtUsbConnection-class will also inherit NxtCommunicationProtocol as NxtBluetoothConnection does.</para>
        /// </remarks>
        private NxtCommunicationProtocol commLink = null;

        /// <summary>
        /// <para>The communication link to the NXT brick.</para>
        /// </summary>
        /// <remarks>
        /// <para>The current version of the API only supports bluetooth links. Possibly a future version will support USB as well.</para>
        /// </remarks>
        /// <exception cref="NxtConnectionException">Throws a NxtConnectionException if not connected.</exception>
        /// <seealso cref="Connect"/>
        /// <seealso cref="Disconnect"/>
        /// <seealso cref="IsConnected"/>
        public NxtCommunicationProtocol CommLink
        {
            get
            {
                TraceUtil.MethodEnter();

                //if (!IsConnected)
                //    throw new NxtConnectionException("The brick is not connected.");

                TraceUtil.MethodExit("return commLink");
                return commLink;
            }
        }

        /// <summary>
        /// <para>Connect to the NXT brick.</para>
        /// </summary>
        /// <seealso cref="CommLink"/>
        /// <seealso cref="Disconnect"/>
        /// <seealso cref="IsConnected"/>
        public void Connect()
        {
            TraceUtil.MethodEnter();

            // Ignore if already connected.
            if (!IsConnected)
            {
                TraceUtil.Note("Not connected");

                // Connect.
                commLink.Connect();

                // Start keep-alive heartbeat.
                StartKeepAliveTimer();
            }
            else
                TraceUtil.Note("Already connected");

            InitSensors();

            EnableAutoPoll();

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>Disconnect from the NXT brick.</para>
        /// </summary>
        /// <seealso cref="CommLink"/>
        /// <seealso cref="Connect"/>
        /// <seealso cref="IsConnected"/>
        public void Disconnect()
        {
            TraceUtil.MethodEnter();

            DisableAutoPoll();

            // Stop keep-alive heartbeat.
            StopKeepAliveTimer();

            // Disconnect, if connected.
            if (IsConnected) commLink.Disconnect();

            TraceUtil.MethodExit(null);
        }

        /// <summary>
        /// <para>Indicates if the NXT brick is connected.</para>
        /// </summary>
        /// <seealso cref="CommLink"/>
        /// <seealso cref="Connect"/>
        /// <seealso cref="Disconnect"/>
        public bool IsConnected
        {
            get
            {
                TraceUtil.MethodVisit();

                return commLink.IsConnected;
            }
        }

        #endregion

        #region Keep-alive heartbeat (timer).

        private Timer keepAliveTimer = null;

        private void keepAliveTimer_Callback(object state)
        {
            TraceUtil.MethodEnter();

            if (IsConnected)
            {
                TraceUtil.Note("Call CommLink.KeepAlive()");
                CommLink.KeepAlive();
            }
            else
            {
                TraceUtil.Note("Call StopKeepAliveTimer()");
                StopKeepAliveTimer();
            }

            TraceUtil.MethodExit(null);
        }

        private void StartKeepAliveTimer()
        {
            TraceUtil.MethodEnter();

            // Query current sleep-time from the NXT.
            TraceUtil.Note("Call commLink.KeepAlive()");
            uint? keepAlive = commLink.KeepAlive();

            // Doesn't seem to wotk - for some reason the System.Threading.Timer
            // doesn't seem to function for timeintervals much above 1 minute...

            // Calculate half the sleep-time.
            //uint keepAlivePeriod = (keepAlive.HasValue) ? keepAlive.Value : 60 * 1000;
            //keepAlivePeriod /= 2;

            uint keepAlivePeriod = 60 * 1000;

            TraceUtil.Note("keepAlivePeriod = " + keepAlivePeriod.ToString());

            // Start the keep-alive timer.
            keepAliveTimer.Change(keepAlivePeriod, keepAlivePeriod);

            TraceUtil.MethodExit(null);
        }

        private void StopKeepAliveTimer()
        {
            TraceUtil.MethodEnter();

            keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}
