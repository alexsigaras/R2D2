using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NKH.MindSqualls;
using System.Windows;

namespace MindstormsNXTControl
{
    public static class StaticVariables
    {
        public static NxtBrick Brick;
        public static NxtMotorSync MotorPair;
        public static string ComPort;

        #region NXTStuff

        public static void ConnectNXT()
        {
            try
            {
                byte comPort = byte.Parse(ComPort);
                Brick = new NxtBrick(comPort);

                Brick.MotorB = new NxtMotor();
                Brick.MotorC = new NxtMotor();
                MotorPair = new NxtMotorSync(Brick.MotorB, Brick.MotorC);

                Brick.Connect();
                MessageBox.Show("Connected: " + Brick.Name);
                Brick.PlaySoundfile("Atention.rso");
            }
            catch
            {
                DisconnectNXT();
            }
        }

        public static void DisconnectNXT()
        {
            Idle();

            if (Brick != null && Brick.IsConnected)
                Brick.Disconnect();

            Brick = null;
            MotorPair = null;

            MessageBox.Show("Disconnected");
        }

        public static void Idle()
        {
            if (Brick != null && Brick.IsConnected)
                MotorPair.Idle();
        }

        public static bool CheckConnection()
        {
            bool ConnectionCheck = false;
            if (Brick != null && Brick.IsConnected)
            {
                ConnectionCheck = true;
            }

            return ConnectionCheck;
        }

        #endregion NXTStuff


        #region NXTNavigation

        public static void MoveForward()
        {
            if (CheckConnection())
            {
                Brick.PlaySoundfile("forward.rso");
                MotorPair.Run(-100, 0, 0);
            }
        }

        public static void MoveBack()
        {
            if (CheckConnection())
            {
                MotorPair.Run(100, 0, 0);
            }
        }

        public static void MoveLeft()
        {
            if (CheckConnection())
            {
                Brick.PlaySoundfile("left.rso");
                Brick.MotorB.Run(-100, 0);
                Brick.MotorC.Run(100, 0);

            }
        }

        public static void MoveRight()
        {
            if (CheckConnection())
            {
                Brick.PlaySoundfile("right.rso");
                Brick.MotorB.Run(100, 0);
                Brick.MotorC.Run(-100, 0);
            }
        }

        public static void Stop()
        {
            Idle();
        }

        #endregion NXTNavigation
    }
}
