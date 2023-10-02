using System;

using Skyline.DataMiner.FlowEngineering.Protocol;
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

			var expectedBitrate = (double)value;

			switch (trigger)
			{
				case Parameter.Write.fleincomingflowstableexpectedrxbitrate:
					UpdateExpectedRxBitrate(protocol, key, expectedBitrate);
					break;
				case Parameter.Write.fleoutgoingflowstableexpectedtxbitrate:
					UpdateExpectedTxBitrate(protocol, key, expectedBitrate);
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

	private static void UpdateExpectedRxBitrate(SLProtocolExt protocol, string key, double expectedBitrate)
	{
		var flowEngineering = FlowEngineeringManager.GetInstance(protocol);

		if (flowEngineering.IncomingFlows.TryGetValue(key, out var flow))
		{
			flow.ExpectedBitrate = expectedBitrate;
		}

		flowEngineering.IncomingFlows.UpdateStatistics(protocol);
		flowEngineering.Interfaces.UpdateStatistics(protocol);
	}

	private static void UpdateExpectedTxBitrate(SLProtocolExt protocol, string key, double expectedBitrate)
	{
		var flowEngineering = FlowEngineeringManager.GetInstance(protocol);

		if (flowEngineering.OutgoingFlows.TryGetValue(key, out var flow))
		{
			flow.ExpectedBitrate = expectedBitrate;
		}

		flowEngineering.OutgoingFlows.UpdateStatistics(protocol);
		flowEngineering.Interfaces.UpdateStatistics(protocol);
	}
}
