using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace ParallelPort.LED.Controller {
    class PortControl {
        [DllImport("Inpout32.dll")]
        public static extern short Inp32(int address);
        [DllImport("inpout32.dll", EntryPoint = "Out32")]
        public static extern void Output(int adress, int value); // decimal

        public static List<string> GetParallelPorts() {
            List<string> ports = new List<string>();
            var parallelPort = new ManagementObjectSearcher("Select * From Win32_ParallelPort");
            //Dump(parallelPort.Get());
            foreach (var rec in parallelPort.Get()) {
                var wql = "Select * From Win32_PnPAllocatedResource";
                var pnp = new ManagementObjectSearcher(wql);

                var searchTerm = rec.Properties["PNPDeviceId"].Value.ToString();
                // compensate for escaping
                searchTerm = searchTerm.Replace(@"\", @"\\");

                foreach (var pnpRec in pnp.Get()) {
                    var objRef = pnpRec.Properties["dependent"].Value.ToString();
                    var antref = pnpRec.Properties["antecedent"].Value.ToString();

                    if (objRef.Contains(searchTerm)) {
                        var wqlPort = "Select * From Win32_PortResource";
                        var port = new ManagementObjectSearcher(wqlPort);
                        foreach (var portRec in port.Get()) {
                            if (portRec.ToString() == antref) {
                                int startAddress = Convert.ToInt32(portRec.Properties["StartingAddress"].Value);
                                int endAddress = Convert.ToInt32(portRec.Properties["EndingAddress"].Value);
                                string range = "0x"+Convert.ToString(startAddress, 16).PadLeft(2, '0').ToUpper() +"h -0x"+Convert.ToString(startAddress, 16).PadLeft(2, '0').ToUpper()+"h";
                                string portDetails = string.Format("{0} ({1})",
                                    rec.Properties["Name"].Value,
                                    range);
                                ports.Add(portDetails);

                            }
                        }
                    }
                }
            }
            return ports;
        }
    }
}
