namespace Skyline.DataMiner.FlowEngineering.Protocol.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Core.DataMinerSystem.Protocol;
	using Skyline.DataMiner.FlowEngineering.Protocol;
	using Skyline.DataMiner.Scripting;

	using FlowProvisioning = Skyline.DataMiner.CommunityLibrary.FlowProvisioning;

	public class RxFlows : Flows<RxFlow>
    {
        public RxFlows(FlowEngineeringManager manager) : base(manager)
        {
        }

        public RxFlow GetOrAdd(string instance)
        {
            if (string.IsNullOrWhiteSpace(instance))
            {
                throw new ArgumentException($"'{nameof(instance)}' cannot be null or whitespace.", nameof(instance));
            }

            if (!TryGetValue(instance, out var flow))
            {
                flow = new RxFlow(instance);
                Add(flow);
            }

            return flow;
        }

        public RxFlow RegisterFlowEngineeringFlow(FlowProvisioning.Info.FlowInfo flowInfo, bool ignoreDestinationPort = false)
        {
            if (flowInfo == null)
            {
                throw new ArgumentNullException(nameof(flowInfo));
            }

            var ip = flowInfo.FlowTransportIp;
            if (ip == null)
            {
                throw new NotSupportedException("Only IP flows are supported");
            }

            var instance = !ignoreDestinationPort
                ? string.Join("/", ip.SourceIp, $"{ip.DestinationIp}:{ip.DestinationPort}")
                : string.Join("/", ip.SourceIp, ip.DestinationIp);

            if (!TryGetValue(instance, out var flow))
            {
                flow = new RxFlow(instance)
                {
                    Source = ip.SourceIp,
                    Destination = ip.DestinationIp,
                    DestinationPort = !ignoreDestinationPort ? Convert.ToInt32(ip.DestinationPort) : -1,
                    TransportType = FlowTransportType.IP,
                };
                Add(flow);
            }

            flow.FlowOwner = FlowOwner.FlowEngineering;
            flow.LinkedFlow = Convert.ToString(flowInfo.FlowId);
            flow.IncomingInterface = flowInfo.Interface;
            flow.ExpectedBitrate = ip.BitRate;

            return flow;
        }

        public RxFlow UnregisterFlowEngineeringFlow(FlowProvisioning.Info.FlowInfo flowInfo, bool ignoreDestinationPort = false)
        {
            if (flowInfo == null)
            {
                throw new ArgumentNullException(nameof(flowInfo));
            }

            var ip = flowInfo.FlowTransportIp;
            if (ip == null)
            {
                throw new NotSupportedException("Only IP flows are supported");
            }

            var instance = !ignoreDestinationPort
                ? string.Join("/", ip.SourceIp, $"{ip.DestinationIp}:{ip.DestinationPort}")
                : string.Join("/", ip.SourceIp, ip.DestinationIp);

            if (!TryGetValue(instance, out var flow))
            {
                return null;
            }

            if (flow.IsPresent)
            {
                flow.FlowOwner = FlowOwner.LocalSystem;
                flow.LinkedFlow = string.Empty;
                flow.ExpectedBitrate = -1;
            }
            else
            {
                Remove(instance);
            }

			return flow;
		}

		public void LoadTable(SLProtocol protocol)
        {
            var table = protocol.GetLocalElement()
                .GetTable(Parameter.Fleincomingflowstable.tablePid)
                .GetColumns(
                    new uint[]
                    {
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstableinstance,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstabledestination,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstabledestinationport,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstablesource,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstableincominginterface,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstabletransporttype,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstablerxbitrate,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstableexpectedrxbitrate,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstablelabel,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstablelinkedflow,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstableflowowner,
                        Parameter.Fleincomingflowstable.Idx.fleincomingflowstablepresent,
                    },
                    (string idx, string dest, int destPort, string source, string intf, int type, double bitrate, double expectedBitrate, string label, string linked, int owner, int present) =>
                    {
                        return new
                        {
                            Idx = idx,
                            Destination = dest,
                            DestinationPort = destPort,
                            Source = source,
                            Interface = intf,
                            TransportType = (FlowTransportType)type,
                            Bitrate = bitrate,
                            ExpectedBitrate = expectedBitrate,
                            Label = label,
                            LinkedFlow = linked,
                            FlowOwner = (FlowOwner)owner,
                            IsPresent = Convert.ToBoolean(present),
                        };
                    }
                );

            foreach (var row in table)
            {
                if (!TryGetValue(row.Idx, out var flow))
                {
                    flow = new RxFlow(row.Idx);
                    Add(flow);
                }

                flow.Destination = row.Destination;
                flow.DestinationPort = row.DestinationPort;
                flow.Source = row.Source;
                flow.Interface = row.Interface;
                flow.TransportType = row.TransportType;
                flow.Bitrate = row.Bitrate;
                flow.ExpectedBitrate = row.ExpectedBitrate;
                flow.Label = row.Label;
                flow.LinkedFlow = row.LinkedFlow;
                flow.FlowOwner = row.FlowOwner;
                flow.IsPresent = row.IsPresent;
            }
        }

        public void UpdateTable(SLProtocol protocol, bool includeStatistics = true)
        {
            var columns = new List<(int Pid, IEnumerable<object> Data)>
            {
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstabledestination, Values.Select(x => x.Destination)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstabledestinationport, Values.Select(x => (object)x.DestinationPort)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablesource, Values.Select(x => x.Source)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstableincominginterface, Values.Select(x => x.Interface)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablefkoutgoing, Values.Select(x => x.ForeignKeyOutgoing)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstabletransporttype, Values.Select(x => (object)x.TransportType)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablelabel, Values.Select(x => x.Label ?? string.Empty)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablelinkedflow, Values.Select(x => x.LinkedFlow ?? string.Empty)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstableflowowner, Values.Select(x => (object)x.FlowOwner)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablepresent, Values.Select(x => (object)(x.IsPresent ? 1 : 0))),
            };

            if (includeStatistics)
            {
                columns.AddRange(GetStatisticsColumns());
            }

            protocol.SetColumns(
                Parameter.Fleincomingflowstable.tablePid,
                deleteOldRows: true,
                Values.Select(x => x.Instance).ToArray(),
                columns.ToArray());
        }

        public void UpdateStatistics(SLProtocol protocol)
        {
            protocol.SetColumns(
                Parameter.Fleincomingflowstable.tablePid,
                deleteOldRows: true,
                Values.Select(x => x.Instance).ToArray(),
                GetStatisticsColumns());
        }

        private (int Pid, IEnumerable<object> Data)[] GetStatisticsColumns()
        {
            return new[]
            {
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstablerxbitrate, Values.Select(x => (object)x.Bitrate)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstableexpectedrxbitrate, Values.Select(x => (object)x.ExpectedBitrate)),
                (Parameter.Fleincomingflowstable.Pid.fleincomingflowstableexpectedrxbitratestate, Values.Select(x => (object)x.ExpectedBitrateStatus)),
            };
        }
    }
}