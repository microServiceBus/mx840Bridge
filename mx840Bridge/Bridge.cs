using Hbm.Api.Common;
using Hbm.Api.Common.Entities.ConnectionInfos;
using Hbm.Api.Common.Entities.Problems;
using Hbm.Api.Common.Entities.Signals;
using Hbm.Api.QuantumX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mx840Bridge
{
    public class Bridge
    {
        QuantumXDevice _deviceToConnect;
        DaqEnvironment _daqEnvironment = null;
        
        [STAThread]
        public async Task<object> Invoke(dynamic request)
        {
            try
            {
               // Console.WriteLine("Invoke...");
                var method = (string)request.operation;

                switch (method)
                {
                    case "open":
                        var host = (string)request.host;
                        var port = (int)request.port;
                        Console.WriteLine("HOST:" + host + "Port" + port);
                        return Open(host, port);
                    case "read":
                        return Read();
                    case "close":
                        return Close();
                    case "test":
                        return Close();
                    default:
                        throw new Exception("Operation not supported");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        public async Task<dynamic> Open(string host, int port, bool isScan = true)
        {
            Console.WriteLine("Opening....");
            port = port == 0 ? 5001 : port;
            
            List<Problem> problemList = new List<Problem>();
            
            //_daqEnvironment = new DaqEnvironment();
            try
            {
                _daqEnvironment = DaqEnvironment.GetInstance();
               // _daqEnvironment.Scan();
                Console.WriteLine(isScan ? "Scan is enabled" : "Static ip is enabled");
                if (isScan)
                    _deviceToConnect = new QuantumXDevice();
                else
                    _deviceToConnect = new QuantumXDevice(host);
                 //host 
                _daqEnvironment.Connect(_deviceToConnect, out problemList);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not open device: Trying to scan device....B");
                Console.WriteLine("------- Scanning for ip -------");
            }
            try
            {
                //scan for mx840 dynamic ip 
                if (isScan)
                {
                    Console.WriteLine("Scaning ip stared");

                    //remove all reasources to this and reconnect using scan option
                    var dataScan = _daqEnvironment.Scan();
                  
                    foreach (var item in dataScan)
                    {
                        var getIpByScan = (item.ConnectionInfo as EthernetConnectionInfo).IpAddress;
                        host = getIpByScan;
                        Console.WriteLine("Found scanned HOST: " + getIpByScan);
                        (_deviceToConnect.ConnectionInfo as EthernetConnectionInfo).IpAddress = host;

                    }
                    _daqEnvironment.Disconnect(_deviceToConnect);
                    _deviceToConnect = new QuantumXDevice(host);
                    problemList.Clear();
                    _daqEnvironment.Connect(_deviceToConnect, out problemList);
                    
                }
                   
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not scan the device : " + ex);
                throw ex;
            }


            if (problemList.Count > 0)
            {
                foreach (var property in problemList)
                {
                    Console.WriteLine(string.Format("Property: {0}, Value: {1}", property.PropertyName, property.Message));
                }

                //                throw new Exception("Unable to connect");
            }


            Console.WriteLine("Reading properties...........");
            foreach (var prop in _deviceToConnect.GetType().GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(_deviceToConnect, null));
            }

            return null;
        }
        public async Task<dynamic> Read()
        {
           // Console.WriteLine("Reading...");
            try
            {
                _deviceToConnect.ReadSingleMeasurementValueOfAllSignals();
                List<Signal> signals = _deviceToConnect.GetAllSignals();
                Dictionary<string, double> dictionary = new Dictionary<string, double>();
                foreach (var signal in signals)
                {
                    dictionary.Add(signal.GetUniqueID(), signal.GetSingleMeasurementValue().Value);
                }
                var dt = DateTime.UtcNow;
                var result = new
                {
                    dt = dt,
                    sensor1_name = "AnalogIn_Connector1.Signal1",
                    sensor1_value = dictionary["AnalogIn_Connector1.Signal1"],
                    sensor1_dt = dt,
                    sensor2_name = "AnalogIn_Connector1.Signal2",
                    sensor2_value = dictionary["AnalogIn_Connector1.Signal2"],
                    sensor2_dt = dt,
                    sensor3_name = "AnalogIn_Connector2.Signal1",
                    sensor3_value = dictionary["AnalogIn_Connector2.Signal1"],
                    sensor3_dt = dt,
                    sensor4_name = "AnalogIn_Connector2.Signal2",
                    sensor4_value = dictionary["AnalogIn_Connector2.Signal2"],
                    sensor4_dt = dt,
                    sensor5_name = "AnalogIn_Connector3.Signal1",
                    sensor5_value = dictionary["AnalogIn_Connector3.Signal1"],
                    sensor5_dt = dt,
                    sensor6_name = "AnalogIn_Connector3.Signal2",
                    sensor6_value = dictionary["AnalogIn_Connector3.Signal2"],
                    sensor6_dt = dt,
                    sensor7_name = "AnalogIn_Connector4.Signal1",
                    sensor7_value = dictionary["AnalogIn_Connector4.Signal1"],
                    sensor7_dt = dt,
                    sensor8_name = "AnalogIn_Connector4.Signal2",
                    sensor8_value = dictionary["AnalogIn_Connector4.Signal2"],
                    sensor8_dt = dt
                };
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not read message: " + ex.Message);
                throw ex;
            }
            
        }
        public async Task<dynamic> Close()
        {
            Console.WriteLine("Closing...");
            _daqEnvironment.Disconnect(_deviceToConnect);
            return null;
        }
        
    }

}
