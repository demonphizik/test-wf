using NLog;
using System;
using System.Activities;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyFirstWorkflow
{
    public class PortListenerActvity : CodeActivity
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private AutoResetEvent _exitSignal = new AutoResetEvent(true);

        [RequiredArgument]
        public InArgument<int> Port { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Start(Port.Get(context));
            }
            catch (SocketException exp)
            {
                _logger.Error("SocketException: {0}", exp);
                Stop();
            }
            _exitSignal.WaitOne();
        }

        public void Stop()
        {
            _exitSignal.Set();
        }

        private void Start(int port)
        {
            _exitSignal.Reset();

            var IPv4Addresses = Dns.GetHostEntry(Dns.GetHostName())
              .AddressList.Where(al => al.AddressFamily == AddressFamily.InterNetwork)
              .ToList();

            foreach (IPAddress ip in IPv4Addresses)
            {
                Sniff(ip, port);
            }
        }

        private void Sniff(IPAddress ip, int port, int startDataPos = 40)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

            socket.Bind(new IPEndPoint(ip, 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            socket.IOControl(IOControlCode.ReceiveAll, new byte[4] { 1, 0, 0, 0 }, null);

            byte[] buffer = new byte[65548];
            //socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);

            //data recieving and processing
            void OnReceive(IAsyncResult ar)
            {
                var count = socket.EndReceive(ar);
                var protocol = ToProtocolString(buffer[9]);
                var fromIP = new IPAddress(BitConverter.ToUInt32(buffer, 12));
                var toIP = new IPAddress(BitConverter.ToUInt32(buffer, 16));
                var fromPort = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 20));
                var toPort = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 22));
                var totalLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 2));

                if (toPort == port || fromPort == port)
                {
                    StringBuilder strBulder = new StringBuilder();
                    strBulder.AppendFormat("{0}\t{1}:{2}\t===>\t{3}:{4}", protocol, fromIP, fromPort, toIP, toPort);
                    if (totalLength > startDataPos)
                    {
                        var strBytes = buffer.Skip(startDataPos).Take(totalLength - startDataPos).Select(b => (byte)(b < 32 ? 32 : b)).ToArray();
                        var str = Encoding.UTF8.GetString(strBytes);
                        strBulder.AppendLine(str);
                        _logger.Info(strBulder.ToString());
                    }
                }
                //clean out our buffer
                Array.Clear(buffer, 0, buffer.Length);
                //continue receiving
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }

            //begin listening to the socket
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        private string ToProtocolString(byte b)
        {
            switch (b)
            {
                case 1: return "ICMP";
                case 2: return "IGMP";
                case 6: return "TCP";
                case 17: return "UDP";
                case 41: return "ENCAP";
                case 89: return "OSPF";
                case 132: return "SCTP";
                default: return "#" + b.ToString();
            }
        }

    }
}
