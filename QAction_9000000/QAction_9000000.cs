using System;
using System.Collections.Generic;

using QAction_9000000;

using Skyline.DataMiner.ConnectorAPI.FlowEngineering;
using Skyline.DataMiner.ConnectorAPI.FlowEngineering.Info;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	private static readonly List<Type> _knownTypes;
	private static readonly IDictionary<Type, Type> _messageToExecutorMapping = new Dictionary<Type, Type>
	{
		{typeof(FlowInfoMessage), typeof(FlowProvisioningExecutor)},
	};

	static QAction()
	{
		_knownTypes = new List<Type>();
		_knownTypes.AddRange(FlowInfoMessage.KnownTypes);
	}

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			var raw = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter()));
			protocol.Log($"QA{protocol.QActionID}|Run|Raw message: {raw}", LogType.DebugInfo, LogLevel.NoLogging);

			var receivedCall = InterAppCallFactory.CreateFromRawAndAcceptMessage(raw, _knownTypes);

			foreach (var message in receivedCall.Messages)
			{
				message.TryExecute(protocol, protocol, _messageToExecutorMapping, out var returnMessage);

				if (returnMessage != null)
				{
					returnMessage.Send(protocol.SLNet.RawConnection, message.ReturnAddress.AgentId, message.ReturnAddress.ElementId, message.ReturnAddress.ParameterId, _knownTypes);
				}
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
