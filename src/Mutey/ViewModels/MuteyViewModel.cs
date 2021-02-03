using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mutey.Input;
using Mutey.Output;

namespace Mutey.ViewModels
{
    public class MuteyViewModel
    {
        private readonly List<IConferencingAppRegistration> appRegistrations = new();

        private readonly IMuteHardwareManager hardwareManager;

        public void RegisterApp(IConferencingAppRegistration registration)
        {
            appRegistrations.Add(registration);
        }

        public void RegisterHardware(IMuteHardwareDetector detector)
        {
            hardwareManager.RegisterHardwareDetector(detector);
        }

        public MuteyViewModel(IMuteHardwareManager hardwareManager)
        {
            this.hardwareManager = hardwareManager;
        }

        /// <summary>
        /// Removes all active connections and checks all available processes and hardware.
        /// </summary>
        public void FullRefresh()
        {
            hardwareManager.ChangeDevice(null);
            if(hardwareManager.AvailableDevices.FirstOrDefault() is { } hardware)
                hardwareManager.ChangeDevice(hardware);

            foreach (IConferenceConnection connection in connections)
            {
                connection.Dispose();
            }

            connections.Clear();

            foreach (IConferenceConnection? connection in Process.GetProcesses()
                .Select(CheckProcess)
                .Where(c => c != null))
            {
                AddConnection(connection!);
            }
        }

        private readonly List<IConferenceConnection> connections = new();
        public IEnumerable<IConferenceConnection> ActiveConnections => connections;
        
        private readonly List<ICall> calls = new();
        public IEnumerable<ICall> ActiveCalls => calls;
    

        private void AttachToActiveHardware(IMuteHardware hardware)
        {
            
        }

        private void AddConnection(IConferenceConnection connection)
        {
            connections.Add(connection);
            connection.Closed += (_, _) => connections.Remove(connection);

            if (connection.CanDetectNewCalls)
                throw new NotSupportedException();
            
            AddActiveCall(connection.DefaultCall);
        }

        private void AddActiveCall(ICall call)
        {
            calls.Add(call);
            call.Ended += (_, _) =>
            {
                calls.Remove(call);

                if (ReferenceEquals(PrimaryCall, call))
                    PrimaryCall = null;
            };

            PrimaryCall = call;
        }
        
        public ICall? PrimaryCall { get; private set; }

        private IConferenceConnection? CheckProcess(Process process)
            => appRegistrations
                .Where(appRegistration => appRegistration.IsMatch(process))
                .Select(appRegistration => appRegistration.Connect(process))
                .FirstOrDefault();
    }
}