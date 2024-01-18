using System;

using Skyline.DataMiner.FlowEngineering.Protocol;
using Skyline.DataMiner.FlowEngineering.Protocol.Model;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			var trigger = protocol.GetTriggerParameter();
			var key = protocol.RowKey();
			var value = protocol.GetParameter(trigger);

			var flowEngineering = FlowEngineeringManager.GetInstance(protocol);

			switch (trigger)
			{
				case Parameter.Write.fleincomingflowstableexpectedrxbitrate:
					UpdateExpectedBitrate(protocol, flowEngineering, flowEngineering.IncomingFlows, key, (double)value);
					break;
				case Parameter.Fleincomingflowstable.Pid.Write.fleincomingflowstabledelete:
					DeleteFlow(protocol, flowEngineering, flowEngineering.IncomingFlows, key);
					break;
				case Parameter.Write.fleoutgoingflowstableexpectedtxbitrate:
					UpdateExpectedBitrate(protocol, flowEngineering, flowEngineering.OutgoingFlows, key, (double)value);
					break;
				case Parameter.Fleoutgoingflowstable.Pid.Write.fleoutgoingflowstabledelete:
					DeleteFlow(protocol, flowEngineering, flowEngineering.OutgoingFlows, key);
					break;
				case Parameter.Fleprovisionedflowstable.Pid.Write.fleprovisionedflowstabledelete:
					DeleteProvisionedFlow(protocol, flowEngineering, key);
					break;
				default:
					throw new InvalidOperationException($"Invalid trigger: {trigger}");
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void UpdateExpectedBitrate<T>(SLProtocolExt protocol, FlowEngineeringManager flowEngineering, Flows<T> flows, string key, double expectedBitrate)
		where T : Flow
	{
		if (!flows.TryGetValue(key, out var flow))
		{
			return;
		}

		flow.ExpectedBitrate = expectedBitrate;

		flows.UpdateStatistics(protocol);
		flowEngineering.Interfaces.UpdateStatistics(protocol);
	}

	private static void DeleteFlow<T>(SLProtocolExt protocol, FlowEngineeringManager flowEngineering, Flows<T> flows, string key)
		where T : Flow
	{
		if (!flows.TryRemove(key, out _))
		{
			return;
		}

		flows.UpdateTable(protocol);
		flowEngineering.Interfaces.UpdateStatistics(protocol);
	}

	private static void DeleteProvisionedFlow(SLProtocolExt protocol, FlowEngineeringManager flowEngineering, string key)
	{
		flowEngineering.UnregisterProvisionedFlow(protocol, Guid.Parse(key));
	}
}
