namespace Skyline.DataMiner.FlowEngineering.Protocol.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Core.DataMinerSystem.Protocol;
	using Skyline.DataMiner.FlowEngineering.Protocol;
	using Skyline.DataMiner.Scripting;

	using FlowProvisioning = Skyline.DataMiner.CommunityLibrary.FlowProvisioning;

	public class TxFlows : Flows<TxFlow>
    {
        public TxFlows(FlowEngineeringManager manager) : base(manager)
        {
        }

        public TxFlow GetOrAdd(string instance)
        {
            if (string.IsNullOrWhiteSpace(instance))
            {
                throw new ArgumentException($"'{nameof(instance)}' cannot be null or whitespace.", nameof(instance));
            }

            if (!TryGetValue(instance, out var flow))
            {
                flow = new TxFlow(instance);
                Add(flow);
            }

            return flow;
        }

        public TxFlow RegisterFlowEngineeringFlow(FlowProvisioning.Info.FlowInfo flowInfo, bool ignoreDestinationPort = false)
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
                ? string.Join("/", ip.SourceIp, $"{ip.DestinationIp}:{ip.DestinationPort}", flowInfo.Interface)
                : string.Join("/", ip.SourceIp, ip.DestinationIp, flowInfo.Interface);

            if (!TryGetValue(instance, out var flow))
            {
                flow = new TxFlow(instance)
                {
                    Source = ip.SourceIp,
                    Destination = ip.DestinationIp,
                    DestinationPort = !ignoreDestinationPort ? Convert.ToInt32(ip.DestinationPort) : -1,
                    TransportType = FlowTransportType.IP,
                };
                Add(flow);
            }

            flow.FlowOwner = FlowOwner.FlowEngineering;
            flow.LinkedFlow = Convert.ToString(flowInfo.SourceFlowId);
            flow.OutgoingInterface = flowInfo.Interface;
            flow.ExpectedBitrate = ip.BitRate;

            return flow;
        }

        public TxFlow UnregisterFlowEngineeringFlow(FlowProvisioning.Info.FlowInfo flowInfo, bool ignoreDestinationPort = false)
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
                ? string.Join("/", ip.SourceIp, $"{ip.DestinationIp}:{ip.DestinationPort}", flowInfo.Interface)
                : string.Join("/", ip.SourceIp, ip.DestinationIp, flowInfo.Interface);

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
                .GetTable(Parameter.Fleoutgoingflowstable.tablePid)
                .GetColumns(
                    new uint[]
                    {
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstableinstance,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstabledestination,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstabledestinationport,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstablesource,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstableoutgoinginterface,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstabletransporttype,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstabletxbitrate,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstableexpectedtxbitrate,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstablelabel,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstablelinkedflow,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstableflowowner,
                        Parameter.Fleoutgoingflowstable.Idx.fleoutgoingflowstablepresent,
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
                    flow = new TxFlow(row.Idx);
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
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstabledestination, Values.Select(x => x.Destination)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstabledestinationport, Values.Select(x => (object)x.DestinationPort)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstablesource, Values.Select(x => x.Source)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstableoutgoinginterface, Values.Select(x => x.Interface)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstablefkincoming, Values.Select(x => x.ForeignKeyIncoming)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstabletransporttype, Values.Select(x => (object)x.TransportType)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstablelabel, Values.Select(x => x.Label ?? string.Empty)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstablelinkedflow, Values.Select(x => x.LinkedFlow ?? string.Empty)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstableflowowner, Values.Select(x => (object)x.FlowOwner)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstablepresent, Values.Select(x => (object)(x.IsPresent ? 1 : 0))),
            };

            if (includeStatistics)
            {
                columns.AddRange(GetStatisticsColumns());
            }

            protocol.SetColumns(
                Parameter.Fleoutgoingflowstable.tablePid,
                deleteOldRows: true,
                Values.Select(x => x.Instance).ToArray(),
                columns.ToArray());
        }

        public void UpdateStatistics(SLProtocol protocol)
        {
            protocol.SetColumns(
                Parameter.Fleoutgoingflowstable.tablePid,
                deleteOldRows: true,
                Values.Select(x => x.Instance).ToArray(),
                GetStatisticsColumns());
        }

        private (int Pid, IEnumerable<object> Data)[] GetStatisticsColumns()
        {
            return new[]
            {
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstabletxbitrate, Values.Select(x => (object)x.Bitrate)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstableexpectedtxbitrate, Values.Select(x => (object)x.ExpectedBitrate)),
                (Parameter.Fleoutgoingflowstable.Pid.fleoutgoingflowstableexpectedtxbitratestate, Values.Select(x => (object)x.ExpectedBitrateStatus)),
            };
        }
    }
}