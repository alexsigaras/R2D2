using System.Collections.Generic;

// The NxtBrick-class is divided over three files:
//
// NxtBrick1.cs - contains all the connectivity-related code.
// NxtBrick2.cs - contains all the general-purpose methods and properties of a NXT brick.
// NxtBrick3.cs - contains all the sensor- and motor-related code.

namespace NKH.MindSqualls
{
    public partial class NxtBrick
    {
        #region Motors.

        private Dictionary<NxtMotorPort, NxtMotor> motorArr = new Dictionary<NxtMotorPort, NxtMotor>();

        /// <summary>
        /// <para>The motor attached to port A.</para>
        /// </summary>
        public NxtMotor MotorA
        {
            get { return motorArr[NxtMotorPort.PortA]; }
            set { AttachMotor(value, NxtMotorPort.PortA); }
        }

        /// <summary>
        /// <para>The motor attached to port B.</para>
        /// </summary>
        public NxtMotor MotorB
        {
            get { return motorArr[NxtMotorPort.PortB]; }
            set { AttachMotor(value, NxtMotorPort.PortB); }
        }

        /// <summary>
        /// <para>The motor attached to port C.</para>
        /// </summary>
        public NxtMotor MotorC
        {
            get { return motorArr[NxtMotorPort.PortC]; }
            set { AttachMotor(value, NxtMotorPort.PortC); }
        }

        /// <summary>
        /// <para>Attaches a motor to the NXT brick.</para>
        /// </summary>
        /// <param name="motor">The motor</param>
        /// <param name="port">The port to attach the motor to</param>
        private void AttachMotor(NxtMotor motor, NxtMotorPort port)
        {
            if (motor != null)
            {
                motorArr[port] = motor;
                motor.Brick = this;
                motor.Port = port;
            }
        }

        #endregion

        #region Sensors.

        private Dictionary<NxtSensorPort, NxtSensor> sensorArr = new Dictionary<NxtSensorPort, NxtSensor>();

        /// <summary>
        /// <para>The sensor attached to port 1.</para>
        /// </summary>
        public NxtSensor Sensor1
        {
            get { return sensorArr[NxtSensorPort.Port1]; }
            set { AttachSensor(value, NxtSensorPort.Port1); }
        }

        /// <summary>
        /// <para>The sensor attached to port 2.</para>
        /// </summary>
        public NxtSensor Sensor2
        {
            get { return sensorArr[NxtSensorPort.Port2]; }
            set { AttachSensor(value, NxtSensorPort.Port2); }
        }

        /// <summary>
        /// <para>The sensor attached to port 3.</para>
        /// </summary>
        public NxtSensor Sensor3
        {
            get { return sensorArr[NxtSensorPort.Port3]; }
            set { AttachSensor(value, NxtSensorPort.Port3); }
        }

        /// <summary>
        /// <para>The sensor attached to port 4.</para>
        /// </summary>
        public NxtSensor Sensor4
        {
            get { return sensorArr[NxtSensorPort.Port4]; }
            set { AttachSensor(value, NxtSensorPort.Port4); }
        }

        /// <summary>
        /// <para>Attaches a sensot to the NXT brick.</para>
        /// </summary>
        /// <param name="sensor">The sensor</param>
        /// <param name="port">The port to attach the sensor to</param>
        private void AttachSensor(NxtSensor sensor, NxtSensorPort port)
        {
            if (sensor != null)
            {
                sensor.Brick = this;
                sensor.Port = port;
                sensorArr[port] = sensor;
            }
        }

        private void InitSensors()
        {
            TraceUtil.MethodEnter();

            foreach (NxtSensor sensor in sensorArr.Values)
                sensor.InitSensor();

            TraceUtil.MethodExit(null);
        }

        #endregion

        #region Auto-polling.

        private void EnableAutoPoll()
        {
            TraceUtil.MethodEnter();

            foreach (NxtMotor motor in motorArr.Values)
                motor.EnableAutoPoll();

            foreach (NxtSensor sensor in sensorArr.Values)
                sensor.EnableAutoPoll();

            TraceUtil.MethodExit(null);
        }

        private void DisableAutoPoll()
        {
            TraceUtil.MethodEnter();

            foreach (NxtSensor sensor in sensorArr.Values)
                sensor.DisableAutoPoll();

            foreach (NxtMotor motor in motorArr.Values)
                motor.DisableAutoPoll();

            TraceUtil.MethodExit(null);
        }

        #endregion
    }
}
