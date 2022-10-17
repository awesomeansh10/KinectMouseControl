using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using WindowsInput;

namespace WindowsFormsApp1
{
    public partial class Mouse_control : Form
    {
        double PreviousX = 0f;
        double PreviousY = 0f;

        double sens = 2500f;
        double min = 5f;
        double MovementX;
        double MovementY;
        KinectSensor kinectsensor = null;
        BodyFrameReader bodyFrameReader = null;
        Body[] bodies = null;

        public Mouse_control()
        {
            InitializeComponent();
            initialiseKinect();
            
        }
        public void initialiseKinect()
        {
            kinectsensor = KinectSensor.GetDefault();
            if(kinectsensor != null)
            {
                //turn on kinect
                kinectsensor.Open();
            }
            bodyFrameReader = kinectsensor.BodyFrameSource.OpenReader();

            if(bodyFrameReader != null)
            {
                bodyFrameReader.FrameArrived += Reader_FrameArrived;
            }
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            using(BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if(bodyFrame != null)
                {
                    if(bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }
                }
                bodyFrame.GetAndRefreshBodyData(bodies);
                dataReceived = true;
            }
            if (dataReceived)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                        Joint RightHand = joints[JointType.HandRight];

                        //System.Diagnostics.Trace.Write(RightHand.Position.X);
                        //System.Diagnostics.Trace.Write(RightHand.Position.Y);
                        //System.Diagnostics.Trace.Write(RightHand.Position.Z);
                        //System.Diagnostics.Trace.Write("\n");

                        InputSimulator inputsim = new InputSimulator();

                        MovementX = RightHand.Position.X - PreviousX;
                        MovementY = RightHand.Position.Y - PreviousY;

                        double newsens = RightHand.Position.Z * sens;
                        MovementX *= sens;
                        MovementY *= sens;

                        MovementY = -MovementY;
                        if(Math.Abs(MovementX) > min || Math.Abs(MovementY) > min){
                            inputsim.Mouse.MoveMouseBy((int)MovementX, (int)MovementY);
                            System.Diagnostics.Trace.WriteLine(MovementX);
                            //System.Diagnostics.Trace.Write("moving", ToString(MovementX));
                        }

                        PreviousX = RightHand.Position.X;
                        PreviousY = RightHand.Position.Y;
                        
                        if(body.HandRightState == HandState.Closed)
                        {
                            inputsim.Mouse.LeftButtonDown();
                            clickorno.Text = "Click";
                            clickorno.BackColor = Color.Green;
                            
                        }
                        else
                        {
                            inputsim.Mouse.LeftButtonUp();
                            clickorno.Text = "No Click";
                            clickorno.BackColor = Color.Red;

                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void clickorno_Click(object sender, EventArgs e)
        {

        }
    }
}
