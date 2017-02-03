using Hbm.Api.Common;
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
                Console.WriteLine("Invoke...");
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
        
        public async Task<dynamic> Open(string host, int port)
        {
            Console.WriteLine("Opening....");
            port = port == 0 ? 5001 : port;
            _deviceToConnect = new QuantumXDevice(host);
            List<Problem> problemList = new List<Problem>();
            
            //_daqEnvironment = new DaqEnvironment();
            try
            {
                _daqEnvironment = DaqEnvironment.GetInstance();
                _daqEnvironment.Connect(_deviceToConnect, out problemList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not open device: " + ex.Message);
                throw ex;
            }
            
            if (problemList.Count>0)
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
            Console.WriteLine("Reading...");
            try
            {
                _deviceToConnect.ReadSingleMeasurementValueOfAllSignals();
                List<Signal> signals = _deviceToConnect.GetAllSignals();
                Dictionary<string, double> dictionary = new Dictionary<string, double>();
                foreach (var signal in signals)
                {
                    dictionary.Add(signal.GetUniqueID(), signal.GetSingleMeasurementValue().Value);
                }

                var result = new
                {
                    dt = DateTime.UtcNow,
                    sensor1_name = "AnalogIn_Connector1.Signal1",
                    sensor1_value = dictionary["AnalogIn_Connector1.Signal1"],
                    sensor2_name = "AnalogIn_Connector1.Signal2",
                    sensor2_value = dictionary["AnalogIn_Connector1.Signal2"],
                    sensor3_name = "AnalogIn_Connector2.Signal1",
                    sensor3_value = dictionary["AnalogIn_Connector2.Signal1"],
                    sensor4_name = "AnalogIn_Connector2.Signal2",
                    sensor4_value = dictionary["AnalogIn_Connector2.Signal2"],
                    sensor5_name = "AnalogIn_Connector3.Signal1",
                    sensor5_value = dictionary["AnalogIn_Connector3.Signal1"],
                    sensor6_name = "AnalogIn_Connector3.Signal2",
                    sensor6_value = dictionary["AnalogIn_Connector3.Signal2"],
                    sensor7_name = "AnalogIn_Connector4.Signal1",
                    sensor7_value = dictionary["AnalogIn_Connector4.Signal1"],
                    sensor8_name = "AnalogIn_Connector4.Signal2",
                    sensor8_value = dictionary["AnalogIn_Connector4.Signal2"]

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
